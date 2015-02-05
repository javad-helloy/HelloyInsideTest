(function($) {
    $.widget('hi.screenresize', {
        options: {
            screenSize: null,
        },

        _create: function () {

            var that = this;

            var setScreenSize = function () {
                if ($(window).width() < 992) {
                    if (that.options.screenSize != "small") {
                        $(document).trigger("screenSizeChange", "small");
                    }
                    that.options.screenSize = "small";
                }
                else {
                    if (that.options.screenSize != "large") {
                        $(document).trigger("screenSizeChange", "large");
                    }
                    that.options.screenSize = "large";
                }
                
            }

            var setScreenSizeThrottled = _.throttle(setScreenSize, 400);

            $(window).on('resize', setScreenSizeThrottled);

            setScreenSize();

            $(document).bind('requestScreenSize', function (event, callback) {
                callback(that.options.screenSize);
            });
        }
    });
}(jQuery));