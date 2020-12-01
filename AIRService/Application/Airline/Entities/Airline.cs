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
    [Table("App_Airline")]
    public partial class Airline : WEBModel
    {
        public Airline()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string CodeID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
    }

    // model
    public class AirlineCreateModel
    {
        public string CodeID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public int Enabled { get; set; }
    }
    public class AirlineUpdateModel : AirlineCreateModel
    {
        public string ID { get; set; }
    }
    public class AirlineIDModel
    {
        public string ID { get; set; }
    }
    public class AirlineResult : WEBModelResult
    {
        public string ID { get; set; }
        public string CodeID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
    }
    public class AirlineOptionModel
    {
        public string ID { get; set; }
        public string CodeID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
    }
}