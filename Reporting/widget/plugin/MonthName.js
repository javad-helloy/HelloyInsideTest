$.fn.ReportName = function (initNameStyle) {
    var element = $(this);
    if (initNameStyle == null) {
        element.text(moment().format('MMMM'));
    } else {
        element.text(moment().format(initNameStyle));
    }
    

    $(document).bind('reportName', function(event, eventData) {
        element.text(eventData);
    });
};