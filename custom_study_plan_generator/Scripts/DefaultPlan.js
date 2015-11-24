var TOTAL_SWAP_SPACES = 12;
var dragParentId;
var preventProgress = false;

$(document).ready(function () {
    // Hide Modal Window Errors.
    $('#dialog-unitsnotadded').hide();
    $('#dialog-confirm').hide();

    var lastValidUnitSelections = null;

    /* Dynamic sizing of background divs and table height */
    var heightTable = $('#planTable').height();
    $('#textDefault').css("height", heightTable * 3.6);
    $('#backerDefault').css("height", heightTable * 3.8);

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
    if (unitListSelected != null) {
        unitListSelected.forEach(function (entry) {

            var innerCellId = "#" + count;

            /* Create inner cell */
            if (entry != "" && entry != null) {
                idCont = "#p" + count;

                $(idCont).append("<div id = '" + count + "' class = 'innerCell active' draggable = 'true' ondragstart = 'drag(event)' ondragend = 'dragend(event, this)'></div>");
                $(innerCellId).text(entry);

                /* Create hover icon */
                var hoverId = 'hover' + count;
                $(innerCellId).append("<img id = '" + hoverId + "' class = 'hover' src = '../Content/Images/hover.png' draggable = 'false' />");

                /* Create prevent icon */
                var preventId = 'prevent' + count;
                $(innerCellId).append("<img id = '" + preventId + "' class = 'prevent' src = '../Content/Images/prevent.png' draggable = 'false' />");

                /* Create delete icon */
                var deleteId = 'delete' + count;
                $(innerCellId).append("<img id = '" + deleteId + "' class = 'delete' src = '../Content/Images/delete.png' draggable = 'false' />");

            }

            /* Create prereq violated exclamation mark icon where required */
            for (var x = 0; x < violatedListConverted.length; x++) {
                if (entry == violatedListConverted[x]) {
                    var exclamationId = 'exclamation' + count;
                    $(innerCellId).append("<img id = '" + exclamationId + "' class = 'exclamation' src = '../Content/Images/exclamation.png' draggable = 'false' />");
                }
            }
            count++
        });
    }

    /* Create swap space inner divs for list of units received from controller */
    var count = 1;
    if (defaultPlanSwap != null) {
        defaultPlanSwap.forEach(function (entry) {

            if (entry != null) {
                var id = "id" + Math.random().toString(16).slice(2)
                var innerCellId = "#" + id;

                // Create inner cell.
                if (entry != "") {
                    idCont = "#ss" + count;

                    $(idCont).append("<div id = '" + id + "' class = 'innerCell active' draggable = 'true' ondragstart = 'drag(event)' ondragend = 'dragend(event, this)'></div>");
                    $(innerCellId).text(entry);

                    // Create hover icon.
                    var hoverId = 'hover' + count;
                    $(innerCellId).append("<img id = '" + hoverId + "' class = 'hover' src = '../Content/Images/hover.png' draggable = 'false' />");

                    // Create prevent icon.
                    var preventId = 'prevent' + count;
                    $(innerCellId).append("<img id = '" + preventId + "' class = 'prevent' src = '../Content/Images/prevent.png' draggable = 'false' />");

                    // Create delete icon.
                    var deleteId = 'delete' + count;
                    $(innerCellId).append("<img id = '" + deleteId + "' class = 'delete' src = '../Content/Images/delete.png' draggable = 'false' />");
                }

            }
            count++;

        });
    }

    /* Delete a unit from the page */
    $('.delete').click(function () {
        deleteInnerCell(this);
    });

    // Set Limitation on the MultiSelect Box, so that the user cannot select more Units than the available Swap Space.
    $('#unitDropDown').click(function (event) {
        // Check current capacity of Swap Space.
        var availableSpaces = 0;
        var swapSpaces = $(".swapSpaceCell");

        // Count available spaces.
        for (var x = 0; x < swapSpaces.length; x++) {
            if ($(swapSpaces[x]).children().length == 0) {
                availableSpaces++;
            }
        }

        // Compare available space against the number of selected Units.
        if ($(this).val().length > availableSpaces) {
            // Too many units selected - not enough space available. Revert to previous selections, display an error to the user.
            $(this).val(lastValidUnitSelections);
            $('#errors').html("Error, too many units selected.");
            $('#errors').show();
            $('#errors').delay(5000).fadeOut('slow');
        }
        else {
            // Valid selections - update the list of currently selected Units.
            lastValidUnitSelections = $(this).val();
        }
    });

    /* Reset Swap Space. */
    $('#resetSwapSpace').click(function () {

        // Reset the Swap Space Session variable.
        $.ajax({
            url: "../Home/DefaultPlanResetSwapSpace",
            type: "POST",
            success: function () {
                location.reload();
            },
            error: function (data) {

            }
        });

    });

    /* Add a unit to the swap space */
    $('#addUnit').click(function () {

        $('#errors').hide();

        var idAString;
        var idBString;
        var ssA;
        var ssB;

        // Get all the parent cells in the swap space and sort them by ID.
        var elements = $('.swapSpaceCell').sort(function (a, b) {
            idAString = (a.id).toString();
            idBString = (b.id).toString();
            ssA = idAString.substring(2);
            ssB = idBString.substring(2);
            return parseInt(ssA) - parseInt(ssB);
        })

        var units = [];
        var duplicateUnits = [];
        var validUnits = [];
        var excessUnits = [];
        var finalUnits = [];
        var countSelected = 0;
        var swapSpacesFull = 0;
        var countValid = 0;
        var countDuplicates = 0;

        // Retrieve all selected units from the Multiselect List.
        $('#unitDropDown :selected').each(function (i, selected) {
            units[countSelected] = $(selected).text();
            countSelected++;
        });

        // Check current capacity of Swap Space.
        for (var x = 0; x < elements.length; x++) {
            if ($(elements[x]).children().length != 0) {
                swapSpacesFull++;
            }
        }

        // Check Units have been slected to add to the Plan.
        if (units.length == 0) {
            // User did not select any units to add - display error message.
            $('#errors').html("Please select a unit to add");
            $('#errors').show();
        }
        else if (swapSpacesFull == TOTAL_SWAP_SPACES) {
            // If the swap space was full, give an error.
            $('#errors').html("Error, swap space is full. Please clear some space first.");
            $('#errors').show();
        }
        else {
            // Proceed if Valid units are selected.
            if (units.length != 0) {
                // Check how many units can fit into the available space.
                var excess = 0;
                var availableSwapSpace = 0;
                availableSwapSpace = (TOTAL_SWAP_SPACES - swapSpacesFull);

                // Make list of units that aren't going to fit.
                if (units.length > availableSwapSpace) {
                    // Find position of first excess unit.
                    excess = (units.length - availableSwapSpace);

                    // Loop from start of excess to end of selected units.
                    for (var x = 0; x < excess; x++) {
                        var pos = (availableSwapSpace + x);
                        excessUnits[x] = units[pos];
                    }
                }

                // Find final number of valid units that will fit.
                var finalLength = (units.length - excess);

                // Create final list of valid units to be Added, minus the duplicates and any excess.
                for (var x = 0; x < finalLength; x++) {
                    finalUnits[x] = units[x];
                }

                // Create list of available blank spaces in the Swap Space.
                var spaces = [];
                var countSpaces = 0;

                for (var x = 0; x < elements.length; x++) {
                    if ($(elements[x]).children().length == 0) {
                        spaces[countSpaces] = x;
                        countSpaces++;
                    }
                }

                // Data string to pass via AJAX to the Controller.
                var units = "";

                // Loop through the valid final units and add them into empty Swap Spaces.
                for (var y = 0; y < finalUnits.length; y++) {
                    var id = "id" + Math.random().toString(16).slice(2)
                    var innerCellId = "#" + id;

                    $(elements[spaces[y]]).append("<div id = '" + id + "' class = 'innerCell active' draggable = 'true' ondragstart = 'drag(event)' ondragend = 'dragend(event, this)'></div>");
                    $(innerCellId).text(finalUnits[y]);

                    // Add unit to Data string.
                    units += spaces[y] + "," + finalUnits[y];

                    // Add second delimiter between units if it is not the last unit.
                    if (y != (finalUnits.length - 1)) {
                        units += "|";
                    }

                    // Create hover icon.
                    var hoverId = 'hover' + id;
                    $(innerCellId).append("<img id = '" + hoverId + "' class = 'hover' src = '../Content/Images/hover.png' draggable = 'false' />");
                    var jId = "#" + hoverId;
                    tooltip(jId, "tooltip");

                    // Create delete icon.
                    var deleteId = 'delete' + id;
                    $(innerCellId).append("<img id = '" + deleteId + "' class = 'delete' src = '../Content/Images/delete.png' draggable = 'false' />");

                    // Reset the delete cell click function to include this unit.
                    $('.delete').click(function () {
                        deleteInnerCell(this);
                    });
                }

                // Add the units to the session variable for the Swap Space List.
                $.ajax({
                    url: "../Home/DefaultPlanBulkAddToSwapSpace",
                    type: "POST",
                    data: { data: units },
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

        // Show error message if there were duplicates or excess units that could not fit into Swap Space.
        if (excessUnits.length > 0) {
            // Reset any previous messages.
            $('.excess').hide();

            // Add Excess units to message window.
            if (excessUnits.length > 0) {
                var excessMsg = "";

                for (var x = 0; x < excessUnits.length; x++) {
                    var listItem = "<li>" + excessUnits[x] + "</li>"
                    excessMsg += listItem;
                }

                $('#excess').html(excessMsg);
                $('.excess').show();
            }

            // Display the Error Message.
            $('.unitsNotAdded').show();

            $(function () {
                $("#dialog-unitsnotadded").dialog({
                    resizable: false,
                    height: 400,
                    width: 400,
                    modal: true,
                    buttons: {
                        Okay: function () {
                            $(this).dialog("close");
                        }
                    }
                });
            });
        }

        // Deselect all items in the MultiSelectList.
        $('#unitDropDown option').removeAttr("selected");
    });

    $('#savePlan').click(function () {
        savePlan();
    });

    $('#resetPlan').click(function () {

        /* Plan is full */
        $.ajax({
            url: "../Home/DefaultPlanReset",
            type: "POST",
            success: function () {
                location.reload();
            },
            error: function (data) {

            }
        });

    });

    $('#checkPrereqs').click(function () {

        /* Refresh the page after waiting for ajax responses */
        $('#error2').html("Checking, please wait...");
        $('#error2').show();
        setTimeout(checkVariable, 1000);

    });

    if (courseSelectedConverted == true) {
        tooltip(".hover", "tooltip");
    }

});

$(window).on('resize', function () {

    /* Dynamic sizing of background divs and table height */
    var heightTable = $('#planTable').height();
    $('#textDefault').css("height", heightTable * 3.1);
    $('#backerDefault').css("height", heightTable * 3.3);

    var heightTopCell = $('#topCell').height();
    var heightp1 = $('#p1').height();

    $('#planTable').css("height", heightTopCell + (heightp1 * 4));
});

function allowDrop(ev) {

    ev.preventDefault();
    if ($(ev.target).children().length > 0)
        ev.dataTransfer.dropEffect = "none"; // dropping is not allowed
    else if ($(ev.target).hasClass('hover'))
        ev.dataTransfer.dropEffect = "none"; // dropping is not allowed
    else if ($(ev.target).hasClass('delete'))
        ev.dataTransfer.dropEffect = "none"; // dropping is not allowed
    else if ($(ev.target).hasClass('exclamation'))
        ev.dataTransfer.dropEffect = "none"; // dropping is not allowed
    else if ($(ev.target).hasClass('prevent'))
        ev.dataTransfer.dropEffect = "none"; // dropping is not allowed
    else
        ev.dataTransfer.dropEffect = "all"; // drop

}

/* This funtions runs when the drag begins */
function drag(ev) {

    /* Set the element being hovered over as a possible drag target */
    ev.dataTransfer.setData("text", ev.target.id);
    $(document.getElementById(ev.target.id).parentElement).addClass("possibleTarget");

    /* Set the global variable dragParentId as the location of where the lement is dragged FROM */
    dragParentId = ev.target.parentElement.id;

}

/* This function runs when the dragged element is dropped */
function drop(ev, target) {

    /* If the drag target is a valid drop location */
    if ($(target).hasClass("target")) {

        /* Append the dragged element to the new location */
        ev.preventDefault();
        var data = ev.dataTransfer.getData("text");
        ev.target.appendChild(document.getElementById(data));

        /* Remove the target drag functions from the new parent, making it no longer a valid target */
        $(document.getElementById(data).parentElement).removeAttr("ondragover");
        $(document.getElementById(data).parentElement).removeAttr("ondrop");

        /* Get all the previous possible targets, and make them valid targets again */
        var elements = document.getElementsByClassName("possibleTarget");
        for (var x = 0; x < elements.length; x++) {
            $(elements[x]).attr({ ondragover: "allowDrop(event)", ondrop: "drop(event, this)" });
            $(elements[x]).addClass("target");
        }

    }

    else {

    }

}

/* this function runs after drag completion, whether it was successful or not */
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

        var PLAN_CELL = 'p';
        var SWAP_CELL = 's';


        /* If moved TO the swap space, remove unit from session plan */
        if ($(target).parent().hasClass('swapSpaceCell')) {

            var idRawFrom = dragParentId;

            if (idRawFrom.indexOf("ss") > -1) {
                var idStringFrom = idRawFrom.toString();
                var idFrom = idStringFrom.substring(2);
                var fromSwap = true;
            }
            else {
                var idStringFrom = idRawFrom.toString();
                var idFrom = idStringFrom.substring(1);
                var fromSwap = false;
            }

            var dataRemove = idFrom;

            var idRawTo = $(target).parent().attr("id");
            var idStringTo = idRawTo.toString();
            var idTo = idStringTo.substring(2);

            dataRemove += "," + idTo;

            if (fromSwap == true) {
                dataRemove += "," + "fromSwap";
            }
            else {
                dataRemove += "," + "fromPlan";
            }

            $.ajax({
                url: "../Home/DefaultPlanRemove",
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

            // If moved TO the plan, add unit to session plan.
        else if ($(target).parent().hasClass('planCell')) {

            var idRawFrom = dragParentId;
            if (idRawFrom.indexOf("p") > -1) {
                var idStringFrom = idRawFrom.toString();
                var idFrom = idStringFrom.substring(1);
                var fromPlan = true;
            }
            else {
                var idStringFrom = idRawFrom.toString();
                var idFrom = idStringFrom.substring(2);
                var fromPlan = false;
            }

            var dataAdd = idFrom;

            var idRawTo = $(target).parent().attr('id');
            var idStringTo = idRawTo.toString();
            var idTo = idStringTo.substring(1);

            dataAdd += "," + idTo;

            if (fromPlan == true) {
                dataAdd += "," + "fromPlan";
            }
            else {
                dataAdd += "," + "fromSwap";
            }

            $.ajax({
                url: "../Home/DefaultPlanAdd",
                type: "POST",
                data: { data: dataAdd },
                success: function (data) {
                    $('.innerCell').attr("draggable", "true");
                    $('.prevent').css("display", "none");
                    preventProgress = false;
                },
                error: function (data) {
                    alert("Error adding unit, please refresh the page." + data.responseText);
                }
            });

        }

      

    }
        /* Drag failed */
    else {

    }

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
                    $(jId).text(data);
                }


            },
            error: function (data) {

            }
        });


    });
}

