$.fn.StarRaiting = function () {
    var element = $(this);
    var clientId = element.data('client-id');
    var contactId = element.data('contact-id');
    var contact;
    $(document).trigger('getContact', [contactId, function (contactRecieved) {
        contact = contactRecieved;
        }
    ]);


    var ratingIterator = 0;
    var currentRaitingScore = parseInt(element.data("score"));
    var autoRatingScore = parseInt(element.data("auto-score"));
    var starsHtml = "";
    var ratingIcon;

    if (currentRaitingScore > 0) {
        ratingIcon = "icon-gold";
        ratingIterator = currentRaitingScore;
    } else {
        ratingIcon = "icon-gray";
        ratingIterator = autoRatingScore;
    }

    for (i = 0; i < 5; i++) {
        var starScore = i + 1;
        if (i < ratingIterator) {
            starsHtml += '<i data-score="' + starScore + '" class="fa fa-star ' + ratingIcon + '"></i>';
        } else {
            starsHtml += '<i data-score="' + starScore + '" class="fa fa-star icon-disable"></i>';
        }
    }
   
    

    var stars = $(starsHtml);
    stars.click(function (event) {
        event.stopImmediatePropagation();
        var score = $(this).data("score");
        var postData = {
            Type: 'RatingScore',
            Value: score
        }

        $.post("/api/client/" + clientId + "/lead/" + contactId + "/interaction", postData);
        contact.RatingScore = score;
        $(document).trigger('contactChanged', contact);
        
        // disable all
        $(this).parent().find('.fa-star').removeClass('icon-gold');
        $(this).parent().find('.fa-star').removeClass('icon-gray');
        $(this).parent().find('.fa-star').addClass('icon-disable');

        //activate this and previous
        var starToSetToActive = $(this);
        while (starToSetToActive.length > 0) {
            starToSetToActive.removeClass('icon-gray');
            starToSetToActive.addClass('icon-gold').removeClass('icon-disable');
            starToSetToActive = starToSetToActive.prev();
        }
    });
    element.html("");
    stars.appendTo(element);
};