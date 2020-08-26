var pageIndex = 1;
var URLC = "/Template/Menu-Item/Action";
var URLA = "/Template/MenuItem";
var layoutController = {
    init: function () {
        layoutController.registerEvent();
    },
    registerEvent: function () {
    },
    LeftMenu: function (_groupId, _activeId, currPath) {
        // model
        var model = {
            
        };
        AjaxFrom.POST({
            url: URLC + '/MenuItem-Manage',
            data: model,
            success: function (result) {
                $('ul#LeftMenu').html('');
                if (result !== null) {
                    if (result.status === 200) {
                        var rowData = '';
                        $.each(result.data, function (index, item) {
                            index = index + 1;
                            var id = item.ID;
                            if (id.length > 0)
                                id = id.trim();
                            var _title = SubStringText.SubTitle(item.Title);
                            var _cls = '';
                            var pathAction = item.PathAction;
                            if (pathAction == null || pathAction == undefined || pathAction == "")
                                pathAction = "#";
                            //
                            //var activeLi = '';
                            var mnOpen = '';

                            var currentPath = '';
                            //
                            //if (_groupId !== undefined && _groupId !== "" && pathAction.toLowerCase().includes(_groupId.toLowerCase())) {
                            //    mnOpen = 'toggled';
                            //    activeLi = 'active';
                            //    currentPath += _groupId.toLowerCase();
                            //}; 
                            // active 
                            //var active = '';
                            //if (_activeId !== undefined && _activeId !== "" && pathAction.toLowerCase().includes(currentPath + "/" + _activeId.toLowerCase())) {
                            //    active = 'active';
                            //};
                            var subMenu = item.SubMenuLevelModel;
                            if (subMenu !== undefined && subMenu !== null && subMenu.length > 0) {
                                _cls = 'menu-toggle';
                                pathAction = "javascript:void(0);";
                            }
                            // 
                            rowData += `<li class=''><a href='${pathAction}' class='${_cls} ${mnOpen}'><span><i class='${item.IconFont}' aria-hidden='true'></i>&nbsp; ${_title}</span></a>`;
                            if (subMenu !== undefined && subMenu !== null && subMenu.length > 0) {
                                rowData += layoutController.LeftSubMenu(index, subMenu, 0, _groupId, _activeId);
                            }
                            rowData += '</li>';
                        });
                        $('ul#LeftMenu').html(rowData);
                        $.AdminBSB.leftSideBar.activate();
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
    LeftSubMenu: function (_index, lstModel, _level, _groupId, _activeId) {
        var rowData = '';
        //
        if (lstModel.length > 0) {
            _level += 1;
            rowData += `<ul class='ml-menu'>`;
            $.each(lstModel, function (index, item) {
                index = index + 1;
                var id = item.ID;
                if (id.length > 0)
                    id = id.trim();
                var action = '';
                _index = _level * 15;
                var _title = SubStringText.SubTitle(item.Title);
                var _cls = '';
                var pathAction = item.PathAction;
                if (pathAction == null || pathAction == undefined || pathAction == "")
                    pathAction = "#";
                //
                //var activeLi = '';
                //var mnOpen = '';
                //if (_groupId !== undefined && _groupId !== "" && pathAction.toLowerCase().includes(_groupId.toLowerCase())) {
                //    mnOpen = 'toggled';
                //    activeLi = '';
                //};
                //// active 
                //var active = '';
                //if (_activeId !== undefined && _activeId !== "" && pathAction.toLowerCase().includes(_activeId.toLowerCase())) {
                //    active = 'active';
                //};
                //
                var subMenu = item.SubMenuLevelModel;
                if (subMenu !== undefined && subMenu !== null && subMenu.length > 0) {
                    _cls = 'menu-toggle';
                    pathAction = "javascript:void(0);";
                }
                rowData += `<li class=''><a href='${pathAction}' class='${_cls}'><span><i class='${item.IconFont}' aria-hidden='true'></i>&nbsp; ${_title}</span></a>`;
                if (subMenu !== undefined && subMenu !== null && subMenu.length > 0) {
                    rowData += layoutController.LeftSubMenu(_index, subMenu, _level);
                }
                rowData += `</li>`;
            });
            rowData += `</ul>`;
        }
        return rowData;
    }
};
layoutController.init();
