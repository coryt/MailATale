notifier = {
    success: function (message) {
        $(".notifier").addClass("alert alert-success");;
        $(".notifier").html(message).fadeIn();
        //this.goAway();
    },
    error: function (message) {
        $(".notifier").addClass("alert alert-error");
        $(".notifier").html(message).fadeIn();
        //this.goAway(10000);
    },
    warn: function (message) {
        $(".notifier").addClass("alert");
        $(".notifier").html(message).fadeIn();
        //this.goAway(5000);
    },
    notify: function (response) {
        if (response.success === true) {
            notifier.success(response.message);
        } else {
            notifier.error(response.message);
        }
    },
    goAway: function (delay) {
        var ms = delay || 3000;
        $(".notifier").delay(ms).fadeOut().removeClass("alert alert-error alert-success");
    }
}