var pageIndex = 1;
var URLC = "/Management/TransactionSpending/Action";
var URLA = "/Management/TransactionSpending";
var TransactionSpendingController = {
    init: function () {
        TransactionSpendingController.registerEvent();
    },
    registerEvent: function () {
        $('#btnCreate').off('click').on('click', function () {
            var flg = true;
            var ddlCustomer = $('#ddlCustomer').val();
            var txtAmount = $('#txtAmount').val();
            var txtSummary = $('#txtSummary').val();
            //
            if (ddlCustomer === "") {
                $('#lblCustomer').html('Vui lòng chọn khách hàng');
                flg = false;
            }
            else {
                $('#lblCustomer').html('');
            }
            //
            if (txtAmount === '') {
                $('#lblAmount').html('Không được để trống số tiền nạp');
                flg = false;
            }
            else if (!FormatNumberFloat.test(txtAmount)) {
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
            //
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
                TransactionSpendingController.Create();
            }
            else {
                Notifization.Error(MessageText.Datamissing);
            }
        });
        $('#btnSearch').off('click').on('click', function () {
            TransactionSpendingController.DataList(1);
        });
    },
    DataList: function (page) {
        var model = {
            Query: $('#txtQuery').val(),
            Page: page
        };
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
                            var role = result.role;
                            if (role !== undefined && role !== null) {
                                var action = `<div class='ddl-action'><span><i class='fa fa-caret-down'></i></span>
                                              <div class='ddl-action-content'>`;
                                if (role.Update)
                                    action += `<a href='${URLA}/update/${id}'><i class='fas fa-pen-square'></i>&nbsp;Edit</a>`;
                                if (role.Delete)
                                    action += `<a onclick="TransactionDepositController.ConfirmDelete('${id}')"><i class='fas fa-trash'></i>&nbsp;Delete</a>`;
                                if (role.Details)
                                    action += `<a href='${URLA}/details/${id}'><i class='fas fa-info-circle'></i>&nbsp;Detail</a>`;
                                action += `</div>
                                           </div>`;
                            }
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

                            if (parseInt(status) == 1) {
                                title += `. + ${LibCurrencies.FormatThousands(amount)} đ`
                            } else {
                                title += `. - ${LibCurrencies.FormatThousands(amount)} đ`
                            }
                            if (summary != null) {
                                summary = ", " + summary;
                            }
                            else {
                                summary = "";
                            }
                            // <td>${transactionId}, giao dịch từ N.Hàng ${bankSent} đến N.Hàng ${bankReceived} ${summary}</td>          
                            var rowNum = parseInt(index) + (parseInt(currentPage) - 1) * parseInt(pageSize);
                            rowData += `
                            <tr>
                                 <td class="text-right">${rowNum}&nbsp;</td>
                                 <td>${title}</td>        
                                 <td class='tbcol-created'>${item.CreatedBy}</td>                                  
                                 <td class="text-center">${HelperModel.StatusIcon(item.Enabled)}</td>
                                 <td class="text-center">${item.CreatedDate}</td>
                                 <td class="tbcol-action">${action}</td>
                            </tr>`;
                        });
                        $('tbody#TblData').html(rowData);
                        if (parseInt(totalPage) > 1) {
                            Paging.Pagination("#Pagination", totalPage, currentPage, TransactionSpendingController.DataList);
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
        var txtAmount = $('#txtAmount').val();
        var txtSummary = $('#txtSummary').val();
        var enabled = 0;
        if ($('input[name="cbxActive"]').is(":checked"))
            enabled = 1;
        //
        var model = {
            CustomerID: ddlCustomer,
            Amount: txtAmount,
            Summary: txtSummary,
            Enabled: enabled
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
    },
    Delete: function (id) {
        var model = {
            Id: id
        };
        AjaxFrom.POST({
            url: URLC + '/Delete',
            data: model,
            success: function (response) {
                if (response !== null) {
                    if (response.status === 200) {
                        Notifization.Success(response.message);
                        TransactionSpendingController.DataList(pageIndex);
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
    Details: function () {
        var id = $('#txtID').val();
        if (id.length <= 0) {
            Notifization.Error(MessageText.NotService);
            return;
        }
        var fData = {
            Id: $('#txtID').val()
        };
        $.ajax({
            url: '/post/detail',
            data: {
                strData: JSON.stringify(fData)
            },
            type: 'POST',
            dataType: 'json',
            success: function (result) {
                if (result !== null) {
                    if (result.status === 200) {
                        var item = result.data;
                        $('#LblAccount').html(item.LoginID);
                        $('#LblDate').html(item.CreatedDate);
                        var action = '';
                        if (item.Enabled)
                            action += `<i class='fa fa-toggle-on'></i> actived`;
                        else
                            action += `<i class='fa fa-toggle-off'></i>not active`;

                        $('#LblActive').html(action);
                        $('#lblLastName').html(item.FirstName + ' ' + item.LastName);
                        $('#LblEmail').html(item.Email);
                        $('#LblPhone').html(item.Phone);
                        $('#LblLanguage').html(item.LanguageID);
                        $('#LblPermission').html(item.PermissionID);

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
    ConfirmDelete: function (id) {
        Confirm.Delete(id, TransactionSpendingController.Delete, null, null);

    }
};

TransactionSpendingController.init();
// customer
$(document).on("change", "#ddlCustomer", function () {
    var ddlCustomer = $(this).val();
    if (ddlCustomer === "") {
        $('#lblCustomer').html('Vui lòng chọn khách hàng');
    }
    else {
        $('#lblCustomer').html('');
        var codeid = $(this).find(':selected').data('codeid');
        $('#lblCustomerCodeID').html(codeid);
    }
});
// amount
$(document).on("keyup", "#txtAmount", function () {
    var txtDeposit = $(this).val();
    if (txtDeposit === '') {
        $('#lblAmount').html('Không được để trống số tiền nạp');
    }
    else if (!FormatNumberFloat.test(txtDeposit)) {
        $('#lblAmount').html('Số tiền nạp không hợp lệ');
    }
    else if (parseFloat(txtDeposit) <= 0) {
        $('#lblAmount').html('Số tiền nạp phải > 0');
    }
    else {
        $('#lblAmount').html('');
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