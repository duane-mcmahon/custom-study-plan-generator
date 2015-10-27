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
    if (studentPlan != null) {
        studentPlan.forEach(function (entry) {
            if (entry != "") {
                idCont = "#p" + count;
                jQuery('<div/>', {
                    id: count,
                    class: 'innerCell active',
                    draggable: 'true',
                    ondragstart: 'drag(event)',
                    ondragend: 'dragend(event, this)',
                    text: entry.name

                }).appendTo(idCont);
            }
            count++
        });
    }

    $('input[name=startSemester]').val("1");

    $('#startSemester').click(function () {
        if ($('input[name=startSemester]').val() == "1") {
            $('input[name=startSemester').val("2");
        }
        else {
            $('input[name=startSemester').val("1");
        }
    });

});

$(window).on('resize', function () {
    var widthBacker = $('#backerCP').width();
    var widthText = $('#textCP').width();
    $('#backerCP').css("height", widthBacker * .50);
    $('#textCP').css("height", widthText * .47);

    var heightTopCell = $('#topCell').height();
    var heightp1 = $('#p1').height();

    $('#planTable').css("height", heightTopCell + (heightp1 * 4));
});