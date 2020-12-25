var pageIndex = 1;
var URLC = "/Management/AgentSpendingLimit/Action";
var URLA = "/Management/AgentSpendingLimit";
var arrFile = [];
var AgentSpendingLimitController = {
    init: function () {
        AgentSpendingLimitController.registerEvent();
    },
    registerEvent: function () {
        $('#btnSearch').off('click').on('click', function () {
            AgentSpendingLimitController.DataList(1);
        });
        $('#btnApply').off('click').on('click', function () {
            var flg = true;
            var txtAmount = $('#txtAmount').val();
            if (txtAmount === '') {
                $('#lblAmount').html('Không được để trống hạn mức');
                flg = false;
            }
            else {
                txtAmount = LibCurrencies.ConvertToCurrency(txtAmount);
                if (!FormatCurrency.test(txtAmount)) {
                    $('#lblAmount').html('Hạn mức không hợp lệ');
                    flg = false;
                }
                else if (parseFloat(txtAmount) < 0) {
                    $('#lblAmount').html('Hạn mức phải >= 0');
                    flg = false;
                }
                else {
                    $('#lblAmount').html('');
                }
            }
            // submit form
            if (flg) {
                AgentSpendingLimitController.Setting();
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
            Status: parseInt($('#ddlStatus').val()),
            TimeZoneLocal: LibDateTime.GetTimeZoneByLocal(),
            AreaID: "",
            ProviceID: "",
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
                            var _unit = 'vnd';
                            var _title = SubStringText.SubTitle(item.Title);
                            var _codeId = item.CodeID;
                            var _amount = LibCurrencies.FormatToCurrency(item.Amount);
                            //  role
                            var action = HelperModel.RolePermission(result.role, "AgentSpendingLimitController", id);
                            //
                            var rowNum = parseInt(index) + (parseInt(currentPage) - 1) * parseInt(pageSize);
                            rowData += `
                            <tr>
                                 <td class="text-right">${rowNum}&nbsp;</td>  
                                 <td>${_title}</td>  
                                 <td>${_codeId}</td>  
                                 <td class='text-right'>${_amount} đ</td>     
                                 <td class="text-center">${HelperModel.StatusIcon(item.Enabled)}</td>
                                 <td class="text-center">${item.CreatedDate}</td> 
                                 <td class="tbcol-action">${action}</td>
                            </tr>`;
                        });
                        $('tbody#TblData').html(rowData);
                        if (parseInt(totalPage) > 1) {
                            Paging.Pagination("#Pagination", totalPage, currentPage, AgentSpendingLimitController.DataList);
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
    Setting: function () {
        //    
        var id = $('#txtID').val();
        var txtAmount = LibCurrencies.ConvertToCurrency($('#txtAmount').val());

        var enabled = 0;
        if ($('input[name="cbxActive"]').is(":checked"))
            enabled = 1;
        //
        var model = {
            AgentID: id,
            Amount: txtAmount,
            Enabled: enabled
        };
        //
        AjaxFrom.POST({
            url: URLC + '/Setting',
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
AgentSpendingLimitController.init();

$(document).on("keyup", "#txtAmount", function () {
    var txtAmount = $(this).val();
    if (txtAmount === '') {
        $('#lblAmount').html('Không được để trống hạn mức');
    }
    else {
        txtAmount = LibCurrencies.ConvertToCurrency(txtAmount);
        if (!FormatCurrency.test(txtAmount)) {
            $('#lblAmount').html('Hạn mức không hợp lệ');
        }
        else if (parseFloat(txtAmount) < 0) {
            $('#lblAmount').html('Hạn mức phải >= 0');
        }
        else {
            $('#lblAmount').html('');
        }
    }
});