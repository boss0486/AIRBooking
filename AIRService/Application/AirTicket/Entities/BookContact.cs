using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;
using System.Collections.Generic;
using WebCore.Model.Entities;
using WebCore.Services;

namespace WebCore.Entities
{
    [ConnectionString(DbConnect.ConnectionString.CMS)]
    [Table("App_BookContact")]
    public partial class BookContact
    {
        public BookContact()
        {
            ID = Guid.NewGuid().ToString();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string PNR { get; set; }
        public string BookOrderID { get; set; }
        public int ContactType { get; set; }
        public string Name { get; set; }
        public string Email { get; set; } 
        public string Phone { get; set; }
        public string CompanyID { get; set; }
        public string CompanyCode { get; set; }
    }
    // model
    public class BookContactCreateModel
    {
        public string PNR { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string CompanyID { get; set; }
        public string CompanyCode { get; set; }
    }
    public class BookContactUpdateModel : BookContactCreateModel
    {
        public string ID { get; set; }
    }
    public class BookContactIDModel
    {
        public string ID { get; set; }
    }
}