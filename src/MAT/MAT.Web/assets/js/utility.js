MAT.Utility = function () {

    var _parseDate = function (jsonDate) {
        var re = /-?\d+/;
        var m = re.exec(jsonDate);
        var d = new Date(parseInt(m[0]));
        var curr_date = d.getDate();
        var curr_month = d.getMonth() + 1; //Months are zero based
        var curr_year = d.getFullYear();
        return curr_month + "-" + curr_date + "-" + curr_year + " " + d.getHours() + ":" + d.getMinutes();
    };

    var _replaceText = function (text, search, replace) {
        return text.replace(search, replace);
    };

    var _checkCCType = function (ccNumber) {
        if (!ccNumber) return "Unknown";
        ccNumber = ccNumber.replace(/\s/g, "");
        // Visa: length 16, prefix 4, dashes optional.
        var visaRE = /^4\d{3}-?\d{4}-?\d{4}-?\d{4}$/;
        // Mastercard: length 16, prefix 51-55, dashes optional.
        var mcRE = /^5[1-5]\d{2}-?\d{4}-?\d{4}-?\d{4}$/;

        if (visaRE.test(ccNumber)) return "Visa";
        if (mcRE.test(ccNumber)) return "Mastercard";
        return "Unknown";
    };

    var _validateCreditCard = function (ccNumber) {
        if (!ccNumber) return false;
        if (_checkCCType(ccNumber) == "Unknown")
            return false;

        // Remove all dashes for the checksum checks to eliminate negative numbers
        ccNumber = ccNumber.replace(/-/g, "");

        // Checksum ("Mod 10")
        // Add even digits in even length strings or odd digits in odd length strings.
        var checksum = 0;
        for (var i = (2 - (ccNumber.length % 2)) ; i <= ccNumber.length; i += 2) {
            checksum += parseInt(ccNumber.charAt(i - 1));
        }
        // Analyze odd digits in even length strings or even digits in odd length strings.
        for (var i = (ccNumber.length % 2) + 1; i < ccNumber.length; i += 2) {
            var digit = parseInt(ccNumber.charAt(i - 1)) * 2;
            if (digit < 10) { checksum += digit; } else { checksum += (digit - 9); }
        }
        if ((checksum % 10) == 0) return true; else return false;
    };

    var _maskCreditCard = function (ccNumber) {
        if (!ccNumber) return "XXXX-XXXX-XXXX-XXXX";
        return "XXXX-XXXX-XXXX-" + ccNumber.substring(ccNumber.length - 4, ccNumber.length);
    };

    var _formatCurrency = function formatCurrency(num) {
        num = num.toString().replace(/\$|\,/g, '');
        if (isNaN(num)) num = "0";
        var sign = (num == (num = Math.abs(num)));
        num = Math.floor(num * 100 + 0.50000000001);
        var cents = num % 100;
        num = Math.floor(num / 100).toString();
        if (cents < 10) cents = "0" + cents;
        for (var i = 0; i < Math.floor((num.length - (1 + i)) / 3); i++)
            num = num.substring(0, num.length - (4 * i + 3)) + ',' + num.substring(num.length - (4 * i + 3));
        return (((sign) ? '' : '-') + '$' + num + '.' + cents);
    };

    var _assignValue = function (property, value) {
        if (value !== null) {
            property(value);
        }
    }

    return {
        parseDate: _parseDate,
        replaceText: _replaceText,
        validateCreditCard: _validateCreditCard,
        checkCCType: _checkCCType,
        maskCreditCard: _maskCreditCard,
        formatCurrency: _formatCurrency,
        assignValue: _assignValue
    };

}();