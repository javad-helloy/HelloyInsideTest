(function($) {

    $.fn.ReportFilter = function (callback) {

        var element = $(this);
        var headerText ="";

        var getParameterValueByName = function (name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }

        var status = getParameterValueByName("status");
        if (status != null && status === 'new') {
            headerText = "Nya Kontakter";
        }
        if (status != null && status === 'Övrigt') {
            headerText = "Övrigt";
        }
        if (status != null && status === 'Affär') {
            headerText = "Affär";
        }
        if (status != null && status === 'Pot. Kund') {
            headerText = "Pot. Kund";
        }

        var rating = getParameterValueByName("rating");
        if (rating != null && rating.toLowerCase() == "lead") {
            headerText = "Lead";
        }
        if (rating != null && rating.toLowerCase() == "ej lead") {
            headerText = "Ej Lead";
        }

        var type = getParameterValueByName("type");
        if (type != null && type.toLowerCase() == "phone") {
            headerText = "Telefon";
        }
        if (type != null && type.toLowerCase() == "email") {
            headerText = "E-post";
        }
        if (type != null && type.toLowerCase() == "chat") {
            headerText = "Chat";
        }
        if (type != null && type.toLowerCase() == "event") {
            headerText = "Event";
        }

        var product = getParameterValueByName("product");
        if (product != null && product.toLowerCase() == "search") {
            headerText = "Search";
        }
        if (product != null && product.toLowerCase() == "display") {
            headerText = "Display";
        }
        if (product != null && product.toLowerCase() == "retargeting") {
            headerText = "Retargeting";
        }
        if (product != null && product.toLowerCase() == "organic") {
            headerText = "Organic";
        }

        var tagName = getParameterValueByName("tagName");
        if (tagName != null && tagName!="") {
            headerText = tagName;
        }

        if (headerText == "") {
            headerText = "Alla Kontakter";
        }

        element.find('span.menu-header-text').text(headerText);
        element.find('.dropdown-menu li').removeClass('current');
        element.find('.dropdown-menu a[data-text="' + headerText + '"]').parent().addClass('current');

        if (callback !== undefined) {
            callback(headerText);
        }
    };
}(jQuery));
