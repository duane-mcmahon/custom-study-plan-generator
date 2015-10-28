var dragParentId;
var preventProgress = false;

$(document).ready(function () {

    /* Dynamic sizing of background divs and table height */
    var widthBacker = $('#backerEdit').width();
    var widthText = $('#textEdit').width();
    $('#backerEdit').css("height", widthBacker * .75);
    $('#textEdit').css("height", widthText * .73);

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
                if (entry != "") {
                    idCont = "#p" + count;

                    $(idCont).append("<div id = '" + count + "' class = 'innerCell active' draggable = 'true' ondragstart = 'drag(event)' ondragend = 'dragend(event, this)'></div>");
                    $(innerCellId).text(entry);

                    /* Create hover icon */
                    var hoverId = 'hover' + count;
                    $(innerCellId).append("<img id = '" + hoverId + "' class = 'hoverModify' src = '../Content/Images/hover.png' />");

                    /* Create prevent icon */
                    var preventId = 'prevent' + count;
                    $(innerCellId).append("<img id = '" + preventId + "' class = 'prevent' src = '../Content/Images/prevent.png' />");
                }

                /* Create prereq violated exclamation mark icon where required */
                for (var x = 0; x < violatedListConverted.length; x++) {
                    if (entry == violatedListConverted[x]) {
                        var exclamationId = 'exclamation' + count;
                        $(innerCellId).append("<img id = '" + exclamationId + "' class = 'exclamation' src = '../Content/Images/exclamation.png' />");
                    }
                }
            }
            count++
        });
    }

    /* Create swap space inner divs for list of units received from controller */
    var count = 1;
    if (studentPlanSwap != null) {
        studentPlanSwap.forEach(function (entry) {

            if (entry != null) {
                var id = "id" + Math.random().toString(16).slice(2)
                var innerCellId = "#" + id;

                /* Create inner cell */
                if (entry != "") {
                    idCont = "#ss" + count;

                    $(idCont).append("<div id = '" + id + "' class = 'innerCell active' draggable = 'true' ondragstart = 'drag(event)' ondragend = 'dragend(event, this)'></div>");
                    $(innerCellId).text(entry);

                    /* Create hover icon */
                    var hoverId = 'hover' + count;
                    $(innerCellId).append("<img id = '" + hoverId + "' class = 'hover' src = '../Content/Images/hover.png' />");

                    /* Create prevent icon */
                    var preventId = 'prevent' + count;
                    $(innerCellId).append("<img id = '" + preventId + "' class = 'prevent' src = '../Content/Images/prevent.png' />");
                }

            }
            count++;

        });
    }


    $('#checkPrereqs').click(function () {

        /* Refresh the page after waiting for ajax responses */
        $('#error2').html("Checking, please wait...");
        $('#error2').show();
        setTimeout(checkVariable, 1000);

    });

    $('#resetChanges').click(function () {

        $('.prevent').css("display", "block");
        $('.innerCell').attr("draggable", "false");
        preventProgress = true;

        $('#error2').html("Resetting, please wait...");
        $('#error2').show();
        

        $.ajax({
            url: "../Home/EditReset",
            type: "POST",
            success: function (data) {
                $('.innerCell').attr("draggable", "true");
                $('.prevent').css("display", "none");
                preventProgress = false;
                window.location.reload();
            },
            error: function (data) {
              
            }
        });
    });

    $('#savePlan').click(function () {
        savePlan();
    });

    tooltip(".hoverModify", "tooltip");

});

$(window).on('resize', function () {
    var widthBacker = $('#backerEdit').width();
    var widthText = $('#textEdit').width();
    $('#backerEdit').css("height", widthBacker * .75);
    $('#textEdit').css("height", widthText * .73);

    var heightTopCell = $('#topCell').height();
    var heightp1 = $('#p1').height();

    $('#planTable').css("height", heightTopCell + (heightp1 * 4));
});


function allowDrop(ev) {

    ev.preventDefault();
}

function drag(ev) {

    ev.dataTransfer.setData("text", ev.target.id);
    $(document.getElementById(ev.target.id).parentElement).addClass("possibleTarget");

    /* Set the global variable dragParentId as the location of where the lement is dragged FROM */
    dragParentId = ev.target.parentElement.id;

}

