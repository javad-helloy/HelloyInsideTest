(function($) {
    $(document).bind("showManageTagsModal", function () {
        $(document).trigger('closeAllModals', "managetag");
        var modal = $('<div class="modal" role="dialog" aria-hidden="true"></div>');
        modal.managetag().appendTo("body");
        modal.modal('show');
    });
}(jQuery));


(function ($) {
    $.widget('hi.managetag', $.hi.reportwidget, {

        tagList: null,
        tableBody: null,
        options: {
            style: null,
        },
        create: function () {
            var that = this;
            var element = $(this.element);
            element.html(' <div class="modal-dialog modal-md">' +
                                '<div class="modal-content ">' +
                                    '<div class="modal-body" id="modal-body" data-client-id="" >' +
                                        '<h4>Etiketter</h4>' +
                                        '<div class="alert-danger" style="display:none;"></div>'+
                                        '<table class="icon-list table tag-list" id="tag-table">' +
                                            '<tbody>' +
                                            '</tbody>'+
                                        '</table>' +
                                        '<div style="text-align:right;">' +
                                            '<button type="button" class="btn btn-default" role="close-modal" style="margin-right: 5px;">Avbryt</button>' +
                                        '</div>' +
                                    '</div>' +
                                '</div>' +
                            '</div>');

            $(document).bind('closeAllModals', function (event, modalCall) {
                if (modalCall != "managetag") {
                    element.modal('hide');
                    that.element.remove();
                }
            });

            $(document).bind('tagListChanged', function () {
                that.update();
            });
            
            element.on('hidden.bs.modal', function () {
                that.element.remove();
            });

            element.find('button[role="close-modal"]').on("click", function () {
                element.modal('hide');
                that.element.remove();
            });

            that.tableBody = element.find('#tag-table tbody');

            $(document).bind('screenSizeChange', function (event, size) {
                if (size == "small") {
                    that.element.addClass("mini");
                } else {
                    that.element.removeClass("mini");
                }
                
            });

            $(document).trigger('requestScreenSize', function (size) {
                if (size == "small") {
                    that.element.addClass("mini");
                } else {
                    that.element.removeClass("mini");
                }

            });
        },

        update: function() {
            var that = this;
            var url = '/api/client/' + that.options.clientId + '/tag';
            $.getJSON(url, function (data) {
                that.tagList = data;
                $.proxy(that.updateList, that)();
            });
        },

        updateList: function () {
            var that = this;
            that.tableBody.html("");
            $.each(that.tagList, function (index, tag) {
                that.addTagToList(that, tag);
            });
        },

        addTagToList: function (widget, tag) {
            
            var tagRow = $('<tr data-tag-id="' + tag.Id + '">' +
                                '<td><a href="/report/' + widget.options.clientId + '/contact?tagName=' + encodeURI(tag.Name) + '"><span class="label label-info">' + tag.Name + '</span></a></td>' +
                                '<td style="text-align:right;"><button type="button" class="btn btn-sm btn-danger" role="delete-tag">Delete</button></td>' +
                            '</tr>');

            tagRow.find('button[role="delete-tag"]').on("click", function () {
                var tagId = tagRow.data("tag-id");
                widget.deleteTag(widget, tagId);
            });
            
            widget.tableBody.append(tagRow);
            

        },

        deleteTag: function(widget, tagId) {
            var url = '/api/client/' + widget.options.clientId + '/tag/' + tagId + '/delete';
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (deletedTag) {
                    $(document).trigger("tagListChanged");
                    widget.updateAllContactsForDeletedTag(deletedTag);

                }
            });
        },

        updateAllContactsForDeletedTag: function (deletedTag) {
            $(document).trigger("getContacts", function (contacts) {
                $.each(contacts, function (key, contact) {
                    $.each(contact.Tags, function (indexOfTag, tagInContact) {
                        if (tagInContact.Id == deletedTag.Id) {
                            contact.Tags.splice(indexOfTag, 1);
                            $(document).trigger("contactChanged", contact);
                            return false;
                        }
                    });
                });
            });
        }
    });
}(jQuery));