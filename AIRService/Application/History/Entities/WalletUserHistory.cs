using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;
using WebCore.Model.Entities;

namespace WebCore.Entities
{
    [ConnectionString(DbConnect.ConnectionString.CMS)]
    [Table("App_WalletUserHistory")]
    public partial class WalletUserHistory : WEBModel
    {
        public WalletUserHistory()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string CustomerID { get; set; }
        public string UserID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
        public double Amount { get; set; }
        public int TransactionType { get; set; }
        public int Status { get; set; }

    }

    // model
    public class WalletUserHistoryCreateModel
    {
        public string CustomerID { get; set; }
        public string UserID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public double Amount { get; set; }
        public double NewBalance { get; set; }
        public int TransactionType { get; set; }
        public int Status { get; set; }

    }
    public class WalletUserHistoryUpdateModel : WalletUserHistoryCreateModel
    {
        public string ID { get; set; }
    }
    public class WalletUserHistoryIDModel
    {
        public string ID { get; set; }
    }
    //
    public partial class WalletUserHistoryResult : WEBModelResult
    {
        public string ID { get; set; }
        public string CustomerID { get; set; }
        public string UserID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
        public double Amount { get; set; }
        public int TransactionType { get; set; }
        public int Status { get; set; }
    }
}