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
        alert("hello");
    }
}

function dragend(ev) {
    var elements = document.getElementsByClassName("possibleTarget");
    for (var x = 0; x < elements.length; x++) {
        $(elements[x]).removeClass("possibleTarget");
    }
}