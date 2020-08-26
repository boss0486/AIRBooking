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
    [Table("UserLogin")]
    public partial class UserLogin
    {
        public UserLogin()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string LoginID { get; set; }
        public string Password { get; set; }
        public string PinCode { get; set; }
        public string TokenID { get; set; }
        public string OTPCode { get; set; }
        public bool IsAdministrator { get; set; }
    }
    //
}