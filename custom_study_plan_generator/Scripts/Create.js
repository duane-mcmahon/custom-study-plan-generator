$(document).ready(function () {

    /* Dynamic sizing of background divs and table height */
    var heightTable = $('#planTable').height();
    $('#textDefault').css("height", heightTable * 1.9);
    $('#backerDefault').css("height", heightTable * 2.1);

    var heightTopCell = $('#topCell').height();
    var heightp1 = $('#p1').height();

    $('#planTable').css("height", heightTopCell + (heightp1 * 4));

    var semesters = (numUnits / 4) | 0;

    if (semesters > 6) {
        var percentage = 96 / semesters;
        var percString = percentage + "%";
        $('.cell').css("width", percString);
        $('.planHeader').css("width", percString)
    }

    var count = 1;
    if (studentPlan != null) {
        studentPlan.forEach(function (entry) {

            var innerCellId = "#" + count;

            if (entry != "") {
                idCont = "#p" + count;

                $(idCont).append("<div id = '" + count + "' class = 'innerCell active' draggable = 'true' ondragstart = 'drag(event)' ondragend = 'dragend(event, this)'></div>");
                $(innerCellId).text(entry.name);

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
    var heightTable = $('#planTable').height();
    $('#textDefault').css("height", heightTable * 1.9);
    $('#backerDefault').css("height", heightTable * 2.1);

    var heightTopCell = $('#topCell').height();
    var heightp1 = $('#p1').height();

    $('#planTable').css("height", heightTopCell + (heightp1 * 4));
});