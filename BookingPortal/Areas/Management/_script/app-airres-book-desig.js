var pageIndex = 1;
var URLC = "/Management/AirResBookDesig/Action";
var URLA = "/Management/AirResBookDesig";
var arrFile = [];
var AirResBookDesigController = {
    init: function () {
        AirResBookDesigController.registerEvent();
    },
    registerEvent: function () {
        $(document).ready(function () {

        });
        $("#btnCreate").off("click").on("click", function () {
            var flg = true;
            // title
            var title = $("#txtTitle").val();
            var txtCode = $("#txtCode").val();
            var txtVoidBookTime = $("#txtVoidBookTime").val();
            var summary = $("#txtSummary").val();
            //
            if (title === "") {
                $("#lblTitle").html("Không được để trống tiêu đề");
                flg = false;
            }
            else if (title.length < 1 || title.length > 80) {
                $("#lblTitle").html("Tiêu đề giới hạn từ 1-> 80 characters");
                flg = false;
            }
            else if (!FormatKeyword.test(title)) {
                $("#lblTitle").html("Tiêu đề không hợp lệ");
                flg = false;
            }
            else {
                $("#lblTitle").html("");
            }
            // code
            $("#lblCode").html("");
            if (txtCode === "") {
                $("#lblCode").html("Không được để trống hạng đặt chỗ");
                flg = false;
            }
            else if (txtCode.length != 1) {
                $("#lblCode").html("Hạng đặt chỗ giới hạn 1 ký tự");
                flg = false;
            }
            else if (!FormatKeyId.test(txtCode)) {
                $("#lblCode").html("Hạng đặt chỗ không hợp lệ");
                flg = false;
            }
            //
            $("#lblVoidBookTime").html("");
            if (txtVoidBookTime == "") {
                $("#lblVoidBookTime").html("Không được để trống hạng đặt chỗ");
                flg = false;

            } else if (!FormatNumber.test(txtVoidBookTime)) {
                $("#lblVoidBookTime").html("Thời gian hủy đặt chỗ không hợp lệ");
                flg = false;
            }
            // summary 
            $("#lblSummary").html("");
            if (summary !== "") {
                if (summary.length < 1 || summary.length > 120) {
                    $("#lblSummary").html("Mô tả giới hạn từ 1-> 120 ký tự");
                    flg = false;
                }
                else if (!FormatKeyword.test(summary)) {
                    $("#lblSummary").html("Mô tả không hợp lệ");
                    flg = false;
                }
                else {
                    $("#lblSummary").html("");
                }
            }
            // submit form
            if (flg)
                AirResBookDesigController.Create();
            else
                Notifization.Error(MessageText.Datamissing);
        });
        $("#btnSearch").off("click").on("click", function () {
            AirResBookDesigController.DataList(1);
        });
        $("#btnUpdate").off("click").on("click", function () {
            var flg = true;
            // code
            var txtCode = $("#txtCode").val();
            if (txtCode === "") {
                $("#lblCode").html("Không được để trống hạng đặt chỗ");
                flg = false;
            }
            else if (txtCode.length != 1) {
                $("#lblCode").html("Hạng đặt chỗ giới hạn 1 ký tự");
                flg = false;
            }
            else if (!FormatKeyId.test(txtCode)) {
                $("#lblCode").html("Hạng đặt chỗ không hợp lệ");
                flg = false;
            }
            else {
                $("#lblCode").html("");
            }
            // title
            var title = $("#txtTitle").val();
            if (title === "") {
                $("#lblTitle").html("Không được để trống tiêu đề");
                flg = false;
            }
            else if (title.length < 1 || title.length > 80) {
                $("#lblTitle").html("Tiêu đề giới hạn từ 1-> 80 characters");
                flg = false;
            }
            else if (!FormatKeyword.test(title)) {
                $("#lblTitle").html("Tiêu đề không hợp lệ");
                flg = false;
            }
            else {
                $("#lblTitle").html("");
            }
            // summary
            var summary = $("#txtSummary").val();
            if (summary !== "") {
                if (summary.length < 1 || summary.length > 120) {
                    $("#lblSummary").html("Mô tả giới hạn từ 1-> 120 ký tự");
                    flg = false;
                }
                else if (!FormatKeyword.test(summary)) {
                    $("#lblSummary").html("Mô tả không hợp lệ");
                    flg = false;
                }
                else {
                    $("#lblSummary").html("");
                }
            }
            else {
                $("#lblSummary").html("");
            }
            // submit form
            if (flg) {
                AirResBookDesigController.Update();
            }
            else {
                Notifization.Error(MessageText.Datamissing);
            }
        });
        $("#btnSetting").off("click").on("click", function () {
            var flg = true;
            // Area 
            var axFee = LibCurrencies.ConvertToCurrency($("#txtAxFee").val());
            var txtVoidBookTime = $("#txtVoidBookTime").val();
            var txtVoidTicketTime = $("#txtVoidTicketTime").val();

            $("#lblAxFee").html("");
            if (axFee != "") {
                axFee = LibCurrencies.ConvertToCurrency(axFee);
                if (!FormatCurrency.test(axFee)) {
                    $("#lblAxFee").html("Phí sân bay không hợp lệ");
                    flg = false;
                }
            }
            $("#lblVoidBookTime").html("");
            if (txtVoidBookTime != "") {
                if (!FormatNumber.test(txtVoidBookTime)) {
                    $("#lblVoidBookTime").html("Thời gian hủy đặt chỗ không hợp lệ");
                    flg = false;
                }
            }
            $("#lblVoidTicketTime").html("");
            if (txtVoidTicketTime != "") {
                if (!FormatNumber.test(txtVoidTicketTime)) {
                    $("#lblVoidTicketTime").html("Thời gian hủy vé không hợp lệ");
                    flg = false;
                }
            }
            // submit form
            if (flg) {
                AirResBookDesigController.Setting();
            }
            else {
                Notifization.Error(MessageText.Datamissing);
            }
        });
    },
    DataList: function (page) {
        //
        var ddlTimeExpress = $("#ddlTimeExpress").val();
        var txtStartDate = $("#txtStartDate").val();
        var txtEndDate = $("#txtEndDate").val();
        var model = {
            Query: $("#txtQuery").val(),
            Page: page,
            TimeExpress: parseInt(ddlTimeExpress),
            StartDate: txtStartDate,
            EndDate: txtEndDate,
            Status: parseInt($("#ddlStatus").val()),
            TimeZoneLocal: LibDateTime.GetTimeZoneByLocal(),
            AreaID: "",
            ProviceID: "",
        };
        //
        AjaxFrom.POST({
            url: URLC + "/DataList",
            data: model,
            success: function (result) {
                $("tbody#TblData").html("");
                $("#Pagination").html("");
                if (result !== null) {
                    if (result.status === 200) {
                        var currentPage = 1;
                        var pagination = result.paging;
                        if (pagination !== null) {
                            totalPage = pagination.TotalPage;
                            currentPage = pagination.Page;
                            pageSize = pagination.PageSize;
                            pageIndex = pagination.Page;
                        }
                        var rowData = "";
                        $.each(result.data, function (index, item) {
                            index = index + 1;
                            var id = item.ID;
                            if (id.length > 0)
                                id = id.trim();
                            //
                            var _title = SubStringText.SubTitle(item.Title);
                            var _summary = SubStringText.SubTitle(item.Summary);
                            var _txtCode = item.CodeID;
                            var _voidBookTime = item.VoidBookTime;
                            //  role
                            var action = HelperModel.RolePermission(result.role, "AirResBookDesigController", id);
                            //
                            var rowNum = parseInt(index) + (parseInt(currentPage) - 1) * parseInt(pageSize);
                            rowData += `
                            <tr>
                                 <td class="text-right">${rowNum}&nbsp;</td>  
                                 <td class="">${_title}</td>  
                                 <td class="text-center bg-success">${_txtCode}</td>                                                                                                                                                                                                                                                                         
                                 <td class="text-right">${_voidBookTime} h</td>                                                                                                                                                                                                                                                                         
                                 <td class="text-left">${_summary}</td>                                                                                                                                                                                                                                                                         
                                 <td class="text-center">${HelperModel.StatusIcon(item.Enabled)}</td>
                                 <td class="tbcol-action">${action}</td>
                            </tr>`;
                        });
                        $("tbody#TblData").html(rowData);
                        if (parseInt(totalPage) > 1) {
                            Paging.Pagination("#Pagination", totalPage, currentPage, AirResBookDesigController.DataList);
                        }
                        return;
                    }
                    else {
                        //Notifization.Error(result.message);
                        console.log("::" + result.message);
                        return;
                    }
                }
                Notifization.Error(MessageText.NotService);
                return;
            },
            error: function (result) {
                console.log("::" + MessageText.NotService);
            }
        });
    },
    Create: function () {
        var title = $("#txtTitle").val();
        var txtCode = $("#txtCode").val();
        var summary = $("#txtSummary").val();
        var txtVoidBookTime = $("#txtVoidBookTime").val();
        var enabled = 0;
        if ($("input[name='cbxActive']").is(":checked"))
            enabled = 1;
        //
        var model = {
            Title: title,
            CodeID: txtCode,
            VoidBookTime: txtVoidBookTime,
            Summary: summary,
            Enabled: enabled
        };
        AjaxFrom.POST({
            url: URLC + "/Create",
            data: model,
            success: function (response) {
                if (response === null || response.status === undefined) {
                    Notifization.Error(MessageText.NotService);
                    return;
                }
                if (response.status === 200) {
                    Notifization.Success(response.message);
                    FData.ResetForm();
                    return;
                }
                Notifization.Error(response.message);
                return;
            },
            error: function (response) {
                console.log("::" + MessageText.NotService);
            }
        });

    },
    Update: function () {
        var id = $("#txtID").val();
        var title = $("#txtTitle").val();
        var txtCode = $("#txtCode").val();
        var summary = $("#txtSummary").val();
        var txtVoidBookTime = $("#txtVoidBookTime").val();
        var enabled = 0;
        if ($("input[name='cbxActive']").is(":checked"))
            enabled = 1;
        //
        var model = {
            ID: id,
            Title: title,
            CodeID: txtCode,
            Summary: summary,
            VoidBookTime: txtVoidBookTime,
            Enabled: enabled
        };
        AjaxFrom.POST({
            url: URLC + "/Update",
            data: model,
            success: function (response) {
                if (response === null || response.status === undefined) {
                    Notifization.Error(MessageText.NotService);
                    return;
                }
                if (response.status === 200) {
                    Notifization.Success(response.message);
                    return;
                }
                Notifization.Error(response.message);
                return;
            },
            error: function (response) {
                console.log("::" + MessageText.NotService);
            }
        });
    },
    Setting: function () {
        var id = $("#txtID").val();
        var axFee = LibCurrencies.ConvertToCurrency($("#txtAxFee").val());
        var txtVoidBookTime = $("#txtVoidBookTime").val();
        var txtVoidTicketTime = $("#txtVoidTicketTime").val();
        //
        var model = {
            ID: id,
            VoidBookTime: txtVoidBookTime,
            VoidTicketTime: txtVoidTicketTime,
            AxFee: axFee
        };
        AjaxFrom.POST({
            url: URLC + "/Setting",
            data: model,
            success: function (response) {
                if (response === null || response.status === undefined) {
                    Notifization.Error(MessageText.NotService);
                    return;
                }
                if (response.status === 200) {
                    Notifization.Success(response.message);
                    return;
                }
                Notifization.Error(response.message);
                return;
            },
            error: function (response) {
                console.log("::" + MessageText.NotService);
            }
        });
    },
    Delete: function (id) {
        var model = {
            Id: id
        };
        AjaxFrom.POST({
            url: URLC + "/Delete",
            data: model,
            success: function (response) {
                if (response === null || response.status === undefined) {
                    Notifization.Error(MessageText.NotService);
                    return;
                }
                if (response.status === 200) {
                    Notifization.Success(response.message);
                    AirResBookDesigController.DataList(pageIndex);
                    return;
                }
                Notifization.Error(response.message);
                return;
            },
            error: function (response) {
                console.log("::" + MessageText.NotService);
            }
        });
    },
    ConfirmDelete: function (id) {
        Confirm.DeleteYN(id, AirResBookDesigController.Delete, null, null);
    }
};
//
AirResBookDesigController.init();
//
$(document).on("keyup", "#txtTitle", function () {
    var title = $(this).val();
    if (title === "") {
        $("#lblTitle").html("Không được để trống tiêu đề");
    }
    else if (title.length < 1 || title.length > 80) {
        $("#lblTitle").html("Tiêu đề giới hạn từ 1-> 80 characters");
    }
    else if (!FormatKeyword.test(title)) {
        $("#lblTitle").html("Tiêu đề không hợp lệ");
    }
    else {
        $("#lblTitle").html("");
    }
});
$(document).on("keyup", "#txtCode", function () {
    var txtCode = $(this).val();
    if (txtCode === "") {
        $("#lblCode").html("Không được để trống hạng đặt chỗ");
    }
    else if (txtCode.length != 1) {
        $("#lblCode").html("Hạng đặt chỗ giới hạn 1 ký tự");
    }
    else if (!FormatKeyId.test(txtCode)) {
        $("#lblCode").html("Hạng đặt chỗ không hợp lệ");
    }
    else {
        $("#lblCode").html("");
    }
});
$(document).on("keyup", "#txtSummary", function () {
    var summary = $(this).val();
    if (summary !== "") {
        if (summary.length < 1 || summary.length > 120) {
            $("#lblSummary").html("Tiêu đề giới hạn từ 1-> 80 characters");
        }
        else if (!FormatKeyword.test(summary)) {
            $("#lblSummary").html("Mô tả không hợp lệ");
        }
        else {
            $("#lblSummary").html("");
        }
    }
    else {
        $("#lblSummary").html("");
    }
});
$(document).on("keyup", "#txtVoidBookTime", function () {
    var txtVoidBookTime = $(this).val();
    $("#lblVoidBookTime").html("");
    if (txtVoidBookTime == "") {
        $("#lblVoidBookTime").html("Không được để trống hạng đặt chỗ");

    } else if (!FormatNumber.test(txtVoidBookTime)) {
        $("#lblVoidBookTime").html("Thời gian hủy đặt chỗ không hợp lệ");
    }
});

