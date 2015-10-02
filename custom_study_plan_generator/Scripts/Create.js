$(document).ready(function () {

    /* Dynamic sizing of background divs and table height */
    var widthBacker = $('#backerCP').width();
    var widthText = $('#textCP').width();
    $('#backerCP').css("height", widthBacker * .50);
    $('#textCP').css("height", widthText * .47);

    var heightTopCell = $('#topCell').height();
    var heightp1 = $('#p1').height();

    $('#planTable').css("height", heightTopCell + (heightp1 * 4));
});

$(window).on('resize', function () {
    alert("hello");
    var widthBacker = $('#backerCP').width();
    var widthText = $('#textCP').width();
    $('#backerCP').css("height", widthBacker * .50);
    $('#textCP').css("height", widthText * .47);

    var heightTopCell = $('#topCell').height();
    var heightp1 = $('#p1').height();

    $('#planTable').css("height", heightTopCell + (heightp1 * 4));
});