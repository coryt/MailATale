MAT.ViewModels = {};

MAT.ViewModels.PreferenceAnswer = function (serverModel) {
    var model = {
        questionId: ko.observable(serverModel.QuestionId),
        answer: ko.observable().extend({ required: true })
    };
    MAT.Utility.assignValue(model.answer, serverModel.Answer);
    model.errors = ko.validation.group(model);
    return model;
};

MAT.ViewModels.Reader = function (serverModel) {
    var model = {
        relationship: ko.observable().extend({ required: true }),
        name: ko.observable().extend({ required: true }),
        dobDay: ko.observable().extend({ required: true }),
        dobMonth: ko.observable().extend({ required: true }),
        dobYear: ko.observable().extend({ required: true }),
        gender: ko.observable().extend({ required: true }),
        gradeLevel: ko.observable().extend({ required: true }),
        readingLevel: ko.observable().extend({ required: true }),
        preferenceAnswers: []
    };

    MAT.Utility.assignValue(model.name, serverModel.Name);
    MAT.Utility.assignValue(model.relationship, serverModel.Relationship);
    MAT.Utility.assignValue(model.dobDay, serverModel.DobDay);
    MAT.Utility.assignValue(model.dobMonth, serverModel.DobMonth);
    MAT.Utility.assignValue(model.dobYear, serverModel.DobYear);
    MAT.Utility.assignValue(model.gender, serverModel.Gender);
    MAT.Utility.assignValue(model.gradeLevel, serverModel.GradeLevel);
    MAT.Utility.assignValue(model.readingLevel, serverModel.ReadingLevel);
    
    var answers = serverModel.PreferenceAnswers ? serverModel.PreferenceAnswers : [];
    for (var i = 0; i < answers.length; i++) {
        var answerModel = new MAT.ViewModels.PreferenceAnswer(answers[i]);
        model.preferenceAnswers.push(answerModel);
    }

    model.dob = ko.computed(function () {
        return model.dobMonth() + "/" + model.dobDay() + "/" + model.dobYear();
    });

    model.answerGenderSalutation = ko.computed(function () {
        return model.gender() === "Male" ? " his" : " her";
    });

    model.storyGenderSalutation = ko.computed(function () {
        return model.gender() === "Male" ? "He " : "She ";
    });

    model.getGradeLevelText = function (item) {
        if (item.Value === "9") return item.Text.toLowerCase();
        if (item.Value >= 3 && item.Value <= 8) return 'in grade ' + item.Text;
        return 'in ' + item.Text;
    };

    model.getReadingLevelText = function (item) {
        if (item.Value === "2") { return item.Text; }
        var first = item.Text.split(' ')[0];
        return item.Text.replace(first, 'reading ' + first + model.answerGenderSalutation());
    };

    model.errors = ko.validation.group(model);

    model.continue = ko.computed(function () {
        var invalidAnswer = ko.utils.arrayFirst(model.preferenceAnswers, function(item) {
            return !item.isValid();
        });
        return model.isValid() && invalidAnswer === null;
    });
    
    return model;
};

MAT.ViewModels.Account = function (serverModel) {
    var model = {
        city: ko.observable().extend({ required: true }),
        email: ko.observable(serverModel.Email).extend({ required: true, email: true }),
        firstName: ko.observable().extend({ required: true }),
        lastName: ko.observable().extend({ required: true }),
        password: ko.observable().extend({ required: true, minLength: 8 }),
        phone: ko.observable().extend({ required: true, pattern: /^[2-9](\d{6}|\d{9})$/ }),
        postalCode: ko.observable().extend({ required: true, pattern: /^[ABCEGHJKLMNPRSTVXY|abceghjklmnprstvxy]{1}\d{1}[A-Z|a-z]{1} *\d{1}[A-Z|a-z]{1}\d{1}$/ }),
        province: ko.observable().extend({ required: true }),
        street1: ko.observable().extend({ required: true }),
        street2: ko.observable()
    };
    model.confirmEmail = ko.observable().extend({ equal: model.email });

    MAT.Utility.assignValue(model.city, serverModel.City);
    MAT.Utility.assignValue(model.email, serverModel.Email);
    MAT.Utility.assignValue(model.confirmEmail, serverModel.ConfirmEmail);
    MAT.Utility.assignValue(model.password, serverModel.Password);
    MAT.Utility.assignValue(model.firstName, serverModel.FirstName);
    MAT.Utility.assignValue(model.lastName, serverModel.LastName);
    MAT.Utility.assignValue(model.phone, serverModel.Phone);
    MAT.Utility.assignValue(model.postalCode, serverModel.PostalCode);
    MAT.Utility.assignValue(model.province, serverModel.Province);
    MAT.Utility.assignValue(model.street1, serverModel.Street1);
    MAT.Utility.assignValue(model.street2, serverModel.Street2);

    model.errors = ko.validation.group(model);

    model.continue = ko.computed(function () {
        return model.isValid();
    });
    
    return model;
};

