using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;

namespace WebCore.Entities
{
    [ConnectionString(DbConnect.ConnectionString.CMS)]
    [Table("CMSUserInfo")]
    public partial class CMSUserInfo 
    {
        public CMSUserInfo()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }

        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string UserID { get; set; }
        public string ImageFile { get; set; }
        public string FullName { get; set; }
        public string NickName { get; set; }
        public DateTime Birthday { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
    }
    public partial class CMSUserInfCreateModel
    {
        public string ImageFile { get; set; }
        public string FullName { get; set; }
        public string Nickname { get; set; }
        public string Birthday { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
    }
    public partial class CMSUserInfUpdateModel : CMSUserInfCreateModel
    {
        public string ID { get; set; }
    }

    public partial class CMSUserResult
    {
        public string ID { get; set; }
        public string AreaID { get; set; }
        public string LoginID { get; set; }
        public string TokenID { get; set; }
        public string OTPCode { get; set; }
        public string ImageFile { get; set; }
        public string FullName { get; set; }
        public string Nickname { get; set; }
        public string Birthday { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string LanguageID { get; set; }
        public bool IsBlock { get; set; }
        public string SiteID { get; set; }
        public string CreatedBy { get; set; }
        public int Enabled { get; set; }
        public string CreatedDate { get; set; }
    }
}