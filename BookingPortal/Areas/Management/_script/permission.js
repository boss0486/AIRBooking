var pageIndex = 1;
var URLC = "/Management/permission/Action";
var URLA = "/Management/permission";
var _PermissionController = {
    init: function () {
        _PermissionController.registerEvent();
    },
    registerEvent: function () {
    },
    PermissionList: function () {
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
                        var rowData = '';
                        $.each(result.data, function (index, item) {
                            index = index + 1;
                            var id = item.ID;
                            if (id.length > 0)
                                id = id.trim();
                            //  role
                            var action = HelperModel.RolePermission(result.role, "_PermissionController", id);
                            //
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
                        $('tbody#TblData').html('');
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
                        _PermissionController.DataList(pageIndex);
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
        _PermissionController.Delete(id);
    },
    FuncGroup: function (roleId, _activeId) {
        // model
        var model = {
            RoleID: roleId
        };
        AjaxFrom.POST({
            url: URLC + '/PermissionData',
            data: model,
            success: function (result) {
                $('#TblFunction > tbody').html('');
                if (result !== null) {
                    if (result.status === 200) {
                        var rowData = '';
                        $.each(result.data, function (index, item) {
                            index = index + 1;
                            var id = item.ID;
                            if (id.length > 0)
                                id = id.trim();
                            // icon sort
                            var _level = 0;
                            var _title = item.Title;
                            var _parentId = item.ParentID;
                            var rowNum = index;
                            // action 
                            var actionHtml = ``;
                            var actionTemp = ``;
                            var actionData = item.Actions;
                            var cntActionActive = 0
                            var totalCtrl = item.Total;
                            var staController = "";

                            var actionState = 'disabled';
                            if (item.Status) {
                                staController = "checked";
                                actionState = '';
                            }

                            if (actionData != null) {
                                $.each(actionData, function (actIndex, actItem) {
                                    var actId = actItem.ID;
                                    var actTitle = actItem.Title;
                                    var staAction = "";
                                    if (actItem.Status) {
                                        staAction = "checked";
                                        cntActionActive++;
                                    }
                                    actionTemp += `<div style='width:120px;display: inline-block;'> 
                                                        <input id="cbx${actId}" data-val="${actId}" type="checkbox" class="filled-in inp-action" ${staAction} ${actionState} />
                                                        <label for="cbx${actId}">${actTitle}</label>
                                                   </div>`;
                                });
                                //
                                if (actionTemp !== '') {
                                    var checkAllAction = '';
                                    if (parseInt(cntActionActive) == parseInt(actionData.length)) {
                                        checkAllAction = 'checked';
                                    }
                                    // check all action
                                    if (HelperModel.AccessInApplication() == RoleEnum.IsCMSUser || HelperModel.AccessInApplication() == RoleEnum.IsAdminInApplication) {
                                        actionHtml += `<div style='width:50px;display: inline-block;'> 
                                                        <input id="cbx-actall-${index}" data-val="" type="checkbox" class="filled-in all-action" ${checkAllAction}  ${actionState} />
                                                        <label for="cbx-actall-${index}"> | </label>
                                                   </div>` + actionTemp;
                                    }
                                    else {
                                        actionHtml += actionTemp
                                    }
                                }
                            }
                            //
                            rowData += `
                            <tr data-rowid='w-${id}'> 
                                <td class='text-left'>
                                    <input id="cbx${id}" data-val="${id}" type="checkbox" class="filled-in inp-controler" ${staController} />
                                    <label for="cbx${id}">${rowNum}. ${_title}</label>
                                </td>
                                <td class='text-left'>${actionHtml}</td>     
                            </tr>`;
                        });
                        $('#TblFunction > tbody').html(rowData);

                        CheckAllForFunction();


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
    GetRoleCategory: function (_id, _envent) {
        // model 
        var _option_default = ``;
        var model = {};
        AjaxFrom.POST({
            url: '/Management/Role/Action/GetDataOption',
            data: model,
            success: function (result) {
                $('ul#Role').html(_option_default);
                if (result !== null) {
                    if (result.status === 200) {
                        var rowData = '';
                        if (result.data.length > 0) {
                            $.each(result.data, function (index, item) {
                                if (item.ParentID == null || item.ParentID == "") {
                                    index = index + 1;
                                    var id = item.ID;
                                    if (id.length > 0)
                                        id = id.trim();
                                    var _title = SubStringText.SubTitle(item.Title);
                                    var subMenu = item.SubOption;
                                    var isChecked = "";

                                    var _level = 0;
                                    rowData += `<li>
                                        <input id="cbxItem${id}" type="checkbox" class="filled-in" data-id='${id}' ${isChecked} value='${id}' />
                                        <label for="cbxItem${id}">${_title}</label>`;
                                    if (subMenu !== undefined && subMenu !== null && subMenu.length > 0) {
                                        rowData += _PermissionController.GetSubRoleCategory(index, subMenu, _level, _id);
                                    }
                                    rowData += '</li>';
                                }
                            });
                        }
                        $('ul#Role').html(_option_default + rowData);
                        if (_envent) {
                            $('ul#Role > li:first > label').click();
                        }
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
    GetSubRoleCategory: function (_index, lstModel, _level, _id) {
        var rowData = '';
        //
        if (lstModel.length > 0) {
            _level += 1;
            rowData += `<ul>`;
            $.each(lstModel, function (index, item) {
                index = index + 1;
                var id = item.ID;
                if (id.length > 0)
                    id = id.trim();

                var _title = SubStringText.SubTitle(item.Title);
                var subMenu = item.SubOption;
                var isChecked = "";
                if (id !== null && id === _id)
                    isChecked = "checked";
                //
                var pading = _level * 38;

                rowData += `<li>
                               <input id="cbxItem${id}" type="checkbox" class="filled-in" data-id='${id}' ${isChecked} value='${id}' />
                               <label for="cbxItem${id}">${_title}</label>`;

                if (subMenu !== undefined && subMenu !== null && subMenu.length > 0) {
                    rowData += _PermissionController.GetSubRoleCategory(_index, subMenu, _level, _id);
                }
                rowData += '</li>';
            });
            rowData += `</ul>`;
        }
        return rowData;
    }
};
_PermissionController.init();
//
$(document).on('click', '#btnUpdate', function () {
    var dataSetting = [];
    var controllers = $('#TblFunction tbody').find('input[type="checkbox"].inp-controler:checkbox:checked');
    if (controllers != undefined && controllers.length > 0) {
        $(controllers).each(function (index, item) {
            var controlerId = $(item).data('val');
            // action of controller
            var arrAction = [];
            var rowData = $(item).closest('tr');
            var actions = $(rowData).find('input[type="checkbox"].inp-action:checkbox:checked');
            if (actions != undefined && actions.length > 0) {
                $(actions).each(function (actIndex, actItem) {
                    var actionId = $(actItem).data('val');
                    arrAction.push(actionId);
                });
            }
            //
            var itemData = {
                ID: controlerId,
                Action: arrAction
            }
            dataSetting.push(itemData);
        });
        //
        $('#lblMessage').html("");
        var roleActive = $('#Role input[type="checkbox"]:checkbox:checked');
        if (roleActive.length < 1) {
            $('#lblMessage').html("Vui lòng chọn nhóm người dùng");
            Notifization.Error("Vui lòng chọn nhóm người dùng");
            return;
        }
        if (roleActive.length > 1) {
            $('#lblMessage').html("Nhóm người dùng không hợp lệ");
            Notifization.Error("Nhóm người dùng không hợp lệ");
            return;
        }
        var roleId = $(roleActive).val();
        if (roleId == undefined || roleId == '') {
            $('#lblMessage').html("Nhóm người dùng không hợp lệ");
            Notifization.Error("Nhóm người dùng không hợp lệ");
            return;
        }
        //
        var model = {
            RoleID: roleId,
            Controllers: dataSetting
        }
        //
        AjaxFrom.POST({
            url: URLC + '/Setting',
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
            error: function (result) {
                console.log('::' + MessageText.NotService);
            }
        });

    }
});
//
function CheckAllForFunction() {
    var notcheck = $('table#TblFunction').find('tbody tr input.inp-controler:checkbox:not(:checked)');
    if (notcheck.length == 0)
        $('table#TblFunction thead input[type="checkbox"]').prop('checked', true);
    else
        $('table#TblFunction thead input[type="checkbox"]').prop('checked', false);
}








