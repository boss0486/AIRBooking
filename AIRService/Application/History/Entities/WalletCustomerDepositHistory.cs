using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;
using WebCore.Model.Entities;

namespace WebCore.Entities
{
    [ConnectionString(DbConnect.ConnectionString.CMS)]
    [Table("App_WalletCustomerDepositHistory")]
    public partial class WalletCustomerDepositHistory : WEBModel
    {
        public WalletCustomerDepositHistory()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string CustomerID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
        public double Amount { get; set; }
        public int TransactionType { get; set; }
        public int TransactionOriginalType { get; set; }
        public string TransactionOriginalID { get; set; }
        public int Status { get; set; }

    }

    // model
    public class WalletCustomerDepositHistoryCreateModel
    {
        public string CustomerID { get; set; }
        public double Amount { get; set; }
        public double NewBalance { get; set; }
        public int TransactionType { get; set; }
        public int TransactionOriginalType { get; set; }
        public string TransactionOriginalID { get; set; }
    }
    public class WalletCustomerDepositHistoryUpdateModel : WalletCustomerDepositHistoryCreateModel
    {
        public string ID { get; set; }
    }
    public class WalletCustomerDepositHistoryIDModel
    {
        public string ID { get; set; }
    }
    //
    public partial class WalletCustomerDepositHistoryResult : WEBModelResult
    {
        public string ID { get; set; }
        public string CustomerID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
        public double Amount { get; set; }
        public int TransactionType { get; set; }
        public int TransactionOriginal { get; set; }
        public string TransactionOriginalID { get; set; }
        public int Status { get; set; }
    }
}