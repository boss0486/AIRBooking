var pageIndex = 1;
var URLC = "/Management/TransactionCustomerSpending/Action";
var URLA = "/Management/TransactionCustomerSpending";
var TransactionSpendingController = {
    init: function () {
        TransactionSpendingController.registerEvent();
    },
    registerEvent: function () {
        $('#btnCreate').off('click').on('click', function () {
            var flg = true;
            var ddlSupplier = $('#ddlSupplier').val();
            var ddlCustomer = $('#ddlCustomer').val();
            var txtAmount = $('#txtAmount').val();
            var txtSummary = $('#txtSummary').val();
            //
            if (HelperModel.AccessInApplication != 3 && HelperModel.AccessInApplication != 4) {
                //
                if (ddlSupplier === "") {
                    $('#lblSupplier').html('Vui lòng chọn nhà cung cấp');
                    flg = false;
                }
                else {
                    $('#lblSupplier').html('');
                }
                //
            }
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
                            var title = item.Title;
                            var amount = item.Amount;
                            var supplierCodeId =item.SupplierCodeID 
                            var customerCodeId= item.CustomerCodeID;
                            var createdBy = item.CreatedBy;
                            var createdDate = item.CreatedDate;
                            var createdFullDate  = item.CreatedFullDate;
                            var enabled = item.Enabled;
                            //  role
                            var action = HelperModel.RolePermission(result.role, "TransactionSpendingController", id);
                            //
                            var rowNum = parseInt(index) + (parseInt(currentPage) - 1) * parseInt(pageSize);
                            rowData += `
                            <tr>
                                 <td class="text-right">${rowNum}&nbsp;</td>
                                 <td>${id}</td>        
                                 <td>${title}</td>        
                                 <td>${supplierCodeId}</td>        
                                 <td>${customerCodeId}</td>        
                                 <td class='text-right'>${LibCurrencies.FormatToCurrency(amount, )} đ</td>                                  
                                 <td class="text-center">${HelperModel.StatusIcon(enabled)}</td>
                                 <td class="text-rigth">${createdDate}</td>
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
        var ddlSupplier = $('#ddlSupplier').val();
        if (ddlSupplier == undefined || ddlSupplier == null) {
            ddlSupplier = '';
        }
        var txtAmount = LibCurrencies.ConvertToCurrency($('#txtAmount').val());
        var txtSummary = $('#txtSummary').val();
        var enabled = 0;
        if ($('input[name="cbxActive"]').is(":checked"))
            enabled = 1;
        //
        var model = {
            SupplierID: ddlSupplier,
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
};

TransactionSpendingController.init();
//
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
        GetCustomerBySupplierID(ddlSupplier, "", false);
    }
});
// customer
$(document).on("change", "#ddlCustomer", function () {
    var ddlCustomer = $(this).val();
    if (ddlCustomer === "") {
        $('#lblCustomer').html('Vui lòng chọn khách hàng');
        $('#lblCustomerCodeID').html('');
    }
    else {
        $('#lblCustomer').html('');
        var codeid = $(this).find(':selected').data('codeid');
        $('#lblCustomerCodeID').html(codeid);
    }
});
// amount
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

function GetCustomerBySupplierID(supplierId, _id, isChangeEvent) {
    var option = `<option value="">-Lựa chọn-</option>`;
    $('#ddlCustomer').html(option);
    $('#ddlCustomer').selectpicker('refresh');
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
                    $('#ddlCustomer').html(option);
                    setTimeout(function () {
                        $('#ddlCustomer').selectpicker('refresh');
                        if (isChangeEvent !== undefined && isChangeEvent == true && attrSelect !== '') {
                            $('#ddlCustomer').change();
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