var pageIndex = 1;
var URLC = "/Development/Menu-Item/Action";
var URLA = "/Development/MenuItem";
//////var MenuCategoryController = {
//////    init: function () {
//////        MenuCategoryController.registerEvent();
//////    },
//////    registerEvent: function () {
//////        $('#btnCreate').off('click').on('click', function () {
//////            var flg = true;
//////            var txtTitle = $('#txtTitle').val();
//////            var txtSummary = $('#txtSummary').val();

//////            if (txtTitle === '') {
//////                $('#lblTitle').html('Không được để trống tiêu đề');
//////                flg = false;
//////            }
//////            else if (txtTitle.length < 1 || txtTitle.length > 80) {
//////                $('#lblTitle').html('Tiêu đề giới hạn từ 1-> 80 ký tự');
//////                flg = false;
//////            }
//////            else if (!FormatKeyword.test(txtTitle)) {
//////                $('#lblTitle').html('Tiêu đề không hợp lệ');
//////                flg = false;
//////            }
//////            else {
//////                $('#lblTitle').html('');
//////            }

//////            if (txtSummary !== '') {
//////                if (txtSummary.length > 120) {
//////                    $('#lblSummary').html('Mô tả giới hạn từ 1-> 120 ký tự');
//////                    flg = false;
//////                }
//////                else if (!FormatKeyword.test(txtSummary)) {
//////                    $('#lblSummary').html('Mô tả không hợp lệ');
//////                    flg = false;
//////                }
//////                else {
//////                    $('#lblSummary').html('');
//////                }
//////            }
//////            else {
//////                $('#lblSummary').html('');
//////            }
//////            // submit

//////            if (flg) {
//////                MenuCategoryController.Create();
//////            }
//////            else {
//////                Notifization.Error(MessageText.Datamissing);
//////            }
//////        });
//////        $('#btnSearch').off('click').on('click', function () {
//////            MenuCategoryController.DataList(1);
//////        });
//////        $('#btnUpdate').off('click').on('click', function () {
//////            var flg = true;
//////            var txtTitle = $('#txtTitle').val();
//////            var txtSummary = $('#txtSummary').val();

//////            if (txtTitle === '') {
//////                $('#lblTitle').html('Không được để trống tiêu đề');
//////                flg = false;
//////            }
//////            else if (txtTitle.length < 1 || txtTitle.length > 80) {
//////                $('#lblTitle').html('Tiêu đề giới hạn từ 1-> 80 ký tự');
//////                flg = false;
//////            }
//////            else if (!FormatKeyword.test(txtTitle)) {
//////                $('#lblTitle').html('Tiêu đề không hợp lệ');
//////                flg = false;
//////            }
//////            else {
//////                $('#lblTitle').html('');
//////            }

//////            if (txtSummary !== '') {
//////                if (txtSummary.length > 120) {
//////                    $('#lblSummary').html('Mô tả giới hạn từ 1-> 120 ký tự');
//////                    flg = false;
//////                }
//////                else if (!FormatKeyword.test(txtSummary)) {
//////                    $('#lblSummary').html('Mô tả không hợp lệ');
//////                    flg = false;
//////                }
//////                else {
//////                    $('#lblSummary').html('');
//////                }
//////            }
//////            else {
//////                $('#lblSummary').html('');
//////            }
//////            // submit
//////            if (flg) {
//////                MenuCategoryController.Update();
//////            }
//////            else {
//////                Notifization.Error(MessageText.Datamissing);
//////            }
//////        });
//////    },
//////    DataList: function (page) {
//////        var model = {
//////            Query: $('#txtQuery').val(),
//////            Page: page
//////        };
//////        AjaxFrom.POST({
//////            url: URLC + '/data-list',
//////            data: model,
//////            success: function (result) {
//////                $('tbody#TblData').html('');
//////                $('#Pagination').html('');
//////                if (result !== null) {
//////                    if (result.status === 200) {
//////                        var currentPage = 1;
//////                        var pagination = result.paging;
//////                        if (pagination !== null) {
//////                            totalPage = pagination.TotalPage;
//////                            currentPage = pagination.Page;
//////                            pageSize = pagination.PageSize;
//////                            pageIndex = pagination.Page;
//////                        }
//////                        var rowData = '';
//////                        $.each(result.data, function (index, item) {
//////                            index = index + 1;
//////                            var id = item.ID;
//////                            if (id.length > 0)
//////                                id = id.trim();
//////                            //  role
//////                            var role = result.role;
//////                            if (role !== undefined && role !== null) {
//////                                var action = `<div class='ddl-action'><span><i class='fa fa-caret-down'></i></span>
//////                                              <div class='ddl-action-content'>`;
//////                                if (role.Update)
//////                                    action += `<a href='${URLA}/update/${id}'><i class='fas fa-pen-square'></i>&nbsp;Edit</a>`;
//////                                if (role.Delete)
//////                                    action += `<a onclick="MenuCategoryController.ConfirmDelete('${id}')"><i class='fas fa-trash'></i>&nbsp;Delete</a>`;
//////                                if (role.Details)
//////                                    action += `<a href='${URLA}/detail/${id}'><i class='fas fa-info-circle'></i>&nbsp;Detail</a>`;
//////                                action += `</div>
//////                                           </div>`;
//////                            }

