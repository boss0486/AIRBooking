using System.Web;
using System.Web.Optimization;

namespace WebApplication
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            // all #########################################################################################################################################################################
            bundles.Add(new StyleBundle("~/library/css").Include(
                //<!-- Bootstrap Core Css -->
                "~/_themes/library/plugins/bootstrap/css/bootstrap.css",
                //
                "~/_themes/fonts/awesome/css/all.css",
                "~/_themes/library/plugins/toastr/toastr.min.css",
                "~/_themes/library/plugins/loading/style.css",
                "~/_themes/library/plugins/file-manage/file-manage.css",
                "~/_themes/library/plugins/datepickers/bootstrap/datepicker.min.css",
                "~/_themes/library/plugins/datetime/timer/timepicker.css",
                "~/_themes/library/plugins/bootstrap-select/css/bootstrap-select.css",
                "~/_themes/library/plugins/confirm/css/jquery-confirm.min.css",
                // font
                "~/_themes/fonts/font.css",
                "~/_themes/fonts/awesome/css/font-icon.css"
            ));
            // font end #########################################################################################################################################################################
            bundles.Add(new StyleBundle("~/fontend/css").Include(
                 "~/_themes/fontend/library/bootstrap/css/bootstrap.min.css",
                 "~/_themes/library/animate/animate.min.css",
                 "~/_themes/library/ionicons/css/ionicons.css",
                 "~/_themes/library/owlcarousel/assets/owl.carousel.min.css",
                 "~/_themes/library/lightbox/css/lightbox.min.css",
                 "~/_themes/fontend/css/style.css"
                ));
            // back end #########################################################################################################################################################################
            bundles.Add(new StyleBundle("~/backend/css").Include(
                "~/_themes/library/plugins/bootstrap/css/bootstrap.css",
                "~/_themes/backend/css/style.css",
                "~/_themes/library/css/all.css",
                "~/_themes/backend/plugins/node-waves/waves.css",
                "~/_themes/backend/plugins/animate-css/animate.css",
                "~/_themes/backend/css/themes/all-themes.css",
                "~/_themes/backend/site/user.css"
            // font icon
            ));
            // manager #########################################################################################################################################################################
            bundles.Add(new StyleBundle("~/manage/css").Include(
             //<!-- Waves Effect Css -->
             //<link href="~/_themes/library/plugins/node-waves/waves.css" rel="stylesheet" />
             "~/_themes/library/plugins/node-waves/waves.css",
            //<!-- Animation Css -->
            //<link href="~/_themes/library/plugins/animate/animate.css" rel="stylesheet" />
             "~/_themes/library/plugins/animate/animate.css",
            //<!-- Custom Css -->
            //<link href="~/_themes/backend/css/style.css" rel="stylesheet" />
             "~/_themes/manage/css/style.css",
            //<!-- AdminBSB Themes. You can choose a theme from css/themes instead of get all themes -->
            //<link href="~/_themes/backend/css/themes/all-themes.css" rel="stylesheet" />
             "~/_themes/manage/css/themes/all-themes.css"
            ));
            bundles.Add(new ScriptBundle("~/library/jquery").Include(
                "~/_themes/library/plugins/jquery/jquery.min.js",
                "~/_themes/library/plugins/bootstrap/js/bootstrap.js",
                "~/_themes/library/plugins/cookie/jquery.cookie.js",
                "~/_themes/library/plugins/tinymce/tinymce.min.js",
                "~/_themes/library/plugins/tinymce/editors.js",
                // forms   
                "~/_themes/library/plugins/jquery-slimscroll/jquery.slimscroll.js",
                "~/_themes/library/plugins/datepickers/bootstrap/bootstrap-datepicker.min.js",
                "~/_themes/library/plugins/bootstrap-datepicker/js/bootstrap-datepicker.language.js",
                "~/_themes/library/plugins/bootstrap-select/js/bootstrap-select.js",
                "~/_themes/library/plugins/node-waves/waves.js",
                "~/_themes/library/plugins/file-manage/file-manage.js",
                 "~/Library/Script/model-form.js"
            ));
            

            bundles.Add(new ScriptBundle("~/manage/js").Include(

            //<!-- Jquery CountTo Plugin Js -->
            //<script src="~/_themes/backend/plugins/jquery-countto/jquery.countTo.js"></script>
            "~/_themes/manage/plugins/jquery-countto/jquery.countTo.js",

            //<!-- Morris Plugin Js -->
            //<script src="~/_themes/backend/plugins/raphael/raphael.min.js"></script>
            //<script src="~/_themes/backend/plugins/morrisjs/morris.js"></script>
            "~/_themes/manage/plugins/raphael/raphael.min.js",
            "~/_themes/manage/plugins/morrisjs/morris.js",

            //<!-- ChartJs -->
            //<script src="~/_themes/backend/plugins/chartjs/Chart.bundle.js"></script>
            "~/_themes/manage/plugins/chartjs/Chart.bundle.js",


            //<!-- Flot Charts Plugin Js -->
            //<script src="~/_themes/backend/plugins/flot-charts/jquery.flot.js"></script>
            //<script src="~/_themes/backend/plugins/flot-charts/jquery.flot.resize.js"></script>
            //<script src="~/_themes/backend/plugins/flot-charts/jquery.flot.pie.js"></script>
            //<script src="~/_themes/backend/plugins/flot-charts/jquery.flot.categories.js"></script>
            //<script src="~/_themes/backend/plugins/flot-charts/jquery.flot.time.js"></script>

            //<!-- Sparkline Chart Plugin Js -->
            //<script src="~/_themes/backend/plugins/jquery-sparkline/jquery.sparkline.js"></script>
            "~/_themes/manage/plugins/jquery-sparkline/jquery.sparkline.js",

            //<!-- Custom Js -->
            //<script src="~/_themes/backend/js/admin.js"></script>
            //<script src="~/Areas/Template/_script/layout.js"></script>
            "~/Areas/Template/_script/layout.js",
            "~/_themes/manage/js/admin.js",
            //<script src="~/_themes/backend/js/pages/index.js"></script>
            "~/_themes/manage/js/pages/index.js"
            //<!-- Demo Js -->
            //<script src="~/_themes/backend/js/demo.js"></script>
             //"~/_themes/manage/js/demo.js"
            //<!--library-->
            //<script src="~/Areas/Backend/_script/layout.js"></script> 
            ));

            // admin #########################################################################################################################################################################
            bundles.Add(new StyleBundle("~/authen/css").Include(
                "~/_themes/authenzition/plugins/bootstrap/css/bootstrap.css",
                "~/_themes/authenzition/css/style.css",
                "~/_themes/authenzition/plugins/node-waves/waves.css",
                "~/_themes/authenzition/plugins/animate-css/animate.css",
                "~/_themes/authenzition/plugins/webcom-scanner/css/style.css",
                "~/_themes/library/plugins/particles/css/style.css"
            ));

            bundles.Add(new ScriptBundle("~/authentization/js").Include(
                "~/_themes/authenzition/plugins/bootstrap/js/bootstrap.js",
                "~/_themes/authenzition/plugins/node-waves/waves.js",
                "~/_themes/authenzition/js/admin.js"
            // processs
            ));

            // Libray #########################################################################################################################################################################
            bundles.Add(new ScriptBundle("~/library/js").Include(
                // pageding
                "~/_themes/library/plugins/page/jquery.bootpag.min.js",
                // confirm
                "~/_themes/library/plugins/confirm/js/jquery-confirm.min.js",
               //  regex
               "~/_themes/library/script/regex.js",
               // inputmask
               "~/_themes/library/script/jquery.inputmask.bundle.min.js",
               // message
               "~/_themes/library/plugins/toastr/toastr.min.js",
               "~/_themes/library/script/notifies.js",
               // all site
               "~/_themes/library/script/_script.js",
               // event
               "~/_themes/library/script/event.js",
               // loading
               "~/_themes/library/plugins/loading/loading.js"
           ));
            //BundleTable.EnableOptimizations = true;
        }
    }
}