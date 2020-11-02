var pageIndex = 1;
var URLC = "/Management/Role/Action";
var URLA = "/Management/Role";
var UserGroupController = {
    init: function () {
        UserGroupController.registerEvent();
    },
    registerEvent: function () {
        $('#btnCreate').off('click').on('click', function () {
            var flg = true;
            var txtTitle = $('#txtTitle').val();
            var txtSummary = $('#txtSummary').val();

            if (txtTitle === '') {
                $('#lblTitle').html('Không được để trống tên nhóm quyền');
                flg = false;
            }
            else if (txtTitle.length < 1 || txtTitle.length > 80) {
                $('#lblTitle').html('Tên nhóm quyền giới hạn từ 1-> 80 characters');
                flg = false;
            }
            else if (!FormatKeyword.test(txtTitle)) {
                $('#lblTitle').html('Tên nhóm quyền không hợp lệ');
                flg = false;
            }
            else {
                $('#lblTitle').html('');
            }

            if (txtSummary !== '') {
                if (txtSummary.length > 120) {
                    $('#lblSummary').html('Mô tả giới hạn từ 1-> 120 ký tự');
                    flg = false;
                }
                else if (!FormatKeyword.test(txtSummary)) {
                    $('#lblSummary').html('Mô tả không hợp lệ');
                    flg = false;
                }
                else {
                    $('#lblSummary').html('');
                }
            }
            else {
                $('#lblSummary').html('');
            }

            if (flg) {
                UserGroupController.Create();
            }
            else {
                Notifization.Error(MessageText.Datamissing);
            }

        });
        $('#btnSearch').off('click').on('click', function () {
            UserGroupController.DataList(1);
        });
        $('#btnUpdate').off('click').on('click', function () {
            var flg = true;
            var txtTitle = $('#txtTitle').val();
            var txtSummary = $('#txtSummary').val();

            if (txtTitle === '') {
                $('#lblTitle').html('Không được để trống tên nhóm quyền');
                flg = false;
            }
            else if (txtTitle.length < 1 || txtTitle.length > 80) {
                $('#lblTitle').html('Tên nhóm quyền giới hạn từ 1-> 80 characters');
                flg = false;
            }
            else if (!FormatKeyword.test(txtTitle)) {
                $('#lblTitle').html('Tên nhóm quyền không hợp lệ');
                flg = false;
            }
            else {
                $('#lblTitle').html('');
            }
            if (txtSummary !== '') {
                if (txtSummary.length > 120) {
                    $('#lblSummary').html('Mô tả giới hạn từ 1-> 120 ký tự');
                    flg = false;
                }
                else if (!FormatKeyword.test(txtSummary)) {
                    $('#lblSummary').html('Mô tả không hợp lệ');
                    flg = false;
                }
                else {
                    $('#lblSummary').html('');
                }
            }
            else {
                $('#lblSummary').html('');
            }
            // submit
            if (flg) {
                UserGroupController.Update();
            }
            else {
                Notifization.Error(MessageText.Datamissing);
            }
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
                            //
                            var _title = SubStringText.SubTitle(item.Title);
                            // icon sort
                            var _level = 0;
                            var _orderId = item.OrderID;
                            var _actionSort = `<i data-sortup='btn-sort-up' data-id='${id}' data-order'${_orderId}' class='fas fa-arrow-circle-up icon-mnsort'></i> <i data-sortdown ='btn-sort-down' data-id='${id}' data-order'${_orderId}' class='fas fa-arrow-circle-down icon-mnsort'></i>`;
                            //
                            //  role 
                            var action = HelperModel.RolePermission(result.role, "UserGroupController", id);
                            //
                            var rowNum = parseInt(index) + (parseInt(currentPage) - 1) * parseInt(pageSize);
                            rowData += `
                            <tr>
                                 <td class="text-right">${rowNum}&nbsp;</td>
                                 <td>${_title}</td>                                  
                                 <td class="text-center">${HelperModel.StateIcon(item.IsAllowSpend)}</td>                                  
                                 <td class="text-center">${_actionSort}</td>                                  
                                 <td>${item.CreatedBy}</td>                                  
                                 <td class="text-center">${HelperModel.StatusIcon(item.Enabled)}</td>
                                 <td class="text-center">${item.CreatedDate}</td>
                                 <td class="tbcol-action">${action}</td>
                            </tr>`;
                            var subMenu = item.SubRoles;
                            if (subMenu !== undefined && subMenu !== null && subMenu.length > 0) {
                                rowData += UserGroupController.GetSubRoleList(index, subMenu, _level, result.role);
                            }
                        });
                        $('tbody#TblData').html(rowData);
                        if (parseInt(totalPage) > 1) {
                            Paging.Pagination("#Pagination", totalPage, currentPage, UserGroupController.DataList);
                        }
                        return;
                    }
                    else {
                        Notifization.Error(result.message);
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
    },
    GetSubRoleList: function (_index, lstModel, _level, _role) {
        var rowData = '';
        //
        if (lstModel.length > 0) {
            _level += 1;
            $.each(lstModel, function (index, item) {
                index = index + 1;
                var id = item.ID;
                if (id.length > 0)
                    id = id.trim();
                var action = '';
                var role = _role;
                //  role
                var action = HelperModel.RolePermission(role, "UserGroupController", id);
                //
                // icon sort
                var _orderId = item.OrderID;
                var _actionSort = `<i data-sortup='btn-sort-up' data-id='${id}' data-order'${_orderId}' class='fas fa-arrow-circle-up icon-mnsort'></i> <i data-sortdown ='btn-sort-down' data-id='${id}' data-order'${_orderId}' class='fas fa-arrow-circle-down icon-mnsort'></i>`;
                //
                var _title = SubStringText.SubTitle(item.Title);
                //var _summary = SubStringText.SubSummary(item.Summary);
                //var _cratedDate = LibDateTime.ConvertUnixTimestampToDate(item.CreatedDate, '-', 'en');
                var rowNum = ''; //parseInt(index) + (parseInt(currentPage) - 1) * parseInt(pageSize);
                var pading = _level * 20;
                rowData += `
                            <tr>
                                 <td class="text-right">${rowNum}&nbsp;</td>
                                 <td class='text-left'><div style='padding-left:${pading}px'>- ${_title}</div></td>                            
                                 <td class="text-center">${HelperModel.StateIcon(item.IsAllowSpend)}</td>                                  
                                 <td class="text-center">${_actionSort}</td>                                  
                                 <td>${item.CreatedBy}</td>                                  
                                 <td class="text-center">${HelperModel.StatusIcon(item.Enabled)}</td>
                                 <td class="text-center">${item.CreatedDate}</td>
                                 <td class="tbcol-action">${action}</td>
                            </tr>`;
                var subMenu = item.SubRoles;
                if (subMenu !== undefined && subMenu !== null && subMenu.length > 0) {
                    rowData += UserGroupController.GetSubRoleList(_index, subMenu, _level, _role);
                }
            });
        }
        return rowData;
    },
    Create: function () {
        var ddlCategory = $('#ddlCategory').val();
        var txtTitle = $('#txtTitle').val();
        var txtSummary = $('#txtSummary').val();
        var isAllowSpend = false;
        if ($('input[name="cbxIsAllowSpend"]').is(":checked"))
            isAllowSpend = true;
        //
        if (ddlCategory == null || ddlCategory == undefined) {
            ddlCategory = "";
        }
        var enabled = 0;
        if ($('input[name="cbxActive"]').is(":checked"))
            enabled = 1;
        //
        var model = {
            ParentID: ddlCategory,
            Title: txtTitle,
            Summary: txtSummary,
            IsAllowSpend: isAllowSpend,
            Level: 0,
            Enabled: enabled
        };
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
        var ddlCategory = $('#ddlCategory').val();
        var txtTitle = $('#txtTitle').val();
        var txtSummary = $('#txtSummary').val();
        var id = $('#txtID').val();
        //
        if (ddlCategory == null || ddlCategory == undefined) {
            ddlCategory = "";
        }
        var isAllowSpend = false;
        if ($('input[name="cbxIsAllowSpend"]').is(":checked"))
            isAllowSpend = true;
        //
        var enabled = 0;
        if ($('input[name="cbxActive"]').is(":checked"))
            enabled = 1;
        //
        var model = {
            ID: id,
            ParentID: ddlCategory,
            Title: txtTitle,
            Summary: txtSummary,
            IsAllowSpend: isAllowSpend,
            Level: 0,
            Enabled: enabled
        };
        AjaxFrom.POST({
            url: URLC + '/Update',
            data: model,
            success: function (response) {
                if (response !== null) {
                    if (response.status === 200) {
                        Notifization.Success(response.message);

                        //UserGroupController.ViewLevel(parseInt(txtLevel), 1);
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
    Details: function () {
        var id = $('#txtID').val();
        if (id.length <= 0) {
            Notifization.Error(MessageText.NotService);
            return;
        }
        var fData = {
            Id: $('#txtID').val()
        };
        $.ajax({
            url: URLC + 'detail',
            data: {
                strData: JSON.stringify(fData)
            },
            type: 'POST',
            dataType: 'json',
            success: function (result) {
                if (result !== null) {
                    if (result.status === 200) {
                        var item = result.data;
                        $('#LblAccount').html(item.LoginID);
                        $('#LblDate').html(item.CreatedDate);
                        var action = '';
                        if (item.Enabled)
                            action += `<i class='fa fa-toggle-on'></i> actived`;
                        else
                            action += `<i class='fa fa-toggle-off'></i>not active`;

                        $('#LblActive').html(action);
                        $('#lblLastName').html(item.FirstName + ' ' + item.LastName);
                        $('#LblEmail').html(item.Email);
                        $('#LblPhone').html(item.Phone);
                        $('#LblLanguage').html(item.LanguageID);
                        $('#LblPermission').html(item.PermissionID);

                        return;
                    }
                    else {
                        //Notifization.Error(result.message);
                        console.log('::' + result.message);
                        return;
                    }
                }
                Notifization.Error(MessageText.NOTSERVICES);
                return;
            },
            error: function (result) {
                console.log('::' + MessageText.NOTSERVICES);
            }
        });
    },
    Delete: function (id) {
        var model = {
            ID: id
        };
        AjaxFrom.POST({
            url: URLC + '/Delete',
            data: model,
            success: function (response) {
                if (response !== null) {
                    if (response.status === 200) {
                        Notifization.Success(response.message);
                        UserGroupController.DataList(pageIndex);
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
    ConfirmDelete: function (id) {
        Confirm.Delete(id, UserGroupController.Delete, null, null);
    }
};
UserGroupController.init();
$(document).on('keyup', '#txtTitle', function () {
    var txtTitle = $(this).val();
    if (txtTitle === '') {
        $('#lblTitle').html('Không được để trống tên nhóm quyền');
    }
    else if (txtTitle.length < 1 || txtTitle.length > 80) {
        $('#lblTitle').html('Tên nhóm quyền giới hạn từ 1-> 80 characters');
    }
    else if (!FormatKeyword.test(txtTitle)) {
        $('#lblTitle').html('Tên nhóm quyền không hợp lệ');
    }
    else {
        $('#lblTitle').html('');
    }
});
$(document).on('keyup', '#txtSummary', function () {
    var txtSummary = $(this).val();
    if (txtSummary !== '') {
        if (txtSummary.length > 120) {
            $('#lblSummary').html('Mô tả giới hạn từ 1-> 120 ký tự');
            flg = false;
        }
        else if (!FormatKeyword.test(txtSummary)) {
            $('#lblSummary').html('Mô tả không hợp lệ');
            flg = false;
        }
        else {
            $('#lblSummary').html('');
        }
    }
    else {
        $('#lblSummary').html('');
    }
});

// menu sort
$(document).on('click', '[data-sortup]', function () {
    var id = $(this).data('id');
    var model = {
        ID: id
    };
    AjaxFrom.POST({
        url: URLC + '/sortup',
        data: model,
        success: function (result) {
            if (result !== null) {
                if (result.status === 200) {
                    Notifization.Success(result.message);
                    UserGroupController.DataList(pageIndex);
                    return;
                }
                else {
                    Notifization.Error(result.message);
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
});
$(document).on('click', '[data-sortdown]', function () {
    var id = $(this).data('id');
    var model = {
        ID: id
    };
    AjaxFrom.POST({
        url: URLC + '/sortdown',
        data: model,
        success: function (result) {
            if (result !== null) {
                if (result.status === 200) {
                    Notifization.Success(result.message);
                    UserGroupController.DataList(pageIndex);
                    return;
                }
                else {
                    Notifization.Error(result.message);
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
});


