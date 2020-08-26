using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;
using System.Collections.Generic;
using WebCore.Model.Entities;
using WebCore.Services;

namespace WebCore.Entities
{
    [ConnectionString(DbConnect.ConnectionString.CMS)]
    [Table("App_Area")]
    public partial class AppArea : WEBModel
    {
        public AppArea()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
    }

    // model
    public class AppAreaCreateModel
    {
        public string Title { get; set; }
        public string Summary { get; set; }
        public int Enabled { get; set; }
    }
    public class AppAreaUpdateModel : AppAreaCreateModel
    {
        public string ID { get; set; }
    }
    public class AppAreaIDModel
    {
        public string ID { get; set; }
    }
    public class RsAppArea
    {

        public string ID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
        //
        private string _createdDate;
        public string CreatedDate {
            get
            {
                return Helper.Library.FormatDate(_createdDate); ;
            }
            set
            {
                _createdDate = value ;
            }
        }
        //
        private string _createdBy;
        public string CreatedBy
        {
            get
            {
                return CMSUserInfoService.GetLoginName(_createdBy); ;
            }
            set
            {
                _createdBy = value;
            }
        }
    }
    public class AppAreaOption
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
    }
}