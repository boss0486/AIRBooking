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

    [ConnectionString(DbConnect.ConnectionString.CMS)]
    [Table("App_ClientLogin")]
    public partial class ClientLogin
    {
        public ClientLogin()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string UserID { get; set; }
        public string AgentID { get; set; }
        public bool IsSuper { get; set; }
    }

    public class ClientOption
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string CodeID { get; set; }
    }
    public class ClientProviderOption
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string CodeID { get; set; }
        public bool IsSupplier { get; set; }
    }
}