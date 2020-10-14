var pageIndex = 1;
var URLC = "/Management/ProductProvider/Action";
var URLA = "/Management/ProductProvider";
var productProviderController = {
    init: function () {
        productProviderController.registerEvent();
    },
    registerEvent: function () {
        $('#btnCreate').off('click').on('click', function () {
            var flg = true;
            var txtTitle = $('#txtTitle').val();
            var txtSummary = $('#txtSummary').val();

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
                productProviderController.Create();
            }
            else {
                Notifization.Error(MessageText.Datamissing);
            }
        });
        $('#btnSearch').off('click').on('click', function () {
            productProviderController.DataList(1);
        });
        $('#btnUpdate').off('click').on('click', function () {
            var flg = true;
            var txtTitle = $('#txtTitle').val();
            var txtSummary = $('#txtSummary').val();

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
                productProviderController.Update();
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
                            var action = HelperModel.RolePermission(result.role, "productProviderController", id);
                            //
                            var rowNum = parseInt(index) + (parseInt(currentPage) - 1) * parseInt(pageSize);
                            rowData += `
                            <tr>
                                 <td class="text-right">${rowNum}&nbsp;</td>
                                 <td>${item.Title}</td>                                  
                                 <td>${item.Summary}</td>                                  
                                 <td class='tbcol-created'>${item.CreatedBy}</td>                                  
                                 <td class="text-center">${HelperModel.StatusIcon(item.Enabled)}</td>
                                 <td class="text-center">${item.CreatedDate}</td>
                                 <td class="tbcol-action">${action}</td>
                            </tr>`;
                        });
                        $('tbody#TblData').html(rowData);
                        if (parseInt(totalPage) > 1) {
                            Paging.Pagination("#Pagination", totalPage, currentPage, productProviderController.DataList);
                        }
                        return;
                    }
                    else {
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
        var title = $('#txtTitle').val();
        var summary = $('#txtSummary').val();
        var enabled = 0;
        if ($('input[name="cbxActive"]').is(":checked"))
            enabled = 1;
        //
        var model = {
            Title: title,
            Summary: summary,
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
    Update: function () {
        var title = $('#txtTitle').val();
        var summary = $('#txtSummary').val();
        var id = $('#txtID').val();
        var enabled = 0;
        if ($('input[name="cbxActive"]').is(":checked"))
            enabled = 1;
        //
        var model = {
            Id: id,
            Title: title,
            Summary: summary,
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
            ID: id
        };
        AjaxFrom.POST({
            url: URLC + '/Delete',
            data: model,
            success: function (response) {
                if (response !== null) {
                    if (response.status === 200) {
                        Notifization.Success(response.message);
                        productProviderController.DataList(pageIndex);
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
        Confirm.Delete(id, productProviderController.Delete, null, null);
    }
};

productProviderController.init();
$(document).on('keyup', '#txtTitle', function () {
    var txtTitle = $(this).val();
    if (txtTitle === '') {
        $('#lblTitle').html('Không được để trống tiêu đề');
    }
    else if (txtTitle.length < 1 || txtTitle.length > 80) {
        $('#lblTitle').html('Tiêu đề giới hạn từ 1-> 80 ký tự');
    }
    else if (!FormatKeyword.test(txtTitle)) {
        $('#lblTitle').html('Tiêu đề không hợp lệ');
    }
    else {
        $('#lblTitle').html('');
    }
});
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