function drop(ev, target) {

    if ($(target).hasClass("target")) {
        ev.preventDefault();
        var data = ev.dataTransfer.getData("text");
        ev.target.appendChild(document.getElementById(data));

        $(document.getElementById(data).parentElement).removeAttr("ondragover");
        $(document.getElementById(data).parentElement).removeAttr("ondrop");

        var elements = document.getElementsByClassName("possibleTarget");
        for (var x = 0; x < elements.length; x++) {
            $(elements[x]).attr({ ondragover: "allowDrop(event)", ondrop: "drop(event, this)" });
            $(elements[x]).addClass("target");
        }

    }

    else {

    }
}

function dragend(ev, target) {

    /* Remove all previous possible targets */
    var elements = document.getElementsByClassName("possibleTarget");

    for (var x = 0; x < elements.length; x++) {
        $(elements[x]).removeClass("possibleTarget");
    }

    /* Hide any previous errors */
    $('#errors').hide();

    /* Drag successful */
    if (ev.dataTransfer.dropEffect == 'move') {

        $('.prevent').css("display", "block");
        $('.innerCell').attr("draggable", "false");
        preventProgress = true;

        /* If moved TO the swap space, remove unit from session plan */
        if ($(target).parent().hasClass('swapSpaceCell')) {

            var idRawFrom = dragParentId;
            var idStringFrom = idRawFrom.toString();
            var idFrom = idStringFrom.substring(1);

            var dataRemove = idFrom;

            var idRawTo = $(target).parent().attr("id");
            var idStringTo = idRawTo.toString();
            var idTo = idStringTo.substring(2);

            dataRemove += "," + idTo;

            $.ajax({
                url: "../Home/EditRemove",
                type: "POST",
                data: { data: dataRemove },
                success: function (data) {
                    $('.innerCell').attr("draggable", "true");
                    $('.prevent').css("display", "none");
                    preventProgress = false;
                },
                error: function (data) {
                    alert("Error removing unit, please refresh the page.");
                }
            });

        }

            /* If moved TO the plan, add unit to session plan */
        else if ($(target).parent().hasClass('planCell')) {

            var idRawFrom = dragParentId;
            var idStringFrom = idRawFrom.toString();
            var idFrom = idStringFrom.substring(2);

            var dataAdd = idFrom;

            var idRawTo = $(target).parent().attr('id');
            var idStringTo = idRawTo.toString();
            var idTo = idStringTo.substring(1);

            dataAdd += "," + idTo;

            $.ajax({
                url: "../Home/EditAdd",
                type: "POST",
                data: { data: dataAdd },
                success: function (data) {
                    $('.innerCell').attr("draggable", "true");
                    $('.prevent').css("display", "none");
                    preventProgress = false;
                },
                error: function (data) {
                    alert("Error adding unit, please refresh the page.");
                }
            });

        }

    }
        /* Drag failed */
    else {

    }

}

function checkVariable() {
    if (preventProgress == false) {
        window.location.reload();
    }
    else setTimeout(checkVariable, 1000);
}

/* Tooltip function */
function tooltip(target, name) {

    /* Loop through all targets */
    $(target).each(function (i) {


        /* Create the id and the jquery readable id */
        var id = name + i;
        var jId = '#' + id;

        /* Create the tootlip element */
        $("body").append("<div class='" + name + "' id='" + id + "'><p>" + "Loading..." + "</p></div>");
        var my_tooltip = $("#" + name + i);

        /* Set the mousover/mouseout effects */
        $(this).removeAttr("title").mouseover(function () {
            my_tooltip.css({ opacity: 0.8, display: "none" }).fadeIn(400);
        }).mousemove(function (kmouse) {
            my_tooltip.css({ left: kmouse.pageX + 15, top: kmouse.pageY + 15 });
        }).mouseout(function () {
            my_tooltip.fadeOut(0);
        });

        /* prepare the unit data to send to ajax */
        data = $(this).parent().text();

        $.ajax({
            url: "../Home/GetPrerequisites",
            type: "POST",
            data: { data: data },
            success: function (data) {

                /* If no prereqs */
                if (data.length == 0) {
                    $(jId).text("Prerequsites: None");
                }
                    /* If prereqs */
                else {
                    $(jId).text("Prerequsites: " + data);
                }


            },
            error: function (data) {
                alert(data.responseText);
            }
        });


    });
}

function savePlan() {

    $('#error2').html("Saving, please wait...");
    $('#error2').show();
    $.ajax({
        url: "../Home/EditSave",
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