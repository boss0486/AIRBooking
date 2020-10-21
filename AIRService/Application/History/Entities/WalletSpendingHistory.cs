using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;
using WebCore.Model.Entities;

namespace WebCore.Entities
{
    [ConnectionString(DbConnect.ConnectionString.CMS)]
    [Table("App_WalletSpendingHistory")]
    public partial class WalletSpendingHistory : WEBModel
    {
        public WalletSpendingHistory()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string SenderUserID { get; set; }
        public string SenderID { get; set; }
        public string ReceivedID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
        public double Amount { get; set; }
        public int TransactionType { get; set; }
        public int TransactionOriginal { get; set; }
        public int Status { get; set; }

    }

    // model
    public class WalletSpendingHistoryCreateModel
    {
        public string SenderUserID { get; set; }
        public string SenderID { get; set; }
        public string ReceivedID { get; set; }
        public double Amount { get; set; }
        public double NewBalance { get; set; }
        public int TransactionType { get; set; }
        public int TransactionOriginal { get; set; }
    }
    public class WalletSpendingHistoryUpdateModel : WalletSpendingHistoryCreateModel
    {
        public string ID { get; set; }
    }
    public class WalletClientSpendingHistoryIDModel
    {
        public string ID { get; set; }
    }
    //
    public partial class WalletSpendingHistoryResult : WEBModelResult
    {
        public string ID { get; set; }
        public string SenderUserID { get; set; }
        public string SenderID { get; set; }
        public string ReceivedID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
        public double Amount { get; set; }
        public int TransactionType { get; set; }
        public int TransactionOriginal { get; set; }
        public int Status { get; set; }
    }
}