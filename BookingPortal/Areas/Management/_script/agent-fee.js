
var pageIndex = 1;
var URLC = "/Management/AirBook/Action";
var URLA = "/Management/AirBook";
var arrFile = [];
//

var AgentFeeConfigController = {
    init: function () {
        AgentFeeConfigController.registerEvent();
    },
    registerEvent: function () {
        $(document).ready(function () {
            // $('[data-dateDefault="true"]').val(LibDateTime.Get_ClientDate(lg = 'en'));
        });
        $('#btnFeeApply').off('click').on('click', function () {
            var flg = true;
            // Flight go
            var ddlCustomer = $('#ddlCustomer').val();
            var txtAmount = $('#txtAmount').val();
            if (HelperModel.AccessInApplication != RoleEnum.IsAdminSupplierLogged && HelperModel.AccessInApplication != RoleEnum.IsCustomerLogged) {
                //
                if (ddlCustomer === "") {
                    $('#lblCustomer').html('Vui lòng chọn khách hàng');
                    flg = false;
                }
                else {
                    $('#lblCustomer').html('');
                }
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
                else if (parseFloat(txtAmount) < 0 || parseFloat(txtAmount) > 100000000) {
                    $('#lblAmount').html('Số tiền giới hạn từ 0 - 100 000 000 đ');
                    flg = false;
                }
                else {
                    $('#lblAmount').html('');
                }
            }

            // submit form
            if (flg) {
                AgentFeeConfigController.AgentFeeConfig();
            }
            else
                Notifization.Error(MessageText.Datamissing);
        });
    },
    AgentFeeConfig: function () {
        var ddlCustomer = $('#ddlCustomer').val();
        var txtAmount = $('#txtAmount').val();
        var model = {
            AgentID: ddlCustomer,
            Amount: txtAmount
        };
        AjaxFrom.POST({
            url: URLC + '/Agent-FeeConfig',
            data: model,
            success: function (response) {
                if (response !== null) {
                    if (response.status === 200) {
                        Notifization.Success(response.message);
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
//
AgentFeeConfigController.init();
//Validate
//###################################################################################################################################################
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
        else if (parseFloat(txtAmount) < 0 || parseFloat(txtAmount) > 100000000) {
            $('#lblAmount').html('Số tiền giới hạn từ 0 - 100 000 000 đ');
        }
        else {
            $('#lblAmount').html('');
        }
    }
});

$(document).on("change", "#ddlCustomer", function () {
    var ddlCustomer = $(this).val();
    if (ddlCustomer === "") {
        $('#lblCustomer').html('Vui lòng chọn khách hàng');
    }
    else {
        $('#lblCustomer').html('');
    }
});

