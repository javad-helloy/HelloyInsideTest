(function($) {
    $.widget('hi.selectHeader', $.hi.reportwidget, {

        create: function () {
            var that = this;
            var element = $(that.element);
            element.html("");

            element.append('<div class="left-menu ">' +
                               '<button class="btn-icon-only btn-empty"><i class="fa fa-arrow-left"></i></button>' +
                           '</div>' +
                           '<div class=" middle-menu menu-item" style="display:inline-block;">' +
                                '<span id="selected-counter-header">1 Selected </span>' +
                            '</div>' +
                            '<div class="dropdown right-menu" style="display:inline-block;">' +
                                '<a href="#" class="btn-icon-only dropdown-toggle" id="dropdownMenu2" data-toggle="dropdown">' +
                                   '<i class="fa fa-tag"></i>' +
                               '</a>' +
                            '</div>');


            element.find('.left-menu button').on("click", function () {
                that.element.hide();
                $(document).trigger("clearSelected");
            });

            $(document).bind("selectedItemsChanged", function (event, items) {
                
                if (items.selected.length > 0) {
                    that.element.show();
                } else {
                    that.element.hide();
                }
                element.find('#selected-counter-header').html(items.selected.length + " Selected");
            });
        },

        update: function() {
            var that = this;
            that.element.find('.right-menu').data("client-id", that.options.clientId);
            that.element.find('.right-menu').selectMenu();
        }
    });
}(jQuery));