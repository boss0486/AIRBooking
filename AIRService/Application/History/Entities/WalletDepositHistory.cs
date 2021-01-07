using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;
using WebCore.Model.Entities;
using WebCore.Services;

namespace WebCore.Entities
{
    [ConnectionString(DbConnect.ConnectionString.CMS)]
    [Table("App_WalletDepositHistory")]
    public partial class WalletDepositHistory : WEBModel
    {
        public WalletDepositHistory()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string AgentID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
        public double Amount { get; set; }
        public string TransactionID { get; set; }
        public int TransactionType { get; set; }
        public int Status { get; set; }

    }

    // model
    public class WalletDepositHistoryCreateModel
    {
        public string AgentID { get; set; }
        public double Amount { get; set; }
        public double NewBalance { get; set; }
        public string TransactionID { get; set; }
        public int TransactionType { get; set; }
    }
    public class WalletDepositHistoryUpdateModel : WalletDepositHistoryCreateModel
    {
        public string ID { get; set; }
    }
    public class WalletDepositHistoryIDModel
    {
        public string ID { get; set; }
    }
    //
    public partial class WalletDepositHistoryResult : WEBModelResult
    {
        public string ID { get; set; }
        string _agentId { get; set; }
        public string AgentID
        {
            get
            {
                return AirAgentService.GetAgentCodeID(_agentId);
            }
            set
            {
                _agentId = value;
            }
        }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
        public double Amount { get; set; }
        public int TransactionType { get; set; }
        public int Status { get; set; }
    }
}