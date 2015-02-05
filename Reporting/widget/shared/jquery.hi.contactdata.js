(function ($) {
    $.widget('hi.contactdata', $.hi.reportwidget, {
        contacts : null,
        nextPage: null,

        options: {
            clinetId: null,
            filterNew: false,
            type: null,
            filterLabel: null,
            product: null,
            rating: null,
            status: null,
            tagName: null
        },

        create: function () {
            var that = this;

            var status = that.getParameterByName("status");
            if (status != null && status === 'new') {
                that.options.filterNew = true;
            }
            if (status != null && status !== "" && status !== "new") {
                that.options.status = status;
            }
            
            var rating = that.getParameterByName("rating");
            if (rating != null && rating !== "") {
                that.options.rating = rating;
            }

            var type = that.getParameterByName("type");
            if (type != null && type !== "") {
                that.options.type = type;
            }

            var product = that.getParameterByName("product");
            if (product != null && product !== "") {
                that.options.product = product;
            }

            var tagName = that.getParameterByName("tagName");
            if (tagName != null && tagName !== "") {
                that.options.tagName = tagName;
            }

            
            $(document).bind('fetchMoreContacts', function (event, callback) {
                that.fetchMoreContacts(callback);
            });

            $(document).bind('getContacts', function (event, callback) {
                if (that.contacts != null) {
                    callback(that.contacts);
                }
            });

            $(document).bind('getContact', function (event, contactId, callback, failCallBack) {
                var contact = _.find(that.contacts, function (contact) { return contact.Id == contactId; });

                if (contact === undefined || contact === null) {
                    that.fetchContactById(contactId, function (contactLoaded) { callback(contactLoaded); }, failCallBack);
                } else {
                    callback(contact);
                }
            });

            $(document).bind('getNextContact', function (event, contactId, callback) {
                var contacIndex;
                var nextContact;
                _.find(that.contacts, function (contact, index) {
                    if (contact.Id == contactId) { contacIndex = index; return true; };
                });

                if (contacIndex != that.contacts.length - 1) {
                    nextContact = that.contacts[contacIndex + 1];
                    callback(nextContact);
                } else {
                    $(document).trigger('fetchMoreContacts', function (contacts) {
                        if (contacts != null) {
                            $(document).trigger('getNextContact', [contactId,  callback]);
                        } else {
                            callback(null);
                        }
                    });
                }

                
            });

            $(document).bind('getPreviousContact', function (event, contactId, callback) {
                var contacIndex;
                var prevContact;
                _.find(that.contacts, function(contact, index) {
                    if (contact.Id == contactId) {
                        contacIndex = index;
                        return true;
                    };
                });

                if (contacIndex != 0) {
                    prevContact = that.contacts[contacIndex - 1];
                    callback(prevContact);
                } else {
                    callback(null);

                }
            });

            $(document).bind('contactChanged', function (event, contact) {
                var contacIndex;
                _.find(that.contacts, function (oldContact, index) {
                    if (oldContact.Id == contact.Id) { contacIndex = index; return true; };
                });

                that.contacts[contacIndex] = contact;
            });
        },

        update: function () {
            var that = this;
            if (that.nextPage == null) {
                var query = {};

                if (that.options.filterNew) {
                    query.filterNew = that.options.filterNew;
                }

                if (that.options.type != null) {
                    query.type = that.options.type;
                }

                if (that.options.product != null) {
                    query.product = that.options.product;
                }

                if (that.options.rating != null) {
                    query.rating = that.options.rating;
                }

                if (that.options.status != null) {
                    query.status = that.options.status;
                }

                if (that.options.tagName != null) {
                    query.tagName = that.options.tagName;
                }

                that.nextPage = '/api/client/' + that.options.clientId + '/contact?'+$.param(query);
            }

            that.fetchMoreContacts();
        },

        fetchMoreContacts: function (callback) {
            var that = this;
            $.getJSON(that.nextPage, function (data) {
                if (that.contacts == null) {
                    that.contacts = [];
                }

                that.nextPage = data.nextPageLink;

                that.contacts = that.contacts.concat(data.results);

                $(document).trigger('moreContacts', { contacts: data.results });
                
                if (callback !== undefined) {
                    callback(data.results);
                }
            });
        },

        fetchContactById: function (contactId, callback, failCallBack) {
            var that = this;
            var url = '/api/client/' + that.options.clientId + '/contact/' + contactId;
            $.getJSON(url, function (data) {
                if (that.contacts == null) {
                    that.contacts = [];
                }

                that.contacts = that.contacts.concat(data);
                
                if (callback !== undefined) {
                    callback(data);
                }
            }).error(function() {
                if (failCallBack !== undefined) {
                    failCallBack();
                }
            });
        },

        destroy: function () {
            $.Widget.prototype.destroy.apply(this, arguments);
        },

        getParameterByName: function (name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }

    });
}(jQuery));
