$(document).ready(function () {

    $('#error4').delay(3000).fadeOut(2000);

    /* Dynamic sizing of background divs and table height */
    var heightTable = $('#planTable').height();
    $('#textEdit').css("height", heightTable * 1.9);
    $('#backerEdit').css("height", heightTable * 2.1);

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

    $('#home').click(function () {
        window.location.href = "../Home/Index";
    });

});

$(window).on('resize', function () {

    var heightTable = $('#planTable').height();
    $('#textEdit').css("height", heightTable * 2.7);
    $('#backerEdit').css("height", heightTable * 2.9);

    var heightTopCell = $('#topCell').height();
    var heightp1 = $('#p1').height();

    $('#planTable').css("height", heightTopCell + (heightp1 * 4));
});

function savePlan() {

    $('#error2').html("Saving, please wait...");
    $('#error2').show();
    $.ajax({
        url: "../Home/FinalSave",
        type: "POST",
        success: function (data) {

            if (data == "success") {
                planSaved = true;
                $('#error2').html("Plan saved");
                $('#error2').delay(5000).fadeOut('slow').css("color", "green");
                $('#error2').delay(5000).queue(function (next) {
                    $(this).css("color", "red");
                    next();
                });
            }
            else {
                $('#error2').html("Error saving plan, contact administrator");
                $('#error2').delay(5000).fadeOut('slow').css("color", "red");
                $('#error2').delay(5000).queue(function (next) {
                    $(this).css("color", "red");
                    next();
                });
            }

            
        },
        error: function (data) {
            alert("Error saving plan");
        }
    });
        
}
