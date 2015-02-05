(function ($) {
    $.widget('hi.contacttable', $.hi.reportwidget, {
        contactTableBody: null,
        
        create: function () {
            var that = this;
            var element = $(this.element);

            var contactTable =
                $('<table class="icon-list table interactable">' +
                    '<thead><tr><td></td><td><span>Datum<span></td><td><span>Etikett<span></td><td><span>Från</span></td><td><span>Betyg</span></td><td><span>Dela</span></td></tr></thead>' +
                    '<tbody><tr class="loading"><td colspan="6" style="text-align:center; font-size: 38px;"><i class="fa fa-spinner fa-spin"></i></td><tr></tbody>' +
                '</table>');

            element.append(contactTable);

            that.contactTableBody = contactTable.find('tbody');

            var loadMore =
                $('<div style="text-align:center;" id="load-more-contacts">' +
                    '<button id="show-more-results" class="btn btn-default btn-default-inverted btn-icon-only btn-rounded">' +
                        '<i class="fa fa-chevron-down"></i>' +
                    '</button>' +
                '</div>');

            loadMore.hide();
            element.append(loadMore);
            var loadMoreButton = loadMore.find('button');
            
            loadMoreButton.click(function () {
                var button = $(this);
                button.attr('disabled', 'disabled');
                button.find('.fa').removeClass('fa-chevron-down').addClass('fa-spinner').addClass('fa-spin');
                $(document).trigger('fetchMoreContacts', function (data) {
                    if (data.length < 20) {
                        button.hide();
                    }

                    button.removeAttr('disabled');
                    button.find('.fa').removeClass('fa-spinner').removeClass('fa-spin').addClass('fa-chevron-down');
                });
            });

            var contactChangedHandler = function (event, contact) {
                var contactRow = that.contactTableBody.find('tr[data-contact-id="' + contact.Id + '"]');
                var rowToAppend = contactRow.prev();
                contactRow.remove();

                var item = that.createItem(contact, that);

                if (rowToAppend.length > 0) {
                    rowToAppend.after(item);
                } else {
                    that.contactTableBody.prepend(item);
                }

            }
            var moreContactHandler = function (event, eventData) {

                $.each(eventData.contacts, function (index, value) {
                    var item = that.createItem(value, that);
                    item.appendTo(that.contactTableBody);
                });
                element.find('.loading').remove();
                element.find('#load-more-contacts').show();

                if (eventData.contacts.length < 20) {
                    element.find('#load-more-contacts').hide();
                }
            }

            var selectedItemChangedHandler = function (event, items) {
                if (items.selected.length == 0) {
                    $('.item-icon').show();
                    $('.select-icon').hide();
                    $('.select-icon').removeClass('fa-check');
                } else {
                    $('.item-icon').hide();
                    $('.select-icon').css('display', 'block');
                }
            }

            var setContactTableAsActive = function () {
                $(document).unbind('moreContacts', moreContactHandler);
                $(document).unbind('contactChanged', contactChangedHandler);
                $(document).unbind('selectedItemsChanged', selectedItemChangedHandler);
                $(document).bind('contactChanged', contactChangedHandler);
                $(document).bind('moreContacts', moreContactHandler);
                $(document).bind('selectedItemsChanged', selectedItemChangedHandler);

                if (element.is(":hidden")) {
                    element.show();
                    that.update();
                    
                }
            }

            var setContactTableAsInActive = function () {
                element.hide();
                that.contactTableBody.html("");
                $(document).unbind('moreContacts', moreContactHandler);
                $(document).unbind('contactChanged', contactChangedHandler);
                $(document).unbind('selectedItemsChanged', selectedItemChangedHandler);
            }

            var screenSizeHandler = function(size) {
                if (size == "small") {
                    setContactTableAsInActive();
                } else {
                    setContactTableAsActive();
                }
            }

            $(document).bind('screenSizeChange', function (event, size) {
                screenSizeHandler(size);
            });

            $(document).trigger('requestScreenSize', function (size) {
                screenSizeHandler(size);
            });
        },
        update: function () {
            var that = this;
            var element = $(this.element);
            
            $(document).trigger('getContacts', function (contacts) {
                that.contactTableBody.html("");
                $.each(contacts, function (index, value) {
                    var item = that.createItem(value, that);
                    item.appendTo(that.contactTableBody);
                });

                element.find('#load-more-contacts').show();

                if (contacts.length < 20) {
                    element.find('#load-more-contacts').hide();
                }
            });
        },

        createItem:function(contact, widget) {
            var contactRowTemplate = 
                '<tr data-contact-id="{{contactId}}">' +
                    '<td>' +
                        '<i class="item-icon fa {{icon}} item-icon-circle item-icon-left" ></i>' +
                        '<i class="select-icon item-icon-left select-item-icon fa " style="display:none;"></i>' +
                    '</td>' +
                    '<td><span>{{dateString}}</span></td>' +
                    '<td class="labels-container">{{{labels}}}</td>' +
                    '<td>{{from}}</td>' +
                    '<td class="rating-container"><div class="raiting" data-score="{{ratingScore}}" data-auto-score="{{autoRatingScore}}" data-contact-id="{{contactId}}" data-client-id="{{clientId}}"></div></td>' +
                    '<td><div class="share-contact" data-contact-id="{{contactId}}"><div></td>' +
                '</tr>';

            var contactViewModel = {};
            contactViewModel.clientId = widget.options.clientId;

            var contactDate = moment(contact.Date);
            var isFromToday = contactDate.isSame(new Date(), "day");
            var isFromThisYear = contactDate.isSame(new Date(), "year");


            //date
            if (isFromToday) {
                contactViewModel.dateString = contactDate.format('HH:mm');
            } else if (isFromThisYear) {
                contactViewModel.dateString = contactDate.format('DD MMM');
            } else {
                contactViewModel.dateString = contactDate.format('DD MMM YYYY');
            }


            //icon
            switch(contact.LeadType) {
                case "Phone":
                    contactViewModel.icon = 'fa-phone';
                    break;
                case "Email":
                    contactViewModel.icon = 'fa-envelope-o';
                    break;
                case "Chat":
                    contactViewModel.icon = 'fa-comment-o';
                    break;
                case "Event":
                    contactViewModel.icon = 'fa-bullseye';
                    break;
            }

            contactViewModel.leadType = contact.LeadType;
            //labels
            contactViewModel.labels = "";
            if (Object.keys(contact.Interaction).length == 0 && contact.RatingScore == null && contact.ReadStatus == null) {
                contactViewModel.labels += '<a href="/report/' + widget.options.clientId + '/contact?status=new" role="link-new"><span class="label label-action label-primary">Ny!</span></a>';
            }

            if (contact.Product !== null) {
                contactViewModel.labels += '<a href="/report/' + widget.options.clientId + '/contact?product=' + encodeURI(contact.Product) + '"><span class="label label-action label-success">' + contact.Product + '</span></a>';
            }

            if (contact.Interaction.Rating !== undefined) {
                contactViewModel.labels += '<a href="/report/' + widget.options.clientId + '/contact?rating=' + encodeURI(contact.Interaction.Rating) + '"><span class="label label-action label-default">' + contact.Interaction.Rating + '</span></a>';
            }

            if (contact.Interaction.Status !== undefined) {
                contactViewModel.labels += '<a href="/report/' + widget.options.clientId + '/contact?status=' + encodeURI(contact.Interaction.Status) + '"><span class="label label-action label-default">' + contact.Interaction.Status + '</span></a>';
            }

            if (contact.Tags !== undefined && contact.Tags.length > 0) {
                $.each(contact.Tags, function (index, tag) {
                    contactViewModel.labels += '<a href="/report/' + widget.options.clientId + '/contact?tagName=' + encodeURI(tag.Name) + '"><span class="label label-action label-info">' + tag.Name + '</span></a>';
                });
            }

            // from
            if (contact.Property.CallerNumber !== undefined) {
                contactViewModel.from = contact.Property.CallerNumber;
            }
            if (contact.LeadType == "Email") {
                if (contact.Property.FromEmail !== undefined) {
                    contactViewModel.from = contact.Property.FromEmail;
                }
                if (contact.Property.FromPhone !== undefined) {
                    if (contact.Property.FromEmail !== undefined) {
                        contactViewModel.from += ' ,'+ contact.Property.FromPhone;
                    } else {
                        contactViewModel.from = contact.Property.FromPhone;
                    }
                }
            }
            if (contact.LeadType == "Chat") {
                if (contact.Property.Email !== undefined) {
                    contactViewModel.from = contact.Property.Email;
                }
                if (contact.Property.Phone !== undefined) {
                    if (contact.Property.Email !== undefined) {
                        contactViewModel.from += ', '+contact.Property.Phone;
                    }
                    else
                    {
                        contactViewModel.from = contact.Property.Phone;
                    }
                }
            }


            // score
            contactViewModel.ratingScore = 0;
            if (contact.RatingScore !== null) {
                contactViewModel.ratingScore = parseInt(contact.RatingScore);
            }

            contactViewModel.autoRatingScore = 0;
            if (contact.AutoRatingScore !== null) {
                contactViewModel.autoRatingScore = parseInt(contact.AutoRatingScore);
            }


            contactViewModel.contactId = contact.Id;

            var contactRowHtml = Mustache.render(contactRowTemplate, contactViewModel);

            var contactRow = $(contactRowHtml);
            contactRow.find('a').click(function(event) {
                event.stopImmediatePropagation();
            });

            contactRow.find('.item-icon').click(function (event) {
                event.stopImmediatePropagation();
                contactRow.find('.select-icon').trigger("click");
            });

            contactRow.find('.select-icon').click(function (event) {
                event.stopImmediatePropagation();
                var ckeckBox = $(this);
                if (ckeckBox.hasClass('fa-check')) {
                    ckeckBox.removeClass('fa-check');
                } else {
                    ckeckBox.addClass('fa-check');
                }

                $(document).trigger("selectedItemToggle", contactRow.data("contact-id"));
            });

            $(document).trigger('getSelected', function (selectedItems) {
                if (selectedItems.length > 0) {
                    contactRow.find('.item-icon').hide();
                    contactRow.find('.select-icon').css('display', 'block');
                    if (selectedItems.indexOf(contact.Id) != -1) {
                        contactRow.find('.select-icon').addClass('fa-check');
                    }
                }
            });


            contactRow.click(function () {
                $(document).trigger('showDetailModal', contact);
            });
            contactRow.find('.raiting').StarRaiting();
            contactRow.find('.share-contact').ShareContact();

            return contactRow;
        },
    });
}(jQuery));