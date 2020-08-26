using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;
using System.Web;
using WebCore.Model.Entities;

namespace WebCore.Entities
{


    [ConnectionString(DbConnect.ConnectionString.CMS)]
    [Table("CMSUserSetting")]
    public partial class CMSUserSetting : WEBModel
    {
        public CMSUserSetting()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }

        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string TimekepingID { get; set; }
        public string UserID { get; set; }
        public bool IsLeader { get; set; }
        public bool IsManager { get; set; }
        public string SecurityPassword { get; set; }
        public string AuthenticateID { get; set; }
        public string DepartmentID { get; set; }
        public string DepartmentPartID { get; set; }
        public string IdentifierID { get; set; }
        public string WorkShiftID { get; set; }
        public bool IsBlock { get; set; }
    }



    public class CMSUserCreateModel : WEBModel
    {
        // infor
        public string ImageFile { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NickName { get; set; }
        public string Birthday { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }

        public string TimekepingID { get; set; }
        public string IdentifierID { get; set; }

        // login
        public string LoginID { get; set; }
        public string PassID { get; set; }
        public string RePassword { get; set; }
        // setting
        public string DepartmentID { get; set; }
        public string DepartmentPartID { get; set; }
        public string WorkShiftID { get; set; }
        // payment
        public string BankID { get; set; }
        public string BankAccount { get; set; }

        [Dapper.NotMapped]
        public HttpPostedFileBase DocumentFile { get; set; }
    }

    public class CMSUserUpdateModel
    {
        // infor
        public string ID { get; set; }
        public string ImageFile { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NickName { get; set; }
        public string Birthday { get; set; }

        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }


        public string TimekepingID { get; set; }
        public string IdentifierID { get; set; }

        // login
        public string LoginID { get; set; }
        // setting
        public string DepartmentID { get; set; }
        public string DepartmentPartID { get; set; }
        public string WorkShiftID { get; set; }

        // payment
        public string BankID { get; set; }
        public string AccountNunber { get; set; }
        // file 
        [Dapper.NotMapped]
        public HttpPostedFileBase DocumentFile { get; set; }
        public string LanguageID { get; set; }
        public int Enabled { get; set; }

    }
    public class CMSUserIDModel
    {
        public string ID { get; set; }
    }

    public class CMSUserOption
    {
        public string ID { get; set; }
        public string Title { get; set; }
    }
    public class CMSUserMaxIDModel
    {
        public int MaxID { get; set; }
    }
    // 
    public class CMSUserEmailModel
    {
        public string Email { get; set; }
    }
    public class CMSResetPasswordModel
    {
        public string OTPCode { get; set; }
        public string Password { get; set; }
        public string RePassword { get; set; }
        public string TokenID { get; set; }
    }
}
