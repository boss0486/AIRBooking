using Dapper;
using Helper.Language;
using Helper.TimeData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCore.Model.Entities;
using WebCore.Services;

namespace WebCore.Entities
{
    public partial class UserClient : WEBModel
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
        public bool IsBlock { get; set; } = true;
    }
    public partial class UserClientResult : WEBModelResult
    {
        public string ID { get; set; }
        public string LoginID { get; set; }
        public string TokenID { get; set; }
        public string OTPCode { get; set; }
        public string ImageFile { get; set; }
        public string FullName { get; set; }
        public string Nickname { get; set; }
        private string _birthday;
        public string Birthday
        {
            get
            {
                return TimeFormat.FormatToViewDate(Convert.ToDateTime(_birthday), LanguagePage.GetLanguageCode);
            }
            set
            {
                _birthday = value;
            }
        }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string ClientID { get; set; }
        public int ClientType { get; set; }
        public string ClientCode { get; set; }
        public bool IsBlock { get; set; }
    }
    public class UserClientCreateModel
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
        public string AgentID { get; set; }
        //
        public string LanguageID { get; set; }
        public bool IsBlock { get; set; }
        public int Enabled { get; set; }
    }

    //
    public class UserClientUpdateModel : UserClientCreateModel
    {
        // infor
        public string ID { get; set; }
    }
    //
    public class UserClientIDModel
    {
        public string ID { get; set; }
    }
    //
    public class UserClientOption
    {
        public string ID { get; set; }
        public string Title { get; set; }
    }
    //
    public class UserClientMaxIDModel
    {
        public int MaxID { get; set; }
    }
    // 
    public class UserClientEmailModel
    {
        public string Email { get; set; }
    }
    //
    public class UserClientResetPasswordModel
    {
        public string OTPCode { get; set; }
        public string Password { get; set; }
        public string RePassword { get; set; }
        public string TokenID { get; set; }
    }
    //
    public class UserClientChangePasswordModel
    {
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string ReNewPassword { get; set; }
    }
    //
    public class UserClientChangePinCodeModel
    {
        public string PinCode { get; set; }
        public string NewPinCode { get; set; }
        public string ReNewPinCode { get; set; }
    }
}
