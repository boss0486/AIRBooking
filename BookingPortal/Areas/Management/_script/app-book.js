var pageIndex = 1;
var URLC = "/Management/AirBook/Action";
var URLA = "/Management/AirBook";
var arrFile = [];
//
var AirBookController = {
    init: function () {
        AirBookController.registerEvent();
    },
    registerEvent: function () {
        $(document).ready(function () {
            // $('[data-dateDefault="true"]').val(LibDateTime.Get_ClientDate(lg = 'en'));
        });
        //$('#btnSearch').off('click').on('click', function () {
        //    AirBookController.DataList(1);
        //});
        $('#btnSearch').off('click').on('click', function () {
            AirBookController.DataList(1);
        });
        //
        $('#btnExport').off('click').on('click', function () {
            //   
            var ddlItinerary = $('#ddlItinerary').val();
            var ddlAgentID = $('#ddlAgentID').val();
            var ddlCustomerType = $('#ddlCustomerType').val();
            var ddlCompanyID = $('#ddlCompanyID').val();
            var ddlTimeExpress = $('#ddlTimeExpress').val();
            var txtStartDate = $('#txtStartDate').val();
            var txtEndDate = $('#txtEndDate').val();
            //
            if (ddlCustomerType == "") {
                ddlCustomerType = 0;
            }
            var model = {
                Query: $('#txtQuery').val(),
                Page: 1,
                TimeExpress: parseInt(ddlTimeExpress),
                StartDate: txtStartDate,
                EndDate: txtEndDate,
                TimeZoneLocal: LibDateTime.GetTimeZoneByLocal(),
                ItineraryType: parseInt(ddlItinerary),
                AgentID: ddlAgentID,
                CustomerType: ddlCustomerType,
                CompanyID: ddlCompanyID
            };
            //
            AjaxFrom.POST({
                url: `/Management/AirOrder/Action/BookExport`,
                data: model,
                success: function (result) {
                    if (result !== null) {
                        if (result.status === 200) {
                            //
                            HelperModel.Download(result.path);
                        }
                        else {
                            Notifization.Error(result.message);
                            return;
                        }
                    }
                    //Message.Error(MessageText.NotService);
                    return;
                },
                error: function (result) {
                    console.log('::' + MessageText.NotService);
                }
            });
        });
        //
        $('#btnSync').off('click').on('click', function () {
            //   
            var txtPnr = $('#txtPnr').val();
            var model = {
                PnrCode: txtPnr
            };
            //
            AjaxFrom.POST({
                url: `/Management/AirOrder/Action/PnrSync`,
                data: model,
                success: function (result) {
                    if (result !== null) {
                        if (result.status === 200) {
                            Notifization.Error(result.message);
                            AirBookController.DataList(1);
                        }
                        else {
                            Notifization.Error(result.message);
                            return;
                        }
                    }
                    //Message.Error(MessageText.NotService);
                    return;
                },
                error: function (result) {
                    console.log('::' + MessageText.NotService);
                }
            });
        });
    },
    DataList: function (page) {
        //   
        var ddlItinerary = $('#ddlItinerary').val();
        var ddlAgentID = $('#ddlAgentID').val();
        var ddlCustomerType = $('#ddlCustomerType').val();
        var ddlCompanyID = $('#ddlCompanyID').val();
        var ddlTimeExpress = $('#ddlTimeExpress').val();
        var txtStartDate = $('#txtStartDate').val();
        var txtEndDate = $('#txtEndDate').val();
        //
        if (ddlCustomerType == "") {
            ddlCustomerType = 0;
        }
        var model = {
            Query: $('#txtQuery').val(),
            Page: page,
            TimeExpress: parseInt(ddlTimeExpress),
            StartDate: txtStartDate,
            EndDate: txtEndDate,
            TimeZoneLocal: LibDateTime.GetTimeZoneByLocal(),
            ItineraryType: parseInt(ddlItinerary),
            AgentID: ddlAgentID,
            CustomerType: ddlCustomerType,
            CompanyID: ddlCompanyID
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
                            var pnr = item.PNR;
                            var issueDate = item.IssueDateText;
                            var airlineId = item.AirlineID;
                            var ticketingId = item.TicketingID;
                            var ticketingName = item.TicketingName;
                            var agentCode = item.AgentCode;
                            var itineraryText = item.ItineraryText;
                            //var customerTypeText = item.CustomerTypeText;
                            //var companyId = item.CompanyID;
                            var companyCode = item.CompanyCode;
                            //var contactName = item.ContactName;
                            //
                            var totalAmount = item.TotalAmount;
                            var fareBasic = item.FareBasic;
                            var fareTax = item.FareTax;
                            var agentPrice = item.AgentPrice;
                            var agentFee = item.AgentFee;
                            var providerFee = item.ProviderFee;
                            //  
                            var mailStatus = item.MailStatus;
                            if (companyCode == null) {
                                companyCode = "-";
                            }
                            var _unit = 'đ';
                            //  role
                            var action = HelperModel.RolePermission(result.role, "AirBook", id);
                            //
                            var rowNum = parseInt(index) + (parseInt(currentPage) - 1) * parseInt(pageSize);
                            rowData += `
                            <tr data-id='${id}' data-ticketingId='${ticketingId}'>
                                 <td class="text-right">${rowNum}&nbsp;</td>  
                                 <td >${issueDate}</td>  
                                 <td class='text-center'>${agentCode}</td>  
                                 <td >${ticketingName}</td>  
                                 <td ><a class='btn-passenger' data-id='${id}'>C.tiết</a></td>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              
                                 <td class='text-center bg-success'>${airlineId}</td>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        
                                 <td class='text-right bg-yellow-1'><a class='btn-farebasic' data-id='${id}'>${LibCurrencies.FormatToCurrency(fareBasic + fareTax)} ${_unit} </a></td>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    
                                 <td class='text-right bg-yellow-1'><input name='inp-agtprice' data-currency="true" data-val='${agentPrice}' value ='${LibCurrencies.FormatToCurrency(agentPrice)}' />${_unit}</td>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    
                                 <td class='text-right bg-yellow-1'><input name='inp-tktfee' data-currency="true" data-val='${agentFee}' value ='${LibCurrencies.FormatToCurrency(agentFee)}' />${_unit}</td>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    
                                 <td class='text-right bg-yellow-1'>${LibCurrencies.FormatToCurrency(providerFee)} ${_unit}</td>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    
                                 <td class='text-right bg-yellow-1'>${LibCurrencies.FormatToCurrency(totalAmount)} ${_unit}</td>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    
                                 <td class='text-left'>${pnr}</td>                                                                                                                                                                                                                                                                         
                                 <td class='tbcol-left tbcol-button'>
                                     <button type="button" class="btn btn-primary btn-sm btn-export" data-id="${id}" data-pnr="${pnr}">Xuất</button>
                                     <button type="button" class="btn btn-warning btn-sm btn-email" data-id="${id}" data-pnr="${pnr}">Gửi ${mailStatus}</button>
                                     <button type="button" class="btn btn-danger btn-sm btn-book-void" data-id="${id}" data-pnr="${pnr}">Hủy</button>
                                 </td>                                  
                                 <td class="tbcol-action">${action}</td>
                            </tr>`;
                        });
                        $('tbody#TblData').html(rowData);
                        if (parseInt(totalPage) > 1) {
                            Paging.Pagination("#Pagination", totalPage, currentPage, AirBookController.DataList);
                        }
                        return;
                    }
                    else {
                        //Notifization.Error(result.message);
                        return;
                    }
                }
                Notifization.Error(MessageText.NotService);
                return;
            },
            error: function (result) {
                console.log('::' + MessageText.NotService);
            }
        });
    },
    VoidBook: function (id) {
        var model = {
            ID: id
        };
        AjaxFrom.POST({
            url: URLC + '/VoidBook',
            data: model,
            success: function (response) {
                if (response !== null) {
                    if (response.status === 200) {
                        Notifization.Success(response.message);
                        AirBookController.DataList(pageIndex);
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
    ConfirmExport: function (id) {
        Confirm.ConfirmYN(id, AirBookController.ExportTiket, Confirm.Text_ExportTicket);
    },
    ExportTiket: function (id) {
        var model = {
            ID: id
        };
        AjaxFrom.POST({
            url: URLC + '/ExTicket',
            data: model,
            success: function (response) {
                if (response !== null) {
                    if (response.status === 200) {
                        Notifization.Success(response.message);
                        AirBookController.DataList(pageIndex);
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
    ConfirmVoidBook: function (id) {
        Confirm.ConfirmYN(id, AirBookController.VoidBook, Confirm.Text_VoidBook);
    }
};
//
AirBookController.init();

//*******************************************************
function BookingOrderLoad() {
    var cookiData = Cookies.GetCookie("FlightOrder");
    if (cookiData !== undefined && cookiData !== "") {
        var order = JSON.parse(cookiData);
        if (order == undefined || order == null)
            return
        //
        var feeTaxModel = [];
        var labelTbl = [];

        var _adt = parseInt(order.ADT);
        var _cnn = parseInt(order.CNN);
        var _inf = parseInt(order.INF);
        var segments = order.Segments;

        // show data 
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
        $.each(segments, function (oIndex, oItem) {
            var _rphFlight = oItem.RPH;
            var _arrivalDateTime = oItem.ArrivalDateTime;
            var _departureDateTime = oItem.DepartureDateTime;
            var _flightNumber = oItem.FlightNumber;
            var _resBookDesigCode = oItem.ResBookDesigCode;
            var _airEquipType = oItem.AirEquipType;
            var _destinationLocation = oItem.DestinationLocation;
            var _originLocation = oItem.OriginLocation;
            let _fareBase = oItem.FareBase;
            //
            var arrPassenger = [];
            if (_fareBase.includes("##")) {
                var arrParam = _fareBase.split("##");
                if (arrParam.length > 0) {
                    $.each(arrParam, function (index, item) {
                        if (item.includes(",")) {
                            var passengerType = '';
                            var quantity = 0;
                            var amount = 0;
                            var rph = 0;
                            var code = 0;
                            if (item.includes(":")) {
                                var passengerType = item.split(":")[0];
                                var passengerFare = item.split(":")[1];
                                var passengerFareInfo = passengerFare.split(",");
                                if (passengerType == "ADT")
                                    quantity = _adt;
                                if (passengerType == "CNN")
                                    quantity = _cnn;
                                if (passengerType == "INF")
                                    quantity = _inf;
                                //
                                rph = parseFloat(passengerFareInfo[0]);
                                amount = parseFloat(passengerFareInfo[1]);
                                code = passengerFareInfo[2];
                            }
                            arrPassenger.push({
                                RPH: rph,
                                PassengerType: passengerType,
                                Quantity: quantity,
                                Amount: amount,
                                FareBaseCode: code
                            });
                        }
                    });
                }
            }
            else {
                var arrParam = _fareBase;
                if (arrParam.length > 0 && arrParam.includes(",")) {
                    var passengerType = '';
                    var quantity = 0;
                    var amount = 0;
                    var rph = 0;
                    var code = 0;
                    if (arrParam.includes(":")) {
                        var passengerType = arrParam.split(":")[0];
                        var passengerFare = arrParam.split(":")[1];
                        var passengerFareInfo = passengerFare.split(",");
                        if (passengerType == "ADT")
                            quantity = _adt;
                        if (passengerType == "CNN")
                            quantity = _cnn;
                        if (passengerType == "INF")
                            quantity = _inf;
                        //
                        rph = parseFloat(passengerFareInfo[0]);
                        amount = parseFloat(passengerFareInfo[1]);
                        code = passengerFareInfo[2];
                    }
                    arrPassenger.push({
                        RPH: rph,
                        PassengerType: passengerType,
                        Quantity: quantity,
                        Amount: amount,
                        FareBaseCode: code
                    });
                }
            }
            //
            var feeBase = {
                RPH: _rphFlight,
                OriginLocation: _originLocation,
                DestinationLocation: _destinationLocation,
                DepartureDateTime: _departureDateTime,
                ArrivalDateTime: _arrivalDateTime,
                AirEquipType: _airEquipType,
                FareBase: arrPassenger,
                ResBookDesigCode: _resBookDesigCode,
                FlightNumber: _flightNumber,
                CurrencyCode: "VND"
            };
            // add array
            feeTaxModel.push(feeBase);
        });
        //******************************************************************************************************************
        if (feeTaxModel.length == 0) {
            Notifization.Error("Dữ liệu không hợp lệ");
            return;
        }
        //
        AjaxFrom.POST({
            url: URLC + '/GetFeeBasic',
            data: { models: feeTaxModel },
            success: function (response) {
                if (response == null || response.status == undefined) {
                    Notifization.Error(MessageText.NotService);
                    return;
                }
                if (response.status == 200) {
                    $("#TblGoPriceInfo").html(`<tr><td>Đang cập nhật...</td></tr>`);
                    // flight 
                    var _fareFlight = 0;
                    $("#TblFlight tbody").html('');
                    _taxRow = "";
                    var totalAll = 0;
                    $.each(response.data, function (index, item) {
                        var mdAirEquipType = item.AirEquipType;
                        var mdFlightNumber = item.FlightNumber;
                        var mdResBookDesigCode = item.ResBookDesigCode;
                        var mdDestinationLocation = item.DestinationLocation;
                        var mdOriginLocation = item.OriginLocation;
                        var mdDepartureDate = "";
                        //var mdArrivalDate = item.ArrivalDateTime;
                        var mdDepartureTime = item.DepartureDateTime;
                        var mdArrivalTime = item.ArrivalDateTime;
                        var fareRound = 0;
                        var _taxRow = "";

                        var flightTaxInfos = item.FlightTaxInfos;
                        if (flightTaxInfos != null) {
                            $.each(flightTaxInfos, function (idxTax, itemTax) {
                                //
                                var _quantities = itemTax.Quantity;
                                var passengerType = itemTax.PassengerType.Code;
                                var taxOfType = itemTax.Total;
                                var flightAmount = 0;
                                var flightTotal = 0;
                                var fareOfType = 0;
                                if (parseFloat(taxOfType) > 0 && parseFloat(_quantities) > 0) {
                                    fareOfType = flightAmount + taxOfType;
                                    flightTotal = fareOfType * parseFloat(_quantities);
                                    fareRound += flightTotal;
                                }
                                //
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
                                var taxes = itemTax.Taxes;
                                //
                            });
                        }
                        //
                        var _idx = (`0${(index + 1)}`).substr(-2);
                        $("#TblFlight > tbody").append(`<tr id="TblRow">
                                <td class='text-right'>${_idx}&nbsp;</td>
                                <td><label class="lblFlightNo">VN ${mdAirEquipType}-${mdFlightNumber}</label></td>
                                <td><label class="lblItinerary"><label class="lblItinerary-location">${mdOriginLocation} - ${mdDestinationLocation}</label></label></td>
                                <td><p class="lblItineraryTime">${mdDepartureTime}</p><p class="lblItineraryTime">${mdArrivalTime}</p></td>
                                <td class="text-center"><label class="lblResbookdesigcode">${mdResBookDesigCode}</label></td>
                                <td class="td-inf-price"><table width="100%" border="0" cellpadding="0" cellspacing="0"> ${_taxRow}</table></td>
                                <td class="td-faretotal text-right"><label class="depart-price">${LibCurrencies.FormatToCurrency(fareRound)} đ</label></td>
                            </tr>`);
                        totalAll += fareRound;
                    });
                    $("#TblFlight > tbody").append(`<tr><td colspan="7" class="total-itinerary text-right"><label>Tổng:&nbsp; </label> <label class="text-right total-itinerary-price" style='color:#F00;'>${LibCurrencies.FormatToCurrency(totalAll)} đ</label></td></tr>`);
                }
            },
            error: function (response) {
                console.log('::' + MessageText.NotService);
            }
        });


    }
}
// valid ddlProvider
//###################################################################################################################################################
// valid full name
$(document).on('keyup', 'input[name="txtFullName"]', function () {
    var _wtrl = $(this).closest("div");
    if ($(this).val() == "") {
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
    if ($(this).val() == "") {
        $(_wtrl).find("span[name='lblBirthDay']").html("Không được để trống ngày sinh");
    }
    else if (!ValidData.ValidDate($(this).val(), "vn")) {
        $(_wtrl).find("span[name='lblBirthDay']").html("Ngày sinh không hợp lệ");
    }
    else {
        $(_wtrl).find("span[name='lblBirthDay']").html("");
    }
});
//KHACH LE **********************************************************************************************************************************

$(document).on('change', '#ddlEmployee', function () {
    var ddlEmployee = $(this).val();
    $("#lblEmployee").html("");
    var rdoPassengerGroup = $("input[name='rdoPassengerGroup']:checked").val();
    if (parseInt(rdoPassengerGroup) == CustomerTypeEnum.Haunt) {
        if (ddlEmployee == "") {
            $("#lblEmployee").html("Vui lòng chọn nhân viên");
        }
    }
});

$(document).on('keyup', '#txtName', function () {
    var name = $(this).val();
    $("#lblName").html("");
    var rdoPassengerGroup = $("input[name='rdoPassengerGroup']:checked").val();
    if (parseInt(rdoPassengerGroup) == 1) {
        if (name == "") {
            $("#lblName").html("Không được để trống họ tên liên hệ");
        }
    }

});// valid phone number
$(document).on('keyup', '#txtPhone', function () {
    var phone = $(this).val();
    $("#lblPhone").html("");
    var rdoPassengerGroup = $("input[name='rdoPassengerGroup']:checked").val();
    if (parseInt(rdoPassengerGroup) == 1) {
        if (phone == "") {
            $("#lblPhone").html("Không được để trống số điện thoại");
        }
        else if (!ValidData.ValidPhoneNumber(phone, "vn")) {
            $("#lblPhone").html("Số điện thoại không hợp lệ");
        }
    }

});
// valid email address
$(document).on('keyup', '#txtEmail', function () {
    var email = $(this).val();
    $("#lblEmail").html("");
    var rdoPassengerGroup = $("input[name='rdoPassengerGroup']:checked").val();
    if (parseInt(rdoPassengerGroup) == 1) {
        if (email !== "" && !ValidData.ValidEmail(email)) {
            $("#lblEmail").html("Địa chỉ email không hợp lệ");
        }
    }
});

//COMP **********************************************************************************************************************************
$(document).on("change", "input[name='rdoPassengerGroup']", function () {
    $("#lblCompany").html("");
    $("#lblName").html("");
    $("#txtPhone").html("");

    var rdoPassengerGroup = $("input[name='rdoPassengerGroup']:checked").val();
    // comp
    var option = `<option value="">-Lựa chọn-</option>`;
    $('#ddlCompany').html(option);
    $('#ddlCompany').selectpicker('refresh');
    if (parseInt(rdoPassengerGroup) == CustomerTypeEnum.Company) {
        //
        $("#txtName").val("");
        $("#txtPhone").val("");
        $("#txtEmail").val("");
        //
        var _id = "";
        var model = {
        };
        AjaxFrom.POST({
            url: URLC + '/GetCompany',
            data: model,
            async: true,
            success: function (result) {
                if (result !== null) {
                    if (result.status == 200) {
                        var attrSelect = '';
                        if (result.data == null || result.data == undefined)
                            return;
                        //
                        $.each(result.data, function (index, item) {
                            var id = item.ID;
                            if (_id !== undefined && _id != "" && _id == item.ID) {
                                attrSelect = "selected";
                            }
                            option += `<option value='${id}' ${attrSelect} data-codeid='${item.CodeID}'>${item.CodeID}: ${item.Title}</option>`;
                        });
                        $('#ddlCompany').html(option);
                        $('#ddlCompany').selectpicker('refresh');
                        return;
                    }
                }
                return;
            },
            error: function (result) {
                console.log('::' + MessageText.NotService);
            }
        });
    }
    else {
        var option = `<option value="">-Lựa chọn-</option>`;
        $('#ddlCompany').html(option);
        $('#ddlCompany').selectpicker('refresh');

    }
});
$(document).on('change', '#ddlCompany', function () {
    var ddlCompany = $(this).val();
    $("#lblCompany").html("");
    var rdoPassengerGroup = $("input[name='rdoPassengerGroup']:checked").val();
    if (parseInt(rdoPassengerGroup) == CustomerTypeEnum.Company) {
        if (ddlCompany == "") {
            $("#lblCompany").html("Vui lòng chọn công ty");
        }
    }
    var codeid = $(this).find(':selected').data('codeid');
    $("#lblCompanyCodeID").html(codeid);

});
$(document).on('keyup', '#txtMessage', function () {
    var txtMessage = $(this).val();
    $("#lblMessage").html("");
    if (txtMessage !== '') {
        if (txtMessage.length > 120) {
            $('#lblMessage').html('Nội dung giới hạn từ 1-> 120 ký tự');
        }
        else if (!FormatKeyword.test(txtMessage)) {
            $('#lblMessage').html('Nội dung không hợp lệ');
        }
    }
});

$(document).on('change', '#ddlProvider', function () {
    var ddlProvider = $(this).val();
    $("#lblProvider").html("");
    if (ddlProvider == "") {
        $("#lblProvider").html("Vui lòng chọn đại lý");
    }
    var option = `<option value="">-Lựa chọn-</option>`;
    $('#ddlEmployee').html(option);
    $('#ddlEmployee').selectpicker('refresh');

    $('select#ddlEmployee').html(option);
    $('select#ddlEmployee').selectpicker('refresh');
    var model = {
        ID: ddlProvider
    };

    //GetTicketing
    AjaxFrom.POST({
        url: '/Management/User/Action/GetTicketing',
        data: model,
        success: function (response) {
            if (response !== null) {
                if (response.status === 200) {
                    $.each(response.data, function (index, item) {
                        index = index + 1;
                        //
                        var strIndex = '';
                        if (index < 10)
                            strIndex += "0" + index;
                        //
                        var id = item.UserID;
                        var title = item.FullName;
                        option += `<option value='${id}'>${title}</option>`;
                    });
                    $('select#ddlEmployee').html(option);
                    $('select#ddlEmployee').selectpicker('refresh');
                    return;
                }
            }
            return;
        },
        error: function (result) {
            console.log('::' + MessageText.NotService);
        }
    });



});

//
$(document).on('click', '#btnBooking', function () {
    var flg = true;
    // 
    // valid full name
    $("#BookingForm input[name='txtFullName']").each(function (index, item) {
        var _wtrl = $(this).closest("div");
        if ($(this).val() == "") {
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
        if ($(this).val() == "") {
            $(_wtrl).find("span[name='lblBirthDay']").html("Không được để trống ngày sinh");
            flg = false;
        }
        else if (!ValidData.ValidDate($(this).val(), "vn")) {
            $(_wtrl).find("span[name='lblBirthDay']").html("Ngày sinh không hợp lệ");
            flg = false;
        }
        else {
            $(_wtrl).find("span[name='lblBirthDay']").html("");
        }
    });

    // FOR CONTACT ************************************************************************************************
    // valid name of passengers   
    var customerType = $("input[name='rdoPassengerGroup']:checked").val();
    var ddlProvider = $("#ddlProvider").val();
    var ddlEmployee = $("#ddlEmployee").val();
    var ddlCompany = $("#ddlCompany").val();
    var name = $("#txtName").val();
    // comp
    $("#lblCompany").html("");
    // Khach le
    $("#lblName").html("");
    $("#txtPhone").html("");
    $("#lblEmail").html("");
    $("#lblCompany").html("");
    $("#lblProvider").html("");
    $("#lblEmployee").html("");
    if (ddlProvider == "") {
        $("#lblProvider").html("Vui lòng chọn đại lý");
        flg = false;
    }
    if (ddlEmployee == "") {
        $("#lblEmployee").html("Vui lòng chọn nhân viên");
        flg = false;
    }

    if (parseInt(customerType) == CustomerTypeEnum.Haunt) {

        //
        $("#lblName").html("");
        if (name == "") {
            $("#lblName").html("Không được để trống họ tên liên hệ");
            flg = false;
        }
        // valid phone number
        var phone = $("#txtPhone").val();
        if (phone == "") {
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
    }
    else if (parseInt(customerType) == CustomerTypeEnum.Company) {
        if (ddlCompany == "") {
            $("#lblCompany").html("Vui lòng chọn công ty");
            flg = false;
        }
    }
    else {
        flg = false;
    }
    // 
    var txtMessage = $("#txtMessage").val();
    $("#lblMessage").html("");
    if (txtMessage !== '') {
        if (txtMessage.length > 120) {
            $('#lblMessage').html('Nội dung giới hạn từ 1-> 120 ký tự');
            flg = false;
        }
        else if (!FormatKeyword.test(txtMessage)) {
            $('#lblMessage').html('Nội dung không hợp lệ');
            flg = false;
        }
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
                if (birthDay != null || birthDay != undefined) {
                    birthDay = LibDateTime.FormatDateForAPI(birthDay);
                }
                lPassenger.push({
                    PassengerType: type,
                    FullName: fullName,
                    Gender: gender,
                    Phone: phone,
                    DateOfBirth: birthDay
                });
            });
        }
    });
    if (lPassenger.length == 0) {
        Notifization.Error("Dữ liệu không hợp lệ");
        return;
    }
    // get flight data
    var cookiData = Cookies.GetCookie("FlightOrder");

    if (cookiData == undefined || cookiData == "") {
        Notifization.Error("Dữ liệu không hợp lệ");
        return;
    }
    var order = JSON.parse(cookiData);
    if (order == null) {
        Notifization.Error("Dữ liệu không hợp lệ");
        return;
    }
    var lFlight = [];
    var ddlAirlineType = order.AirlineType;
    var ddlItineraryType = order.ItineraryType;
    //   
    $.each(order, function (index, item) { 
        var _arrivalDateTime = item.ArrivalDateTime;
        var _departureDateTime = item.DepartureDateTime;
        var _flightNumber = item.FlightNumber;
        var _numberInParty = item.NumberInParty;
        var _resBookDesigCode = item.ResBookDesigCode;
        var _airEquipType = item.AirEquipType;
        var _destinationLocation = item.DestinationLocation;
        var _originLocation = item.OriginLocation;
        // 
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
    });
    //
    if (ddlProvider == undefined)
        ddlProvider = "";
    //
    if (ddlEmployee == undefined)
        ddlEmployee = "";
    //
    if (ddlCompany == undefined)
        ddlCompany = "";
    //  copntact of passengers
    var ticketingInfo = {
        ProviderID: ddlProvider,
        TiketingID: ddlEmployee
    };
    //
    var haunt = {
        Name: name,
        Email: email,
        Phone: phone
    };
    var company = {
        CompanyID: ddlCompany
    };
    //
    var lContact = {
        HauntContact: haunt,
        CompanyContact: company,
    };
    //
    var bookModel = {
        ItineraryType: ddlItineraryType,
        AirlineType: ddlAirlineType,
        CustomerType: customerType,
        TicketingInfo: ticketingInfo,
        Contacts: lContact,
        Passengers: lPassenger,
        Segments: lFlight,
        Summary: txtMessage
    };
    console.log("ok" + JSON.stringify(bookModel));
    return; 
    // call api
    if (flg) {
        AjaxFrom.POST({
            url: URLC + '/booking',
            data: bookModel,
            success: function (response) {
                if (response == null || response.status == undefined) {
                    Notifization.Error(MessageText.message);
                    return;
                }
                if (response.status == 200) {
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
    }
});
//
$(document).on('click', '#btnRelease', function () {
    var pnrCode = $('#lblPNRCode').data("pnr");
    if (pnrCode == undefined || pnrCode == "") {
        Notifization.Error("Không thể xuất vé với PNR này.");
        return;
    }
    AjaxFrom.POST({
        url: URLC + '/ExTicket',
        data: { PNR: pnrCode },
        success: function (response) {
            if (response == null || response.status == undefined) {
                Notifization.Error(MessageText.NotService);
                return;
            }
            if (response.status == 200) {
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
//*******************************************************
$(document).on("click", ".btn-export", function () {
    var id = $(this).data("id");
    AirBookController.ConfirmExport(id);
})
$(document).on("click", ".btn-book-void", function () {
    var id = $(this).data("id");
    AirBookController.ConfirmVoidBook(id);

})
// list *******************************************************

$(document).on('change', '#ddlAgentID', function () {
    var option = `<option value="">-Công ty-</option>`;
    $('select#ddlCompanyID').html(option);
    $('select#ddlCompanyID').selectpicker('refresh');

    $("#ddlCustomerType")[0].selectedIndex = 0;
    $("#ddlCustomerType").selectpicker("refresh");
});
$(document).on('change', '#ddlCustomerType', function () {
    var option = `<option value="">-Công ty-</option>`;
    $('select#ddlCompanyID').html(option);
    $('select#ddlCompanyID').selectpicker('refresh');
    var ddlCustomerType = $(this).val();
    if (parseInt(ddlCustomerType) == 1) {
        GetCompanyFilter();
    }
});
function GetCompanyFilter() {
    var option = `<option value="">-Công ty-</option>`;
    var _agentId = $("#ddlAgentID").val();
    var model = {
        AgentID: _agentId
    };
    //GetTicketing
    AjaxFrom.POST({
        url: URLC + '/GetCompByAgentID',
        data: model,
        success: function (response) {
            if (response !== null) {
                if (response.status === 200) {
                    if (response.data != null) {
                        $.each(response.data, function (index, item) {
                            var id = item.ID;
                            var title = item.CodeID;
                            option += `<option value='${id}'>${title}</option>`;
                        });
                        $('select#ddlCompanyID').html(option);
                        $('select#ddlCompanyID').selectpicker('refresh');
                    }
                    return;
                }
            }
            return;
        },
        error: function (result) {
            console.log('::' + MessageText.NotService);
        }
    });
}


//*******************************************************
function BookOrderStatus(_status) {
    var result = '';
    switch (_status) {
        case -1:
            result = "<i class='fas fa-circle'></i>";
            break;
        case 0:
            result = "";
            break;
        case 1:
            result = "<i class='fas fa-check-circle'></i>";
            break;
        default:
            result = "";
            break;
    }
    return result;
}

//*******************************************************

$(document).on("click", ".btn-passenger", function () {
    var id = $(this).data("id");
    var model = {
        ID: id
    };
    //
    $('#PassengerModal tbody#TblModalData').html('');
    AjaxFrom.POST({
        url: '/Management/AirOrder/Action/GetPassenger',
        data: model,
        success: function (result) {
            if (result !== null) {
                if (result.status === 200) {
                    var rowData = '';
                    var cnt = 1;
                    $.each(result.data, function (index, item) {
                        index = index + 1;
                        var id = item.ID;
                        if (id.length > 0)
                            id = id.trim();
                        //
                        var name = item.FullName;
                        var gender = item.GenderText;
                        var dateOfBirth = item.DateOfBirth;
                        var passengerType = item.PassengerType;
                        // 
                        rowData += `
                            <tr>
                                 <td class="text-right">${cnt}&nbsp;</td>  
                                 <td>${name}</td>  
                                 <td>${passengerType}</td>  
                                 <td>${gender}</td>  
                                 <td>${dateOfBirth}</td>   
                            </tr>`;
                        cnt++;
                    });
                    $('#PassengerModal tbody#TblModalData').html(rowData);
                    $("#PassengerModal").modal();
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
            console.log('::' + MessageText.NotService);
        }
    });
})

$(document).on("click", ".btn-farebasic", function () {
    var id = $(this).data("id");
    var model = {
        ID: id
    };
    //
    $('#FaseBasicModal tbody#TblModalData').html('');
    AjaxFrom.POST({
        url: '/Management/AirOrder/Action/GetFaseBasic',
        data: model,
        success: function (result) {
            if (result !== null) {
                if (result.status === 200) {
                    var rowData = '';
                    var cnt = 1;
                    var totalPrice = 0;
                    var totalTax = 0;
                    var totalQty = 0;
                    $.each(result.data, function (index, item) {
                        index = index + 1;
                        //
                        var passengerType = item.PassengerType;
                        var quantity = item.Quantity;
                        var amount = item.Amount;
                        var taxAmount = item.TaxAmount;
                        // 
                        rowData += `
                            <tr>
                                 <td class="text-right">${cnt}&nbsp;</td>  
                                 <td>${passengerType}</td>  
                                 <td class="text-right">${quantity}</td>  
                                 <td class="text-right">${LibCurrencies.FormatToCurrency(amount)} đ</td>  
                                 <td class="text-right">${LibCurrencies.FormatToCurrency(taxAmount)} đ</td>   
                            </tr>`;
                        cnt++;
                        totalPrice += amount;
                        totalTax += taxAmount;
                        totalQty += quantity;
                    });
                    // toatl

                    rowData += `
                            <tr class='highlight'>
                                 <td class="text-right">=></td>  
                                 <td>Tổng: ${LibCurrencies.FormatToCurrency(totalPrice + totalTax)}</td>  
                                 <td class="text-right">${totalQty}</td>  
                                 <td class="text-right">${LibCurrencies.FormatToCurrency(totalPrice)} đ</td>  
                                 <td class="text-right">${LibCurrencies.FormatToCurrency(totalTax)} đ</td>   
                            </tr>`;

                    $('#FaseBasicModal tbody#TblModalData').html(rowData);
                    $("#FaseBasicModal").modal();
                    return;
                }
                else {
                    Notifization.Error(result.message);;
                    return;
                }
            }
            Notifization.Error(MessageText.NotService);
            return;
        },
        error: function (result) {
            console.log('::' + MessageText.NotService);
        }
    });
})

//$(document).on("click", ".btn-contact", function () {
//    var id = $(this).data("id");
//    var model = {
//        ID: id
//    };
//    //
//    $('#ContactModal tbody#TblModalData').html('');
//    AjaxFrom.POST({
//        url: '/Management/AirOrder/Action/GetCompany',
//        data: model,
//        success: function (result) {
//            if (result !== null) {
//                if (result.status === 200) {
//                    var rowData = '';
//                    var cnt = 1;
//                    $.each(result.data, function (index, item) {
//                        index = index + 1;
//                        var id = item.ID;
//                        if (id.length > 0)
//                            id = id.trim();
//                        //
//                        var name = item.FullName;
//                        var gender = item.GenderText;
//                        var dateOfBirth = item.DateOfBirth;
//                        var passengerType = item.PassengerType;
//                        // 
//                        rowData += `
//                            <tr>
//                                 <td class="text-right">${cnt}&nbsp;</td>  
//                                 <td>${name}</td>  
//                                 <td>${passengerType}</td>  
//                                 <td>${gender}</td>  
//                                 <td>${dateOfBirth}</td>   
//                            </tr>`;
//                        cnt++;
//                    });
//                    $('#ContactModal tbody#TblModalData').html(rowData);
//                    $("#PassengerModal").modal();
//                    return;
//                }
//                else {
//                    Notifization.Error(result.message);
//                    return;
//                }
//            }
//            Notifization.Error(MessageText.NotService);
//            return;
//        },
//        error: function (result) {
//            console.log('::' + MessageText.NotService);
//        }
//    });
//})

$(document).on("blur", "table#TblBooking tbody#TblData input[name='inp-agtprice']", function () {
    var trObj = $(this).closest("tr");
    var _id = $(trObj).data("id");
    var _ticketingId = $(trObj).data("ticketingid");
    var _oldVal = $(this).data("val");
    var _val = $(this).val();
    //
    if (_val == "") {
        $(this).val(_oldVal);
        return;
    }
    if (!FormatCurrency.test(_val)) {
        $(this).addClass("error");
        return;
    }
    //
    _val = LibCurrencies.ConvertToCurrency(_val);
    if (_val == _oldVal) {
        $(this).removeClass("error");
        return;
    }
    //
    $(this).removeClass("error");
    var model = {
        ID: _id,
        TicktingID: _ticketingId,
        Amount: _val,
    };
    ////
    AjaxFrom.POST({
        url: '/Management/AirOrder/Action/BookEditPrice',
        data: model,
        success: function (result) {
            if (result !== null) {
                if (result.status === 200) {
                    Notifization.Success(result.message);
                    AirBookController.DataList(pageIndex);
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
            console.log('::' + MessageText.NotService);
        }
    });
})
$(document).on("blur", "table#TblBooking tbody#TblData input[name='inp-tktfee']", function () {
    var trObj = $(this).closest("tr");
    var _id = $(trObj).data("id");
    var _ticketingId = $(trObj).data("ticketingid");
    var _oldVal = $(this).data("val");
    var _val = $(this).val();
    //
    if (_val == "") {
        $(this).val(_oldVal);
        return;
    }
    if (!FormatCurrency.test(_val)) {
        $(this).addClass("error");
        return;
    }
    //
    _val = LibCurrencies.ConvertToCurrency(_val);
    if (_val == _oldVal) {
        $(this).removeClass("error");
        return;
    }
    //
    $(this).removeClass("error");
    var model = {
        ID: _id,
        TicktingID: _ticketingId,
        Amount: _val,
    };
    ////
    AjaxFrom.POST({
        url: '/Management/AirOrder/Action/BookEditAgentFee',
        data: model,
        success: function (result) {
            if (result !== null) {
                if (result.status === 200) {
                    Notifization.Success(result.message);
                    AirBookController.DataList(pageIndex);
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
            console.log('::' + MessageText.NotService);
        }
    });
})
$(document).on("click", ".btn-email", function () {
    var id = $(this).data("id");
    var model = {
        ID: id
    };
    //
    AjaxFrom.POST({
        url: '/Management/AirOrder/Action/BookEmail',
        data: model,
        success: function (result) {
            if (result !== null) {
                if (result.status === 200) {
                    AirBookController.DataList(pageIndex);
                    Notifization.Success(result.message);
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
            console.log('::' + MessageText.NotService);
        }
    });
})