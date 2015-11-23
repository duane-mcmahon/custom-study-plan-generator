var dragParentId;
var preventProgress = false;

$(document).ready(function () {
    
    /* Dynamic sizing of background divs and table height */
    var heightTable = $('#planTable').height();
    $('#textEdit').css("height", heightTable * 2.7);
    $('#backerEdit').css("height", heightTable * 2.9);

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

                    $(idCont).append("<div id = '" + count + "' class = 'innerCell active' draggable = 'true' ondragstart = 'drag(event)' ondragend = 'dragend(event, this)'></div>");
                    $(innerCellId).text(entry.name);

                    /* Create hover icon */
                    var hoverId = 'hover' + count;
                    $(innerCellId).append("<img id = '" + hoverId + "' class = 'hoverModify' src = '../Content/Images/hover.png' draggable = 'false' />");

                    /* Create prevent icon */
                    var preventId = 'prevent' + count;
                    $(innerCellId).append("<img id = '" + preventId + "' class = 'prevent' src = '../Content/Images/prevent.png' draggable = 'false' />");
                }

                /* Create prereq violated exclamation mark icon where required */
                for (var x = 0; x < violatedListConverted.length; x++) {
                    if (entry.name == violatedListConverted[x]) {
                        var exclamationId = 'exclamation' + count;
                        $(innerCellId).append("<img id = '" + exclamationId + "' class = 'exclamation' src = '../Content/Images/exclamation.png' draggable = 'false' />");
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
                if (entry != "" && entry.exempt == false) {
                    idCont = "#ss" + count;

                    $(idCont).append("<div id = '" + id + "' class = 'innerCell active' draggable = 'true' ondragstart = 'drag(event)' ondragend = 'dragend(event, this)'></div>");
                    $(innerCellId).text(entry.name);

                    /* Create hover icon */
                    var hoverId = 'hover' + count;
                    $(innerCellId).append("<img id = '" + hoverId + "' class = 'hover' src = '../Content/Images/hover.png' draggable = 'false' />");

                    /* Create prevent icon */
                    var preventId = 'prevent' + count;
                    $(innerCellId).append("<img id = '" + preventId + "' class = 'prevent' src = '../Content/Images/prevent.png' draggable = 'false' />");
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

    tooltip(".hoverModify", "tooltip");

});

$(window).on('resize', function () {

    var heightTable = $('#planTable').height();
    $('#textEdit').css("height", heightTable * 2.7);
    $('#backerEdit').css("height", heightTable * 2.9);

    var heightTopCell = $('#topCell').height();
    var heightp1 = $('#p1').height();

    $('#planTable').css("height", heightTopCell + (heightp1 * 4));
});


function allowDrop(ev) {

    ev.preventDefault();
    if ($(ev.target).children().length > 0)
        ev.dataTransfer.dropEffect = "none"; // dropping is not allowed
    else if ($(ev.target).hasClass('hoverModify'))
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

        var PLAN_CELL = 'p';
        var SWAP_CELL = 's';

        /* If moved TO the swap space, remove unit from session plan */
        if ($(target).parent().hasClass('swapSpaceCell') &&
             (dragParentId.indexOf(PLAN_CELL) === 0)) {

            var idRawFrom = dragParentId;
            var idStringFrom = idRawFrom.toString();
            var idFrom = idStringFrom.replace(/\D/g,'');

            var idRawTo = $(target).parent().attr("id");
            var idStringTo = idRawTo.toString();
            var idTo = idStringTo.replace(/\D/g, '');

            dataRemove = idFrom + "," + idTo;
            
            $.ajax({
                url: "../Home/ModifyRemove",
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
        else if ($(target).parent().hasClass('planCell') && 
                  (dragParentId.indexOf(SWAP_CELL) === 0)) {

            var idRawFrom = dragParentId;
            var idStringFrom = idRawFrom.toString();
            var idFrom = idStringFrom.replace(/\D/g,'');

            var idRawTo = $(target).parent().attr('id');
            var idStringTo = idRawTo.toString();
            var idTo = idStringTo.replace(/\D/g, '');

            dataAdd = idFrom + "," + idTo;

            $.ajax({
                url: "../Home/ModifyAdd",
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

        /* If moved WITHIN the plan, move the unit to its new position in the session plan */
        else if ($(target).parent().hasClass('planCell') &&
                  (dragParentId.indexOf(PLAN_CELL) === 0)) {

            var idRawFrom = dragParentId;
            var idStringFrom = idRawFrom.toString();
            var idFrom = idStringFrom.replace(/\D/g, '');

            var idRawTo = $(target).parent().attr('id');
            var idStringTo = idRawTo.toString();
            var idTo = idStringTo.replace(/\D/g, '');

            dataAdd = idFrom + "," + idTo;

            $.ajax({
                url: "../Home/ModifyMove",
                type: "POST",
                data: { data: dataAdd },
                success: function (data) {
                    $('.innerCell').attr("draggable", "true");
                    $('.prevent').css("display", "none");
                    preventProgress = false;
                },
                error: function (data) {
                    alert("Error moving unit, please refresh the page.");
                }
            });

        }

        /* If moved WITHIN the swap space, move it to its new position in the session swap list. */
        else if ($(target).parent().hasClass('swapSpaceCell') &&
                  (dragParentId.indexOf(SWAP_CELL) === 0)) {
            
            var idRawFrom = dragParentId;
            var idStringFrom = idRawFrom.toString();
            var idFrom = idStringFrom.replace(/\D/g, '');

            var idRawTo = $(target).parent().attr('id');
            var idStringTo = idRawTo.toString();
            var idTo = idStringTo.replace(/\D/g, '');

            dataAdd = idFrom + "," + idTo;

            $.ajax({
                url: "../Home/ModifySwap",
                type: "POST",
                data: { data: dataAdd },
                success: function (data) {
                    $('.innerCell').attr("draggable", "true");
                    $('.prevent').css("display", "none");
                    preventProgress = false;
                },
                error: function (data) {
                    alert("Error moving unit, please refresh the page.");
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
                    $(jId).text(data);
                }


            },
            error: function (data) {
                alert(data.responseText);
            }
        });


    });
}