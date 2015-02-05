(function($) {
    $.widget('hi.selectMenu', $.hi.reportwidget, {
        tagList: null,
        selectedItems: [],
        labelMenu: null,
        createNewButton: null,
        manageTagButton:null,

        create: function() {
            var that = this;

            var labelMenu = $('<ul class="dropdown-menu dropdown-menu-large right" role="menu" aria-labelledby="dropdownMenu2">' +
                '</ul>');

            that.element.append(labelMenu);
            that.labelMenu = labelMenu;

            that.createNewButton = $('<li class="item-labels" role="createNew">' +
                                        '<a role="menuitem" class="dropdown-menu-item with-icon" tabindex="-1" href="#" data-label-name="Skapa Ny">' +
                                            '<i class="fa fa-plus" ></i>' +
                                            '<span>Skapa Ny</span>' +
                                        '</a>' +
                                    '</li>');

            that.manageTagButton = $('<li class="item-labels" role="manageTag">' +
                                        '<a role="menuitem" class="dropdown-menu-item with-icon" tabindex="-1" href="#" data-label-name="Hantera Etiketter">' +
                                            '<i class="fa fa-cog" ></i>' +
                                            '<span>Hantera Etiketter</span>' +
                                        '</a>' +
                                    '</li>');

            $(document).bind("contactChanged", function () {
                if (that.tagList != null) {
                    that.checkForCommonTagsBetweenSelectedContacts();
                }
            });

            $(document).bind("selectedItemsChanged", function (event, items) {
                that.selectedItems = items.selected;
                if (that.tagList != null) {
                    that.checkForCommonTagsBetweenSelectedContacts();
                }
            });


            $(document).bind("tagListChanged", function () {
                that.updateTagList(that, function() {
                    that.checkForCommonTagsBetweenSelectedContacts();
                });
            });

            $(document).trigger("getSelected", function (selectedItems) {
                that.selectedItems = selectedItems;
                if (that.tagList != null) {
                    that.checkForCommonTagsBetweenSelectedContacts();
                }
            });
        },

        update: function () {
            var that = this;
            if (that.tagList == null) {
                that.updateTagList(that);
            }  
        },

        updateTagList: function(widget, callback) {
            var url = '/api/client/' + widget.options.clientId + '/tag';
            $.getJSON(url, function(data) {
                widget.tagList = data;
                widget.updateMenu(widget);
                if (callback != undefined) {
                    callback();
                }
            });
        },

        updateMenu: function (widget) {
            var that = widget;
            that.labelMenu.html("");
            $.each(that.tagList, function (index, tag) {
                that.addTagToMenu(that, tag);
            });

            that.labelMenu.append('<li class="divider" role="presentation"></li>');
            that.labelMenu.append(that.createNewButton);

            that.labelMenu.find('li[role="createNew"]').on("click", function () {
                $(document).trigger("showNewTagModal");
            });

            that.labelMenu.append('<li class="divider" role="presentation"></li>');
            that.labelMenu.append(that.manageTagButton);

            that.labelMenu.find('li[role="manageTag"]').on("click", function () {
                $(document).trigger("showManageTagsModal");
            });
        },

        addTagToMenu: function (widget, tag) {
            
            var labelRow = $('<li class="item-labels" role="presentation">' +
                               '<a role="menuitem" class="dropdown-menu-item with-icon" tabindex="-1" href="#" data-label-name="' + tag.Name + '" data-label-id="' + tag.Id + '">' +
                                   '<i class="fa fa-square-o " ></i>' +
                                   '<span>' + tag.Name + '</span>' +
                               '</a>' +
                           '</li>');

            if (tag.isCommonlySelected) {
                labelRow.find('i').removeClass().addClass("fa fa-check-square fa-success");
                //labelRow.addClass("current");
            }

            labelRow.find('a').on("click", function () {
                var labelElement = $(this);
                $.each(widget.selectedItems, function(index, contactId) {
                    $(document).trigger("getContact", [contactId, function(contactObject) {
                        if (tag.isCommonlySelected) {
                            widget.unSetTagForContact(widget, contactId, labelElement.data("label-id"), function() {
                                contactObject.Tags = _.filter(contactObject.Tags, function(tagValue) { return tagValue.Name != labelElement.data("label-name"); });
                                $(document).trigger("contactChanged", contactObject);
                                widget.checkForCommonTagsBetweenSelectedContacts();
                            });
                        } else {
                            var contactHasTag = _.filter(contactObject.Tags, function (tagValue) { return tagValue.Name == labelElement.data("label-name"); }).length > 0;
                            if (!contactHasTag) {
                                widget.setTagForContact(widget, contactId, labelElement.data("label-id"), function() {
                                    contactObject.Tags.push({ Id: labelElement.data("label-id"), Name: labelElement.data("label-name") });
                                    $(document).trigger("contactChanged", contactObject);
                                    widget.checkForCommonTagsBetweenSelectedContacts();
                                });
                            }
                        }
                    }]);
                });
                
            });

            widget.labelMenu.append(labelRow);
        },

        checkForCommonTagsBetweenSelectedContacts: function () {
            var that = this;
            var allContactTags = {};
            $.each(that.tagList, function (i, tag) {
                allContactTags[tag.Id] = {
                    Count: 0
                };
            });

            $.each(that.selectedItems, function (index, contactId) {
                $(document).trigger("getContact", [contactId, function (contactObject) {
                    $.each(contactObject.Tags, function (i, tag) {
                        allContactTags[tag.Id]["Count"] += 1;
                    });

                }]);
            });

            $.each(that.tagList, function (i, tag) {
                tag.isCommonlySelected = false;
                if (allContactTags[tag.Id]["Count"] == that.selectedItems.length && that.selectedItems.length>0) {
                    tag.isCommonlySelected = true;
                }
            });

            that.updateMenu(that);
        },

        setTagForContact: function (widget, contactId, tagId, callback) {
            var url = '/api/client/' + widget.options.clientId + '/tag/' + tagId + '/contact/' + contactId;
            $.ajax({
                url: url,
                type: 'PUT',
                success: function (result) {
                    if (callback != undefined) {
                        callback();
                    }
                }
            });
        },

        unSetTagForContact: function (widget, contactId, tagId, callback) {
            var url = '/api/client/' + widget.options.clientId + '/tag/' + tagId + '/contact/' + contactId;
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (result) {
                    if (callback != undefined) {
                        callback();
                    }
                }
            });
        }


    });
}(jQuery));