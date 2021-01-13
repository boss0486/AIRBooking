var pageIndex = 1;
var URLC = "/product";
var URLA = "/backend/product";
var arrFile = [];
var productController = {
    init: function () {
        productController.registerEvent();
    },
    registerEvent: function () {
        $(document).ready(function () {
            $('[data-date="true"]').val(DateTime.Get_ClientDate(lg = 'en'));
        });
        $('#btnCreate').off('click').on('click', function () {
            var flg = true;
            var textId = $('#txtTextID').val();
            if (textId === '') {
                $('#lblTextID').html('Không được để trống mã sản phẩm');
                flg = false;
            }
            else if (textId.length < 5 || textId.length > 5) {
                $('#lblTextID').html('Mã sản phẩm giới hạn từ 5 characters');
                flg = false;
            }
            else if (!FormatKeyId.test(textId)) {
                $('#lblTextID').html('Mã sản phẩm không hợp lệ');
                flg = false;
            }
            else {
                $('#lblTextID').html('');
            }
            //
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
            // 
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
            // create date display
            var viewDate = $('#txtViewDate').val();
            if (viewDate !== '') {
                if (!FormatDateVN.test(viewDate)) {
                    $('#lblViewDate').html('Ngày hiển thị không hợp lệ');
                    flg = false;
                }
                else {
                    $('#lblViewDate').html('');
                }
            } else {
                $('#lblViewDate').html('');
            }
            // ViewTotal
            var viewTotal = $('#txtViewTotal').val();
            if (viewTotal !== "") {
                if (!FormatNumber.test(viewTotal)) {
                    $('#lblViewTotal').html('Số lượt xem sản phẩm không hợp lệ');
                    flg = false;
                }
                else {
                    $('#lblViewTotal').html('');
                }
            }
            // category
            var ddlCategory = $('#ddlCategoryID').val();
            if (ddlCategory === "") {
                $('#lblCategory').html('Vui lòng chọn danh mục');
                flg = false;
            }
            else {
                $('#lblCategory').html('');
            }
            // price
            var price = $('#txtPrice').val();
            if (price === "") {
                $('#lblPrice').html('Không được để trống giá sản phẩm');
                flg = false;
            }
            else if (!FormatCurrency.test(price)) {
                $('#lblPrice').html('Giá sản phẩm không hợp lệ');
            }
            else {
                $('#lblPrice').html('');
            }
            // 
            var priceListed = $('#txtPriceListed').val();
            if (priceListed === "") {
                $('#lblPriceListed').html('Không được để trống giá khuyến mại');
                flg = false;
            }
            else if (!FormatCurrency.test(priceListed) < 0) {
                $('#lblPriceListed').html('Giá khuyến mại không hợp lệ');
                flg = false;
            }
            else {
                $('#lblPriceListed').html('');
            }
            //// lblProductWarranty
            //var ddlProductWarranty = $('#ddlProductWarranty').val();
            //if (ddlProductWarranty === "") {
            //    $('#lblProductWarranty').html('Vui lòng chọn thời gian bảo hành');
            //    flg = false;
            //}
            //else {
            //    $('#lblProductWarranty').html('');
            //}
            //// ProductWarranty
            //var ddlProductProvider = $('#ddlProductProvider').val();
            //if (ddlProductProvider === "") {
            //    $('#lblProductProvider').html('Vui lòng chọn nhà cung cấp');
            //}
            //else {
            //    $('#lblProductWarranty').html('');
            //}

            // ProductState
            var ddlProductState = $('#ddlProductState').val();
            if (parseInt(ddlProductState) === -1) {
                $('#lblProductState').html('Vui lòng chọn tình trạng');
                flg = false;
            }
            else
                $('#lblProductState').html('');
            // submit form
            if (flg)
                productController.Create();
            else
                Notifization.Error(MessageText.DATAMISSING);
        });
        $('#btnSearch').off('click').on('click', function () {
            productController.DataList(1);
        });
        $('#btnUpdate').off('click').on('click', function () {
            var flg = true;
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
            // 
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
            // create date display
            var viewDate = $('#txtViewDate').val();
            if (viewDate !== '') {
                if (!FormatDateVN.test(viewDate)) {
                    $('#lblViewDate').html('Ngày hiển thị không hợp lệ');
                    flg = false;
                }
                else {
                    $('#lblViewDate').html('');
                }
            } else {
                $('#lblViewDate').html('');
            }
            // ViewTotal
            var viewTotal = $('#txtViewTotal').val();
            if (viewTotal !== "") {
                if (!FormatNumber.test(viewTotal)) {
                    $('#lblViewTotal').html('Số lượt xem sản phẩm không hợp lệ');
                    flg = false;
                }
                else {
                    $('#lblViewTotal').html('');
                }
            }
            // category
            var ddlCategory = $('#ddlCategoryID').val();
            if (ddlCategory === "") {
                $('#lblCategory').html('Vui lòng chọn danh mục');
                flg = false;
            }
            else {
                $('#lblCategory').html('');
            }
            // price
            var price = $('#txtPrice').val();
            if (price === "") {
                $('#lblPrice').html('Không được để trống giá sản phẩm');
                flg = false;
            }
            else if (!FormatCurrency.test(price)) {
                $('#lblPrice').html('Giá sản phẩm không hợp lệ');
            }
            else {
                $('#lblPrice').html('');
            }
            // 
            var priceListed = $('#txtPriceListed').val();
            if (priceListed === "") {
                $('#lblPriceListed').html('Không được để trống giá khuyến mại');
                flg = false;
            }
            else if (!FormatCurrency.test(priceListed) < 0) {
                $('#lblPriceListed').html('Giá khuyến mại không hợp lệ');
                flg = false;
            }
            else {
                $('#lblPriceListed').html('');
            }
            //// lblProductWarranty
            //var ddlProductWarranty = $('#ddlProductWarranty').val();
            //if (ddlProductWarranty === "") {
            //    $('#lblProductWarranty').html('Vui lòng chọn thời gian bảo hành');
            //    flg = false;
            //}
            //else {
            //    $('#lblProductWarranty').html('');
            //}
            //// ProductWarranty
            //var ddlProductProvider = $('#ddlProductProvider').val();
            //if (ddlProductProvider === "") {
            //    $('#lblProductProvider').html('Vui lòng chọn nhà cung cấp');
            //}
            //else {
            //    $('#lblProductWarranty').html('');
            //}
            // ProductState
            var ddlProductState = $('#ddlProductState').val();
            if (parseInt(ddlProductState) === -1) {
                $('#lblProductState').html('Vui lòng chọn tình trạng');
                flg = false;
            }
            else
                $('#lblProductState').html('');
            // submit form
            if (flg) {
                productController.Update();
            }
            else {
                Notifization.Error(MessageText.DATAMISSING);
            }
        });
    },
    Create: function () {
        var textId = $('#txtTextID').val();
        var title = $('#txtTitle').val();
        var alias = $('#txtAlias').val();
        var summary = $('#txtSummary').val();
        var tag = $('#txtTag').val();
        var viewTotal = $('#txtViewTotal').val();
        var viewDate = $('#txtViewDate').val();
        var ddlCategory = $('#ddlCategoryID').val();
        var price = $('#txtPrice').val();
        var priceListed = $('#txtPriceListed').val();
        var ddlProductWarranty = $('#ddlProductWarranty').val();
        var ddlProductProvider = $('#ddlProductProvider').val();
        var ddlProductState = $('#ddlProductState').val();
        var htmlText = tinyMCE.editors[$('#txtHtmlText').attr('id')].getContent();
        var htmlNote = tinyMCE.editors[$('#txtNote').attr('id')].getContent();

        var enabled = 0;
        if ($('#cbxActive').hasClass('actived'))
            enabled = 1;
        //
        var _imgFile = '';
        var _imgFileView = $('.new-box-preview img');
        if (_imgFileView !== '' && _imgFileView !== undefined)
            _imgFile = $(_imgFileView).data('id');

        // photo
        var arrPhoto = [];
        var _imgList = $('.pre-view .pre-item-box');
        if (_imgList.length > 0) {
            $.each(_imgList, function (index, preItem) {
                if ($(this).attr('id') !== '') {
                    var _iBoxId = $(this).attr('id');
                    if (_iBoxId.length > 0) {
                        var _pathFile = $('#' + _iBoxId + ' .image-box img').data('id');
                        arrPhoto.push(_pathFile);
                    }
                }
            });
        }
        var model = {
            CategoryID: ddlCategory,
            Title: title,
            Alias: alias,
            TextID: textId,
            Summary: summary,
            HtmlNote: htmlNote,
            HtmlText: htmlText,
            Tag: tag,
            ImageFile: _imgFile,
            Price: price,
            PriceListed: priceListed,
            PriceText: '',
            Originate: ddlProductProvider,
            MadeIn: ddlProductProvider,
            Warranty: ddlProductWarranty,
            ViewTotal: viewTotal,
            ViewDate: viewDate,
            Photos: arrPhoto,
            State: ddlProductState,
            Enabled: enabled
        };
        AjaxFrom.POST({
            url: URLC + '/create',
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
    Update: function () {
        var id = $('#txtID').val();
        var textId = $('#txtTextID').val();
        var title = $('#txtTitle').val();
        var alias = $('#txtAlias').val();
        var summary = $('#txtSummary').val();
        var tag = $('#txtTag').val();
        var viewTotal = $('#txtViewTotal').val();
        var viewDate = $('#txtViewDate').val();
        var ddlCategory = $('#ddlCategoryID').val();
        var price = $('#txtPrice').val();
        var priceListed = $('#txtPriceListed').val();
        var ddlProductWarranty = $('#ddlProductWarranty').val();
        var ddlProductProvider = $('#ddlProductProvider').val();
        var ddlProductState = $('#ddlProductState').val();
        var htmlText = tinyMCE.editors[$('#txtHtmlText').attr('id')].getContent();
        var htmlNote = tinyMCE.editors[$('#txtNote').attr('id')].getContent();
        var enabled = 0;
        if ($('#cbxActive').hasClass('actived'))
            enabled = 1;
        //
        var _imgFile = '';
        var _imgFileView = $('.new-box-preview img');
        if (_imgFileView !== '' && _imgFileView !== undefined)
            _imgFile = $(_imgFileView).data('id');

        // photo
        var arrPhoto = [];
        var _imgList = $('.pre-view .pre-item-box');
        if (_imgList.length > 0) {
            $.each(_imgList, function (index, preItem) {
                if ($(this).attr('id') !== '') {
                    var _iBoxId = $(this).attr('id');
                    if (_iBoxId.length > 0) {
                        var _pathFile = $('#' + _iBoxId + ' .image-box img').data('id');
                        arrPhoto.push(_pathFile);
                    }
                }
            });
        }
        var model = {
            ID: id,
            CategoryID: ddlCategory,
            Title: title,
            Alias: alias,
            TextID: textId,
            Summary: summary,
            HtmlNote: htmlNote,
            HtmlText: htmlText,
            Tag: tag,
            ImageFile: _imgFile,
            Price: price,
            PriceListed: priceListed,
            PriceText: '',
            Originate: ddlProductProvider,
            MadeIn: ddlProductProvider,
            Warranty: ddlProductWarranty,
            ViewTotal: viewTotal,
            ViewDate: viewDate,
            Photos: arrPhoto,
            State: ddlProductState,
            Enabled: enabled
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
                Message.Error(MessageText.NotService);
                return;
            },
            error: function (response) {
                console.log('::' + MessageText.NotService);
            }
        });
    },
    DataList: function (page) {
        var _query = $('#txtQuery').val();
        var _categoryId = $('#ddlCategoryID').val();
        var _state = $('#ddlProductStage').val();
        var _status = $('#ddlProductStatus').val();
        var model = {
            Query: _query,
            Page: page,
            CategoryID: _categoryId,
            State: _state,
            Status: _status
        };
        AjaxFrom.POST({
            url: URLC + '/datalist',
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
                                    action += `<a onclick="productController.ConfirmDelete('${id}')"><i class='fas fa-trash'></i>&nbsp;Delete</a>`;
                                if (role.Detail)
                                    action += `<a href='${URLA}/detail/${id}'><i class='fas fa-info-circle'></i>&nbsp;Detail</a>`;
                                action += `</div>
                                           </div>`;
                            }

                            var rowNum = parseInt(index) + (parseInt(currentPage) - 1) * parseInt(pageSize);
                            var _unit = 'vnd';
                            var _title = SubStringText.SubTitle(item.Title);
                            var _summary = SubStringText.SubSummary(item.Summary); 
                            rowData += `
                            <tr>
                                 <td class="text-right">${rowNum}&nbsp;</td>  
                                 <td class='tbcol-photo'><img src='${item.ImagePath}' /></td>  
                                 <td class='text-left'><p class='title'>${_title}</p><p class='date'><i class='far fa-clock'></i> ${item.ViewDate} | <i class="fas fa-check-double"></i> ${item.ViewTotal}</p></td>      
                                 <td class="text-left">${item.CategoryName}</td>                                                                                      
                                 <td class='text-right'><p class='price'>${item.Price} <span>${_unit}</span></p><p class='price-listed'>${item.PriceListed}<span> ${_unit}</span></p></td>                                                                                            
                                 <td class='text-center'>${item.State ? "<i class='far fa-check-circle'></i>" : "<i class='far fa-times-circle'></i>"}</td>                                                                                            
                                 <td class="text-center">${HelperModel.StatusIcon(item.Enabled)}</td>
                                 <td class="tbcol-action">${action}</td>
                            </tr>`;
                        });
                        $('tbody#TblData').html(rowData);
                        if (parseInt(totalPage) > 1) {
                            Paging.Pagination("#Pagination", totalPage, currentPage, productController.DataList);
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
                        productController.DataList(pageIndex);
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
    ConfirmDelete: function (id) {
        productController.Delete(id);
    },
    Detail: function () {
        var id = $('#txtID').val();
        if (id.length <= 0) {
            Notifization.Error(MessageText.NotService);
            return;
        }
        var model = {
            ID: id
        };
        $.ajax({
            url: URLC + '/detail',
            data: model,
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
    }
};
productController.init();
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

$(document).on('keyup', '#txtTextID', function () {
    var textId = $(this).val();
    if (textId === '') {
        $('#lblTextID').html('Không được để trống mã sản phẩm');
    }
    else if (textId.length < 5 || textId.length > 5) {
        $('#lblTextID').html('Mã sản phẩm giới hạn từ 5 characters');
    }
    else if (!FormatKeyId.test(textId)) {
        $('#lblTextID').html('Mã sản phẩm không hợp lệ');
    }
    else {
        $('#lblTextID').html('');
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
            $('#lblViewTotal').html('Số lượt xem sản phẩm không hợp lệ');
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
        $('#lblPrice').html('Không được để trống giá sản phẩm');
    }
    else if (!FormatCurrency.test(price)) {
        $('#lblPrice').html('Giá sản phẩm không hợp lệ');
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
$(document).on("change", "#ddlCategoryID", function () {
    var txtCtl = $(this).val();
    if (txtCtl === "") {
        $('#lblCategory').html('Vui lòng chọn danh mục');
    }
    else {
        $('#lblCategory').html('');
    }
});

//$(document).on("change", "#ddlProductWarranty", function () {
//    var txtCtl = $(this).val();
//    if (txtCtl === "") {
//        $('#lblProductWarranty').html('Vui lòng chọn thời gian bảo hành');
//    }
//    else {
//        $('#lblProductWarranty').html('');
//    }
//});
//$(document).on("change", "#ddlProductProvider", function () {
//    var txtCtl = $(this).val();
//    if (txtCtl === "") {
//        $('#lblProductProvider').html('Vui lòng chọn nhà cung cấp');
//    }
//    else {
//        $('#lblProductProvider').html('');
//    }
//});
//$(document).on("change", "#ddlProductState", function () {
//    var txtCtl = $(this).val();
//    if (txtCtl === "") {
//        $('#lblProductState').html('Vui lòng chọn tình trạng');
//    }
//    else {
//        $('#lblProductState').html('');
//    }
//});

$(document).on('click', '#cbxActive', function () {
    if ($(this).hasClass('actived')) {
        // remove
        $(this).children('i').removeClass('fa-check-square');
        $(this).children('i').addClass('fa-square');
        $(this).removeClass('actived');
    }
    else {
        $(this).children('i').addClass('fa-check-square');
        $(this).children('i').removeClass('fa-square');
        $(this).addClass('actived');
    }
});
//$(document).on("click", "#btnFile", function () {
//    $('#ImageFile').click();
//});
//$(document).on("change", "#ImageFile", function (elm) {
//    $('#inputFileControl').html('');
//    $('#lblFile').html('');
//    var _file = $(this)[0].files[0];
//    if (_file !== '') {
//        if (!IsImageFile(_file.name)) {
//            $('#lblFile').html('Tệp tin ảnh không hợp lệ');
//            $(this).val('');
//            $('#inputFileControl').html('');
//        }
//        else {
//            $('#inputFileControl').html(SubStringText.SubFileName(_file.name));
//            ImgPreview(this, '.new-box-preview');
//        }
//    }
//});
//function ImgPreview(inputFile, imgView) {
//    if (inputFile.files && inputFile.files[0]) {
//        var reader = new FileReader();
//        reader.onload = function (e) {
//            //$(imgView).attr('src', e.target.result);
//            $(imgView).css('background-image', 'url(' + e.target.result + ')');
//        };
//        reader.readAsDataURL(inputFile.files[0]);
//    }
//}
$(document).on('', '.img-caption-text', function () {
    $('.new-box-preview img').click();
});
