var pageIndex = 1;
var URLC = "/Management/AirBook/Action";
var URLA = "/Management/AirBook";
var arrSearch = null;
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
            $("#InforCustomerError").html("").removeClass("on").addClass("off");
            var flg = true;
            var segmentList = $("#segmentList .segment-item");
            if (segmentList.length == 0) {
                Notifization.Error("Dữ liệu hành trình không hợp lệ");
                flg = false;
                return;
            }
            // call api
            if (flg) {
                var data = $("#segmentList .segment-item.actived");
                if (data == null || data == "") {
                    Notifization.Error("Không tìm thấy dữ liệu");
                    return;
                }
                //
                flightBookingController.Search(1);
                $("#btnDataTab").click();
            }
            else
                Notifization.Error(MessageText.Datamissing);
        });
        $('#btnItineraryAdd').off('click').on('click', function () {
            var flg = true;
            // Flight go
            var ddlDestinationLocation = $('#ddlDestinationLocation').val();
            var ddlOriginLocation = $('#ddlOriginLocation').val();
            var departureDateTime = $("#DepartureDateTime").val();
            var returnDateTime = $('#ReturnDateTime').val();
            var segmentCount = 1;
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
            if (departureDateTime == "") {
                $('#lblDepartureDateTime').html('Vui lòng nhập ngày đi');
            }
            //  
            $('#lblReturnDateTime').html('');
            var ddlFlightType = $('#ddlFlightType').val();
            if (parseInt(ddlFlightType) == 2) {
                if (returnDateTime == "") {
                    $('#lblReturnDateTime').html('Vui lòng nhập ngày về');
                    flg = false;
                }
                //
                segmentCount = 2;
            }
            //
            var adt = parseInt($("#ddlAdt").val());
            var inf = parseInt($("#ddlInf").val());
            var cnn = parseInt($("#ddlCnn").val());
            if (inf > adt) {
                Notifization.Error("Số lượng em bé không hợp lệ");
                flg = false;
            }
            // submit form
            if (flg) {
                // 
                $("#infAdt").data("val", adt).html($("#ddlAdt option:selected").html());
                $("#infCnn").data("val", cnn).html($("#ddlCnn option:selected").html());
                $("#infInf").data("val", inf).html($("#ddlInf option:selected").html());
                $("#infItinerary").html($("#ddlItinerary option:selected").html());
                $("#infAirline").html($("#ddlAirline option:selected").html());
                // 
                var segmentItem = $("#segmentList .segment-item");
                var segmentState = true;
                if (segmentItem.length + segmentCount > 6) {
                    Notifization.Error("Chặng bay giới hạn 1-6 chặng");
                    flg = false;
                    segmentState = false;
                }
                //
                var _index = parseInt(segmentItem.length) + 1;
                if (parseInt(_index) < 10)
                    _index = `0${_index}`;
                //  
                $(segmentItem).each(function (index, item) {
                    var _origin = $(item).find(".location").data("origin");
                    var _destination = $(item).find(".location").data("destination");
                    var dtime = $(item).find(".date").data("dtime");
                    if (`${_origin}${_destination}` == `${ddlOriginLocation}${ddlDestinationLocation}` && (dtime == departureDateTime || dtime == returnDateTime)) {
                        Notifization.Error("Chặng bay đã tồn tại");
                        flg = false;
                        segmentState = false;
                        return false;
                    }
                });
                if (segmentState) {
                    $("#segmentList").append(`<p class="form-label segment-item actived"><span class='btnSegDelete'>&nbsp;<i class="fas fa-times"></i>&nbsp;</span> ${_index}.<label class='location' data-origin ='${ddlOriginLocation}' data-destination ='${ddlDestinationLocation}'>${ddlOriginLocation}-${ddlDestinationLocation}</label>: <span class='date' data-dtime='${departureDateTime}'>${departureDateTime}</span></p>`);
                    if (parseInt(ddlFlightType) == 2) {
                        _index = parseInt(_index) + 1;
                        if (parseInt(_index) < 10)
                            _index = `0${_index}`;
                        $("#segmentList").append(`<p class="form-label segment-item actived"><span class='btnSegDelete'>&nbsp;<i class="fas fa-times"></i>&nbsp;</span> ${_index}.<label class='location' data-origin ='${ddlDestinationLocation}' data-destination ='${ddlOriginLocation}'>${ddlDestinationLocation}-${ddlOriginLocation}</label>: <span class='date' data-dtime='${returnDateTime}'>${returnDateTime}</span></p>`);
                    }
                }
            }
            else
                Notifization.Error(MessageText.Datamissing);
        });
    },
    Search: function (segIndex) {
        if (segIndex == 1)
            $("#TblSegment tbody#tblSegmentData").html(`<tr class=trdefault><td>......</td><td>......</td><td>......</td><td>......</td><td>......</td><td>......</td></tr><tr class=trdefault><td>......</td><td>......</td><td>......</td><td>......</td><td>......</td><td>......</td></tr><tr class=trdefault><td>......</td><td>......</td><td>......</td><td>......</td><td>......</td><td>......</td></tr>`);
        //
        if ($('#segmentList .segment-item.actived').length == 0)
            return;
        //
        var _row = $('#segmentList .segment-item.actived').first();
        $(_row).removeClass("actived");
        var adt = $("#infAdt").data("val");
        var cnn = $("#infCnn").data("val");
        var inf = $("#infInf").data("val");
        var _origin = $(_row).find(".location").data("origin");
        var _destination = $(_row).find(".location").data("destination");
        var dtime = $(_row).find(".date").data("dtime");
        var _itinerary = $("#infItinerary").data("val");
        var _airline = $("#infAirline").data("val");
        var isHasTax = 0; 
        if ($('input[name="cbxHasTax"]').is(":checked"))
            isHasTax = 1; 
        //
        var model = {
            ADT: parseInt(adt),
            CNN: parseInt(cnn),
            INF: parseInt(inf),
            //
            OriginLocation: _origin,
            DestinationLocation: _destination,
            DepartureDateTime: LibDateTime.FormatDateForAPI(dtime),
            //
            IsHasTax: isHasTax,
            ItineraryType: _itinerary,
            AirlineID: _airline,
            TimeZoneLocal: LibDateTime.GetTimeZoneByLocal()
        };
        // 
        AjaxFrom.POST({
            url: URLC + '/search',
            data: model,
            success: function (response) {
                if (response.status == 200) {
                    if (segIndex == 1) {
                        $("#TblSegment tbody#tblSegmentData").html("");
                    }
                    $('#Pagination').html('');
                    //FlightList
                    var currentPage = 1;
                    ////var pagination = response.paging;
                    ////if (pagination !== null) {
                    ////    totalPage = pagination.TotalPage;
                    ////    currentPage = pagination.Page;
                    ////    pageSize = pagination.PageSize;
                    ////    pageIndex = pagination.Page;
                    ////}
                    var segCnt = 1;
                    var segment = response.data;
                    if (segment != undefined || segment != null) {
                        var originLocation = segment.OriginLocation;
                        var destinationLocation = segment.DestinationLocation;
                        var dateOfFlight = segment.DepartureDateTime;
                        //
                        $("tbody#tblSegmentData").append(`<tr data-segment="${segIndex}" class="segIndex-${segIndex}"><td colspan="6"><i class="fa fa-plane" aria-hidden="true"></i> Chặng bay: ${originLocation}-${destinationLocation}, ngày khởi hành: ${dateOfFlight} </td></tr>`);
                        if (segment.Details != undefined && segment.Details != null) {
                            $.each(segment.Details, function (index, item) {
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
                                        var strRph = "";
                                        var strCode = "";
                                        var strAmount = "";
                                        var strParam = "";
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
                                var strCnt = ("0" + index).substr(-2);
                                //
                                $('tbody#tblSegmentData').append(`
                                <tr data-FlightNumber='${flightNo}' data-AirEquipType=${airEquipType} data-NumberInParty='${numberInParty}' data-OriginLocation='${originLocation}' data-DestinationLocation='${destinationLocation}' data-DateOfFlight='${dateOfFlight}' data-DepartureDateTime= '${departureDateTime}' data-ArrivalDateTime= '${arrivalDateTime}'>
                                    <td class='td-firm text-right'>${strCnt}.</td>
                                    <td class='td-flightNo'><label data-AirEquipType='${airEquipType}'>VN.<span>${airEquipType}</span>-<span data-flightNo='${flightNo}'>${flightNo} </label></td>
                                    <td class='td-itinerary'>${originLocation}-${destinationLocation}</td>
                                    <td class='td-time'>${departureTime} - ${arrivalTime}</td>
                                    <td class='td-price'><label class='lbl-special'>${special}</label><label class='lbl-list'>${priceDetails} </label></td>
                                    <td class='td-action actsegment-${segIndex} ${_active}' data-segIndex='${segIndex}'><i class="fa fa-check-circle" aria-hidden="true"></i></td>
                                </tr>`);
                            });
                        }
                        segIndex++;
                    }

                    // display
                    $('.flight-wcontent-go').addClass('active');
                    //Loading.HideLoading();
                    //$('#TblSegment').DataTable();
                    //if (parseInt(totalPage) > 1) {
                    //    Paging.Pagination("#Pagination", totalPage, currentPage, flightBookingController.DataList);
                    //} 
                    $(_row).removeClass("actived");
                    flightBookingController.Search(segIndex);
                    return;
                }
                else if (response.status == 503) {
                    Notifization.Error(response.message);
                    return;
                }
                else {
                    $(_row).removeClass("actived");
                    flightBookingController.Search(segIndex);
                }
            },
            error: function (response) {
                console.log('::' + MessageText.NotService);
            }
        });
    },
    BookingOrder: function () {
        // update new
        var segmentList = [];
        var ddlItineraryType = parseInt($('#ddlItineraryType').val());
        var ddlAirline = $('#ddlAirline').val();
        //
        var adt = parseInt($("#infAdt").data("val"));
        var cnn = parseInt($("#infCnn").data("val"));
        var inf = parseInt($("#infInf").data("val"));
        //
        $("table#TblSegment td.td-action.active").each(function (index, item) {
            //
            var segment = $(item).closest("tr");
            if ($(segment) !== undefined && $(segment).length > 0) {
                var flightNumber = $(segment).data("flightnumber");
                var airequiptype = $(segment).data("airequiptype");
                var numberInParty = $(segment).data("numberinparty");
                var dateOfFlight = $(segment).data("dateofflight");
                var ddlOriginLocation = $(segment).data("originlocation");
                var ddlDestinationLocation = $(segment).data("destinationlocation");
                var arrivalDateTime = $(segment).data("arrivaldatetime");
                var departureDateTime = $(segment).data("departuredatetime");
                //
                var resBookDesigCode = $(segment).find('td.td-price .lbl-special .lbl-special-item').data("resbookdesigcode");
                var flightrph = $(segment).find('td.td-price .lbl-special .lbl-special-item').data("flightrph");
                var param = $(segment).find('td.td-price .lbl-special .lbl-special-item').data("param");
                segmentList.push({
                    RPH: flightrph,
                    NumberInParty: numberInParty,
                    OriginLocation: ddlOriginLocation,
                    DestinationLocation: ddlDestinationLocation,
                    DepartureDateTime: LibDateTime.FormatDateForAPI(departureDateTime) ,
                    ArrivalDateTime: LibDateTime.FormatDateForAPI(arrivalDateTime),
                    AirEquipType: airequiptype,
                    ResBookDesigCode: resBookDesigCode,
                    FlightNumber: flightNumber,
                    FareBase: param
                });
            }
        });
        //
        var model = {
            ADT: adt,
            CNN: cnn,
            INF: inf,
            ItineraryType: ddlItineraryType,
            AirlineID: ddlAirline,
            Segments: segmentList,
            TimeZoneLocal: LibDateTime.GetTimeZoneByLocal()
        }
        //
        AjaxFrom.POST({
            url: URLC + '/OrderTemp',
            data: model,
            success: function (response) {
                if (response.status == 200) {
                    // rediect 
                    Loading.ShowLoading();
                    //
                    setTimeout(function () {
                        location.href = "/management/airbook/booking";
                    }, 2000);
                }
                else if (response.status == 503) {
                    Notifization.Error(response.message);
                    return;
                }
            },
            error: function (response) {
                console.log('::' + MessageText.NotService);
            }
        });
    },
    ShopingCart: function () {
        $("#tblCartData").html("");
        $('table#TblSegment td.td-action.active').each(function (index, item) {
            var rowData = $(item).closest("tr").html();
            if (rowData != undefined || rowData != null) {
                $("#tblCartData").append(`<tr>${rowData}</tr>`);
            }
        });
    },
};
//
flightBookingController.init();
$(document).on("click", ".btnSegDelete", function () {
    var item = $(this).closest("p");
    $(item).remove();
});


