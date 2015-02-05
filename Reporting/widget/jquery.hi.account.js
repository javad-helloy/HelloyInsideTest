(function($) {
    $.widget('hi.account', $.hi.reportwidget, {
        options:{
            receiveEmail: false,
            receiveSms: false,
            phoneNumber: null,
        },
        create: function () {
            var that = this;

            that.element.append('<header><h2 class="">Inställningar</h2></header>' +
                ' <section class="widget left-form">' +
                    '<div class="form-group">' +
                        '<div class="checkbox">'+
                            '<label>'+
                                '<input type="checkbox" id="receiveEmail"> Jag vill ha e-post' +
                            '</label>'+
                            '</div>' +
                    '</div>'+
                    '<div class="form-group">'+
                        '<div class="checkbox">'+
                            '<label>'+
                                '<input type="checkbox" id="receiveSms"> Jag vill ha SMS' +
                            '</label>'+
                            '</div>' +
                    '</div>'+
                    '<div class="form-group">'+
                        '<label for="Phone">Telefonnummer</label>'+
                        '<div>'+
                            '<input class="form-control" id="phone" placeholder="Telefonnummer" type="text">'+
                        '</div>'+
                    '</div>'+
                    '<div class="form-group" style="text-align: right;">'+
                        '<button type="button" class="btn btn-success" role="update-setting">Spara</button>'+
                    '</div>' +
                '</section>');
        },

        update: function() {
            var that = this;
            that.element.find('#receiveEmail').prop("checked", that.options.receiveEmail=="True");
            that.element.find('#receiveSms').prop("checked", that.options.receiveSms=="True");
            that.element.find('#phone').val(that.options.phoneNumber);
            
            that.element.find('button[role="update-setting"]').click(function() {
                var receiveEmail = $('#receiveEmail').prop("checked");
                var receiveSms = $('#receiveSms').prop("checked");
                var phoneNumber = $('#phone').val();
                var url = '/Sirius/UpdateSettings?' + $.param({
                    receiveEmail: receiveEmail,
                    receiveSms: receiveSms,
                    phone: phoneNumber
                });
                
                $.post(url, function (data) {
                    var alert = $(' <div class="alert alert-success role="alert">' +
                        '<button type="button" class="close" ><span aria-hidden="true">×</span><span class="sr-only">Close</span></button>' +
                        '<p>' + data.MessageText + '</p>' +
                        '</div>');
                    that.element.find('header').append(alert);
                    if (data.Type == "Success") {
                        
                    } else {
                        alert.removeClass('.alert-success').addClass('.alert-danger');
                    }
                    
                    alert.alert();
                    setTimeout(function () {
                        alert.alert('close');
                    }, 2000);

                }).fail(function() {

                });
            });
        }
    });
}(jQuery))