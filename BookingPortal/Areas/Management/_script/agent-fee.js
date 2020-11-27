var pageIndex = 1;
var URLC = "/Management/AgentFee/Action";
var URLA = "/Management/AgentFee";
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
    DataList: function (page) {
        //       
        var _ariaId = $('#ddlAreaID').val();
        var _province = $('#ddlProvince').val();
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
            Status: parseInt($('#ddlStatus').val()),
            AreaID: _ariaId,
            ProviceID: _province,
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
                            var _unit = 'đ';
                            var _mkh = SubStringText.SubTitle(item.Title);
                            var _amount = item.Amount;
                            var _agentName = SubStringText.SubTitle(item.AgentName);
                            //  role
                            var action = HelperModel.RolePermission(result.role, "AgentFeeConfigController", id);
                            //
                            var rowNum = parseInt(index) + (parseInt(currentPage) - 1) * parseInt(pageSize);
                            rowData += `
                            <tr>
                                 <td class="text-right">${rowNum}&nbsp;</td> 
                                 <td class='text-left'>${_mkh}</td>                                                                                                                                                                                                                                                                         
                                 <td class='text-left'>${_agentName}</td>                                                                                                                                                                                                                                                                         
                                 <td class='text-right'>${_amount} ${_unit}</td>                                                                                                                                                                                                                                                                         
                                 <td class="tbcol-action">${action}</td>
                            </tr>`;
                        });
                        $('tbody#TblData').html(rowData);
                        if (parseInt(totalPage) > 1) {
                            Paging.Pagination("#Pagination", totalPage, currentPage, FlightController.DataList);
                        }
                        return;
                    }
                    else {
                        //Notifization.Error(result.message);
                        console.log('::' + result.message);
                        return;
                    }
                }
                Notifization.Error(MessageText.NOTSERVICES);
                return;
            },
            error: function (result) {
                console.log('::' + MessageText.NOTSERVICES);
            }
        });
    },
    AgentFeeConfig: function () {
        var ddlCustomer = $('#ddlCustomer').val();
        var txtAmount = $('#txtAmount').val();
        var model = {
            AgentID: ddlCustomer,
            Amount: LibCurrencies.ConvertToCurrency(txtAmount)
        };
        AjaxFrom.POST({
            url: URLC + '/Update',
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
        // get data fill to form 
        var model = {
            AgentID: ddlCustomer
        };
        AjaxFrom.POST({
            url: URLC + '/GetAgentFee',
            data: model,
            success: function (response) {
                if (response !== null) {
                    if (response.status === 200) {
                        //
                        var data = response.data;
                        if (data != null) {
                            var amount = LibCurrencies.FormatToCurrency(data.Amount);
                            $('#txtAmount').val(amount);
                        }
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
                console.log('::' + MessageText.NotService + JSON.stringify(response));
            }
        });

    }
});

