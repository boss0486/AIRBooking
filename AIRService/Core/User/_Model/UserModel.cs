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
    public partial class UserModel : WEBModel
    {
        public string ID { get; set; }
        public string AreaID { get; set; }
        public string LoginID { get; set; }
        public string TokenID { get; set; }
        public string OTPCode { get; set; }
        public string ImageFile { get; set; }
        public string FullName { get; set; }
        public string Nickname { get; set; }
        public DateTime Birthday { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string RoleID { get; set; }
        public bool IsBlock { get; set; }
    }
    public partial class UserResult : WEBModelResult
    {
        public string ID { get; set; }
        public string LoginID { get; set; }
        public string ImageFile { get; set; }
        public string FullName { get; set; }
        public string Nickname { get; set; }
        private string _birthday;
        public string Birthday
        {
            get
            {
                return TimeFormat.FormatToViewDate(_birthday, LanguagePage.GetLanguageCode);
            }
            set
            {
                _birthday = value;
            }
        }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public bool IsBlock { get; set; }
        public bool IsAdministrator { get; set; }
    }
    public class UserCreateModel
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
        public bool IsRootUser { get; set; }
        public string RoleID { get; set; }
        public bool IsBlock { get; set; }
        public int Enabled { get; set; }
    }
    //
    public class UserUpdateModel : UserCreateModel
    {
        // infor
        public string ID { get; set; }
    }
    //
    public class UserIDModel
    {
        public string ID { get; set; }
    }
    public class UserRoleSettingModel
    {
        public string UserID { get; set; }
        public string RoleID { get; set; }
    }
    //
    public class UserOption
    {
        public string ID { get; set; }
        public string Title { get; set; }
    }
    //
    public class UserMaxIDModel
    {
        public int MaxID { get; set; }
    }
    // 
    public class UserEmailModel
    {
        public string Email { get; set; }
    }
    //
    public class UserResetPasswordModel
    {
        public string OTPCode { get; set; }
        public string Password { get; set; }
        public string RePassword { get; set; }
        public string TokenID { get; set; }
    }
    //
    public class UserChangePasswordModel
    {
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string ReNewPassword { get; set; }
    }
    //
    public class UserChangePinCodeModel
    {
        public string PinCode { get; set; }
        public string NewPinCode { get; set; }
        public string ReNewPinCode { get; set; }
    }

    public class EmployeeModel
    {
        public string UserID { get; set; }
        public string FullName { get; set; }
        public string ClientID { get; set; }

    }
}
