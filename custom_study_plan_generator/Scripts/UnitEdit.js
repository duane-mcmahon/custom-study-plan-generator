$(document).ready(function () {

    $('#semester').hide();

    $('#submit').click(function () {

        if ($('#semester1:checked').val() == "true" || $('#semester2:checked').val() == "true") {

            return true;
        }

        $('#semester').show();
        $('#semester').fadeOut(3000).delay(2000);
        return false;
    })

});