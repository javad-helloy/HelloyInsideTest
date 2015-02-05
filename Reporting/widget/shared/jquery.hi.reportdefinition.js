(function ($) {
    $.widget('hi.reportselector', {
        widgetEventPrefix: 'report',
        options: {
            clientId: null,
            startDate: null,
            endDate: null
        },

        _create: function () {
            var that = this;
            $(document).bind('requestReportInfo', function (event, eventData) {
                var optionsToSet = {
                    clientId: that.options.clientId,
                    startDate: that.options.startDate,
                    endDate: that.options.endDate
                }
                eventData._setOptions(optionsToSet);
            });

            $(document).bind('requestClientId', function (event, eventData) {
                eventData._setOption('clientId', that.options.clientId);
            });
            
            $(document).bind('requestStartDate', function (event, eventData) {
                eventData._setOption('startDate', that.options.startDate);
            });
            
            $(document).bind('requestEndDate', function (event, eventData) {
                eventData._setOption('endDate', that.options.endD);
            });
            
            $(document).trigger('clientId', that.options.clientId);
            $(document).trigger('startDate', that.options.startDate);
            $(document).trigger('endDate', that.options.endDate);
        },

        _setOption: function (name, value) {
            $.Widget.prototype._setOption(key, value);

            if (name == 'clientId') {
                $(document).trigger('clientId', this.options.clientId);
            }

            if (name == 'startDate') {
                $(document).trigger('startDate', this.options.startDate);
            }

            if (name == 'endDate') {
                $(document).trigger('endDate', this.options.endDate);
            }
        },

        destroy: function () {
            $.Widget.prototype.destroy.apply(this, arguments);
        }
    });

}(jQuery));