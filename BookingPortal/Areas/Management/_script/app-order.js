var pageIndex = 1;
var URLC = "/Management/AirOrder/Action";
var URLA = "/Management/AirOrder";
var arrFile = [];
//
var AirOrderController = {
    init: function () {
        AirOrderController.registerEvent();
    },
    registerEvent: function () {
        $(document).ready(function () {
            // $('[data-dateDefault="true"]').val(LibDateTime.Get_ClientDate(lg = 'en'));
        });
        $('#btnSearch').off('click').on('click', function () {
            AirOrderController.DataList(1);
        });
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
                url: `${URLC}/OrderExport`,
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
                            var id = item.ID;
                            if (id.length > 0)
                                id = id.trim();
                            //
                            var ticketNo = item.TicketNumber;
                            if (ticketNo == null) {
                                ticketNo = "";
                            }
                            var fullName = item.FullName;
                            var pnr = item.PNR;
                            var issueDate = item.IssueDateText;
                            var airlineId = item.AirlineID;
                            var ticketingId = item.TicketingID;
                            var ticketingName = item.TicketingName;
                            var agentCode = item.AgentCode;
                            var itineraryText = item.ItineraryText;
                            var customerTypeText = item.CustomerTypeText;
                            //var companyId = item.CompanyID;
                            var companyCode = item.CompanyCode;
                            //var contactName = item.ContactName;
                            var totalAmount = item.TotalAmount;
                            var fareBasic = item.FareBasic;
                            var fareTax = item.FareTax;
                            var vatFee = item.VAT;
                            var agentPrice = item.AgentPrice;
                            var agentFee = item.AgentFee;
                            var providerFee = item.ProviderFee;
                            var providerCode = item.ProviderCode;
                            //   
                            var _unit = 'đ';
                            //  role
                            var action = HelperModel.RolePermission(result.role, "FlightController", id);
                            //
                            var rowNum = parseInt(index) + (parseInt(currentPage) - 1) * parseInt(pageSize);
                            rowData += `
                            <tr>
                                 <td class="text-right">${rowNum}&nbsp;</td>  
                                 <td class=''>${issueDate}</td>  
                                 <td class=''>${ticketNo}</td>  
                                 <td class=''>${fullName.toUpperCase()}</td>  
                                 <td class='text-center'>${providerCode}</td>  
                                 <td class='text-center'>${agentCode}</td>  
                                 <td class=''>${customerTypeText}</td>          
                                 <td class='text-center bg-success'>${airlineId}</td>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               
                                 <td class='text-right bg-yellow-1'>${LibCurrencies.FormatToCurrency(fareBasic)} ${_unit}</td>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     
                                 <td class='text-right bg-yellow-1'>${LibCurrencies.FormatToCurrency(fareTax)} ${_unit}</td>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     
                                 <td class='text-right bg-yellow-1'>${LibCurrencies.FormatToCurrency(vatFee)} ${_unit}</td>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     
                                 <td class='text-right bg-success'>${LibCurrencies.FormatToCurrency(agentPrice)} ${_unit}</td>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     
                                 <td class='text-right bg-success'>${LibCurrencies.FormatToCurrency(providerFee)} ${_unit}</td>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     
                                 <td class='text-right bg-success'>${LibCurrencies.FormatToCurrency(agentFee)} ${_unit}</td>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     
                                 <td class='text-right'>${LibCurrencies.FormatToCurrency(totalAmount)} ${_unit}</td>  
                                 <td class='tbcol-left tbcol-button'>
                                     <button type="button" class="btn btn-danger btn-sm btn-voidTicket" data-id="${id}" data-ticketNo = '${ticketNo}' data-pnr="${pnr}">Hủy vé</button>
                                 </td>  
                                 <td class="tbcol-action">${action}</td>
                            </tr>`;
                        });
                        $('tbody#TblData').html(rowData);
                        if (parseInt(totalPage) > 1) {
                            Paging.Pagination("#Pagination", totalPage, currentPage, AirOrderController.DataList);
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
                console.log('::' + MessageText.NotService);
            }
        });
    },

    VoidTiket: function (id) {
        var model = {
            ID: id
        };
        AjaxFrom.POST({
            url: URLC + '/VoidTicket',
            data: model,
            success: function (response) {
                if (response !== null) {
                    if (response.status === 200) {
                        Notifization.Success(response.message);
                        AirOrderController.DataList(pageIndex);
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
    ConfirmVoidTiket: function (id) {
        Confirm.ConfirmYN(id, AirOrderController.VoidTiket, Confirm.Text_VoidTicket);
    }
};
//
AirOrderController.init();
// list *******************************************************

$(document).on('change', '#ddlAgentID', function () {
    var option = `<option value="">-MKH-</option>`;
    $('select#ddlCompanyID').html(option);
    $('select#ddlCompanyID').selectpicker('refresh');
    var ddlAgent = $(this).val();
    var model = {
        AgentID: ddlAgent
    };

    //GetTicketing
    AjaxFrom.POST({
        url: '/Management/AirBook/Action/GetCompByAgentID',
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
                        var id = item.ID;
                        var title = item.CodeID;
                        option += `<option value='${id}'>${title}</option>`;
                    });
                    $('select#ddlCompanyID').html(option);
                    $('select#ddlCompanyID').selectpicker('refresh');
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
//*******************************************************
$(document).on("click", ".btn-voidTicket", function () {
    var id = $(this).data("id");
    AirOrderController.ConfirmVoidTiket(id);
})
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

