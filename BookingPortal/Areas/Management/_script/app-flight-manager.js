﻿var pageIndex = 1;
var URLC = "/Management/AirBook/Action";
var URLA = "/Management/AirBook";
var arrFile = [];
//
var BookingManagerController = {
    init: function () {
        BookingManagerController.registerEvent();
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
    DataList: function (page) {
        //   
        var _ariaId = $('#ddlAreaID').val();
        var _province = $('#ddlProvince').val();
        var ddlTimeExpress = $('#ddlTimeExpress').val();
        var txtStartDate = $('#txtStartDate').val();
        var txtEndDate = $('#txtEndDate').val();
        var model = {
            Query: $('#txtQuery').val(),
            Page: page,
            TimeExpress: parseInt(ddlTimeExpress),
            StartDate: LibDateTime.FormatToServerDate(txtStartDate),
            EndDate: LibDateTime.FormatToServerDate(txtEndDate),
            TimeZoneLocal: LibDateTime.GetTimeZoneByLocal(),
            Status: parseInt($('#ddlStatus').val()),
            AreaID: _ariaId,
            ProviceID: _province,
        };
        //
        AjaxFrom.POST({
            url: URLC + '/BookList',
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
                            var codeId = item.CodeID;
                            var airlineId = item.AirlineID;
                            var ticketingName = item.TicketingName;
                            var clientCode = item.ClientCode;
                            var amount = item.TotalAmount;
                            var contactName = item.ContactName;
                            var _unit = 'đ';

                            //  role
                            var action = HelperModel.RolePermission(result.role, "FlightController", id);
                            //
                            var rowNum = parseInt(index) + (parseInt(currentPage) - 1) * parseInt(pageSize);
                            rowData += `
                            <tr>
                                 <td class="text-right">${rowNum}&nbsp;</td>  
                                 <td class='tbcol-photo'>${airlineId} - ${pnr}</td>  
                                 <td class='tbcol-photo'>${codeId}</td>  
                                 <td class='text-left'>${clientCode}-${ticketingName}</td>                                                                                                                                                                                                                                                                         
                                 <td class='text-left'>COM</td>                                                                                                                                                                                                                                                                         
                                 <td class='text-left'>${contactName}</td>                                                                                                                                                                                                                                                                         
                                 <td class='text-right'>${LibCurrencies.FormatToCurrency(amount)} ${_unit}</td>                                                                                                                                                                                                                                                                         
                                 <td class="text-center">${BookOrderStatus(item.Status)}</td>
                                 <td class="text-center">${item.CreatedDate}</td>                                                                                                                                                                                                                                                                       
                                 <td class='tbcol-left'>
                                     <button type="button" class="btn btn-primary btn-sm">Xuất vé</button>
                                 </td>                                  
                                 <td class="tbcol-action">${action}</td>
                            </tr>`;
                        });
                        $('tbody#TblData').html(rowData);
                        if (parseInt(totalPage) > 1) {
                            Paging.Pagination("#Pagination", totalPage, currentPage, BookingManagerController.DataList);
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
BookingManagerController.init();




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

