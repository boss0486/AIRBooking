using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebCore.Core;

namespace WebApplication.Management.Controllers
{
    [RouteArea("Management")]
    [RoutePrefix("Home-Page")]
    [IsManage(act:false)]
    public class HomeController : Controller
    {
        // GET: Management/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}