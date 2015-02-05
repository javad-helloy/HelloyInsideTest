(function($) {
    $.widget('hi.selecteddata', $.hi.reportwidget, {
        selectedItems: [],

        options: {

        },

        create: function () {
            var that = this;

            $(document).bind('getSelected', function (event, callback) {
                callback(that.selectedItems);
            });

            $(document).bind('isContactSelected', function (event, contactId, callback) {
                if (that.selectedItems.indexOf(contactId) != -1) {
                    callback(true);
                } else {
                    callback(false);
                }
                
            });

            $(document).bind('clearSelected', function () {
                that.selectedItems = [];
                $(document).trigger("selectedItemsChanged", { selected: that.selectedItems });
            });

            $(document).bind('selectedItemToggle', function (event, item) {
                
                if (that.selectedItems.indexOf(item) != -1) {
                    that.selectedItems.splice($.inArray(item, that.selectedItems), 1);

                } else {
                    that.selectedItems.push(item);
                }

                $(document).trigger("selectedItemsChanged", {selected: that.selectedItems });

            });
        }
    });
}(jQuery));