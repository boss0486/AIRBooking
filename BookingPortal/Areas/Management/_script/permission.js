var pageIndex = 1;
var URLC = "/Management/permission/Action";
var URLA = "/Management/permission";
var _PermissionController = {
    init: function () {
        _PermissionController.registerEvent();
    },
    registerEvent: function () {
        $(document).ready(function () {
            $("#Role li:first-child a:first-child").click();
        });
    },
    PermissionList: function () {
        var model = {
            RoleID: 0
        };
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
                            var role = result.role;
                            if (role !== undefined && role !== null) {
                                var action = `<div class='ddl-action'><span><i class='fa fa-caret-down'></i></span>
                                              <div class='ddl-action-content'>`;
                                if (role.Update)
                                    action += `<a href='${URLC}/Update/${id}'><i class='fas fa-pen-square'></i>&nbsp;Edit</a>`;
                                if (role.Delete)
                                    action += `<a onclick="_PermissionController.ConfirmDelete('${id}')"><i class='fas fa-trash'></i>&nbsp;Delete</a>`;
                                if (role.Detail)
                                    action += `<a href='${URLC}/Details/${id}'><i class='fas fa-info-circle'></i>&nbsp;Detail</a>`;
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
    RoleOptionList: function (id, page) {
        var model = {
            Query: '',
            Page: 1,
            Status: -1
        };
        AjaxFrom.POST({
            url: '/Management/Role/Action/DropDownList',
            data: model,
            success: function (result) {
                $('ul#Role').html('');
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
                            var id = item.ID;
                            rowData += `                
                              <a class="list-group-item">                                     
                                    <input id="${id}" data-CategoryID='${id}' type="checkbox" class="filled-in action-item-input  " value="${id}" />
                                    <label style="margin:0px;" for="${id}">${strIndex}. ${item.Title} <span class="badge badge-primary badge-pill ${active}">Cấp: ${strLevel}</span></label>
                              </a>`;
                        });
                        $('ul#Role').html(rowData);
                        $('ul#Role a.list-group-item:first input[type="checkbox"]').prop('checked', true);
                        setTimeout(function () {
                            $('ul#Role a.list-group-item:first input[type="checkbox"]').change();
                        }, 500);
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
                            if (totalCtrl > 0) {
                                staController = "checked";
                                  actionState = '';
                            }
                            if (actionData != null) {
                                $.each(actionData, function (actIndex, actItem) {
                                    var actId = actItem.ID;
                                    var actTitle = actItem.Title;
                                    var staAction = "";
                                    var totalAction = actItem.Total;
                                    if (totalAction > 0) {
                                        staAction = "checked";
                                        cntActionActive++;
                                    }
                                    actionTemp += `<div style='width:120px;display: inline-block;'> 
                                                        <input id="cbx${actId}" data-val="${actId}" type="checkbox" class="filled-in inp-action" ${staAction} ${actionState} />
                                                        <label for="cbx${actId}">${actTitle} ${totalAction}</label>
                                                   </div>`;
                                });
                                //
                                if (actionTemp !== '') {
                                    var checkAllAction = '';
                                    if (parseInt(cntActionActive) == parseInt(actionData.length)) {
                                        checkAllAction = 'checked';
                                    }
                                    actionHtml += `<div style='width:80px;display: inline-block;'> 
                                                        <input id="cbx-actall-${index}" data-val="" type="checkbox" class="filled-in all-action" ${checkAllAction}  ${actionState} />
                                                        <label for="cbx-actall-${index}">Tất cả</label>
                                                   </div>` + actionTemp; 
                                }
                            }
                            //
                            rowData += `
                            <tr data-rowid='w-${id}'> 
                                <td class='text-left'>
                                    <input id="cbx${id}" data-val="${id}" type="checkbox" class="filled-in inp-controler" ${staController} />
                                    <label for="cbx${id}">${rowNum}. ${_title} ${actionData.length}</label>
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


    // var controllers = $('#TblFunction input.inp-controler').is(":checked");
    //  console.log('::' + controllers.length);




});
//
function CheckAllForFunction() {
    var notcheck = $('table#TblFunction').find('tbody tr input.inp-controler:checkbox:not(:checked)');
    if (notcheck.length == 0)
        $('table#TblFunction thead input[type="checkbox"]').prop('checked', true);
    else
        $('table#TblFunction thead input[type="checkbox"]').prop('checked', false);
}








