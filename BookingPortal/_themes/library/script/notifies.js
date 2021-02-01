
//import { setTimeout } from "timers";
$("form").submit(function (e) {
    e.preventDefault();
});
//

var milisecondsInterval = 100000;
class NotifizationResult {

    static Message(text, status) {
        if (status === 200) {
            toastr.success(text);
        }
        else if (status === 500 || status === 501 || status === 502 || status === 503) {
            toastr.error(text);
        }
        else if (status === '000') {
            toastr.info(text);
        }
        else {
            toastr.warning("Lỗi không xác định");
        }
    }
}

class MessageText {
    static Invalid = "Dữ liệu không hợp lệ";
    static NotFound = "Không tìm thấy dữ liệu";
    static NotService = "Xin vui lòng truy cập sau !";
    static Datamissing = "Tồn tại dữ liệu không hợp lệ";
    static Complexity = "Yêu cầu mật khẩu phức tạp hơn";
    static confirm_delete = "Bạn có muốn xóa dữ liệu này không? Dữ liệu không thể khôi phục sau khi xóa.";
}
class Notifization {
    static Success(message, callback, milisecondsInterval) {
        this.showToastrMessage(message, 'success', callback, milisecondsInterval);
    }
    static Warning(message, callback, milisecondsInterval) {
        this.showToastrMessage(message, 'warning', callback, 3000);
    }
    static Error(message, callback, milisecondsInterval) {
        this.showToastrMessage(message, 'error', callback, 3000);
    }
    static showInfoMessage(message, callback, milisecondsInterval) {
        this.showToastrMessage(message, 'info', callback, milisecondsInterval);
    }
    static showQuestionMessage(message, callback, milisecondsInterval) {
        this.showToastrMessage(message, 'question', callback, milisecondsInterval);
    }
    static showToastrMessage(message, type, callback, milisecondsInterval = 2000) {
        toastr.options.timeOut = milisecondsInterval;
        toastr.options.closeButton = false;
        if (type === 'error') {
            toastr.error(message);
        } else if (type === 'success') {
            toastr.success(message);
        } else if (type === 'warning') {
            toastr.warning(message);
        } else if (type === 'info') {
            toastr.info(message);
        }
    }
}

class Paging {
    static Pagination(PageList, totalPage, page, callBack) {
        if (totalPage <= 1) {
            $(PageList).empty();
            return;
        }
        $(PageList).bootpag({
            total: parseInt(totalPage),
            page: page,
            maxVisible: 5,
            leaps: true,
            firstLastUse: true,
            first: '←',
            last: '→',
            wrapClass: 'pagination',
            activeClass: 'active',
            disabledClass: 'disabled',
            nextClass: 'next',
            prevClass: 'prev',
            lastClass: 'last',
            firstClass: 'first',
            callback: callBack
        });
    }
}
class ToolTip {
    static get Delete() {
        return "Xóa";
    }
    static get Edit() {
        return "Sửa";
    }
    static get Add() {
        return "title ='Thêm mới'";
    }
    static get Detail() {
        return "title ='chi tiết'";
    }
    static get Code() {
        return "title ='Mã vạch'";
    }
    static get List() {
        return "title = 'Danh sách'";
    }
    static get Check() {
        return "title ='Chọn'";
    }
    static get UnCheck() {
        return "title ='Bỏ Chọn'";
    }
    static get Seach() {
        return "title ='Tìm kiếm'";
    }
}
class FData {
    static FomatDateVN(strDate) {
        if (strDate.length < 0 || strDate === 'undefined')
            return "---";
        console.log(strDate);
        //Number.prototype.padLeft = function (base, chr) {
        //    var len = (String(base || 10).length - String(this).length) + 1;
        //    return len > 0 ? new Array(len).join(chr || '0') + this : this;
        //};
        //var d = new Date(strDate),
        //    dformat =
        //        [d.getDate().padLeft(),
        //        (d.getMonth() + 1).padLeft(),
        //        d.getFullYear()].join('/');
        //+
        //' ' +
        //[d.getHours().padLeft(),
        //d.getMinutes().padLeft(),
        //d.getSeconds().padLeft()].join(':');
        //return dformat;
    }
    static FomatTimeVN(strDate) {
        if (strDate === "") {
            return "";
        }
        Number.prototype.padLeft = function (base, chr) {
            var len = (String(base || 10).length - String(this).length) + 1;
            return len > 0 ? new Array(len).join(chr || '0') + this : this;
        };
        var d = new Date(strDate),
            dformat =
                //[d.getDate().padLeft(),
                //(d.getMonth() + 1).padLeft(),
                //d.getFullYear()].join('/');
                //+
                //' ' +
                [d.getHours().padLeft(),
                d.getMinutes().padLeft(),
                d.getSeconds().padLeft()].join(':');
        return dformat;
    }
    static ResetForm(tag) {
        var element = document;
        if (tag != undefined || tag != null) {
            element = $(tag);
        }

        $(element).find("input[type=text]:not([data-notreload]) ,input[type=password]:not([data-notreload]),textarea,input[type=file]:not([data-notreload])").val('');

        $(element).find("input[type=checkbox]:not([data-notreload])").prop('checked', false);
  
        $("select:not([data-notreload])").each(function (index,item) {
            $(item)[0].selectedIndex = 0;
            $(item).selectpicker("refresh");
        });
       

        //
        if (tinymce !== undefined) {

            //if ($('#txtNote') !== undefined && $('#txtNote').length > 0)
            //    tinymce.get("txtNote").setContent('');
            //if ($('#txtHtmlText') !== undefined && $('#txtHtmlText').length > 0)
            //    tinymce.get("txtHtmlText").setContent('');
        }
        //
        $(element).find(".file-preview .pre-view").html('');
        $(element).find(".photo-caption-text").html('');
        $(element).find(".new-box-preview").html(' ... chọn file');
        $(element).find(".img-caption-text").html(' ... chọn file');
    }
}
class File {
    static ValidateFileImg() {
        var _validFileExtensions = [".jpg", ".jpeg", ".bmp", ".gif", ".png"];
        var arrInputs = $("input");
        for (var i = 0; i < arrInputs.length; i++) {
            var oInput = arrInputs[i];
            if (oInput.type === "file") {
                var sFileName = oInput.value;
                if (sFileName.length > 0) {
                    var blnValid = false;
                    for (var j = 0; j < _validFileExtensions.length; j++) {
                        var sCurExtension = _validFileExtensions[j];
                        if (sFileName.substr(sFileName.length - sCurExtension.length, sCurExtension.length).toLowerCase() === sCurExtension.toLowerCase()) {
                            blnValid = true;
                            break;
                        }
                    }

                    if (!blnValid) {
                        return false;
                    }
                }
            }
        }
        return true;
    }
    static ValidateFileDoc(extension) {
        return extensionDoc.includes(extension);
    }
}
class BgColor {
    static bg_green() {
        return "bg-green";
    }
    static bg_green_light() {
        return "bg-light-green";
    }
    static bg_red() {
        return "bg-red";
    }
    static bg_yellow() {
        return "bg-yellow";
    }
    static bg_deep_orange() {
        return "bg-deep-orange";
    }

}


// onpage
$(function () {
    Loading.PageLoad();
});

