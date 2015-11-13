$(document).ready(function () {

    var loginFormPosition = $('#textLogin').outerWidth() - $('#loginForm').width();
    var tempLoginPosition = $('#textLogin').outerWidth() - $('#tempButton').width();
    var margin = 10;

    $('#loginForm').css("left", loginFormPosition / 2);
    $('#loginForm').css("bottom", margin + "px");

    $('#tempButton').css("left", tempLoginPosition / 2);
    $('#tempButton').css("bottom", $('#CAS').height() + margin + margin);

});

$(window).on('resize', function () {

    var loginFormPosition = $('#textLogin').outerWidth() - $('#loginForm').width();
    var tempLoginPosition = $('#textLogin').outerWidth() - $('#tempButton').width();
    var margin = 10;

    $('#loginForm').css("left", loginFormPosition / 2);
    $('#loginForm').css("bottom", margin + "px");

    $('#tempButton').css("left", tempLoginPosition / 2);
    $('#tempButton').css("bottom", $('#CAS').height() + margin + margin);

});