(function ($) {
    $.widget('hi.contact', $.hi.reportwidget, {
        contact: null,
        options: {
            contactId: null,
            style: null
        },

        create: function() {
            var that = this;
            var element = $(this.element);
            element.html("");
            element.append(' <header><h2 class="hidden-when-header">Kontakt</h2></header>');
            element.append('<div class="widget contact-detail">' +
                '<div class="header header-fixed" id="header" data-client-id="" data-contact-id=""></div>' +
                '<div class="body" id="body" data-client-id=""><div class="loading" style="text-align:center; padding-left: 0; font-size: 38px;"><i class="fa fa-spinner fa-spin"></i></div></div>' +
                '</div>');

            var onSuccessContactRecieve = function (contactRecieved) {
                that.contact = contactRecieved;
                that.contact.ReadStatus = "Read";
                that.setInteraction(that, "ReadStatus", "Read");
                if (that.contact != null) {
                    $.proxy(that.render(), that);
                }
            }

            var onFailContactReceive = function () {
                element.html("");
                element.append(' <header><h2 class="hidden-when-header">Kontakt</h2></header>');
                element.append('<div style="text-align:center;" class="alert alert-danger">Kontakten hittades ej</div>');
            }

            $(document).trigger('getContact', [that.options.contactId, onSuccessContactRecieve, onFailContactReceive]);

            var screenSizeHandler = function(size) {
                if (size == "small") {
                    that.options.style = "mini";
                } else {
                    that.options.style = null;
                }

                if (that.contact != null) {
                    $.proxy(that.render(), that);
                }
            }

            $(document).trigger('requestScreenSize', function (size) {
                screenSizeHandler(size);
            });

            $(document).bind('screenSizeChange', function (event, size) {
                screenSizeHandler(size);
            });
        },

        render: function () {
            var that = this;

            var element = that.element.find('.contact-detail');

            if (that.options.style == "mini") {
                element.addClass("mini");
            } else {
                element.removeClass("mini");
            }

            var body = element.find("#body");
            body.html("");

            var header = $('#header');
            header.html("");

            var contact = that.contact;

            header.data("client-id", that.options.clientId);
            header.data("contact-id", contact.Id);
            header.ContactDetail({ action: "header", contact: contact, style: that.options.style });

            body.data("client-id", that.options.clientId);
            body.ContactDetail({ action: "body", contact: contact, style: that.options.style });

        },

        setInteraction: function (widget, type, value) {

            var postData = {
                Type: type,
                Value: value
            }

            $.post("/api/client/" + widget.options.clientId + "/lead/" + widget.contact.Id + "/interaction", postData);
            //$(document).trigger('contactChanged', widget.contact);
        }
    });
}(jQuery));