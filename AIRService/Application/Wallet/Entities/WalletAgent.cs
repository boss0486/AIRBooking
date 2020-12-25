using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;
using WebCore.Model.Entities;

namespace WebCore.Entities
{
    [ConnectionString(DbConnect.ConnectionString.CMS)]
    [Table("App_WalletAgent")]
    public partial class WalletAgent
    {
        public WalletAgent()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string AgentID { get; set; }
        public int AgentType { get; set; }
        public double SpendingLimit { get; set; }
    }

    // model
    public class WalletAgentChangeModel
    {
        public string AgentID { get; set; }
        public double Amount { get; set; }
        public int TransactionType { get; set; }
    }
    public class WalletAgentUpdateModel : WalletAgentChangeModel
    {
        public string ID { get; set; }
    }
    public class WalletCustomerIDModel
    {
        public string ID { get; set; }
    }
}