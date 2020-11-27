var pageIndex = 1;
var URLC = "/Management/AirFlight/Action";
var URLA = "/Management/AirFlight";
var arrFile = [];
var FlightController = {
    init: function () {
        FlightController.registerEvent();
    },
    registerEvent: function () {
        $(document).ready(function () {

        });
        $('#btnCreate').off('click').on('click', function () {
            var flg = true;
            // Area
            var ddlArea = $('#ddlAreaID').val();
            var axFee = $('#txtAxFee').val();
            //
            if (ddlArea === "-" || ddlArea === '') {
                $('#lblAreaID').html('Vui lòng chọn vùng/miền');
                flg = false;
            }
            else {
                $('#lblAreaID').html('');
            }
            // Iata code
            var iataCode = $('#txtIataCode').val();
            if (iataCode === '') {
                $('#lblIataCode').html('Không được để trống mã chuyến bay');
                flg = false;
            }
            else if (iataCode.length < 1 || iataCode.length > 5) {
                $('#lblIataCode').html('Mã chuyến bay giới hạn từ 5 characters');
                flg = false;
            }
            else if (!FormatKeyId.test(iataCode)) {
                $('#lblIataCode').html('Mã chuyến bay không hợp lệ');
                flg = false;
            }
            else {
                $('#lblIataCode').html('');
            }

            $('#lblAxFee').html('');
            if (axFee !== "") {
                axFee = LibCurrencies.ConvertToCurrency(axFee);
                if (!FormatCurrency.test(axFee)) {
                    $('#lblAxFee').html('Phí sân bay không hợp lệ');
                    flg = false;
                }
            }
            

            // title
            var title = $('#txtTitle').val();
            if (title === '') {
                $('#lblTitle').html('Không được để trống tiêu đề');
                flg = false;
            }
            else if (title.length < 1 || title.length > 80) {
                $('#lblTitle').html('Tiêu đề giới hạn từ 1-> 80 characters');
                flg = false;
            }
            else if (!FormatKeyword.test(title)) {
                $('#lblTitle').html('Tiêu đề không hợp lệ');
                flg = false;
            }
            else {
                $('#lblTitle').html('');
            }
            // summary
            var summary = $('#txtSummary').val();
            if (summary !== '') {
                if (summary.length < 1 || summary.length > 120) {
                    $('#lblSummary').html('Mô tả giới hạn từ 1-> 120 ký tự');
                    flg = false;
                }
                else if (!FormatKeyword.test(summary)) {
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
            // submit form
            if (flg)
                FlightController.Create();
            else
                Notifization.Error(MessageText.Datamissing);
        });
        $('#btnSearch').off('click').on('click', function () {
            FlightController.DataList(1);
        });
        $('#btnUpdate').off('click').on('click', function () {
            var flg = true;
            // Area
            var ddlArea = $('#ddlAreaID').val();
            var axFee = $('#txtAxFee').val();
            //
            if (ddlArea === "-" || ddlArea === '') {
                $('#lblAreaID').html('Vui lòng chọn vùng/miền');
                flg = false;
            }
            else {
                $('#lblAreaID').html('');
            }
            // Iata code
            var iataCode = $('#txtIataCode').val();
            if (iataCode === '') {
                $('#lblIataCode').html('Không được để trống mã chuyến bay');
                flg = false;
            }
            else if (iataCode.length < 1 || iataCode.length > 5) {
                $('#lblIataCode').html('Mã chuyến bay giới hạn từ 5 characters');
                flg = false;
            }
            else if (!FormatKeyId.test(iataCode)) {
                $('#lblIataCode').html('Mã chuyến bay không hợp lệ');
                flg = false;
            }
            else {
                $('#lblIataCode').html('');
            }

            $('#lblAxFee').html('');
            if (axFee != "") {
                axFee = LibCurrencies.ConvertToCurrency(axFee);
                if (!FormatCurrency.test(axFee)) {
                    $('#lblAxFee').html('Phí sân bay không hợp lệ');
                    flg = false;
                }
            }
            // title
            var title = $('#txtTitle').val();
            if (title === '') {
                $('#lblTitle').html('Không được để trống tiêu đề');
                flg = false;
            }
            else if (title.length < 1 || title.length > 80) {
                $('#lblTitle').html('Tiêu đề giới hạn từ 1-> 80 characters');
                flg = false;
            }
            else if (!FormatKeyword.test(title)) {
                $('#lblTitle').html('Tiêu đề không hợp lệ');
                flg = false;
            }
            else {
                $('#lblTitle').html('');
            }
            // summary
            var summary = $('#txtSummary').val();
            if (summary !== '') {
                if (summary.length < 1 || summary.length > 120) {
                    $('#lblSummary').html('Mô tả giới hạn từ 1-> 120 ký tự');
                    flg = false;
                }
                else if (!FormatKeyword.test(summary)) {
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
            // submit form
            if (flg) {
                FlightController.Update();
            }
            else {
                Notifization.Error(MessageText.Datamissing);
            }
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
                            var _unit = 'vnd';
                            var _title = SubStringText.SubTitle(item.Title);
                            var _summary = SubStringText.SubSummary(item.Summary);
                            var _iatacode = item.IATACode;
                            var _area = item.AreaName;
                            //  role
                            var action = HelperModel.RolePermission(result.role, "FlightController", id);
                            //
                            var rowNum = parseInt(index) + (parseInt(currentPage) - 1) * parseInt(pageSize);
                            rowData += `
                            <tr>
                                 <td class="text-right">${rowNum}&nbsp;</td>  
                                 <td class='tbcol-photo'>${_area}</td>  
                                 <td class='tbcol-photo'>${_iatacode}</td>  
                                 <td class='text-left'>${_title}</td>                                                                                                                                                                                                                                                                         
                                 <td class="text-center">${HelperModel.StatusIcon(item.Enabled)}</td>
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
    Create: function () {
        var iataCode = $('#txtIataCode').val();
        var axFee = LibCurrencies.ConvertToCurrency($('#txtAxFee').val());    
        var title = $('#txtTitle').val();
        var summary = $('#txtSummary').val();
        var ddlArea = $('#ddlAreaID').val();
        var enabled = 0;
        if ($('input[name="cbxActive"]').is(":checked"))
            enabled = 1;
        //
        var model = {
            CategoryID: ddlArea,
            Title: title,
            IataCode: iataCode,
            AxFee: axFee,
            Summary: summary,
            Enabled: enabled
        };
        AjaxFrom.POST({
            url: URLC + '/Create',
            data: model,
            success: function (response) {
                if (response === null || response.status === undefined) {
                    Notifization.Error(MessageText.NOTSERVICES);
                    return;
                }
                if (response.status === 200) {
                    Notifization.Success(response.message);
                    FData.ResetForm();
                    return;
                }
                Notifization.Error(response.message);
                return;
            },
            error: function (response) {
                console.log('::' + MessageText.NOTSERVICES);
            }
        });

    },
    Update: function () {
        var id = $('#txtID').val();
        var iataCode = $('#txtIataCode').val();
        var axFee = LibCurrencies.ConvertToCurrency($('#txtAxFee').val());    
        var title = $('#txtTitle').val();
        var summary = $('#txtSummary').val();
        var ddlArea = $('#ddlAreaID').val();
        var enabled = 0;
        if ($('input[name="cbxActive"]').is(":checked"))
            enabled = 1;
        //
        var model = {
            ID: id,
            CategoryID: ddlArea,
            Title: title,
            IataCode: iataCode,
            AxFee: axFee,
            Summary: summary,
            Enabled: enabled
        };
        AjaxFrom.POST({
            url: URLC + '/Update',
            data: model,
            success: function (response) {
                if (response === null || response.status === undefined) {
                    Notifization.Error(MessageText.NOTSERVICES);
                    return;
                }
                if (response.status === 200) {
                    Notifization.Success(response.message);
                    return;
                }
                Notifization.Error(response.message);
                return;
            },
            error: function (response) {
                console.log('::' + MessageText.NOTSERVICES);
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
                if (response === null || response.status === undefined) {
                    Notifization.Error(MessageText.NOTSERVICES);
                    return;
                }
                if (response.status === 200) {
                    Notifization.Success(response.message);
                    FlightController.DataList(pageIndex);
                    return;
                }
                Notifization.Error(response.message);
                return;
            },
            error: function (response) {
                console.log('::' + MessageText.NOTSERVICES);
            }
        });
    },
    ConfirmDelete: function (id) {
        Confirm.Delete(id, FlightController.Delete, null, null);
    }
};
//
FlightController.init();
//
$(document).on('keyup', '#txtTitle', function () {
    var title = $(this).val();
    if (title === '') {
        $('#lblTitle').html('Không được để trống tiêu đề');
    }
    else if (title.length < 1 || title.length > 80) {
        $('#lblTitle').html('Tiêu đề giới hạn từ 1-> 80 characters');
    }
    else if (!FormatKeyword.test(title)) {
        $('#lblTitle').html('Tiêu đề không hợp lệ');
    }
    else {
        $('#lblTitle').html('');
    }
});
//
$(document).on('keyup', '#txtIataCode', function () {
    var iataCode = $(this).val();
    if (iataCode === '') {
        $('#lblIataCode').html('Không được để trống mã chuyến bay');
    }
    else if (iataCode.length < 1 || iataCode.length > 5) {
        $('#lblIataCode').html('Mã chuyến bay giới hạn từ 5 characters');
    }
    else if (!FormatKeyId.test(iataCode)) {
        $('#lblIataCode').html('Mã chuyến bay không hợp lệ');
    }
    else {
        $('#lblIataCode').html('');
    }
});
$(document).on('keyup', '#txtSummary', function () {
    var summary = $(this).val();
    if (summary !== '') {
        if (summary.length < 1 || summary.length > 120) {
            $('#lblSummary').html('Tiêu đề giới hạn từ 1-> 80 characters');
        }
        else if (!FormatKeyword.test(summary)) {
            $('#lblSummary').html('Mô tả không hợp lệ');
        }
        else {
            $('#lblSummary').html('');
        }
    }
    else {
        $('#lblSummary').html('');
    }
});
$(document).on('keyup', '#txtAlias', function () {
    var alias = $('#txtAlias').val();
    if (alias !== '') {
        if (alias.length > 80) {
            $('#lblAlias').html('Đường dẫn giới hạn từ 0-> 80 ký tự');
        }
        else if (!FormatUnicode.test(alias)) {
            $('#lblAlias').html('Đường dẫn không hợp lệ');
        }
        else {
            $('#lblAlias').html('');
        }
    } else {
        $('#lblAlias').html('');
    }
});
// ViewTotal
$(document).on('keyup', '#txtViewTotal', function () {
    var viewTotal = $(this).val();
    if (viewTotal !== "") {
        if (!FormatNumber.test(viewTotal)) {
            $('#lblViewTotal').html('Số lượt xem chuyến bay không hợp lệ');
        }
        else {
            $('#lblViewTotal').html('');
        }
    }
});
// view date
$(document).on('keyup', '#txtViewDate', function () {
    var viewDate = $(this).val();
    if (viewDate !== '') {
        if (!FormatDateVN.test(viewDate)) {
            $('#lblViewDate').html('Ngày hiển thị không hợp lệ');
        }
        else {
            $('#lblViewDate').html('');
        }
    } else {
        $('#lblViewDate').html('');
    }
});
// price
$(document).on('keyup', '#txtPrice', function () {
    var price = $(this).val();

    if (price === "") {
        $('#lblPrice').html('Không được để trống giá chuyến bay');
    }
    else if (!FormatCurrency.test(price)) {
        $('#lblPrice').html('Giá chuyến bay không hợp lệ');
    }
    else {
        $('#lblPrice').html('');
    }
});
//PriceListed
$(document).on('keyup', '#txtPriceListed', function () {
    var priceListed = $(this).val();
    if (priceListed === "") {
        $('#lblPriceListed').html('Không được để trống giá khuyến mại');
    }
    else if (!FormatCurrency.test(priceListed)) {
        $('#lblPriceListed').html('Giá khuyến mại không hợp lệ');
    }
    else {
        $('#lblPriceListed').html('');
    }
});

$(document).on("change", "#ddlAreaID", function () {
    var txtCtl = $(this).val();
    if (txtCtl === "-" || txtCtl === "") {
        $('#lblAreaID').html('Vui lòng chọn vùng/miền');
    }
    else {
        $('#lblAreaID').html('');
    }
});
//
$(document).on('', '.img-caption-text', function () {
    $('.new-box-preview img').click();
});
$(document).on('keyup', '#txtAxFee', function () {
    var axFee = $(this).val();
    $('#lblAxFee').html('');
    if (axFee !== "") {
        axFee = LibCurrencies.ConvertToCurrency(axFee);
        if (!FormatCurrency.test(axFee)) {
            $('#lblAxFee').html('Phí sân bay không hợp lệ');
        } 
    }
});
