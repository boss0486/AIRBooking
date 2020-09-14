using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using WebCore.Model.Entities;
namespace WebCore.Entities
{
    public class PermissionIDModel
    {
        public string ID { get; set; }
    }

    public class RoleSettingRequest
    {
        public string RouteArea { get; set; }
        public string RoleID { get; set; }
        public List<RoleSettingController> Controllers { get; set; } = null;
    }
    public class RoleSettingController
    {
        public string ID { get; set; }
        public List<string> Action { get; set; }
    }
    //
    public class PerissionControllerModel
    {
        public string KeyID { get; set; }
        public string Title { get; set; }
        public int OrderID { get; set; }

        public List<PerissionActionModel> Action { get; set; }
    }
    public class PerissionActionModel
    {
        public int OrderID { get; set; }
        public string KeyID { get; set; }
        public string Method { get; set; }
        public string APIRouter { get; set; }
        public string Title { get; set; }
    }
}