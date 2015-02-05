$.fn.ShareContact = function () {
    var element = $(this);

    
    var button = $('<button class="btn btn-success" id="event-label"><i class = "fa fa-share-alt"></i></button>');
    element.append(button);

    button.on("click",function (event) {
        event.stopImmediatePropagation();
        $(document).trigger('closeAllModals');
        var contactId = parseInt(element.data("contact-id"));
        var modalHtml = '<div class="modal" id="share-contact">' + 
                            '<div class="modal-dialog">' + 
                                '<div class="modal-content">' +
                                  '<div class="modal-body">' +
                                  '<div class="decription-container">' +
                                        '<h4>Mottagare</h4>' +
                                        '<input type="email" class="form-control" placeholder="E-post" id="emailAddress">' +
                                        '<h4>Kontaktinfo</h4>' +
                                        '<div class="form-group">' +
                                            '<input type="text" class="form-control" id="contact" placeholder="E-post eller telefon">' + 
                                        '</div>' + 
                                        '<div class="form-group">' + 
                                            '<textarea class="form-control" rows="6" placeholder="Kommentar" id="comment"></textarea>' +
                                        '</div>' +
                                    '</div>' +
                                    '<div class="form-group" style="text-align:right;">' +
                                        '<button type="button" class="btn btn-default" role="close-modal" style="margin-right: 5px;">Avbryt</button>' +
                                        '<button type="button" class="btn btn-success" role="send-contact" style="margin-left: 5px;">Dela</button>' +
                                    '</div>' +
                                  '</div>' + 
                                '</div>' + 
                            '</div>'+
                        '</div>';

        var modalElement = $(modalHtml);
        $('body').prepend(modalElement);
        modalElement.modal('show');
        modalElement.find('#emailAddress').focus();

        modalElement.on('hidden.bs.modal', function () {
            modalElement.remove();
        });

        $(document).trigger('getContact', [contactId, function (contact) {
            modalElement.find('#emailAddress').focus();
            if (contact.LeadType == "Phone") {
                modalElement.find('#contact').val(contact.Property.CallerNumber);
            }
            else if (contact.LeadType == "Email") {
                modalElement.find('#contact').val(contact.Property.FromEmail);
                modalElement.find('#comment').val($('<div>'+contact.Property.Html+'</div>').text());

            }
            else if (contact.LeadType == "Chat") {
                var contacts = [];

                if (contact.Property.Email !== undefined) {
                    contacts.push(contact.Property.Email);
                }
                if (contact.Property.Phone !== undefined) {
                    contacts.push(contact.Property.Phone);
                }

                modalElement.find('#contact').val(contacts.join(", "));
                modalElement.find('#comment').val(contact.Property.Description);
            }
        }]);

        var sendButton = modalElement.find('button[role="send-contact"]');
        sendButton.click(function () {
            modalElement.find('input, textarea').prop('readonly', true);;
            modalElement.find('button[role="send-contact"]').html('<i class="fa fa-spinner fa-spin"></i> Skickar').prop("disabled", true);;

            var query = {
                contactId: contactId,
                emailAddress: $('#emailAddress').val(),
                customerContact: $('#contact').val(),
                comment: $('#comment').val(),
            }

            $.get('/api/social/sendcontact', query).success(function () {
                modalElement.modal('hide');
                modalElement.remove();

            }).fail(function() {
                modalElement.find('button[role="send-contact"]').html('<i class="fa fa-exclamation-circle"></i> Misslyckades').prop("disabled", true).addClass('btn-danger').removeClass('btn-success');
            });
        });

        var closeButton = modalElement.find('button[role="close-modal"]');
        closeButton.click(function () {
            modalElement.modal('hide');
            modalElement.remove();
        });
    });
};