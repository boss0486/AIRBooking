var ajaxStatus = 0;
//$(document).ajaxStart(function () {
//    ajaxStatus++;  
//    console.log("ajaxStatus:" + ajaxStatus);

//}).ajaxComplete(function () {
//    ajaxStatus--;  
//    console.log("ajaxStatus:" + ajaxStatus);
//});

class AjaxFrom {
    static POST(_form) {
        Loading.ShowLoading();
        ajaxStatus++;
        _form.method = "POST";
        _form.dataType = 'json';
        _form.async = false;
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
    static POSTFILE(_form) {
        Loading.ShowLoading();
        ajaxStatus++;
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
}

