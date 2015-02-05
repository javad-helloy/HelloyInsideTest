(function ($) {
    $.widget('hi.keywordslist', $.hi.datedependentwidget, {
        keywordsList: null,

        options: {
            show: false,
            isKeyword: false,
            firstDay: null,
            lastDay: null,
        },

        createReport: function () {
            var that = this;
            that.options.firstDay = moment().format('YYYY-MM-01');
            that.options.lastDay = moment(that.options.firstDay).add(1, 'months').add(-1, 'days').format('YYYY-MM-DD');
            that.element.hide();

            var element = $(this.element);

            var keywordsList = $('<ol class="icon-list large-list"></ol>');
            that.keywordsList = keywordsList;
            keywordsList.appendTo(element);

           var setCampaignTableAsActive = function () {
                if (that.options.show) {
                    that.keywordsList.html("");
                    that.updateData();
                    element.show();
                }
            }

            var setCampaignTableAsInActive = function () {
                element.hide();
                that.keywordsList.html("");
            }

            var widgetVisibilityHandler = function (size) {
                if (size == "small" && that.options.isKeyword) {
                    that.options.show = true;
                    setCampaignTableAsActive();
                } else {
                    that.options.show = false;
                    setCampaignTableAsInActive();
                }
            }

            $(document).bind('keywordReport', function () {
                $(document).trigger('requestScreenSize', function (size) {
                    that.options.isKeyword = true;
                    widgetVisibilityHandler(size);
                });
            });

            $(document).bind('campaignReport', function () {
                that.options.isKeyword = false;
                that.options.show = false;
                that.element.hide();
            });

            $(document).bind('screenSizeChange', function (event, size) {
                widgetVisibilityHandler(size);
            });

            $(document).trigger('requestScreenSize', function (size) {
                widgetVisibilityHandler(size);
            });

        },

        update: function () {
            var that = this;
            if (that.options.show) {
                that.updateData();
            }

        },

        updateData: function () {
            var that = this;

            if (that.options.clientId == null) {
                return;
            }

            var firstDay = that.options.firstDay;
            var lastDay = that.options.lastDay;

            that.keywordsList.html('<li style="text-align:center; padding-left: 0; font-size: 38px;"><i class="fa fa-spinner fa-spin"></i></li>');
            $.getJSON('/api/client/' + that.options.clientId + '/source/searchkeywords?startDate=' + firstDay + '&endDate=' + lastDay, function (data) {
                that.keywordsList.html("");

                $(data).each(function (index, value) {
                    that.addItemToList(value, that.keywordsList);
                });
            });
        },

        addItemToList: function (item, list) {
            var rowItem = {};
            rowItem.KeyWord = item.Name;

            var seperateThousands = function (x) {
                return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, " ");
            }

            if (item.AdClicks != null) {
                rowItem.AdClicks = seperateThousands(item.AdClicks);
            }
            else {
                rowItem.AdClicks = "--";
            }

            if (item.AdCost != null) {
                rowItem.AdCost = seperateThousands(Math.round(item.AdCost)) + '  kr';
            }
            else {
                rowItem.AdCost = "--";
            }

            if (item.Impressions != null) {
                rowItem.Impressions = seperateThousands(item.Impressions);
            }
            else {
                rowItem.Impressions = "--";
            }

            if (item.ContactCollection.IndexValue != null) {
                rowItem.IndexValue = Math.round(item.ContactCollection.IndexValue * 10);
            } else {
                rowItem.IndexValue = 0;
            }

            if (item.NumContacts != null) {
                rowItem.NumContacts = seperateThousands(Math.round(item.NumContacts));
            }
            else {
                rowItem.NumContacts = "--";
            }

            var row = $(
                '<li >' +
                    '<i class="item-icon fa fa-search-plus item-icon-circle item-icon-left"></i>' +
                    '<div class="top-line info-container">' +
                        '<span>' + rowItem.KeyWord + '</span>' +
                    '</div>' +
                    '<table class="table">' +
                        '<tr>' +
                            '<td>' +
                                '<span class="lighter">Visningar</span>' +
                            '</td>' +
                            '<td style="text-align:right">' +
                                '<span class="darker" >' + rowItem.Impressions + '</span>' +
                            '</td>' +
                        '</tr>' +
                        '<tr >' +
                            '<td >' +
                                '<span class="lighter">Klick</span>' +
                            '</td>' +
                            '<td style="text-align:right">' +
                                '<span class="darker" >' + rowItem.AdClicks + '</span>' +
                            '</td>' +
                        '</tr>' +
                        '<tr>' +
                            '<td >' +
                                '<span class="lighter">Kontakter</span>' +
                            '</td>' +
                            '<td  style="text-align:right">' +
                                '<span class="darker" >' + rowItem.NumContacts + '</span>' +
                            '</td>' +
                        '</tr>' +
                        '<tr>' +
                            '<td >' +
                                '<span class="lighter">Kostnad</span>' +
                            '</td>' +
                            '<td  style="text-align:right">' +
                                '<span class="darker" >' + rowItem.AdCost + '</span>' +
                            '</td>' +
                        '</tr>' +
                        '<tr>' +
                            '<td >' +
                                '<span class="lighter">Helloy Index</i></span>' +
                            '</td>' +
                            '<td  style="text-align:right">' +
                                '<span class="index-score" data-index-score="' + rowItem.IndexValue + '" ></span>' +
                            '</td>' +
                        '</tr>' +
                    '</table>' +
                '</li>');

            row.find('.index-score').IndexScore();
            list.append(row);
        },

    });

}(jQuery));