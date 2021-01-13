var URLC = "/Development/Account/Action";
var accountController = {
    init: function () {
        accountController.registerEvent();
    },
    registerEvent: function () {
        $('#btnChangePassword').off('click').on('click', function () {
            var flg = true;
            var password = $('#txtOldPassword').val();
            var newPassword = $('#txtNewPassword').val();
            var reNewPassword = $('#txtReNewPassword').val();
            // old password
            if (password === '') {
                $('#lblOldPassword').html('Không được để trống mật khẩu');
                flg = false;
            }
            else {
                $('#lblOldPassword').html('');
            }

            if (newPassword === "") {
                $('#lblNewPassword').html('Không được để trống mật khẩu mới');
                $('#txtReNewPassword').val('');
                $('#lblReNewPassword').html("");
                flg = false;

            } else if (newPassword.length < 2 || newPassword.length > 30) {
                $('#lblNewPassword').html('Mật khẩu mới giới hạn [2-30] ký tự');
                $('#txtReNewPassword').val('');
                $('#lblReNewPassword').html("");
                flg = false;

            }
            else if (!FormatPass.test(newPassword)) {
                $('#lblNewPassword').html('Yêu cầu mật khẩu mới bảo mật hơn');
                $('#txtReNewPassword').val('');
                $('#lblReNewPassword').html("");
                flg = false;
            }
            else {
                $('#lblNewPassword').html('');
            }

            if (reNewPassword === "") {
                $('#lblReNewPassword').html("Vui lòng xác nhận mật khẩu mới");
            }
            else if (reNewPassword !== newPassword) {
                $('#lblReNewPassword').html('Xác nhận mật khẩu mới chưa đúng');
            }
            else {
                $('#lblReNewPassword').html('');
            }
            // submit
            if (flg)
                accountController.ChangePassword();
            else
                Notifization.Error(MessageText.DATAMISSING);
        });
        $('#btnChangePinCode').off('click').on('click', function () {
            var flg = true;
            var pinCode = $('#txtPinCode').val();
            var newPinCode = $('#txtNewPinCode').val();
            var reNewPinCode = $('#txtReNewPinCode').val();
            // old password
            if (pinCode === '') {
                $('#lblPinCode').html('Không được để trống mã pin');
                flg = false;
            }
            else {
                $('#lblPinCode').html('');
            }

            if (newPinCode === "") {
                $('#lblNewPinCode').html('Không được để trống mã pin mới');
                $('#txtReNewPinCode').val('');
                $('#lblReNewPinCode').html("");
                flg = false;

            } else if (newPinCode.length !== 8) {
                $('#lblNewPinCode').html('Mã pin mới giới hạn [8] ký tự');
                $('#txtReNewPinCode').val('');
                $('#lblReNewPinCode').html("");
                flg = false;

            }
            else if (!FormatPin.test(newPinCode)) {
                $('#lblNewPinCode').html('Mã pin mới giới hạn [0-9] ký tự');
                $('#txtReNewPinCode').val('');
                $('#lblReNewPinCode').html("");
                flg = false;
            }
            else {
                $('#lblNewPassword').html('');
            }

            if (reNewPinCode === "") {
                $('#lblReNewPinCode').html("Vui lòng xác nhận mã pin mới");
            }
            else if (reNewPinCode !== newPinCode) {
                $('#lblReNewPinCode').html('Xác nhận mã pin mới chưa đúng');
            }
            else {
                $('#lblReNewPassword').html('');
            }
            // submit
            if (flg)
                accountController.ChangePinCode();
            else
                Notifization.Error(MessageText.DATAMISSING);
        });
    },
    ChangePassword: function () {
        var password = $('#txtOldPassword').val();
        var newPassword = $('#txtNewPassword').val();
        var reNewPassword = $('#txtReNewPassword').val();

        var model = {
            Password: password,
            NewPassword: newPassword,
            ReNewPassword: reNewPassword
        };
        AjaxFrom.POST({
            url: URLC + '/ChangePassword',
            data: model,
            success: function (response) {
                if (response !== null) {
                    if (response.status === 200) {
                        Notifization.Success(response.message);
                        FData.ResetForm();
                        return;
                    }
                    else {
                        Notifization.Error(response.message);
                        return;
                    }
                }
                Notifization.Error(MessageText.NotService);
                return;
            },
            error: function (response) {
                console.log('::' + MessageText.NotService);
            }
        });
    },
    ChangePinCode: function () {
        var pinCode = $('#txtPinCode').val();
        var newPinCode = $('#txtNewPinCode').val();
        var reNewPinCode = $('#txtReNewPinCode').val();
        var model = {
            PinCode: pinCode,
            NewPinCode: newPinCode,
            ReNewPinCode: reNewPinCode
        };
        AjaxFrom.POST({
            url: URLC + '/changepincode',
            data: model,
            success: function (response) {
                if (response !== null) {
                    if (response.status === 200) {
                        Notifization.Success(response.message);
                        FData.ResetForm();
                        return;
                    }
                    else {
                        Notifization.Error(response.message);
                        return;
                    }
                }
                Notifization.Error(MessageText.NotService);
                return;
            },
            error: function (response) {
                console.log('::' + MessageText.NotService);
            }
        });
    }
};
accountController.init();
//  change password
$(document).on("keyup", "#txtOldPassword", function () {
    var password = $(this).val();
    if (password === '') {
        $('#lblOldPassword').html('Không được để trống mật khẩu');
    }
    else {
        $('#lblOldPassword').html('');
    }
});
$(document).on("keyup", "#txtNewPassword", function () {
    var txtPassID = $(this).val();
    if (txtPassID === "") {
        $('#lblNewPassword').html('Không được để trống mật khẩu mới');
        $('#txtReNewPassword').val('');
        $('#lblReNewPassword').html("");

    } else if (txtPassID.length < 2 || txtPassID.length > 30) {
        $('#lblNewPassword').html('Mật khẩu mới giới hạn [2-30] ký tự');
        $('#txtReNewPassword').val('');
        $('#lblReNewPassword').html("");

    }
    else if (!FormatPass.test(txtPassID)) {
        $('#lblNewPassword').html('Yêu cầu mật khẩu mới bảo mật hơn');
        $('#txtReNewPassword').val('');
        $('#lblReNewPassword').html("");

    }
    else {
        $('#lblNewPassword').html('');
    }
});
$(document).on("keyup", "#txtReNewPassword", function () {
    let txtReNewPassword = $(this).val();
    let txtNewPassword = $("#txtNewPassword").val();
    if (txtReNewPassword === "") {
        $('#lblReNewPassword').html("Vui lòng xác nhận mật khẩu mới");
    }
    else if (txtReNewPassword !== txtNewPassword) {
        $('#lblReNewPassword').html('Xác nhận mật khẩu mới chưa đúng');
    }
    else {
        $('#lblReNewPassword').html('');
    }
});

