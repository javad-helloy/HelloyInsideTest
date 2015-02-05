(function ($) {

    $.fn.ContactDetail = function (options) {
        var settings = $.extend({
            action: "",
            contact: null,
            style: null

        }, options);

        var element = $(this);
        var clientId = element.data('client-id');

        var contact = settings.contact;
        if (settings.action === "header") {
            var headerTemplate = '<div class="share-contact" data-contact-id="{{contactId}}" ></div>' +
                                 '<div class="raiting" data-score="{{ratingScore}}" data-auto-score="{{autoRatingScore}}" data-contact-id="{{contactId}}" data-client-id="{{clientId}}"></div>';
                         

            var headerViewModel = {};
            
            headerViewModel.clientId = clientId;
            headerViewModel.contactId = contact.Id;
            headerViewModel.ratingScore = 0;
            if (contact.RatingScore !== null) {
                headerViewModel.ratingScore = parseInt(contact.RatingScore);
            }

            headerViewModel.autoRatingScore = 0;
            if (contact.AutoRatingScore !== null) {
                headerViewModel.autoRatingScore = parseInt(contact.AutoRatingScore);
            }

            var headerHtml = Mustache.render(headerTemplate, headerViewModel);

            element.html("");
            $(headerHtml).appendTo(element);

            
            element.find('.raiting').StarRaiting();
            element.find('.share-contact').ShareContact();
        }

        else if (settings.action === "body") {
            var contactModalTemplate = '<div class="info-container" id="info-container">' +
                                            '{{{contactInfo}}}' +
                                            '<span><i class="fa fa-calendar"></i>{{dateTimeString}}</span>' +
                                            '{{{searchPhrase}}}' +
                                       '</div>' +
                                       '<div class="info-container" id="lable-container">' +
                                            '<span>' +
                                                '<i class="fa fa-tag"></i>' +
                                                    '{{{labels}}}' +
                                            '</span>' +
                                       '</div>' +
                                       '<div class="decription-container">' +
                                           '<h4>{{contentHeader}}</h4>' +
                                           '<div class="decription-content {{descriptionClassType}}" id="decription-container" >' +
                                                '{{{description}}}' +
                                           '</div>' +
                                       '</div>';


            var contactViewModel = {};

            var contactDate = moment(contact.Date);
            contactViewModel.clientId = clientId;
            //date
            contactViewModel.dateTimeString = contactDate.format('DD MMM HH:mm');

            // from

            switch (contact.LeadType) {
                case "Phone":
                    if (contact.Property.CallerNumber !== undefined) {
                        contactViewModel.contactInfo = '<span><i class="fa fa-phone"></i>' + contact.Property.CallerNumber + '</span>';
                    } else {
                        contactViewModel.contactInfo = '<span><i class="fa fa-user"></i>Okänd</span>';
                    }
                    break;
                case "Email":
                    if (contact.Property.FromEmail !== undefined) {
                        contactViewModel.contactInfo = '<span><i class="fa fa-envelope-o"></i>' + contact.Property.FromEmail + '</span>';
                    }
                    if (contact.Property.FromPhone !== undefined) {
                        if (contactViewModel.contactInfo !== undefined) {
                            contactViewModel.contactInfo += '<span><i class="fa fa-phone"></i>' + contact.Property.FromPhone + '</span>';
                        } else {
                            contactViewModel.contactInfo = '<span><i class="fa fa-phone"></i>' + contact.Property.FromPhone + '</span>';
                        }

                    }
                    if (contact.Property.FromPhone === undefined && contact.Property.FromEmail === undefined) {
                        contactViewModel.contactInfo = '<span><i class="fa fa-user"></i>Okänd</span>';
                    }
                    break;
                case "Chat":
                    if (contact.Property.Email !== undefined) {
                        contactViewModel.contactInfo = '<span><i class="fa fa-envelope-o"></i>' + contact.Property.Email + '</span>';
                    }

                    if (contact.Property.Phone !== undefined) {
                        if (contactViewModel.contactInfo !== undefined) {
                            contactViewModel.contactInfo += '<span><i class="fa fa-phone"></i>' + contact.Property.Phone + '</span>';
                        } else {
                            contactViewModel.contactInfo = '<span><i class="fa fa-phone"></i>' + contact.Property.Phone + '</span>';
                        }
                        
                    }
                    if (contact.Property.Phone === undefined && contact.Property.Email === undefined) {
                        contactViewModel.contactInfo = '<span><i class="fa fa-user"></i>Okänd</span>';
                    }
                    break;
            }

            //description
            switch (contact.LeadType) {
                case "Phone":
                    var path = contact.Property.Audio;
                    if (path != undefined && path != null && path.indexOf(".mp3") == -1) {
                        path = path.concat(".mp3");
                    }

                    var duration = contact.Property.Duration;
                    if (duration === undefined || duration === null) {
                        duration = 0;
                    }
                    contactViewModel.description = '<div class="audio-player" data-path="' + path + '" data-duration="' + duration + '"></div>';
                    break;
                case "Email":
                    var html = contact.Property.Html;
                    if (html === undefined || html === null) {
                        html = "";
                        if (contact.Property.FormData !== undefined && contact.Property.FormData !== null) {
                            var formData = JSON.parse(contact.Property.FormData);
                            for (var prop in formData) {
                                if (formData.hasOwnProperty(prop)) {
                                    html += '<p><span class = "strong">' + prop + ": </span> <span>" + formData[prop] + '</span></p>';
                                }
                            }
                        } 
                    }
                    contactViewModel.description = '<p>' + html + '</p>';
                    contactViewModel.descriptionClassType = 'text';
                    contactViewModel.contentHeader = "Innehåll";
                    break;
                case "Chat":
                    contactViewModel.description = '<p>' + contact.Property.Description + '</p>';
                    contactViewModel.descriptionClassType = 'text';
                    contactViewModel.contentHeader = "Innehåll";
                    break;
            }


            //labels
            contactViewModel.labels = "";
            if (contact.Product !== null) {
                contactViewModel.labels += '<a href="/report/' + clientId + '/contact?product=' + encodeURI(contact.Product) + '"><span class="label label-action label-success">' + contact.Product + '</span></a>';
            }

            if (contact.Interaction.Rating !== undefined) {
                contactViewModel.labels += '<a href="/report/' + clientId + '/contact?rating=' + encodeURI(contact.Interaction.Rating) + '"><span class="label label-action label-default">' + contact.Interaction.Rating + '</span></a>';
            }

            if (contact.Interaction.Status !== undefined) {
                contactViewModel.labels += '<a href="/report/' + clientId + '/contact?status=' + encodeURI(contact.Interaction.Status) + '"><span class="label label-action label-default">' + contact.Interaction.Status + '</span></a>';
            }

            if (contact.Tags !== undefined && contact.Tags.length > 0) {
                $.each(contact.Tags, function (index, tag) {
                    contactViewModel.labels += '<a href="/report/' + clientId + '/contact?tagName=' + encodeURI(tag.Name) + '"><span class="label label-action label-info">' + tag.Name + '</span></a>';
                });
            }

            //search phrase
            contactViewModel.searchPhrase = "";
            if (contact.SearchPhrase !== null) {
                contactViewModel.searchPhrase = '<span><i class="fa fa-search" ></i>' + contact.SearchPhrase + '</span>';
            }

            contactViewModel.contactId = contact.Id;

            var modalRowHtml = Mustache.render(contactModalTemplate, contactViewModel);
            element.html("");
            $(modalRowHtml).appendTo(element);

            if (contact.LeadType == "Phone") {
                if (contact.Property.Audio != undefined) {
                    element.find('.audio-player').AudioPlayer(settings.style);
                } else {
                    element.find('.audio-player').append('<div class="alert alert-danger" role="alert" style="height: 100%;">Hittade ingen inspelning</div>');
                }
            }

            if (contactViewModel.labels == "") {
                element.find('#lable-container').remove();
            }
        }

    };


})(jQuery);