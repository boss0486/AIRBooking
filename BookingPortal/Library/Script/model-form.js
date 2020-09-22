var ajaxStatus = 0;


$(document).ajaxStart(function () {
    Loading.ShowLoading();
    ajaxStatus++;

}).ajaxComplete(function () {
    ajaxStatus--;
     
});

class AjaxFrom {
    static POST(_form) {
        _form.method = "POST";
        _form.dataType = 'json';
        _form.async = false;
        $.ajax(_form).done(function () {
            setTimeout(function () {
                Loading.HideLoading();
            }, 1500);
            //if (ajaxStatus == 0) {
            //    setTimeout(function () {
            //        console.log("o11k");
            //        Loading.HideLoading();
            //    }, 1500);
            //}
        });
    }
    static POSTFILE(_form) {
        //Loading.ShowLoading();
        //ajaxStatus++;
        _form.method = "POST";
        _form.dataType = 'json';
        _form.contentType = false;
        _form.processData = false;
        $.ajax(_form).done(function () {
            ajaxStatus--;
            if (ajaxStatus === 0) {
                setTimeout(function () {
                    console.log("o11k");
                    Loading.HideLoading();
                }, 1500);
            }
        });
    }
}

//$(document).bind("ajaxSend", function () {
//    $("#loading").show();
//}).bind("ajaxComplete", function () {
//    $("#loading").hide();
//});


$(document).ready(function () {

    //var startDate = new Date('01/01/2019');
    //var _startDate = new Date();
    //var _endDate =  '';
    //$('input[date-datepicker="1"]').datepicker({
    //    format: 'dd-mm-yyyy',
    //    todayHighlight: true,
    //    startDate: '01/01/2019',
    //    endDate: _startDate,
    //    autoclose: true
    //}).on('changeDate', function (selected) {
    //        startDate = new Date(selected.date.valueOf());
    //        startDate.setDate(startDate.getDate(new Date(selected.date.valueOf())));
    //    $('input[date-datepicker="2"]').datepicker('setStartDate', startDate);
    //    });
    //$('input[date-datepicker="2"]').datepicker({
    //    format: 'dd-mm-yyyy',
    //    todayHighlight: true,
    //    startDate: startDate,
    //    endDate: _endDate,
    //    autoclose: true
    //}).on('changeDate', function (selected) {
    //        _startDate = new Date(selected.date.valueOf());
    //        _startDate.setDate(_startDate.getDate(new Date(selected.date.valueOf())));
    //    $('input[date-datepicker="1"]').datepicker('setEndDate', _startDate);
    //    });
});

class HelperModel {
    static Status(_status) {
        var result = '';
        switch (_status) {
            case 0:
                result = "Disable";
                break;
            case 1:
                result = "Enabled";
                break;
            default:
                result = "";
                break;
        }
        return result;
    }
    static StatusIcon(_status) {
        var result = '';
        switch (_status) {
            case 0:
                result = "<i class='fa fa-toggle-off'></i>";
                break;
            case 1:
                result = "<i class='fa fa-toggle-on'></i>";
                break;
            default:
                result = "";
                break;
        }
        return result;
    }
    static RolePermission(role, _controlInit, id) {
        if (role !== undefined && role !== null) {
            var action = `<div class='ddl-action'><span><i class='fa fa-caret-down'></i></span><div class='ddl-action-content'>`;
            $.each(role, function (index, item) {

                //if (role.Block) {
                //    if (item.IsBlock)
                //        action += `<a onclick="AccountController.Unlock('${id}')"><i class='far fa-dot-circle'></i>&nbsp;Unlock</a>`;
                //    else
                //        action += `<a onclick="AccountController.Block('${id}')"><i class='fas fa-ban'></i>&nbsp;Block</a>`;
                //}
                //if (role.Active) {
                //    if (item.Enabled)
                //        action += `<a onclick="AccountController.UnActive('${id}')"><i class='fas fa-toggle-off'></i>&nbsp;UnActive</a>`;
                //    else
                //        action += `<a onclick="AccountController.Active('${id}')"><i class='fas fa-toggle-on'></i>&nbsp;Active</a>`;
                //}


                if (item.KeyID == "Update")
                    action += `<a href='${URLA}/Update/${id}' target="_blank"><i class='fas fa-pen-square'></i>&nbsp;${item.Title}</a>`;
                if (item.KeyID == "Delete")
                    action += `<a onclick="${_controlInit}.ConfirmDelete('${id}')" target="_blank"><i class='fas fa-trash'></i>&nbsp;${item.Title}</a>`;
                if (item.KeyID == "Details")
                    action += `<a href='${URLA}/Details/${id}' target="_blank"><i class='fas fa-info-circle'></i>&nbsp;${item.Title}</a>`;
                if (item.KeyID == "Profile")
                    action += `<a href='${URLA}/Profile/${id}' target="_blank"><i class='fas fa-info-circle'></i>&nbsp;${item.Title}</a>`;
                //
            });
            action += `</div></div>`;
            return action;
        }
        return "";
    }
    static AccessInApplication() {

        var _val = $('#txtAccessInApplication').val();
        if (_val == undefined || _val == null) {
            return -1;
        }
        else
            return parseInt(_val);
    }
}

