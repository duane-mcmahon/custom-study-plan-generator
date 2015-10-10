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
        alert(percString);
        $('.cell').css("width", percString);
        $('.planHeader').css("width", percString)
    }


    var count = 1;
    unitListSelected.forEach(function (entry) {
        idCont = "#p" + count;
        jQuery('<div/>', {
            id: count,
            class: 'innerCell active',
            draggable: 'true',
            ondragstart: 'drag(event)',
            ondragend: 'dragend(event)',
            text: entry

        }).appendTo(idCont);

        count++
    });

    $('#addUnit').click(function () {

        $('#errors').hide();

        var idAString;
        var idBString;
        var ssA;
        var ssB;
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

        $('.innerCell').each(function () {
            if ($(this).html() == unit) {
                duplicate = true;
                $('#errors').html("Error, unit is already on plan. Please select a different unit");
                $('#errors').show();

            }
        });

        if (duplicate == false) {
            for (var x = 0; x < elements.length; x++) {
                if ($(elements[x]).children().length == 0) {
                    jQuery('<div/>', {
                        id: count,
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

        if (swapSpaceFull == 12) {
            $('#errors').html("Error, swap space is full. Please clear some space first.");
            $('#errors').show();
        }

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

function drag(ev) {

    ev.dataTransfer.setData("text", ev.target.id);
    $(document.getElementById(ev.target.id).parentElement).addClass("possibleTarget");
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
    var elements = document.getElementsByClassName("possibleTarget");
    for (var x = 0; x < elements.length; x++) {
        $(elements[x]).removeClass("possibleTarget");
    }
    $('#errors').hide();

    /* Drag successful */
    if (ev.dataTransfer.dropEffect !== 'none') {
        
        /* If moved TO the swap space, remove unit from plan */
        if ($(target).parent().hasClass('swapSpaceCell')) {
          
        }
        /* If moved TO the plan, add unit from plan */
        else if ($(target).parent().hasClass('planCell')) {
           
        }

    }
        /* Drag failed */
    else {
        alert("failed");
    }
    
}