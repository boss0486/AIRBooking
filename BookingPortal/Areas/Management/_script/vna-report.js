var pageIndex = 1;
var URLC = "/Management/VNAReport/Action";
var URLA = "/Management/VNAReport";
var VNAReportController = {
    init: function () {
        VNAReportController.registerEvent();
    },
    registerEvent: function () {
        $('#btnSearch').off('click').on('click', function () {

            var reportDate = $("#txtReportDate").val();
            if (reportDate == "") {
                Notifization.Error("Không được để trống ngày báo cáo");
                $("#txtReportDate").focus();
                return;
            }
            if (!ValidData.ValidDate(reportDate, "vn")) {
                Notifization.Error("Ngày báo cáo không hợp lệ");
                $("#txtReportDate").focus();
                return;
            }
            VNAReportController.DataList(1);
        });
    },
    DataList: function (page) {
        //
        var reportDate = $("#txtReportDate").val();
        var employeeNumber = $("#txtEmployeeNumber").val();
        var docummentNumber = $("#txtDocummentNumber").val();

        var model = {
            ReportDate: LibDateTime.FormatToServerDate(reportDate),
            EmployeeNumber: employeeNumber,
            DocummentNumber: docummentNumber,
        };
        //
        AjaxFrom.POST({
            url: URLC + '/DataList',
            data: model,
            success: function (result) {
                $('tbody#TblData').html('');
                if (result !== null) {
                    if (result.status === 200) {

                        var rowData = '';
                        // employee
                        var empData = result.data;
                        var _stt = 1;
                        $.each(empData, function (index, item) {


                            var empNumber = item.EmpNumber;
                            var transaction = item.SaleSummaryTransaction;
                            $.each(transaction, function (transIndex, transItem) {

                                transIndex = transIndex + 1;
                                //  role
                                var action = HelperModel.RolePermission(result.role, "VNAReportController", transItem.DocumentNumber);
                                //
                                //var rowNum = parseInt(transIndex) + (parseInt(currentPage) - 1) * parseInt(pageSize);

                                var transFee = transItem.SaleSummaryTransactionSSFop;
                                
                                rowData += `
                                    <tr>
                                         <td class="text-right">${_stt}&nbsp;</td>                             
                                         <td>${empNumber}-${transItem.DocumentNumber}</td>                                                               
                                         <td class="tbcol-none">${transItem.PassengerName}</td>
                                         <td class="tbcol-none">${transItem.PnrLocator}</td>
                                         <td class="tbcol-none">${transItem.TicketPrinterLniata}</td>
                                         <td class="text-right" style = "background: #fee1ee;">${transItem.TransactionTime}</td>
                                         <td class="text-right" style = "background: #fee1ee;">${LibCurrencies.FormatToCurrency(transFee.FareAmount)} đ</td>
                                         <td class="tbcol-none report-trans" style = "background: #fee1ee;">${transFee.FopCode}</td>
                                         <td class="text-right" style = "background: #fee1ee;">${LibCurrencies.FormatToCurrency(transFee.TaxAmount)} đ</td>
                                         <td class="text-right" style = "background: #fee1ee;">${LibCurrencies.FormatToCurrency(transFee.TotalAmount)} đ</td>
                                    </tr>`;
                                _stt += 1;
                            });
                        });

                        $('tbody#TblData').html(rowData);
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
    },
};
VNAReportController.init();


