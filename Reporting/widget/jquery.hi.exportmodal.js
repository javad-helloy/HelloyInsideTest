(function ($) {
    $(document).bind("showExportModal", function (event, style) {
        $(document).trigger('closeAllModals', "exportmodal");
        var modal = $('<div class="modal" role="dialog" aria-hidden="true"></div>');
        modal.exportmodal({ style: style }).appendTo("body");
        modal.modal('show');
        
    });
}(jQuery));

(function ($) {
    $.widget('hi.exportmodal', $.hi.reportwidget, {
        options: {
            style: null,
        },
        create: function() {
            var that = this;
            var element = $(this.element);
            element.html("");

            element.append(' <div class="modal-dialog"><div class="modal-content "><div class="modal-body" id="modal-body"></div></div></div>');

            $(document).bind('closeAllModals', function (event, modalCall) {
                if (modalCall != "exportmodal") {
                    element.modal('hide');
                    that.element.remove();
                }
            });
        },

        update: function () {
            this.show();
        },


        show: function() {
            var that = this;

            var modal = that.element;

            if (that.options.style == "mini") {
                modal.addClass("mini");
            } else {
                modal.removeClass("mini");
            }

            var body = modal.find('#modal-body');
            body.html('<div class="decription-container">' +
                        '<p>Denna export kan ta upp till en minut att generera. Den kommer innehålla en CSV-fil som går att öppna i exempelvis Excel.</p>' +
                      '</div>' +
                      '<div style="text-align:right;">' +
                        '<button type="button" class="btn btn-default" role="close-modal" style="margin-right: 5px;">Avbryt</button>' +
                        '<a class="btn btn-success " href="/Export/ExportData?clientId=' + that.options.clientId + '" target="_blank" style="margin-left: 5px;">Exportera <i class="fa fa-cloud-download"></i></a>' +
                      '</div>');


            modal.modal('show');

            modal.on('hidden.bs.modal', function () {
                that.element.remove();
            });

            modal.find('button[role="close-modal"]').click(function () {
                modal.modal('hide');
            });

        },

    });
}(jQuery));