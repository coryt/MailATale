MAT.SignUp = {
    Start: function (serverModel) {
        MAT.KnockoutBindings.Init();
        var model = MAT.SignUp.SignUpModel(serverModel);
        MAT.SignUp.WireEvents(model);
        ko.applyBindings(model);
    }
};

MAT.SignUp.WireEvents = function (model) {
    $("#signupform").submit(model.formSubmitted);
    $("#signupform").delegate("#check-promotion", "click", model.applyRefferalCode);
    $("#signupform").delegate("input[id$='PostalCode']", "focusout", model.updateOrderSummary);
    $("#signupform").delegate("[id$='Province']", "change", model.updateOrderSummary);
    $('.calculate-shipping').live('click', function () {
        $(".modal", $(this).parent()).modal();
    });
    $("#signupform").delegate('.calculateShippingFee', 'click', model.calculateShipping);
};

MAT.SignUp.SignUpModel = function (serverModel) {
    var model = {
        numberOfReaders: ko.observable(0),
        order: new MAT.SignUp.OrderModel(serverModel)
    };

    MAT.Utility.assignValue(model.numberOfReaders, serverModel.Readers);
    if (serverModel.Readers > 0) {
        for (var i = 0; i < serverModel.Readers; i++) {
            model.order.addLineItem(serverModel.Lines[i]);

            var subscriptionButton = $(".btn-" + serverModel.Lines[i].Subscription, ".bookbox-" + i);
            var type = subscriptionButton.data("boxtype");
            $('img[class*="box-selected-"]', subscriptionButton.parents("section.subscription")).hide();
            model.order.lines()[i].subscription(type);
            model.order.lines()[i].price((type === 'plus') ? 39.95 : 19.95);
            $('.box-selected-' + type, subscriptionButton.parent()).show();
        }
    }
    
    model.numberOfReaders.subscribe(function (newValue) {
        if (newValue >= 1) {
            var diff = newValue - model.order.lines().length;
            if (diff > 0) {
                for (var i = 0; i < diff; i++) {
                    model.order.addLineItem(serverModel.Lines[newValue - 1]);
                }
            } else if (diff < 0) {
                for (diff; diff < 0; diff++) {
                    model.order.lines.pop();
                }
            }
        }
        else {
            model.order.lines.removeAll();
        }
    });
    
    model.showReader = function (index) {
        if (model.numberOfReaders < index) {
            return false;
        }
        return (index == 0 || (index >= 1 && model.order.lines()[index - 1].reader.continue()));
    };

    model.order.subTotal.subscribe(function () {
        model.updateOrderSummary();
    });

    model.order.lines.subscribe(function () {
        model.updateOrderSummary();
    });

    model.order.discount.subscribe(function () {
        model.updateOrderSummary();
    });

    //events
    model.formSubmitted = function () {
        if (!model.order.account.isValid()) {
            model.order.account.errors.showAllMessages();
            model.order.payment.errors.showAllMessages();
            $.each(model.order.lines(), function (index, item) {
                item.reader.errors.showAllMessages();
            });
            return false;
        }
        return true;
    };

    model.applyRefferalCode = function () {
        MAT.ASync.verifyReferralCode(model.order.referralName(), function (result) {
            if (result.status === "Error") {
                notifier.error(result.message);
            }

            if (result.status === "Success") {
                model.order.referralName(result.message);
                model.order.discount(result.discount);
            }
        });
    };

    model.updateOrderSummary = function () {
        if (model.order.lines().length === 0) return;
        if (!model.order.account.postalCode()) return;
        if (!model.order.account.province()) return;

        MAT.ASync.calculateSignUpOrder(ko.mapping.toJSON(model.order), function (result) {
            if (result.status === "Success") {
                model.order.shippingFee(result.shipping);
                model.order.tax(result.tax);
            }
        });
    };

    model.calculateShipping = function () {
        var modal = $(this).parent();
        var pcode = $('input.postalcode', modal).val();
        if (pcode.length < 6) {
            $('.shipping-result', modal).hide();
            $('.shipping-invalid-result', modal).show();
            $('.shipping-invalid-result', modal).text("Invalid Postal Code");
            return;
        }

        var shippingInfo = {
            postalCode: $('input.postalcode', modal).val(),
            area: "FSA"
        };

        MAT.ASync.lookupShippingRate(shippingInfo, function (result) {
            if (result.status === "Error") {
                $('.shipping-result', modal).hide();
                $('.shipping-invalid-result', modal).show();
                $('.shipping-invalid-result', modal).text(result.message);
                return;
            }

            $('.shipping-invalid-result', modal).hide();
            $('.shipping-result', modal).show();
            $('span.fee', modal).text(result.message);
        });
    };

    return model;
};

MAT.SignUp.OrderModel = function (serverModel) {
    var model = {
        lines: ko.observableArray([]),
        account: new MAT.ViewModels.Account(serverModel.Account),
        payment: new MAT.ViewModels.Payment(serverModel.Payment),
        tax: ko.observable(0),
        shippingFee: ko.observable(0),
        referralName: ko.observable(serverModel.ReferralName),
        discount: ko.observable(0),
        area: ko.observable("FSA")
    };
    
    model.subTotal = ko.computed(function () {
        var total = 0.00;
        $.each(model.lines(), function () { total += this.price(); });
        return total - model.discount();
    });
    model.displayDiscount = ko.computed(function () {
        return MAT.Utility.formatCurrency(model.discount() * -1);
    });
    model.grandTotal = ko.computed(function () {
        return (model.subTotal() + model.tax() + model.shippingFee());
    });
    model.displaySubTotal = ko.computed(function () {
        return MAT.Utility.formatCurrency(model.subTotal());
    });
    model.displayTax = ko.computed(function () {
        return MAT.Utility.formatCurrency(model.tax());
    });
    model.displayShipping = ko.computed(function () {
        return MAT.Utility.formatCurrency(model.shippingFee());
    });
    model.displayTotal = ko.computed(function () {
        return MAT.Utility.formatCurrency(model.grandTotal());
    });

    model.addLineItem = function (serverModel) {
        model.lines.push(new MAT.SignUp.OrderLineModel(serverModel));
    };
    
    return model;
};

MAT.SignUp.OrderLineModel = function (serverModel) {
    var model = {
        reader: new MAT.ViewModels.Reader(serverModel ? serverModel.Reader : {}),
        deliverySchedule: ko.observable("monthly"),
        subscription: ko.observable("basic"),
        price: ko.observable(19.95)
    };

    MAT.Utility.assignValue(model.deliverySchedule, serverModel ? serverModel.DeliverySchedule : "monthly");
    MAT.Utility.assignValue(model.subscription, serverModel ? serverModel.Subscription : "basic");
    
    model.boxName = ko.computed(function () {
        return model.reader.name();
    });
    
    model.selectSubscription = function (data, event) {
        var type = $(event.target).data("boxtype");
        $('img[class*="box-selected-"]', $(event.target).parents("section.subscription")).hide();
        model.subscription(type);
        model.price((type === 'plus') ? 39.95 : 19.95);
        $('.box-selected-' + type, $(event.target).parent()).show();
    };

    model.displayPrice = ko.computed(function () {
        return MAT.Utility.formatCurrency(model.price());
    });

    model.name = ko.computed(function () {
        return (model.boxName() ? model.boxName() : "Your child") + "'s" + (model.subscription() === 'plus' ? " Book Box Plus" : " Book Box");
    });
   
    return model;
};