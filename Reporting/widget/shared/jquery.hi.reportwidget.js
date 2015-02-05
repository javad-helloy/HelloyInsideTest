(function ($) {
    $.widget('hi.reportwidget', {
        widgetEventPrefix: 'hi',
        options: {
            clientId: null,
        },

        _create: function () {
            var that = this;

            $(document).bind('clientId', function (event, eventData) {
                that.options.clientId = eventData;
                that.update();
            });

            this.create();
        },

        _setOption: function (key, value) {
            var that = this;

            if (key == "clientId") {
                that.options.clientId = value;
                this.update();
            }

            this.setOption(key, value);
        },

        setOption: function (key, value) {
        },

        create: function () {
        },

        update: function () {
        },

        _init: function () {
            $(document).trigger('requestReportInfo', this);
        },

        destroy: function () {
            $.Widget.prototype.destroy.apply(this, arguments);
        }
    });
}(jQuery));
