using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;
using WebCore.Model.Entities;

namespace WebCore.Entities
{
 
    [ConnectionString(DbConnect.ConnectionString.CMS)]
    [Table("App_CardCredit")]
    public partial class CardCredit : WEBModel
    {
        public CardCredit()
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
    public class CardCreditCreateModel
    {
        public string Title { get; set; }
        public string Summary { get; set; }
        public int Enabled { get; set; }

    }
    public class CardCreditUpdateModel : CardCreditCreateModel
    {
        public string ID { get; set; }
    }
    public class CardCreditIDModel
    {
        public string ID { get; set; }
    }
    public class CardCreditResult : WEBModelResult
    {

        public string ID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
    }
    public class CardCreditOption
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
    }
}