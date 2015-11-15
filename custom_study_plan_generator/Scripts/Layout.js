var planSaved = false;

$(document).ready(function () {

    $('.dialog').hide();

    $('#submitPlan').click(function () {

        

        if (planSaved == false) {

            $(function () {

                $('.dialogOnly').show();
                $("#dialog").dialog();
            });

            return false;
        }

        else {

            return true;

        }

    });

});