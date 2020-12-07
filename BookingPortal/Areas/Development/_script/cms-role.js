var pageIndex = 1;
var URLC = "/Development/CMSRole/Action";
var URLA = "/Development/CMSRole";
var RoleController = {
    init: function () {
        RoleController.registerEvent();
    },
    registerEvent: function () {
        $('#btnCreate').off('click').on('click', function () {
            var flg = true;
            var areaId = $('#ddlArea').val();
            var txtTitle = $('#txtTitle').val();
            var txtSummary = $('#txtSummary').val();
            // area
            if (areaId == "") {
                $('#lblArea').html('Vui lòng chọn phân vùng');
                flg = false;
            }
            else {
                $('#lblArea').html('');
            }
            // title
            if (txtTitle === '') {
                $('#lblTitle').html('Không được để trống tên nhóm quyền');
                flg = false;
            }
            else if (txtTitle.length < 1 || txtTitle.length > 80) {
                $('#lblTitle').html('Tên nhóm quyền giới hạn từ 1-> 80 ký tự');
                flg = false;
            }
            else if (!FormatKeyword.test(txtTitle)) {
                $('#lblTitle').html('Tên nhóm quyền không hợp lệ');
                flg = false;
            }
            else {
                $('#lblTitle').html('');
            }
            // level
            var txtLevel = $('#txtLevel').val();
            if (txtLevel === '') {
                $('#lblLevel').html('Không được để trống cấp độ');
                flg = false;
            }
            else if (!FormatNumber.test(txtLevel)) {
                $('#lblLevel').html('Xin vui lòng sử dụng số 0-9');
                flg = false;
            }
            else {
                $('#lblLevel').html('');
            }
            // summary
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
                RoleController.Create();
            }
            else {
                Notifization.Error(MessageText.Datamissing);
            }

        });
        $('#btnSearch').off('click').on('click', function () {
            RoleController.DataList(1);
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
                $('#lblTitle').html('Tên nhóm quyền giới hạn từ 1-> 80 ký tự');
                flg = false;
            }
            else if (!FormatKeyword.test(txtTitle)) {
                $('#lblTitle').html('Tên nhóm quyền không hợp lệ');
                flg = false;
            }
            else {
                $('#lblTitle').html('');
            }

            var txtLevel = $('#txtLevel').val();
            if (txtLevel === '') {
                $('#lblLevel').html('Không được để trống cấp độ');
                flg = false;
            }
            else if (!FormatNumber.test(txtLevel)) {
                $('#lblLevel').html('Xin vui lòng sử dụng số 0-9');
                flg = false;
            }
            else {
                $('#lblLevel').html('');
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
                RoleController.Update();
            }
            else {
                Notifization.Error(MessageText.Datamissing);
            }
        });
    },
    DataList: function (page) {
        var model = {
            Query: $('#txtQuery').val(),
            Page: page,
            Status: $('#ddlStatus').val()
        };
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
                            var role = result.role;

                            var action = '';
                            if (role !== undefined && role !== null) {
                                action = `<div class='ddl-action'><span><i class='fa fa-caret-down'></i></span>
                                              <div class='ddl-action-content'>`;
                                if (role.Update)
                                    action += `<a href='${URLA}/update/${id}'><i class='fas fa-pen-square'></i>&nbsp;Edit</a>`;
                                if (role.Delete)
                                    action += `<a onclick="RoleController.ConfirmDelete('${id}')"><i class='fas fa-trash'></i>&nbsp;Delete</a>`;
                                if (role.Details)
                                    action += `<a href='${URLA}/detail/${id}'><i class='fas fa-info-circle'></i>&nbsp;Detail</a>`;
                                action += `</div>
                                           </div>`;
                            }

                            var rowNum = parseInt(index) + (parseInt(currentPage) - 1) * parseInt(pageSize);
                            rowData += `
                            <tr>
                                 <td class="text-right">${rowNum}&nbsp;</td>
                                 <td>${item.Title}</td>                                  
                                 <td>${item.Summary}</td>                                  
                                 <td>${item.CreatedBy}</td>                                  
                                 <td class="text-center">${HelperModel.StatusIcon(item.Enabled)}</td>
                                 <td class="text-center">${item.CreatedDate}</td>
                                 <td class="tbcol-action">${action}</td>
                            </tr>`;
                        });
                        $('tbody#TblData').html(rowData);
                        if (parseInt(totalPage) > 1) {
                            Paging.Pagination("#Pagination", totalPage, currentPage, RoleController.DataList);
                        }
                        return;
                    }
                    else {
                        Notifization.Error(result.message);
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
    Details: function () {
        var id = $('#txtID').val();
        if (id.length <= 0) {
            Notifization.Error(MessageText.NOTSERVICES);
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
    Create: function () {
        var ddlAreaId = $('#ddlArea').val();
        var txtTitle = $('#txtTitle').val();
        var txtSummary = $('#txtSummary').val();
        var txtLevel = $('#txtLevel').val();
        var enabled = 0;
        if ($('input[name="cbxActive"]').is(":checked"))
            enabled = 1;
        //
        var model = {
            AreaID: ddlAreaId,
            Title: txtTitle,
            Summary: txtSummary,
            Level: txtLevel,
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
                Notifization.Error(MessageText.NOTSERVICES);
                return;
            },
            error: function (response) {
                console.log('::' + MessageText.NOTSERVICES);
            }
        });

    },
    Update: function () {
        var id = $('#txtID').val();
        var ddlAreaId = $('#ddlArea').val();
        var txtTitle = $('#txtTitle').val();
        var txtSummary = $('#txtSummary').val();
        var txtLevel = $('#txtLevel').val();
        var enabled = 0;
        if ($('input[name="cbxActive"]').is(":checked"))
            enabled = 1;
        //
        var model = {
            Id: id,
            AreaID: ddlAreaId,
            Title: txtTitle,
            Summary: txtSummary,
            Level: txtLevel,
            Enabled: enabled
        };
        AjaxFrom.POST({
            url: URLC + '/Update',
            data: model,
            success: function (response) {
                if (response !== null) {
                    if (response.status === 200) {
                        Notifization.Success(response.message);
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
                console.log('::' + MessageText.NOTSERVICES);
            }
        });
    },
    Delete: function (id) {
        var model = {
            Id: id
        };
        AjaxFrom.POST({
            url: URLC + '/Delete',
            data: model,
            success: function (response) {
                if (response !== null) {
                    if (response.status === 200) {
                        Notifization.Success(response.message);
                        RoleController.DataList(pageIndex);
                        return;
                    }
                    else {
                        Notifization.Error(response.message);
                        return;
                    }
                }
                Notifization.Error(MessageText.NOTSERVICES);
                return;
            },
            error: function (response) {
                console.log('::' + MessageText.NOTSERVICES);
            }
        });
    },
    ConfirmDelete: function (id) {
        Confirm.DeleteYN(id, RoleController.Delete, null, null);
    }
};
RoleController.init();
$(document).on('keyup', '#txtTitle', function () {
    var txtTitle = $(this).val();
    if (txtTitle === '') {
        $('#lblTitle').html('Không được để trống tên nhóm quyền');
    }
    else if (txtTitle.length < 1 || txtTitle.length > 80) {
        $('#lblTitle').html('Tên nhóm quyền giới hạn từ 1-> 80 ký tự');
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

$(document).on('keyup', '#txtLevel', function () {
    var txtLevel = $(this).val();
    if (txtLevel === '') {
        $('#lblLevel').html('Không được để trống cấp độ');
    }
    else if (!FormatNumber.test(txtLevel)) {
        $('#lblLevel').html('Xin vui lòng sử dụng số 0-9');
    }
    else {
        $('#lblLevel').html('');
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

$(document).on('change', '#ddlArea', function (index, item) {
    var page = $(this).data("action");
    if (page == undefined || page == null || page == "") {
        console.log(">>:None page");
        return;
    }
    var areaId = $(this).val();
    switch (page.toLowerCase()) {
        case "create":
            $('#lblArea').html('');
            if (areaId == "") {
                $('#lblArea').html('Vui lòng chọn phân vùng');
            }
            break;
        case "datalist":
            alert("o11k")
            break;
        default:
    }
})

function Role_AreaOption(_id, isChangeEvent) {
    var model = {

    };
    AjaxFrom.POST({
        url: '/Development/Area/Action/DropdownList',
        data: model,
        async: true,
        success: function (result) {
            if (result !== null) {
                if (result.status === 200) {
                    var option = `<option value="">-Phân vùng-</option>`;
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
                    $('#ddlArea').html(option);
                    setTimeout(function () {
                        $('#ddlArea').selectpicker('refresh');
                        if (isChangeEvent !== undefined && isChangeEvent == true && attrSelect !== '') {
                            $('#ddlArea').change();
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






