(function ($) {
    $.widget('hi.flowSummary', $.hi.reportwidget, {
        widgetEventPrefix: 'hi',
        data: null,
        options: {
            
            take: 3
        },

        create: function () {
            var that = this;
            var element = that.element;
            element.html("");

            element.append(' <header><h2 class="">Resultat per månad</h2></header>');
            var flowSummaryList = $('<ol id="flow-list" class="budget-list"></ol>');
            var flowSummaryListItem=$(' <li class="widget">' +
                '<table class="table-centered">'+
                    '<tr>'+
                        '<td>'+
                            '<i class="fa fa-calendar"></i>'+
                        '</td>'+
                        '<td>'+
                            '<span class="large-value">--</span>'+
                        '</td>'+
                        '<td>'+
                            '<span class="large-value">--</span>' +
                        '</td>'+
                        '<td>'+
                            '<span class="large-value">--</span>' +
                        '</td>'+
                        '<td colspan="2" class="hidden-us">'+
                            '<div class="raiting-readonly" >' +
                                '<i class="fa fa-star icon-disable"></i>' +
                                '<i class="fa fa-star icon-disable"></i>' +
                                '<i class="fa fa-star icon-disable"></i>' +
                                '<i class="fa fa-star icon-disable"></i>' +
                                '<i class="fa fa-star icon-disable"></i>'+
                            '</div>'+
                        '</td>'+
                    '</tr>'+
                    '<tr>'+
                        '<td>'+
                            '<div class=""></div>'+
                        '</td>'+
                        '<td>'+
                            'Samtal'+
                        '</td>'+
                        '<td>'+
                            'E-post'+
                        '</td>'+
                        '<td>'+
                            'Chat'+
                        '</td>'+
                        '<td colspan="2" class="hidden-us">'+
                            'Medelbetyg: --'+
                        '</td>'+
                    '</tr>'+
                '</table>'+
                '</li>');
            flowSummaryList.append(flowSummaryListItem);
            flowSummaryList.append(flowSummaryListItem.clone());
            flowSummaryList.append(flowSummaryListItem.clone());
            element.append(flowSummaryList);
            

            //that.updateData();

        },

        update: function () {
            this.updateData();
        },

        updateData: function () {
            var that = this;
            var url = '/api/client/' + that.options.clientId + '/lead/aggregateType';

            $.getJSON(url, function (data) {

                that.data = data;
                $.proxy(that.updateList, that)();
            });
        },

        updateList: function () {

            var that = this;
            var element = that.element;
            var flowList = element.find("#flow-list");
           
            var flowListItemTemplate = ' <li class="widget" >' +
                '<table class="table-centered">'+
                    '<tr >'+
                        '<td>'+
                            '<i class="fa fa-calendar"></i>'+
                        '</td>'+
                        '<td>'+
                            '<span class="large-value">{{Phone}}</span>' +
                        '</td>'+
                        '<td>'+
                            '<span class="large-value">{{Email}}</span>' +
                        '</td>'+
                        '<td>'+
                            '<span class="large-value">{{Chat}}</span>' +
                        '</td>' +
                        '<td id="custom-contact-data" style="display:none;">' +
                            '<span class="large-value">{{Event}}</span>' +
                        '</td>' +
                        '<td colspan="2" class="hidden-us">'+
                            '<div class="raiting-readonly">' +
                                '<i class="fa fa-star icon-disable"></i>' +
                                '<i class="fa fa-star icon-disable"></i>' +
                                '<i class="fa fa-star icon-disable"></i>' +
                                '<i class="fa fa-star icon-disable"></i>' +
                                '<i class="fa fa-star icon-disable"></i>'+
                            '</div>'+
                        '</td>'+
                    '</tr>'+
                    '<tr>'+
                        '<td>'+
                            '<div id="calendar" class="label label-success">{{Month}} {{Year}}</div>'+
                        '</td>'+
                        '<td>'+
                            'Samtal'+
                        '</td>'+
                        '<td>'+
                            'E-post'+
                        '</td>'+
                        '<td>'+
                            'Chat'+
                        '</td>' +
                        '<td id="custom-contact-memo" style="display:none;">' +
                            'Event' +
                        '</td>' +
                        '<td colspan="2" class="hidden-us">'+
                            'Medelbetyg: {{Rating}}'+
                        '</td>'+
                    '</tr>'+
                '</table>'+
                '</li>';


            var sortedData = that.data.sort(function (a, b) {
                return a.Year - b.Year || a.Month - b.Month;
            });

            if (sortedData.length > 0) {
                flowList.html("");
            }
            $.each(sortedData.reverse(), function (index, item) {

                var summaryData = {
                    Phone: item.Count.Phone,
                    Email: item.Count.Email,
                    Chat: item.Count.Chat,
                    Event: item.Count.Event,
                    Rating: item.Scores.AverageScore != null ? Math.round(item.Scores.AverageScore * 10) / 10 : 0,
                    Year: item.Year,
                    Month: moment.months(parseInt(item.Month) - 1).substring(0, 3),
                    
                };
                
                var row = $(Mustache.to_html(flowListItemTemplate, summaryData));

                if (that.options.showCustomContacts=="True") {
                    var customEventDataItem = row.find('#custom-contact-data');
                    var customEventMemoItem = row.find('#custom-contact-memo');
                    row.find('table').addClass('columns-7');

                    customEventDataItem.show();
                    customEventMemoItem.show();
                }


                if (index > 0) {
                    row.addClass("passed");
                    row.find('#calendar').removeClass('label-success').addClass('label-info');
                }

                if (index >= that.options.take) {
                    row.hide();
                }
                var ratingStars = row.find('.raiting-readonly i');

                var rating = Math.round(item.Scores.AverageScore);
                for (var i = 0; i < rating; i++) {
                    $(ratingStars[i]).removeClass("icon-disable").addClass("icon-gold");
                }

                
                flowList.append(row);
            });

            var showMoreItem = $('<li class="widget-empty" style="text-align:center;">'+
                                    '<button id="show-more-results" class="btn btn-default btn-default-inverted btn-icon-only btn-rounded">'+
                                        '<i class="fa fa-chevron-down"></i>'+
                                    '</button>'+
                                '</li>');

           
            showMoreItem.find('#show-more-results').on('click', function() {
                that.element.find("li:hidden:first").fadeIn("fast");
                that.options.take += 1;
                if (that.options.take >= that.element.find('li.widget').length) {
                    $(this).hide();
                }
            });
           
            flowList.append(showMoreItem);

        },

        destroy: function () {
            $.Widget.prototype.destroy.apply(this, arguments);
        }
    });
}(jQuery));