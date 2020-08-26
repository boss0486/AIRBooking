$(document).ready(function () {
    $('li i').click(function (e) {
        e.preventDefault();
        var parent = $(this).closest("li");

        console.log("::" + $(parent).attr('class'));

        if ($(parent).has("ul")) {
             
            if ($(parent).hasClass('activado')) {
                $(parent).removeClass('activado');
                $(parent).children('ul').slideUp();

            } else {
                $(parent).find('li ul').slideUp();
                $(parent).find('li').removeClass('activado');
                $(parent).addClass('activado');
                $(parent).children('ul').slideDown();
            }
        }

        //$('.menu li ul li a i').click(function() {
        //	window.location.href = $(this).attr('href');
        //})
    });
});