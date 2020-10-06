
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
            if (ddlOriginLocation === "-" || ddlOriginLocation === '') {
                $('#lblOriginLocation').html('Vui lòng chọn nơi đi');
                flg = false;
            }
            else if (ddlOriginLocation === ddlDestinationLocation) {
                $('#lblOriginLocation').html('Nơi đi và nơi đến không được trùng nhau');
                flg = false;
            }
            else {
                $('#lblOriginLocation').html('');
            }
            // Flight to
            if (ddlDestinationLocation === "-" || ddlDestinationLocation === '') {
                $('#lblDestinationLocation').html('Vui lòng chọn nơi đến');
                flg = false;
            }
            else if (ddlOriginLocation === ddlDestinationLocation) {
                $('#lblDestinationLocation').html('Nơi đi và nơi đến không được trùng nhau');
                flg = false;
            }
            else {
                $('#lblDestinationLocation').html('');
            }
            //
            $('#lblDepartureDateTime').html('');
            var dateGo = $("#DepartureDateTime").val();
            if (dateGo === "") {
                $('#lblDepartureDateTime').html('Vui lòng nhập ngày đi');
            }
            //  
            $('#lblReturnDateTime').html('');
            var ddlFlightType = $('#ddlFlightType').val();
            if (parseInt(ddlFlightType) === 2) {
                var returnDateTime = $('#ReturnDateTime').val();

                if (returnDateTime === "") {
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
            IsHasTax: isHasTax
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

                        var fareItemCount = ":";
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

                                fareItemCount += itemFare.ResBookDesigCode + ":" + fareItem.length + " ";
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

                        console.log("::" + fareItemCount);
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
                                <td class='td-flightNo'><label data-AirEquipType='${airEquipType}'>VN.<span>${airEquipType}</span> / <span data-flightNo='${flightNo}'>${flightNo} </label></td>
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
                                <td class='td-flightNo'><label data-AirEquipType='${airEquipType}'>VN.<span>${airEquipType}</span> / <span data-flightNo='${flightNo}'>${flightNo} </label></td>
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
flightBookingController.init();
//Validate
//###################################################################################################################################################

$(document).on('click', '#TblFlightGo td.td-action', function () {
    $("#TblFlightGo td.td-action").removeClass('active');
    $(this).addClass('active');
    // scroll to  div
    var flightGoInfo = $('table#TblFlightGo').find('td.td-action.active');
    var flightReturnInfo = $('table#TblFlightReturn').find('td.td-action.active');
    var _mgtop = $(window).height() / 4;
    if ($(flightReturnInfo) === undefined || $(flightReturnInfo).length === 0) {
        $([document.documentElement, document.body]).animate({
            scrollTop: $("#TblFlightReturn").offset().top - _mgtop
        }, 2000);
    }
    else {
        $([document.documentElement, document.body]).animate({
            scrollTop: $("#btnNextToInf").offset().top - _mgtop
        }, 2000);
    }
    // remove message erors
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
    var flightReturnInfo = $('table#TblFlightReturn').find('td.td-action.active');
    var _mgtop = $(window).height() / 4;
    if ($(flightGoInfo) === undefined || $(flightGoInfo).length === 0) {
        $([document.documentElement, document.body]).animate({
            scrollTop: $("#TblFlightGo").offset().top - _mgtop
        }, 2000);
    }
    else {
        $([document.documentElement, document.body]).animate({
            scrollTop: $("#btnNextToInf").offset().top - _mgtop
        }, 2000);
    }
    // remove message erors
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
    if (ddlFlightType === 2 && ($(flightGoInfo) === undefined || $(flightGoInfo).length === 0) && ($(flightReturnInfo) === undefined || $(flightReturnInfo).length === 0)) {
        htmlError = "Vui lòng chọn chuyến bay đi và chuyến bay về";
        setTimeout(function () {
            $([document.documentElement, document.body]).animate({
                scrollTop: $("#TblFlightGo").offset().top - _mgtop
            }, 3000);
        }, 1000);
    }
    else if ($(flightGoInfo) === undefined || $(flightGoInfo).length === 0) {
        htmlError = "Vui lòng chọn chuyến bay đi";
        setTimeout(function () {
            $([document.documentElement, document.body]).animate({
                scrollTop: $("#TblFlightGo").offset().top - _mgtop
            }, 3000);
        }, 1000);
    } else if (ddlFlightType === 2 && $(flightReturnInfo) === undefined || $(flightReturnInfo).length === 0) {
        htmlError = "Vui lòng chọn chuyến bay về";
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
    setTimeout(function () {
        location.href = "/management/airbook/booking";
    }, 2000);
});
//
$(document).on('click', '.lbl-list label.fare-item', function () {
    var unit = "vnd";

    var adtAmount = $(this).data("adtamount");
    var param = $(this).data("param");
    var flightNo = $(this).data("FlightNo");
    var airEquipType = $(this).data("AirEquipType");
    var flightRph = $(this).data("flightrph");
    var resbookdesigcode = $(this).data("resbookdesigcode");

    var tr = $(this).closest('tr');
    $(tr).find(".fare-item span.resbook").removeClass("on");
    $(this).find("span.resbook").addClass("on");

    var _html = `<label class='lbl-special-item' data-param='${param}' data-FlightNo='${flightNo}' data-AirEquipType='${airEquipType}' data-FlightRph='${flightRph}' data-ResBookDesigCode='${resbookdesigcode}'><span class='resbook'>${resbookdesigcode}:</span><span class='fare'>${LibCurrencies.FormatToCurrency(adtAmount)}.0 ${unit}</span></label>`
    $(tr).find("label.lbl-special").html(_html);
});
//
$(document).on('click', '#btnBooking', function () {
    var flg = true;
    // valid full name
    $("#BookingForm input[name='txtFullName']").each(function (index, item) {
        var _wtrl = $(this).closest("div");
        if ($(this).val() === "") {
            $(_wtrl).find("span[name='lblFullName']").html("Không được để trống họ tên");
            flg = false;
        }
        else if ($(this).val().length < 5 || $(this).val().length > 30) {
            $(_wtrl).find("span[name='lblFullName']").html("Họ tên giới hạn [5-30] ký tự");
            flg = false;
        }
        else if (!ValidData.ValidName($(this).val(), "vn")) {
            $(_wtrl).find("span[name='lblFullName']").html("Họ tên không hợp lệ");
            flg = false;
        }
        else {
            $(_wtrl).find("span[name='lblFullName']").html("");
        }
    });
    // valid birthday
    $("#BookingForm input[name='txtBirthDay']").each(function (index, item) {
        var _wtrl = $(this).closest("div");
        if ($(this).val() === "") {
            $(_wtrl).find("span[name='lblBirthDay']").html("Không được để trống ngày sinh");
            flg = false;
        }
        else if (!ValidData.ValidDate($(this).val(), "vn")) {
            $(_wtrl).find("span[name='lblBirthDay']").html("Ngày sinh không hợp lệ, format dd-mm-yyyy");
            flg = false;
        }
        else {
            $(_wtrl).find("span[name='lblBirthDay']").html("");
        }
    });

    // FOR CONTACT ************************************************************************************************
    // valid name of passengers
    var name = $("#txtName").val();
    $("#lblName").html("");
    if (name === "") {
        $("#lblName").html("Không được để trống họ tên liên hệ");
        flg = false;
    }
    // valid phone number
    var phone = $("#txtPhone").val();
    if (phone === "") {
        $("#lblPhone").html("Không được để trống số điện thoại");
        flg = false;
    }
    else if (!ValidData.ValidPhoneNumber(phone, "vn")) {
        $("#lblPhone").html("Số điện thoại không hợp lệ");
        flg = false;
    }
    else {
        $("#lblPhone").html("");
    }
    // valid email address
    var email = $("#txtEmail").val();
    $("#lblEmail").html("");
    if (email !== "" && !ValidData.ValidEmail(email)) {
        $("#lblEmail").html("Địa chỉ email không hợp lệ");
        flg = false;
    }
    // 
    if (!flg) {
        Notifization.Error(MessageText.NotService + 1);
        return;
    }
    var lPassenger = [];
    $("#BookingForm [data-PassengerType]").each(function (index, item) {
        var type = $(item).data("passengertype");
        var passenger = $(item).find("[data-passengerinf]");
        if (passenger !== undefined && passenger.length > 0) {
            $.each(passenger, function (index1, item1) {
                var fullName = $(item1).find("input[name='txtFullName']").val();
                var gender = $(item1).find("select[name='ddlGender']").val();
                var birthDay = $(item1).find("input[name='txtBirthDay']").val();
                //
                lPassenger.push({
                    PassengerType: type,
                    FullName: fullName,
                    Gender: gender,
                    Phone: phone,
                    DateOfBirth: LibDateTime.FormatDateForAPI(birthDay)
                });
            });
        }
    });
    if (lPassenger.length === 0) {
        Notifization.Error("Dữ liệu không hợp lệ");
        return;
    }
    // get flight data
    var cookiData = Cookies.GetCookie("FlightOrder");
    if (cookiData === undefined || cookiData === "") {
        Notifization.Error("Dữ liệu không hợp lệ");
        return;
    }
    var order = JSON.parse(cookiData);

    if (order == null) {
        Notifization.Error("Dữ liệu không hợp lệ");
        return;
    }
    var totalItineraryPrice = 0;
    var lFlight = [];
    var _type = parseInt(order.FlightType);
    // go data 
    var orderGo = order.Flight[0];
    var _arrivalDateTime = orderGo.ArrivalDateTime;
    var _departureDateTime = orderGo.DepartureDateTime;
    var _flightNumber = orderGo.FlightNumber;
    var _numberInParty = orderGo.NumberInParty;
    var _resBookDesigCode = orderGo.ResBookDesigCode;
    var _airEquipType = orderGo.AirEquipType;
    var _destinationLocation = orderGo.DestinationLocation;
    var _originLocation = orderGo.OriginLocation;
    //
    _departureDateTime = LibDateTime.FormatDateForAPI(_departureDateTime);
    _arrivalDateTime = LibDateTime.FormatDateForAPI(_arrivalDateTime);
    // add data for go
    lFlight.push({
        DepartureDateTime: _departureDateTime,
        ArrivalDateTime: _arrivalDateTime,
        FlightNumber: _flightNumber,
        NumberInParty: _numberInParty,
        ResBookDesigCode: _resBookDesigCode,
        AirEquipType: _airEquipType,
        DestinationLocation: _destinationLocation,
        OriginLocation: _originLocation
    });
    // return data
    if (_type === 2) {
        orderReturn = order.Flight[1];
        _arrivalDateTime = orderReturn.ArrivalDateTime;
        _departureDateTime = orderReturn.DepartureDateTime;
        _flightNumber = orderReturn.FlightNumber;
        _numberInParty = orderReturn.NumberInParty;
        _resBookDesigCode = orderReturn.ResBookDesigCode;
        _airEquipType = orderReturn.AirEquipType;
        _destinationLocation = orderReturn.DestinationLocation;
        _originLocation = orderReturn.OriginLocation;
        //
        _departureDateTime = LibDateTime.FormatDateForAPI(_departureDateTime);
        _arrivalDateTime = LibDateTime.FormatDateForAPI(_arrivalDateTime);
        // add data
        lFlight.push({
            DepartureDateTime: _departureDateTime,
            ArrivalDateTime: _arrivalDateTime,
            FlightNumber: _flightNumber,
            NumberInParty: _numberInParty,
            ResBookDesigCode: _resBookDesigCode,
            AirEquipType: _airEquipType,
            DestinationLocation: _destinationLocation,
            OriginLocation: _originLocation
        });
    }

    // mode

    //  copntact of passengers
    var lContact = [];
    var contact = {
        Name: name,
        Email: email,
        Phone: phone
    };
    lContact.push(contact);
    var bookModel = {
        Contacts: lContact,
        Passengers: lPassenger,
        Flights: lFlight
    };
    console.log(JSON.stringify(bookModel));
    // call api
    AjaxFrom.POST({
        url: URLC + '/book',
        data: bookModel,
        success: function (response) {
            if (response === null || response.status === undefined) {
                Notifization.Error(MessageText.message);
                return;
            }
            if (response.status === 200) {
                Notifization.Success(response.message);
                //location.href = response.data;
                return;
            }
            Notifization.Error(response.message);
            return;
        },
        error: function (response) {
            console.log('::' + MessageText.NotService);
        }
    });

});

$(document).on('click', '#btnRelease', function () {

    console.log('::ok');
    var pnrCode = $('#lblPNRCode').data("pnr");
    if (pnrCode === undefined || pnrCode === "") {
        Notifization.Error("Không thể xuất vé với PNR này.");
        return;
    }
    AjaxFrom.POST({
        url: URLC + '/ExTicket',
        data: { PNR: pnrCode },
        success: function (response) {
            if (response === null || response.status === undefined) {
                Notifization.Error(MessageText.NotService);
                return;
            }
            if (response.status === 200) {
                Notifization.Success(response.message);
                //location.href = response.data;
                return;
            }
            Notifization.Error(MessageText.NotService);
            return;
        },
        error: function (response) {
            console.log('::' + MessageText.NotService);
        }
    });
});

//
function BookingOrder() {
    // delete data
    Cookies.DelCookie("FlightOrder");
    // update new
    var flightGoInfo = $('table#TblFlightGo').find('td.td-action.active');
    var flightReturnInfo = $('table#TblFlightReturn').find('td.td-action.active');
    var htmlError = "";
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
    if (ddlFlightType === 2 && $(trReturn) !== undefined && $(trReturn).length > 0) {
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
        FlightType: ddlFlightType
    };
    // create session
    Cookies.SetCookie("FlightOrder", JSON.stringify(orderInfo));
}
//
function BookingOrderLoad() {
    var cookiData = Cookies.GetCookie("FlightOrder");
    if (cookiData !== undefined && cookiData !== "") {
        var order = JSON.parse(cookiData);
        if (order !== null) {
            var totalItineraryPrice = 0;
            var lFlight = [];
            // go data
            var _adt = parseInt(order.ADT);
            var _cnn = parseInt(order.CNN);
            var _inf = parseInt(order.INF);
            var _type = parseInt(order.FlightType);
            //
            var orderGo = order.Flight[0];
            var _rphFlight = orderGo.RPH;
            var _arrivalDateTime = orderGo.ArrivalDateTime;
            var _departureDateTime = orderGo.DepartureDateTime;
            var _flightNumber = orderGo.FlightNumber;
            var _numberInParty = orderGo.NumberInParty;
            var _resBookDesigCode = orderGo.ResBookDesigCode;
            var _airEquipType = orderGo.AirEquipType;
            var _destinationLocation = orderGo.DestinationLocation;
            var _originLocation = orderGo.OriginLocation;
            let _fareBaseGo = orderGo.FareBase;
            var _fareBaseCode = orderGo.FareBasisCode;
            departureDateTime = LibDateTime.FormatDateForAPI(_departureDateTime);
            //returnDateTime = LibDateTime.FormatDateForAPI(returnDateTime);
            var departureTime = LibDateTime.GetTime(_departureDateTime);
            var arrivalTime = LibDateTime.GetTime(_arrivalDateTime);
            // show data
            //var trGo = $('#TblRowGo');
            //$(trGo).find("td .lblTrademark").html("VNA");
            //$(trGo).find("td .lblFlightNo").html(`VN${_flightNumber}`);
            //$(trGo).find("td .lblItinerary").html(`<span class='lblItinerary-location'>${_originLocation} - ${_destinationLocation}</span>, <span class='lblItinerary-date'>${departureDateTime}</span>`);
            //$(trGo).find("td .lblItineraryTime").html(`${departureTime} - ${arrivalTime}`);
            //$(trGo).find("td .lblResbookdesigcode").html(`${_resBookDesigCode}`);
            // get feeBase

            var labelTbl = [];
            if (_adt > 0) {
                labelTbl.push({
                    PassengerType: "ADT",
                    Quantity: _adt
                }); // true alway
            }
            if (_cnn > 0) {
                labelTbl.push({
                    PassengerType: "CNN",
                    Quantity: _cnn
                });
            }
            if (_inf > 0) {
                labelTbl.push({
                    PassengerType: "INF",
                    Quantity: _inf
                });
            }
            //
            var arrPassengerBase = [];
            if (_fareBaseGo.includes("##")) {
                var arrParam = _fareBaseGo.split("##");
                if (arrParam.length > 0) {
                    $.each(arrParam, function (index, item) {
                        var passengerType = '';
                        var quantity = 0;
                        var amount = 0;
                        var rph = 0;
                        var code = 0;
                        if (item.includes(":")) {
                            var passengerType = item.split(":")[0];
                            var passengerFare = item.split(":")[1];
                            var passengerFareInfo = passengerFare.split(",");
                            if (passengerType === "ADT")
                                quantity = _adt;
                            if (passengerType === "CNN")
                                quantity = _cnn;
                            if (passengerType === "INF")
                                quantity = _inf;
                            //
                            rph = parseFloat(passengerFareInfo[0]);
                            amount = parseFloat(passengerFareInfo[1]);
                            code = passengerFareInfo[2];
                        }
                        arrPassengerBase.push({
                            RPH: rph,
                            PassengerType: passengerType,
                            Quantity: quantity,
                            Amount: amount,
                            FareBaseCode: code
                        });

                    });
                }
            }
            //
            var feeTaxModel = [];
            var feeBaseGo = {
                RPH: _rphFlight,
                OriginLocation: _originLocation,
                DestinationLocation: _destinationLocation,
                DepartureDateTime: LibDateTime.FormatDateForAPI(_departureDateTime),
                ArrivalDateTime: LibDateTime.FormatDateForAPI(_arrivalDateTime),
                AirEquipType: _airEquipType,
                FareBase: arrPassengerBase,
                ResBookDesigCode: _resBookDesigCode,
                FlightNumber: _flightNumber,
                CurrencyCode: "VND"
            };
            // add array
            feeTaxModel.push(feeBaseGo);
            //
            if (_type === 2) {
                var orderReturn = order.Flight[1];
                _rphFlight = orderReturn.RPH;
                _arrivalDateTime = orderReturn.ArrivalDateTime;
                _departureDateTime = orderReturn.DepartureDateTime;
                _flightNumber = orderReturn.FlightNumber;
                _numberInParty = orderReturn.NumberInParty;
                _resBookDesigCode = orderReturn.ResBookDesigCode;
                _airEquipType = orderReturn.AirEquipType;
                _destinationLocation = orderReturn.OriginLocation;
                _originLocation = orderReturn.DestinationLocation;
                var _fareBaseReturn = orderReturn.FareBase;
                _test = orderReturn.FareBase;
                arrPassengerBase = [];
                if (_fareBaseReturn.includes("##")) {
                    var arrParam = _fareBaseReturn.split("##");
                    if (arrParam.length > 0) {
                        $.each(arrParam, function (index, item) {
                            var passengerType = '';
                            var quantity = 0;
                            var amount = 0;
                            var rph = 0;
                            var code = 0;
                            if (item.includes(":")) {
                                var passengerType = item.split(":")[0].trim();
                                var passengerFare = item.split(":")[1].trim();
                                var passengerFareInfo = passengerFare.split(",");
                                if (passengerType === "ADT")
                                    quantity = _adt;
                                if (passengerType === "CNN")
                                    quantity = _cnn;
                                if (passengerType === "INF")
                                    quantity = _inf;
                                //
                                rph = parseFloat(passengerFareInfo[0]);
                                amount = parseFloat(passengerFareInfo[1]);
                                code = passengerFareInfo[2];
                            }
                            arrPassengerBase.push({
                                RPH: rph,
                                PassengerType: passengerType,
                                Quantity: quantity,
                                Amount: amount,
                                FareBaseCode: code
                            });

                        });
                    }
                }
                //
                var feeBaseReturn = {
                    RPH: _rphFlight,
                    OriginLocation: _destinationLocation,
                    DestinationLocation: _originLocation,
                    DepartureDateTime: LibDateTime.FormatDateForAPI(_departureDateTime),
                    ArrivalDateTime: LibDateTime.FormatDateForAPI(_arrivalDateTime),
                    AirEquipType: _airEquipType,
                    FareBase: arrPassengerBase,
                    ResBookDesigCode: _resBookDesigCode,
                    FlightNumber: _flightNumber,
                    CurrencyCode: "VND"
                };
                feeTaxModel.push(feeBaseReturn);
            }

            console.log(":Tax:" + JSON.stringify(feeTaxModel));

            AjaxFrom.POST({
                url: URLC + '/GetFeeBasic',
                data: { models: feeTaxModel },
                success: function (response) {

                    console.log(JSON.stringify(response));

                    if (response === null || response.status === undefined) {
                        Notifization.Error(MessageText.NotService);
                        return;
                    }
                    if (response.status === 200) {
                        var _rowPrice = "";
                        $("#TblGoPriceInfo").html(`<tr><td>Đang cập nhật...</td></tr>`);
                        var fareAmount = _fareBaseGo.Amount;
                        var _fareAllTotal = 0;

                        // flight 
                        var _fareFlight = 0;

                        $("#TblFlight tbody").html('');
                        $.each(feeTaxModel, function (index, item) {
                            var mdRph = parseInt(item.RPH);
                            var mdAirEquipType = parseInt(item.AirEquipType);
                            var mdFlightNumber = parseInt(item.FlightNumber);

                            var mdArrivalDateTime = item.ArrivalDateTime;
                            var mdDepartureDateTime = item.DepartureDateTime;
                            var mdNumberInParty = item.NumberInParty;
                            var mdResBookDesigCode = item.ResBookDesigCode;
                            var mdDestinationLocation = item.DestinationLocation;
                            var mdOriginLocation = item.OriginLocation;
                            var mdFareBaseCode = item.FareBasisCode;
                            var mdFareBase = item.FareBase;
                            var mdDepartureDate = LibDateTime.GetDate(mdDepartureDateTime);
                            var mdArrivalDate = LibDateTime.GetDate(mdArrivalDateTime);

                            var mdDepartureTime = LibDateTime.GetTime(mdDepartureDateTime);
                            var mdArrivalTime = LibDateTime.GetTime(mdArrivalDateTime);




                            console.log("_flightAmount:" + JSON.stringify(item));

                            // tax for flight   
                            var fareRound = 0;
                            var _taxRow = '<tr><td>Đang cập nhật..1.</td></tr>';
                            if (response.data.length > 0) {
                                _taxRow = '';

                                $.each(response.data, function (indexFlight, itemFilght) {
                                    if (parseInt(itemFilght.RPH) === mdRph && itemFilght.AirEquipType === mdAirEquipType && itemFilght.FlightNumber === mdFlightNumber) {
                                        var flightTaxInfos = itemFilght.FlightTaxInfos;
                                        if (flightTaxInfos.length > 0) {


                                            $.each(flightTaxInfos, function (indexTax, itemTax) {
                                                //
                                                var _quantities = 0;
                                                var passengerType = itemTax.PassengerType.Code;


                                                var taxOfType = itemTax.Total;
                                                var feeTax = 0;
                                                var flightAmount = 0;
                                                $.each(mdFareBase, function (indexFareBase, itemFareBase) {
                                                    if (itemFareBase.PassengerType === passengerType) {
                                                        flightAmount = itemFareBase.Amount;
                                                        return;
                                                    }
                                                });
                                                if (passengerType === "ADT") {
                                                    _quantities = _adt;
                                                }
                                                //
                                                if (passengerType === "CNN")
                                                    _quantities = _cnn;
                                                //
                                                if (passengerType === "INF") {
                                                    _quantities = _inf;
                                                }
                                                //
                                                var flightTotal = 0;
                                                var fareOfType = 0;
                                                if (parseFloat(taxOfType) > 0 && parseFloat(_quantities) > 0) {
                                                    fareOfType = flightAmount + taxOfType;

                                                    flightTotal = fareOfType * parseFloat(_quantities);
                                                    fareRound += flightTotal;
                                                }
                                                var taxes = itemTax.Taxes;
                                                if (taxes.length > 0) {
                                                    _taxRow += `
                                                        <tr>
                                                            <td style='width: 25px;'>${passengerType}</td>
                                                            <td style='width: 25px;' class='text-center'>:</td>
                                                            <td style='width: 25px;' class="text-right">${_quantities}</td>
                                                            <td style='width: 25px;' class='text-center'>x</td>
                                                            <td style='width: 100px;' class="text-right" data-tax='${taxOfType}'>${LibCurrencies.FormatToCurrency(fareOfType)}</td>
                                                            <td style='width: 25px;' class='text-center'>=</td>
                                                            <td style=' 'class="text-right">${LibCurrencies.FormatToCurrency(flightTotal)} đ</td>
                                                        </tr>`;
                                                };
                                                //
                                            });
                                        }
                                        //
                                    }
                                });
                                // total fee 
                                _fareFlight += fareRound;
                            }
                            else {
                                _taxRow = '<tr><td>Lỗi cập nhật giá...</td></tr>';
                            }
                            var _flightRow = `<tr id="TblRow" ${index + 1}>
                                <td>
                                    <label class="lblTrademark">VNA-${mdAirEquipType}</label>
                                </td>
                                <td>
                                     <label class="lblFlightNo"><i class="fa fa-plane" aria-hidden="true"></i> VN ${mdFlightNumber}</label>
                                </td>
                                <td>
                                    <label class="lblItinerary"><label class="lblItinerary-location">${mdOriginLocation} - ${mdDestinationLocation}</label>, <span class="lblItinerary-date">${mdDepartureDate}</span></label>
                                </td>
                                <td>
                                    <span class="lblItineraryTime">${mdDepartureTime} - ${mdArrivalTime}</span>
                                </td>
                                <td class="text-center">
                                    <label class="lblResbookdesigcode">${mdResBookDesigCode}</label>
                                </td>
                                <td class="td-inf-price">
                                    <table width="100%" border="0" cellpadding="0" cellspacing="0"> ${_taxRow}</table>
                                </td>
                                <td class="td-faretotal text-right">
                                    <label class="depart-price">${LibCurrencies.FormatToCurrency(fareRound)} đ</label>
                                </td>
                            </tr>`;
                            // add row to table
                            $("#TblFlight > tbody").append(_flightRow);
                        });
                        var _flightRow = `<tr>
                                <td colspan="7" class="total-itinerary text-right"><label>Tổng:&nbsp; </label> <label class="text-right total-itinerary-price" style='color:#F00;'>${LibCurrencies.FormatToCurrency(_fareFlight)} đ</label></td>
                            </tr>`
                        // add row to table
                        $("#TblFlight > tbody").append(_flightRow);





                    }
                },
                error: function (response) {
                    console.log('::' + MessageText.NotService);
                }
            });
            //
            //lFlight.push({
            //    DepartureDateTime: _departureDateTime,
            //    ArrivalDateTime: _arrivalDateTime,
            //    FlightNumber: _flightNumber,
            //    NumberInParty: _numberInParty,
            //    ResBookDesigCode: _resBookDesigCode,
            //    AirEquipType: _airEquipType,
            //    DestinationLocation: _destinationLocation,
            //    OriginLocation: _originLocation
            //});
        }
    }
}



//###################################################################################################################################################
$(document).on('keyup', '#txtTitle', function () {
    var title = $(this).val();
    if (title === '') {
        $('#lblTitle').html('Không được để trống tiêu đề');
    }
    else if (title.length < 1 || title.length > 80) {
        $('#lblTitle').html('Tiêu đề giới hạn từ 1-> 80 characters');
    }
    else if (!FormatKeyword.test(title)) {
        $('#lblTitle').html('Tiêu đề không hợp lệ');
    }
    else {
        $('#lblTitle').html('');
    }
});
//
$(document).on('keyup', '#txtIataCode', function () {
    var iataCode = $(this).val();
    if (iataCode === '') {
        $('#lblIataCode').html('Không được để trống mã chuyến bay');
    }
    else if (iataCode.length < 1 || iataCode.length > 5) {
        $('#lblIataCode').html('Mã chuyến bay giới hạn từ 5 characters');
    }
    else if (!FormatKeyId.test(iataCode)) {
        $('#lblIataCode').html('Mã chuyến bay không hợp lệ');
    }
    else {
        $('#lblIataCode').html('');
    }
});
//
$(document).on('keyup', '#txtSummary', function () {
    var summary = $(this).val();
    if (summary !== '') {
        if (summary.length < 1 || summary.length > 120) {
            $('#lblSummary').html('Tiêu đề giới hạn từ 1-> 80 characters');
        }
        else if (!FormatKeyword.test(summary)) {
            $('#lblSummary').html('Mô tả không hợp lệ');
        }
        else {
            $('#lblSummary').html('');
        }
    }
    else {
        $('#lblSummary').html('');
    }
});
//
$(document).on('blur', '#DepartureDateTime', function () {
    var dateGo = $(this).val();
    $('#lblDepartureDateTime').html('');
    if (dateGo === "") {
        $('#lblDepartureDateTime').html('Vui lòng nhập ngày đi');
    }
});
//
$(document).on('blur', '#ReturnDateTime', function () {
    var dateReturn = $(this).val();
    $('#lblReturnDateTime').html('');
    var ddlFlightType = $('#ddlFlightType').val();
    if (parseInt(ddlFlightType) === 2) {
        if (dateReturn === "") {
            $('#lblReturnDateTime').html('Vui lòng nhập ngày về');
        }
    }
});
//
$(document).on("change", "#ddlOriginLocation", function () {
    var ddlDestinationLocation = $('#ddlDestinationLocation').val();
    var txtCtl = $(this).val();
    $('#lblOriginLocation').html('');
    if (txtCtl === "-" || txtCtl === "") {
        $('#lblOriginLocation').html('Vui lòng chọn nơi đi');
    }
    else if (txtCtl === ddlDestinationLocation && ddlDestinationLocation !== "" && ddlDestinationLocation !== "-") {
        $('#lblOriginLocation').html('Nơi đi và nơi đến không được trùng nhau');
        $('#lblDestinationLocation').html('');
    }
});
//
$(document).on("change", "#ddlDestinationLocation", function () {
    var ddlOriginLocation = $('#ddlOriginLocation').val();
    var txtCtl = $(this).val();
    $('#lblDestinationLocation').html('');
    if (txtCtl === "-" || txtCtl === "") {
        $('#lblDestinationLocation').html('Vui lòng chọn nơi đến');
    }
    else if (ddlOriginLocation === txtCtl && ddlOriginLocation !== "" && ddlOriginLocation !== "-") {
        $('#lblDestinationLocation').html('Nơi đi và nơi đến không được trùng nhau');
        $('#lblOriginLocation').html('');
    }
});
//
$(document).on("change", "#ddlFlightType", function () {
    $('#lblReturnDateTime').html('');
    var txtCtl = $(this).val();
    if (parseInt(txtCtl) === 2) {
        // Flight to
        var DepartureDateTime = $('#DepartureDateTime').val();
        if (DepartureDateTime === "") {
            $('#lblReturnDateTime').html('Vui lòng nhập ngày về');
        }
    }
});
//
$(document).on('click', '#cbxActive', function () {
    if ($(this).hasClass('actived')) {
        // remove
        $(this).children('i').removeClass('fa-check-square');
        $(this).children('i').addClass('fa-square');
        $(this).removeClass('actived');
    }
    else {
        $(this).children('i').addClass('fa-check-square');
        $(this).children('i').removeClass('fa-square');
        $(this).addClass('actived');
    }
});
//
$(document).on('', '.img-caption-text', function () {
    $('.new-box-preview img').click();
});


//###################################################################################################################################################
// valid full name
$(document).on('keyup', 'input[name="txtFullName"]', function () {
    var _wtrl = $(this).closest("div");
    if ($(this).val() === "") {
        $(_wtrl).find("span[name='lblFullName']").html("Không được để trống họ tên");
    }
    else if ($(this).val().length < 5 || $(this).val().length > 30) {
        $(_wtrl).find("span[name='lblFullName']").html("Họ tên giới hạn [5-30] ký tự");
    }
    else if (!ValidData.ValidName($(this).val(), "vn")) {
        $(_wtrl).find("span[name='lblFullName']").html("Họ tên không hợp lệ");
    }
    else {
        $(_wtrl).find("span[name='lblFullName']").html("");
    }
});

// valid birthday
$(document).on('keyup', 'input[name="txtBirthDay"]', function () {
    var _wtrl = $(this).closest("div");
    if ($(this).val() === "") {
        $(_wtrl).find("span[name='lblBirthDay']").html("Không được để trống ngày sinh");
    }
    else if (!LibDateTime.ValidDate($(this).val(), "vn")) {
        $(_wtrl).find("span[name='lblBirthDay']").html("Ngày sinh không hợp lệ, format dd-mm-yyyy");
    }
    else {
        $(_wtrl).find("span[name='lblBirthDay']").html("");
    }
});

// valid name in contact
$(document).on('keyup', '#txtName', function () {
    var name = $(this).val();
    if (name === "") {
        $("#lblName").html("Không được để trống họ tên liên hệ");
    }
    else {
        $("#lblName").html("");
    }
});// valid phone number
$(document).on('keyup', '#txtPhone', function () {
    var phone = $(this).val();
    if (phone === "") {
        $("#lblPhone").html("Không được để trống số điện thoại");
    }
    else if (!ValidData.ValidPhoneNumber(phone, "vn")) {
        $("#lblPhone").html("Số điện thoại không hợp lệ");
    }
    else {
        $("#lblPhone").html("");
    }
});
// valid email address
$(document).on('keyup', '#txtEmail', function () {
    var email = $(this).val();
    $("#lblEmail").html("");
    if (email !== "" && !ValidData.ValidEmail(email)) {
        $("#lblEmail").html("Địa chỉ email không hợp lệ");
        flg = false;
    }
});

