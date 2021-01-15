var pageIndex = 1;
var URLC = "/Management/AirFlight/Action";
var URLA = "/Management/AirFlight";
var AirportConfigController = {
    init: function () {
        AirportConfigController.registerEvent();
    },
    registerEvent: function () {
        $('#btnSearch').off('click').on('click', function () {
            AirportConfigController.ExSetting(1);
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
                AirportConfigController.Update();
            }
            else {
                Notifization.Error(MessageText.Datamissing);
            }
        });
    },
    ExSetting: function (page) {
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
            url: URLC + '/ExSetting',
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
                            var itacode = item.IATACode;
                            var axFee = item.AxFee;
                            var voidBookTime = item.VoidBookTime;
                            var voidTicketTime = item.VoidTicketTime; 
                            var createdBy = item.CreatedBy;

                            //  role 
                            var rowNum = parseInt(index) + (parseInt(currentPage) - 1) * parseInt(pageSize);
                            rowData += `
                            <tr>
                                 <td class="text-right">${rowNum}&nbsp;</td>
                                 <td class='text-center'>${itacode}</td>                                    
                                 <td>${title}</td>                                                                    
                                 <td class="text-right">${voidBookTime} h</td>                                    
                                 <td class="text-right">${voidTicketTime} h</td>                                    
                                 <td class="text-right">${LibCurrencies.FormatToCurrency(axFee)} đ</td>                                                                    
                                 <td class="text-center">${item.CreatedDate}</td>
                            </tr>`;
                        });
                        $('tbody#TblData').html(rowData);
                        if (parseInt(totalPage) > 1) {
                            Paging.Pagination("#Pagination", totalPage, currentPage, AirportConfigController.DataList);
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

AirportConfigController.init();
 