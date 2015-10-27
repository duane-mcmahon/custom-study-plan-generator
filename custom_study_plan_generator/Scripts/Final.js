$(document).ready(function () {

    /* Dynamic sizing of background divs and table height */
    var widthBacker = $('#backerEdit').width();
    var widthText = $('#textEdit').width();
    $('#backerEdit').css("height", widthBacker * .50);
    $('#textEdit').css("height", widthText * .47);

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

    /* Create plan table inner divs for list of units received from controller */
    var count = 1;
    if (studentPlan != null) {
        studentPlan.forEach(function (entry) {

            if (entry != null) {
                var innerCellId = "#" + count;

                /* Create inner cell */
                if (entry != "" && entry.exempt == false) {
                    idCont = "#p" + count;

                    $(idCont).append("<div id = '" + count + "' class = 'innerCell active'</div>");
                    $(innerCellId).text(entry.name);

                }
            }
            count++
        });
    }

    $('#savePlan').click(function () {
        savePlan();
    });

});

function savePlan() {

    $('#error2').html("Saving, please wait...");
    $('#error2').show();
    $.ajax({
        url: "../Home/FinalSave",
        type: "POST",
        success: function (data) {
            $('#error2').html("Plan saved and uploaded");
            $('#error2').delay(5000).fadeOut('slow').css("color", "green");
            $('#error2').delay(5000).queue(function (next) {
                $(this).css("color", "red");
                next();
            });
        },
        error: function (data) {
            alert("Error saving plan" + data.responseText);
        }
    });
        
}