//////                            var rowNum = parseInt(index) + (parseInt(currentPage) - 1) * parseInt(pageSize);
//////                            rowData += `
//////                            <tr>
//////                                 <td class="text-right">${rowNum}&nbsp;</td>
//////                                 <td>${item.Title}</td>                                  
//////                                 <td>${item.Summary}</td>                                  
//////                                 <td class='tbcol-created'>${item.CreatedBy}</td>                                  
//////                                 <td class="text-center">${HelperModel.StatusIcon(item.Enabled)}</td>
//////                                 <td class="text-center">${item.CreatedDate}</td>
//////                                 <td class="tbcol-action">${action}</td>
//////                            </tr>`;
//////                        });
//////                        $('tbody#TblData').html(rowData);
//////                        if (parseInt(totalPage) > 1) {
//////                            Paging.Pagination("#Pagination", totalPage, currentPage, MenuCategoryController.DataList);
//////                        }
//////                        return;
//////                    }
//////                    else {
//////                        //Notifization.Error(result.message);
//////                        console.log('::' + result.message);
//////                        return;
//////                    }
//////                }
//////                Notifization.Error(MessageText.NotService);
//////                return;
//////            },
//////            error: function (result) {
//////                console.log('::' + MessageText.NotService);
//////            }
//////        });
//////    },
//////    Create: function () {
//////        var title = $('#txtTitle').val();
//////        var summary = $('#txtSummary').val();
//////        var enabled = 0;
//////        if ($('input[name="cbxActive"]').is(":checked"))
//////            enabled = 1;
//////        //
//////        var model = {
//////            Title: title,
//////            Summary: summary,
//////            Enabled: enabled
//////        };
//////        AjaxFrom.POST({
//////            url: URLC + '/create',
//////            data: model,
//////            success: function (response) {
//////                if (response !== null) {
//////                    if (response.status === 200) {
//////                        Notifization.Success(response.message);
//////                        FData.ResetForm();
//////                        return;
//////                    }
//////                    else {
//////                        Notifization.Error(response.message);
//////                        return;
//////                    }
//////                }
//////                Notifization.Error(MessageText.NotService);
//////                return;
//////            },
//////            error: function (response) {
//////                console.log('::' + MessageText.NotService);
//////            }
//////        });

