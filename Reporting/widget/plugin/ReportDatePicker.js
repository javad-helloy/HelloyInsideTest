$.fn.ReportDatePicker = function (pluginStyle) {
    var element = $(this);

    var datePick = "monthly";
    var menu;
    if (pluginStyle == "mini") {
        element.addClass('dropdown');
        menu = $('<span class="prev-date btn-icon-only btn-empty" style="cursor: pointer;"><i class="fa fa-chevron-left" style="padding-right: 10px;"></i></span>' +
                '<span class="next-date btn-icon-only btn-empty" style="cursor: pointer;"><i class="fa fa-chevron-right" style="padding-left: 10px; padding-right: 20px;"></i></span>' +
                '<a href="#" class="dropdown-toggle btn-icon-only btn-empty" id="dropdownMenu1" data-toggle="dropdown">' +
                    '<i class="fa fa-calendar"></i>' +
                '</a>' +
                '<ul class="dropdown-menu dropdown-menu-large right" role="menu" aria-labelledby="dropdownMenu1">' +
                    '<li class="current monthly" role="presentation"><a role="menuitem" tabindex="-1" data-text="Månad">Månad</a></li>' +
                    '<li class="divider" role="presentation"></li>' +
                    '<li class="yearly" role="presentation"><a role="menuitem" tabindex="-1" data-text="År">År</a></li>' +
                    '<li class="divider" role="presentation"></li>' +
                    '<li class="alltime" role="presentation"><a role="menuitem" tabindex="-1" data-text="Totalt">Totalt</a></li>' +
                '</ul>');
    } else {
        menu = $('<label class="prev-date btn btn-default active nav-btn">' +
                            '<span><i class="fa fa-chevron-left"></i></span>' +
                        '</label>' +
                        '<label class="btn btn-default btn-title active focus monthly" style="width:95px">' +
                            '<span class="month-name">Oktober</span>' +
                        '</label>' +
                        '<label class="next-date btn btn-default active nav-btn">' +
                            '<span><i class="fa fa-chevron-right"></i></span>' +
                        '</label>' +
                        '<label class="prev-date btn btn-default active nav-btn slide-out-left">' +
                            '<span><i class="fa fa-chevron-left"></i></span>' +
                        '</label>' +
                        '<label class="btn btn-default btn-title yearly">' +
                            '<span> År</span>' +
                        '</label>' +
                        '<label class="next-date btn btn-default active nav-btn slide-out-right">' +
                            '<span><i class="fa fa-chevron-right"></i></span>' +
                        '</label>' +
                        '<label class="btn btn-default btn-title alltime">' +
                            '<span> Totalt</span>' +
                        '</label>');
    }
   
    element.append(menu);

    element.find('.monthly .month-name').ReportName('MMMM');

    element.find('.prev-date').click(function (event) {
        
        if (datePick == "monthly") {
            $(document).trigger('prevMonth');
        }
        else if (datePick == "yearly") {
            $(document).trigger('prevYear');
        }
    });

    element.find('.next-date').click(function (event) {
        
        if (datePick == "monthly") {
            $(document).trigger('nextMonth');
        }
        else if (datePick == "yearly") {
            $(document).trigger('nextYear');
        }
    });

    var renderMenuForMonthlyReport = function () {
        datePick = "monthly";
        if (pluginStyle == "mini") {
            element.find('.menu-header-text').html('Månad ');
            element.find('.dropdown-menu li').removeClass('current');
            element.find('li.monthly').addClass('current');
        } else {
            var that = element.find('.btn.monthly');
            that.next('.prev-date.slide-out-left').after($('label.monthly'));

            element.find('.btn-title').removeClass('active');
            element.find('label.monthly').addClass('active');
            element.find('.btn-title').removeClass('focus');
            that.addClass('focus');

            element.find('.prev-date').addClass('slide-out-left');
            element.find('.next-date').addClass('slide-out-right');

            that.prev('.prev-date').removeClass('slide-out-left');
            that.next('.next-date').removeClass('slide-out-right');

            element.find('.btn.yearly').html('<span> År</span>');

            that.html('<span class="month-name"></span>');
        }
        
        element.find('.month-name').ReportName('MMMM');


    }
    $(document).bind("monthlyReport", renderMenuForMonthlyReport);

    var renderMenuForYearlyReport = function() {
        datePick = "yearly";

        if (pluginStyle == "mini") {
            element.find('.menu-header-text').html('År ');
            element.find('.dropdown-menu li').removeClass('current');
            element.find('li.yearly').addClass('current');
        } else {
            var that = element.find('.btn.yearly');
            element.find('.btn-title').removeClass('active');
            element.find('label.yearly').addClass('active');
            element.find('.btn-title').removeClass('focus');
            that.addClass('focus');

            element.find('.prev-date').addClass('slide-out-left');
            element.find('.next-date').addClass('slide-out-right');

            that.prev('.prev-date').removeClass('slide-out-left');
            that.next('.next-date').removeClass('slide-out-right');

            element.find('.prev-date.slide-out-left').before($('label.monthly'));

            element.find('.btn.monthly').html('<span> Månad</span>');

            that.html('<span class="year-name"></span>');
        }

        element.find('.year-name').ReportName('YYYY');
    }
    $(document).bind("yearlyReport", renderMenuForYearlyReport);

    var renderMenuForAllTimeReports = function () {
        datePick = "alltime";

        if (pluginStyle == "mini") {
            element.find('.menu-header-text').html('Alltid ');
            element.find('.dropdown-menu li').removeClass('current');
            element.find('li.alltime').addClass('current');
        } else {
            element.find('.btn-title').removeClass('active');
            element.find('label.alltime').addClass('active');
            element.find('.btn-title').removeClass('focus');

            element.find('.prev-date').addClass('slide-out-left');
            element.find('.next-date').addClass('slide-out-right');

            element.find('.prev-date.slide-out-left:first').before($('label.monthly'));

            element.find('.btn.yearly').html('<span> År</span>');
            element.find('.btn.monthly').html('<span> Månad</span>');
        }
        
    }
    $(document).bind("alltimeReport", renderMenuForAllTimeReports);

    element.find('.monthly').click(function (event) {
        $(document).trigger("monthlyReport");
    });

    element.find('.yearly').click(function (event) {
        $(document).trigger("yearlyReport");
    });

    element.find('.alltime').click(function (event) {
        $(document).trigger("alltimeReport");
    });

    
};