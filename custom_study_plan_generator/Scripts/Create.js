$(document).ready(function () {

    /* Dynamic sizing of background divs and table height */
    var widthBacker = $('#backerCP').width();
    var widthText = $('#textCP').width();
    $('#backerCP').css("height", widthBacker * .50);
    $('#textCP').css("height", widthText * .47);

    var heightTopCell = $('#topCell').height();
    var heightp1 = $('#p1').height();

    $('#planTable').css("height", heightTopCell + (heightp1 * 4));

    if (numUnits == 32) {
        $('.cell').css("width", "12%");
        $('.planHeader').css("width", "12%")
    }

    var count = 1;
    unitListSelected.forEach(function (entry) {
        var id = "#" + count;
        $(id).html(entry);
        count++
    });
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