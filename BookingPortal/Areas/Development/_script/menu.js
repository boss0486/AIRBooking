var pageIndex = 1;
var URLC = "/Development/Menu-Item/Action";
var URLA = "/Development/MenuItem";
var menuController = {
    init: function () {
        menuController.registerEvent();
    },
    registerEvent: function () {
        $('#btnCreate').off('click').on('click', function () {
            var flg = true;
            var txtTitle = $('#txtTitle').val();
            var txtSummary = $('#txtSummary').val();

            if (txtTitle === '') {
                $('#lblTitle').html('Không được để trống tiêu đề');
                flg = false;
            }
            else if (txtTitle.length < 1 || txtTitle.length > 80) {
                $('#lblTitle').html('Tiêu đề giới hạn từ 1-> 80 ký tự');
                flg = false;
            }
            else if (!FormatKeyword.test(txtTitle)) {
                $('#lblTitle').html('Tiêu đề không hợp lệ');
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
            //
            //var ddlOrder = $('#ddlOrder').val();
            //if (parseInt(ddlOrder) === 0) {
            //    $('#lblOrder').html('Vui lòng chọn vị trí');
            //    flg = false;
            //}
            //else {
            //    $('#lblOrder').html('');
            //}
            if (flg)
                menuController.Create();
            else
                Notifization.Error(MessageText.Datamissing);
        });
        $('#btnSearch').off('click').on('click', function () {
            menuController.DataList(1);
        });
        $('#btnUpdate').off('click').on('click', function () {
            var flg = true;
            var txtTitle = $('#txtTitle').val();
            var txtSummary = $('#txtSummary').val();

            if (txtTitle === '') {
                $('#lblTitle').html('Không được để trống tiêu đề');
                flg = false;
            }
            else if (txtTitle.length < 1 || txtTitle.length > 80) {
                $('#lblTitle').html('Tiêu đề giới hạn từ 1-> 80 ký tự');
                flg = false;
            }
            else if (!FormatKeyword.test(txtTitle)) {
                $('#lblTitle').html('Tiêu đề không hợp lệ');
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
            //
            //var ddlOrder = $('#ddlOrder').val();
            //if (parseInt(ddlOrder) === 0) {
            //    $('#lblOrder').html('Vui lòng chọn vị trí');
            //    flg = false;
            //}
            //else {
            //    $('#lblOrder').html('');
            //}

            if (flg)
                menuController.Update();
            else
                Notifization.Error(MessageText.Datamissing);
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
            url: '/post/detail',
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
    DataList: function (page) {
        var _query = $('#txtQuery').val();
        var _status = $('#ddlStatus').val();
        var _areaId = $('#ddlArea').val();
        // model
        var model = {
            Query: _query,
            Page: page,
            Status: _status,
            AreaID: _areaId
        };
        AjaxFrom.POST({
            url: URLC + '/DataList',
            data: model,
            success: function (result) {
                $('tbody#TblData').html('');
                if (result !== null) {
                    if (result.status === 200) {
                        $('tbody#TblData').html('');
                        $('#Pagination').html('');
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
                            if (role !== undefined && role !== null) {
                                var action = `<div class='ddl-action'><span><i class='fa fa-caret-down'></i></span>
                                              <div class='ddl-action-content'>`;
                                if (role.Update)
                                    action += `<a href='${URLA}/update/${id}' target="_blank"><i class='fas fa-pen-square'></i>&nbsp;Edit</a>`;
                                if (role.Delete)
                                    action += `<a onclick="menuController.ConfirmDelete('${id}')" target="_blank"><i class='fas fa-trash'></i>&nbsp;Delete</a>`;
                                if (role.Details)
                                    action += `<a href='${URLA}/detail/${id}' target="_blank"><i class='fas fa-info-circle'></i>&nbsp;Detail</a>`;
                                action += `</div>
                                           </div>`;
                            }
                            // icon sort
                            var _level = 0;
                            var _orderId = item.OrderID;
                            var _actionSort = `<i data-sortup='btn-sort-up' data-id='${id}' data-order'${_orderId}' class='fas fa-arrow-circle-up icon-mnsort'></i> <i data-sortdown ='btn-sort-down' data-id='${id}' data-order'${_orderId}' class='fas fa-arrow-circle-down icon-mnsort'></i>`;
                            //
                            var _title = SubStringText.SubTitle(item.Title);
                            var _areaKey = item.AreaID;
                            //var _summary = SubStringText.SubSummary(item.Summary);
                            //var _createdDate = LibDateTime.ConvertUnixTimestampToDate(item.CreatedDate, '-', 'en');
                            var rowNum = parseInt(index) + (parseInt(currentPage) - 1) * parseInt(pageSize);
                            rowData += `
                            <tr>
                                 <td class='text-right'>${rowNum}&nbsp;</td>
                                 <td class='text-left' style='background:#F1F5F7'>- ${_title}</td>                                  
                                 <td class='text-left'>${_areaKey}</td>                                  
                                 <td class='text-left'>Cấp ${_level + 1}</td>                                  
                                 <td class='text-right'>${_orderId}</td>                                  
                                 <td class='text-center'>${_actionSort}</td>                                                                                                                                
                                 <td class='text-center'>${HelperModel.StatusIcon(item.Enabled)}</td>
                                 <td class='text-center'>${action}</td>
                            </tr>`;
                            var subMenu = item.SubMenuLevelModel;
                            if (subMenu !== undefined && subMenu !== null && subMenu.length > 0) {
                                rowData += menuController.GetSubMenuList(index, subMenu, _level, role);
                            }
                        });
                        $('tbody#TblData').html(rowData);
                        if (parseInt(totalPage) > 1) {
                            Paging.Pagination("#Pagination", totalPage, currentPage, appMenuController.DataList);
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
    GetSubMenuList: function (_index, lstModel, _level, _role) {
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
                if (role !== undefined && role !== null) {
                    action = `<div class='ddl-action'><span><i class='fa fa-caret-down'></i></span><div class='ddl-action-content'>`;
                    if (role.Update)
                        action += `<a href='${URLA}/update/${id}' target="_blank"><i class='fas fa-pen-square'></i>&nbsp;Edit</a>`;
                    if (role.Delete)
                        action += `<a onclick="menuController.ConfirmDelete('${id}')" target="_blank"><i class='fas fa-trash'></i>&nbsp;Delete</a>`;
                    if (role.Details)
                        action += `<a href='${URLA}/detail/${id}' target="_blank"><i class='fas fa-info-circle'></i>&nbsp;Detail</a>`;
                    action += `</div></div>`;
                }
                // icon sort
                // icon sort
                var _orderId = item.OrderID;
                var _actionSort = `<i data-sortup='btn-sort-up' data-id='${id}' data-order'${_orderId}' class='fas fa-arrow-circle-up icon-mnsort'></i> <i data-sortdown ='btn-sort-down' data-id='${id}' data-order'${_orderId}' class='fas fa-arrow-circle-down icon-mnsort'></i>`;
                //
                var _title = SubStringText.SubTitle(item.Title);
                var _areaKey = item.AreaID;
                //var _summary = SubStringText.SubSummary(item.Summary);
                //var _cratedDate = LibDateTime.ConvertUnixTimestampToDate(item.CreatedDate, '-', 'en');
                var rowNum = ''; //parseInt(index) + (parseInt(currentPage) - 1) * parseInt(pageSize);
                var pading = _level * 20;
                rowData += `
                            <tr>
                                 <td class="text-right">${rowNum}&nbsp;</td>
                                 <td class='text-left'><div style='padding-left:${pading}px'>- ${_title}</div></td> 
                                 <td class='text-left'>${_areaKey}</td> 
                                 <td class='text-left'>Cấp ${_level + 1}</td>            
                                 <td class='text-right'>${_orderId}</td>
                                 <td class='text-center'>${_actionSort}</td>                                                                                                                             
                                 <td class='text-center'>${HelperModel.StatusIcon(item.Enabled)}</td>
                                 <td class='tbcol-action'>${action}</td>
                            </tr>`;

                var subMenu = item.SubMenuLevelModel;
                if (subMenu !== undefined && subMenu !== null && subMenu.length > 0) {
                    rowData += menuController.GetSubMenuList(_index, subMenu, _level, _role);
                }
            });
        }
        return rowData;
    },
    Create: function () {
        var title = $('#txtTitle').val();
        var summary = $('#txtSummary').val();
        var ddlController = $('#ddlController').val();
        var ddlAction = $('#ddlAction').val();
        var areaId = $('#ddlArea').val();
        var enabled = 0;
        if ($('input[name="cbxActive"]').is(":checked"))
            enabled = 1;
        //
        var isPermission = 0;
        if ($('input[name="cbxPermission"]').is(":checked"))
            isPermission = 1;
        //cbxPermission
        var parentId = 0;
        //
        var checked = $(".vatical-menu input[type='checkbox']:checked");
        if (checked.length > 0)
            parentId = $(checked).data('id');
        //
        var iconFont = $('#txtIconFont').val();
        var ddlOrder = parseInt($('#ddlOrder').val());
        //
        var model = {
            RouteArea: areaId,
            ParentID: parentId,
            Title: title,
            Summary: summary,
            IconFont: iconFont,
            OrderID: ddlOrder,
            IsPermission: isPermission,
            MvcController: ddlController,
            MvcAction: ddlAction,
            Enabled: enabled
        };
        AjaxFrom.POST({
            url: URLC + '/create',
            data: model,
            success: function (response) {
                if (response !== null) {
                    if (response.status === 200) {
                        Notifization.Success(response.message);
                        menuController.GetMenuCategory(areaId);
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
        var id = $('#txtID').val();
        var title = $('#txtTitle').val();
        var summary = $('#txtSummary').val();
        var ddlController = $('#ddlController').val();
        var ddlAction = $('#ddlAction').val();
        var areaId = $('#ddlArea').data("id");
        var enabled = 0;
        if ($('input[name="cbxActive"]').is(":checked"))
            enabled = 1;
        //
        var isPermission = 0;
        if ($('input[name="cbxPermission"]').is(":checked"))
            isPermission = 1;
        //cbxPermission
        var parentId = 0;
        //
        var checked = $(".vatical-menu input[type='checkbox']:checked");
        if (checked.length > 0)
            parentId = $(checked).data('id');
        //
        var iconFont = $('#txtIconFont').val();
        var ddlOrder = parseInt($('#ddlOrder').val());
        //
        var model = {
            ID: id,
            ParentID: parentId,
            Title: title,
            Summary: summary,
            IconFont: iconFont,
            OrderID: ddlOrder,
            IsPermission: isPermission,
            MvcController: ddlController,
            MvcAction: ddlAction,
            Enabled: enabled
        };
        AjaxFrom.POST({
            url: URLC + '/update',
            data: model,
            success: function (response) {
                if (response !== null) {
                    if (response.status === 200) {
                        Notifization.Success(response.message);
                        menuController.GetMenuCategory(areaId, parentId);
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
    InLineList: function (page) {
        var model = {
            Query: $('#txtQuery').val(),
            Page: page
        };
        AjaxFrom.POST({
            url: URLC + '/datalist',
            data: model,
            success: function (result) {
                if (result !== null) {
                    if (result.status === 200) {
                        $('tbody#TblData').html('');
                        $('#Pagination').html('');
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
                            var rowNum = parseInt(index) + (parseInt(currentPage) - 1) * parseInt(pageSize);
                            rowData += `
                            <tr>
                                <td class="tbcol-title"><input id='Title'          type='text' data-column='Title'         data-id='${id}' value='${item.Title}'        data-oldval='${item.Title}'           max-length='100' style='width:100%' /></td>
                                <td class="tbcol-none" ><input id='PathAction'     type='text' data-column='PathAction'    data-id='${id}' value='${item.PathAction}'   data-oldval='${item.PathAction}'      max-length='100' style='width:100%' /></td>
                                <td class="tbcol-none" ><input id='MvcController'  type='text' data-column='Controller' data-id='${id}' value='${item.MvcController}'data-oldval='${item.MvcController}'   max-length='100' style='width:80px' /></td>
                                <td class="tbcol-none" ><input id='MvcAction'      type='text' data-column='Action'     data-id='${id}' value='${item.MvcAction}'    data-oldval='${item.MvcAction}'       max-length='100' style='width:80px' /></td>
                                <td class="tbcol-none" ><input id='IsPermission'   type='text' data-column='IsPermission'  data-id='${id}' value='${item.IsPermission}' data-oldval='${item.IsPermission}'    max-length='100' style='width:50px' /></td>
                                <td class="tbcol-none" ><input id='Enabled'        type='text' data-column='Enabled'       data-id='${id}' value='${item.Enabled}'      data-oldval='${item.Enabled}'         max-length='100' style='width:50px' /></td>
                                <td class="tbcol-none" ><input id='OrderID'        type='text' data-column='OrderID'       data-id='${id}' value='${item.OrderID}'      data-oldval='${item.OrderID}'         max-length='100' style='width:50px' /></td>
                            </tr>`;
                        });
                        $('tbody#TblData').html(rowData);
                        if (parseInt(totalPage) > 1) {
                            Paging.Pagination("#Pagination", totalPage, currentPage, menuController.InLineList);
                        }
                        return;
                    }
                    else {
                        //Notifization.Error(result.message);
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
    Delete: function (id) {
        var model = {
            Id: id
        };
        AjaxFrom.POST({
            url: URLC + '/delete',
            data: model,
            success: function (response) {
                if (response !== null) {
                    if (response.status === 200) {
                        Notifization.Success(response.message);
                        menuController.DataList(pageIndex);
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

        Confirm.Delete(id, menuController.Delete, null, null);

        //Confirm.ConfirmDelete({
        //    Title: 'Xác nhận!',
        //    Message: "This is a confirm with custom button text and color! Do you like it?",
        //    Modalc: 'modal-col-pink',
        //    YesEvent: function () { menuController.Delete(id); },
        //    NoEvent: null,
        //    Callback: null
        //});
    },
    Active: function (id) {
        var fData = {
            Id: id
        };
        $.ajax({
            url: '/PostCategory/Active',
            data: {
                strData: JSON.stringify(fData)
            },
            type: 'POST',
            dataType: 'json',
            success: function (response) {
                if (response !== null) {
                    if (response.status === 200) {
                        Notifization.Success(response.message);
                        menuController.DataList(pageIndex);
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
            url: '/PostCategory/UnActive',
            data: {
                strData: JSON.stringify(fData)
            },
            type: 'POST',
            dataType: 'json',
            success: function (response) {
                if (response !== null) {
                    if (response.status === 200) {
                        Notifization.Success(response.message);
                        menuController.DataList(pageIndex);
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
    GetMenuCategory: function (_areaId, _id) {
        // model 
        var _option_default = `<li>
                                    <input id="cbxItem0" type="checkbox" class="filled-in" data-id='0' />
                                    <label for="cbxItem0">Tạo mới</label>
                               </li>`;
        var model = { RouteArea: _areaId };
        AjaxFrom.POST({
            url: URLC + '/MenuItem-ByLevel',
            data: model,
            success: function (result) {
                $('ul#MenuItemCategory').html(_option_default);
                if (result !== null) {
                    if (result.status === 200) {
                        var rowData = '';
                        $.each(result.data, function (index, item) {
                            index = index + 1;
                            var id = item.ID;
                            if (id.length > 0)
                                id = id.trim();
                            var _title = SubStringText.SubTitle(item.Title);
                            var subMenu = item.SubMenuLevelModel;
                            var iAngle = '';
                            if (subMenu !== undefined && subMenu !== null && subMenu.length > 0)
                                iAngle = "<i class='fa fa-angle-left iplus'></i>";
                            var isChecked = "";
                            if (id !== null && id === _id)
                                isChecked = "checked";
                            var _level = 0;
                            rowData += `<li>
                                        <input id="cbxItem${id}" type="checkbox" class="filled-in" data-id='${id}' ${isChecked} />
                                        <label for="cbxItem${id}">- ${_title}</label>`;
                            if (subMenu !== undefined && subMenu !== null && subMenu.length > 0) {
                                rowData += menuController.GetSubMenuCategoryList(index, subMenu, _level, _id);
                            }
                            rowData += '</li>';


                        });
                        $('ul#MenuItemCategory').html(_option_default + rowData);
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
    GetSubMenuCategoryList: function (_index, lstModel, _level, _id) {
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
                var subMenu = item.SubMenuLevelModel;
                var iAngle = '';
                if (subMenu !== undefined && subMenu !== null && subMenu.length > 0)
                    iAngle = "<i class='fa fa-angle-left iplus'></i>";
                var isChecked = "";
                if (id !== null && id === _id)
                    isChecked = "checked";
                //
                var pading = _level * 38;

                rowData += `<li>
                               <input id="cbxItem${id}" type="checkbox" class="filled-in" data-id='${id}' ${isChecked} />
                               <label for="cbxItem${id}">${_title}</label>`;

                if (subMenu !== undefined && subMenu !== null && subMenu.length > 0) {
                    rowData += menuController.GetSubMenuCategoryList(_index, subMenu, _level, _id);
                }
                rowData += '</li>';
            });
            rowData += `</ul>`;
        }
        return rowData;
    }
};
menuController.init();
$(document).on('keyup', '#txtTitle', function () {
    var txtTitle = $(this).val();
    if (txtTitle === '') {
        $('#lblTitle').html('Không được để trống tiêu đề');
    }
    else if (txtTitle.length < 1 || txtTitle.length > 80) {
        $('#lblTitle').html('Tiêu đề giới hạn từ 1-> 80 ký tự');
    }
    else if (!FormatKeyword.test(txtTitle)) {
        $('#lblTitle').html('Tiêu đề không hợp lệ');
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
//
$(document).on('click', '#cbxPermission', function () {
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
$(document).on("click", '.mn-ckeck', function () {
    if ($('i').hasClass('fa-check-square')) {
        // remove
        $('.menu-category i.mn-ckeck').removeClass('fa-check-square');
        $('.menu-category i.mn-ckeck').addClass('fa-square');
        $('.menu-category i.actived').removeClass('actived');
    }
    if ($('i').hasClass('fa-square')) {
        $(this).removeClass('fa-square');
        $(this).addClass('fa-check-square');
        $(this).addClass('actived');
    }
});
$(document).on("click", "#btnFile", function () {
    $('#ImageFile').click();
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
                    menuController.DataList(pageIndex);
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
                    menuController.DataList(pageIndex);
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
//
$(document).on('change', '#ddlController', function () {
    var id = $(this).val();
    GetActionByCategoryID("", id, false);
});

$(document).on('change', '#ddlArea', function (index, item) {
    var areaId = $(this).val();
    var page = $(this).data("action");
    if (page == undefined || page == null || page == "") {
        console.log(">>:None page");
        return;
    }

    switch (page.toLowerCase()) {
        case "create":
            menuController.GetMenuCategory(areaId);
            GetControllerListByAreaId("", areaId, true);
            break;
        case "datalist":
            menuController.DataList(1)
            break;
        default:
    }
})


function GetControllerListByAreaId(_id, _cateId, isChangeEvent) {
    var model = {
        cateId: _cateId
    };
    AjaxFrom.POST({
        url: '/Development/Menu-Controller/Action/DropdownList',
        data: model,
        async: true,
        success: function (result) {
            if (result !== null) {
                if (result.status === 200) {
                    var option = `<option value="">-Lựa chọn-</option>`;
                    $.each(result.data, function (index, item) {
                        var id = item.ID;
                        var attrSelect = '';
                        if (_id !== undefined && _id != "" && _id === id) {
                            attrSelect = "selected";
                        }
                        option += `<option value='${id}' ${attrSelect}>${item.Title}</option>`;
                    });
                    $('#ddlController').html(option);
                    $('#ddlController').selectpicker('refresh');
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
function GetActionByCategoryID(_id, categoryId, isChangeEvent) {
    var option = `<option value="">-Lựa chọn-</option>`;
    $('#ddlAction').html(option);
    $('#ddlAction').selectpicker('refresh');
    var model = {
        ID: _id,
        CategoryID: categoryId
    };
    AjaxFrom.POST({
        url: '/Development/Menu-Action/Action/Get-ActionBy-CategoryID',
        data: model,
        async: true,
        success: function (result) {
            if (result !== null) {
                if (result.status === 200) {
                    $.each(result.data, function (index, item) {
                        var id = item.ID;
                        var attrSelect = '';
                        if (_id !== undefined && _id != "" && _id === id) {
                            attrSelect = "selected";
                        }
                        option += `<option value='${id}' ${attrSelect}>${item.Title}</option>`;
                    });
                    $('#ddlAction').html(option);
                    $('#ddlAction').selectpicker('refresh');
                    setTimeout(function () {
                        if (isChangeEvent !== undefined && isChangeEvent == true) {
                            $('#ddlAction').change();
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
//
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
                    var option = `<option value="">-Phân vùng-</option>`;
                    var attrSelect = '';
                    $.each(result.data, function (index, item) {
                        var id = item.ID;
                        if (_id !== undefined && _id != "" && _id === item.ID) {
                            attrSelect = "selected";
                        } else if (index == 0 && isChangeEvent == true) {
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
