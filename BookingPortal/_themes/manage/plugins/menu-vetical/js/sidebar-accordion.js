$(document).ready(function () {

    $(document).on('click', '.menu li:has(ul)', function (e) {
        e.preventDefault();

        if ($(this).hasClass('activado')) {
            $(this).removeClass('activado');
            $(this).children('ul').slideUp();
        } else {
            $('.menu li ul').slideUp();
            $('.menu li').removeClass('activado');
            $(this).addClass('activado');
            $(this).children('ul').slideDown();
        }

        $('.menu li ul li a').click(function () {
            window.location.href = $(this).attr('href');
        })
    });
});
