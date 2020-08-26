using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using WebCore.Model.Entities;
using WebCore.Services;

namespace WebCore.Entities
{
    [ConnectionString(DbConnect.ConnectionString.CMS)]
    [Table("CMSRole")]
    public partial class CMSRole : WEBModel
    {
        public CMSRole()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string AreaID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
        public int Level { get; set; }
    }

    // model
    public class CMSRoleCreateModel
    {
        public string AreaID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public int Level { get; set; }
        public int Enabled { get; set; }

    }
    public class CMSRoleUpdateModel : CMSRoleCreateModel
    {
        public string ID { get; set; }
    }
    public class CMSRoleIDModel
    {
        public string ID { get; set; }
    }

    public class CMSRoleAreaIDModel
    {
        public string AreaID { get; set; }
    }
    public class CMSRoleResult : WEBModelResult
    {

        public string ID { get; set; }
        [NotMapped]
        public string AreaID
        {
            get
            {
                return AreaApplicationService.GetAreaID("Development");
            }
        }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
        public int Level { get; set; }
    }
    public class CMSRoleOption
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public int Level { get; set; }

    }
}