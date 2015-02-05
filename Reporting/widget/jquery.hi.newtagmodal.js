(function ($) {
    $(document).bind("showNewTagModal", function () {
        $(document).trigger('closeAllModals', "newtagmodal");
        var modal = $('<div class="modal" role="dialog" aria-hidden="true"></div>');
        modal.newtagmodal().appendTo("body");
        modal.modal('show');
    });
}(jQuery));

(function ($) {
    $.widget('hi.newtagmodal', $.hi.reportwidget, {
        
        options: {
            style: null,
        },
        create: function() {
            var that = this;
            var element = $(this.element);
            element.html("");

            element.append(' <div class="modal-dialog modal-md">' +
                                '<div class="modal-content ">' +
                                    '<div class="modal-body" id="modal-body" data-client-id="" >' +
                                        '<div class="form-group">' +
                                            '<input type="text" class="form-control" id="tagName" placeholder="Label Name">' +
                                        '</div>' +
                                        '<div style="text-align:right;">' +
                                            '<button type="button" class="btn btn-default" role="close-modal" style="margin-right: 5px;">Avbryt</button>' +
                                            '<button type="button" class="btn btn-success" role="create-label" style="margin-left: 5px;">Skapa</button>' +
                                        '</div>' +
                                    '</div>' +
                                '</div>' +
                            '</div>');

            $(document).bind('closeAllModals', function (event, modalCall) {
                if (modalCall != "newtagmodal") {
                    element.modal('hide');
                }
            });

            element.find('button[role="create-label"]').on("click", function () {
                var tagName = element.find('#tagName').val();
                that.createTag(that, tagName);
            });

            element.on('hidden.bs.modal', function () {
                that.element.remove();
            });

            element.find('button[role="close-modal"]').on("click", function () {
                element.modal('hide');
                that.element.remove();
            });

        },

        createTag: function (widget, tagName) {
            var url = '/api/client/' + widget.options.clientId + '/tag/create?tagName=' + tagName;
            $.ajax({
                url: url,
                type: 'PUT',
                success: function(createdTag) {
                    widget.element.modal('hide');
                    $(document).trigger("tagListChanged", createdTag);
                    $(document).trigger("getSelected", function(selecteditems) {
                        widget.updateSelectedContactsWithNewTag(widget.options.clientId, createdTag);
                    });
                }
            });
        },

        updateSelectedContactsWithNewTag: function (clientId, createdTag) {
            $(document).trigger("getSelected", function (items) {
                $.each(items, function (index, contactId) {
                    var urltoSet = '/api/client/' + clientId + '/tag/' + createdTag.Id + '/contact/' + contactId;
                    $.ajax({
                        url: urltoSet,
                        type: 'PUT',
                        success: function (result) {
                            $(document).trigger("getContact", [contactId, function (contactObject) {
                                contactObject.Tags.push(createdTag);
                                $(document).trigger("contactChanged", contactObject);
                            }]);
                        }
                    });
                });
            });
        }
    });
}(jQuery));