using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;

namespace WebCore.Entities
{
    [ConnectionString(DbConnect.ConnectionString.CMS)]
    [Table("UserInfo")]
    public partial class UserInfo
    {
        public UserInfo()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string UserID { get; set; }
        public string ImageFile { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NickName { get; set; }
        public DateTime Birthday { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
    }

    public partial class UserViewModal
    {
        public string ID { get; set; }
        public string LoginID { get; set; }
        public string TokenID { get; set; }
        public string OTPCode { get; set; }
        public string ImageFile { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Nickname { get; set; }
        public DateTime Birthday { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string IdentifierID { get; set; }
        public string TimekepingID { get; set; }
        public string WorkShiftID { get; set; }
        public string WorkShiftName { get; set; }
        public string DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentPartID { get; set; }
        public string DepartmentPartName { get; set; }
        public string LanguageID { get; set; }
        public bool IsBlock { get; set; }
        public string SiteID { get; set; }
        public string CreatedBy { get; set; }
        public int Enabled { get; set; }
        public DateTime CreatedDate { get; set; }
        public UserViewModal()
        {
        }
        public UserViewModal(string Id, string loginId, string imageFile, string fistName, string lastName, string nickname, DateTime birthday, string email, string phone, string address, string identifierId, string timekepingId, string workShiftId, string workShiftName, string departmentId, string departmentPartId, string languageId, string siteId, string createdBy, bool isBlock, int enabled, DateTime createdDate)
        {
            this.ID = Id;
            this.LoginID = loginId;
            //this.TokenID = tokenId;
            //this.OTPCode = loginId;
            this.ImageFile = imageFile;
            this.FirstName = fistName;
            this.LastName = lastName;
            this.Nickname = nickname;
            this.Birthday = birthday;
            this.Email = email;
            this.Phone = phone;
            this.Address = address;
            this.IdentifierID = identifierId;
            this.TimekepingID = timekepingId;
            this.WorkShiftID = workShiftId;
            this.WorkShiftName = workShiftName;
            this.DepartmentID = departmentId;
            this.DepartmentPartID = departmentPartId;
            this.LanguageID = languageId;
            this.IsBlock = isBlock;
            this.SiteID = siteId;
            this.CreatedBy = createdBy;
            this.Enabled = enabled;
            this.CreatedDate = createdDate;
        }
    }
    public partial class RsUserModel
    {
        public string ID { get; set; }
        public string LoginID { get; set; }
        public string TokenID { get; set; }
        public string OTPCode { get; set; }
        public string RoleGroupID { get; set; }
        public string RoleGroupName { get; set; }
        public string ImageFile { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Nickname { get; set; }
        public DateTime Birthday { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string IdentifierID { get; set; }
        public string TimekepingID { get; set; }
        public string WorkShiftName { get; set; }
        public string DepartmentID { get; set; }
        public string DepartmentPartID { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentPartName { get; set; }
        public string LanguageID { get; set; }
        public bool IsBlock { get; set; }
        public string SiteID { get; set; }
        public string CreatedBy { get; set; }
        public int Enabled { get; set; }
        public string CreatedDate { get; set; }
        public RsUserModel()
        {
        }
        public RsUserModel(string Id, string loginId, string imageFile, string firstName, string lastName, string nickname, DateTime birthday, string email, string phone, string address, string identifierId, string timekepingId, string workShiftName, string departmentId, string departmentPartId, string departmentName, string departmentPartName, string languageId, string siteId, string createdBy, bool isBlock, int enabled, DateTime createdDate)
        {
            this.ID = Id;
            this.LoginID = loginId;
            //this.TokenID = tokenId;
            //this.OTPCode = loginId;
            this.ImageFile = imageFile;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Nickname = nickname;
            this.Birthday = birthday;
            this.Email = email;
            this.Phone = phone;
            this.Address = address;
            this.IdentifierID = identifierId;
            this.TimekepingID = timekepingId;
            this.WorkShiftName = workShiftName;
            this.DepartmentID = departmentId;
            this.DepartmentPartID = departmentPartId;
            this.DepartmentName = departmentName;
            this.DepartmentPartName = departmentPartName;
            this.LanguageID = languageId;
            this.IsBlock = isBlock;
            this.SiteID = siteId;
            this.CreatedBy = createdBy;
            this.Enabled = enabled;
            this.CreatedDate = Helper.Library.FormatDate(createdDate);
        }
    }
    public partial class UserLangID
    {
        public string LanguageID { get; set; }
    }

    // create  model
    public partial class UserInfoCreateFormModel
    {
        public string ImageFile { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Nickname { get; set; }
        public string Birthday { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
    }
    public partial class UserInfoUpdateFormModel : UserInfoCreateFormModel
    {
        public string ID { get; set; }
    }

    public partial class MaxRowNumber
    {
        public int MaxNumber { get; set; }
    }
}