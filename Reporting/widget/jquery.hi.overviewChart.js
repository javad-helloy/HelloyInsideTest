(function ($) {
    $.widget('hi.overviewChart', $.hi.reportwidget, {
        widgetEventPrefix: 'hi',
        data: null,
        options: {
            series: [],
            categories: []
        },

        create: function () {
            var that = this;
            var element = that.element;
            element.html("");

            element.append(' <header><h2 class="">Resultat totalt</h2></header>');
            element.append('<section class="widget"><div style="text-align:center;">'+
                            '<table class="table-centered">'+
                                '<tr>'+
                                    '<td>'+
                                        '<span id="total-value" class="large-value">--</span>'+
                                    '</td>'+
                                '</tr>'+
                                '<tr>'+
                                    '<td>'+
                                        '<span>Kundkontakter totalt</span>'+
                                    '</td>'+
                                '</tr>'+
                            '</table>'+
                        '</div>'+
                        '<div id="chart" style="height:205px;margin-top:30px;">'+
                        '</div></section>');
            //that.updateData();

        },

        update: function () {
            this.updateData();
        },

        updateData: function () {
            var that = this;
            var url = '/api/client/' + this.options.clientId + '/lead/aggregate/' ;

            $.getJSON(url, function (data) {

                var sortedData = data.Aggregates.sort(function (a, b) {
                    return a.Year - b.Year || a.Month - b.Month;
                });
                $.each(sortedData, function (index, item) {
                    var count = item.ContactCount;

                    if (index > 0) {
                        count += that.options.series[index - 1];
                    }
                    that.options.series.push(count);
                    that.options.categories.push(moment.months(parseInt(item.Month) - 1).substring(0, 3) + " " + item.Year);
                });

                that.data = data;
                $.proxy(that.updateChart, that)();
            });
        },

        updateChart: function () {
            var that = this;
            var totalValueElement = that.element.find('#total-value');
            var chartContainer = that.element.find('#chart');
            if (that.data.Total != null || that.data.Total!= undefined) {
                totalValueElement.html(that.data.Total);
            } else {
                totalValueElement.html(0);
            }

            var tickInterval = Math.ceil(that.options.series.length/4);
            var areaChartOptions = {
                title: null,
                chart: {
                    type: 'areaspline'
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
                    name: 'Kundkontakter',
                    marker: {
                        symbol: 'circle'
                    },
                    data: that.options.series

                }]
            };

            chartContainer.highcharts(areaChartOptions);
            
        },

       
        destroy: function () {
            $.Widget.prototype.destroy.apply(this, arguments);
        }
    });
}(jQuery));