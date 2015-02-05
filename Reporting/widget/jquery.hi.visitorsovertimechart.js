(function ($) {
    $.widget('hi.visitorsovertimechart', $.hi.reportwidget, {
        widgetEventPrefix: 'hi',
        data: null,
        chart: null,
        options: {
            series: [],
            categories: [],
            startDate: null,
            screenSize: null,
            groupByFormat: "YYYY-MM-DD",
            aggregateType: "Daily",

        },

        create: function() {
            var that = this;
            var element = that.element;
            element.html("");

            element.append('<section><div style="text-align:center;">' +
                '<table class="table-centered">' +
                '<tr>' +
                '<td>' +
                '<span id="total-value" class="large-value">--</span>' +
                '</td>' +
                '</tr>' +
                '<tr>' +
                '<td id="visitors-subtitle">' +
                '<span>Besökare Månad</span>' +
                '</td>' +
                '</tr>' +
                '</table>' +
                '</div>' +
                '<div id="chart" style="margin-top:30px;">' +
                '</div></section>');

            $(document).bind("monthlyReport", function() {
                that.options.startDate = moment().add(-1, 'months').format("YYYY-MM-DD");
                that.options.groupByFormat = "YYYY-MM-DD";
                that.options.aggregateType = "Daily";
                $('#visitors-subtitle span').html("Besökare Månad");
                that.updateData();
            });

            $(document).bind("yearlyReport", function() {
                that.options.startDate = moment().add(-1, 'years').format("YYYY-MM-DD");
                that.options.groupByFormat = "MMMM YYYY";
                that.options.aggregateType = "Monthly";
                $('#visitors-subtitle span').html("Besökare År");
                that.updateData();
            });

            $(document).bind("allTimeReport", function() {
                that.options.startDate = moment("2011-01-01").format("YYYY-MM-DD");
                that.options.groupByFormat = "MMMM YYYY";
                that.options.aggregateType = "Monthly";
                $('#visitors-subtitle span').html("Besökare Totalt");
                that.updateData();
            });

            $(document).bind('screenSizeChange', function (event, screenSize) {
                that.options.screenSize = screenSize;
                that.redrawChart();
            });

            $(document).trigger("requestScreenSize", function(screenSize) {
                that.options.screenSize = screenSize;
                that.redrawChart();
            });

            that.options.startDate = moment().add(-1, 'months').format("YYYY-MM-DD");
        },

        update: function() {
            var that = this;
            that.updateData();
        },

        redrawChart: function () {
            var that = this;
            var tickInterval;
            if (that.options.screenSize == "small" && that.chart) {
                that.options.chartStyle = "Mobile";
                tickInterval = Math.ceil(that.options.series.length / 4);
                that.chart.options.chart.height = 150;
                that.chart.xAxis[0].options.tickInterval = tickInterval;
                that.chart.redraw();
            } else if (that.chart) {
                that.options.chartStyle = null;
                tickInterval = Math.ceil(that.options.series.length / 10);
                that.chart.options.chart.height = 400;
                that.chart.xAxis[0].options.tickInterval = tickInterval;
                that.chart.redraw();
            }
        },

        updateData: function () {
            var that = this;
            that.element.find('#total-value').html('<i class="fa fa-spinner fa-spin"></i>');
            that.element.find('#chart').html("");
            var endDate = moment().format("YYYY-MM-DD");
            var url = '/api/client/' + this.options.clientId + '/source/visitor/?' + $.param({
                startDate: that.options.startDate,
                endDate: endDate,
                aggregateType: that.options.aggregateType
            });

            $.getJSON(url, function (data) {

                var filteredData = that.removeLeadingZero(data);
                that.options.categories = _.map(filteredData, function (val) {
                    var stringDate = moment(val.date).format(that.options.groupByFormat);
                    return  stringDate.charAt(0).toUpperCase() + stringDate.slice(1);
                });

                that.options.series = _.map(filteredData, function (val) {
                    return val.visitors;
                });

                filteredData.Total = _.reduce(that.options.series, function (memo, num) { return memo + num; }, 0);

                that.data = filteredData;
                $.proxy(that.createChart, that)();
            });
        },

        removeLeadingZero: function(array) {
            var hasVisitors = array[0].visitors > 0;
            
            while (!hasVisitors && array.length) {
                if (array[0].visitors == 0) {
                    array.splice(0, 1);
                } else {
                    hasVisitors = true;
                }
            }
            return array;
        },

        createChart: function () {
            var that = this;
            
            var totalValueElement = that.element.find('#total-value');
            var chartContainer = that.element.find('#chart');

            if (that.data.Total != null || that.data.Total != undefined) {
                totalValueElement.html(that.data.Total);
            } else {
                totalValueElement.html(0);
            }
            var tickInterval = Math.ceil(that.options.series.length / 10);
            var areaChartOptions = {
                title: null,
                chart: {
                    type: 'areaspline',
                    renderTo: chartContainer[0],
                },
                credits: {
                    enabled: false
                },
                legend: {
                    enabled: false
                },
                xAxis: {
                    labels: {
                        style: {
                            color: "#a6a6a6",
                        }
                    },
                    type: "datetime",
                    dateTimeLabelFormats: {
                        month: '%b %e, %Y'
                    },
                    tickInterval: tickInterval,
                    tickmarkPlacement: 'on',
                    gridLineColor: "#e6e6e6",
                    categories: that.options.categories
                },
                colors: [
                    '#3db762'
                ],
                yAxis: {
                    title: {
                        text: null
                    },
                    labels: {
                        style: {
                            color: "#a6a6a6",
                        }
                    },
                    gridLineColor: "#e6e6e6",
                    gridLineDashStyle: "Dash"
                },
                tooltip: {
                    crosshairs: true,
                    shared: true
                },
                plotOptions: {
                    areaspline: {
                        marker: {
                            radius: 3,
                            lineColor: '#3db762',
                            lineWidth: 2
                        },
                        fillOpacity: 0.1
                    }
                },

                series: [{
                    name: 'besökare',
                    marker: {
                        symbol: 'circle'
                    },
                    data: that.options.series

                }]
            };

            if (that.options.screenSize == "small") {
                areaChartOptions.chart.height = 150;
                areaChartOptions.xAxis.tickInterval = Math.ceil(that.options.series.length / 4);
            } else {
                areaChartOptions.chart.height = 400;
                areaChartOptions.xAxis.tickInterval = Math.ceil(that.options.series.length / 10);
            }

            that.chart = new Highcharts.Chart(areaChartOptions);

        },


        destroy: function () {
            $.Widget.prototype.destroy.apply(this, arguments);
        }
    });
}(jQuery));