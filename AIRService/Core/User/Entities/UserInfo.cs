using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;
using WebCore.Model.Entities;
using WebCore.Services;

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
        public string FullName { get; set; }
        public string NickName { get; set; }
        public DateTime Birthday { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
    }

    // create  model
    public partial class UserInfoCreateModel
    {
        public string ImageFile { get; set; }
        public string FullName { get; set; }
        public string Nickname { get; set; }
        public string Birthday { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
    }
    public partial class UserInfoUpdateModel : UserInfoCreateModel
    {
        public string ID { get; set; }
    }

    public partial class UserMaxRowNumber
    {
        public int MaxNumber { get; set; }
    }
     
}