//  change pin
$(document).on("keyup", "#txtPinCode", function () {
    var password = $(this).val();
    if (password === '') {
        $('#lblPinCode').html('Không được để trống mã pin');
    }
    else {
        $('#lblPinCode').html('');
    }
});
$(document).on("keyup", "#txtNewPinCode", function () {
    var txtPassID = $(this).val();
    if (txtPassID === "") {
        $('#lblNewPinCode').html('Không được để trống mã pin mới');
        $('#txtReNewPinCode').val('');
        $('#lblReNewPinCode').html("");

    } else if (txtPassID.length !== 8) {
        $('#lblNewPinCode').html('Mã pin mới giới hạn [8] ký tự');
        $('#txtReNewPinCode').val('');
        $('#lblReNewPinCode').html("");

    }
    else if (!FormatPin.test(txtPassID)) {
        $('#lblNewPinCode').html('Mã pin mới giới hạn [0-9]');
        $('#txtReNewPinCode').val('');
        $('#lblReNewPinCode').html("");

    }
    else {
        $('#lblNewPinCode').html('');
    }
});
$(document).on("keyup", "#txtReNewPinCode", function () {
    let reNewPinCode = $(this).val();
    let newPinCode = $("#txtNewPinCode").val();
    if (reNewPinCode === "") {
        $('#lblReNewPinCode').html("Vui lòng xác nhận mã pin mới");
    }
    else if (reNewPinCode !== newPinCode) {
        $('#lblReNewPinCode').html('Xác nhận mã pin mới chưa đúng');
    }
    else {
        $('#lblReNewPinCode').html('');
    }
});