var pageIndex = 1;
var URLC = "/Management/TicketCondition/Action";
var URLA = "/Management/TicketCondition";
var arrFile = [];
//

var ConditionFeeConfigController = {
    init: function () {
        ConditionFeeConfigController.registerEvent();
    },
    registerEvent: function () {
        $(document).ready(function () {
            // $('[data-dateDefault="true"]').val(LibDateTime.Get_ClientDate(lg = 'en'));
        });
        // **********************************************************************************************************
        $('#btnApply04').off('click').on('click', function () {
            var flg = true;
            // Flight go
            var txtStartNo04 = $('#txtStartNo04').val();
            var txtEndNo04 = $('#txtEndNo04').val();
            var txtStartDate04 = $('#txtStartDate04').val();
            var txtEndDate04 = $('#txtEndDate04').val();
            //
            if (txtStartNo04 === "") {
                $('#lblAircraftNo04').html('Không được để trống số hiệu bắt đầu');
                flg = false;
            } else if (!FormatNumber.test(txtStartNo04)) {
                $('#lblAircraftNo04').html('Số hiệu bắt đầu không hợp lệ 1');
                flg = false;
            }
            else if (txtEndNo04 === "") {
                $('#lblAircraftNo04').html('Không được để trống số hiệu kết thúc');
                flg = false;
            } else if (!FormatNumber.test(txtEndNo04)) {
                $('#lblAircraftNo04').html('Số hiệu bắt đầu không hợp lệ 2');
                flg = false;
            }
            else {
                $('#lblAircraftNo04').html('');
            }
            //
            if (txtStartDate04 != "") {
                if (!FormatDateVN.test(txtStartDate04)) {
                    $('#lblApplyNo04').html('Thời gian bắt đầu không hợp lệ');
                    flg = false;
                } else {
                    $('#lblApplyNo04').html('');
                }
            }
            else if (txtEndDate04 != "") {
                if (!FormatDateVN.test(txtEndDate04)) {
                    $('#lblApplyNo04').html('Thời gian kết thúc không hợp lệ');
                    flg = false;
                } else {
                    $('#lblApplyNo04').html('');
                }
            }
            else {
                $('#lblApplyNo04').html('');
            }

            // submit form
            if (flg) {
                ConditionFeeConfigController.ConditionFee04Config();
            }
            else
                Notifization.Error(MessageText.Datamissing);
        });
        $('#btEventEnd04').off('click').on('click', function () {
            var conditionId = $(this).data('conditionid');
            if (conditionId == undefined || conditionId == "") {
                Notifization.Error("Dữ liệu không hợp lệ");
                return;
            }
            var model = {
                ConditionID: conditionId
            };
            AjaxFrom.POST({
                url: URLC + '/EventEnd',
                data: model,
                success: function (response) {
                    if (response !== null) {
                        if (response.status === 200) {
                            Notifization.Success(response.message);
                            $('#applieState04').html(`<i class="far fa-hand-point-right"></i> <label class="col-pink">Không áp dụng</label>`);
                            FData.ResetForm("#form04");
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
        });
        // **********************************************************************************************************
    },
    ConditionFee04Config: function () {
        var txtStartNo04 = $('#txtStartNo04').val();
        var txtEndNo04 = $('#txtEndNo04').val();
        var txtStartDate04 = $('#txtStartDate04').val();
        var txtEndDate04 = $('#txtEndDate04').val();
        var model = {
            PlaneNoFrom: txtStartNo04,
            PlaneNoTo: txtEndNo04,
            TimeStart: txtStartDate04,
            TimeEnd: txtEndDate04
        };
        AjaxFrom.POST({
            url: URLC + '/Condition04',
            data: model,
            success: function (response) {
                if (response !== null) {
                    if (response.status === 200) {
                        Notifization.Success(response.message);
                        $('#applieState04').html(`<i class="far fa-hand-point-right"></i> <label class="col-green">Đang áp dụng</label>`);
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
    }
};
//
ConditionFeeConfigController.init();
//Validate
//###################################################################################################################################################
$(document).on("keyup", "#txtAmount", function () {
    var txtAmount = $(this).val();
    if (txtAmount === '') {
        $('#lblAmount').html('Không được để trống số tiền nạp');
    }
    else {
        txtAmount = LibCurrencies.ConvertToCurrency(txtAmount);
        if (!FormatCurrency.test(txtAmount)) {
            $('#lblAmount').html('Số tiền nạp không hợp lệ');
        }
        else if (parseFloat(txtAmount) < 0 || parseFloat(txtAmount) > 100000000) {
            $('#lblAmount').html('Số tiền giới hạn từ 0 - 100 000 000 đ');
        }
        else {
            $('#lblAmount').html('');
        }
    }
});
$(document).on("change", "#ddlCustomer", function () {
    var ddlCustomer = $(this).val();
    if (ddlCustomer === "") {
        $('#lblCustomer').html('Vui lòng chọn khách hàng');
    }
    else {
        $('#lblCustomer').html('');
        // get data fill to form 
        var model = {
            AgentID: ddlCustomer
        };
        AjaxFrom.POST({
            url: URLC + '/GetConditionFee',
            data: model,
            success: function (response) {
                if (response !== null) {
                    if (response.status === 200) {
                        //
                        var data = response.data;
                        if (data != null) {
                            var amount = LibCurrencies.FormatToCurrency(data.Amount);
                            $('#txtAmount').val(amount);
                        }
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
                console.log('::' + MessageText.NotService + JSON.stringify(response));
            }
        });

    }
});

