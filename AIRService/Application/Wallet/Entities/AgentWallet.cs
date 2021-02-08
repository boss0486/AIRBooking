using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;
using WebCore.Model.Entities;

namespace WebCore.Entities
{
    [ConnectionString(DbConnect.ConnectionString.CMS)]
    [Table("App_AgentWallet")]
    public partial class AgentWallet
    {
        public AgentWallet()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string AgentID { get; set; }
        public double Balance { get; set; }
        public bool Unlimited { get; set; }
    }

    // model
    public class AgentWalletChangeModel
    {
        public string AgentID { get; set; }
        public double Amount { get; set; }
        public int TransactionType { get; set; }
    }
}