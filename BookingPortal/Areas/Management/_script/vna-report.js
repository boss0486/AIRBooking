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
        $('#btnEprExport').off('click').on('click', function () {
            var ddlTimeExpress = $('#ddlTimeExpress').val();
            var txtStartDate = $('#txtStartDate').val();
            var txtEndDate = $('#txtEndDate').val();
            var currentStatus = $('#ddlCurrStatus').val();
            var model = {
                Query: $('#txtQuery').val(),
                Page: 0,
                TimeExpress: parseInt(ddlTimeExpress),
                StartDate: txtStartDate,
                EndDate: txtEndDate,
                CurrentStatus: currentStatus,
                TimeZoneLocal: LibDateTime.GetTimeZoneByLocal()
            };
            //
            AjaxFrom.POST({
                url: URLC + '/EprExport',
                data: model,
                success: function (result) {
                    $('tbody#TblData').html('');
                    $('#Pagination').html('');
                    if (result !== null) {
                        if (result.status === 200) {
                            //
                            HelperModel.Download(result.path);
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
                result = `<span class="btn btn-success btn-sm" style='width:100%;'>OK</span>`;
                break;
            case "CKIN":
                result = `<span class="btn btn-warning btn-sm" style='width:100%;'>CKIN</span>`;
                break;
            case "LFTD":
                result = `<span class="btn btn-green btn-sm" style='width:100%;'>LFTD</span>`;
                break;
            case "USED":
                result = `<span class="btn btn-primary btn-sm" style='width:100%;'>USED</span>`;
                break;
            case "NOGO":
                result = `<span class="btn btn-secondary btn-sm" style='width:100%;'>NOGO</span>`;
                break;
            case "VOID":
                result = `<span class="btn btn-danger btn-sm" style='width:100%;'>VOID</span>`;
                break;
            case "RFND":
                result = `<span class="btn btn-info btn-sm" style='width:100%;'>RFND</span>`;
                break;
            case "EXCH":
                result = `<span class="btn btn-orange btn-sm" style='width:100%;'>EXCH</span>`;
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
                result = `<span class="btn btn-success btn-sm" style='width:100%;'>OK</span>`;
                break;
            case "HK":
                result = `<span class="btn btn-warning btn-sm" style='width:100%;'>HK</span>`;
                break;
            case "KL":
                result = `<span class="btn btn-green btn-sm" style='width:100%;'>KL</span>`;
                break;
            case "UC":
                result = `<span class="btn btn-primary btn-sm" style='width:100%;'>UC</span>`;
                break;
            case "GN":
                result = `<span class="btn btn-secondary btn-sm" style='width:100%;'>GN</span>`;
                break;
            case "JL":
                result = `<span class="btn btn-danger btn-sm" style='width:100%;'>JL</span>`;
                break;
            case "HL":
                result = `<span class="btn btn-info btn-sm" style='width:100%;'>HL</span>`;
                break;
            case "WK":
                result = `<span class="btn btn-orange btn-sm" style='width:100%;'>WK</span>`;
            case "SC":
                result = `<span class="btn btn-orange btn-sm" style='width:100%;'>SC</span>`;
                break;
            case "NS":
                result = `<span class="btn btn-default btn-sm" style='width:100%;'>NS</span>`;
                break;
            case "RQ":
                result = `<span class="btn btn-default btn-sm" style='width:100%;'>RQ</span>`;
                break;
            default:
                result = _status;
                break;
        }
        return result;
    }
}
