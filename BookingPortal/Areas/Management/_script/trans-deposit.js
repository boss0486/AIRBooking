var pageIndex = 1;
var URLC = "/Management/TransactionCustomerDeposit/Action";
var URLA = "/Management/TransactionCustomerDeposit";
var _TransactionDepositController = {
    init: function () {
        _TransactionDepositController.registerEvent();
    },
    registerEvent: function () {
        $('#btnCreate').off('click').on('click', function () {
            var flg = true;
            var ddlCustomer = $('#ddlCustomer').val();
            var txtTransactionID = $('#txtTransactionID').val();
            var ddlBankSent = $('#ddlBankSent').val();
            var txtBankIDSent = $('#txtBankIDSent').val();
            var ddlBankReceived = $('#ddlBankReceived').val();
            var txtBankIDReceived = $('#txtBankIDReceived').val();
            var txtAmount = $('#txtAmount').val();
            var txtReceivedDate = $('#txtReceivedDate').val();
            var txtTitle = $('#txtTitle').val();
            var txtSummary = $('#txtSummary').val();
            //
            if (ddlCustomer === "") {
                $('#lblCustomer').html('Vui lòng chọn khách hàng');
                flg = false;
            }
            else {
                $('#lblCustomer').html('');
            }
            // transaction id
            if (txtTransactionID === '') {
                $('#lblTransactionID').html('Không được để trống mã giao dịch');
                flg = false;
            }
            else if (!FormatKeyword.test(txtTransactionID)) {
                $('#lblTransactionID').html('Mã giao dịch không hợp lệ');
                flg = false;
            }
            else {
                $('#lblTransactionID').html('');
            }

            // bank sent
            if (ddlBankSent === "") {
                $('#lblBankSent').html('Vui lòng chọn ngân hàng chuyển');
                flg = false;
            }
            else {
                $('#lblBankSent').html('');
            }
            //
            if (txtBankIDSent === '') {
                $('#lblBankIDSent').html('Không được để trống số TK Ng.Hàng chuyển');
                flg = false;
            }
            else if (!FormatKeyword.test(txtBankIDSent)) {
                $('#lblBankIDSent').html('Số TK Ng.Hàng chuyển không hợp lệ');
                flg = false;
            }
            else {
                $('#lblBankIDSent').html('');
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
            if (txtBankIDReceived === '') {
                $('#lblBankIDReceived').html('Không được để trống số T.Khoản Ng.Hàng nhận');
                flg = false;
            }
            else if (!FormatKeyword.test(txtBankIDReceived)) {
                $('#lblBankIDReceived').html('Số TK Ng.Hàng nhân không hợp lệ');
                flg = false;
            }
            else {
                $('#lblBankIDReceived').html('');
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
                _TransactionDepositController.Create();
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
            var ddlCustomer = $('#ddlCustomer').val();
            var txtTransactionID = $('#txtTransactionID').val();
            var ddlBankSent = $('#ddlBankSent').val();
            var txtBankIDSent = $('#txtBankIDSent').val();
            var ddlBankReceived = $('#ddlBankReceived').val();
            var txtBankIDReceived = $('#txtBankIDReceived').val();
            var txtAmount = $('#txtAmount').val();
            var txtReceivedDate = $('#txtReceivedDate').val();
            var txtTitle = $('#txtTitle').val();
            var txtSummary = $('#txtSummary').val();
            //
            if (ddlCustomer === "") {
                $('#lblCustomer').html('Vui lòng chọn khách hàng');
                flg = false;
            }
            else {
                $('#lblCustomer').html('');
            }
            // transaction id
            if (txtTransactionID === '') {
                $('#lblTransactionID').html('Không được để trống mã giao dịch');
                flg = false;
            }
            else if (!FormatKeyword.test(txtTransactionID)) {
                $('#lblTransactionID').html('Mã giao dịch không hợp lệ');
                flg = false;
            }
            else {
                $('#lblTransactionID').html('');
            }

            // bank sent
            if (ddlBankSent === "") {
                $('#lblBankSent').html('Vui lòng chọn ngân hàng chuyển');
                flg = false;
            }
            else {
                $('#lblBankSent').html('');
            }
            //
            if (txtBankIDSent === '') {
                $('#lblBankIDSent').html('Không được để trống số TK Ng.Hàng chuyển');
                flg = false;
            }
            else if (!FormatKeyword.test(txtBankIDSent)) {
                $('#lblBankIDSent').html('Số TK Ng.Hàng chuyển không hợp lệ');
                flg = false;
            }
            else {
                $('#lblBankIDSent').html('');
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
            if (txtBankIDReceived === '') {
                $('#lblBankIDReceived').html('Không được để trống số TK Ng.Hàng nhận');
                flg = false;
            }
            else if (!FormatKeyword.test(txtBankIDReceived)) {
                $('#lblBankIDReceived').html('Số TK Ng.Hàng nhận không hợp lệ');
                flg = false;
            }
            else {
                $('#lblBankIDReceived').html('');
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
                            //
                            var customerId = item.CustomerID;
                            var title = item.Title;
                            var summary = item.Summary;
                            var customerId = item.CustomerID;
                            var transactionId = item.TransactionID;
                            var bankSent = item.BankSent;
                            var bankIDSent = item.BankIDSent;
                            var bankReceived = item.BankReceived;
                            var bankIDReceived = item.BankIDReceived;
                            var receivedDate = item.ReceivedDate;
                            var amount = item.Amount;
                            var status = item.Status;
                            var enabled = item.Enabled;

                            //  role
                            var action = HelperModel.RolePermission(result.role, "_TransactionDepositController", id);
                            //
                            var rowNum = parseInt(index) + (parseInt(currentPage) - 1) * parseInt(pageSize);
                            rowData += `
                            <tr>
                                 <td class="text-right">${rowNum}&nbsp;</td>
                                 <td>${title}</td>                                    
                                 <td>${customerId}</td>                                    
                                 <td class="text-right"> ${LibCurrencies.FormatToCurrency(amount)} đ</td>                                    
                                 <td>${summary}</td>                                    
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
    Create: function () {
        var ddlCustomer = $('#ddlCustomer').val();
        var txtTransactionID = $('#txtTransactionID').val();
        var ddlBankSent = $('#ddlBankSent').val();
        var txtBankIDSent = $('#txtBankIDSent').val();
        var ddlBankReceived = $('#ddlBankReceived').val();
        var txtBankIDReceived = $('#txtBankIDReceived').val();
        var txtAmount = LibCurrencies.ConvertToCurrency($('#txtAmount').val());  
        var txtReceivedDate = $('#txtReceivedDate').val();
        var txtTitle = $('#txtTitle').val();
        var txtSummary = $('#txtSummary').val(); 
        //
        var model = {
            CustomerID: ddlCustomer,
            TransactionID: txtTransactionID,
            BankSent: ddlBankSent,
            BankIDSent: txtBankIDSent,
            BankReceived: ddlBankReceived,
            BankReceived: ddlBankReceived,
            BankIDReceived: txtBankIDReceived,
            Amount: txtAmount,
            ReceivedDate: txtReceivedDate,
            Title: txtTitle,
            Summary: txtSummary,
            Enabled: 1
        };
        AjaxFrom.POST({
            url: URLC + '/Create',
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

    }
};

_TransactionDepositController.init();
// customer
$(document).on("change", "#ddlCustomer", function () {
    var ddlCustomer = $(this).val();
    if (ddlCustomer === "") {
        $('#lblCustomer').html('Vui lòng chọn khách hàng');
    }
    else {
        $('#lblCustomer').html('');
    }
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
$(document).on('keyup', '#txtTransactionID', function () {
    var txtTransactionID = $(this).val();
    if (txtTransactionID === '') {
        $('#lblTransactionID').html('Không được để trống mã giao dịch');
    }
    else if (!FormatKeyword.test(txtTransactionID)) {
        $('#lblTransactionID').html('Mã giao dịch không hợp lệ');
    }
    else {
        $('#lblTransactionID').html('');
    }
});
// 

$(document).on("change", "#ddlBankSent", function () {
    var ddlBankSent = $(this).val();
    if (ddlBankSent === "") {
        $('#lblBankSent').html('Vui lòng chọn ngân hàng chuyển');
    }
    else {
        $('#lblBankSent').html('');
    }
});

$(document).on('keyup', '#txtBankIDSent', function () {
    var txtBankIDSent = $(this).val();
    if (txtBankIDSent === '') {
        $('#lblBankIDSent').html('Không được để trống số TK Ng.Hàng chuyển');
    }
    else if (!FormatKeyword.test(txtBankIDSent)) {
        $('#lblBankIDSent').html('Số TK Ng.Hàng chuyển không hợp lệ');
    }
    else {
        $('#lblBankIDSent').html('');
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
$(document).on('keyup', '#txtBankIDReceived', function () {
    var txtBankIDReceived = $(this).val();
    if (txtBankIDReceived === '') {
        $('#lblBankIDReceived').html('Không được để trống số TK Ng.Hàng nhận');
    }
    else if (!FormatKeyword.test(txtBankIDReceived)) {
        $('#lblBankIDReceived').html('Số TK Ng.Hàng nhân không hợp lệ');
    }
    else {
        $('#lblBankIDReceived').html('');
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