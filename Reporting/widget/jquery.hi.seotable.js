(function ($) {
    $.widget('hi.seotable', $.hi.reportwidget, {
        data: null,
        tableBody: null,
        options: {
            startDate: null
        },

        create: function () {
            var that = this;
            var element = that.element;
            element.html("");
            
            that.options.startDate = moment().add(-1, 'months').format("YYYY-MM-DD");
        },

        update: function () {
           
            this.updateData();
        },

        updateData: function () {
            var that = this;
            
            var endDate = moment().format("YYYY-MM-DD");
            var url = '/Seo/GetPositionsWithHistory?' + $.param({
                clientId: that.options.clientId,
                startDate: that.options.startDate,
                endDate: endDate
            });
            
            $.getJSON(url, function (data) {
                that.data = data;
                ($.proxy(that.updateList, that))();
            });

        },

        updateList: function () {
            var that = this;

            if (that.data.length <= 0) {
                that.element.hide();
            }

            that.element.append(' <header>'+
                                    '<h2>Mina sökord </h2>'+
                                '</header>');

            var widget = $('<section class="widget"></section>');
            that.element.append(widget);

            var tableContainer = $('<table class="table icon-list">' +
                                        '<thead>' +
                                            '<tr>' +
                                                '<td style="width:150px;"></td>' +
                                                '<td><span>Sökord</span></td>' +
                                                '<td ><span>Position</span></td>' +
                                                '<td class="visible-md visible-lg"><span>Startposition</span></td>' +
                                                '<td class="visible-sm visible-md visible-lg" style="width:250px;"><span>Utveckling under perioden</span></td>' +
                                            '</tr>' +
                                        '<thead>' +
                                   '</table>');

            that.tableBody = $('<tbody></tbody>');
            tableContainer.append(that.tableBody);
            widget.append(tableContainer);

            that.tableBody.html("");
            var seoListItemTemplate =
                '<tr > ' +
                         '<td style="width:150px;">' +
                            '<i class="item-icon  fa fa-arrow-{{Trend}}"></i>' +
                        '</td> ' +
                        '<td >' +
                            '<p>{{Keyword}}</p>' +
                        '</td> '+
                        '<td > ' +
                           '<p>{{Position}}</p>' +
                        '</td> ' +
                        '<td class="visible-md visible-lg" > ' +
                           '<p >{{Start}}</p>' +
                        '</td> ' +
                         '<td class="visible-sm visible-md visible-lg" style="width:250px;">' +
                            '<div class="keyword-trend"></div>' +
                            '</td>' +
                    '</tr >';

            var numAddedItems = 0;
            
            $.each(that.data, function (index, keyword) {

                var keywordData = {
                    Keyword: keyword.Keyword,
                    Position: (keyword.Position !== null && keyword.Position !=0 ? keyword.Position : "-"),
                    Start: (keyword.Start !== null && keyword.Start != 0 ? keyword.Start : "-"),
                };

                if (keyword.History.length > 0) {
                    if (keyword.History[keyword.History.length - 1].Position - keyword.History[0].Position > 0) {
                        keywordData.Trend = "down";
                    } else if (keyword.History[keyword.History.length - 1].Position - keyword.History[0].Position == 0) {
                        keywordData.Trend = "right";
                    } else if (keyword.History[keyword.History.length - 1].Position - keyword.History[0].Position < 0) {
                        keywordData.Trend = "up";
                    }
                } else {
                    keywordData.Trend = "right";
                }

                var keywordRow = $(Mustache.to_html(seoListItemTemplate, keywordData));
                
                var graphData = {
                    animate: false,
                    includeXLabelInTooltip: true,
                    categories: [],
                    series: []
                };
                
                $(keyword.History).each(function (i, datapoint) {
                    var dateString = moment(datapoint.Date).format().substring(0, 10);
                    graphData.categories.push(dateString);
                    
                });
                graphData.series.push({
                    
                    name: "Position",
                    data: _.map(keyword.History, function (val) {
                        return val.Position;
                    })
                });
                that.tableBody.append(keywordRow);
                keywordRow.find('.keyword-trend').summarychart(graphData);
                numAddedItems++;
                
            });
            
        },
    });

}(jQuery));