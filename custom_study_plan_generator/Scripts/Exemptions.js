$(document).ready(function () {
     
    /* Dynamic sizing of background divs and table height */
    var widthBacker = $('#backerCP').width();
    var widthText = $('#textCP').width();
    $('#backerCP').css("height", widthBacker * .50);
    $('#textCP').css("height", widthText * .47);

    var heightTopCell = $('#topCell').height();
    var heightp1 = $('#p1').height();

    if (numUnits == 32) {
        $('.cell').css("width", "12%");
        $('.planHeader').css("width", "12%")
    }

    var count = 1;
    converted.forEach(function (entry) {
        var id = "#" + count;
        $(id).html(entry.name);
        count++
    });


    /* Mark and unmark exempt units */
    $(".innerCell").click(function () {

        var id = $(this).attr('id');
        id = '#' + id;

        if ($(id).hasClass("marked")) {
            $(id).removeClass("marked");
        }
        else {
            $(id).addClass("marked");
        }
    });

    /* Send an ajax array of selected units to the Home/RemoveExemptions controller */
    $('#removeExemptions').click(function () {

        var id;
        var array = [];
        var elements = document.getElementsByClassName("marked");
        for (var x = 0; x < elements.length; x++) {
            id = $(elements[x]).get(0).id;
            array[x] = id;
        }

        $.ajax({
            url: 'RemoveExemptions',
            error: function (data) {
                alert("Error processing RemoveExcemptions ajax request");
            },
            type: "POST",
            data: { data: array },
            success: function (data) {
                $(".marked").remove();
            }
        });
    });




});

$(window).on('resize', function () {
    alert("hello");
    var widthBacker = $('#backerCP').width();
    var widthText = $('#textCP').width();
    $('#backerCP').css("height", widthBacker * .50);
    $('#textCP').css("height", widthText * .47);

    var heightTopCell = $('#topCell').height();
    var heightp1 = $('#p1').height();

    $('#planTable').css("height", heightTopCell + (heightp1 * 4));
});