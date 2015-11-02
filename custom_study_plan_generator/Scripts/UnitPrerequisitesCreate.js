$(document).ready(function () {

    $('#prereqExists').delay(3000).fadeOut(2000);

    $('#submit').click(function (event) {
        
        if ($('#unitDropDown').val() == null || $('#prerequisiteDropDown').val() == null) {
            $('#emptyLists').show();
            $('#emptyLists').delay(3000).fadeOut(2000);

            return false;
        }

        if ($('#unitDropDown').val() == $('#prerequisiteDropDown').val()) {
            $('#unitPrereqSame').show();
            $('#unitPrereqSame').delay(3000).fadeOut(2000);

            return false;
        }

    });

});