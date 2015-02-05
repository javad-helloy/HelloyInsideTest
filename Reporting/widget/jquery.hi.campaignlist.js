(function($) {
    $.widget('hi.campaignlist', $.hi.datedependentwidget, {
        campaignList: null,
        options: {
            show: false,
            isCampaign: true,
            firstDay: null,
            lastDay: null,
        },

        createReport: function () {
            var that = this;
            that.options.firstDay = moment().format('YYYY-MM-01');
            that.options.lastDay = moment(that.options.firstDay).add(1, 'months').add(-1, 'days').format('YYYY-MM-DD');

            var element = $(this.element);
           
            var campaignList = $('<ol class="icon-list large-list"></ol>');
            that.campaignList = campaignList;
            campaignList.appendTo(element);

            
            var setCampaignTableAsActive = function () {
               if (that.options.show) {
                    that.campaignList.html("");
                    that.updateData();
                    element.show();
                }
            }

            var setCampaignTableAsInActive = function () {
                element.hide();
                that.campaignList.html("");
            }

            var widgetVisibilityHandler = function(size) {
                if (size == "small" && that.options.isCampaign) {
                    that.options.show = true;
                    setCampaignTableAsActive();
                } else {
                    that.options.show = false;
                    setCampaignTableAsInActive();
                }
            }

            $(document).bind('screenSizeChange', function (event, size) {
                widgetVisibilityHandler(size);
            });

            $(document).bind('campaignReport', function () {
                $(document).trigger('requestScreenSize', function (size) {
                    that.options.isCampaign = true;
                    widgetVisibilityHandler(size);
                });
            });

            $(document).bind('keywordReport', function () {
                that.options.isCampaign = false;
                that.options.show = false;
                that.element.hide();
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

            that.campaignList.html('<li style="text-align:center; padding-left: 0; font-size: 38px;"><i class="fa fa-spinner fa-spin"></i></li>');
            $.getJSON('/api/client/' + that.options.clientId + '/source/campaignsdetails?startDate=' + firstDay + '&endDate=' + lastDay, function (data) {
                that.campaignList.html("");

                $(data).each(function (index, value) {
                    that.addItemToList(value, that.campaignList);
                });
            });
        },

        addItemToList: function (item, list) {
            var rowItem = {};
            rowItem.Product = item.Product;
            rowItem.AdProvider = item.AdProvider == "adwords" ? "AdWords" : item.AdProvider;
            rowItem.Campaign = item.Campaign;

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

            if (item.Product == "Search") {
                rowItem.Icon = "fa-search";
            }
            else if (item.Product == "Display") {
                rowItem.Icon = "fa-arrows-alt";
            }
            else if (item.Product == "Retargeting") {
                rowItem.Icon = "fa-exchange";
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
                    '<i class="item-icon fa ' + rowItem.Icon + ' item-icon-circle item-icon-left"></i>' +
                    '<div class="top-line info-container">' +
                        '<span>' + rowItem.Campaign + '</span>' +
                    '</div>' +
                    '<div class="bottom-line info-container">' +
                        '<span class="label label-success">' + rowItem.Product + '</span>' +
                        '<span class="label label-default">' + rowItem.AdProvider + '</span>' +
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
                                '<span class="lighter">Helloy Index</span>' +
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