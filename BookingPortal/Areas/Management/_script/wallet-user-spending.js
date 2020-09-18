﻿var pageIndex = 1;
var URLC = "/Management/Wallet-User-Spending-History/Action";
var URLA = "/Management/WalletUserSpendingHistory";
var WalletUserSpendingHistoryController = {
    init: function () {
        WalletUserSpendingHistoryController.registerEvent();
    },
    registerEvent: function () {
        $('#btnSearch').off('click').on('click', function () {
            WalletUserSpendingHistoryController.DataList(1);
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
                            //  role
                            var title = item.Title;
                            var amount = item.Amount;
                            title += ` thời gian: ${item.CreatedFullDate} `;      
                            var rowNum = parseInt(index) + (parseInt(currentPage) - 1) * parseInt(pageSize);

                            rowData += `
                            <tr>
                                 <td class="text-right">${rowNum}&nbsp;</td>
                                 <td>${title}</td>                                    
                                 <td class='text-right'>${LibCurrencies.FormatToCurrency(amount)} đ</td>                                    
                                 <td class="text-center">${item.CreatedDate}</td>
                            </tr>`;
                        });
                        $('tbody#TblData').html(rowData);
                        if (parseInt(totalPage) > 1) {
                            Paging.Pagination("#Pagination", totalPage, currentPage, WalletUserSpendingHistoryController.DataList);
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

WalletUserSpendingHistoryController.init();