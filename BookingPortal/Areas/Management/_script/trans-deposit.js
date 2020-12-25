var pageIndex = 1;
var URLC = "/Management/TransactionDeposit/Action";
var URLA = "/Management/TransactionDeposit";
var _TransactionDepositController = {
    init: function () {
        _TransactionDepositController.registerEvent();
    },
    registerEvent: function () {
        $('#btnDeposit').off('click').on('click', function () {
            var flg = true;
            //var ddlSupplier = $('#ddlSupplier').val();
            var ddlAgentReceived = $('#ddlAgentReceived').val();
            var txtTransactionCode = $('#txtTransactionCode').val();
            var ddlBankSend = $('#ddlBankSend').val();
            var txtBankSendNumber = $('#txtBankSendNumber').val();
            var ddlBankReceived = $('#ddlBankReceived').val();
            var txtBankReceivedNumber = $('#txtBankReceivedNumber').val();
            var txtAmount = $('#txtAmount').val();
            var txtReceivedDate = $('#txtReceivedDate').val();
            var txtTitle = $('#txtTitle').val();
            var txtSummary = $('#txtSummary').val();
            //
            //if (HelperModel.AccessInApplication != RoleEnum.IsAdminSupplierLogged && HelperModel.AccessInApplication != RoleEnum.IsSupplierLogged) {
            //    //
            //    if (ddlSupplier === "") {
            //        $('#lblSupplier').html('Vui lòng chọn nhà cung cấp');
            //        flg = false;
            //    }
            //    else {
            //        $('#lblSupplier').html('');
            //    }
            //    //
            //}
            //

            if (ddlAgentReceived === "") {
                $('#lblAgentReceived').html('Vui lòng chọn đại lý');
                flg = false;
            }
            else {
                $('#lblAgentReceived').html('');
            }
            // transaction id
            if (txtTransactionCode === '') {
                $('#lblTransactionCode').html('Không được để trống mã giao dịch');
                flg = false;
            }
            else if (!FormatKeyword.test(txtTransactionCode)) {
                $('#lblTransactionCode').html('Mã giao dịch không hợp lệ');
                flg = false;
            }
            else {
                $('#lblTransactionCode').html('');
            }

            // bank sent
            if (ddlBankSend === "") {
                $('#lblBankSend').html('Vui lòng chọn ngân hàng chuyển');
                flg = false;
            }
            else {
                $('#lblBankSend').html('');
            }
            //
            if (txtBankSendNumber === '') {
                $('#lblBankSendNumber').html('Không được để trống số TK Ng.Hàng chuyển');
                flg = false;
            }
            else if (!FormatKeyword.test(txtBankSendNumber)) {
                $('#lblBankSendNumber').html('Số TK Ng.Hàng chuyển không hợp lệ');
                flg = false;
            }
            else {
                $('#lblBankSendNumber').html('');
            }

            // bank receive
            if (ddlBankReceived === "") {
                $('#lblBankReceived').html('Vui lòng chọn ngân hàng nhận');
                flg = false;
            }
            else {
                $('#lblBankReceived').html('');
            }
            //
            if (txtBankReceivedNumber === '') {
                $('#lblBankReceivedNumber').html('Không được để trống số T.Khoản Ng.Hàng nhận');
                flg = false;
            }
            else if (!FormatKeyword.test(txtBankReceivedNumber)) {
                $('#lblBankReceivedNumber').html('Số TK Ng.Hàng nhân không hợp lệ');
                flg = false;
            }
            else {
                $('#lblBankReceivedNumber').html('');
            }

            //
            if (txtAmount === '') {
                $('#lblAmount').html('Không được để trống số tiền nạp');
                flg = false;
            }
            else {
                txtAmount = LibCurrencies.ConvertToCurrency(txtAmount);
                if (!FormatCurrency.test(txtAmount)) {
                    $('#lblAmount').html('Số tiền nạp không hợp lệ');
                    flg = false;
                }
                else if (parseFloat(txtAmount) <= 0) {
                    $('#lblAmount').html('Số tiền nạp phải > 0');
                    flg = false;
                }
                else {
                    $('#lblAmount').html('');
                }
            }
            // lblReceivedDate
            if (txtReceivedDate === '') {
                $('#lblReceivedDate').html('Không được để trống ngày nhận tiền');
                flg = false;
            }
            else if (!FormatDateVN.test(txtReceivedDate)) {
                $('#lblReceivedDate').html('Ngày nhận tiền không hợp lệ');
                flg = false;
            }
            else {
                $('#lblReceivedDate').html('');
            }
            //
            if (txtTitle === '') {
                $('#lblTitle').html('Không được để trống tiêu đề');
                flg = false;
            }
            else if (txtTitle.length < 1 || txtTitle.length > 80) {
                $('#lblTitle').html('Tiêu đề giới hạn từ 1-> 80 ký tự');
                flg = false;
            }
            else if (!FormatKeyword.test(txtTitle)) {
                $('#lblTitle').html('Tiêu đề không hợp lệ');
                flg = false;
            }
            else {
                $('#lblTitle').html('');
            }

            if (txtSummary !== '') {
                if (txtSummary.length > 120) {
                    $('#lblSummary').html('Mô tả giới hạn từ 1-> 120 ký tự');
                    flg = false;
                }
                else if (!FormatKeyword.test(txtSummary)) {
                    $('#lblSummary').html('Mô tả không hợp lệ');
                    flg = false;
                }
                else {
                    $('#lblSummary').html('');
                }
            }
            else {
                $('#lblSummary').html('');
            }
            // submit

            if (flg) {
                _TransactionDepositController.Deposit();
            }
            else {
                Notifization.Error(MessageText.Datamissing);
            }
        });
        $('#btnSearch').off('click').on('click', function () {
            _TransactionDepositController.DataList(1);
        });
        $('#btnUpdate').off('click').on('click', function () {
            var flg = true;
            var ddlAgentReceived = $('#ddlAgentReceived').val();
            var txtTransactionCode = $('#txtTransactionCode').val();
            var ddlBankSend = $('#ddlBankSend').val();
            var txtBankSendNumber = $('#txtBankSendNumber').val();
            var ddlBankReceived = $('#ddlBankReceived').val();
            var txtBankReceivedNumber = $('#txtBankReceivedNumber').val();
            var txtAmount = $('#txtAmount').val();
            var txtReceivedDate = $('#txtReceivedDate').val();
            var txtTitle = $('#txtTitle').val();
            var txtSummary = $('#txtSummary').val();
            //
            if (ddlAgentReceived === "") {
                $('#lblAgentReceived').html('Vui lòng chọn đại lý');
                flg = false;
            }
            else {
                $('#lblAgentReceived').html('');
            }
            // transaction id
            if (txtTransactionCode === '') {
                $('#lblTransactionCode').html('Không được để trống mã giao dịch');
                flg = false;
            }
            else if (!FormatKeyword.test(txtTransactionCode)) {
                $('#lblTransactionCode').html('Mã giao dịch không hợp lệ');
                flg = false;
            }
            else {
                $('#lblTransactionCode').html('');
            }

            // bank sent
            if (ddlBankSend === "") {
                $('#lblBankSend').html('Vui lòng chọn ngân hàng chuyển');
                flg = false;
            }
            else {
                $('#lblBankSend').html('');
            }
            //
            if (txtBankSendNumber === '') {
                $('#lblBankSendNumber').html('Không được để trống số TK Ng.Hàng chuyển');
                flg = false;
            }
            else if (!FormatKeyword.test(txtBankSendNumber)) {
                $('#lblBankSendNumber').html('Số TK Ng.Hàng chuyển không hợp lệ');
                flg = false;
            }
            else {
                $('#lblBankSendNumber').html('');
            }

            // bank receive
            if (ddlBankReceived === "") {
                $('#lblBankReceived').html('Vui lòng chọn ngân hàng nhận');
                flg = false;
            }
            else {
                $('#lblBankReceived').html('');
            }
            //
            if (txtBankReceivedNumber === '') {
                $('#lblBankReceivedNumber').html('Không được để trống số TK Ng.Hàng nhận');
                flg = false;
            }
            else if (!FormatKeyword.test(txtBankReceivedNumber)) {
                $('#lblBankReceivedNumber').html('Số TK Ng.Hàng nhận không hợp lệ');
                flg = false;
            }
            else {
                $('#lblBankReceivedNumber').html('');
            }

            //
            if (txtAmount === '') {
                $('#lblAmount').html('Không được để trống số tiền nạp');
                flg = false;
            }
            else {
                txtAmount = LibCurrencies.ConvertToCurrency(txtAmount);
                if (!FormatCurrency.test(txtAmount)) {
                    $('#lblAmount').html('Số tiền nạp không hợp lệ');
                    flg = false;
                }
                else if (parseFloat(txtAmount) <= 0) {
                    $('#lblAmount').html('Số tiền nạp phải > 0');
                    flg = false;
                }
                else {
                    $('#lblAmount').html('');
                }
            }

            // lblReceivedDate
            if (txtReceivedDate === '') {
                $('#lblReceivedDate').html('Không được để trống ngày nhận tiền');
                flg = false;
            }
            else if (!FormatDateVN.test(txtReceivedDate)) {
                $('#lblReceivedDate').html('Ngày nhận tiền không hợp lệ');
                flg = false;
            }
            else {
                $('#lblReceivedDate').html('');
            }
            //
            if (txtTitle === '') {
                $('#lblTitle').html('Không được để trống tiêu đề');
                flg = false;
            }
            else if (txtTitle.length < 1 || txtTitle.length > 80) {
                $('#lblTitle').html('Tiêu đề giới hạn từ 1-> 80 ký tự');
                flg = false;
            }
            else if (!FormatKeyword.test(txtTitle)) {
                $('#lblTitle').html('Tiêu đề không hợp lệ');
                flg = false;
            }
            else {
                $('#lblTitle').html('');
            }

            if (txtSummary !== '') {
                if (txtSummary.length > 120) {
                    $('#lblSummary').html('Mô tả giới hạn từ 1-> 120 ký tự');
                    flg = false;
                }
                else if (!FormatKeyword.test(txtSummary)) {
                    $('#lblSummary').html('Mô tả không hợp lệ');
                    flg = false;
                }
                else {
                    $('#lblSummary').html('');
                }
            }
            else {
                $('#lblSummary').html('');
            }
            // submit
            if (flg) {
                _TransactionDepositController.Update();
            }
            else {
                Notifization.Error(MessageText.Datamissing);
            }
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
                            //
                            var title = item.Title;
                            var receivedId = item.ReceivedID;
                            var transactionCode = item.TransactionCode;
                            var amount = item.Amount;
                            var sentCode = item.AgentSentCode;
                            var receivedCode = item.AgentReceivedCode;
                            var amount = item.Amount;
                            var createdBy = item.CreatedBy;

                            //  role
                            var action = HelperModel.RolePermission(result.role, "_TransactionDepositController", id);
                            //
                            var rowNum = parseInt(index) + (parseInt(currentPage) - 1) * parseInt(pageSize);
                            rowData += `
                            <tr>
                                 <td class="text-right">${rowNum}&nbsp;</td>
                                 <td>${title}</td>                                    
                                 <td>${transactionCode}</td>                                    
                                 <td>${sentCode}</td>                                    
                                 <td>${receivedCode}</td>                                    
                                 <td class="text-right"> ${LibCurrencies.FormatToCurrency(amount)} đ</td>                                    
                                 <td>${createdBy}</td>                                    
                                 <td class="text-center">${HelperModel.StatusIcon(item.Enabled)}</td>
                                 <td class="text-center">${item.CreatedDate}</td>
                                 <td class="tbcol-action">${action}</td>
                            </tr>`;
                        });
                        $('tbody#TblData').html(rowData);
                        if (parseInt(totalPage) > 1) {
                            Paging.Pagination("#Pagination", totalPage, currentPage, _TransactionDepositController.DataList);
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
    Deposit: function () {
        //var ddlSupplier = $('#ddlSupplier').val();
        //if (ddlSupplier == undefined || ddlSupplier == null) {
        //    ddlSupplier = '';
        //}
        var ddlAgentReceived = $('#ddlAgentReceived').val();
        var txtTransactionCode = $('#txtTransactionCode').val();
        var ddlBankSend = $('#ddlBankSend').val();
        var txtBankSendNumber = $('#txtBankSendNumber').val();
        var ddlBankReceived = $('#ddlBankReceived').val();
        var txtBankReceivedNumber = $('#txtBankReceivedNumber').val();
        var txtAmount = LibCurrencies.ConvertToCurrency($('#txtAmount').val());
        var txtReceivedDate = $('#txtReceivedDate').val();
        var txtTitle = $('#txtTitle').val();
        var txtSummary = $('#txtSummary').val();
        //
        var model = {

            Summary: txtSummary,
            //AgentSentID: ddl,
            //AgentSentCode:,
            AgentReceivedID: ddlAgentReceived,
            TransactionCode: txtTransactionCode,
            BankSentID: ddlBankSend,
            BankSentNumber: txtBankSendNumber,
            BankReceivedID: ddlBankReceived ,
            BankReceivedNumber: txtBankReceivedNumber,
            ReceivedDate: txtReceivedDate,
            Amount: txtAmount,
            Enabled: 1,
        };
        AjaxFrom.POST({
            url: URLC + '/deposit',
            data: model,
            success: function (response) {
                if (response !== null) {
                    if (response.status === 200) {
                        Notifization.Success(response.message);
                        FData.ResetForm();
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
    GetCustomerBySupplierID: function (supplierId, _id, isChangeEvent) {
        var option = `<option value="">-Lựa chọn-</option>`;
        $('#ddlAgentReceived').html(option);
        $('#ddlAgentReceived').selectpicker('refresh');
        var model = {
            ID: supplierId
        };
        AjaxFrom.POST({
            url: '/Management/Customer/Action/GetCustomer-By-SuplierID',
            data: model,
            async: true,
            success: function (result) {
                if (result !== null) {
                    if (result.status === 200) {

                        var attrSelect = '';
                        $.each(result.data, function (index, item) {
                            var id = item.ID;
                            var codeid = item.CodeID;
                            if (_id !== undefined && _id != "" && _id === item.ID) {
                                attrSelect = "selected";
                            }
                            else {
                                attrSelect = '--';
                            }
                            option += `<option value='${id}' data-codeid='${codeid}' ${attrSelect}>${item.Title}</option>`;
                        });
                        //
                        $('#ddlAgentReceived').html(option);
                        setTimeout(function () {
                            $('#ddlAgentReceived').selectpicker('refresh');
                            if (isChangeEvent !== undefined && isChangeEvent == true && attrSelect !== '') {
                                $('#ddlAgentReceived').change();
                            }
                        }, 1000);
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
    }
};

_TransactionDepositController.init();
// supplier
$(document).on("change", "#ddlSupplier", function () {
    var ddlSupplier = $(this).val();
    if (ddlSupplier === "") {
        $('#lblSupplier').html('Vui lòng chọn nhà cung cấp');
        $('#lblSupplierCodeID').html('');
    }
    else {
        $('#lblSupplier').html('');
        var codeid = $(this).find(':selected').data('codeid');
        $('#lblSupplierCodeID').html(codeid);
        // load customer
        _TransactionDepositController.GetCustomerBySupplierID(ddlSupplier, "", false);
    }
});

// customer
$(document).on("change", "#ddlAgentReceived", function () {
    var ddlAgentReceived = $(this).val();
    if (ddlAgentReceived === "") {
        $('#lblAgentReceived').html('Vui lòng chọn đại lý');
    }
    else {
        $('#lblAgentReceived').html('');
    } 
    $('#lblAgentReceivedCode').html('');
    var codeid = $(this).find(':selected').data('codeid');
    $('#lblAgentReceivedCode').html(codeid);
        // load customer

});
$(document).on("keyup", "#txtAmount", function () {
    var txtAmount = $(this).val();
    if (txtAmount === '') {
        $('#lblAmount').html('Không được để trống số tiền nạp');
    }
    else {
        txtAmount = LibCurrencies.ConvertToCurrency(txtAmount);
        if (!FormatCurrency.test(txtAmount)) {
            $('#lblAmount').html('Số tiền nạp không hợp lệ');
        }
        else if (parseFloat(txtAmount) <= 0) {
            $('#lblAmount').html('Số tiền nạp phải > 0');
        }
        else {
            $('#lblAmount').html('');
        }
    }
});

// ReceivedDate
$(document).on("keyup", "#txtReceivedDate", function () {
    var txtReceivedDate = $(this).val();
    if (txtReceivedDate === '') {
        $('#lblReceivedDate').html('Không được để trống ngày nhận tiền');
    }
    else if (!FormatDateVN.test(txtReceivedDate)) {
        $('#lblReceivedDate').html('Ngày nhận tiền không hợp lệ');
    }
    else {
        $('#lblReceivedDate').html('');
    }
    //
});

// transaction 
$(document).on('keyup', '#txtTransactionCode', function () {
    var txtTransactionCode = $(this).val();
    if (txtTransactionCode === '') {
        $('#lblTransactionCode').html('Không được để trống mã giao dịch');
    }
    else if (!FormatKeyword.test(txtTransactionCode)) {
        $('#lblTransactionCode').html('Mã giao dịch không hợp lệ');
    }
    else {
        $('#lblTransactionCode').html('');
    }
});
// 
$(document).on("change", "#ddlBankSend", function () {
    var ddlBankSend = $(this).val();
    if (ddlBankSend === "") {
        $('#lblBankSend').html('Vui lòng chọn ngân hàng chuyển');
    }
    else {
        $('#lblBankSend').html('');
    }
});

$(document).on('keyup', '#txtBankSendNumber', function () {
    var txtBankSendNumber = $(this).val();
    if (txtBankSendNumber === '') {
        $('#lblBankSendNumber').html('Không được để trống số TK Ng.Hàng chuyển');
    }
    else if (!FormatKeyword.test(txtBankSendNumber)) {
        $('#lblBankSendNumber').html('Số TK Ng.Hàng chuyển không hợp lệ');
    }
    else {
        $('#lblBankSendNumber').html('');
    }
});
// bank receive
$(document).on("change", "#ddlBankReceived", function () {
    var ddlBankReceived = $(this).val();
    if (ddlBankReceived === "") {
        $('#lblBankReceived').html('Vui lòng chọn ngân hàng nhận');
    }
    else {
        $('#lblBankReceived').html('');
    }
});
$(document).on('keyup', '#txtBankReceivedNumber', function () {
    var txtBankReceivedNumber = $(this).val();
    if (txtBankReceivedNumber === '') {
        $('#lblBankReceivedNumber').html('Không được để trống số TK Ng.Hàng nhận');
    }
    else if (!FormatKeyword.test(txtBankReceivedNumber)) {
        $('#lblBankReceivedNumber').html('Số TK Ng.Hàng nhân không hợp lệ');
    }
    else {
        $('#lblBankReceivedNumber').html('');
    }
});
// title
$(document).on('keyup', '#txtTitle', function () {
    var txtTitle = $(this).val();
    if (txtTitle === '') {
        $('#lblTitle').html('Không được để trống tiêu đề');
    }
    else if (txtTitle.length < 1 || txtTitle.length > 80) {
        $('#lblTitle').html('Tiêu đề giới hạn từ 1-> 80 ký tự');
    }
    else if (!FormatKeyword.test(txtTitle)) {
        $('#lblTitle').html('Tiêu đề không hợp lệ');
    }
    else {
        $('#lblTitle').html('');
    }
});
// summary
$(document).on('keyup', '#txtSummary', function () {
    var txtSummary = $(this).val();
    if (txtSummary !== '') {
        if (txtSummary.length > 120) {
            $('#lblSummary').html('Mô tả giới hạn từ 1-> 120 ký tự');
            flg = false;
        }
        else if (!FormatKeyword.test(txtSummary)) {
            $('#lblSummary').html('Mô tả không hợp lệ');
            flg = false;
        }
        else {
            $('#lblSummary').html('');
        }
    }
    else {
        $('#lblSummary').html('');
    }
});