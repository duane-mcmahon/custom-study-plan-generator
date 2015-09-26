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






});