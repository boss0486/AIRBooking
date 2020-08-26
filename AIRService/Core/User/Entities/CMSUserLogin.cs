using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;

namespace WebCore.Entities
{
    [ConnectionString(DbConnect.ConnectionString.CMS)]
    [Table("CMSUserLogin")]
    public partial class CMSUserLogin
    {
        public CMSUserLogin()
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
    }
    //
}