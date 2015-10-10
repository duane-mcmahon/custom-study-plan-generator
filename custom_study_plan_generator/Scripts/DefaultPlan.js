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

function dragend(ev) {
    var elements = document.getElementsByClassName("possibleTarget");
    for (var x = 0; x < elements.length; x++) {
        $(elements[x]).removeClass("possibleTarget");
    }
}