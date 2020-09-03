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
    [Table("AttachmentCategory")]
    public partial class AttachmentCategory : WEBModel
    {
        public AttachmentCategory()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
        public string ControllerID { get; set; }
    }

    // model
    public class AttachmentCategoryCreateModel
    {
        public string Title { get; set; }
        public string Summary { get; set; }
        public string ControllerID { get; set; }

        public int Enabled { get; set; }
    }
    public class AttachmentCategoryUpdateModel : AttachmentCategoryCreateModel
    {
        public string ID { get; set; }
    }
    public class AttachmentCategoryIDModel
    {
        public string ID { get; set; }
    }
    public class AttachmentCategoryResult : WEBModelResult
    {

        public string ID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
        public string ControllerID { get; set; }
    }
    public class AttachmentCategoryOption
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
    }
}