//Validate
//###################################################################################################################################################
$(document).on('click', '#TblSegment td.td-action', function () {
    var segIndex = parseInt($(this).data("segindex"));
    if ($(this).hasClass("active")) {
        $(this).removeClass("active");
    } else {
        $(`#TblSegment td.td-action.actsegment-${segIndex}`).removeClass('active');
        $(this).addClass('active');
    }
    if ($(`#TblSegment tr[data-segment]`).hasClass(`segIndex-${segIndex + 1}`))
        HelperPage.ScrollToElement(`.segIndex-${segIndex + 1}`);
    else
        HelperPage.ScrollToElement(`#btnNextToInf`);
    // ******************************************************
    flightBookingController.ShopingCart();
});
//
$(document).on('click', '#btnNextToInf', function () {
    $("#InforCustomerError").html("").removeClass("on");
    $("#btnCart").click();
    var flightGoInfo = $("table#TblSegment").find('td.td-action.active');
    var htmlError = "";
    if ($(flightGoInfo) == undefined || $(flightGoInfo).length == 0) {
        htmlError = "Vui lòng chọn chặng bay"
        $("#InforCustomerError").html(htmlError).addClass("on");
        Notifization.Error(htmlError);
        return;
    }

    // update order
    flightBookingController.BookingOrder();
    Notifization.Success("Tiến hành đặt vé, xin chờ...");
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

// 
//###################################################################################################################################################
$(document).on('change', '#DepartureDateTime', function () {
    var dateGo = $(this).val();
    $('#lblDepartureDateTime').html('');
    if (dateGo == "") {
        $('#lblDepartureDateTime').html('Vui lòng nhập ngày đi');
    }
    else if (!ValidData.ValidDate(dateGo, "vn")) {
        $('#lblDepartureDateTime').html('Ngày đi không hợp lệ');

    }
});
//
$(document).on('change', '#ReturnDateTime', function () {
    var dateReturn = $(this).val();
    $('#lblReturnDateTime').html('');
    var ddlFlightType = $('#ddlFlightType').val();
    if (parseInt(ddlFlightType) == 2) {
        if (dateReturn == "") {
            $('#lblReturnDateTime').html('Vui lòng nhập ngày về');
        }
        else if (!ValidData.ValidDate(dateReturn, "vn")) {
            $('#lblReturnDateTime').html('Ngày về không hợp lệ');

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
        var departureDateTime = $('#DepartureDateTime').val();
        if (departureDateTime == "") {
            $('#lblReturnDateTime').html('Vui lòng nhập ngày về');
        }
    }
    else {
        $('#lblReturnDateTime').html('');
        $('#ReturnDateTime').val('');
    }
});
// chage tax checkbox
$(document).on('change', '#cbxHasTax', function () {
    var unit = " đ";
    var fareAmount = 0;
    var cbxHasTax = $('input[name="cbxHasTax"]').is(":checked");

    var dataGo = $('#TblSegment').find('.lbl-special-item');
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
//
$(document).on('click', '#btnSearchReset', function () {
    $("#infAdt").data("val", 0).html("00");
    $("#infCnn").data("val", 0).html("00");
    $("#infInf").data("val", 0).html("00");
    $("#infItinerary").html("......");
    $("#infAirline").html("......");
    $("#segmentList").html('');
    FData.ResetForm();
    Cookies.DelCookie("FlightSearch");
});

$(document).on('click', '#btnSearchTab', function () {
    $("#segmentList .segment-item").addClass("actived");
});

$(document).on('click', '#btnCart', function () {

});


