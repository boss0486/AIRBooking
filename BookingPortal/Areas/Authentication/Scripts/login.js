var _urlAction = "/Authentication/Action";

var loginController = {
    init: function () {
        loginController.registerEvent();
    },
    registerEvent: function () {
        $(document).on('click', '#btnLogin', function (event) {
            var flg = true;
            var loginId = $('#txtLoginID').val();
            var passId = $('#txtPassID').val();
            if (loginId === "") {
                $('#lblLoginID').html("Không được để trống tài khoản");
                flg = false;
            }
            else if (!FormatUser.test(loginId)) {
                $('#lblLoginID').html('Tài khoản không hợp lệ');
                flg = false;
            }
            else {
                $('#lblLoginID').html('');
            }
            if (passId === "") {
                $('#lblPassID').html('Không được để trống mật khẩu');
                flg = false;
            }
            else {
                $('#lblPassID').html('');
            }
            if (flg) {
                loginController.Login();
                // $(this).off(event);
            }
            else
                Notifization.Error(MessageText.Datamissing);
        });
        //$('#btnLogin').off('click').on('click', function (event) {
        //    //.one('click', function (event) 
        //    event.stopPropagation();         

        //});
        $('#btnForgotPassword').off('click').on('click', function (event) {
            var flg = true;
            var email = $('#txtEmail').val();
            if (email === "") {
                $('#lblEmail').html("Không được để trống địa chỉ email");
                flg = false;
            }
            else if (!FormatEmail.test(email)) {
                $('#lblEmail').html('Địa chỉ email không hợp lệ');
                flg = false;
            }
            else {
                $('#lblEmail').html('');
            }
            if (flg)
                loginController.SendOTP();
        });
        $('#btnResetPassword').off('click').on('click', function (event) {
            var flg = true;
            var email = $('#txtEmail').val();
            if (email === "") {
                $('#lblEmail').html("Không được để trống địa chỉ email");
                flg = false;
            }
            else if (!FormatEmail.test(email)) {
                $('#lblEmail').html('Địa chỉ email không hợp lệ');
                flg = false;
            }
            else {
                $('#lblEmail').html('');
            }
            if (flg)
                loginController.ResetPassword();
            else
                Notifization.Error(MessageText.Datamissing);
        });
    },
    Login: function () {
        var loginId = $('#txtLoginID').val();
        var passId = $('#txtPassID').val();
        var remember = false;
        if ($('#reMemberPassword:checked').length > 0) {
            remember = true;
        }
        var model = {
            LoginID: loginId,
            Password: passId,
            IsRemember: remember,
            Url: window.location.href
        };
        AjaxFrom.POST({
            url: _urlAction + '/Login',
            data: model,
            success: function (response) {
                if (response !== null) {
                    if (response.status === undefined) {
                        Notifization.Error(MessageText.NotService);
                        return;
                    }
                    //
                    if (response.status === 200) {
                        Loading.ShowLoading();
                        Notifization.Success(response.message);
                        //setTimeout(function () {
                        //    location.href = response.data;
                        //}, 2000);
                        return;
                    }
                    //
                    Notifization.Error(response.message);
                    return;
                }
            },
            error: function (response) {
                //Notifization.Error(MessageText.NOTSERVICES);
                console.log('::' + MessageText.NotService);
            }
        });
    },
    SendOTP: function () {
        var email = $('#txtEmail').val();
        var model = {
            Email: email
        };
        AjaxFrom.POST({
            url: _urlAction + '/Forgot',
            data: model,
            success: function (response) {
                if (response !== null) {
                    if (response.status === undefined) {
                        Notifization.Error(MessageText.NotService);
                        return;
                    }
                    //
                    if (response.status === 200) {
                        Notifization.Success(response.message);
                        setTimeout(function () {
                            location.href = response.data;
                        }, 3000);
                        return;
                    }
                    //
                    Notifization.Error(response.message);
                    return;
                }
            },
            error: function (response) {
                console.log('::' + MessageText.NotService);
            }
        });
    }
};
loginController.init();
$(document).on("keyup", "#txtLoginID", function () {
    var txtLoginID = $(this).val();
    if (txtLoginID === '') {
        $('#lblLoginID').html('Không được để trống tài khoản');
    }
    else if (!FormatUser.test(txtLoginID)) {
        $('#lblLoginID').html('Tài khoản không hợp lệ');
    }
    else {

        $('#lblLoginID').html('');
    }
});
$(document).on("keyup", "#txtPassID", function () {
    var txtPassID = $(this).val();
    if (txtPassID === "") {
        $('#lblPassID').html('Không được để trống mật khẩu');
    }
    else {
        $('#lblPassID').html('');
    }
});
$(document).on("keyup", "#txtEmail", function () {
    var email = $(this).val();
    if (email === "") {
        $('#lblEmail').html("Không được để trống địa chỉ email");
    }
    else if (!FormatEmail.test(email)) {
        $('#lblEmail').html('Địa chỉ email không hợp lệ');
    }
    else {
        $('#lblEmail').html('');
    }
});
$(document).on("keyup", "#txtOTP", function () {
    var otp = $(this).val();
    if (otp === "") {
        $('#lblOtp').html('Mã OTP không được để trống');
    }
    else {
        $('#lblOtp').html('');
    }
});
