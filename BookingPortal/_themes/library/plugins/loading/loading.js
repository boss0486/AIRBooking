var dtTime = new Date();
class Loading {
    static PageLoad() {
        $(document).find("body").prepend(`<div id='spinner' data-datetime='${dtTime.getTime()}' class='spinner' ></div>`);
    }
    static ShowLoading() {
        $('#spinner').html("<div class='spinner-block'><div class='spinner spinner-1'></div></div>").addClass("loading").addClass("actived");
    }
    static HideLoading() {
        $('#spinner').html('').removeClass("loading").removeClass("actived");
    }
}

//<div id="spinner" class="spinner loading actived"><div class="spinner-block"><div class="spinner spinner-1"></div></div></div>