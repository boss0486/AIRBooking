using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCore.Model.Entities;
using WebCore.Services;

namespace WebCore.Entities
{

    public partial class AccountModel : WEBModel
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
        public string RoleID { get; set; }
        public bool IsBlock { get; set; }
    }
    public partial class AccountResult : WEBModelResult
    {
        public string ID { get; set; }
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
        public bool IsBlock { get; set; }
    }
    public class AccountCreateModel
    {
        public string FullName { get; set; }
        public string NickName { get; set; }
        public string Birthday { get; set; }
        public string ImageFile { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        // login
        public string LoginID { get; set; }
        public string Password { get; set; }
        public string RePassword { get; set; }
        //
        public string LanguageID { get; set; }
        public bool IsRootAccount { get; set; }
        public string RoleID { get; set; }
        public bool IsBlock { get; set; }
        public int Enabled { get; set; }
    }
    //
    public class AccountUpdateModel : AccountCreateModel
    {
        // infor
        public string ID { get; set; }
    }
    //
    public class AccountIDModel
    {
        public string ID { get; set; }
    }
    //
    public class AccountOption
    {
        public string ID { get; set; }
        public string Title { get; set; }
    }
    //
    public class AccountMaxIDModel
    {
        public int MaxID { get; set; }
    }
    // 
    public class AccountEmailModel
    {
        public string Email { get; set; }
    }
    //
    public class AccountResetPasswordModel
    {
        public string OTPCode { get; set; }
        public string Password { get; set; }
        public string RePassword { get; set; }
        public string TokenID { get; set; }
    }
    //
    public class AccountChangePasswordModel
    {
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string ReNewPassword { get; set; }
    }
    //
    public class AccountChangePinCodeModel
    {
        public string PinCode { get; set; }
        public string NewPinCode { get; set; }
        public string ReNewPinCode { get; set; }
    }
}
