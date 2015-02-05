$.fn.IndexScore = function () {
    var element = $(this);
    
    var currentIndexScore = parseInt(element.data("index-score"));
    var starsHtml = "";
    
    for (var i = 0; i < 10; i++) {
        if (i < currentIndexScore) {
            starsHtml += '<i class="fa fa-minus fa-rotate-90 icon-enable"></i>';
        } else {
            starsHtml += '<i class="fa fa-minus fa-rotate-90 icon-disable"></i>';
        }
    }
    var scores = $(starsHtml);
    element.html("");
    scores.appendTo(element);
};