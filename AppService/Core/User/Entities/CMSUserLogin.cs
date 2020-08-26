using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;

namespace CMSCore.Entities
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
        public string PassID { get; set; }
        public string PinCode { get; set; }
        public string TokenID { get; set; }
        public string OTPCode { get; set; }
    }
    public class LoginModel
    {
        public string ID { get; set; }
        public string LoginID { get; set; }
        public string PassID { get; set; }
        public string TokenID { get; set; }
        public string OTPCode { get; set; }
        public int Enabled { get; set; }
        public bool IsBlock { get; set; }
        public bool SkipAuthorization { get; set; }
        public string ImageFile { get; set; }
        public string Email { get; set; }
        public bool IsCMSUser { get; set; }
    }
    public class CookiModel
    {
        public string LoginID { get; set; }
        public string PassID { get; set; }
        public bool IsRemember { get; set; }

    }
    public class LoginInForModel

    {
        public string ID { get; set; }
        public string UserID { get; set; }
        public string ImageFile { get; set; }
        public string Birthday { get; set; }
        public string NickName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [NotMapped]
        public string FullName
        {
            get { return FirstName + " " + LastName; }
        }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
    }
    public class LoginQRModel
    {
        public string ID { get; set; }
        public string LoginID { get; set; }
        public string PinCode { get; set; }
        public string TokenID { get; set; }
        public string OTPCode { get; set; }
        public int Enabled { get; set; }
        public bool IsBlock { get; set; }
        public bool SkipAuthorization { get; set; }
    }
    public class UserChangePasswordModel
    {
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string ReNewPassword { get; set; }
    }

    public class UserChangePinCodeModel
    {
        public string PinCode { get; set; }
        public string NewPinCode { get; set; }
        public string ReNewPinCode { get; set; }
    }


    public class LoginReqestModel
    {
        public string LoginID { get; set; }
        public string PassID { get; set; }
        public bool IsRemember { get; set; }
        public string Url { get; set; }

    }
}