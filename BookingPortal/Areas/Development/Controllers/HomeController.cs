using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebCore.Core;

namespace WebApplication.Development.Controllers
{
    [IsManage(act: false)]
    [RouteArea("Development")]
    [RoutePrefix("Home")]
    public class HomeController : CMSController
    {
        // GET: Development/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}