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
        $('#btnEprReportSearch').off('click').on('click', function () {
            var txtReportDate = $('#txtReportDate').val();
            if (txtReportDate == "") {
                Notifization.Error("Không được để trống ngày báo cáo");
                return;
            }
            if (!FormatDateVN.test(txtReportDate)) {
                Notifization.Error("Ngày báo cáo không hợp lệ");
                return;
            }
            VNAReportController.EprSearch();
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
                            var action = HelperModel.RolePermission(result.role, "AccountController", id);
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
                                         <td class="text-right" style = "background: #fee1ee;">${item.TransactionTime}</td>
                                         <td class="text-right" style = "background: #fee1ee;">${LibCurrencies.FormatToCurrency(item.FareAmount)} đ</td>
                                         <td class="text-right" style = "background: #fee1ee;">${LibCurrencies.FormatToCurrency(item.TaxAmount)} đ</td>
                                         <td class="text-right" style = "background: #fee1ee;">${LibCurrencies.FormatToCurrency(item.TotalAmount)} đ</td>
                                         <td class="tbcol-action">${action} </td>
                                    </tr>`;
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
    EprSearch: function (page) {
        //
        var txtQuery = $('#txtQuery').val();
        var txtReportDate = LibDateTime.FormatToServerDate($('#txtReportDate').val());
        var model = {
            Query: txtQuery,
            ReportDate: txtReportDate
        };
        //
        AjaxFrom.POST({
            url: URLC + '/EPR-Search',
            data: model,
            success: function (result) {
                $('tbody#TblData').html('');
                $('#Pagination').html('');
                if (result !== null) {
                    if (result.status === 200) {
                        var rowData = '';
                        var _stt = 1;
                        //

                        //
                        $.each(result.data, function (indEmp, itemEmp) {

                            var saleSummaryTransaction = itemEmp.SaleSummaryTransaction;

                            if (saleSummaryTransaction != null) {
                                $.each(saleSummaryTransaction, function (indTrans, item) {
                                    var id = item.DocumentNumber;

                                    // 
                                    var fee = item.SaleSummaryTransactionSSFop;
                                    rowData += `
                                    <tr>
                                         <td class="text-right">${_stt}&nbsp;</td>                             
                                         <td>${itemEmp.EmpNumber}-${id}</td>                                                               
                                         <td class="tbcol-none">${item.PnrLocator}</td>
                                         <td class="tbcol-none">${item.PassengerName}</td> 
                                         <td class="text-right" style = "background: #fee1ee;">${item.TransactionTime}</td>
                                         <td class="text-right" style = "background: #fee1ee;">${LibCurrencies.FormatToCurrency(fee.FareAmount)} đ</td>
                                         <td class="text-right" style = "background: #fee1ee;">${LibCurrencies.FormatToCurrency(fee.TaxAmount)} đ</td>
                                         <td class="text-right" style = "background: #fee1ee;">${LibCurrencies.FormatToCurrency(fee.TotalAmount)} đ</td>
                                         <td class="tbcol-action"><a href='${URLA}/RpDetails/${id}' target="_blank"><i class='fas fa-pen-square'></i>&nbsp;Chi tiết</a></td>
                                    </tr>`;
                                    _stt += 1;
                                });
                            }
                        });
                        $('tbody#TblData').html(rowData);
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