MAT.ViewModels.GiftRecipient = function () {
    var model = {
        name: ko.observable().extend({ required: true }),
        email: ko.observable().extend({ required: true, email: true }),
        readerName: ko.observable().extend({ required: true }),
        gender: ko.observable().extend({ required: true }),
        province: ko.observable().extend({ required: true }),
    };

    model.errors = ko.validation.group(model);
    
    model.continue = ko.computed(function () {
        return model.isValid();
    });
    
    return model;
};

MAT.ViewModels.Payment = function (serverModel) {
    var model = {
        creditCardExpiryMonth: ko.observable().extend({ required: true }),
        creditCardExpiryYear: ko.observable().extend({ required: true }),
        creditCardName: ko.observable().extend({ required: true }),
        creditCardSalutation: ko.observable(),
        creditCardNumber: ko.observable().extend({ required: true, pattern: '^(?:4[0-9]{12}(?:[0-9]{3})?|5[1-5][0-9]{14}|6(?:011|5[0-9][0-9])[0-9]{12}|3[47][0-9]{13}|3(?:0[0-5]|[68][0-9])[0-9]{11}|(?:2131|1800|35\d{3})\d{11})$' }),
        creditCardSecurityCode: ko.observable().extend({ required: true, minLength: 3, maxLength: 4 }),
    };

    MAT.Utility.assignValue(model.creditCardExpiryMonth, serverModel.CreditCardExpiryMonth);
    MAT.Utility.assignValue(model.creditCardExpiryYear, serverModel.CreditCardExpiryYear);
    MAT.Utility.assignValue(model.creditCardName, serverModel.CreditCardName);
    MAT.Utility.assignValue(model.creditCardNumber, serverModel.CreditCardNumber);
    MAT.Utility.assignValue(model.creditCardSecurityCode, serverModel.CreditCardSecurityCode);

    model.creditCardType = ko.computed(function () {
        return MAT.Utility.checkCCType(model.creditCardNumber());
    });
    
    model.maskedCreditCardNumber = ko.computed(function () {
        return MAT.Utility.maskCreditCard(model.creditCardNumber());
    });
    
    model.creditCardExpiry = ko.computed(function () {
        return "Exp. " + model.creditCardExpiryMonth() + "/" + model.creditCardExpiryYear();
    });
    
    model.errors = ko.validation.group(model);
    
    model.continue = ko.computed(function () {
        return model.isValid();
    });
    
    return model;
};

MAT.ViewModels.Address = function () {
    var model = {
        city: ko.observable().extend({ required: true }),
        phone: ko.observable().extend({ required: true, pattern: /^[2-9](\d{6}|\d{9})$/ }),
        postalCode: ko.observable().extend({ required: true, pattern: /^[ABCEGHJKLMNPRSTVXY|abceghjklmnprstvxy]{1}\d{1}[A-Z|a-z]{1} *\d{1}[A-Z|a-z]{1}\d{1}$/ }),
        province: ko.observable().extend({ required: true }),
        street1: ko.observable().extend({ required: true }),
        street2: ko.observable(),
        addressType: ko.observable("Shipping")
    };

    model.errors = ko.validation.group(model);

    model.continue = ko.computed(function () {
        return model.isValid();
    });

    return model;
};