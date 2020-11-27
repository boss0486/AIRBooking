var pageIndex = 1;
var URLC = "/Management/VNAReport/Action";
var URLA = "/Management/VNAReport";
var VNAReportController = {
    init: function () {
        VNAReportController.registerEvent();
    },
    registerEvent: function () {
        $('#btnSearch').off('click').on('click', function () {
            VNAReportController.DataList(1);
        });
        $('#btnEprSearch').off('click').on('click', function () {
            VNAReportController.EprSearch(1);
        });
    },
    DataList: function (page) {
        //
        var ddlTimeExpress = $('#ddlTimeExpress').val();
        var txtStartDate = $('#txtStartDate').val();
        var txtEndDate = $('#txtEndDate').val();
        var model = {
            Query: $('#txtQuery').val(),
            Page: page,
            TimeExpress: parseInt(ddlTimeExpress),
            StartDate: txtStartDate,
            EndDate: txtEndDate,
            TimeZoneLocal: LibDateTime.GetTimeZoneByLocal(),
            Status: -1
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
                            //  role
                            var action = HelperModel.RolePermission(result.role, "VNAReportController", id);
                            //
                            var reportDate = item.ReportDate;

                            var rowNum = parseInt(index) + (parseInt(currentPage) - 1) * parseInt(pageSize);
                            rowData += `
                                    <tr>
                                         <td class="text-right">${rowNum}&nbsp;</td>                             
                                         <td>${item.EmployeeNumber}-${item.DocumentNumber}</td>                                                               
                                         <td class="tbcol-none">${item.PnrLocator}</td>
                                         <td class="tbcol-none">${item.PassengerName}</td>
                                         <td class="tbcol-none">${reportDate}</td>
                                         <td class="text-right bg-danger">${item.TransactionTime}</td>
                                         <td class="text-right bg-danger">${LibCurrencies.FormatToCurrency(item.FareAmount)} đ</td>
                                         <td class="text-right bg-danger">${LibCurrencies.FormatToCurrency(item.TaxAmount)} đ</td>
                                         <td class="text-right bg-danger">${LibCurrencies.FormatToCurrency(item.TotalAmount)} đ</td>
                                         <td class="tbcol-action">${action} </td>
                                    </tr>`;
                        });
                        $('tbody#TblData').html(rowData);
                        if (parseInt(totalPage) > 1) {
                            Paging.Pagination("#Pagination", totalPage, currentPage, VNAReportController.DataList);
                        }
                        return;
                    }
                    else {
                        //Notifization.Error(result.message);
                        console.log('::' + result.message);
                        return;
                    }
                }
                //Message.Error(MessageText.NOTSERVICES);
                return;
            },
            error: function (result) {
                console.log('::' + MessageText.NotService);
            }
        });
    },
    EprSearch: function (page) {
        var ddlTimeExpress = $('#ddlTimeExpress').val();
        var txtStartDate = $('#txtStartDate').val();
        var txtEndDate = $('#txtEndDate').val();
        var currentStatus = $('#ddlCurrStatus').val();
        var model = {
            Query: $('#txtQuery').val(),
            Page: page,
            TimeExpress: parseInt(ddlTimeExpress),
            StartDate: txtStartDate,
            EndDate: txtEndDate,
            CurrentStatus: currentStatus,
            TimeZoneLocal: LibDateTime.GetTimeZoneByLocal()
        };
        //
        AjaxFrom.POST({
            url: URLC + '/Search',
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
                            //  role
                            var docId = item.DocumentNumber;
                            var reportSaleSummaryId = item.ReportSaleSummaryID;
                            var startTime = item.StartDateTime;
                            var endTime = item.EndDateTime;
                            var bookingStatus = item.BookingStatus;
                            var currentStatus = item.CurrentStatus;
                            //
                            var action = HelperModel.RolePermission(result.role, "VNAReportController", reportSaleSummaryId);
                            //
                            var reportDate = item.ReportDate;

                            var _bookingStatus = EPRReportModel.BookingStatus(bookingStatus);
                            var _currentStatus = EPRReportModel.CurrStatus(currentStatus); 
                            var rowNum = parseInt(index) + (parseInt(currentPage) - 1) * parseInt(pageSize);
                            rowData += `
                                    <tr>
                                         <td class="text-right">${rowNum}&nbsp;</td>                                                                                       
                                         <td class="tbcol-none">${item.PnrLocator} / ${item.FareBasis}</td>
                                         <td class="tbcol-none">${item.PassengerName}</td>
                                         <td class="tbcol-none">${docId}</td>
                                         <td class="text-right">${startTime}</td>
                                         <td class="text-center">${_bookingStatus}</td>
                                         <td class="text-center">${_currentStatus}</td>
                                         <td class="tbcol-action">${action} </td>
                                    </tr>`;
                        });
                        $('tbody#TblData').html(rowData);
                        if (parseInt(totalPage) > 1) {
                            Paging.Pagination("#Pagination", totalPage, currentPage, VNAReportController.EprSearch);
                        }
                        return;
                    }
                    else {
                        //Notifization.Error(result.message);
                        console.log('::' + result.message);
                        return;
                    }
                }
                //Message.Error(MessageText.NOTSERVICES);
                return;
            },
            error: function (result) {
                console.log('::' + MessageText.NotService);
            }
        });
    },
};
VNAReportController.init();


