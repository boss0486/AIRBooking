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
           // Response.Redirect("/Authentication");
            return View("~/Areas/Authentication/Views/Authen/Login.cshtml");
        }
    }
}