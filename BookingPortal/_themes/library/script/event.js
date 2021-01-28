
//var input = $("input:text,input:password");
//input.addEventListener("keyup", function (event) {
//    event.preventDefault();
//    if (event.keyCode === 13) {
//        document.getElementById("myBtn").click();
//    }
//});

/**
* Arabic translation for bootstrap-datetimepicker
* Ala' Mohammad <amohammad@birzeit.ecu>
*/

$(function () {
    //
    $('body').on('keypress', '[data-keyenter], input', function (e) {
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
        todayHighlight: true,
        language: 'vi'
    }).on('changeDate', function (index, item) {
        $("[data-datesearch='true'] #ddlTimeExpress")[0].selectedIndex = 0;
        $("[data-datesearch='true'] #ddlTimeExpress").selectpicker('refresh');
        $('.datepicker').hide();
    });
    $("[data-datesearch='true'] #txtEndDate").datepicker({
        format: 'dd-mm-yyyy',
        startDate: $("[data-datesearch='true'] #txtStartDate").val(),
        todayHighlight: true,
        language: 'vi'
    }).on('changeDate', function (index, item) {
        $("[data-datesearch='true'] #ddlTimeExpress")[0].selectedIndex = 0;
        $("[data-datesearch='true'] #ddlTimeExpress").selectpicker('refresh');
        $('.datepicker').hide();
    });
    //
    $(document).on('change', "[data-datesearch='true'] #ddlTimeExpress", function () {
        $("[data-datesearch='true'] #txtStartDate").val('');
        $("[data-datesearch='true'] #txtEndDate").val('');
    });

    //

    $("[data-date='true'], [data-date='false']").datepicker({
        format: 'dd-mm-yyyy',
        startDate: $(this).val(),
        todayHighlight: true,
        language: 'vi'
    }).on('changeDate', function (index, item) {
        $('.datepicker').hide();
        $(this).keyup();
        $(this).change(); 
    });



    //
    $(document).on("blur", "input[data-currency='true']", function () {
        var _crrVal = $(this).val();
        var _val = LibCurrencies.ConvertToCurrency(_crrVal);
        if (!FormatCurrency.test(_val)) {
            $(this).val(_crrVal);
        } else {
            $(this).val(LibCurrencies.FormatToCurrency(_val));
        }
        $(this).keypress();
    });
    // scroll 


    if ($('.scroll-height') != undefined) {
        $.each($('.scroll-height'), function (index, item) {
            var _height = $(this).outerHeight();
            $(item).slimScroll({
                height: _height,
                size: '8px',
                color: "#000",
                railBorderRadius: '2px;',
                borderRadius: '2px;'
            });
        });
    }

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











