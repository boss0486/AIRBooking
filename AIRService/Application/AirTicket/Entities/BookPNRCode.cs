using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;
using System.Collections.Generic;
using WebCore.Model.Entities;

namespace WebCore.Entities
{
    [ConnectionString(DbConnect.ConnectionString.CMS)]
    [Table("App_BookPNRCode")]
    public partial class BookPNRCode : WEBModel
    {
        public BookPNRCode()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string OrderID { get; set; }
        public string AirlineID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
        public int Status { get; set; }

    }

    // model
    public class BookPNRCodeCreateModel
    {
        public string OrderID { get; set; }
        public string AirlineID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public int Status { get; set; }
        public int Enabled { get; set; }
    }
    public class BookPNRCodeUpdateModel : BookPNRCodeCreateModel
    {
        public string ID { get; set; }
    }
    public class BookPNRCodeIDModel
    {
        public string ID { get; set; }
    }
    public class BookPNRCodeOption
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
    }
}