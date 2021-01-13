var pageIndex = 1;
var URLC = "/Development/Account/Action";
var URLA = "/Development/Account";

var DevAccountController = {
    init: function () {
        DevAccountController.registerEvent();
    },
    registerEvent: function () {
        $('#btnCreate').off('click').on('click', function () {
            var flg = true;
            var file = "";
            // account validate
            var txtLoginID = $('#txtLoginID').val();
            if (txtLoginID === '') {
                $('#lblLoginID').html('Không được để trống tài khoản');
                flg = false;
            }
            else if (!FormatUser.test(txtLoginID)) {
                $('#lblLoginID').html('Tài khoản không hợp lệ');
                flg = false;
            }
            else if (txtLoginID.length < 4 || txtLoginID.length > 16) {
                $('#lblLoginID').html('Tài khoản giới hạn [6-16] ký tự');
                flg = false;
            }
            else {
                $('#lblLoginID').html('');
            }
            // valid password
            var txtPassword = $('#txtPassword').val();
            if (txtPassword === "") {
                $('#lblPassword').html('Không được để trống mật khẩu');
                $('#txtRePassword').val('');
                flg = false;
            } else if (txtPassword.length < 4 || txtPassword.length > 16) {
                $('#lblPassword').html('Mật khẩu giới hạn [4-16] ký tự');
                $('#txtRePassword').val('');
                flg = false;
            }
            else if (!FormatPass.test(txtPassword)) {
                $('#lblPassword').html('Yêu cầu mật khẩu bảo mật hơn');
                $('#txtRePassword').val('');
                flg = false;
            }
            else {
                $('#lblPassword').html('');
            }
            // Re-password
            var txtRePassword = $('#txtRePassword').val();
            if (txtRePassword === "") {
                $('#lblRePassword').html("Vui lòng xác nhận mật khẩu");
                flg = false;
            }
            else if (txtRePassword !== txtPassword) {
                $('#lblRePassword').html('Xác nhận mật khẩu chưa đúng');
                flg = false;
            }
            else {
                $('#lblRePassword').html('');
            }
            // full name
            var txtFullName = $('#txtFullName').val();
            if (txtFullName === '') {
                $('#lblFullName').html('Không được để trống tên');
                flg = false;
            }
            else if (txtFullName.length < 2 || txtFullName.length > 30) {
                $('#lblFullName').html('Họ tên giới hạn [2-30] ký tự');
                flg = false;
            }
            else if (!FormatFName.test(txtFullName)) {
                $('#lblFullName').html('Họ tên không hợp lệ');
                flg = false;
            }
            else {
                $('#lblFullName').html('');
            }
            // birthday
            var txtBirthday = $('#txtBirthday').val();
            if (txtBirthday !== '') {
                if (!FormatDateVN.test(txtBirthday)) {
                    $('#lblBirthday').html('Ngày sinh không hợp lệ');
                    flg = false;
                } else {
                    $('#lblBirthday').html('');
                }
            }
            // email validate
            var txtEmail = $('#txtEmail').val();
            if (txtEmail === "") {
                $('#lblEmail').html('Không được để trống địa chỉ email');
                flg = false;
            }
            else if (!FormatEmail.test(txtEmail)) {
                $('#lblEmail').html('Địa chỉ email không hợp lệ');
                flg = false;
            }
            else {
                $('#lblEmail').html('');
            }
            // phonnumber validate
            var txtPhone = $('#txtPhone').val();
            if (txtPhone !== "") {
                if (!FormatPhone.test(txtPhone)) {
                    $('#lblPhone').html('Số điện thoại không hợp lệ');
                    flg = false;
                }
                else {
                    $('#lblPhone').html('');
                }
            }
            // address validate
            var txtAddress = $('#txtAddress').val();
            if (txtAddress !== "") {
                if (!FormatKeyword.test(txtAddress)) {
                    $('#lblAddress').html('Địa chỉ không hợp lệ');
                    flg = false;
                }
                else {
                    $('#lblAddress').html('');
                }
            }
            //var ddlLanguage = $('#ddlLanguage').val();
            //if (ddlLanguage === "") {
            //    $('#lblLanguage').html('Vui lòng chọn ngôn ngữ');
            //    flg = false;
            //}
            //else {
            //    $('#lblLanguage').html('');
            //}

            if (flg)
                DevAccountController.Create();
            else
                Notifization.Error(MessageText.Datamissing);
        });
        $('#btnSearch').off('click').on('click', function () {
            DevAccountController.DataList(1);
        });
        $('#btnUpdate').off('click').on('click', function () {
            var flg = true;
            var file = "";
            // full name
            var txtFullName = $('#txtFullName').val();
            if (txtFullName === '') {
                $('#lblFullName').html('Không được để trống tên');
                flg = false;
            }
            else if (txtFullName.length < 2 || txtFullName.length > 30) {
                $('#lblFullName').html('Họ tên giới hạn [2-30] ký tự');
                flg = false;
            }
            else if (!FormatFName.test(txtFullName)) {
                $('#lblFullName').html('Họ tên không hợp lệ');
                flg = false;
            }
            else {
                $('#lblFullName').html('');
            }

            // birthday
            var txtBirthday = $('#txtBirthday').val();
            if (txtBirthday !== '') {
                if (!FormatDateVN.test(txtBirthday)) {
                    $('#lblBirthday').html('Không được để trống ngày sinh');
                    flg = false;
                }
                else {
                    $('#lblBirthday').html('');
                }
            }
            // email validate
            var txtEmail = $('#txtEmail').val();
            if (txtEmail === "") {
                $('#lblEmail').html('Không được để trống địa chỉ email');
                flg = false;
            }
            else if (!FormatEmail.test(txtEmail)) {
                $('#lblEmail').html('Địa chỉ email không hợp lệ');
                flg = false;
            }
            else {
                $('#lblEmail').html('');
            }
            // phonnumber validate
            var txtPhone = $('#txtPhone').val();
            if (txtPhone !== "") {
                if (!FormatPhone.test(txtPhone)) {
                    $('#lblPhone').html('Số điện thoại không hợp lệ');
                    flg = false;
                }
                else {
                    $('#lblPhone').html('');
                }
            }
            // address validate
            var txtAddress = $('#txtAddress').val();
            if (txtAddress !== "") {
                if (!FormatKeyword.test(txtAddress)) {
                    $('#lblAddress').html('Địa chỉ không hợp lệ');
                    flg = false;
                }
                else {
                    $('#lblAddress').html('');
                }
            }
            //var ddlRole = $('#ddlRole').val();
            //if (ddlRole === "") {
            //    $('#lblRole').html('Vui lòng chọn nhóm quyền');
            //    flg = false;
            //}
            // bank
            //var ddlLanguage = $('#ddlLanguage').val();
            //if (ddlLanguage === "") {
            //    $('#lblLanguage').html('Vui lòng chọn ngôn ngữ');
            //    flg = false;
            //}
            //else {
            //    $('#lblLanguage').html('');
            //}
            if (flg) {
                DevAccountController.Update();
            }
            else
                Notifization.Error(MessageText.Datamissing + "11");
        });
        $('#btnChangePassword').off('click').on('click', function () {
            var flg = true;
            var txtOldPassword = $('#txtOldPassword').val();
            var txtNewPassword = $('#txtNewPassword').val();
            var txtReNewPassword = $('#txtReNewPassword').val();
            // old password
            if (txtOldPassword === '') {
                $('#lblOldPassword').html('Không được để trống mật khẩu');
                flg = false;
            }
            else {
                $('#lblOldPassword').html('');
            }

            if (txtNewPassword === "") {
                $('#lblNewPassword').html('Không được để trống mật khẩu mới');
                $('#txtReNewPassword').val('');
                $('#lblReNewPassword').html("");
                flg = false;

            } else if (txtNewPassword.length < 2 || txtNewPassword.length > 30) {
                $('#lblNewPassword').html('Mật khẩu mới giới hạn [2-30] ký tự');
                $('#txtReNewPassword').val('');
                $('#lblReNewPassword').html("");
                flg = false;

            }
            else if (!FormatPass.test(txtNewPassword)) {
                $('#lblNewPassword').html('Yêu cầu mật khẩu mới bảo mật hơn');
                $('#txtReNewPassword').val('');
                $('#lblReNewPassword').html("");
                flg = false;
            }
            else {
                $('#lblNewPassword').html('');
            }

            if (txtReNewPassword === "") {
                $('#lblReNewPassword').html("Vui lòng xác nhận mật khẩu mới");
            }
            else if (txtReNewPassword !== txtNewPassword) {
                $('#lblReNewPassword').html('Xác nhận mật khẩu mới chưa đúng');
            }
            else {
                $('#lblReNewPassword').html('');
            }
            // submit
            if (flg)
                DevAccountController.ChangePassword();
            else
                Notifization.Error(MessageText.Datamissing);
        });
    },
    DataList: function (page) {
        //       
        var ddlTimeExpress = $('#ddlTimeExpress').val();
        var txtStartDate = $('#txtStartDate').val();
        var txtEndDate = $('#txtEndDate').val();
        var model = {
            Query: $('#txtQuery').val(),
            Page: page,
            TimeExpress: parseInt(ddlTimeExpress),
            StartDate: txtStartDate,
            EndDate: txtEndDate,
            TimeZoneLocal: LibDateTime.GetTimeZoneByLocal(),
            Status: parseInt($('#ddlStatus').val())
        };
        //
        AjaxFrom.POST({
            url: URLC + '/DataList',
            data: model,
            success: function (result) {
                $('tbody#TblData').html('');
                $('#Pagination').html('');
                if (result !== null) {
                    if (result.status === 200) {
                        var currentPage = 1;
                        var pagination = result.paging;
                        if (pagination !== null) {
                            totalPage = pagination.TotalPage;
                            currentPage = pagination.Page;
                            pageSize = pagination.PageSize;
                            pageIndex = pagination.Page;
                        }
                        var rowData = '';
                        $.each(result.data, function (index, item) {
                            index = index + 1;
                            var id = item.ID;
                            if (id.length > 0)
                                id = id.trim();
                            //
                            //  role
                            var action = HelperModel.RolePermission(result.role, "DevAccountController", id);
                            //
                            var rowNum = parseInt(index) + (parseInt(currentPage) - 1) * parseInt(pageSize);
                            rowData += `
                            <tr>
                                 <td class="text-right">${rowNum}&nbsp;</td>
                                 <td>${item.LoginID}</td>                                  
                                 <td>${item.FullName}</td>
                                 <td class="text-center">${!item.IsBlock ? "<i class='fas fa-check-circle'></i>" : "<i class='fas fa-ban'></i>"}</td>
                                 <td class="text-center">${item.Enabled ? "<i class='fas fa-check-circle'></i>" : "<i class='fas fa-ban'></i>"}</td>
                                 <td class="text-center">${item.CreatedDate}</td>
                                 <td class="tbcol-action">${action}</td>
                            </tr>`;
                        });
                        $('tbody#TblData').html(rowData);
                        if (parseInt(totalPage) > 1) {
                            Paging.Pagination("#Pagination", totalPage, currentPage, DevAccountController.DataList);
                        }
                        return;
                    }
                    else {
                        //Notifization.Error(result.message);
                        console.log('::' + result.message);
                        return;
                    }
                }
                //Message.Error(MessageText.NotService);
                return;
            },
            error: function (result) {
                console.log('::' + MessageText.NotService);
            }
        });
    },
    Create: function () {
        var txtLoginID = $('#txtLoginID').val();
        var txtPassword = $('#txtPassword').val();
        var txtFullName = $('#txtFullName').val();
        var txtBirthday = $('#txtBirthday').val();
        var txtEmail = $('#txtEmail').val();
        var txtPhone = $('#txtPhone').val();
        var txtAddress = $('#txtAddress').val();
        var ddlRoleId = $('#ddlRole').val();
        //  
        var ddlLanguage = ''; //$('#ddlLanguage').val();
        var enabled = 0;
        if ($('input[name="cbxActive"]').is(":checked"))
            enabled = 1;
        //
        var isBlock = false;
        if ($('input[name="cbxBlock"]').is(":checked"))
            isBlock = true;
        //
        var file = "";
        var model = {
            FullName: txtFullName,
            Birthday: txtBirthday,
            ImageFile: file,
            Email: txtEmail,
            Phone: txtPhone,
            Address: txtAddress,
            LoginID: txtLoginID,
            Password: txtPassword,
            LanguageID: ddlLanguage,
            RoleID: ddlRoleId,
            IsBlock: isBlock,
            Enabled: enabled
        };
        // form
        AjaxFrom.POST({
            url: URLC + '/Create',
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
    Update: function () {
        var txtID = $('#txtID').val();
        var txtLoginID = $('#txtLoginID').val();
        var txtPassword = $('#txtPassword').val();
        var txtFullName = $('#txtFullName').val();
        var txtBirthday = $('#txtBirthday').val();
        var txtEmail = $('#txtEmail').val();
        var txtPhone = $('#txtPhone').val();
        var txtAddress = $('#txtAddress').val();
        var ddlRoleId = $('#ddlRole').val();
        //  
        var ddlLanguage = ''; //$('#ddlLanguage').val();
        var enabled = 0;
        if ($('input[name="cbxActive"]').is(":checked"))
            enabled = 1;
        //
        var isBlock = false;
        if ($('input[name="cbxBlock"]').is(":checked"))
            isBlock = true;

        console.log("::" + isBlock);
        //
        var file = "";
        var model = {
            ID: txtID,
            FullName: txtFullName,
            Birthday: txtBirthday,
            ImageFile: file,
            Email: txtEmail,
            Phone: txtPhone,
            Address: txtAddress,
            LoginID: txtLoginID,
            Password: txtPassword,
            LanguageID: ddlLanguage,
            RoleID: ddlRoleId,
            IsBlock: isBlock,
            Enabled: enabled
        };
        AjaxFrom.POST({
            url: URLC + '/Update',
            data: model,
            type: 'POST',
            dataType: 'json',
            success: function (response) {
                if (response !== null) {
                    if (response.status === 200) {
                        Notifization.Success(response.message);
                        //location.reload();
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
                console.log('::' + response);
            }
        });
    },
    ConfirmDelete: function (id) {
        Confirm.DeleteYN(id, DevAccountController.Delete, null, null);
    },
    Delete: function (id) {
        var model = {
            ID: id
        };
        $.ajax({
            url: URLC + '/Delete',
            data: model,
            type: 'POST',
            dataType: 'json',
            success: function (response) {
                if (response !== null) {
                    if (response.status === 200) {
                        Notifization.Success(response.message);
                        DevAccountController.DataList(pageIndex);
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
    Block: function (id) {
        var fData = {
            Id: id
        };
        $.ajax({
            url: URLC + '/block',
            data: {
                strData: JSON.stringify(fData)
            },
            type: 'POST',
            dataType: 'json',
            success: function (response) {
                if (response !== null) {
                    if (response.status === 200) {
                        Notifization.Success(response.message);
                        DevAccountController.DataList(pageIndex);
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
    Unlock: function (id) {
        var fData = {
            Id: id
        };
        $.ajax({
            url: URLC + '/unlock',
            data: {
                strData: JSON.stringify(fData)
            },
            type: 'POST',
            dataType: 'json',
            success: function (response) {
                if (response !== null) {
                    if (response.status === 200) {
                        Notifization.Success(response.message);
                        DevAccountController.DataList(pageIndex);
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
    Active: function (id) {
        var fData = {
            Id: id
        };
        $.ajax({
            url: URLC + '/active',
            data: {
                strData: JSON.stringify(fData)
            },
            type: 'POST',
            dataType: 'json',
            success: function (response) {
                if (response !== null) {
                    if (response.status === 200) {
                        Notifization.Success(response.message);
                        DevAccountController.DataList(pageIndex);
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
    UnActive: function (id) {
        var fData = {
            Id: id
        };
        $.ajax({
            url: URLC + '/unactive',
            data: {
                strData: JSON.stringify(fData)
            },
            type: 'POST',
            dataType: 'json',
            success: function (response) {
                if (response !== null) {
                    if (response.status === 200) {
                        Notifization.Success(response.message);
                        DevAccountController.DataList(pageIndex);
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
    ChangePassword: function () {
        var txtOldPassword = $('#txtOldPassword').val();
        var txtNewPassword = $('#txtNewPassword').val();
        var txtReNewPassword = $('#txtReNewPassword').val();
        var model = {
            Password: txtOldPassword,
            NewPassword: txtNewPassword,
            ReNewPassword: txtReNewPassword
        };
        AjaxFrom.POST({
            url: URLC + '/changepassword',
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
DevAccountController.init();
$(document).on("keyup", "#txtFirstName", function () {
    var txtFirstName = $(this).val();
    if (txtFirstName === '') {
        $('#lblFirstName').html('Không được để trống họ/tên đệm');
    }
    else if (txtFirstName.length < 2 || txtFirstName.length > 30) {
        $('#lblFirstName').html('Họ/tên đệm giới hạn [2-30] ký tự');
    }
    else if (!FormatFName.test(txtFirstName)) {
        $('#lblFirstName').html('Họ/tên đệm không hợp lệ');
    }
    else {
        $('#lblFirstName').html('');
    }

});
$(document).on("keyup", "#txtFullName", function () {
    var txtFullName = $(this).val();
    if (txtFullName === '') {
        $('#lblFullName').html('Không được để trống tên');
    }
    else if (txtFullName.length < 2 || txtFullName.length > 30) {
        $('#lblFullName').html('Họ tên giới hạn [2-30] ký tự');
    }
    else if (!FormatFName.test(txtFullName)) {
        $('#lblFullName').html('Họ tên không hợp lệ');
    }
    else {
        $('#lblFullName').html('');
    }
});
$(document).on("keyup", "#txtBirthday", function () {
    var txtBirthday = $(this).val();
    if (txtBirthday !== '') {
        if (!FormatDateVN.test(txtBirthday)) {
            $('#lblBirthday').html('Ngày sinh không hợp lệ');
        }
        else {
            $('#lblBirthday').html('');
        }
    }
});
$(document).on("keyup", "#txtEmail", function () {
    var txtEmail = $(this).val();
    if (txtEmail === "") {
        $('#lblEmail').html('Không được để trống địa chỉ email');
    }
    else {
        if (!FormatEmail.test(txtEmail)) {
            $('#lblEmail').html('Địa chỉ email không hợp lệ');
        }
        else {
            $('#lblEmail').html('');
        }
    }
});

$(document).on("keyup", "#txtPhone", function () {
    var txtPhone = $(this).val();
    if (txtPhone !== '') {
        if (!FormatPhone.test(txtPhone)) {
            $('#lblPhone').html('Số điện thoại không hợp lệ');
        }
        else {
            $('#lblPhone').html('');
        }
    }
});
$(document).on("keyup", "#txtAddress", function () {
    var txtAddress = $(this).val();
    if (txtAddress !== "") {
        if (!FormatKeyword.test(txtAddress)) {
            $('#lblAddress').html('Địa chỉ không hợp lệ');
        }
        else {
            $('#lblAddress').html('');
        }
    }
});
// account
$(document).on("keyup", "#txtLoginID", function () {
    var txtLoginID = $(this).val();
    if (txtLoginID === '') {
        $('#lblLoginID').html('Không được để trống tài khoản');
    }
    else if (!FormatUser.test(txtLoginID)) {
        $('#lblLoginID').html('Tài khoản không hợp lệ');
    }
    else if (txtLoginID.length < 4 || txtLoginID.length > 16) {
        $('#lblLoginID').html('Tài khoản giới hạn [4-16] ký tự');
    }
    else {

        $('#lblLoginID').html('');
    }
});
$(document).on("keyup", "#txtPassword", function () {
    var txtPassID = $(this).val();
    if (txtPassID === "") {
        $('#lblPassword').html('Không được để trống mật khẩu');
        $('#txtRePassID').val('');
        $('#lblRePassID').html('');

    } else if (txtPassID.length < 4 || txtPassID.length > 16) {
        $('#lblPassword').html('Mật khẩu giới hạn [4-16] ký tự');
        $('#txtRePassID').val('');
        $('#lblRePassID').html('');

    }
    else if (!FormatPass.test(txtPassID)) {
        $('#lblPassword').html('Yêu cầu mật khẩu bảo mật hơn');
        $('#txtRePassID').val('');
        $('#lblRePassID').html('');

    }
    else {
        $('#lblPassword').html('');
    }
});
$(document).on("keyup", "#txtRePassword", function () {
    let txtRePassID = $(this).val();
    let txtPassID = $("#txtPassword").val();
    if (txtRePassword === "") {
        $('#lblRePassword').html("Vui lòng xác nhận mật khẩu");
    }
    else if (txtRePassID !== txtPassID) {
        $('#lblRePassword').html('Xác nhận mật khẩu chưa đúng');
    }
    else {
        $('#lblRePassword').html('');
    }
});
//
$(document).on("change", "#ddlLanguage", function () {
    var ddlLanguage = $(this).val();
    if (ddlLanguage === "") {
        $('#lblLanguage').html('Vui lòng chọn ngôn ngữ');
    }
    else {
        $('#lblLanguage').html('');
    }
});

function GetRole(_id, isChangeEvent) {
    var option = `<option value="">-Nhóm quyền-</option>`;
    $('#ddlRole').html(option);
    $('#ddlRole').selectpicker('refresh');
    AjaxFrom.POST({
        url: '/Development/CMSRole/Action/DropdownList',
        data: model,
        async: true,
        success: function (result) {
            if (result !== null) {
                if (result.status === 200) {

                    var attrSelect = '';
                    $.each(result.data, function (index, item) {
                        var id = item.ID;
                        if (_id !== undefined && _id != "" && _id === item.ID) {
                            attrSelect = "selected";
                        } else if (index == 0 && _id != "-1") {
                            attrSelect = "selected";
                        }
                        else {
                            attrSelect = '--';
                        }
                        option += `<option value='${id}' ${attrSelect}>${item.Title}</option>`;
                    });
                    console.log('::' + attrSelect);
                    $('#ddlRole').html(option);
                    setTimeout(function () {
                        $('#ddlRole').selectpicker('refresh');
                        if (isChangeEvent !== undefined && isChangeEvent == true && attrSelect !== '') {
                            $('#ddlRole').change();
                        }
                    }, 1000);
                    return;
                }
                else {
                    //Notifization.Error(result.message);
                    console.log('::' + result.message);
                    return;
                }
            }
            Notifization.Error(MessageText.NotService);
            return;
        },
        error: function (result) {
            console.log('::' + MessageText.NotService);
        }
    });
}