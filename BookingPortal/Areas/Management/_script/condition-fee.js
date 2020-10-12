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
                url: URLC + '/EventEnd04',
                data: model,
                success: function (response) {
                    if (response !== null) {
                        if (response.status === 200) {
                            Notifization.Success(response.message);
                            $('#applieState04').html(`<i class="far fa-hand-point-right"></i> <label class="col-pink">Không áp dụng</label>`);
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

        $('#btnApply05').off('click').on('click', function () {
            var flg = true;
            var ddlFlightLocationId = $('#ddlFlightLocation05').val();
            //var cbxResbookDesig = $('input[name="cbxResbookDesig"]').is(":checked");
            var txtTimeBookHolder05 = $('#txtTimeBookHolder05').val();
            var txtTimePlaceHolder05 = $('#txtTimePlaceHolder05').val();
            if (ddlFlightLocationId === "") {
                $('#lblFlightLocation05').html('Vui lòng chọn chặng bay');
                flg = false;
            }
            else {
                $('#lblFlightLocation05').html('');
            }

            if (txtTimePlaceHolder05 === '') {
                $('#lblTimePlaceHolder05').html('Không được để trống thời gian giữ chỗ');
                flg = false;
            }
            else {
                if (!FormatNumber.test(txtTimePlaceHolder05)) {
                    $('#lblTimePlaceHolder05').html('Thời gian giữ chỗ không hợp lệ');
                    flg = false;
                }
                else if (parseInt(txtTimePlaceHolder05) < 0 || parseInt(txtTimePlaceHolder05) > 30) {
                    $('#lblTimePlaceHolder05').html('Thời gian giữ chỗ giới hạn từ 0-30 ngày');
                    flg = false;
                }
                else {
                    $('#lblTimePlaceHolder05').html('');
                }
            }

            if (txtTimeBookHolder05 === '') {
                $('#lblTimeBookHolder05').html('Không được để trống thời gian xuất vé');
                flg = false;
            }
            else {
                if (!FormatNumber.test(txtTimeBookHolder05)) {
                    $('#lblTimeBookHolder05').html('Thời gian xuất vé không hợp lệ');
                    flg = false;
                }
                else if (parseInt(txtTimeBookHolder05) < 0 || parseInt(txtTimeBookHolder05) > 30) {
                    $('#lblTimeBookHolder05').html('Thời gian xuất vé giới hạn từ 0-30 ngày');
                    flg = false;
                }
                else {
                    $('#lblTimeBookHolder05').html('');
                }
            }
            // submit form
            if (flg) {
                ConditionFeeConfigController.ConditionFee05Config();
            }
            else
                Notifization.Error(MessageText.Datamissing);
        });
        $('#btEventEnd05').off('click').on('click', function () {
            var ddlFlightLocationId = $('#ddlFlightLocation05').val();
            if (ddlFlightLocationId == undefined || ddlFlightLocationId == "") {
                Notifization.Error("Dữ liệu không hợp lệ");
                return;
            }
            var model = {
                FlightLocationID: ddlFlightLocationId
            };
            AjaxFrom.POST({
                url: URLC + '/EventEnd05',
                data: model,
                success: function (response) {
                    if (response !== null) {
                        if (response.status === 200) {
                            Notifization.Success(response.message);
                            $('#applieState05').html(`<i class="far fa-hand-point-right"></i> <label class="col-pink">Không áp dụng</label>`);
                            $("#ddlFlightLocation05").change();
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
    },
    ConditionFee05Config: function () {
        var ddlFlightLocationId = $('#ddlFlightLocation05').val();
        var txtTimeBookHolder05 = $('#txtTimeBookHolder05').val();
        var txtTimePlaceHolder05 = $('#txtTimePlaceHolder05').val();
        var strResBookDesig = [];
        $('input[name="cbxResBookDesig"]:checkbox:checked').each(function (index, item) {
            if (true) {

            }
            strResBookDesig.push($(item).val());
        });
        if (strResBookDesig.length <= 0) {
            strResBookDesig = [];
        }
        var model = {
            FlightLocationID: ddlFlightLocationId,
            ResBookDesigCode: strResBookDesig,
            TimePlaceHolder: txtTimePlaceHolder05,
            TimeBookHolder: txtTimeBookHolder05
        };
        AjaxFrom.POST({
            url: URLC + '/Condition05',
            data: model,
            success: function (response) {
                if (response !== null) {
                    if (response.status === 200) {
                        Notifization.Success(response.message);
                        $('#applieState05').html(`<i class="far fa-hand-point-right"></i> <label class="col-green">Đang áp dụng</label>`);
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
$(document).on("change", "#ddlFlightLocation05", function () {
    var ddlFlightLocationId = $(this).val();
    if (ddlFlightLocationId === "") {
        $('#lblFlightLocation05').html('Vui lòng chọn chặng bay');
    }
    else {
        $('#lblFlightLocation05').html('');
        $('#lblFlightLocationCode05').html(ddlFlightLocationId);
        var model = {
            FlightLocationID: ddlFlightLocationId,
        };
        AjaxFrom.POST({
            url: URLC + '/GetCondition05',
            data: model,
            success: function (response) {
                if (response !== null) {
                    if (response.status === 200) {
                        //
                        var data = response.data;
                        if (data != null) {

                            var resBookDesigCode = data.ResBookDesigCode; 
 
                            $('#txtTimePlaceHolder05').val(data.TimePlaceHolder);
                            $('#txtTimeBookHolder05').val(data.TimeBookHolder);
                            var arrResBookDesigCode = [];
                            if (resBookDesigCode != null && resBookDesigCode.includes(",")) {
                                var arrResBookDesigCode = resBookDesigCode.split(","); 
                            }
                            $('input[name="cbxResBookDesig"]:checkbox').each(function (index, item) {
                                if (arrResBookDesigCode.includes($(item).val()) == true) {
                                    $(item).prop('checked', true);
                                } else {
                                    $(item).prop('checked', false);
                                }
                            });
                            if (data.IsApplied) {
                                $('#applieState05').html(`<i class="far fa-hand-point-right"></i> <label class="col-green">Đang áp dụng</label>`);
                            }
                            else {
                                $('#applieState05').html(`<i class="far fa-hand-point-right"></i> <label class="col-pink">Không áp dụng</label>`);

                            }
                            
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

$(document).on("keyup", "#txtTimePlaceHolder05", function () {
    var txtTimePlaceHolder05 = $(this).val();
    if (txtTimePlaceHolder05 === '') {
        $('#lblTimePlaceHolder05').html('Không được để trống thời gian giữ chỗ');
    }
    else {
        if (!FormatNumber.test(txtTimePlaceHolder05)) {
            $('#lblTimePlaceHolder05').html('Thời gian giữ chỗ không hợp lệ');
        }
        else if (parseInt(txtTimePlaceHolder05) < 0 || parseInt(txtTimePlaceHolder05) > 30) {
            $('#lblTimePlaceHolder05').html('Thời gian giữ chỗ giới hạn từ 0-30 ngày');
        }
        else {
            $('#lblTimePlaceHolder05').html('');
        }
    }
});

$(document).on("keyup", "#txtTimeBookHolder05", function () {
    var txtTimeBookHolder05 = $(this).val();
    if (txtTimeBookHolder05 === '') {
        $('#lblTimeBookHolder05').html('Không được để trống thời gian xuất vé');
    }
    else {
        if (!FormatNumber.test(txtTimeBookHolder05)) {
            $('#lblTimeBookHolder05').html('Thời gian xuất vé không hợp lệ');
        }
        else if (parseInt(txtTimeBookHolder05) < 0 || parseInt(txtTimeBookHolder05) > 30) {
            $('#lblTimeBookHolder05').html('Thời gian xuất vé giới hạn từ 0-30 ngày');
        }
        else {
            $('#lblTimeBookHolder05').html('');
        }
    }
});
