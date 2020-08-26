var pageIndex = 1;
var URLC = "/Management/Wallet-Customer-Deposit-History/Action";
var URLA = "/Management/WalletCustomerDepositHistory";
var WalletCustomerDepositHistoryController = {
    init: function () {
        WalletCustomerDepositHistoryController.registerEvent();
    },
    registerEvent: function () {
        $('#btnSearch').off('click').on('click', function () {
            WalletCustomerDepositHistoryController.DataList(1);
        });
    },
    DataList: function (page) {
        var today = new Date();
        var dd = String(today.getDate()).padStart(2, '0');
        var mm = String(today.getMonth() + 1).padStart(2, '0'); //January is 0!
        var yyyy = today.getFullYear();
        today = yyyy + "-" + mm + "-" + dd;
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
            ClientTime: today
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
                            var customerId = item.CustomerID;
                            var title = item.Title;
                            var summary = item.Summary;
                            var transactionId = item.TransactionID;
                            var bankSent = item.BankSent;
                            var bankIDSent = item.BankIDSent;
                            var bankReceived = item.BankReceived;
                            var bankIDReceived = item.BankIDReceived;
                            var receivedDate = item.ReceivedDate;
                            var amount = item.Amount;
                            var status = item.Status;
                            var enabled = item.Enabled;

                            title += ` thời gian: ${item.CreatedFullDate} `;
                            // <td>${transactionId}, giao dịch từ N.Hàng ${bankSent} đến N.Hàng ${bankReceived} ${summary}</td>          
                            var rowNum = parseInt(index) + (parseInt(currentPage) - 1) * parseInt(pageSize);
                            rowData += `
                            <tr>
                                 <td class="text-right">${rowNum}&nbsp;</td>
                                 <td>${title}</td>                                    
                                 <td class='text-right'>${LibCurrencies.FormatThousands(amount)} đ</td>                                    
                                 <td class="text-center">${item.CreatedDate}</td>
                            </tr>`;
                        });
                        $('tbody#TblData').html(rowData);
                        if (parseInt(totalPage) > 1) {
                            Paging.Pagination("#Pagination", totalPage, currentPage, WalletCustomerDepositHistoryController.DataList);
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
};

WalletCustomerDepositHistoryController.init();