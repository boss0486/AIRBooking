class ValidData {
    static ValidDate(_str, ext) {
        if (ext === "vn") {
            if (FormatDateVN.test(_str))
                return true;
        }
        return false;
    }

    static ValidName(_str, ext) {
        if (ext === "vn") {
            if (FormatFName.test(_str))
                return true;
        }
        return false;
    }

    static ValidPhoneNumber(_str, ext) {
        if (ext === "vn") {
            if (FormatPhone.test(_str))
                return true;
        }
        return false;
    }

    static ValidEmail(_str) {
        if (FormatEmail.test(_str))
            return true;
        return false;
    }
}
// date

// date
class LibDateTime {
    static AddDays(n) {
        var t = new Date();
        t.setDate(t.getDate() + n);
        var month = "0" + (t.getMonth() + 1);
        var date = "0" + t.getDate();
        month = month.slice(-2);
        date = date.slice(-2);
        return date = date + "/" + month + "/" + t.getFullYear();
    }

    static Get_ClientDate(_lg, ext) {
        if (ext === undefined)
            ext = "/";
        else
            ext = "-";
        var t = new Date();
        var day = t.getDate();
        var _month = t.getMonth() + 1;
        if (day.toString().length === 1)
            day = "0" + day;
        if (_month.toString().length === 1)
            _month = "0" + _month;
        return day + ext + _month + ext + t.getFullYear();
    }

    // input datetime: '/Date(1569079409997)/
    static ConvertUnixTimestampToDate(strDate, seperate, languaId) {
        if (strDate !== null && strDate.length > 0) {
            let date = new Date(strDate.match(/\d+/).map(Number)[0]);
            let day = date.getDate();
            if (day.toString().length === 1)
                day = "0" + day;
            let month = date.getMonth() + 1;
            if (month.toString().length === 1)
                month = "0" + month;
            let year = date.getFullYear();
            if (seperate === undefined)
                seperate = "/";
            return day + seperate + month + seperate + year;
        }
    }
    // get time from rexgex

    static GetTime(str, ext) {
        var matches = str.match(/([0-9]?[0-9]?:[0-9]?[0-9]):[0-9]?[0-9]/i);
        if (ext !== undefined) {
            matches = str.match(/([0-9]?[0-9]?:[0-9]?[0-9]):[0-9]?[0-9](\s[ap]m)/i);
        }
        var time = str;
        if (matches !== null) {
            time = matches[0];
        }
        return time;
    }

    static GetDate(str, ext) {
        var d1 = Date.parse(str);
        if (ext === undefined) {
            ext = "-";
        }
        let date = new Date(str);
        let day = date.getDate();
        if (day.toString().length === 1)
            day = "0" + day;
        let month = date.getMonth() + 1;
        if (month.toString().length === 1)
            month = "0" + month;
        let year = date.getFullYear();

        return day + ext + month + ext + year;


        if (ext.includes("/")) {
            return d1.toString('dd/mm/yyyy');
        }
        else
            return d1.toString('dd-mm-yyyy');
    }
    static FormatDateForAPI(date, ext) {
        if (ext === undefined)
            ext = "/";
        else
            ext = "-";
        //
        var initial = "";
        if (date.includes("/")) {
            initial = date.split(/\//);
        }
        if (date.includes("-")) {
            initial = date.split(/-/);
        }
        return [initial[1], initial[0], initial[2]].join('/');
    }
    static FormatToServerDate(_date) {
        if (_date == '') {
            return "";
        }
        var res = _date.split("-");
        return res[2] + "-" + res[1] + "-" + res[0];
    }

    //

    //
    static GetTimeZoneByLocal2(callback) {
        // timezone local
        var utcLocal = Intl.DateTimeFormat().resolvedOptions().timeZone;
        //
        if (utcLocal == undefined || utcLocal == "") {
            return "";
        }
        var file = "/library/script/timezones.json";
        var result = "";
        var rawFile = new XMLHttpRequest();
        rawFile.overrideMimeType("application/json");
        rawFile.open("GET", file, true);
        rawFile.onreadystatechange = function () {
            if (rawFile.readyState === 4 && rawFile.status == "200") {
                $.each(JSON.parse(rawFile.response), function (key, val) {
                    if (val.utc != undefined && val.utc != null) {
                        $.each(val.utc, function (utcKey, utcVal) {
                            if (utcVal == utcLocal) {
                                callback(val.value);
                            }
                        });
                    }
                });
            }
        }
        rawFile.send(null);

        //var a = "";
        //const result = await $.ajax({
        //    url: "/library/script/timezones.json",
        //    dataType: 'json',
        //    async: true,
        //    data: '',
        //    success: function (data) {
        //        $.each(data, function (key, val) {
        //            console.log("------------------------------------------:" + JSON.stringify(val.value));

        //            if (val.utc != undefined && val.utc != null) {
        //                $.each(val.utc, function (utcKey, utcVal) {
        //                    console.log(JSON.stringify(utcVal));
        //                    if (utcVal == utcLocal) {
        //                        console.log("------------------------------------------:ok" + val.value);
        //                        a = val.value;
        //                    }
        //                });
        //            }
        //        });

        //    }
        //});

        //return JSON.stringify(a);

    }

    static GetTimeZoneByLocal() {
        // timezone local
        var utcLocal = Intl.DateTimeFormat().resolvedOptions().timeZone;
        //
        if (utcLocal == undefined || utcLocal == "") {
            return "";
        }
        return utcLocal;
        //var result = "";
        //$.ajax({
        //    url: "/library/script/timezones.json",
        //    async: true,
        //    success: function (data) {
        //        $.each(data, function (key, val) {
        //            var _val = val.value;
        //            if (val.utc != undefined && val.utc != null) {
        //                $.each(val.utc, function (utcKey, utcVal) {
        //                    if (utcVal == utcLocal) {
        //                        result = _val;
        //                        // return _val;
        //                    }
        //                });
        //            }
        //        });

        //    }
        //});
        //return result;
        //var a = "";
        //const result = await $.ajax({
        //    url: "/library/script/timezones.json",
        //    dataType: 'json',
        //    async: true,
        //    data: '',
        //    success: function (data) {
        //        $.each(data, function (key, val) {
        //            console.log("------------------------------------------:" + JSON.stringify(val.value));

        //            if (val.utc != undefined && val.utc != null) {
        //                $.each(val.utc, function (utcKey, utcVal) {
        //                    console.log(JSON.stringify(utcVal));
        //                    if (utcVal == utcLocal) {
        //                        console.log("------------------------------------------:ok" + val.value);
        //                        a = val.value;
        //                    }
        //                });
        //            }
        //        });

        //    }
        //});

        //return JSON.stringify(a);

    }
}

class LibCurrencies {
    static FormatToCurrency(n, dp) {
        var s = '' + (Math.floor(n)), d = n % 1, i = s.length, r = '';
        while ((i -= 3) > 0) { r = ' ' + s.substr(i, 3) + r; }
        return s.substr(0, i + 3) + r + (d ? '.' + Math.round(d * Math.pow(10, dp || 2)) : '');
    }

