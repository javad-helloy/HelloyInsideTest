(function($) {
    $.widget('hi.contactlist', $.hi.reportwidget, {
        contactList: null,
       
        create: function () {
            var that = this;
            var element = $(this.element);

            var contactList = $('<ol class="icon-list"></ol>');
            that.contactList = contactList;

            var moreContactsHandler= function (event, eventData) {
                $.each(eventData.contacts, function (index, value) {
                    var item = that.createItem(value, that);
                    item.appendTo(that.contactList);
                });

                element.find('#load-more-contacts').show();
                element.find('.loading').hide();

                if (eventData.contacts.length < 20) {
                    element.find('#load-more-contacts').hide();
                }
            }
            var contactChangedHandler= function (event, contact) {
                var contactRow = that.contactList.find('li[data-contact-id="' + contact.Id + '"]');
                var rowToAppend = contactRow.prev();
                contactRow.remove();

                var item = that.createItem(contact, that);

                if (rowToAppend.length > 0) {
                    rowToAppend.after(item);
                } else {
                    that.contactList.prepend(item);
                }
            }

            var selectedItemChangedHandler = function (event, items) {
                if (items.selected.length == 0) {
                    $('.item-icon').show();
                    $('.select-icon').hide();
                    $('.select-icon').removeClass('fa-check');
                } else {
                    $('.item-icon').hide();
                    $('.select-icon').show();
                }
            }

            var screenSizeHandler = function(size) {
                if (size == "large") {
                    element.hide();
                    that.contactList.html("");
                    $(document).unbind('moreContacts', moreContactsHandler);
                    $(document).unbind('contactChanged', contactChangedHandler);
                    $(document).unbind('selectedItemsChanged', selectedItemChangedHandler);
                } else {
                    $(document).unbind('moreContacts', moreContactsHandler).bind('moreContacts', moreContactsHandler);
                    $(document).unbind('contactChanged', contactChangedHandler).bind('contactChanged', contactChangedHandler);
                    $(document).unbind('selectedItemsChanged', selectedItemChangedHandler).bind('selectedItemsChanged', selectedItemChangedHandler);;
                    if (element.is(':hidden')) {
                        element.show();
                        that.contactList.html("");
                        that.update();

                    }
                }
            }
            
            $(document).bind('screenSizeChange', function (event, size) {
                screenSizeHandler(size);
            });

            $(document).trigger('requestScreenSize', function (size) {
                screenSizeHandler(size);
            });

            element.append('<div class="loading" style="text-align:center; padding-left: 0; font-size: 38px;"><i class="fa fa-spinner fa-spin"></i></div>');
            var loadMore =
                $('<div style="text-align:center;" id="load-more-contacts">' +
                    '<button id="show-more-results" class="btn btn-default btn-default-inverted btn-icon-only btn-rounded">' +
                        '<i class="fa fa-chevron-down"></i>' +
                    '</button>' +
                '</div>');

            contactList.appendTo(element);
            var loadMoreButton = loadMore.find('button');
            loadMore.hide();
            loadMore.appendTo(element);

            loadMoreButton.on("click",function () {
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
        },

        update: function () {
            var that = this;
            var element = $(this.element);

            $(document).trigger('getContacts', function (contacts) {
                that.contactList.html("");
                $.each(contacts, function (index, value) {
                    var item = that.createItem(value, that);
                    item.appendTo(that.contactList);
                });
                if (contacts.length>0) {
                    element.find('.loading').hide();
                    element.find('#load-more-contacts').show();
                }
                if (contacts.length < 20) {
                    element.find('#load-more-contacts').hide();
                }
            });
        },

        createItem: function (contact, widget) {

            var contactItemTemplate =
                '<li data-contact-id="{{contactId}}">' +
					'<i class="item-icon fa {{icon}} item-icon-circle item-icon-left" ></i>' +
                    '<i class="select-icon item-icon-left select-item-icon fa " style="display:none;" data-selected="false"></i>' +
					'{{{contactInfo}}}'+
					'<div class="bottom-line info-container">'+
						'<span><i class="fa fa-clock-o"></i>{{time}}</span>' +
                        '<span><i class="fa fa-calendar-o" ></i>{{dateString}}</span>' +
					'</div>' +
                    '<div class="bottom-line info-container" id="label-container">' +
					    '{{{labels}}}' +
                    '</div>' +
				'</li>';

            var contactViewModel = {};
            
            var contactDate = moment(contact.Date);

            //date
            contactViewModel.dateString = contactDate.format('DD MMM YYYY');
            contactViewModel.time = contactDate.format('HH:mm');


            //icon
            switch (contact.LeadType) {
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


            //labels
            contactViewModel.labels = "";
            if (Object.keys(contact.Interaction).length == 0 && contact.RatingScore == null && contact.ReadStatus==null) {
                contactViewModel.labels += '<a href="/sirius/contactlist?clientId=' + widget.options.clientId + '&status=new" role="link-new"><span class="label label-action label-primary">Ny!</span></a>';
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

            switch (contact.LeadType) {
                case "Phone":
                    if (contact.Property.CallerNumber !== undefined) {
                       
                        contactViewModel.contactInfo = '<div class="top-line info-container">' +
 						                                    '<span><i class="fa fa-phone"></i>' + contact.Property.CallerNumber + '</span>' +
					                                    '</div>';
                    } else {
                        contactViewModel.contactInfo = '<span><i class="fa fa-user"></i>Okänd</span>';
                    }
                    break;
                case "Email":
                    if (contact.Property.FromEmail !== undefined) {
                        contactViewModel.contactInfo = '<div class="top-line info-container">' +
 						                                    '<span><i class="fa fa-envelope-o"></i>' + contact.Property.FromEmail + '</span>' +
					                                    '</div>';
                    }

                    if (contact.Property.FromPhone !== undefined) {
                        if (contactViewModel.contactInfo !== undefined) {
                            contactViewModel.contactInfo += '<div class="top-line info-container">' +
                                                                '<span><i class="fa fa-phone"></i>' + contact.Property.FromPhone + '</span>' +
                                                            '</div>';
                        } else {
                            contactViewModel.contactInfo = '<div class="top-line info-container">' +
                                                                '<span><i class="fa fa-phone"></i>' + contact.Property.FromPhone + '</span>' +
                                                            '</div>';
                        }
                    }
                    if (contact.Property.FromPhone === undefined && contact.Property.FromEmail === undefined) {
                        contactViewModel.contactInfo = '<span><i class="fa fa-user"></i>Okänd</span>';
                    }
                    break;
                case "Chat":
                    if (contact.Property.Email !== undefined) {
                        contactViewModel.contactInfo = '<div class="top-line info-container">' +
 						                                    '<span><i class="fa fa-envelope-o"></i>' + contact.Property.Email + '</span>' +
					                                    '</div>';
                    }

                    if (contact.Property.Phone !== undefined) {
                        if (contactViewModel.contactInfo !== undefined) {
                            contactViewModel.contactInfo += '<div class="top-line info-container">' +
                                                                '<span><i class="fa fa-phone"></i>' + contact.Property.Phone + '</span>' +
                                                            '</div>';
                        } else {
                            contactViewModel.contactInfo = '<div class="top-line info-container">' +
                                                                '<span><i class="fa fa-phone"></i>' + contact.Property.Phone + '</span>' +
                                                            '</div>';
                        }
                    }
                    if (contact.Property.Phone === undefined && contact.Property.Email === undefined) {
                        contactViewModel.contactInfo = '<span><i class="fa fa-user"></i>Okänd</span>';
                    }
                    break;
            }

            contactViewModel.contactId = contact.Id;

            var contactRowHtml = Mustache.render(contactItemTemplate, contactViewModel);

            var contactRow = $(contactRowHtml);

            
            contactRow.find('a').on("click", function (event) {
                event.stopImmediatePropagation();
            });

            if (contactViewModel.labels=="") {
                contactRow.find('#label-container').remove();
            }

            contactRow.find('.item-icon').on("click", function (event) {
                event.stopImmediatePropagation();
                contactRow.find('.select-icon').trigger("click");
                return false;
                //onSelectIcoonClick(event);
            });


            contactRow.find('.select-icon').on("click", function(event) {
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
                    contactRow.find('.select-icon').show();
                    if (selectedItems.indexOf(contact.Id) != -1) {
                        contactRow.find('.select-icon').addClass('fa-check');
                    }
                }
            });

            contactRow.on("click", function (event) {
               
                if (!$('body').hasClass('sidebar-active')) {
                    $(document).trigger('showDetailModal', [contact, "mini"]);
                }
            });

            return contactRow;

        },

    });

}(jQuery));