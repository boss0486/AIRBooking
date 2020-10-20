using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;
using WebCore.Model.Entities;

namespace WebCore.Entities
{
    [ConnectionString(DbConnect.ConnectionString.CMS)]
    [Table("App_WalletCustomer")]
    public partial class WalletCustomer 
    {
        public WalletCustomer()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string CustomerID { get; set; }
        public double SpendingLimitAmount { get; set; }
        public double DepositAmount { get; set; }
        public double SpendingAmount { get; set; }
    }

    // model
    public class WalletCustomerChangeModel
    {
        public string CustomerID { get; set; }
        public double Amount { get; set; }
        public int TransactionType { get; set; }
    }
    public class WalletCustomerUpdateModel : WalletCustomerChangeModel
    {
        public string ID { get; set; }
    }
    public class WalletCustomerIDModel
    {
        public string ID { get; set; }
    }
}