    static ConvertToCurrency(_val) {
        //
        if (_val != undefined && _val != '')
            return _val.replace(/\s/g, "");
        else
            return 0;
    }
}
class SubStringText {
    static SubText(_length, _text) {
        if (_text !== '' > 0 && _text.length > _length)
            return _text.substring(0, _length);
        return _text;
    }
    static SubTitle(_text) {
        if (_text == undefined || _text == null)
            return '';
        if (_text !== '' && _text.length > 65)
            return _text.substring(0, 65);
        //
        return _text;
    }
    static SubSummary(_text) {
        if (_text == undefined || _text == null)
            return '';
        if (_text !== '' && _text.length > 65)
            return _text.substring(0, 65);
        return _text;
    }
    static SubFileName(_text) {
        if (_text !== undefined && _text === null)
            return '';
        if (_text !== null && _text.length > 10)
            return "..." + _text.substring(_text.length - 10, _text.length);
        return _text;
    }
}
// web method
class AttachmentFile {
    static ImgFile(_fileName, _createdDate) {
        if (_fileName !== '' && _fileName !== undefined) {
            var _pathFile = "/files/upload/" + _fileName;
            $.ajax({
                url: _pathFile,
                error: function () {
                    return IFile.NoImangeFile;
                },
                success: function () {
                    return _pathFile;
                }
            });
        } else
            return IFile.NoImangeFile;

    }
    static ProductImg(_fileName, _createdDate) {
        if (_fileName !== '' && _fileName !== undefined) {
            var year = createdDate.Year;
            var month = createdDate.Month;
            //return "/files/upload/" + year + "/" + month + "/" + id + "" + extension;
            var _pathFile = "/files/upload/" + _fileName;
            $.ajax({
                url: _pathFile,
                error: function () {
                    return IFile.NoImangeFile;
                },
                success: function () {
                    return _pathFile;
                }
            });
        } else
            return IFile.NoImangeFile;
    }
}

class IFile {
    static NoImangeFile = "/files/default/00000000-0000-0000-0000-000000000000.gif";
    static NoImangeUser = "/files/default/00000000-0000-0000-0000-000000000000.gif";

}
// cookie
class Cookies {
    static SetCookie(_name, _val) {
        $.cookie(_name, _val, {
            expires: 10, // Expires in 10 days
            path: '/',   // The value of the path attribute of the cookie // (Default: path of page that created the cookie).
            domain: window.location.hostname, // The value of the domain attribute of the cookie// (Default: domain of page that created the cookie).
            //secure: false // If set to true the secure attribute of the cookie
            // will be set and the cookie transmission will
            // require a secure protocol (defaults to false).
        });
    }
    static GetCookie(_name) {
        return $.cookie(_name);
    }
    static DelCookie(_name) {
        $.removeCookie(_name);
    }
}
class Confirm {
    static Text_Delete = 1;
    static Text_ExportTicket = 2;
    static Text_VoidTicket = 3;

    static ConfirmYN(id, funcY, title_enum = 0) {
        var _content = "...";
        if (title_enum == 1)
            _content = "Thực hiện xóa dữ liệu, bạn có chắc chắn muốn xóa dữ liệu này không ?";
        // 
        if (title_enum == 2)
            _content = "Thực hiện xuất vé. Bạn có chắc chắn muốn thực hiện hành động này không?"
        //
        if (title_enum == 3)
            _content = "Thực hiện hủy vé. Bạn có chắc chắn muốn thực hiện hành động này không?"
        // 
        $.confirm({
            title: 'Xác nhận!',
            content: _content,
            buttons: {
                confirm: function () {
                    Notifization.Success("Thực hiện hành động");
                    setTimeout(function () {
                        funcY(id);
                    }, 2000);
                    //return;
                },
                cancel: function () {
                    Notifization.Error("Hủy hành động");
                    return;
                }
            }
        });
    }
}

