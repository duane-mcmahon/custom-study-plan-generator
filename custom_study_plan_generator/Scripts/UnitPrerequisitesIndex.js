$(document).ready(function () {

    

    /* Delete a prerequisite */
    $('.prereqDelete').click(function () {

        deletePrerequisite(this);

        return false;

    });


});

function deletePrerequisite(xThis) {

    $('.dialogConfirm').show();
    var deletePrereq = false;
    $(function () {
            $("#dialog-confirm").dialog({
                resizable: false,
                height: 200,
                modal: true,
                buttons: {
                    "Delete Prerequisite": function () {
                        deletePrereq = true;
                        $(this).dialog("close");
                        $.get(xThis.href);
                        window.location.reload();

                    },
                Cancel: function () {
                        $(this).dialog("close");

                    }
                }
            });
    });

    
    return deletePrereq;
}
