using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;
using WebCore.Model.Entities;

namespace WebCore.Entities
{
    [ConnectionString(DbConnect.ConnectionString.CMS)]
    [Table("App_WalletInvestmentHistory")]
    public partial class WalletInvestmentHistory : WEBModel
    {
        public WalletInvestmentHistory()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string ClientID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
        public double Amount { get; set; }
        public int TransactionType { get; set; }
        public int TransactionOriginal { get; set; }
        public int Status { get; set; }

    }

    // model
    public class WalletInvestmentHistoryCreateModel
    { 
        public string ClientID { get; set; }
        public double Amount { get; set; }
        public double NewBalance { get; set; }
        public int TransactionType { get; set; }
        public int TransactionOriginal { get; set; }
    }
    public class WalletInvestmentHistoryUpdateModel : WalletInvestmentHistoryCreateModel
    {
        public string ID { get; set; }
    }
    public class WalletInvestmentHistoryIDModel
    {
        public string ID { get; set; }
    }
    //
    public partial class WalletInvestmentHistoryResult : WEBModelResult
    {
        public string ID { get; set; }
        public string ClientID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
        public double Amount { get; set; }
        public int TransactionType { get; set; }
        public int TransactionOriginal { get; set; }
        public int Status { get; set; }
    }
    public class WalletInvestmentHistoryMessageModel
    {
        public bool Status { get; set; }
        public string Message { get; set; }
    }
}