//////    },
//////    Update: function () {
//////        var title = $('#txtTitle').val();
//////        var summary = $('#txtSummary').val();
//////        var id = $('#txtID').val();
//////        var enabled = 0;
//////        if ($('input[name="cbxActive"]').is(":checked"))
//////            enabled = 1;
//////        //
//////        var model = {
//////            Id: id,
//////            Title: title,
//////            Summary: summary,
//////            Enabled: enabled
//////        };
//////        AjaxFrom.POST({
//////            url: URLC + '/Update',
//////            data: model,
//////            success: function (response) {
//////                if (response !== null) {
//////                    if (response.status === 200) {
//////                        Notifization.Success(response.message);
//////                        return;
//////                    }
//////                    else {
//////                        Notifization.Error(response.message);
//////                        return;
//////                    }
//////                }
//////                Notifization.Error(MessageText.NotService);
//////                return;
//////            },
//////            error: function (response) {
//////                console.log('::' + MessageText.NotService);
//////            }
//////        });
//////    },
//////    Delete: function (id) {
//////        var model = {
//////            Id: id
//////        };
//////        AjaxFrom.POST({
//////            url: URLC + '/Delete',
//////            data: model,
//////            success: function (response) {
//////                if (response !== null) {
//////                    if (response.status === 200) {
//////                        Notifization.Success(response.message);
//////                        MenuCategoryController.DataList(pageIndex);
//////                        return;
//////                    }
//////                    else {
//////                        Notifization.Error(response.message);
//////                        return;
//////                    }
//////                }
//////                Notifization.Error(MessageText.NotService);
//////                return;
//////            },
//////            error: function (response) {
//////                console.log('::' + MessageText.NotService);
//////            }
//////        });
//////    },
//////    Details: function () {
//////        var id = $('#txtID').val();
//////        if (id.length <= 0) {
//////            Notifization.Error(MessageText.NotService);
//////            return;
//////        }
//////        var fData = {
//////            Id: $('#txtID').val()
//////        };
//////        $.ajax({
//////            url: '/post/detail',
//////            data: {
//////                strData: JSON.stringify(fData)
//////            },
//////            type: 'POST',
//////            dataType: 'json',
//////            success: function (result) {
//////                if (result !== null) {
//////                    if (result.status === 200) {
//////                        var item = result.data;
//////                        $('#LblAccount').html(item.LoginID);
//////                        $('#LblDate').html(item.CreatedDate);
//////                        var action = '';
//////                        if (item.Enabled)
//////                            action += `<i class='fa fa-toggle-on'></i> actived`;
//////                        else
//////                            action += `<i class='fa fa-toggle-off'></i>not active`;

//////                        $('#LblActive').html(action);
//////                        $('#lblLastName').html(item.FirstName + ' ' + item.LastName);
//////                        $('#LblEmail').html(item.Email);
//////                        $('#LblPhone').html(item.Phone);
//////                        $('#LblLanguage').html(item.LanguageID);
//////                        $('#LblPermission').html(item.PermissionID);

//////                        return;
//////                    }
//////                    else {
//////                        Notifization.Error(result.message);
//////                        return;
//////                    }
//////                }
//////                Notifization.Error(MessageText.NotService);
//////                return;
//////            },
//////            error: function (result) {
//////                console.log('::' + MessageText.NotService);
//////            }
//////        });
//////    },
//////    ConfirmDelete: function (id) {
//////        Confirm.Delete(id, MenuCategoryController.Delete, null, null);

//////    },
//////    KeywordList: function (page) {
//////        var model = {
//////            Query: $('#txtQuery').val(),
//////            Page: page
//////        };
//////        AjaxFrom.POST({
//////            url: URLC + '/Menu-Keyword',
//////            data: model,
//////            success: function (result) {
//////                $('tbody#TblData').html('');
//////                $('#Pagination').html('');
//////                if (result !== null) {
//////                    if (result.status === 200) {
//////                        var currentPage = 1;
//////                        var pagination = result.paging;
//////                        if (pagination !== null) {
//////                            totalPage = pagination.TotalPage;
//////                            currentPage = pagination.Page;
//////                            pageSize = pagination.PageSize;
//////                            pageIndex = pagination.Page;
//////                        }
//////                        var rowData = '';
//////                        $.each(result.data, function (index, item) {
//////                            index = index + 1;
//////                            var id = item.ID;
//////                            if (id.length > 0)
//////                                id = id.trim();
//////                            //  role
//////                            var rowNum = parseInt(index) + (parseInt(currentPage) - 1) * parseInt(pageSize);
//////                            rowData += `
//////                            <tr>
//////                                 <td class="text-right">${rowNum}&nbsp;</td>
//////                                 <td>${item.Title}</td>                                  
//////                                 <td>${item.Summary}</td>                                  
//////                                 <td class='tbcol-created'>${item.CreatedBy}</td>                                  
//////                                 <td class="text-center">${HelperModel.StatusIcon(item.Enabled)}</td>
//////                                 <td class="text-center">${item.CreatedDate}</td>
//////                                 <td class="tbcol-action">${action}</td>
//////                            </tr>`;
//////                        });
//////                        $('tbody#TblData').html(rowData);
//////                        if (parseInt(totalPage) > 1) {
//////                            Paging.Pagination("#Pagination", totalPage, currentPage, MenuCategoryController.DataList);
//////                        }
//////                        return;
//////                    }
//////                    else {
//////                        //Notifization.Error(result.message);
//////                        console.log('::' + result.message);
//////                        return;
//////                    }
//////                }
//////                Notifization.Error(MessageText.NotService);
//////                return;
//////            },
//////            error: function (result) {
//////                console.log('::' + MessageText.NotService);
//////            }
//////        });
//////    },

