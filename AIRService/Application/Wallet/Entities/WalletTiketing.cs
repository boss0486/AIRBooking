using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;
using WebCore.Model.Entities;

namespace WebCore.Entities
{
    [ConnectionString(DbConnect.ConnectionString.CMS)]
    [Table("App_WalletTiketing")]
    public partial class WalletTiketing
    {
        public WalletTiketing()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string AgentID { get; set; }
        public string UserID { get; set; }
        public double Amount { get; set; }
    }

    // model
    public class WalletTiketingChangeModel
    {
        public string ClientID { get; set; }
        public string UserID { get; set; }
        public double Amount { get; set; }
        public int TransactionType { get; set; }

    }
    public class WalletTiketingIDModel
    {
        public string ID { get; set; }
    }
}