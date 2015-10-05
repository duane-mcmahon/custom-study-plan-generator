$(document).ready(function () {
    /* Set main area of window to have a minimum width that will allow the footer to sit at the bottom of the page */
    var header = $('#header').outerHeight();
    var footer = $('#footer').outerHeight();
    var windowHeight = $(window).height();
    $('#CRUDMain').css("min-height", windowHeight - header - footer);

    $('#CRUDHeadingHolder').css("margin-left", $('#table').css("margin-left"));
});