//////};
//////MenuCategoryController.init();
//////$(document).on('keyup', '#txtTitle', function () {
//////    var txtTitle = $(this).val();
//////    if (txtTitle === '') {
//////        $('#lblTitle').html('Không được để trống tiêu đề');
//////    }
//////    else if (txtTitle.length < 1 || txtTitle.length > 80) {
//////        $('#lblTitle').html('Tiêu đề giới hạn từ 1-> 80 ký tự');
//////    }
//////    else if (!FormatKeyword.test(txtTitle)) {
//////        $('#lblTitle').html('Tiêu đề không hợp lệ');
//////    }
//////    else {
//////        $('#lblTitle').html('');
//////    }
//////});
//////$(document).on('keyup', '#txtSummary', function () {
//////    var txtSummary = $(this).val();
//////    if (txtSummary !== '') {
//////        if (txtSummary.length > 120) {
//////            $('#lblSummary').html('Mô tả giới hạn từ 1-> 120 ký tự');
//////            flg = false;
//////        }
//////        else if (!FormatKeyword.test(txtSummary)) {
//////            $('#lblSummary').html('Mô tả không hợp lệ');
//////            flg = false;
//////        }
//////        else {
//////            $('#lblSummary').html('');
//////        }
//////    }
//////    else {
//////        $('#lblSummary').html('');
//////    }
//////});
$(document).on('click', '#btnSyncControl', function () {
    var model = {
        Query: '',
        Page: 0
    };
    AjaxFrom.POST({
        url: URLC + '/Menu-Sync',
        data: model,
        async: true,
        success: function (result) {
            if (result !== null) {
                if (result.status === 200) {
                    Notifization.Success(result.message);
                    Loading.PageLoad();
                    // get menu
                    GetMenuArea("", true);
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
//////$(document).on('click', 'input[type=checkbox].action-all-input', function (index, item) {
//////    if ($(this).is(":checked"))
//////        $(this).closest('div.list-group-item').find('.action-item-input').prop('checked', true);
//////    else
//////        $(this).closest('div.list-group-item').find('.action-item-input').prop('checked', false);
//////});
//////$(document).on('click', 'input[type=checkbox].action-item-input', function (index, item) {
//////    var itemControl = $(this).closest('div.list-group-item');
//////    if (itemControl === undefined || itemControl === null)
//////        return;
//////    //
//////    if ($(this).is(":checked")) {
//////        var notcheck = $(itemControl).find('input.action-item-input:checkbox:not(:checked)');
//////        if (notcheck.length == 0)
//////            $(itemControl).find('.action-all-input').prop('checked', true);
//////    }
//////    else {
//////        $(itemControl).find('.action-all-input').prop('checked', false);
//////    }

//////});
//////$(document).on('change', '#ddlAreaID', function (index, item) {
//////    GetDataOptionList();
//////})
////////
//////$(document).on('click', '#btnCreate', function (index, item) {

//////});
function GetMenuArea(_id, isChangeEvent) {
    var model = {
    };
    AjaxFrom.POST({
        url: '/Development/Area/Action/DropdownList',
        data: model,
        async: true,
        success: function (result) {
            if (result !== null) {
                if (result.status === 200) {
                    var option = `<option value="">-Lựa chọn-</option>`;
                    $.each(result.data, function (index, item) {
                        var id = item.ID;
                        var attrSelect = '';
                        if (_id !== undefined && _id != "" && _id === item.ID) {
                            attrSelect = "selected";
                        } else if (index == 0) {
                            attrSelect = "selected";
                        }
                        option += `<option value='${id}' ${attrSelect}>${item.Title}</option>`;
                    });
                    $('#ddlAreaID').html(option);
                    setTimeout(function () {
                        $('#ddlAreaID').selectpicker('refresh');
                        if (isChangeEvent !== undefined && isChangeEvent == true) {
                            $('#ddlAreaID').change();
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
//function GetDataOptionList() {
//    var model = {
//        RouteArea: $('#ddlAreaID').val(),
//    };
//    AjaxFrom.POST({
//        url: URLC + '/Menu-Sync-List',
//        data: model,
//        success: function (result) {
//            if (result !== null) {
//                if (result.status === 200) {
//                    var rowData = '';
//                    var actCountMax = 0;
//                    $.each(result.data, function (index, item) {
//                        if (item.ActionCount > actCountMax)
//                            actCountMax = item.ActionCount;
//                    });
//                    // 
//                    $.each(result.data, function (index, item) {
//                        index = index + 1;
//                        var ctrlId = item.ID;
//                        var ctrlStrId = 'cbx' + item.ID;
//                        var ctrlTitle = item.Title;

//                        var htmlAction = '';
//                        var actionList = item.Actions;
//                        htmlAction += '<hr class="m-t-5" />';
//                        htmlAction += `<div class="form-group">
//                                                    <input id="actAll${ctrlId}" type="checkbox" class="filled-in action-all-input" value="1" />
//                                                    <label style="margin-top:5px;" for="actAll${ctrlId}"><b>All action</b><span class="badge-icon bg-purple">${item.ActionCount}</span></label>
//                                               </div>`;
//                        if (actionList !== undefined && actionList !== null) {
//                            if (actionList.length > 0) { 
//                                $.each(actionList, function (acIndex, acItem) {
//                                    var actionId = acItem.ID;
//                                    var actionStrId = 'cbx' + acItem.ID;
//                                    var actionTitle = acItem.Title;
//                                    var isAPIAction = acItem.APIAction;
//                                    var strApi = '';
//                                    var clsApi = '';
//                                    if (isAPIAction) {
//                                        strApi = 'API: ';
//                                        clsApi = '-isapi';
//                                    }

//                                    actionTitle = strApi + actionTitle;
//                                    htmlAction += `<div class="form-group action-item">
//                                                        <input id="${actionStrId}" data-CategoryID='${ctrlId}' type="checkbox" class="filled-in action-item-input ${clsApi} " value="${actionId}" />
//                                                        <label style="margin-top:5px;" for="${actionStrId}">${actionTitle}</label>
//                                                   </div>`;

//                                });
//                            }
//                            if (actCountMax > 0) {
//                                var cntEmpty = parseInt(actCountMax) - parseInt(item.ActionCount);
//                                if (cntEmpty > 0) { 
//                                    for (var i = 0; i < cntEmpty; i++) {
//                                        htmlAction += `<div class="form-group"><label style="margin-top:5px;" for="">-</label></div>`;
//                                    }
//                                }
//                            }
//                        }
//                        // 
//                        rowData += `<div class="col-md-3">
//                                        <div class="list-group">
//                                           <div class="list-group-item control-item">
//                                                <div class="form-group">
//                                                    <input id="${ctrlStrId}" type="checkbox" class="filled-in control-item-input" value="${ctrlId}" />
//                                                    <label style="margin-top:5px;" for="${ctrlStrId}">${ctrlTitle}</label>
//                                                </div>
//                                                 ${htmlAction}
//                                            </div>
//                                        </div>
//                                    </div>`;
//                    });

//                    $("#MenuControlList").html(rowData);
//                    return;
//                }
//                else {
//                    //Notifization.Error(result.message);
//                    console.log('::' + result.message);
//                    return;
//                }
//            }
//            Notifization.Error(MessageText.NotService);
//            return;
//        },
//        error: function (result) {
//            console.log('::' + MessageText.NotService);
//        }
//    });
//}
