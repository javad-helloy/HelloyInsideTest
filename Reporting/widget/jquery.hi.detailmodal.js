(function ($) {
    $(document).bind("showDetailModal", function (event, eventContact, style) {
        $(document).trigger('closeAllModals', "detailmodal");
        var modal = $('<div class="modal" role="dialog" aria-hidden="true"></div>');
        modal.detailmodal({ contact: eventContact, style: style }).appendTo("body");
        modal.modal('show');

    });
}(jQuery));

(function ($) {
    $.widget('hi.detailmodal', $.hi.reportwidget, {
        
        options: {
            contact: null,
            style: null,
        },
        create: function() {
            var that = this;
            var element = $(this.element);
            element.html("");

            element.append(' <div class="modal-dialog modal-lg">' +
                                '<div class="modal-content ">' +
                                    '<div class="modal-header header-fixed" data-client-id="" data-contact-id="" id="modal-header"></div> ' +
                                    '<div class="modal-body" id="modal-body" data-client-id="" ></div>' +
                                '</div>' +
                            '</div>');

            $(document).bind('closeAllModals', function (event, modalCall) {
                if (modalCall != "detailmodal") {
                    element.modal('hide');
                    that.element.remove();
                }
            });

            
        },

        update: function() {
            this.show();
        },

        show: function() {
            var that = this;

            var modal = that.element;
            var contact = that.options.contact;

            if (that.options.style == "mini") {
                modal.addClass("mini");
            } else {
                modal.removeClass("mini");
            }

            var header = modal.find('#modal-header');
            header.html("");

            header.data("client-id", that.options.clientId);
            header.data("contact-id", contact.Id);
            header.ContactDetail({ action: "header", contact: contact, style: that.options.style });
            
            header.append('<div class="btn-group">' +
                               '<button id="previous-contact" type="button" class="btn btn-default btn-lg pull-left"><i class="fa fa-angle-left" ></i></button>' +
                               '<button id="next-contact" type="button" class="btn btn-default btn-lg pull-right"><i class="fa fa-angle-right" ></i></button>' +
                         '</div>');

            var modalBody = modal.find("#modal-body");
            modalBody.html("");

            modalBody.data("client-id", that.options.clientId);
            modalBody.ContactDetail({ action: "body", contact: contact, style: that.options.style });
            modalBody.append('<div style="text-align:right;">' +
                                '<button type="button" class="btn btn-default" role="close-modal">Stäng</button>' +
                            '</div>');

            modal.find('#next-contact').on("click", function (event) {
                event.stopImmediatePropagation();
                $(document).trigger('getNextContact', [contact.Id, function (nextContact) {
                    if (nextContact != null) {
                        that.options.contact = nextContact;
                        $.proxy(that.show(), that);
                    }
                }]);
            });

            modal.find('#previous-contact').on("click", function (event)
            {
                event.stopImmediatePropagation();
                $(document).trigger('getPreviousContact', [contact.Id, function (prevContact) {
                    if (prevContact != null) {
                        that.options.contact = prevContact;
                        $.proxy(that.show(), that);
                    }
                }]);
            });

            modal.on('hidden.bs.modal', function() {
                $('audio').remove();
                that.element.remove();
            });

            modal.find('button[role="close-modal"]').on("click", function (event) {
                event.stopImmediatePropagation();
                modal.modal('hide');
            });

            that.options.contact.ReadStatus = "Read";
            that.setInteraction(that, "ReadStatus", "Read");
            
        },

        setInteraction: function(widget, type, value) {

            var postData = {
                Type: type,
                Value: value
            }

            $.post("/api/client/" + widget.options.clientId + "/lead/" + widget.options.contact.Id + "/interaction", postData);
            $(document).trigger('contactChanged', widget.options.contact);
        }
    });
}(jQuery));