var dragParentId;
var preventProgress = false;

$(document).ready(function () {

    /* Dynamic sizing of background divs and table height */
    var widthBacker = $('#backerDefault').width();
    var widthText = $('#textDefault').width();
    $('#backerDefault').css("height", widthBacker * .80);
    $('#textDefault').css("height", widthText * .77);

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
            if (entry != "") {
                idCont = "#p" + count;

                $(idCont).append("<div id = '" + count + "' class = 'innerCell active' draggable = 'true' ondragstart = 'drag(event)' ondragend = 'dragend(event, this)'></div>");
                $(innerCellId).text(entry);

                /* Create hover icon */
                var hoverId = 'hover' + count;
                $(innerCellId).append("<img id = '" + hoverId + "' class = 'hover' src = '../Content/Images/hover.png' />");

                /* Create prevent icon */
                var preventId = 'prevent' + count;
                $(innerCellId).append("<img id = '" + preventId + "' class = 'prevent' src = '../Content/Images/prevent.png' />");

                /* Create delete icon */
                var deleteId = 'delete' + count;
                $(innerCellId).append("<img id = '" + deleteId + "' class = 'delete' src = '../Content/Images/delete.png' />");

            }

            /* Create prereq violated exclamation mark icon where required */
            for (var x = 0; x < violatedListConverted.length; x++) {
                if (entry == violatedListConverted[x]) {
                    var exclamationId = 'exclamation' + count;
                    $(innerCellId).append("<img id = '" + exclamationId + "' class = 'exclamation' src = '../Content/Images/exclamation.png' />");
                }
            }
            count++
        });
    }

    /* Delete a unit from the page */
    $('.delete').click(function () {
        
        deleteInnerCell(this);

    });

    /* Add a unit to the swap space */
    $('#addUnit').click(function () {

        $('#errors').hide();

        var idAString;
        var idBString;
        var ssA;
        var ssB;

        /* Get all the parent cells in the swap space and sort them by ID */
        var elements = $('.swapSpaceCell').sort(function (a, b) {
            idAString = (a.id).toString();
            idBString = (b.id).toString();
            ssA = idAString.substring(2);
            ssB = idBString.substring(2);
            return parseInt(ssA) - parseInt(ssB);
        })

        var unit = $('#unitDropDown :selected').text();
        var swapSpaceFull = 0;
        var duplicate = false;

        /* Check if the unit already exists, if it does give an error  */
        $('.innerCell').each(function () {
            if ($(this).html() == unit) {
                duplicate = true;
                $('#errors').html("Error, unit is already on plan. Please select a different unit");
                $('#errors').show();

            }
        });

        /* If the unit doesn't exist, cycle through the swap space parent cells and add to the first empty one */
        /* Generate a unique id */
        var id = "id" + Math.random().toString(16).slice(2)
        var innerCellId = "#" + id;

        if (duplicate == false) {
            for (var x = 0; x < elements.length; x++) {
                if ($(elements[x]).children().length == 0) {

                    $(elements[x]).append("<div id = '" + id + "' class = 'innerCell active' draggable = 'true' ondragstart = 'drag(event)' ondragend = 'dragend(event, this)'></div>");
                    $(innerCellId).text(unit);

                    /* Create hover icon */
                    var hoverId = 'hover' + id;
                    $(innerCellId).append("<img id = '" + hoverId + "' class = 'hover' src = '../Content/Images/hover.png' />");
                    var jId = "#" + hoverId;
                    tooltip(jId, "tooltip");

                    /* Create delete icon */
                    var deleteId = 'delete' + count;
                    $(innerCellId).append("<img id = '" + deleteId + "' class = 'delete' src = '../Content/Images/delete.png' />");

                    break;
                }
                else {
                    swapSpaceFull += 1;
                }

            }
        }

        /* If the swap space was full, give an error */
        if (swapSpaceFull == 12) {
            $('#errors').html("Error, swap space is full. Please clear some space first.");
            $('#errors').show();
        }

        /* Reset the delete cell click functiuon to include this unit */
        $('.delete').click(function () {

            deleteInnerCell(this);

        });

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
    var widthBacker = $('#backerDefault').width();
    var widthText = $('#textDefault').width();
    $('#backerDefault').css("height", widthBacker * .80);
    $('#textDefault').css("height", widthText * .77);

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

        

        /* If moved TO the swap space, remove unit from session plan */
        if ($(target).parent().hasClass('swapSpaceCell')) {

            var idRaw = dragParentId;
            var idString = idRaw.toString();
            var id = idString.substring(1);

            dataRemove = id;

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

        /* If moved TO the plan, add unit to session plan */
        else if ($(target).parent().hasClass('planCell')) {

            var idRaw = $(target).parent().attr('id');
            var idString = idRaw.toString();
            var id = idString.substring(1);

            var unit = $(target).text();

            var data = id + "," + unit;


            $.ajax({
                url: "../Home/DefaultPlanAdd",
                type: "POST",
                data: { data: data },
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

    $(function () {
        $("#dialog-confirm").dialog({
            resizable: false,
            height: 200,
            modal: true,
            buttons: {
                "Delete unit": function () {

                    if ($(target).hasClass("planCell")) {
                        /* Set the prevent other actions settings to on */
                        $('.prevent').css("display", "block");
                        $('.innerCell').attr("draggable", "false");
                        preventProgress = true;

                        /* convert the id to a number that will match the session plan */
                        var idString = targetId.toString();
                        var id = idString.substring(1);
                        dataRemove = id;

                        /* Delete the unit */
                        $.ajax({
                            url: "../Home/DefaultPlanRemove",
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