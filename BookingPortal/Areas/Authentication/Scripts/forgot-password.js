$(function () {
    $('#txtEmailAddress').focus();
});

 




function BtnReceiveClick2(elm) {
    $(elm).removeAttr('onclick');
    var flg = 0;

    var TxtEmail = $('#TxtEmail').val();
    if (TxtEmail == "") {
        $('#lblemail').html('The email address is required');
        flg = 1;
    }
    else {
        if (!FormatEmail.test(TxtEmail)) {
            $('#lblemail').html('The email address invalid');
            flg = 1;
        }
        else {
            $('#lblemail').html('');
        }
    }
    if (flg == 0) {
        NotifyView();
        $.ajax({
            url: "/handler/authorization",
            type: 'POST',
            data: $("#aspnetForm").serialize() + "&_key=authenticate-attotp",
            cache: false,
            dataType: "html",
            success: function (rs) {
                console.log("::" + rs);
                if (rs != "" && rs.indexOf('#') != -1) {
                    var res = rs.split('#');
                    if (res[0] == '1') {
                        Notification("", res[1], "ss");
                        $('#TxtEmail').val('');
                        location.href = res[2];
                    }
                    else {
                        Notification("", res[1], "wr");
                    }

                    setTimeout(function () {
                        $(elm).attr('onclick', 'BtnReceiveClick2(this)');
                    }, 3000)
                }
                NotifyRemove();
            },
            error: function (rs) {
                // something here
            }
        });
    }
}
$('#TxtEmail').keyup(function (event) {
    var Email = $(this).val();
    if (Email == "") {
        $('#lblemail').html('Email is required');
    }
    else {
        if (!FormatEmail.test(Email)) {
            $('#lblemail').html('Invalid email');
        }
        else {
            $('#lblemail').html('');
        }
    }
});
