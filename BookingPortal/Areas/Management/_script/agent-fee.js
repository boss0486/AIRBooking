
var pageIndex = 1;
var URLC = "/Management/AirBook/Action";
var URLA = "/Management/AirBook";
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
        $('#btnFeeApply').off('click').on('click', function () {
            var flg = true;
            // Flight go
            var txtFee = $('#txtFee').val();
            var txtAmount = $('#txtAmount').val();

            //
            if (txtAmount === '') {
                $('#lblAmount').html('Không được để trống số tiền nạp');
                flg = false;
            }
            else {
                txtAmount = LibCurrencies.ConvertToCurrency(txtAmount);
                if (!FormatCurrency.test(txtAmount)) {
                    $('#lblAmount').html('Số tiền nạp không hợp lệ');
                    flg = false;
                }
                else if (parseFloat(txtAmount) <= 0) {
                    $('#lblAmount').html('Số tiền nạp phải > 0');
                    flg = false;
                }
                else {
                    $('#lblAmount').html('');
                }
            }

            // submit form
            if (flg) {
                flightBookingController.Search();
            }
            else
                Notifization.Error(MessageText.Datamissing);
        });
    },
    AgentFeeConfig: function () {
        var ddlOriginLocation = $('#ddlOriginLocation').val();
        var ddlDestinationLocation = $('#ddlDestinationLocation').val();
        var departureDateTime = $("#DepartureDateTime").val();
        var returnDateTime = $("#ReturnDateTime").val();
        var ddlFlightType = $('#ddlFlightType').val();
        var adt = parseInt($("#ddlAdt").val());
        var cnn = parseInt($("#ddlChd").val());
        var inf = parseInt($("#ddlInf").val());
        var isRoundTrip = "False";
        if (parseInt(ddlFlightType) === 2) {
            isRoundTrip = "True";
        }

        //
        departureDateTime = LibDateTime.FormatDateForAPI(departureDateTime);
        returnDateTime = LibDateTime.FormatDateForAPI(returnDateTime);
        // set information in title
        $('.flight-go .flight-name').html(ddlOriginLocation + " - " + ddlDestinationLocation);
        $('.flight-go .flight-date').html(departureDateTime);
        $('.flight-go .flight-adt-total').html(adt);
        //
        $('.flight-to .flight-name').html(ddlDestinationLocation + " - " + ddlOriginLocation);
        $('.flight-to .flight-date').html(returnDateTime);
        $('.flight-to .flight-adt-total').html(adt);
        // go
        //var cookiData = {
        //    OriginLocation: ddlOriginLocation,
        //    DestinationLocation: ddlDestinationLocation,
        //    DepartureDateTime: departureDateTime,
        //    ReturnDateTime: returnDateTime,
        //    ADT: adt,
        //    CNN: cnn,
        //    INF: inf,
        //    IsRoundTrip: isRoundTrip
        //};
        //cookiData = `{"OriginLocation":"HAN1","DestinationLocation":"SGN","DepartureDateTime":"06/30/2020","ReturnDateTime":"06/30/2020""ADT": 1,"CNN": 0,"INF":0,"IsRoundTrip":true}`;
        //Cookies.SetCookie("FlightSearch1", JSON.stringify(cookiData));
        //
        var modelGo = {
            OriginLocation: ddlOriginLocation,
            DestinationLocation: ddlDestinationLocation,
            DepartureDateTime: departureDateTime,
            ReturnDateTime: returnDateTime,
            ADT: adt,
            CNN: cnn,
            INF: inf,
            IsRoundTrip: isRoundTrip
        };
        AjaxFrom.POST({
            url: URLC + '/search',
            data: modelGo,
            success: function (response) {
                if (response === null || response.status === undefined) {
                    Notifization.Error(MessageText.NotService);
                    return;
                }
                if (response.status === 200) {
                    $('tbody#TblGoData').html('');
                    $('tbody#TblReturnData').html('');
                    $('#PaginationGo').html('');
                    //FlightList
                    var currentPage = 1;
                    ////var pagination = response.paging;
                    ////if (pagination !== null) {
                    ////    totalPage = pagination.TotalPage;
                    ////    currentPage = pagination.Page;
                    ////    pageSize = pagination.PageSize;
                    ////    pageIndex = pagination.Page;
                    ////}
                    var htmlGo = '';
                    var htmlReturn = '';
                    var cntGo = false;
                    var cntReturn = false;
                    $.each(response.data, function (index, item) {



                        index = index + 1;
                        //var id = item.ID;
                        //if (id.length > 0)
                        //    id = id.trim();
                        //
                        //var rowNum = parseInt(index) + (parseInt(currentPage) - 1) * parseInt(pageSize);
                        var airEquipType = item.AirEquipType;
                        var flightRph = item.RPH;
                        var unit = 'vnd';
                        var flightNo = item.FlightNo;
                        var numberInParty = item.NumberInParty;
                        var flightType = item.FlightType;
                        var originLocation = item.OriginLocation;
                        var destinationLocation = item.DestinationLocation;
                        var departureDateTime = item.DepartureDateTime;
                        var arrivalDateTime = item.ArrivalDateTime;
                        var departureTime = LibDateTime.GetTime(departureDateTime);
                        var arrivalTime = LibDateTime.GetTime(arrivalDateTime);
                        var fareDetails = item.FareDetails;

                        var special_Price = 0;
                        var special_pesBookDesigCode = "";
                        var special_rph = "";
                        var special_code = "";
                        var arrFare = [];
                        if (fareDetails !== undefined && fareDetails.length > 0) {
                            $.each(fareDetails, function (indexFare, itemFare) {
                                var adtRph = 0;
                                var adtCode = "";
                                var adtAmount = 0;

                                var cnnRph = 0;
                                var cnnCode = " ";
                                var cnnAmount = 0;

                                var infRph = 0;
                                var infCode = " ";
                                var infAmount = 0;

                                var fareItem = itemFare.FareItem;
                                // lấy giá 

                                if (fareItem !== null && fareItem.length > 0) {
                                    $.each(fareItem, function (indexFareItem, itemFareItem) {
                                        if (itemFareItem.PassengerType === "ADT") {
                                            if (adtAmount === 0) {
                                                adtAmount = itemFareItem.FareAmount;
                                                adtRph = itemFareItem.RPH;
                                                adtCode = itemFareItem.Code;
                                            }
                                            //
                                            if (adtAmount > itemFareItem.FareAmount) {
                                                adtAmount = itemFareItem.FareAmount;
                                                adtRph = itemFareItem.RPH;
                                                adtCode = itemFareItem.Code;
                                            }
                                        }
                                        if (itemFareItem.PassengerType != undefined && itemFareItem.PassengerType === "CNN") {
                                            if (cnnAmount === 0) {
                                                cnnAmount = itemFareItem.FareAmount;
                                                cnnRph = itemFareItem.RPH;
                                                cnnCode = itemFareItem.Code;
                                            }
                                            //
                                            if (cnnAmount > itemFareItem.FareAmount) {
                                                cnnAmount = itemFareItem.FareAmount;
                                                cnnRph = itemFareItem.RPH;
                                                cnnCode = itemFareItem.Code;
                                            }
                                        }
                                        if (itemFareItem.PassengerType != undefined && itemFareItem.PassengerType === "INF") {
                                            if (infAmount === 0) {
                                                infAmount = itemFareItem.FareAmount;
                                                infRph = itemFareItem.RPH;
                                                infCode = itemFareItem.Code;
                                            }
                                            //
                                            if (infAmount > itemFareItem.FareAmount) {
                                                infAmount = itemFareItem.FareAmount;
                                                infRph = itemFareItem.RPH;
                                                infCode = itemFareItem.Code;
                                            }
                                        }
                                    });

                                }

                                var strActive = '';
                                // gán mặc định cho min     || set lại giá trị min
                                if (indexFare === 0) {
                                    special_rph = adtRph;
                                    special_code = adtCode;
                                    special_Price = adtAmount;
                                    special_pesBookDesigCode = itemFare.ResBookDesigCode;
                                    strActive = special_pesBookDesigCode;
                                }
                                if (special_Price > adtAmount) {
                                    special_rph = adtRph;
                                    special_code = adtCode;
                                    special_Price = adtAmount;
                                    special_pesBookDesigCode = itemFare.ResBookDesigCode;
                                    strActive = special_pesBookDesigCode;
                                }

                                // active hightlight
                                var strRph = ``;
                                var strCode = ``;
                                var strAmount = ``;

                                var strParam = '';


                                if (adt > 0) {
                                    strParam += `ADT:${adtRph},${adtAmount},${adtCode}`;
                                    strRph += `${adtRph}`;
                                    strCode += `${adtCode}`;
                                    strAmount += `${adtAmount}`;
                                }
                                //
                                if (cnn > 0) {
                                    strParam += `##CNN:${cnnRph},${cnnAmount},${cnnCode}`;
                                    strRph += `,${cnnRph}`;
                                    strCode += `,${cnnCode}`;
                                    strAmount += `,${cnnAmount}`;
                                }
                                //
                                if (inf > 0) {
                                    strParam += `##INF:${infRph},${infAmount},${infCode}`;
                                    strRph += `,${infRph}`;
                                    strCode += `,${infCode}`;
                                    strAmount += `,${infAmount}`;
                                }
                                arrFare.push({
                                    RPH: strRph,
                                    Code: strCode,
                                    Fare: strAmount,
                                    AdtAmount: adtAmount,
                                    ResBookDesigCode: itemFare.ResBookDesigCode,
                                    Params: strParam
                                });
                                //
                            });
                        }
                        //
                        var _active = "";
                        var special = `<label name='special-tag'><span data-ResBookDesigCode=''> </span>: <span data-AdtAmount=''>0.0 ${unit}</span></label>`;
                        // 
                        var priceDetails = "";
                        if (arrFare.length > 0) {
                            $.each(arrFare, function (index, item) {
                                var strPrice = "";
                                var rbsc = item.ResBookDesigCode;
                                var active = "";
                                if (rbsc === special_pesBookDesigCode) {
                                    special = `<label class='lbl-special-item' data-Param='${item.Params}' data-FlightNo='${flightNo}' data-AirEquipType='${airEquipType}' data-FlightRph='${flightRph}' data-ResBookDesigCode='${item.ResBookDesigCode}'><span class='resbook'>${item.ResBookDesigCode}:</span><span class='fare'>${LibCurrencies.FormatToCurrency(item.AdtAmount)}.0 ${unit}</span></label>`
                                    active = "on";
                                }
                                //
                                priceDetails += `<label class='fare-item'  data-Param='${item.Params}' data-FlightNo='${flightNo}' data-AirEquipType='${airEquipType}' data-FlightRph='${flightRph}' data-ResBookDesigCode='${item.ResBookDesigCode}' data-AdtAmount='${item.AdtAmount}'><span class='${active} resbook'>${rbsc}</span></label>`;
                            });
                        }
                        if (parseInt(flightType) === 1) {
                            if (cntGo === true)
                                _active = "active";
                            htmlGo = `
                            <tr data-FlightNumber='${flightNo}' data-AirEquipType=${airEquipType} data-NumberInParty='${numberInParty}' data-DepartureDateTime= '${departureDateTime}' data-ArrivalDateTime= '${arrivalDateTime}'>
                                <td class='td-firm'><i class="fa fa-plane" aria-hidden="true"></i> VNA</td>
                                <td class='td-flightNo'><label data-flightNo='${flightNo}'>VN${flightNo} </label> No.<span data-AirEquipType='${airEquipType}'>${airEquipType}</span></td>
                                <td class='td-itinerary'>${originLocation} -> ${destinationLocation}</td>
                                <td class='td-time'>${departureTime} - ${arrivalTime}</td>
                                <td class='td-price'><label class='lbl-special'>${special}</label><label class='lbl-list'>${priceDetails} </label></td>
                                <td class='td-action ${_active}'><i class="fa fa-check-circle" aria-hidden="true"></i></td>
                            </tr>`;
                            cntGo = false;
                            $('tbody#TblGoData').append(htmlGo);
                        }

                        if (parseInt(ddlFlightType) === 2 && parseInt(flightType) === 2) {
                            if (cntReturn === true)
                                _active = "active";
                            htmlReturn = `
                            <tr data-FlightNumber='${flightNo}' data-AirEquipType=${airEquipType} data-NumberInParty='${numberInParty}' data-DepartureDateTime= '${departureDateTime}' data-ArrivalDateTime= '${arrivalDateTime}'>
                                <td class='td-firm'><i class="fa fa-plane" aria-hidden="true"></i> VNA</td>
                                <td class='td-flightNo'><label data-flightNo='${flightNo}'>VN${flightNo} </label> No.<span data-AirEquipType='${airEquipType}'>${airEquipType}</span></td>
                                <td class='td-itinerary'>${originLocation} -> ${destinationLocation}</td>
                                <td class='td-time'>${departureTime} - ${arrivalTime}</td>
                                <td class='td-price'><label class='lbl-special'>${special}</label><label class='lbl-list'>${priceDetails} </label></td>
                                <td class='td-action ${_active}'><i class="fa fa-check-circle" aria-hidden="true"></i></td>
                            </tr>`;
                            cntReturn = false;
                            $('tbody#TblReturnData').append(htmlReturn);

                        }
                    });
                    //$('tbody#TblGoData').html(htmlGo);

                    // display
                    $('.flight-wcontent-go').addClass('active');
                    //
                    if (parseInt(ddlFlightType) === 2) {
                        // $('tbody#TblReturnData').html(htmlReturn);
                        $('.flight-wcontent-return').addClass('active');
                    }
                    //
                    $('#btnDataTab').click();

                    //Loading.HideLoading();
                    //$('#TblFlightGo').DataTable();
                    //if (parseInt(totalPage) > 1) {
                    //    Paging.Pagination("#Pagination", totalPage, currentPage, flightBookingController.DataList);
                    //} 
                    return;
                }
                Notifization.Error(response.message);
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
        else if (parseFloat(txtAmount) <= 0) {
            $('#lblAmount').html('Số tiền nạp phải > 0');
        }
        else {
            $('#lblAmount').html('');
        }
    }
});

