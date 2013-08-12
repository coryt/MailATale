MAT.KnockoutBindings = {
    Init: function () {
        ko.validation.rules.pattern.message = 'Invalid.';
        ko.validation.configure({
            registerExtenders: true,
            messagesOnModified: true,
            insertMessages: true,
            parseInputAttributes: true,
            decorateElement: true,
            errorElementClass: 'error',
            messageTemplate: null
        });
        
        ko.bindingHandlers.dateText = {
            init: function(element, value) {
                var val = value();
                $(element).text(MAT.Utility.parseDate(val));
            }
        };
        
        ko.bindingHandlers.fadeVisible = {
            init: function (element, valueAccessor) {
                // Initially set the element to be instantly visible/hidden depending on the value
                var value = valueAccessor();
                $(element).toggle(ko.utils.unwrapObservable(value)); // Use "unwrapObservable" so we can handle values that may or may not be observable
            },
            update: function (element, valueAccessor) {
                // Whenever the value subsequently changes, slowly fade the element in or out
                var value = valueAccessor();
                ko.utils.unwrapObservable(value) ? $(element).slideDown(2000) : $(element).slideUp();
            }
        };
        
        ko.virtualElements.allowedBindings.fadeVisible = true;
    }
};