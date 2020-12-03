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
    [Table("App_Geographical")]
    public partial class AreaInland : WEBModel
    {
        public AreaInland()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string NationalID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
    }

    // model
    public class AreaInlandCreateModel
    {
        public string NationalID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public int Enabled { get; set; }
    }
    public class AreaInlandUpdateModel : AreaInlandCreateModel
    {
        public string ID { get; set; }
    }
    public class AreaInlandIDModel
    {
        public string ID { get; set; }
    }
    public class AreaInlandNationalIDModel
    {
        public string NationalID { get; set; }
    }
    public class AreaInlandResult : WEBModelResult
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string NationalID { get; set; }
        [NotMapped]
        public string NationalName => NationalService.GetNationalName(NationalID);
        public string Summary { get; set; }
        public string Alias { get; set; }
    }
    public class AreaInlandOptionModel
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
    }
}