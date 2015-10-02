$(document).ready(function () {

    /* Dynamic sizing of background divs and table height */
    var widthBacker = $('#backerEdit').width();
    var widthText = $('#textEdit').width();
    $('#backerEdit').css("height", widthBacker * .75);
    $('#textEdit').css("height", widthText * .73);

    var heightTopCell = $('#topCell').height();
    var heightp1 = $('#p1').height();

    $('#planTable').css("height", heightTopCell + (heightp1 * 4));
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