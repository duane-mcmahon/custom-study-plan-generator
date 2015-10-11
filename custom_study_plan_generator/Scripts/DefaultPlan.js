var dragParentId;

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
            if (entry != "") {
                idCont = "#p" + count;
                jQuery('<div/>', {
                    id: count,
                    class: 'innerCell active',
                    draggable: 'true',
                    ondragstart: 'drag(event)',
                    ondragend: 'dragend(event, this)',
                    text: entry

                }).appendTo(idCont);
            }
            count++
        });
    }

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

        if (duplicate == false) {
            for (var x = 0; x < elements.length; x++) {
                if ($(elements[x]).children().length == 0) {
                    jQuery('<div/>', {
                        id: id,
                        class: 'innerCell active',
                        draggable: 'true',
                        ondragstart: 'drag(event)',
                        ondragend: 'dragend(event, this)',
                        text: unit

                    }).appendTo(elements[x]);
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

    });

    $('#savePlan').click(function () {

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
                    alert("Error saving plan" + data.responseText);
                }
            });
        }

        /* Plan is not full */
        else {
            $('#error2').html("Error, plan is not full");
            $('#error2').show();
            $('#error2').delay(5000).fadeOut('slow');
        }
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
                alert("Error saving plan" + data.responseText);
            }
        });

    });

           

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
}

/* This funtions runs when the drag begins */
function drag(ev) {

    /* Set the element being hovered over as a pssible drag target */
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

/* this function runs after drag completion, wjether it was successful or not */
function dragend(ev, target) {


    /* Remove all previous possible targets */
    var elements = document.getElementsByClassName("possibleTarget");
    for (var x = 0; x < elements.length; x++) {
        $(elements[x]).removeClass("possibleTarget");
    }

    /* Hide any previous errors */
    $('#errors').hide();

    /* Drag successful */
    if (ev.dataTransfer.dropEffect !== 'none') {
        
        /* If moved TO the swap space, remove unit from session plan */
        if ($(target).parent().hasClass('swapSpaceCell')) {

            var idRaw = dragParentId;
            var idString = idRaw.toString();
            var id = idString.substring(1);

            data = id;

            $.ajax({
                url: "../Home/DefaultPlanRemove",
                type: "POST",
                data: { data: data },
                success: function (data) {
                    
                },
                error: function (data) {
                    alert("Error removing unit" + data.responseText);
                }
            });
        }
        /* If moved TO the plan, add unit to session plan */

        else if ($(target).parent().hasClass('planCell')) {

            var idRaw = $(target).parent().attr('id');
            var idString = idRaw.toString();
            var id = idString.substring(1);
      
            var unit = $(target).html();

            var data = id + "," + unit;

            $.ajax({
                url: "../Home/DefaultPlanAdd",
                type: "POST",
                data: { data: data },
                success: function (data) {
                   
                },
                error: function (data) {
                    alert("Error adding unit");
                }
            });
        }

    }
        /* Drag failed */
    else {

    }
    
}