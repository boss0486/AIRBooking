var pageIndex = 1;
var URLC = "/user";
var UserController = {
    init: function () {
        UserController.registerEvent();
    },
    registerEvent: function () {
        $('#btnCreate').off('click').on('click', function () {
            var flg = true;
            var file = $('#ImageFile')[0].files[0];
            if (file !== '' && file !== undefined) {
                $('#lblFile').html('');
                if (!IsImageFile(file.name)) {
                    $('#lblFile').html('Tệp tin ảnh không hợp lệ');
                    flg = false;
                }
            }
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
            // fist name
            var txtFirstName = $('#txtFirstName').val();
            if (txtFirstName === '') {
                $('#lblFirstName').html('Không được để trống họ/tên đệm');
                flg = false;
            }
            else if (txtFirstName.length < 2 || txtFirstName.length > 30) {
                $('#lblFirstName').html('Họ/tên đệm giới hạn [2-30] ký tự');
                flg = false;
            }
            else if (!FormatFName.test(txtFirstName)) {
                $('#lblFirstName').html('Họ/tên đệm không hợp lệ');
                flg = false;
            }
            else {
                $('#lblFirstName').html('');
            }
            // full name
            var txtLastName = $('#txtLastName').val();
            if (txtLastName === '') {
                $('#lblLastName').html('Không được để trống tên');
                flg = false;
            }
            else if (txtLastName.length < 2 || txtLastName.length > 30) {
                $('#lblLastName').html('Họ tên giới hạn [2-30] ký tự');
                flg = false;
            }
            else if (!FormatFName.test(txtLastName)) {
                $('#lblLastName').html('Họ tên không hợp lệ');
                flg = false;
            }
            else {
                $('#lblLastName').html('');
            }
            // nick name
            var txtNickname = $('#txtNickname').val();
            if (txtNickname !== '') {
                if (txtNickname.length < 2 || txtNickname.length > 30) {
                    $('#lblNickname').html('Biệt danh giới hạn [2-30] ký tự');
                    flg = false;
                }
                else if (!FormatFName.test(txtNickname)) {
                    $('#lblNickname').html('Biệt danh không hợp lệ');
                    flg = false;
                }
                else {
                    $('#lblNickname').html('');
                }
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
            // bank
            //var ddlBankId = $('#ddlBankId').val();
            //if (ddlBankId === "") {
            //    $('#lblBankId').html('Vui lòng chọn ngân hàng');
            //    flg = false;
            //}
            //else {
            //    $('#lblBankId').html('');
            //}

            //var txtBankAccount = $('#txtBankAccount').val();
            //if (txtBankAccount === "") {
            //    $('#lblBankAccount').html('Không được để trống số tài khoản');
            //    flg = false;
            //}
            //else if (!regexKeyId.test(txtBankAccount)) {
            //    $('#lblBankAccount').html('Số tài khoản không hợp lệ');
            //    flg = false;
            //}
            //else {
            //    $('#lblBankAccount').html('');
            //}

            // WorkShift
            var ddlWorkShift = $('#ddlWorkShift').val();
            if (ddlWorkShift === "") {
                $('#lblWorkShift').html('Vui lòng chọn thời gian làm việc');
                flg = false;
            }
            else {
                $('#lblWorkShift').html('');
            }

            //var ddlDepartment = $('#ddlDepartment').val();
            //if (ddlDepartment === "") {
            //    $('#lblDepartment').html('Vui lòng chọn phòng ban');
            //    flg = false;
            //}
            //else {
            //    $('#lblDepartment').html('');
            //}
            //var ddlDepartmentPart = $('#ddlDepartmentPart').val();
            //if (ddlDepartmentPart === "") {
            //    $('#lblDepartmentPart').html('Vui lòng chọn bộ phận/team');
            //    flg = false;
            //}
            //else {
            //    $('#lblDepartmentPart').html('');
            //}
            //var roleGroup = [];
            //$('#ddlRoleGroup a.selected').each(function () {
            //    roleGroup.push($(this).data('val'));
            //});

            //var ddlLanguage = $('#ddlLanguage').val();
            //if (ddlLanguage === "") {
            //    $('#lblLanguage').html('Vui lòng chọn ngôn ngữ');
            //    flg = false;
            //}
            //else {
            //    $('#lblLanguage').html('');
            //}

            if (flg)
                UserController.Create();
            else
                Notifization.Error(MessageText.Datamissing);
        });
        $('#btnSearch').off('click').on('click', function () {
            UserController.DataList(1);
        });
        $('#btnUpdate').off('click').on('click', function () {
            var flg = true;
            var file = $('#ImageFile')[0].files[0];
            if (file !== '' && file !== undefined) {
                $('#lblFile').html('');
                if (!IsImageFile(file.name)) {
                    $('#lblFile').html('Tệp tin ảnh không hợp lệ');
                    flg = false;
                }
            }
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
                $('#lblLoginID').html('Tài khoản giới hạn [6-30] ký tự');
                flg = false;
            }
            else {
                $('#lblLoginID').html('');
            }
            // full name
            var txtLastName = $('#txtLastName').val();
            if (txtLastName === '') {
                $('#lblLastName').html('Không được để trống tên');
                flg = false;
            }
            else if (txtLastName.length < 2 || txtLastName.length > 30) {
                $('#lblLastName').html('Họ tên giới hạn [2-30] ký tự');
                flg = false;
            }
            else if (!FormatFName.test(txtLastName)) {
                $('#lblLastName').html('Họ tên không hợp lệ');
                flg = false;
            }
            else {
                $('#lblLastName').html('');
            }
            // nick name
            var txtNickname = $('#txtNickname').val();
            if (txtNickname !== '') {
                if (txtNickname.length < 2 || txtNickname.length > 30) {
                    $('#lblNickname').html('Biệt danh giới hạn [2-30] ký tự');
                    flg = false;
                }
                else if (!FormatFName.test(txtNickname)) {
                    $('#lblNickname').html('Biệt danh không hợp lệ');
                    flg = false;
                }
                else {
                    $('#lblNickname').html('');
                }
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
            // bank
            //var ddlBankId = $('#ddlBankId').val();
            //if (ddlBankId === "") {
            //    $('#lblBankId').html('Vui lòng chọn ngân hàng');
            //    flg = false;
            //}
            //else {
            //    $('#lblBankId').html('');
            //}

            //var txtBankAccount = $('#txtBankAccount').val();
            //if (txtBankAccount === "") {
            //    $('#lblBankAccount').html('Không được để trống số tài khoản');
            //    flg = false;
            //}
            //else if (!regexKeyId.test(txtBankAccount)) {
            //    $('#lblBankAccount').html('Số tài khoản không hợp lệ');
            //    flg = false;
            //}
            //else {
            //    $('#lblBankAccount').html('');
            //}

            // WorkShift
            var ddlWorkShift = $('#ddlWorkShift').val();
            if (ddlWorkShift === "") {
                $('#lblWorkShift').html('Vui lòng chọn thời gian làm việc');
                flg = false;
            }
            else {
                $('#lblWorkShift').html('');
            }

            //var ddlDepartment = $('#ddlDepartment').val();
            //if (ddlDepartment === "") {
            //    $('#lblDepartment').html('Vui lòng chọn phòng ban');
            //    flg = false;
            //}
            //else {
            //    $('#lblDepartment').html('');
            //}
            //var ddlDepartmentPart = $('#ddlDepartmentPart').val();
            //if (ddlDepartmentPart === "") {
            //    $('#lblDepartmentPart').html('Vui lòng chọn bộ phận/team');
            //    flg = false;
            //}
            //else {
            //    $('#lblDepartmentPart').html('');
            //}
            //var roleGroup = [];
            //$('#ddlRoleGroup a.selected').each(function () {
            //    roleGroup.push($(this).data('val'));
            //});

            //var ddlLanguage = $('#ddlLanguage').val();
            //if (ddlLanguage === "") {
            //    $('#lblLanguage').html('Vui lòng chọn ngôn ngữ');
            //    flg = false;
            //}
            //else {
            //    $('#lblLanguage').html('');
            //}

            if (flg)
                UserController.Update();
            else
                Notifization.Error(MessageText.Datamissing);
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
                UserController.ChangePassword();
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
            StartDate: LibDateTime.FormatToServerDate(txtStartDate),
            EndDate: LibDateTime.FormatToServerDate(txtEndDate),
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
                            //  role
                            var action = HelperModel.RolePermission(result.role, "UserController", id);
                            //
                            var rowNum = parseInt(index) + (parseInt(currentPage) - 1) * parseInt(pageSize);
                            rowData += `
                            <tr>
                                 <td class="text-right">${rowNum}&nbsp;</td>
                                 <td class="text-right">${item.IdentifierID}&nbsp;</td>
                                 <td>${item.LoginID}</td>                                  
                                 <td>${item.FirstName} ${item.LastName}</td>
                                 <td>${item.DepartmentName}</td>
                                 <td>${item.DepartmentPartName}</td>
                                 <td class="text-right">${item.WorkShiftName}</td>
                                 <td class="text-center">${item.IsBlock ? "<i class='fas fa-ban'></i>" : "<i class='far fa-dot-circle'></i>"}</td>
                                 <td class="text-center">${item.Enabled ? "<i class='fas fa-toggle-on'></i>" : "<i class='fas fa-toggle-off'></i>"}</td>
                                 <td class="text-center">${item.CreatedDate}</td>
                                 <td class="tbcol-action">${action}</td>
                            </tr>`;
                        });
                        $('tbody#TblData').html(rowData);
                        if (parseInt(totalPage) > 1) {
                            Paging.Pagination("#Pagination", totalPage, currentPage, UserController.DataList);
                        }
                        return;
                    }
                    else {
                        //Notifization.Error(result.message);
                        console.log('::' + result.message);
                        return;
                    }
                }
                //Message.Error(MessageText.NOTSERVICES);
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
        var txtFirstName = $('#txtFirstName').val();
        var txtLastName = $('#txtLastName').val();
        var txtFName = $('#txtFName').val();
        var txtNickname = $('#txtNickname').val();
        var txtBirthday = $('#txtBirthday').val();
        var txtEmail = $('#txtEmail').val();
        var txtPhone = $('#txtPhone').val();
        var txtAddress = $('#txtAddress').val();
        var ddlBankId = $('#ddlBankId').val();
        var txtBankAccount = $('#txtBankAccount').val();
        var workShiftID = $('#ddlWorkShift').val();
        //  seting
        var ddlDepartment = $('#ddlDepartment').val();
        var ddlDepartmentPart = $('#ddlDepartmentPart').val();
        //  
        var ddlLanguage = ''; //$('#ddlLanguage').val();

        var active = false;
        if ($('#cbxActive').is(":checked"))
            active = true;

        var file = $('#ImageFile')[0].files[0];
        var model = new FormData();
        model.append("DocumentFile", file);
        model.append("FirstName", txtFirstName);
        model.append("LastName", txtLastName);
        model.append("NickName", txtNickname);
        model.append("Birthday", txtBirthday);
        model.append("Email", txtEmail);
        model.append("Phone", txtPhone);
        model.append("Address", txtAddress);
        model.append("BankID", ddlBankId);
        model.append("BankAccount", txtBankAccount);
        model.append("LoginID", txtLoginID);
        model.append("Password", txtPassword);
        model.append("WorkShiftID", workShiftID);

        model.append("DepartmentID", ddlDepartment);
        model.append("DepartmentPartID", ddlDepartmentPart);
        model.append("LanguageID", ddlLanguage);
        model.append("Enabled", active);

        // form
        AjaxFrom.POSTFILE({
            url: URLC + '/create',
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
        var txtLoginID = $('#txtLoginID').val();
        var txtFirstName = $('#txtFirstName').val();
        var txtLastName = $('#txtLastName').val();
        var txtNickname = $('#txtNickname').val();
        var txtBirthday = $('#txtBirthday').val();
        var txtEmail = $('#txtEmail').val();
        var txtPhone = $('#txtPhone').val();
        var txtAddress = $('#txtAddress').val();
        var ddlBankId = $('#ddlBankId').val();
        var txtBankAccount = $('#txtBankAccount').val();
        var workShiftID = $('#ddlWorkShift').val();
        var ddlDepartment = $('#ddlDepartment').val();
        var ddlDepartmentPart = $('#ddlDepartmentPart').val();
        var ddlLanguage = '';

        var id = $('#txtID').val();
        var file = $('#ImageFile')[0].files[0];

        if (file === "undefined")
            file = "";

        var model = new FormData();
        model.append("DocumentFile", file);
        model.append("LoginID", txtLoginID);
        model.append("FirstName", txtFirstName);
        model.append("LastName", txtLastName);
        model.append("NickName", txtNickname);
        model.append("Birthday", txtBirthday);
        model.append("Email", txtEmail);
        model.append("Phone", txtPhone);
        model.append("WorkShiftID", workShiftID);
        model.append("DepartmentID", ddlDepartment);
        model.append("DepartmentPartID", ddlDepartmentPart);
        model.append("Address", txtAddress);
        model.append("BankID", ddlBankId);
        model.append("BankAccount", txtBankAccount);
        model.append("LanguageID", ddlLanguage);
        model.append("ID", id);

        AjaxFrom.POSTFILE({
            url: URLC + '/update',
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
        Confirm.Delete(id, UserController.Delete, null, null);
    },
    Delete: function (id) {
        var fData = {
            Id: id
        };
        $.ajax({
            url: URLC + '/delete',
            data: {
                strData: JSON.stringify(fData)
            },
            type: 'POST',
            dataType: 'json',
            success: function (response) {
                if (response !== null) {
                    if (response.status === 200) {
                        Notifization.Success(response.message);
                        UserController.DataList(pageIndex);
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
                        UserController.DataList(pageIndex);
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
                        UserController.DataList(pageIndex);
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
                        UserController.DataList(pageIndex);
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
                        UserController.DataList(pageIndex);
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
UserController.init();
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
$(document).on("keyup", "#txtLastName", function () {
    var txtLastName = $(this).val();
    if (txtLastName === '') {
        $('#lblLastName').html('Không được để trống tên');
    }
    else if (txtLastName.length < 2 || txtLastName.length > 30) {
        $('#lblLastName').html('Họ tên giới hạn [2-30] ký tự');
    }
    else if (!FormatFName.test(txtLastName)) {
        $('#lblLastName').html('Họ tên không hợp lệ');
    }
    else {
        $('#lblLastName').html('');
    }
});
$(document).on("keyup", "#txtNickname", function () {
    var txtNickname = $(this).val();
    if (txtNickname !== '') {
        if (txtNickname.length < 2 || txtNickname.length > 30) {
            $('#lblNickname').html('Biệt danh giới hạn [2-30] ký tự');
        }
        else if (!FormatFName.test(txtNickname)) {
            $('#lblNickname').html('Biệt danh không hợp lệ');
        }
        else {
            $('#lblNickname').html('');
        }
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
$(document).on("change", "#ddlBankId", function () {
    //var ddlBankId = $(this).val();
    //if (ddlBankId === "") {
    //    $('#lblBankId').html('Vui lòng chọn ngân hàng');
    //}
    //else {
    //    $('#lblBankId').html('');
    //}
});
$(document).on("keyup", "#txtBankAccount", function () {
    var txtBankAccount = $(this).val();
    if (txtBankAccount !== "") {
        if (!FormatKeyId.test(txtBankAccount)) {
            $('#lblBankAccount').html('Số tài khoản không hợp lệ');
        }
        else {
            $('#lblBankAccount').html('');
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
$(document).on("change", "#ddlLanguage", function () {
    var ddlLanguage = $(this).val();
    if (ddlLanguage === "") {
        $('#lblLanguage').html('Vui lòng chọn ngôn ngữ');
    }
    else {
        $('#lblLanguage').html('');
    }
});
//
$(document).on("click", "#btnFile", function () {
    $('#ImageFile').click();
});
$(document).on("change", "#ImageFile", function (elm) {
    var file = $(this)[0].files[0];
    if (file !== '') {
        $('#lblFile').html('');
        $('#fake-file-input-name').val($(this).val());
        if (!IsImageFile(file.name))
            $('#lblFile').html('Tệp tin ảnh không hợp lệ');
    }
});
$(document).on("change", "#ddlDepartment", function () {
    //var ddlDepartment = $(this).val();
    //if (ddlDepartment === "") {
    //    $('#lblDepartment').html('Vui lòng chọn phòng ban');
    //}
    //else {
    //    $('#lblDepartment').html('');
    //}
    //LoadDefaultOption();
    //LoadDepartmentPart();
    //$('#ddlDepartmentPart').change();
});
$(document).on("change", "#ddlDepartmentPart", function () {
    //var ddlDepartmentPart = $(this).val();
    //if (ddlDepartmentPart.length <= 0) {
    //    $('#lblDepartmentPart').html('Vui lòng chọn bộ phận/team');
    //}
    //else {
    //    $('#lblDepartmentPart').html('');
    //}
});
//
$(document).on("change", "#ddlWorkShift", function () {
    var ddlWorkShift = $(this).val();
    if (ddlWorkShift === "") {
        $('#lblWorkShift').html('Vui lòng chọn thời gian làm việc');
    }
    else {
        $('#lblWorkShift').html('');
    }
});

function LoadDepartmentPart() {

    var department = $('#ddlDepartment').val();
    if (department !== "") {
        var model = {
            DepartmentID: department
        };
        AjaxFrom.POST({
            url: '/departmentpart/option',
            data: model,
            success: function (result) {
                if (result !== null) {
                    if (result.data.length > 10) {
                        $('#ddlDepartmentPart').html(result.data);
                        $('select').selectpicker('refresh');
                        $('#ddlDepartmentPart').change();
                        return;
                    }
                    else {
                        LoadDefaultOption();
                        return;
                    }
                }
                //Notifization.Error(MessageText.NOTSERVICES);
                return;
            },
            error: function (result) {
                console.log('::' + MessageText.NotService);
            }
        });
    }

}
 