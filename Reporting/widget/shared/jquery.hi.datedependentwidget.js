(function($) {
    $.widget('hi.datedependentwidget', $.hi.reportwidget, {
        options: {
            firstDay: null,
            lastDay: null,
        },

        create: function () {
            var that = this;

            $(document).bind('monthlyReport', function () {
                that.options.firstDay = moment().format('YYYY-MM-01');
                that.options.lastDay = moment(that.options.firstDay).add(1, 'months').add(-1, 'days').format('YYYY-MM-DD');
                $(document).trigger('reportName', moment(that.options.firstDay).format('MMMM'));
                that.update();
            });

            $(document).bind('yearlyReport', function () {
                that.options.firstDay = moment().format('YYYY-01-01');
                that.options.lastDay = moment(that.options.firstDay).add(1, 'years').add(-1, 'days').format('YYYY-MM-DD');
                $(document).trigger('reportName', moment(that.options.firstDay).format('YYYY'));
                that.update();
            });

            $(document).bind('alltimeReport', function () {
                that.options.firstDay = moment("2010-01-01").format('YYYY-MM-DD');
                that.options.lastDay = moment().format('YYYY-MM-DD');
                $(document).trigger('reportName', "totalt");
                that.update();
            });


            $(document).bind('nextMonth', function () {
                var newMonth = moment(that.options.firstDay).add(1, 'months');
                that.options.firstDay = newMonth.format('YYYY-MM-01');
                that.options.lastDay = moment(that.options.firstDay).add(1, 'months').add(-1, 'days').format('YYYY-MM-DD');
                that.update();
                

                if (newMonth.format('YYYY') != moment().format('YYYY')) {
                    $(document).trigger('reportName', newMonth.format('MMM YYYY'));
                } else {
                    $(document).trigger('reportName', newMonth.format('MMMM'));
                }

            });

            $(document).bind('prevMonth', function () {
                var newMonth = moment(that.options.firstDay).add(-1, 'months');
                that.options.firstDay = newMonth.format('YYYY-MM-01');
                that.options.lastDay = moment(that.options.firstDay).add(1, 'months').add(-1, 'days').format('YYYY-MM-DD');
                that.update();

                if (newMonth.format('YYYY') != moment().format('YYYY')) {
                    $(document).trigger('reportName', newMonth.format('MMM YYYY'));
                } else {
                    $(document).trigger('reportName', newMonth.format('MMMM'));
                }
            });

            $(document).bind('nextYear', function () {
                var newYear = moment(that.options.firstDay).add(1, 'years');
                that.options.firstDay = newYear.format('YYYY-01-01');
                that.options.lastDay = moment(that.options.firstDay).add(1, 'years').add(-1, 'days').format('YYYY-MM-DD');
                that.update();

                $(document).trigger('reportName', newYear.format('YYYY'));

            });

            $(document).bind('prevYear', function () {
                var newYear = moment(that.options.firstDay).add(-1, 'years');
                that.options.firstDay = newYear.format('YYYY-01-01');
                that.options.lastDay = moment(that.options.firstDay).add(1, 'years').add(-1, 'days').format('YYYY-MM-DD');
                that.update();

                $(document).trigger('reportName', newYear.format('YYYY'));
            });

            that.createReport();
        },

        createReport: function() {
        },

        updateData: function() {
            
        }
    });
}(jQuery));