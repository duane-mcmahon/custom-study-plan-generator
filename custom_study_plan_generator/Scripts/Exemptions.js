$(document).ready(function () {
     
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