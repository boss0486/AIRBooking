using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication.HomePage.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Response.Redirect("/Authentication");
            return View();
        }

        public ActionResult About()
        {
            
            ViewBag.Message = "Your application description page. ewqe";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
    }
}