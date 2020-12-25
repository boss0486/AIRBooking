var pageIndex = 1;
var URLC = "/Management/AgentFee/Action";
var URLA = "/Management/AgentFee";
var arrFile = [];
//

var AgentFeeConfigController = {
    init: function () {
        AgentFeeConfigController.registerEvent();
    },
    registerEvent: function () {
        $(document).ready(function () {
            // $('[data-dateDefault="true"]').val(LibDateTime.Get_ClientDate(lg = 'en'));
        });
        $('#btnInlandUpdate').off('click').on('click', function () {
            var flg = true;
            var ddlAgentID = $('#ddlAgentID').val();
            if (ddlAgentID === "") {
                $('#lblAgentID').html('Vui lòng chọn đại lý 3');
                flg = false;
            }
            else {
                $('#lblAgentID').html('');
            }
            //
            var arrFee = [];
            $('#TblInlandFeeData input[name="inpFee"]').each(function (index, item) {
                var txtAmount = $(item).val();
                txtAmount = LibCurrencies.ConvertToCurrency(txtAmount);
                if (!FormatCurrency.test(txtAmount)) {
                    $('#lblFee').html('Phí không hợp lệ');
                    $(item).addClass("error");
                    flg = false;
                }
                else if (parseFloat(txtAmount) < 0 || parseFloat(txtAmount) > 100000000) {
                    $('#lblFee').html('Phí giới hạn từ [0-100 000 000] đ');
                    $(item).addClass("error");
                    flg = false;
                }
                else {
                    $('#lblFee').html('');
                    $(item).removeClass("error");
                    //
                    var airlineCode = $(item).data("airlinecode");
                    var itineraryId = $(item).data("itineraryid");

                    var airlineFee = {
                        AirlineCode: airlineCode,
                        ItineraryID: itineraryId,
                        Amount: LibCurrencies.FormatToCurrency(txtAmount)
                    }
                    arrFee.push(airlineFee);
                }
            });
            if (arrFee.length <= 0)
                flg = false;
            // 
            if (!flg)
                Notifization.Error("Dữ liệu phí nội địa không hợp lệ");
            else

                AgentFeeConfigController.AgentFeeConfig(1);
        });
        //International  *************************************************************************************************************************************************************
        $('#btnInternationalUpdate').off('click').on('click', function () {
            var flg = true;
            var ddlAgentID = $('#ddlAgentID').val();
            if (ddlAgentID === "") {
                $('#lblAgentID').html('Vui lòng chọn đại lý 2');
                flg = false;
            }
            else {
                $('#lblAgentID').html('');
            }
            var ddlNational = $('#ddlNational').val();
            if (ddlNational === "") {
                $('#lblNational').html('Vui lòng chọn quốc gia');
                flg = false;
            }
            else {
                $('#lblNational').html('');
            }
            var arrFee = [];
            $('#TblInternationalFeeData input[name="inpFee"]').each(function (index, item) {
                var txtAmount = $(item).val();
                txtAmount = LibCurrencies.ConvertToCurrency(txtAmount);
                if (!FormatCurrency.test(txtAmount)) {
                    $('#lblFee').html('Phí không hợp lệ');
                    $(item).addClass("error");
                    flg = false;
                }
                else if (parseFloat(txtAmount) < 0 || parseFloat(txtAmount) > 100000000) {
                    $('#lblFee').html('Phí giới hạn từ [0-100 000 000] đ');
                    $(item).addClass("error");
                    flg = false;
                }
                else {
                    $('#lblFee').html('');
                    $(item).removeClass("error");
                    //
                    var airlineCode = $(item).data("airlinecode");
                    var itineraryId = $(item).data("itineraryid");

                    var airlineFee = {
                        AirlineCode: airlineCode,
                        ItineraryID: itineraryId,
                        Amount: LibCurrencies.FormatToCurrency(txtAmount)
                    }
                    arrFee.push(airlineFee);
                }
            });
            if (arrFee.length <= 0)
                flg = false;
            // 
            if (!flg)
                Notifization.Error("Dữ liệu phí quốc tế không hợp lệ");
            else
                AgentFeeConfigController.AgentFeeConfig(2);
        });
        $('#btnSearch').off('click').on('click', function () {
            AgentFeeConfigController.DataList(1);
        });
    },
    DataList: function (page) {
        var model = {
            Query: $('#txtQuery').val(),
            Page: page,
            Status: -1,
            TimeExpress: 0,
            StartDate: "",
            EndDate: "",
            TimeZoneLocal: LibDateTime.GetTimeZoneByLocal(),
        };
        //
        AjaxFrom.POST({
            url: URLC + '/DataList',
            data: model,
            success: function (result) {
                $('tbody#TblData').html('');
                $('#Pagination').html('');
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
                        var rowData = '';
                        $.each(result.data, function (index, item) {
                            index = index + 1;
                            var id = item.ID;
                            if (id.length > 0)
                                id = id.trim();
                            //
                            var _unit = 'đ';
                            var _mkh = SubStringText.SubTitle(item.Title);
                            var _amount = item.Amount;
                            var _agentName = SubStringText.SubTitle(item.AgentName);
                            //  role
                            var action = HelperModel.RolePermission(result.role, "AgentFeeConfigController", id);
                            //
                            var rowNum = parseInt(index) + (parseInt(currentPage) - 1) * parseInt(pageSize);
                            rowData += `
                            <tr>
                                 <td class="text-right">${rowNum}&nbsp;</td> 
                                 <td class='text-left'>${_mkh}</td>                                                                                                                                                                                                                                                                         
                                 <td class='text-left'>${_agentName}</td>                                                                                                                                                                                                                                                                         
                                 <td class='text-right'>${_amount} ${_unit}</td>                                                                                                                                                                                                                                                                         
                                 <td class="tbcol-action">${action}</td>
                            </tr>`;
                        });
                        $('tbody#TblData').html(rowData);
                        if (parseInt(totalPage) > 1) {
                            Paging.Pagination("#Pagination", totalPage, currentPage, AgentFeeConfigController.DataList);
                        }
                        return;
                    }
                    else {
                        //Notifization.Error(result.message);
                        console.log('::' + result.message);
                        return;
                    }
                }
                Notifization.Error(MessageText.NotService);
                return;
            },
            error: function (result) {
                console.log('::' + MessageText.NOTSERVICES);
            }
        });
    },
    AgentFeeConfig: function (_itineraryType) {
        var ddlAgentID = $('#ddlAgentID').val();
        var ddlNational = "";
        var arrFee = [];
        if (_itineraryType == 1) {
            $('#TblInlandFeeData input[name="inpFee"]').each(function (index, item) {
                var txtAmount = $(item).val();
                var airlineId = $(item).data("airlineid");
                var airlineCode = $(item).data("airlinecode");
                var airlineFee = {
                    AirlineID: airlineId,
                    AirlineCode: airlineCode,
                    Amount: LibCurrencies.ConvertToCurrency(txtAmount)
                }
                arrFee.push(airlineFee);
            });
        }
        //
        if (_itineraryType == 2) {
            ddlNational = $('#ddlNational').val();
            $('#TblInternationalFeeData input[name="inpFee"]').each(function (index, item) {
                var txtAmount = $(item).val();
                var airlineId = $(item).data("airlineid");
                var airlineCode = $(item).data("airlinecode");
                var airlineFee = {
                    AirlineID: airlineId,
                    AirlineCode: airlineCode,
                    Amount: LibCurrencies.ConvertToCurrency(txtAmount)
                }
                arrFee.push(airlineFee);
            });
        }
        if (arrFee.length <= 0) {
            Notifization.Error("Dữ liệu bảng phí không hợp lệ");
            return;
        }
        var model = {
            ItineraryType: _itineraryType,
            NationalID: ddlNational,
            AgentID: ddlAgentID,
            AirlineFees: arrFee
        };
        AjaxFrom.POST({
            url: URLC + '/ConfigFee',
            data: model,
            success: function (response) {
                if (response !== null) {
                    if (response.status === 200) {
                        Notifization.Success(response.message);
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
AgentFeeConfigController.init();
//Validate
//###################################################################################################################################################
$(document).on("keyup", '#TblInlandFeeData input[name="inpFee"]', function () {
    var txtAmount = $(this).val();
    txtAmount = LibCurrencies.ConvertToCurrency(txtAmount);
    if (!FormatCurrency.test(txtAmount)) {
        $('#lblFee').html('Phí không hợp lệ');
        $(this).addClass("error");
    }
    else if (parseFloat(txtAmount) < 0 || parseFloat(txtAmount) > 100000000) {
        $('#lblFee').html('Phí giới hạn từ [0-100 000 000] đ');
        $(this).addClass("error");
    }
    else {
        $('#lblFee').html('');
        $(this).removeClass("error");
    }
});

$(document).on("change", "#ddlAgentID", function () {
    $('#lblAgentCode').html("");
    var ddlAgentId = $(this).val();
    if (ddlAgentId === "") {
        $('#lblAgentID').html('Vui lòng chọn đại lý 1');
    }
    else {
        var codeid = $(this).find(':selected').data('codeid');
        $('#lblAgentCode').html(codeid);
        $('#lblAgentID').html('');
        //
        var ddlNational = $('#ddlNational').val();
        AgentFeeInlandLoad(ddlAgentId);
        AgentFeeInternationalLoad(ddlAgentId, ddlNational)
    }
});

$(document).on("change", "#ddlNational", function () {
    // 
    var ddlAgentID = "";
    if ($("#ddlAgentID") != undefined) {
        ddlAgentID = $("#ddlAgentID").val();
    }
    //
    $('#lblNationalCode').html("");
    var ddlNational = $('#ddlNational').val();
    if (ddlNational === "") {
        $('#lblNational').html('Vui lòng chọn quốc gia');
    }
    else {
        var codeid = $(this).find(':selected').data('codeid');
        $('#lblNationalCode').html(codeid);
        $('#lblNational').html('');
        //load data 
        AgentFeeInternationalLoad(ddlAgentID, ddlNational);
    }
});

function AgentFeeInternationalLoad(_agentId, _nationalId) {
    $('#TblInternationalFeeData input[name="inpFee"]').each(function (indexTbl, itemTbl) {
        $(itemTbl).val(0);
    });
    var model = {
        AgentID: _agentId,
        NationalID: _nationalId,
        ItineraryID: 2,
    };
    AjaxFrom.POST({
        url: URLC + '/GetFeeConfig',
        data: model,
        success: function (response) {
            if (response !== null) {
                if (response.status === 200) {
                    //
                    var data = response.data;
                    if (data == null || data == undefined)
                        return;
                    //
                    $('#TblInternationalFeeData input[name="inpFee"]').each(function (indexTbl, itemTbl) {
                        var tblAirlineid = $(itemTbl).data("airlineid");
                        $(data).each(function (index, dataItem) {
                            if (tblAirlineid == dataItem.AirlineID) {
                                var amount = dataItem.FeeAmount;
                                $(itemTbl).val(LibCurrencies.FormatToCurrency(amount));
                            }
                        });
                    });
                    return;
                }
            }
            return;
        },
        error: function (response) {
            console.log(MessageText.NotService);
        }
    });
}
function AgentFeeInlandLoad(_agentId) {
    $('#TblInlandFeeData input[name="inpFee"]').each(function (indexTbl, itemTbl) {
        $(itemTbl).val(0);
    });
    var model = {
        ItineraryID: 1,
        AgentID: _agentId
    };
    AjaxFrom.POST({
        url: URLC + '/GetFeeConfig',
        data: model,
        success: function (response) {
            if (response !== null) {
                if (response.status === 200) {
                    //
                    var data = response.data;
                    if (data == null || data == undefined)
                        return;
                    //
                    $('#TblInlandFeeData input[name="inpFee"]').each(function (indexTbl, itemTbl) {
                        var tblAirlineid = $(itemTbl).data("airlineid");
                        $(data).each(function (index, dataItem) {
                            if (tblAirlineid == dataItem.AirlineID) {
                                var amount = dataItem.FeeAmount;
                                $(itemTbl).val(LibCurrencies.FormatToCurrency(amount));
                            }
                        });
                    });
                    return;
                }
            }
            return;
        },
        error: function (response) {
            console.log(MessageText.NotService);
        }
    });
}