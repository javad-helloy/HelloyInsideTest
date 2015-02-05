(function ($) {
    $.widget('hi.campaigntable', $.hi.datedependentwidget, {
        tableBody: null,

        options: {
            show: true,
            isCampaign: true,
            firstDay: null,
            lastDay: null,
        },

        createReport: function () {
            var that = this;
            that.options.firstDay = moment().format('YYYY-MM-01');
            that.options.lastDay = moment(that.options.firstDay).add(1, 'months').add(-1, 'days').format('YYYY-MM-DD');

            var table =
                $('<table class="table">' +
                    '<thead>' +
                        '<tr>' +
                            '<td><span>Produkt</span></td>' +
                            '<td><span>Annonseringskanal</span></td>' +
                            '<td><span>Kampanj</span></td>' +
                            '<td style="text-align:right"><span>Visningar</span></td>' +
                            '<td style="text-align:right"><span>Klick</span></td>' +
                            '<td style="text-align:right"><span>Kontakter</span></td>' +
                            '<td style="text-align:right"><span>Annonseringskostnad</span></td>' +
                            '<td style="text-align: center; font-size: 18px;"><i class="fa fa-thumbs-up"></i></td>' +
                    '</thead>' +
                '</table>');

            that.tableBody = $('<tbody></tbody>');
            table.append(that.tableBody);
            that.element.append(table);
            
            
            
            var setCampaignTableAsActive = function () {
                
                if (that.options.show) {
                    that.tableBody.html("");
                    that.updateData();
                    that.element.show();
                }
            }

            var setCampaignTableAsInActive = function () {
                that.element.hide();
                that.tableBody.html("");
                
            }

            var widgetVisibilityHandler = function(size) {
                if (size == "large" && that.options.isCampaign) {
                    that.options.show = true;
                    setCampaignTableAsActive();

                } else {
                    
                    that.options.show = false;
                    setCampaignTableAsInActive();
                }
            }

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

            $(document).bind('screenSizeChange', function (event, size) {
                widgetVisibilityHandler(size);
            });

            $(document).trigger('requestScreenSize', function (size) {
                widgetVisibilityHandler(size);
            });
        },

        update : function() {
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
            
            that.tableBody.html('<tr><td colspan="8" style="text-align:center; font-size: 38px;"><i class="fa fa-spinner fa-spin"></i></td><tr>');
            $.getJSON('/api/client/' + that.options.clientId + '/source/campaignsdetails?startDate=' + firstDay + '&endDate=' + lastDay, function (data) {
                that.tableBody.html("");
                
                $(data).each(function (index, value) {
                    that.addItemToTable(value, that.tableBody);
                });
            });
        },

        addItemToTable: function (item, table) {
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
                '<tr>' +
                    '<td><span class="label label-success">' + rowItem.Product + '</span></td>' +
                    '<td>' + rowItem.AdProvider + '</td>' +
                    '<td>' + rowItem.Campaign + '</td>' +
                    '<td style="text-align:right">' + rowItem.Impressions + '</td>' +
                    '<td style="text-align:right">' + rowItem.AdClicks + '</td>' +
                    '<td style="text-align:right">' + rowItem.NumContacts + '</td>' +
                    '<td style="text-align:right">' + rowItem.AdCost + '</td>' +
                    '<td class="index-score" data-index-score="' + rowItem.IndexValue + '" ></td>' +
                '</tr>');

            row.find('.index-score').IndexScore();
            table.append(row);
        }
    });
}(jQuery));