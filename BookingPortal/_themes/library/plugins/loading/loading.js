var _cnt = 1;
class Loading {
    static PageLoad() {
        $(document).find("body").prepend("<div id='spinner' class='spinner'></div>");
    }
    static ShowLoading() {
        $('#spinner').html("<div class='spinner-block'><div class='spinner spinner-1'></div></div>");
        $('#spinner').addClass("loading");
        $('#spinner').addClass("actived");
    }
    static HideLoading() {
        $('#spinner').html('');
        $('#spinner').removeClass("loading");
        $('#spinner').removeClass("actived");
    }
}

//<div id="spinner" class="spinner loading actived"><div class="spinner-block"><div class="spinner spinner-1"></div></div></div>