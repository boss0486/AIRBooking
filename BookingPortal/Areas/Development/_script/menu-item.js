//var pageIndex = 1;
//var URLC = "/Development/Menu-Item/Action";
//var URLA = "/Development/MenuItem"; 
//$(document).on('click', '#btnSyncControl', function () {
//    var model = {
//        Query: '',
//        Page: 0
//    };
//    AjaxFrom.POST({
//        url: URLC + '/Menu-Sync',
//        data: model,
//        async: true,
//        success: function (result) {
//            if (result !== null) {
//                if (result.status === 200) {
//                    Notifization.Success(result.message);
//                    // get menu
//                    GetMenuArea("", true);
//                    return;
//                }
//                else {
//                    Notifization.Error(result.message);
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
//});
//function GetMenuArea(_id, isChangeEvent) {
//    var model = {
//    };
//    AjaxFrom.POST({
//        url: '/Development/Area/Action/DropdownList',
//        data: model,
//        async: true,
//        success: function (result) {
//            if (result !== null) {
//                if (result.status === 200) {
//                    var option = `<option value="">-Lựa chọn-</option>`;
//                    $.each(result.data, function (index, item) {
//                        var id = item.ID;
//                        var attrSelect = '';
//                        if (_id !== undefined && _id != "" && _id === item.ID) {
//                            attrSelect = "selected";
//                        } else if (index == 0) {
//                            attrSelect = "selected";
//                        }
//                        option += `<option value='${id}' ${attrSelect}>${item.Title}</option>`;
//                    });
//                    $('#ddlAreaID').html(option);
//                    setTimeout(function () {
//                        $('#ddlAreaID').selectpicker('refresh');
//                        if (isChangeEvent !== undefined && isChangeEvent == true) {
//                            $('#ddlAreaID').change();
//                        }
//                    }, 1000);
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
 