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
    [Table("App_BookEmail")]
    public partial class BookEmail
    {
        public BookEmail()
        {
            ID = Guid.NewGuid().ToString();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }
        public string Summary { get; set; }
    }
    // model
    public class BookEmailCreateModel
    {
        public string Email { get; set; }
        public string Phone { get; set; }
        public string IsActive { get; set; }
        public string Summary { get; set; }
        public int Enabled { get; set; }

    }
    public class BookEmailUpdateModel : BookEmailCreateModel
    {
        public string ID { get; set; }
    }
    public class BookEmailIDModel
    {
        public string ID { get; set; }
    }
}