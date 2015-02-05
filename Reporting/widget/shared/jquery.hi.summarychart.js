(function ($) {
    $.widget('hi.summarychart', {
        widgetEventPrefix: 'hi',
        options: {
            series: null,
            categories: null,
            animate: true,
            includeXLabelInTooltip: false,
        },

        _init: function () {
           var categories = this.options.categories;
           var areaChartOptions = {
               title: null,
               credits: {
                   enabled: false
               },
                chart: {
                    type: 'area',
                    backgroundColor: null,
                    margin: [0, 2, 0, 2],
                    width: 220,
                    height: 30,
                    style: {
                        overflow: 'visible'
                    },
                    skipClone: true
                   
                },
                colors: [
                    '#3db762'
                    ],
                legend: {
                    enabled: false
                },
                xAxis: {
                    
                    labels: {
                        enabled: false
                    },
                    tickWidth: 0,
                    startOnTick: false,
                    endOnTick: false,
                    tickPositions: [],
                    categories: categories
                },
                yAxis: {
                    endOnTick: false,
                    startOnTick: false,
                    reversed: true,
                    gridLineColor: "#ffffff",
                    labels: {
                        enabled: false
                    },
                    title: {
                        text: null
                    },
                    tickPositions: [0]
                },
               
                tooltip: {

                    shared: true,
                    
                    backgroundColor: null,
                    borderColor: "#3e3e3e",
                    style: {
                        color: '#000',
                        fontSize: '12px',
                        padding: '8px'
                    },
                    
                    borderWidth: 0,
                    shadow: false,
                    useHTML: true,
                    hideDelay: 0,
                    padding: 0,
                    positioner: function (w, h, point) {
                        return { x: point.plotX - w / 2, y: point.plotY - h };
                    },
                    
                },
                plotOptions: {
                    area: {
                        stacking: 'normal',
                        fillOpacity: 0.1,
                        threshold: 500,
                        marker: {
                            enabled:false
                        }
                    },
                    series: {
                        enableMouseTracking: true
                    }
                },
                series: [
                ],
            };

           var colors = ['#3db762'];
           var maxValue = 0;

            for (var iterPosition = 0; iterPosition < this.options.series[0].data.length; iterPosition++) {
                var currentValueWhenStacked = 0;
                for (var iterSeries = 0; iterSeries < this.options.series.length; iterSeries++) {
                    currentValueWhenStacked += this.options.series[iterSeries].data[iterPosition];                     
                }
                
                if (currentValueWhenStacked > maxValue) {
                    maxValue = currentValueWhenStacked;
                }
            }
            areaChartOptions.yAxis.max = maxValue;
            if (!this.options.animate) {
               
                for (var iterSeries = 0; iterSeries < this.options.series.length; iterSeries++) {
                    areaChartOptions.series.push({
                        name: this.options.series[iterSeries].name,
                        color: colors[iterSeries],
                        data: this.options.series[iterSeries].data,
                       
                    });
                }
            }
            this.element.highcharts(areaChartOptions);
        },

        destroy: function () {
            $.Widget.prototype.destroy.apply(this, arguments);
        }
    });
}(jQuery));
