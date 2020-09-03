
//var input = $("input:text,input:password");
//input.addEventListener("keyup", function (event) {
//    event.preventDefault();
//    if (event.keyCode === 13) {
//        document.getElementById("myBtn").click();
//    }
//});

$(function () {
    $('body').on('keypress', '[data-keyenter], input[type=text], input[type=password]', function (e) {

        console.log('ok');


        if (e.keyCode === 13) {
            var $action = $($(this).parents('[data-keyenter]').data('keyenter'));
            if ($action.length > 0) {
                $action.click();
                $action.focus();
            }
        }
    });
    //
    //$('#txtStartDate').val(today);
    $("[data-datesearch='true'] #txtStartDate").datepicker({
        format: 'dd-mm-yyyy',
        startDate: '01-07-2020',
        todayHighlight: true

    }).on('changeDate', function (index, item) {
        $("[data-datesearch='true'] #ddlTimeExpress")[0].selectedIndex = 0;
        $("[data-datesearch='true'] #ddlTimeExpress").selectpicker('refresh');
       
    });
    $("[data-datesearch='true'] #txtEndDate").datepicker({
        format: 'dd-mm-yyyy',
        startDate: $("[data-datesearch='true'] #txtStartDate").val(),
        todayHighlight: true

    }).on('changeDate', function (index, item) {
        $("[data-datesearch='true'] #ddlTimeExpress")[0].selectedIndex = 0;
        $("[data-datesearch='true'] #ddlTimeExpress").selectpicker('refresh');
    });
    //
    $(document).on('change', "[data-datesearch='true'] #ddlTimeExpress", function () {
        $("[data-datesearch='true'] #txtStartDate").val('');
        $("[data-datesearch='true'] #txtEndDate").val('');
    });
});

function EventCopy(elm, eclick) {
    $(eclick).html('Copied');
    var input = $('.' + elm);
    var success = true,
        range = document.createRange(),
        selection;

    if (window.clipboardData) {
        window.clipboardData.setData("Text", input.html());
    } else {
        // Create a temporary element off screen.
        var tmpElem = $('<div>');
        tmpElem.css({
            position: "absolute",
            left: "-1000px",
            top: "-1000px"
        });
        // Add the input value to the temp element.
        tmpElem.text(input.html());
        $("body").append(tmpElem);
        // Select temp element.
        range.selectNodeContents(tmpElem.get(0));
        selection = window.getSelection();
        selection.removeAllRanges();
        selection.addRange(range);
        // Lets copy.
        try {
            success = document.execCommand("copy", false, null);
        }
        catch (e) {
            // can not copy
        }
        if (success) {

            tmpElem.remove();
        }
    }
}
function goBack() {
    window.history.back();
}











