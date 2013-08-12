window.MAT = {};

MAT.ASync = function () {

    var _calculateGiftOrder = function (data, callback) {
        $.ajax({
            type: "POST",
            url: "/api/signuphelper/calculategiftorder",
            dataType: 'json',
            contentType: "application/json",
            async: true,
            processData: false,
            data: data,
            success: callback
        });
    };

    var _lookupShippingRate = function (shippingInfo, callback) {
        $.post("/api/signuphelper/lookupshippingrate", shippingInfo, callback);
    };
    
    var _verifyReferralCode = function (data, callback) {
        var code = {
            referralCode: referralCode
        };
        $.post("/api/signuphelper/validatereferralcode", code, callback);
    };

    var _calculateSignUpOrder = function (data, callback) {
        $.ajax({
            type: "POST",
            url: "/api/signuphelper/calculatesignuporder",
            dataType: 'json',
            contentType: "application/json",
            async: true,
            processData: false,
            data: data,
            success: callback
        });
    };
   
    return {
        lookupShippingRate: _lookupShippingRate,
        calculateGiftOrder: _calculateGiftOrder,
        verifyReferralCode: _verifyReferralCode,
        calculateSignUpOrder: _calculateSignUpOrder
    };

}();