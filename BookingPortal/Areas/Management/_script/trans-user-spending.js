﻿var pageIndex = 1;
var URLC = "/Management/UserSpending/Action";
var URLA = "/Management/UserSpending";
var UserSpendingController = {
    init: function () {
        UserSpendingController.registerEvent();
    },
    registerEvent: function () {
        $("#btnApply").off("click").on("click", function () {
            var flg = true;
            var ddlAgent = $("#ddlAgent").val();
            var ddlEmployee = $("#ddlEmployee").val();
            var txtAmount = $("#txtAmount").val();
            var txtSummary = $("#txtSummary").val();
            //
            $("#lblAgent").html("");
            if (HelperModel.AccessInApplication() != RoleEnum.IsAdminCustomerLogged) {
                if (ddlAgent == "") {
                    $("#lblAgent").html("Vui lòng chọn đại lý");
                    flg = false;
                }
            }
            // transaction id
            if (ddlEmployee == "") {
                $("#lblEmployee").html("Vui lòng chọn nhân viên");
                flg = false;
            }
            else {
                $("#lblEmployee").html("");
            }
            //
            if (txtAmount == "") {
                $("#lblAmount").html("Không được để trống hạn mức");
                flg = false;
            }
            else {
                txtAmount = LibCurrencies.ConvertToCurrency(txtAmount);
                var agentRemain = $("#ddlAgent").find(":selected").data("remain");
                var useSpending = $("#ddlEmployee").find(":selected").data("spending");
                var unLimited = 0;
                var checkLimit = $("#ddlAgent").find(":selected").data("unlimited");
                if (checkLimit != undefined && parseInt(checkLimit) == 1) {
                    unLimited = 1;
                }
                if (!FormatCurrency.test(txtAmount)) {
                    $("#lblAmount").html("Hạn mức không hợp lệ");
                    flg = false;
                }
                else if (parseFloat(txtAmount) < 0) {
                    $("#lblAmount").html("Hạn mức phải >= 0");
                    flg = false;
                }
                else if (parseInt(unLimited) != 1 && parseFloat(txtAmount) > parseFloat(agentRemain + useSpending)) {
                    $("#lblAmount").html("Hạn mức đại lý không đủ");
                    flg = false;
                }
                else {
                    $("#lblAmount").html("");
                }
            }
            //
            if (txtSummary !== "") {
                if (txtSummary.length > 120) {
                    $("#lblSummary").html("Mô tả giới hạn từ 1-> 120 ký tự");
                    flg = false;
                }
                else if (!FormatKeyword.test(txtSummary)) {
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
            // submit
            if (flg) {
                UserSpendingController.Setting();
            }
            else {
                Notifization.Error(MessageText.Datamissing);
            }
        });
        $("#btnSearch").off("click").on("click", function () {
            UserSpendingController.DataList(1);
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
            TimeZoneLocal: LibDateTime.GetTimeZoneByLocal(),
            Status: parseInt($("#ddlStatus").val())
        };
        //
        AjaxFrom.POST({
            url: URLC + "/DataList",
            data: model,
            success: function (result) {
                $("tbody#TblData").html("");
                $("#Pagination").html("");
                if (result !== null) {
                    if (result.status == 200) {
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

                            var customerId = item.CustomerID;
                            var title = item.Title;
                            var summary = item.Summary;
                            var transactionId = item.TransactionID;
                            var bankSent = item.BankSent;
                            var bankIDSent = item.BankIDSent;
                            var bankReceived = item.BankReceived;
                            var bankIDReceived = item.BankIDReceived;
                            var receivedDate = item.ReceivedDate;
                            var amount = item.Amount;
                            var status = item.Status;
                            var enabled = item.Enabled;

                            if (parseInt(status) == 1) {
                                title += `. + ${LibCurrencies.FormatToCurrency(amount)} đ`
                            } else {
                                title += `. - ${LibCurrencies.FormatToCurrency(amount)} đ`
                            }
                            if (summary != null) {
                                summary = ", " + summary;
                            }
                            else {
                                summary = "";
                            }
                            //  role
                            var action = HelperModel.RolePermission(result.role, "UserSpendingController", id);
                            //
                            var rowNum = parseInt(index) + (parseInt(currentPage) - 1) * parseInt(pageSize);
                            rowData += `
                            <tr>
                                 <td class="text-right">${rowNum}&nbsp;</td>
                                 <td>${title}</td>        
                                 <td class="tbcol-created">${item.CreatedBy}</td>                                  
                                 <td class="text-center">${HelperModel.StatusIcon(item.Enabled)}</td>
                                 <td class="text-center">${item.CreatedDate}</td>
                                 <td class="tbcol-action">${action}</td>
                            </tr>`;
                        });
                        $("tbody#TblData").html(rowData);
                        if (parseInt(totalPage) > 1) {
                            Paging.Pagination("#Pagination", totalPage, currentPage, UserSpendingController.DataList);
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
    Setting: function () {
        var ddlAgent = $("#ddlAgent").val();
        var ddlEmployee = $("#ddlEmployee").val();
        var txtAmount = LibCurrencies.ConvertToCurrency($("#txtAmount").val());
        var txtSummary = $("#txtSummary").val();
        var enabled = 0;
        if ($("input[name='cbxActive']").is(":checked"))
            enabled = 1;
        //
        var model = {
            AgentID: ddlAgent,
            TicketingID: ddlEmployee,
            Amount: txtAmount,
            Summary: txtSummary,
            Enabled: enabled
        };
        AjaxFrom.POST({
            url: URLC + "/Setting",
            data: model,
            success: function (response) {
                if (response !== null) {
                    if (response.status == 200) {
                        Notifization.Success(response.message);
                        UserSpendingController.GetAgentHasSending(ddlAgent, true);
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
                if (response !== null) {
                    if (response.status == 200) {
                        Notifization.Success(response.message);
                        UserSpendingController.DataList(pageIndex);
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
                console.log("::" + MessageText.NotService);
            }
        });
    },
    Details: function () {
        var id = $("#txtID").val();
        if (id.length <= 0) {
            Notifization.Error(MessageText.NotService);
            return;
        }
        var fData = {
            Id: $("#txtID").val()
        };
        $.ajax({
            url: "/post/detail",
            data: {
                strData: JSON.stringify(fData)
            },
            type: "POST",
            dataType: "json",
            success: function (result) {
                if (result !== null) {
                    if (result.status == 200) {
                        var item = result.data;
                        $("#LblAccount").html(item.LoginID);
                        $("#LblDate").html(item.CreatedDate);
                        var action = "";
                        if (item.Enabled)
                            action += `<i class="fa fa-toggle-on"></i> actived`;
                        else
                            action += `<i class="fa fa-toggle-off"></i>not active`;

                        $("#LblActive").html(action);
                        $("#lblLastName").html(item.FirstName + " " + item.LastName);
                        $("#LblEmail").html(item.Email);
                        $("#LblPhone").html(item.Phone);
                        $("#LblLanguage").html(item.LanguageID);
                        $("#LblPermission").html(item.PermissionID);

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
                console.log("::" + MessageText.NotService);
            }
        });
    },
    ConfirmDelete: function (id) {
        Confirm.Delete(id, UserSpendingController.Delete, null, null);
    },
    GetAgentHasSending(agentId, _echanged = false) {
        var option = `<option value="">-Lựa chọn-</option>`;
        $("#ddlAgent").html(option);
        $("#ddlAgent").selectpicker("refresh");
        var model = {
            ID: agentId
        };
        AjaxFrom.POST({
            url: "/Management/Agent/Action/GetAgentHasSpending",
            data: model,
            success: function (response) {
                if (response !== null) {
                    if (response.status == 200) {
                        $.each(response.data, function (index, item) {
                            index = index + 1;
                            //
                            var strIndex = "";
                            if (index < 10)
                                strIndex += "0" + index;
                            //
                            var id = item.ID;
                            var codeId = item.CodeID;
                            var title = item.Title;
                            var spending = item.Spending;
                            var remain = item.Remain;
                            var parentId = item.ParentID;
                            var unLimited = 0;
                            if (parentId == null) {
                                unLimited = 1;
                            }
                            var selected = "";
                            if (agentId == id) {
                                selected = "selected";
                            }
                            option += `<option value="${id}" data-spending='${spending}' data-remain='${remain}' ${selected} data-unLimited='${unLimited}'>${title}</option>`;
                        });
                        $("select#ddlAgent").html(option);
                        $("select#ddlAgent").selectpicker("refresh");
                        CanculateAgentSpandingMax();
                        if (_echanged) {
                            $("select#ddlAgent").change();
                        }
                        return;
                    }
                }
                return;
            },
            error: function (result) {
                console.log("::" + MessageText.NotService);
            }
        });
    },
    GetTiketing(agentId, ticketingId) {
        var option = `<option value="">-Lựa chọn-</option>`;
        $("#ddlEmployee").html(option);
        $("#ddlEmployee").selectpicker("refresh");
        var model = {
            ID: agentId
        };
        AjaxFrom.POST({
            url: "/Management/User/Action/GetTicketing",
            data: model,
            success: function (response) {
                if (response !== null) {
                    if (response.status == 200) {
                        $.each(response.data, function (index, item) {
                            index = index + 1;
                            //
                            var strIndex = "";
                            if (index < 10)
                                strIndex += "0" + index;
                            //
                            var id = item.UserID;
                            var title = item.FullName;
                            var spending = item.Spending;
                            var selected = "";
                            if (id == ticketingId) {
                                selected = "selected";
                            }
                            option += `<option value="${id}" data-spending='${spending}' ${selected}>${title}</option>`;
                        });
                        $("select#ddlEmployee").html(option);
                        $("select#ddlEmployee").selectpicker("refresh");
                        CanculateAgentSpandingMax();
                        return;
                    }
                }
                return;
            },
            error: function (result) {
                console.log("::" + MessageText.NotService);
            }
        });
    }
};

UserSpendingController.init();
// Agent
$(document).on("change", "#ddlAgent", function () {
    var ddlAgent = $(this).val();
    $("#lblAgent").html("");
    $("#lblAgentSpend").html(0);
    if (ddlAgent == "") {
        $("#lblAgent").html("Vui lòng chọn đại lý");
        return;
    }
    // canclulate
    var ticketingId = $("#ddlEmployee").val();
    UserSpendingController.GetTiketing(ddlAgent, ticketingId);
});
// emloyee
$(document).on("change", "#ddlEmployee", function () {
    var ddlEmployee = $(this).val();
    $("#txtAmount").val(0);
    if (ddlEmployee == "") {
        $("#lblEmployee").html("Vui lòng chọn nhân viên");
    }
    else {
        $("#lblEmployee").html("");
        var useSpending = $(this).find(":selected").data("spending");
        $("#txtAmount").val(LibCurrencies.FormatToCurrency(useSpending));
        UserSpendingController.GetAgentHasSending($("#ddlAgent").val());

    }
    CanculateAgentSpandingMax();
});
$(document).on("keyup", "#txtAmount", function () {
    var txtAmount = $(this).val();
    if (txtAmount == "") {
        $("#lblAmount").html("Không được để trống hạn mức");
    }
    else {
        txtAmount = LibCurrencies.ConvertToCurrency(txtAmount);
        var agentRemain = $("#ddlAgent").find(":selected").data("remain");
        var useSpending = $("#ddlEmployee").find(":selected").data("spending");
        var unLimited = 0;
        var checkLimit = $("#ddlAgent").find(":selected").data("unlimited");
        if (checkLimit != undefined && parseInt(checkLimit) == 1) {
            unLimited = 1;
        }
        if (!FormatCurrency.test(txtAmount)) {
            $("#lblAmount").html("Hạn mức không hợp lệ");
        }
        else if (parseFloat(txtAmount) < 0) {
            $("#lblAmount").html("Hạn mức phải >= 0");
        }
        else if (unLimited != 1 && parseFloat(txtAmount) > parseFloat(agentRemain + useSpending)) {
            $("#lblAmount").html("Hạn mức đại lý không đủ");
        }
        else {
            $("#lblAmount").html("");
        }
    }
});
// summary
$(document).on("keyup", "#txtSummary", function () {
    var txtSummary = $(this).val();
    if (txtSummary !== "") {
        if (txtSummary.length > 120) {
            $("#lblSummary").html("Mô tả giới hạn từ 1-> 120 ký tự");
            flg = false;
        }
        else if (!FormatKeyword.test(txtSummary)) {
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
});

function CanculateAgentSpandingMax() {
    var agentRemain = 0;
    var userSpending = 0;
    var ddlAgent = $("#ddlAgent").val();
    var ddlEmployee = $("#ddlEmployee").val();
    var limited = "";
    if (ddlAgent != "") {
        agentRemain = $("#ddlAgent").find(":selected").data("remain");
        var checkLimit = $("#ddlAgent").find(":selected").data("unlimited");
        if (checkLimit != undefined && parseInt(checkLimit) == 1) {
            $("#lblAgentSpend").html("UnLimited");
            return;
        }
    }
    if (ddlEmployee != "") {
        userSpending = $("#ddlEmployee").find(":selected").data("spending");
    }
    $("#lblAgentSpend").html(LibCurrencies.FormatToCurrency(parseFloat(agentRemain + userSpending)));
}
