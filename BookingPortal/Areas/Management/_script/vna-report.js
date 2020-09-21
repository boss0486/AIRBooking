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
            StartDate: LibDateTime.FormatToServerDate(txtStartDate),
            EndDate: LibDateTime.FormatToServerDate(txtEndDate),
            TimeZoneLocal: LibDateTime.GetTimeZoneByLocal(),
            Status: parseInt($('#ddlStatus').val())
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
                            var action = HelperModel.RolePermission(result.role, "AccountController", id);
                            //
                            var rowNum = parseInt(index) + (parseInt(currentPage) - 1) * parseInt(pageSize);
                            var transFee = item.SaleSummaryTransactionSSFop;

                            rowData += `
                                    <tr>
                                         <td class="text-right">${rowNum}&nbsp;</td>                             
                                         <td>${item.EmployeeNumber}-${item.DocumentNumber}</td>                                                               
                                         <td class="tbcol-none">${item.PassengerName}</td>
                                         <td class="tbcol-none">${item.PnrLocator}</td>
                                         <td class="tbcol-none">${item.TicketPrinterLniata}</td>
                                         <td class="text-right" style = "background: #fee1ee;">${item.TransactionTime}</td>
                                         <td class="text-right" style = "background: #fee1ee;">${LibCurrencies.FormatToCurrency(transFee.FareAmount)} đ</td>
                                         <td class="tbcol-none report-trans" style = "background: #fee1ee;">${transFee.FopCode}</td>
                                         <td class="text-right" style = "background: #fee1ee;">${LibCurrencies.FormatToCurrency(transFee.TaxAmount)} đ</td>
                                         <td class="text-right" style = "background: #fee1ee;">${LibCurrencies.FormatToCurrency(transFee.TotalAmount)} đ</td>
                                    </tr>`;
                            _stt += 1;
                        });
                        $('tbody#TblData').html(rowData);
                        if (parseInt(totalPage) > 1) {
                            Paging.Pagination("#Pagination", totalPage, currentPage, AccountController.DataList);
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