class EPRReportModel {
    static CurrStatus(_status) {
        var result = '';
        switch (_status) {
            case "OK":
                result = `<a class="btn btn-success btn-sm" style='width:100%;'>OK</a>`;
                break;
            case "CKIN":
                result = `<a class="btn btn-warning btn-sm" style='width:100%;'>CKIN</a>`;
                break;
            case "LFTD":
                result = `<a class="btn btn-green btn-sm" style='width:100%;'>LFTD</a>`;
                break;
            case "USED":
                result = `<a class="btn btn-primary btn-sm" style='width:100%;'>USED</a>`;
                break;
            case "NOGO":
                result = `<a class="btn btn-secondary btn-sm" style='width:100%;'>NOGO</a>`;
                break;
            case "VOID":
                result = `<a class="btn btn-danger btn-sm" style='width:100%;'>VOID</a>`;
                break;
            case "RFND":
                result = `<a class="btn btn-info btn-sm" style='width:100%;'>RFND</a>`;
                break;
            case "EXCH":
                result = `<a class="btn btn-orange btn-sm" style='width:100%;'>EXCH</a>`;
                break;
            default:
                result = "";
                break;
        }
        return result;
    }
       
    static BookingStatus(_status) {
        var result = '';
        switch (_status) {
            case "OK":
                result = `<a class="btn btn-success btn-sm" style='width:100%;'>OK</a>`;
                break;
            case "HK":
                result = `<a class="btn btn-warning btn-sm" style='width:100%;'>HK</a>`;
                break;
            case "KL":
                result = `<a class="btn btn-green btn-sm" style='width:100%;'>KL</a>`;
                break;
            case "UC":
                result = `<a class="btn btn-primary btn-sm" style='width:100%;'>UC</a>`;
                break;
            case "GN":
                result = `<a class="btn btn-secondary btn-sm" style='width:100%;'>GN</a>`;
                break;
            case "JL":
                result = `<a class="btn btn-danger btn-sm" style='width:100%;'>JL</a>`;
                break;
            case "HL":
                result = `<a class="btn btn-info btn-sm" style='width:100%;'>HL</a>`;
                break;
            case "WK":
                result = `<a class="btn btn-orange btn-sm" style='width:100%;'>WK</a>`;
            case "SC":
                result = `<a class="btn btn-orange btn-sm" style='width:100%;'>SC</a>`;
                break;
            case "NS":
                result = `<a class="btn btn-default btn-sm" style='width:100%;'>NS</a>`;
                break;
            case "RQ":
                result = `<a class="btn btn-default btn-sm" style='width:100%;'>RQ</a>`;
                break;
            default:
                result = _status;
                break;
        }
        return result;
    }
}
