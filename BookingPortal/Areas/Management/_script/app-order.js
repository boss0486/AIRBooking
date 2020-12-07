﻿var pageIndex = 1;
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
                            var pnr = item.PNR;
                            var orderDate = item.OrderDate;
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
                            var agentPrice = item.AgentPrice;
                            var agentFee = item.AgentFee;
                            var providerFee = item.ProviderFee;
                            //   
                            var _unit = 'đ';
                            //  role
                            var action = HelperModel.RolePermission(result.role, "FlightController", id);
                            //
                            var rowNum = parseInt(index) + (parseInt(currentPage) - 1) * parseInt(pageSize);
                            rowData += `
                            <tr>
                                 <td class="text-right">${rowNum}&nbsp;</td>  
                                 <td class=''>${orderDate}</td>  
                                 <td class='text-center'>${agentCode}</td>  
                                 <td class=''>${customerTypeText}</td>          
                                 <td class='text-center bg-success'>${airlineId}</td>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                
                                 <td class='tbcol-left'>00011</td>                                  
                                 <td class='text-right bg-yellow-1'>${LibCurrencies.FormatToCurrency(fareBasic)} ${_unit}</td>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     
                                 <td class='text-right bg-yellow-1'>${LibCurrencies.FormatToCurrency(fareTax)} ${_unit}</td>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     
                                 <td class='text-right bg-yellow-1'>${LibCurrencies.FormatToCurrency(fareTax)} ${_unit}</td>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     
                                 <td class='text-right bg-success'>${LibCurrencies.FormatToCurrency(agentPrice)} ${_unit}</td>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     
                                 <td class='text-right bg-success'>${LibCurrencies.FormatToCurrency(providerFee)} ${_unit}</td>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     
                                 <td class='text-right bg-success'>${LibCurrencies.FormatToCurrency(agentFee)} ${_unit}</td>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     
                                 <td class='text-right'>${LibCurrencies.FormatToCurrency(totalAmount)} ${_unit}</td>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     
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
                Notifization.Error(MessageText.NOTSERVICES);
                return;
            },
            error: function (result) {
                console.log('::' + MessageText.NOTSERVICES);
            }
        });
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
$(document).on("click", ".btn-export", function () {
    var id = $(this).data("id");
    console.log(":::" + id);
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

