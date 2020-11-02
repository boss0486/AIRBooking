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
                            //  role 
                            var action = HelperModel.RolePermission(result.role, "UserGroupController", id);
                            //
                            var rowNum = parseInt(index) + (parseInt(currentPage) - 1) * parseInt(pageSize);
                            rowData += `
                            <tr>
                                 <td class="text-right">${rowNum}&nbsp;</td>
                                 <td>${item.Title}</td>                                  
                                 <td class="text-center">${HelperModel.StateIcon(item.IsAllowSpend)}</td>                                  
                                 <td>${item.CreatedBy}</td>                                  
                                 <td class="text-center">${HelperModel.StatusIcon(item.Enabled)}</td>
                                 <td class="text-center">${item.CreatedDate}</td>
                                 <td class="tbcol-action">${action}</td>
                            </tr>`;

                            var cnt = 1;
                            var subRoles = item.SubRoles;
                            if (subRoles != null && subRoles.length > 0) {
                                $.each(subRoles, function (subIndex, subItem) {
                                    var subAction = HelperModel.RolePermission(result.role, "UserGroupController", subItem.ID);
                                    rowData += `<tr>
                                          <td>&nbsp;</td>
                                          <td style='padding-left:30px;'>${cnt}. ${subItem.Title}</td>                                  
                                          <td class="text-center">${HelperModel.StateIcon(subItem.IsAllowSpend)}</td>                                
                                          <td>${subItem.CreatedBy}</td>                                  
                                          <td class="text-center">${HelperModel.StatusIcon(subItem.Enabled)}</td>
                                          <td class="text-center">${subItem.CreatedDate}</td>
                                          <td class="tbcol-action">${subAction}</td>
                                     </tr>`;
                                    cnt++;
                                });
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
    },
    ViewLevel: function (id, page) {
        var model = {
            Query: '',
            Page: 1,
            Status: -1
        };
        AjaxFrom.POST({
            url: URLC + '/DropDownList',
            data: model,
            success: function (result) {
                $('#ListGroup').html('');
                if (result !== null) {
                    if (result.status === 200) {
                        var rowData = '';
                        $.each(result.data, function (index, item) {
                            index = index + 1;
                            //
                            var strIndex = index;
                            if (index < 10)
                                strIndex = "0" + index;
                            //
                            var level = parseInt(item.Level);
                            var strLevel = level;
                            if (level < 10)
                                strLevel = "0" + level;
                            //
                            var active = '';
                            if (id != undefined && id == item.ID) {
                                active = 'active';
                            }
                            rowData += `                
                              <a class="list-group-item">
                               ${strIndex}. ${item.Title} <span class="badge badge-primary badge-pill ${active}">Cấp: ${strLevel}</span>
                              </a>`;
                        });
                        $('#ListGroup').html(rowData);
                        return;
                    }
                    else {
                        return;
                    }
                }
                return;
            },
            error: function (result) {
                console.log('::' + MessageText.NotService);
            }
        });
    },
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


$(document).on('click', '#cbxActive', function () {
    if ($(this).hasClass('actived')) {
        // remove
        $(this).children('i').removeClass('fa-check-square');
        $(this).children('i').addClass('fa-square');
        $(this).removeClass('actived');
    }
    else {
        $(this).children('i').addClass('fa-check-square');
        $(this).children('i').removeClass('fa-square');
        $(this).addClass('actived');
    }
});



