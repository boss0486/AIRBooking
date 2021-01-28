var pageIndex = 1;
var URLC = "/Management/AirBook/Action";
var URLA = "/Management/AirBook";
var arrFile = [];
//
var flightBookingController = {
    init: function () {
        flightBookingController.registerEvent();
    },
    registerEvent: function () {
        $(document).ready(function () {
            // $('[data-dateDefault="true"]').val(LibDateTime.Get_ClientDate(lg = 'en'));
        });
        $('#btnSearch').off('click').on('click', function () {
            var flg = true;
            // Flight go
            var ddlDestinationLocation = $('#ddlDestinationLocation').val();
            var ddlOriginLocation = $('#ddlOriginLocation').val();
            //
            if (ddlOriginLocation == "-" || ddlOriginLocation == '') {
                $('#lblOriginLocation').html('Vui lòng chọn nơi đi');
                flg = false;
            }
            else if (ddlOriginLocation == ddlDestinationLocation) {
                $('#lblOriginLocation').html('Nơi đi và nơi đến không được trùng nhau');
                flg = false;
            }
            else {
                $('#lblOriginLocation').html('');
            }
            // Flight to
            if (ddlDestinationLocation == "-" || ddlDestinationLocation == '') {
                $('#lblDestinationLocation').html('Vui lòng chọn nơi đến');
                flg = false;
            }
            else if (ddlOriginLocation == ddlDestinationLocation) {
                $('#lblDestinationLocation').html('Nơi đi và nơi đến không được trùng nhau');
                flg = false;
            }
            else {
                $('#lblDestinationLocation').html('');
            }
            //
            $('#lblDepartureDateTime').html('');
            var dateGo = $("#DepartureDateTime").val();
            if (dateGo == "") {
                $('#lblDepartureDateTime').html('Vui lòng nhập ngày đi');
            }
            //  
            $('#lblReturnDateTime').html('');
            var ddlFlightType = $('#ddlFlightType').val();
            if (parseInt(ddlFlightType) == 2) {
                var returnDateTime = $('#ReturnDateTime').val();

                if (returnDateTime == "") {
                    $('#lblReturnDateTime').html('Vui lòng nhập ngày về');
                    flg = false;
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
    Search: function () {
        //
        var isHasTax = false;
        if ($('input[name="cbxHasTax"]').is(":checked"))
            isHasTax = true;
        //
        var ddlItineraryType = $('#ddlItineraryType').val();
        var ddlAirlineType = $('#ddlAirlineType').val();
        var ddlOriginLocation = $('#ddlOriginLocation').val();
        var ddlDestinationLocation = $('#ddlDestinationLocation').val();
        var departureDateTime = $("#DepartureDateTime").val();
        var returnDateTime = $("#ReturnDateTime").val();
        var ddlFlightType = $('#ddlFlightType').val();
        var adt = parseInt($("#ddlAdt").val());
        var cnn = parseInt($("#ddlChd").val());
        var inf = parseInt($("#ddlInf").val());
        var isRoundTrip = "False";
        if (parseInt(ddlFlightType) == 2) {
            isRoundTrip = "True";
        }

        // set information in title
        $('.flight-go .flight-name').html(ddlOriginLocation + " - " + ddlDestinationLocation);
        $('.flight-go .flight-date').html(departureDateTime);
        $('.flight-go .flight-adt-total').html(adt);
        //
        $('.flight-to .flight-name').html(ddlDestinationLocation + " - " + ddlOriginLocation);
        $('.flight-to .flight-date').html(returnDateTime);
        $('.flight-to .flight-adt-total').html(adt);
        // go
        departureDateTime = LibDateTime.FormatDateForAPI(departureDateTime);
        returnDateTime = LibDateTime.FormatDateForAPI(returnDateTime);
        var modelGo = {
            OriginLocation: ddlOriginLocation,
            DestinationLocation: ddlDestinationLocation,
            DepartureDateTime: departureDateTime,
            ReturnDateTime: returnDateTime,
            ADT: adt,
            CNN: cnn,
            INF: inf,
            IsRoundTrip: isRoundTrip,
            IsHasTax: isHasTax,
            ItineraryType: ddlItineraryType,
            AirlineType: ddlAirlineType,
            TimeZoneLocal: LibDateTime.GetTimeZoneByLocal()
        };
        AjaxFrom.POST({
            url: URLC + '/search',
            data: modelGo,
            success: function (response) {
                if (response == null || response.status == undefined) {
                    Notifization.Error(MessageText.NotService);
                    return;
                }
                if (response.status == 200) {
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
                    var intGo = 1;
                    var intRt = 1;
                    $.each(response.data, function (index, item) {
                        index = index + 1;
                        //var id = item.ID;
                        //if (id.length > 0)
                        //    id = id.trim();
                        //
                        //var rowNum = parseInt(index) + (parseInt(currentPage) - 1) * parseInt(pageSize);
                        var airEquipType = item.AirEquipType;
                        var flightRph = item.RPH;
                        var unit = 'đ';
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
                        //
                        if (fareDetails !== undefined && fareDetails.length > 0) {
                            $.each(fareDetails, function (indexFare, itemFare) {
                                var adtRph = 0;
                                var adtCode = "";
                                var adtAmount = 0;
                                var adtAmountTotal = 0;
                                var cnnRph = 0;
                                var cnnCode = " ";
                                var cnnAmount = 0;
                                var cnnAmountTotal = 0;
                                var infRph = 0;
                                var infCode = " ";
                                var infAmount = 0;
                                var infAmountTotal = 0;
                                var fareItem = itemFare.FareItem;
                                // get fare
                                if (fareItem !== null && fareItem.length > 0) {
                                    $.each(fareItem, function (indexFareItem, itemFareItem) {

                                        if (itemFareItem.PassengerType == "ADT") {
                                            if (adtAmount == 0) {
                                                adtAmount = itemFareItem.FareAmount;
                                                adtAmountTotal = itemFareItem.FareTotal;
                                                adtRph = itemFareItem.RPH;
                                                adtCode = itemFareItem.Code;
                                            }
                                            //
                                            if (adtAmount > itemFareItem.FareAmount) {
                                                adtAmount = itemFareItem.FareAmount;
                                                adtAmountTotal = itemFareItem.FareTotal;
                                                adtRph = itemFareItem.RPH;
                                                adtCode = itemFareItem.Code;
                                            }
                                        }
                                        if (itemFareItem.PassengerType != undefined && itemFareItem.PassengerType == "CNN") {
                                            if (cnnAmount == 0) {
                                                cnnAmount = itemFareItem.FareAmount;
                                                cnnAmountTotal = itemFareItem.FareTotal;
                                                cnnRph = itemFareItem.RPH;
                                                cnnCode = itemFareItem.Code;
                                            }
                                            //
                                            if (cnnAmount > itemFareItem.FareAmount) {
                                                cnnAmount = itemFareItem.FareAmount;
                                                cnnAmountTotal = itemFareItem.FareTotal;
                                                cnnRph = itemFareItem.RPH;
                                                cnnCode = itemFareItem.Code;
                                            }
                                        }
                                        if (itemFareItem.PassengerType != undefined && itemFareItem.PassengerType == "INF") {
                                            if (infAmount == 0) {
                                                infAmount = itemFareItem.FareAmount;
                                                infAmountTotal = itemFareItem.FareTotal;
                                                infRph = itemFareItem.RPH;
                                                infCode = itemFareItem.Code;
                                            }
                                            //
                                            if (infAmount > itemFareItem.FareAmount) {
                                                infAmount = itemFareItem.FareAmount;
                                                infAmountTotal = itemFareItem.FareTotal;
                                                infRph = itemFareItem.RPH;
                                                infCode = itemFareItem.Code;
                                            }
                                        }
                                    });

                                }
                                //
                                // get min fare
                                var strActive = '';
                                if (indexFare == 0) {
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
                                    AdtAmountTotal: adtAmountTotal,
                                    ResBookDesigCode: itemFare.ResBookDesigCode,
                                    Params: strParam
                                });
                                //
                            });
                        }
                        //
                        var _active = "";
                        var special = ``;
                        // 
                        var priceDetails = "";
                        if (arrFare.length > 0) {
                            $.each(arrFare, function (index, item) {
                                var strPrice = "";
                                var rbsc = item.ResBookDesigCode;
                                var active = "";
                                var salePrice = item.AdtAmountTotal;
                                if (!isHasTax)
                                    salePrice = item.AdtAmount;
                                //
                                if (rbsc == special_pesBookDesigCode) {
                                    //
                                    special = `<label class='lbl-special-item' data-Param='${item.Params}' data-FlightNo='${flightNo}' data-AirEquipType='${airEquipType}' data-FlightRph='${flightRph}' data-ResBookDesigCode='${item.ResBookDesigCode}'><span class='resbook'>${item.ResBookDesigCode}:</span><span class='fare' data-fareBasic='${item.AdtAmount}' data-fareTotal='${item.AdtAmountTotal}' >${LibCurrencies.FormatToCurrency(salePrice)}.0 ${unit}</span></label>`
                                    active = "on";
                                }
                                //
                                priceDetails += `<label class='fare-item'  data-Param='${item.Params}' data-FlightNo='${flightNo}' data-AirEquipType='${airEquipType}' data-FlightRph='${flightRph}' data-ResBookDesigCode='${item.ResBookDesigCode}' data-fareBasic='${item.AdtAmount}' data-fareTotal='${item.AdtAmountTotal}' data-AdtAmount='${item.AdtAmount}'><span class='${active} resbook'>${rbsc}</span></label>`;
                            });
                        }
                        if (parseInt(flightType) == 1) {
                            var strCnt = intGo;
                            if (intGo < 10) {
                                strCnt = "0" + intGo;
                            }
                            if (cntGo == true)
                                _active = "active";
                            htmlGo = `
                            <tr data-FlightNumber='${flightNo}' data-AirEquipType=${airEquipType} data-NumberInParty='${numberInParty}' data-DepartureDateTime= '${departureDateTime}' data-ArrivalDateTime= '${arrivalDateTime}'>
                                <td class='td-firm'>${strCnt}. <i class="fa fa-plane" aria-hidden="true"></i> </td>
                                <td class='td-flightNo'><label data-AirEquipType='${airEquipType}'>VN.<span>${airEquipType}</span>-<span data-flightNo='${flightNo}'>${flightNo} </label></td>
                                <td class='td-itinerary'>${originLocation}-${destinationLocation}</td>
                                <td class='td-time'>${departureTime} - ${arrivalTime}</td>
                                <td class='td-price'><label class='lbl-special'>${special}</label><label class='lbl-list'>${priceDetails} </label></td>
                                <td class='td-action ${_active}'><i class="fa fa-check-circle" aria-hidden="true"></i></td>
                            </tr>`;
                            cntGo = false;
                            intGo++;
                            $('tbody#TblGoData').append(htmlGo);
                        }
                        //
                        if (parseInt(ddlFlightType) == 2 && parseInt(flightType) == 2) {
                            var strCnt = intRt;
                            if (intRt < 10) {
                                strCnt = "0" + intRt;
                            }
                            //
                            if (cntReturn == true)
                                _active = "active";
                            //
                            htmlReturn = `
                            <tr data-FlightNumber='${flightNo}' data-AirEquipType=${airEquipType} data-NumberInParty='${numberInParty}' data-DepartureDateTime= '${departureDateTime}' data-ArrivalDateTime= '${arrivalDateTime}'>
                                <td class='td-firm'>${strCnt}. <i class="fa fa-plane" aria-hidden="true"></i></td>
                                <td class='td-flightNo'><label data-AirEquipType='${airEquipType}'>VN.<span>${airEquipType}</span>-<span data-flightNo='${flightNo}'>${flightNo} </label></td>
                                <td class='td-itinerary'>${originLocation}-${destinationLocation}</td>
                                <td class='td-time'>${departureTime} - ${arrivalTime}</td>
                                <td class='td-price'><label class='lbl-special'>${special}</label><label class='lbl-list'>${priceDetails} </label></td>
                                <td class='td-action ${_active}'><i class="fa fa-check-circle" aria-hidden="true"></i></td>
                            </tr>`;
                            cntReturn = false;
                            intRt++;
                            $('tbody#TblReturnData').append(htmlReturn);
                        }
                    });
                    // display
                    $('.flight-wcontent-go').addClass('active');
                    //
                    if (parseInt(ddlFlightType) == 2) {
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
flightBookingController.init();
//Validate
//###################################################################################################################################################

$(document).on('click', '#TblFlightGo td.td-action', function () {
    $("#TblFlightGo td.td-action").removeClass('active');
    $(this).addClass('active');
    // scroll to  div
    var flightReturnInfo = $('table#TblFlightReturn').find('td.td-action.active');
    var ddlFlightType = $("#ddlFlightType").val();
    if (parseInt(ddlFlightType) == 1) {
        HelperPage.GoToByScroll("btnNextToInf");
    }
    else if (parseInt(ddlFlightType) == 2) { 
        if ($(flightReturnInfo) == undefined || $(flightReturnInfo).length == 0)
            HelperPage.GoToByScroll("TblFlightReturn");
        else
            HelperPage.GoToByScroll("btnNextToInf");
    }
    //// remove message erors
    if ($(flightReturnInfo) !== undefined && $(flightReturnInfo).length > 0) {
        $('#InforCustomerError').html('').removeClass("on");
    }
    // update order
    BookingOrder();
});
//
$(document).on('click', '#TblFlightReturn td.td-action', function () {
    $("#TblFlightReturn td.td-action").removeClass('active');
    $(this).addClass('active');
    // scroll to  div
    var flightGoInfo = $('table#TblFlightGo').find('td.td-action.active');
    var ddlFlightType = $("#ddlFlightType").val();
    if (parseInt(ddlFlightType) == 1) {
        HelperPage.GoToByScroll("btnNextToInf");
    }
    else if (parseInt(ddlFlightType) == 2) {
        if ($(flightGoInfo) == undefined || $(flightGoInfo).length == 0)
            HelperPage.GoToByScroll("TblFlightGo");
        else
            HelperPage.GoToByScroll("btnNextToInf");
    }
    //
    if ($(flightGoInfo) !== undefined && $(flightGoInfo).length > 0) {
        $('#InforCustomerError').html('').removeClass("on");
    }
    // update order
    BookingOrder();
});
//
$(document).on('click', '#btnNextToInf', function () {
    $('#InforCustomerError').html('').removeClass("on");
    var flightGoInfo = $('table#TblFlightGo').find('td.td-action.active');
    var flightReturnInfo = $('table#TblFlightReturn').find('td.td-action.active');
    var htmlError = "";
    var ddlFlightType = parseInt($('#ddlFlightType').val());

    var _mgtop = $(window).height() / 4;
    //    
    if (ddlFlightType == 2 && ($(flightGoInfo) == undefined || $(flightGoInfo).length == 0) && ($(flightReturnInfo) == undefined || $(flightReturnInfo).length == 0)) {
        htmlError = "Vui lòng chọn chuyến bay đi và chuyến bay về";
        setTimeout(function () {
            $([document.documentElement, document.body]).animate({
                scrollTop: $("#TblFlightGo").offset().top - _mgtop
            }, 3000);
        }, 1000);
    }
    else if ($(flightGoInfo) == undefined || $(flightGoInfo).length == 0) {
        htmlError = "Vui lòng chọn chuyến bay đi";
        setTimeout(function () {
            $([document.documentElement, document.body]).animate({
                scrollTop: $("#TblFlightGo").offset().top - _mgtop
            }, 3000);
        }, 1000);
    } else if (ddlFlightType == 2 && ($(flightReturnInfo) == undefined || $(flightReturnInfo).length == 0)) {
        htmlError = "Vui lòng chọn chuyến bay về 11" + ddlFlightType;
        setTimeout(function () {
            $([document.documentElement, document.body]).animate({
                scrollTop: $("#TblFlightReturn").offset().top - _mgtop
            }, 3000);
        }, 1000);
    }
    if (htmlError !== "") {
        $('#InforCustomerError').html(htmlError).addClass("on");
        Notifization.Error(htmlError);
        return;
    }
    // update order
    BookingOrder();
    // rediect
    Notifization.Success("Tiến hành đặt vé, xin chờ...");
    Loading.ShowLoading();
    //
    setTimeout(function () {
        location.href = "/management/airbook/booking";
    }, 2000);
});
//
$(document).on('click', '.lbl-list label.fare-item', function () {
    var unit = "đ";
    //
    var adtAmount = $(this).data("faretotal");
    if (!$('input[name="cbxHasTax"]').is(":checked"))
        adtAmount = $(this).data("farebasic");
    //
    var param = $(this).data("param");
    var flightNo = $(this).data("FlightNo");
    var airEquipType = $(this).data("AirEquipType");
    var flightRph = $(this).data("flightrph");
    var resbookdesigcode = $(this).data("resbookdesigcode");
    var fareBasic = $(this).data("farebasic");
    var fareTotal = $(this).data("faretotal");

    var tr = $(this).closest('tr');
    $(tr).find(".fare-item span.resbook").removeClass("on");
    $(this).find("span.resbook").addClass("on");
    //
    var _html = `<label class='lbl-special-item' data-param='${param}' data-FlightNo='${flightNo}' data-AirEquipType='${airEquipType}' data-FlightRph='${flightRph}' data-ResBookDesigCode='${resbookdesigcode}'><span class='resbook'>${resbookdesigcode}:</span><span class='fare' data-fareBasic='${fareBasic}' data-fareTotal='${fareTotal}'>${LibCurrencies.FormatToCurrency(adtAmount)}.0 ${unit}</span></label>`
    $(tr).find("label.lbl-special").html(_html);
});
// 
function BookingOrder() {
    // delete data
    Cookies.DelCookie("FlightOrder");
    // update new
    var flightGoInfo = $('table#TblFlightGo').find('td.td-action.active');
    var flightReturnInfo = $('table#TblFlightReturn').find('td.td-action.active');
    var htmlError = "";
    var ddlItineraryType = parseInt($('#ddlItineraryType').val());
    var ddlAirlineType = $('#ddlAirlineType').val();
    var ddlFlightType = parseInt($('#ddlFlightType').val());
    //
    var adt = parseInt($("#ddlAdt").val());
    var cnn = parseInt($("#ddlChd").val());
    var inf = parseInt($("#ddlInf").val());
    var lFlight = [];
    //

    var ddlOriginLocation = $('#ddlOriginLocation').val();
    var ddlDestinationLocation = $('#ddlDestinationLocation').val();
    var trGo = $(flightGoInfo).closest('tr');
    if ($(trGo) !== undefined && $(trGo).length > 0) {
        var flightNumber = $(trGo).data("flightnumber");
        var airequiptype = $(trGo).data("airequiptype");
        var arrivalDateTime = $(trGo).data("arrivaldatetime");
        var departureDateTime = $(trGo).data("departuredatetime");
        var numberInParty = $(trGo).data("numberinparty");
        //
        var resBookDesigCode = $(trGo).find('td.td-price .lbl-special .lbl-special-item').data("resbookdesigcode");
        var flightrph = $(trGo).find('td.td-price .lbl-special .lbl-special-item').data("flightrph");
        var param = $(trGo).find('td.td-price .lbl-special .lbl-special-item').data("param");
        var currency = "VND";
        lFlight.push({
            "RPH": flightrph,
            "NumberInParty": numberInParty,
            "OriginLocation": ddlOriginLocation,
            "DestinationLocation": ddlDestinationLocation,
            "DepartureDateTime": departureDateTime,
            "ArrivalDateTime": arrivalDateTime,
            "AirEquipType": airequiptype,
            "ResBookDesigCode": resBookDesigCode,
            "FlightNumber": flightNumber,
            "FareBase": param
        });
    }
    //
    var trReturn = $(flightReturnInfo).closest('tr');
    if (ddlFlightType == 2 && $(trReturn) !== undefined && $(trReturn).length > 0) {
        //
        flightNumber = $(trReturn).data("flightnumber");
        airequiptype = $(trReturn).data("airequiptype");
        arrivalDateTime = $(trReturn).data("arrivaldatetime");
        departureDateTime = $(trReturn).data("departuredatetime");
        numberInParty = $(trReturn).data("numberinparty");
        resBookDesigCode = $(trReturn).find('td.td-price .lbl-special .lbl-special-item').data("resbookdesigcode");
        flightrph = $(trReturn).find('td.td-price .lbl-special .lbl-special-item').data("flightrph");
        var param = $(trReturn).find('td.td-price .lbl-special .lbl-special-item').data("param");
        lFlight.push({
            "RPH": flightrph,
            "NumberInParty": numberInParty,
            "OriginLocation": ddlDestinationLocation,
            "DestinationLocation": ddlOriginLocation,
            "DepartureDateTime": departureDateTime,
            "ArrivalDateTime": arrivalDateTime,
            "AirEquipType": airequiptype,
            "ResBookDesigCode": resBookDesigCode,
            "FlightNumber": flightNumber,
            "FareBase": param
        });
    }
    var orderInfo = {
        ADT: adt,
        CNN: cnn,
        INF: inf,
        Flight: lFlight,
        FlightType: ddlFlightType,
        ItineraryType: ddlItineraryType,
        AirlineType: ddlAirlineType,
        TimeZoneLocal: LibDateTime.GetTimeZoneByLocal()
    };
    // create session
    Cookies.SetCookie("FlightOrder", JSON.stringify(orderInfo));
}
// 
//###################################################################################################################################################
$(document).on('blur', '#DepartureDateTime', function () {
    var dateGo = $(this).val();
    $('#lblDepartureDateTime').html('');
    if (dateGo == "") {
        $('#lblDepartureDateTime').html('Vui lòng nhập ngày đi');
    }
});
//
$(document).on('blur', '#ReturnDateTime', function () {
    var dateReturn = $(this).val();
    $('#lblReturnDateTime').html('');
    var ddlFlightType = $('#ddlFlightType').val();
    if (parseInt(ddlFlightType) == 2) {
        if (dateReturn == "") {
            $('#lblReturnDateTime').html('Vui lòng nhập ngày về');
        }
    }
});
//
$(document).on("change", "#ddlOriginLocation", function () {
    var ddlDestinationLocation = $('#ddlDestinationLocation').val();
    var txtCtl = $(this).val();
    $('#lblOriginLocation').html('');
    if (txtCtl == "-" || txtCtl == "") {
        $('#lblOriginLocation').html('Vui lòng chọn nơi đi');
    }
    else if (txtCtl == ddlDestinationLocation && ddlDestinationLocation !== "" && ddlDestinationLocation !== "-") {
        $('#lblOriginLocation').html('Nơi đi và nơi đến không được trùng nhau');
        $('#lblDestinationLocation').html('');
    }
});
//
$(document).on("change", "#ddlDestinationLocation", function () {
    var ddlOriginLocation = $('#ddlOriginLocation').val();
    var txtCtl = $(this).val();
    $('#lblDestinationLocation').html('');
    if (txtCtl == "-" || txtCtl == "") {
        $('#lblDestinationLocation').html('Vui lòng chọn nơi đến');
    }
    else if (ddlOriginLocation == txtCtl && ddlOriginLocation !== "" && ddlOriginLocation !== "-") {
        $('#lblDestinationLocation').html('Nơi đi và nơi đến không được trùng nhau');
        $('#lblOriginLocation').html('');
    }
});
//
$(document).on("change", "#ddlFlightType", function () {
    $('#lblReturnDateTime').html('');
    var txtCtl = $(this).val();
    if (parseInt(txtCtl) == 2) {
        // Flight to
        var DepartureDateTime = $('#DepartureDateTime').val();
        if (DepartureDateTime == "") {
            $('#lblReturnDateTime').html('Vui lòng nhập ngày về');
        }
    }
});
// chage tax checkbox
$(document).on('change', '#cbxHasTax', function () {
    var unit = " đ";
    var fareAmount = 0;
    var cbxHasTax = $('input[name="cbxHasTax"]').is(":checked");

    var dataGo = $('#TblFlightGo').find('.lbl-special-item');
    $.each(dataGo, function (index, item) {
        var fare = $(item).find('.fare');
        if (cbxHasTax)
            fareAmount = $(fare).data("faretotal");
        else
            fareAmount = $(fare).data("farebasic");
        //
        $(fare).html(LibCurrencies.FormatToCurrency(fareAmount) + ".0" + unit);
    });
    var dataReturn = $('#TblReturnData').find('.lbl-special-item');
    $.each(dataReturn, function (index, item) {
        var fare = $(item).find('.fare');
        if (cbxHasTax)
            fareAmount = $(fare).data("faretotal");
        else
            fareAmount = $(fare).data("farebasic");
        //
        $(fare).html(LibCurrencies.FormatToCurrency(fareAmount) + ".0" + unit);
    });
});


