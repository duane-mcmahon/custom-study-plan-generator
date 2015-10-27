$(document).ready(function () {

    $('#createButton').click(function() {

        var data = $('#studentIdInput').val();
       
        $.ajax({
            url: "../Home/CheckStudentID",
            type: "POST",
            data: { data: data },
            success: function (data) {
                if (data == "true") {
                    $('input[name=formInput]').val("create");
                    $('#selectionForm').submit();
                }
                else {

                    $('#error3').text("Student ID does not exist.");
                    $('#error3').show();
                    $('#error3').fadeOut(2000).delay(2000);
                }
            },
            error: function (data) {
                
            }
        });

    });

    $('#editButton').click(function () {

        var data = $('#studentIdInput').val();

        $.ajax({
            url: "../Home/CheckStudentID",
            type: "POST",
            data: { data: data },
            success: function (data) {
                if (data == "true") {
                    $('input[name=formInput]').val("edit");
                    $('#selectionForm').submit();
                }
                else {

                    $('#error3').text("Student ID does not exist.");
                    $('#error3').show();
                    $('#error3').fadeOut(2000).delay(2000);
                }
            },
            error: function (data) {

            }
        });

    });

});