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
    [Table("CMSContact")]
    public class Contact : WEBModel
    {
        [Key]
        [IgnoreInsert]
        [IgnoreUpdate]
        public string ID { get; set; }
        public int Name { get; set; }
        public string Title { get; set; }
        public string ContText { get; set; }
        public string Summary { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }

    public class ContactCreateModelForm : WEBModel
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string ContText { get; set; }
        public string Summary { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
    public class ContactUpdateModelForm : ContactCreateModelForm
    {
        public string ID { get; set; }
    }
    public class Delete  
    {
        public string ID { get; set; }
    }

}