/* Timer to wait until ajax has updated before refreshing page */
function checkVariable() {
    if (preventProgress == false) {
        window.location.reload();
    }
    else setTimeout(checkVariable, 1000);
}

function savePlan() {

    if (preventProgress == false) {

        /* Plan is full */
        if ($('.planCell').children().length == numUnits) {

            $('#error2').html("Saving, please wait...");
            $('#error2').show();
            $.ajax({
                url: "../Home/DefaultPlanSave",
                type: "POST",
                success: function (data) {
                    $('#error2').html("Default plan saved");
                    $('#error2').delay(5000).fadeOut('slow').css("color", "green");
                    $('#error2').delay(5000).queue(function (next) {
                        $(this).css("color", "red");
                        next();
                    });
                },
                error: function (data) {

                }
            });
        }

            /* Plan is not full */
        else {
            $('#error2').html("Error, plan is not full");
            $('#error2').show();
            $('#error2').delay(5000).fadeOut('slow');
        }

    }

    else setTimeout(savePlan, 1000);
}

function deleteInnerCell(xThis) {
    var target = $(xThis).parent().parent();
    var targetId = $(target).attr("id");
    var targetActual = $(xThis).parent();

    $('.dialogConfirm').show();

    $(function () {
        $("#dialog-confirm").dialog({
            resizable: false,
            height: 200,
            modal: true,
            buttons: {
                "Delete Unit": function () {

                    if ($(target).hasClass("planCell") || $(target).hasClass("swapSpaceCell")) {
                        /* Set the prevent other actions settings to on */
                        $('.prevent').css("display", "block");
                        $('.innerCell').attr("draggable", "false");
                        preventProgress = true;

                        /* convert the id to a number that will match the session plan */
                        var idString = targetId.toString();
                        var id = idString.replace(/\D/g, '');
                        var type = "";

                        if ($(target).hasClass("planCell")) {
                            type = "p";
                        }
                        else if ($(target).hasClass("swapSpaceCell")) {
                            type = "s";
                        }

                        dataRemove = id + "," + type;

                        /* Delete the unit */
                        $.ajax({
                            url: "../Home/DefaultPlanDelete",
                            type: "POST",
                            data: { data: dataRemove },
                            success: function (data) {
                                $('.innerCell').attr("draggable", "true");
                                $('.prevent').css("display", "none");
                                preventProgress = false;
                                $(targetActual).remove();

                            },
                            error: function (data) {
                                alert("Error removing unit, please refresh the page.");
                            }
                        });

                        $(this).dialog("close");

                    }

                    else {

                        $(targetActual).remove();
                        $(this).dialog("close");

                    }
                },
                Cancel: function () {
                    $(this).dialog("close");
                }
            }
        });
    });
}