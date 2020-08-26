var _pageIndex = 0;
var arrList = [];
var _container;
var _typeEnum;
var _captiontext;
var _ctrlInput = null;

class FileManager {
    static FileEnum = {
        NONE: 0,
        ALONE: 1,
        MULTI: 2
    };
    static FileTEnum = {
        NONE: 0,
        EDITTOR: 1
    };
    static FileManager(_tenum, _preview, _captionText, _enum) {
        $("#FileManagerModal").remove();
        $("body").append(FileManager.FileManagerModal());
        $('#FileManagerModal').modal('toggle');
        _container = _preview;
        _typeEnum = _enum;
        _ctrlInput = _tenum;
        _captiontext = _captionText;
    }
    static FileManagerModal() {
        var _modal = `
       <div id='FileManagerModal' data-dismiss='modal' class='file-modal modal fade bd-example-modal-lg file-manager-modal' tabindex='-1' role='dialog' aria-labelledby='myLargeModalLabel' data-backdrop='static' data-keyboard='false' aria-hidden='true' style='display:block; opacity: initial'>
        <div class='modal-dialog modal-lg'>
            <div class='container-fuild modal-container-layout'>
                <div class='modal-header'>
                    <div class='col-md-8'>
                        <h4>File Manage</h4>
                    </div>
                    <div class='col-md-4 align-right'>
                        <div class='ctrl-search-bar'>
                            <input id='txtQueryFile' class='form-control' type='text' placeholder='Nhập từ khóa..1111.' />
                            <div id='btnSearchFile' class='search-icon'>
                                <i class='fa fa-search'></i>
                            </div>
                        </div>
                    </div>
                    <i id='btnCloseModal' data-dismiss='close' class='fas fa-times btn-close'></i>
                </div>
                <div class='modal-body'>
                    <div class='row'>
                        <div class='col-md-3'>
                            <div class='input-group' style='margin-top:15px;'>
                                <input type='file' class='form-control' id='UploadFile' style='display:none;' />
                                <div class='form-control'><i id='btnOpenClientFile' class='far fa-folder'></i> <span id='inputFileControl'>&nbsp;</span></div>
                                <div id='btnUpload' class='input-group-addon'><i class='fas fa-upload'></i></div>
                            </div>
                            <ul class='list-group' style='margin-top:15px;'>
                                <li class='list-group-item'><i class='far fa-folder'></i> Sản phẩm</li>
                                <li class='list-group-item'><i class='far fa-folder'></i> Tin tức</li>
                                <li class='list-group-item'><i class='far fa-folder'></i> *</li>
                            </ul>
                        </div>
                        <div class='col-md-9'><div class='row' id='FileManagerList'>
                            ${FileManager.ItemFileLoad(1)}
                        </div></div>
                    </div>
                </div>
                <div class='modal-footer'>
                    <div id='FileManagerPagination' class='col-md-6'></div><div class='col-md-6 text-right'><button data-dismiss='close' class='btn btn-danger' style='margin: 20px auto' ;>Close</button></div>
                </div>
            </div>
        </div>
    </div>`;
        return _modal;
    }
    static ItemFileLoad(page) {
        var model = {
            Query: $('#txtQuery').val(),
            Page: page
        };
        AjaxFrom.POST({
            url: '/AttachmentFile/DataListByCategoryID',
            data: model,
            async: true,
            success: function (result) {
                $('#FileManagerList').html('');
                $('#Pagination').html('');
                if (result !== null) {
                    if (result.status === 200) {
                        var currentPage = 1;
                        var pagination = result.paging;
                        var totalPage = 0;
                        var pageSize = 0;

                        if (pagination !== null) {
                            totalPage = pagination.TotalPage;
                            currentPage = pagination.Page;
                            pageSize = pagination.PageSize;
                            _pageIndex = pagination.Page;
                        }
                        var rowData = '';
                        $.each(result.data, function (index, item) {
                            index = index + 1;
                            var id = item.ID;
                            if (id.length > 0)
                                id = id.trim();
                            var _name = id + item.Extension;
                            var _imgPath = item.ImagePath;
                            var _caption = SubStringText.SubFileName(_name);
                            var _summary = item.ContentLength + 'k';
                            var _preViewList = $(_container + ' .pre-item-box');
                            var _state = false;

                            if (_ctrlInput !== undefined && _ctrlInput !== null) {
                                if (_ctrlInput.EditorInputVal !== '') {
                                    var filename = _ctrlInput.EditorInputVal.substring(_ctrlInput.EditorInputVal.lastIndexOf('/') + 1);
                                    var _img = id + item.Extension;
                                    if (filename !== '' && _img === filename) {
                                        _state = true;
                                    }
                                }
                            }
                            else {
                                if (_preViewList.length > 0) {
                                    //var itemFile = _preViewList.findIndex(x => $(x).attr('id') !== '' &&  $(x).attr('id').toLowerCase() === 'ibox' + id.toLowerCase());
                                    //if (itemFile > -1) {
                                    //    itemFile(0).attr('id')
                                    //}
                                    $.each(_preViewList, function (index, preItem) {
                                        if ($(this).attr('id') !== '') {
                                            if ($(this).attr('id').toLowerCase() === 'ibox' + id.toLowerCase()) {
                                                _state = true;
                                                var _iBoxId = $(this).attr('id');
                                                _imgPath = $('#' + _iBoxId + " img").attr("src");
                                                var ImgModel = {
                                                    ID: _iBoxId.replace('Ibox', ''),
                                                    Path: _imgPath,
                                                    Caption: _caption,
                                                    Summary: _summary
                                                };
                                                arrList.push(ImgModel);
                                                return;
                                            }
                                        }
                                    });
                                }
                            }
                            var _active = '';
                            var _dtac = 'false';
                            var _iconCheck = 'fa-circle';
                            if (_state) {
                                _active = 'actived';
                                _dtac = 'true';
                                _iconCheck = 'fa-check-circle';
                            }
                            rowData += `
                                            <div class="col-md-3 col-sm-4 col-xs-6" id='Box${id}'>
                                                <div class='item-image ${_active}' data-actived='${_dtac}' id='Item${id}'>
                                                    <div class='image-box'><img class="thumbnail img-responsive img-file" src="${item.ImagePath}"></div>
                                                    <div class='note-box'> <span class='item-caption'>${_caption}</span> <br /> <span class='size-info'>${_summary}</span></div>
                                                    <div class='action-box'><i class="far ${_iconCheck}" onclick="FileManager.FileSelect(this);" data-id='${item.ID}'></i> | <i class="fas fa-file-upload icon-upload"></i> | <i class="far fa-trash-alt icon-trash" data-id='${item.ID}'></i></div>   
                                                </div>
                                            </div>`;
                        });
                        $('#FileManagerList').html(rowData);
                        if (parseInt(totalPage) > 1) {
                            Paging.Pagination("#FileManagerPagination", totalPage, currentPage, FileManager.ItemFileLoad);
                        }
                    }
                }
            }
        });
    }
    static FileSelect(_elm) {
        var ImgModel;
        var _summary;
        var _caption;
        var _imgPath;
        var index;

        var _id = $(_elm).data('id');
        switch (parseInt(_typeEnum)) {
            case FileManager.FileEnum.MULTI:
                if (_ctrlInput !== undefined && _ctrlInput !== null) {
                    alert('not support');
                }
                else {
                    if ($(_elm).hasClass('fa-check-circle')) {
                        $(_elm).removeClass('fa-check-circle').addClass('fa-circle');
                        $('#Item' + _id).removeClass("actived");
                        if (arrList.length > 0) {
                            _imgPath = $('#Item' + _id + " img").attr("src");
                            if (_imgPath !== '' && _imgPath !== undefined) {
                                index = arrList.findIndex(x => x.ID === _id);
                                //var index = arrList.indexOf(_img);
                                if (index > -1) {
                                    arrList.splice(index, 1);
                                    FileManager.PreviewLoad(arrList);
                                }
                            }
                        }
                    }
                    else {
                        $(_elm).removeClass('fa-circle').addClass('fa-check-circle');
                        // actived
                        $('#Item' + _id).addClass("actived");
                        _imgPath = $('#Item' + _id + " img").attr("src");
                        _summary = $('#Item' + _id + " .note-box >.size-info").html();
                        _caption = $('#Item' + _id + " .note-box >.item-caption ").html();
                        if (_imgPath !== '' && _imgPath !== undefined) {
                            ImgModel = {
                                ID: _id,
                                Path: _imgPath,
                                Caption: _caption,
                                Summary: _summary
                            };
                            arrList.push(ImgModel);
                            FileManager.PreviewLoad(arrList);
                        }
                    }
                }
                break;
            case FileManager.FileEnum.ALONE:
                if (_ctrlInput !== undefined && _ctrlInput !== null) {
                    if ($(_elm).hasClass('fa-check-circle')) {
                        $('#FileManagerList .item-image').removeClass("actived");
                        $('#FileManagerList .item-image .action-box .fa-check-circle').removeClass('fa-check-circle').addClass('fa-circle');
                        arrList = [];
                        _ctrlInput.EditorInputUrl('', { title: "" });
                    }
                    else {
                        // remove all active
                        $('#FileManagerList .item-image').removeClass("actived");
                        $('#FileManagerList .item-image .action-box .fa-check-circle').removeClass('fa-check-circle').addClass('fa-circle');
                        $(_elm).removeClass('fa-circle').addClass('fa-check-circle');
                        arrList = [];
                        // actived
                        $('#Item' + _id).addClass("actived");
                        _imgPath = $('#Item' + _id + " img").attr("src");
                        _caption = $('#Item' + _id + " .note-box >.item-caption ").html();
                        if (_imgPath !== '' && _imgPath !== undefined) {
                            _ctrlInput.EditorInputUrl(_imgPath, { title: "" });
                            $('[data-dismiss="close"]').click();
                        }
                    }
                }
                else {
                    if ($(_elm).hasClass('fa-check-circle')) {
                        $('#FileManagerList .item-image').removeClass("actived");
                        $('#FileManagerList .item-image .action-box .fa-check-circle').removeClass('fa-check-circle').addClass('fa-circle');
                        arrList = [];
                        FileManager.PreviewLoad(arrList);
                    }
                    else {
                        // remove all active
                        $('#FileManagerList .item-image').removeClass("actived");
                        $('#FileManagerList .item-image .action-box .fa-check-circle').removeClass('fa-check-circle').addClass('fa-circle');
                        $(_elm).removeClass('fa-circle').addClass('fa-check-circle');
                        arrList = [];
                        // actived
                        $('#Item' + _id).addClass("actived");
                        _imgPath = $('#Item' + _id + " img").attr("src");
                        _summary = $('#Item' + _id + " .note-box >.size-info").html();
                        _caption = $('#Item' + _id + " .note-box >.item-caption ").html();

                        if (_imgPath !== '' && _imgPath !== undefined) {
                            ImgModel = {
                                ID: _id,
                                Path: _imgPath,
                                Caption: _caption,
                                Summary: _summary
                            };
                            arrList.push(ImgModel);
                            FileManager.PreviewLoad(arrList);
                        }
                    }
                }
                break;
            default:
        }
    }
    static PreviewLoad(_listItem) {
        var _htmlItem = '';
        switch (parseInt(_typeEnum)) {
            case FileManager.FileEnum.ALONE:
                if (_listItem.length > 0) {
                    _htmlItem += `<img class="img-responsive" src="${_listItem[0].Path}" data-id='${_listItem[0].ID}' />`;
                    $(_captiontext).html(_listItem[0].Caption);
                }
                $(_container).html(_htmlItem);
                break;
            case FileManager.FileEnum.MULTI:
                if (_listItem.length > 0) {
                    $.each(_listItem, function (index, item) {
                        _htmlItem += `
                        <div class="col-md-4 pre-item-box" id='Ibox${item.ID}'>
                            <div class='item-image' data-actived='false'>
                                <div class='image-box'>
                                    <img class="thumbnail img-responsive" src="${item.Path}" data-id='${item.ID}' />
                                    <i class="fas fa-times icon-delete" data-id='#Ibox${item.ID}'></i>
                                </div>
                                <div class='note-box'>
                                    ${item.Caption}<br />
                                    ${item.Summary}  
                                </div>
                                <div class='file-info'><i class="fas fa-check-circle"></i></div>
                            </div>
                        </div>`;
                    });
                }
                $(_container).html(_htmlItem);
                $(_captiontext).html(` ... <span data-type='itemno'> ${_listItem.length} </span> file`);
                break;
            default:
        }
    }
}
$(document).on('click', '[data-dismiss="close"]', function () {
    $('#FileManagerModal').modal('hide');
    arrList = [];
});
$(document).on('click', '.file-preview .icon-delete', function () {
    var _itembox = $(this).data('id');
    $(_itembox).remove();
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
    $(_captiontext + ' span[data-type="itemno"]').html(arrPhoto.length);
});
$(document).on('click', '#btnUpload', function () {
    var _file = $('#UploadFile')[0].files[0];
    if (_file === '' || _file === undefined) {
        Notifization.Error("Vui lòng chọn file");
        return;
    }
    if (!IsImageFile(_file.name)) {
        Notifization.Error("Tệp tin ảnh không hợp lệ");
        $('#UploadFile').val('');
        $('#inputFileControl').html('');
        return;
    }
    var model = new FormData();
    model.append("DocumentFile", _file);
    AjaxFrom.POSTFILE({
        url: '/AttachmentFile/UploadFile',
        data: model,
        success: function (response) {
            if (response !== null) {
                if (response.status === 200) {
                    Notifization.Success(response.message);
                    $('#inputFileControl').html('');
                    FileManager.ItemFileLoad(_pageIndex);
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
            console.log('::' + response);
        }
    });
});
$(document).on('click', '#btnOpenClientFile', function () {
    $('#UploadFile').click();
});
$(document).on("change", "#UploadFile", function (elm) {
    $('#inputFileControl').html('');
    var _file = $(this)[0].files[0];
    if (_file !== '') {
        if (!IsImageFile(_file.name)) {
            Notifization.Error("Tệp tin ảnh không hợp lệ");
            $(this).val('');
            $('#inputFileControl').html('');
        }
        else {
            $('#inputFileControl').html(SubStringText.SubFileName(_file.name));
        }
    }
});
$(document).on('click', '.icon-trash', function () {
    var _id = $(this).data('id');
    if (_id !== '' && _id !== undefined) {
        var model = {
            ID: _id
        };
        AjaxFrom.POST({
            url: '/AttachmentFile/Delete',
            data: model,
            success: function (response) {
                if (response !== null) {
                    if (response.status === 200) {
                        Notifization.Success(response.message);
                        FileManager.ItemFileLoad(_pageIndex);
                        var index = arrList.findIndex(x => x.ID === _id);
                        if (index > -1) {
                            arrList.splice(index, 1);
                            FileManager.PreviewLoad(arrList);
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
                console.log('::' + MessageText.NotService);
            }
        });
    }
});
