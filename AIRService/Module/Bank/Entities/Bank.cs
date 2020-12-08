using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;
using WebCore.Model.Entities;

namespace WebCore.Entities
{
    [ConnectionString(DbConnect.ConnectionString.CMS)]
    [Table("App_Bank")]
    public partial class Bank : WEBModel
    {
        public Bank()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string Title { get; set; }
        public string CodeID { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }

    }

    // model
    public class BankCreateModel
    {
        public string Title { get; set; }
        public string CodeID { get; set; }
        public string Summary { get; set; }
        public int Enabled { get; set; }

    }
    public class BankUpdateModel : BankCreateModel
    {
        public string ID { get; set; }
    }
    public class BankIDModel
    {
        public string ID { get; set; }
    }
    public class BankResult : WEBModelResult
    {

        public string ID { get; set; }
        public string CodeID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
    }
    public class BankOption
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string CodeID { get; set; }
        public string Alias { get; set; }
    }
}