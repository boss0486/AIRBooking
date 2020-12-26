using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;
using WebCore.Model.Entities;
using WebCore.Model.Services;
using WebCore.Services;

namespace WebCore.Entities
{
    [ConnectionString(DbConnect.ConnectionString.CMS)]
    [Table("App_AgentSpendingLimitPayment")]
    public partial class AgentSpendingLimitPayment : WEBModel
    {
        public AgentSpendingLimitPayment()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string AgentID { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public bool State { get; set; }
        public DateTime PaymentDate { get; set; }
        
    }
    // model
    public class AgentSpendingLimitPaymentSettingModel
    {
        public string AgentID { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }

    }
     
    public class AgentSpendingLimitPaymentIDModel
    {
        public string ID { get; set; }
    } 
}