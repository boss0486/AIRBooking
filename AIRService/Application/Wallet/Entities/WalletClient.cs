using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;
using WebCore.Model.Entities;

namespace WebCore.Entities
{
    [ConnectionString(DbConnect.ConnectionString.CMS)]
    [Table("App_WalletClient")]
    public partial class WalletClient 
    {
        public WalletClient()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string ClientID { get; set; }
        public int ClientType { get; set; }
        public double InvestedAmount { get; set; }
        public double SpendingLimitAmount { get; set; }
        public double DepositAmount { get; set; }
        public double SpendingAmount { get; set; }
    }

    // model
    public class WalletClientChangeModel
    {
        public string ClientID { get; set; }
        public double Amount { get; set; }
        public int TransactionType { get; set; }
    }
    public class WalletCustomerUpdateModel : WalletClientChangeModel
    {
        public string ID { get; set; }
    }
    public class WalletCustomerIDModel
    {
        public string ID { get; set; }
    }
}