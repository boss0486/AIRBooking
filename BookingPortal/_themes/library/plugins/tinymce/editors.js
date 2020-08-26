$(function () {
    //TinyMCE
    tinymce.init({
        selector: ".HtmlEditors",
        theme: "modern",
        height: 500,
        plugins: [
            'advlist autolink lists link image charmap print preview hr anchor pagebreak',
            'searchreplace wordcount visualblocks visualchars code fullscreen',
            'contextmenu directionality',
            'image emoticons template paste textcolor colorpicker textpattern imagetools'
        ],
        toolbar: 'undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image  | print| forecolor backcolor emoticons',
        image_title: true,
        automatic_uploads: false, // enable automatic uploads of images represented by blob or data URIs      
        file_picker_types: 'image',// add custom filepicker only to Image dialog  
        content_css: "/_themes/lib/plugins/tinymce/content.min.css",
        //file_picker_callback: function (cb, value, meta) {
        //    var input = document.createElement('input');
        //    input.setAttribute('type', 'file');
        //    input.setAttribute('accept', 'image/*');
        //    input.onchange = function () {
        //        var file = this.files[0];
        //        input.setAttribute('type', 'file');
        //        this.files[0].mozFullPath;
        //        var reader = new FileReader();
        //        reader.onload = function () {
        //            var id = 'blobid' + (new Date()).getTime();
        //            var blobCache = tinymce.activeEditor.editorUpload.blobCache;
        //            var base64 = reader.result.split(',')[1];
        //            var blobInfo = blobCache.create(id, file, base64);
        //            blobCache.add(blobInfo);
        //            // call the callback and populate the Title field with the file name
        //            cb(blobInfo.blobUri(), { title: file.name });
        //        };
        //        reader.readAsDataURL(file);
        //    };
        //    input.click();
        //}
        file_picker_callback: function (cb, value, meta) {
            var _ctrInput = {
                EditorInputUrl: cb,
                EditorInputVal: value
            };
            FileManager.FileManager(_ctrInput, '', '', FileManager.FileEnum.ALONE);
        }
    });
});
$(function () {
    //TinyMCE
    tinymce.init({
        selector: ".HtmlEditor",
        theme: "modern",
        height: 300,
        menubar: false,
        plugins: [],
        toolbar1: 'bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | forecolor backcolor',
        content_css: "/_themes/lib/plugins/tinymce/content.min.css"
    });
}); 