(function($) {
    $.fn.AudioPlayer = function(arg) {
        var Player = {
            element: null,
            isPlaying: false,
            isLoading: false,
            mediaDuration: 0,
            media: null
        }

        Player.element = $(this);

        var playerFunctions = {
            resetLayout: function () {
                Player.element.find('#time-spent').text(playerFunctions.formatTime(0));
                playerFunctions.updateSliderPosition(0);
                playerFunctions.updatePlayPauseIcon("fa-play");
            },

            playPause: function() {
                if (Player.isPlaying === false) {
                    Player.media.play();
                    Player.isPlaying = true;
                    playerFunctions.updatePlayPauseIcon("fa-pause");
                } else {
                    Player.media.pause();
                    Player.isPlaying = false;
                    playerFunctions.updatePlayPauseIcon("fa-play");
                }
            },

            updateSliderPosition: function (step) {
                var progress = Player.element.find('.progress-bar');

                if (step < progress.attr('aria-valuemin')) {
                    progress.width("0%");
                } else if (step > progress.attr('aria-valuemax')) {
                    progress.width("100%");
                } else {
                    progress.width(step + "%");
                }
            },

            seekPosition: function (seconds) {
                if (Player.media === null)
                    return;
                Player.media.currentTime = (seconds);
            },

            updateTimeText: function (seconds) {
                Player.element.find('#time-spent').text(playerFunctions.formatTime(seconds));

            },

            updatePlayPauseIcon: function (icon) {
                Player.element.find('.play-button i').removeClass().addClass('fa ' + icon);

            },

            setPlayPauseIconToLoadinf: function () {
                Player.element.find('.play-button i').removeClass().addClass('fa fa-spinner fa-spin ');

            },

            updateVolumeIcon: function (isMuted) {
                if (isMuted) {
                    Player.element.find('.volume-button i').removeClass().addClass('fa fa-volume-off');
                } else {
                    Player.element.find('.volume-button i').removeClass().addClass('fa fa-volume-up');
                }

            },

            enablePlayPauseIcon: function () {
                Player.element.find('.play-button button').prop("disabled", false);

            },

            disablePlayPauseIcon: function () {
                Player.element.find('.play-button button').prop("disabled", "disabled");

            },

            formatTime: function (milliseconds) {
                if (milliseconds <= 0)
                    return '00:00';

                var seconds = Math.round(milliseconds);
                var minutes = Math.floor(seconds / 60);
                if (minutes < 10)
                    minutes = '0' + minutes;

                seconds = seconds % 60;
                if (seconds < 10)
                    seconds = '0' + seconds;

                return minutes + ':' + seconds;
            },
        }

        Player.element.html('<div class="play-button" style="display: none;">' +
                                    '<button class="btn btn-info btn-inverted">' +
                                        '<i class="fa fa-play"></i>' +
                                    '</button>' +
                              '</div>' +
                              '<div class="start-button" >' +
                                    '<button class="btn btn-info btn-inverted">' +
                                        '<i class="fa fa-play"></i>' +
                                    '</button>' +
                              '</div>' +
                              '<div class="audio-seek">' +
                                    '<div class="progress">' +
                                        '<div class="progress-bar progress-bar-default progress-bar-striped active" role="progressbar" aria-valuemin="0" aria-valuemax="100" style="width: 0%">' +
                                            '<span class="sr-only"></span>' +
                                        '</div>' +
                                    '</div>' +
                                    '<span id="time-spent">00:00</span> <span id="total-time"> / ' + playerFunctions.formatTime(Player.element.data("duration")) + '</span>' +
                              '</div>' +
                              '<div class="volume-button">' +
                                    '<button class="btn btn-default btn-inverted" id="volume-button">' +
                                        '<i class="fa fa-volume-up"></i>' +
                                    '</button>' +
                              '</div>');

        if (arg == "mini") {
            Player.element.addClass("mini");
        }
        
        Player.media = new Audio();
        Player.media.src = Player.element.data("path");
        Player.media.preload = "none";
        Player.element.append(Player.media);
        
        Player.media.addEventListener('durationchange', function (event) {
            var duration = $.prop(this, 'duration');

            var durationIsMissing = !duration;
            var isInValidDuration = isNaN(duration) || !isFinite(duration);

            if (durationIsMissing || isInValidDuration) {
                return;
            }

            Player.mediaDuration = Math.round(duration);
            Player.element.find('#total-time').text(' / ' + playerFunctions.formatTime(duration));
            Player.element.find('#time-spent').text(playerFunctions.formatTime(0));
        });

        Player.media.addEventListener('timeupdate', function (event) {
            if (Player.element.find('#time-spent').text() !== playerFunctions.formatTime(event.target.currentTime)) {
                if (Player.isLoading) {
                    if (Player.media.paused) {
                        playerFunctions.updatePlayPauseIcon("fa-play");
                    } else {
                        playerFunctions.updatePlayPauseIcon("fa-pause");
                    }
                    Player.isLoading = false;
                }

                playerFunctions.enablePlayPauseIcon();
                playerFunctions.updateTimeText(event.target.currentTime);
                if (Player.isPlaying) {
                    playerFunctions.updateSliderPosition(Math.round(Math.round(event.target.currentTime) * 100 / Player.mediaDuration));
                }
            }
        });

        Player.media.addEventListener('ended', function () {
            playerFunctions.resetLayout();
            Player.isPlaying = false;
        });

        Player.element.find('.play-button').click(function (e) {
            e.preventDefault();
            e.stopImmediatePropagation();
            playerFunctions.playPause();

        });

        Player.element.find('.start-button').click(function (e) {
            e.preventDefault();
            e.stopImmediatePropagation();
            playerFunctions.playPause();

            Player.element.find('.play-button').show();
            Player.element.find('.start-button').hide();

            if (!Player.isLoading) {
                playerFunctions.setPlayPauseIconToLoadinf();
                playerFunctions.disablePlayPauseIcon();
                Player.isLoading = true;
            }

        });

        Player.element.find('.volume-button').click(function (e) {
            Player.media.muted = !Player.media.muted;
            playerFunctions.updateVolumeIcon(Player.media.muted);

        });

        Player.element.find('.progress').click(function (event) {

            if (!Player.isLoading) {
                playerFunctions.setPlayPauseIconToLoadinf();
                playerFunctions.disablePlayPauseIcon();
                Player.isLoading = true;
            }
            var progress = this;
            var clickPosition;
            if (event.offsetX === undefined) {
                clickPosition = event.originalEvent.layerX;
            } else {
                clickPosition = event.offsetX;
            }
            var position = Math.round(clickPosition * Player.mediaDuration / progress.offsetWidth);
            playerFunctions.seekPosition(position);
            var width = Math.round(Math.round(position) * 100 / Player.mediaDuration);
            playerFunctions.updateSliderPosition(width);
        });
    };
})(jQuery);
