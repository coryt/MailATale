MAT.Gift = {
    Start: function (serverModel) {
        MAT.KnockoutBindings.Init();
        var model = MAT.Gift.GiftPurchaseModel(serverModel);
        MAT.Gift.WireEvents(model);
        ko.applyBindings(model);
    }
};

MAT.Gift.WireEvents = function (model) {
    $("#signupform").delegate("[id$='Province']", "change", model.updateOrderSummary);
    $("#signupform").delegate("#bookBox-basic", "change", model.updateOrderSummary);
    $("#signupform").delegate("#bookBox-plus", "change", model.updateOrderSummary);
    $("#bookBox-basic").change(function () {
        $("#bookBox-plus").val(0);
    });
    $("#bookBox-plus").change(function () {
        $("#bookBox-basic").val(0);
    });
};

MAT.Gift.GiftPurchaseModel = function (serverModel) {
    var model = {
        genderList: serverModel.genders,
        provinceList: serverModel.provinces,
        order: new MAT.Gift.OrderModel(serverModel)
    };
    
    model.updateOrderSummary = function () {
        if (!model.order.Lines()[0].recipient.province()) return;
        if (model.order.Lines()[0].giftLength() === "0") return;

        MAT.ASync.calculateGiftOrder(ko.mapping.toJSON(model.order.Lines()[0]), function (result) {
            if (result.status === "Success") {
                model.order.shippingFee(result.shipping);
                model.order.tax(result.tax);
            }
        });
    };

    return model;
};

MAT.Gift.OrderLineModel = function () {
    var model = {
        recipient: new MAT.ViewModels.GiftRecipient(),
        subscription: ko.observable(),
        price: ko.observable(0),
        giftLength: ko.observable(),
        bookBoxLength: ko.observable(0),
        bookBoxPlusLength: ko.observable(0),
        area: "Province"
    };

    model.bookBoxLength.subscribe(function (newValue) {
        model.subscription("basic");
        model.price(newValue * 19.95);
        model.giftLength(newValue);
    });
    
    model.bookBoxPlusLength.subscribe(function (newValue) {
        model.subscription("plus");
        model.price(newValue * 39.95);
        model.giftLength(newValue);
    });
    
    model.displayPrice = ko.computed(function () {
        return MAT.Utility.formatCurrency(model.price());
    });
    model.name = ko.computed(function () {
        return model.giftLength() + (model.subscription() === 'plus' ? " Month Book Box Plus" : " Month Book Box");
    });
    return model;
};

MAT.Gift.OrderModel = function (serverModel) {
    var model = {
        Lines: ko.observableArray([new MAT.Gift.OrderLineModel()]),
        email: ko.observable(serverModel.Email).extend({ required: true, email: true }),
        fullName: ko.observable().extend({ required: true }),
        billingAddress: new MAT.ViewModels.Address(),
        payment: new MAT.ViewModels.Payment(serverModel.PaymentInfo),
        tax: ko.observable(0),
        shippingFee: ko.observable(0),
        personalMessage: ko.observable(""),
        sendGiftNoticeOn: ko.observable("")
    };
    model.billingAddress.addressType("Billing");
    
    model.subTotal = ko.computed(function () {
        var total = 0.00;
        $.each(model.Lines(), function () { total += this.price(); });
        return total;
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

    return model;
};
