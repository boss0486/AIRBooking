using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;
using WebCore.Model.Entities;

namespace WebCore.Entities
{
    [ConnectionString(DbConnect.ConnectionString.CMS)]
    [Table("App_WalletUser")]
    public partial class WalletUser 
    {
        public WalletUser()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string CustomerID { get; set; }
        public string UserID { get; set; }
        public double Amount { get; set; }
    }

    // model
    public class WalletUserChangeModel
    {
        public string CustomerID { get; set; }
        public string UserID { get; set; }
        public double Amount { get; set; }
        public int TransactionType { get; set; }

    }
    public class WalletUserIDModel
    {
        public string ID { get; set; }
    }
}