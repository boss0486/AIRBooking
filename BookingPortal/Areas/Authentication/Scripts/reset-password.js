var _urlAction = "/api/authen";
var resetController = {
    init: function () {
        resetController.registerEvent();
    },
    registerEvent: function () {
        $('#btnResetPassword').off('click').on('click', function (event) {
            var flg = true;
            var otp = $('#txtOTP').val();
            if (otp === '') {
                $('#lblOtp').html('Không được để trống mã OTP');
                flg = false;
            }
            else {
                $('#lblOtp').html('');
            }
            // valid password
            var password = $('#txtPassword').val();
            if (password === "") {
                $('#lblPassword').html('Không được để trống mật khẩu');
                $('#txtRePassword').val('');
                flg = false;
            } else if (password.length < 4 || password.length > 16) {
                $('#lblPassword').html('Mật khẩu giới hạn [4-16] ký tự');
                $('#txtRePassword').val('');
                flg = false;
            }
            else if (!FormatPass.test(password)) {
                $('#lblPassword').html('Yêu cầu mật khẩu bảo mật hơn');
                $('#txtRePassword').val('');
                flg = false;
            }
            else {
                $('#lblPassword').html('');
            }
            // Re-password
            var rePassword = $('#txtRePassword').val();
            if (rePassword === "") {
                $('#lblRePassword').html("Vui lòng xác nhận mật khẩu");
                flg = false;
            }
            else if (rePassword !== password) {
                $('#lblRePassword').html('Xác nhận mật khẩu chưa đúng');
                flg = false;
            }
            else {
                $('#lblRePassword').html('');
            }

            if (flg)
                resetController.ResetPassword();
            else
                Notifization.Error(MessageText.Datamissing);
        });
    },
    ResetPassword: function () {
        var otpCode = $('#txtOTP').val();
        var password = $('#txtPassword').val();
        var rePassword = $('#txtRePassword').val();
        var token = $('#txtToken').val();
        var model = {
            OTPCode: otpCode,
            Password: password,
            RePassword: rePassword,
            TokenID: token
        };

        AjaxFrom.POST({
            url: _urlAction + '/Password',
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
resetController.init();
$(document).on("keyup", "#txtOTP", function () {
    var otp = $(this).val();
    if (otp === '') {
        $('#lblOtp').html('Không được để trống mã OTP');
    }
    else {
        $('#lblOtp').html('');
    }
});
$(document).on("keyup", "#txtPassword", function () {
    var password = $(this).val();
    if (password === "") {
        $('#lblPassword').html('Không được để trống mật khẩu');
        $('#txtRePassword').val('');
        $('#lblRePassID').html('');

    } else if (password.length < 4 || password.length > 16) {
        $('#lblPassword').html('Mật khẩu giới hạn [4-16] ký tự');
        $('#txtRePassword').val('');
        $('#lblRePassID').html('');
    }
    else if (!FormatPass.test(password)) {
        $('#lblPassword').html('Yêu cầu mật khẩu bảo mật hơn');
        $('#txtRePassword').val('');
        $('#lblRePassID').html('');
    }
    else {
        $('#lblPassword').html('');
    }
});
$(document).on("keyup", "#txtRePassword", function () {
    let rePassword = $(this).val();
    let password = $("#txtPassword").val();
    if (rePassword === "") {
        $('#lblRePassword').html("Vui lòng xác nhận mật khẩu");
    }
    else if (rePassword !== password) {
        $('#lblRePassword').html('Xác nhận mật khẩu chưa đúng');
    }
    else {
        $('#lblRePassword').html('');
    }
});