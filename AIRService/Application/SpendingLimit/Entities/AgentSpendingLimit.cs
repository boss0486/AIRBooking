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
    [Table("App_AgentSpendingLimit")]
    public partial class AgentSpendingLimit : WEBModel
    {
        public AgentSpendingLimit()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string AgentID { get; set; }
        public double Amount { get; set; }
    }
    // model
    public class AgentSpendingLimitSettingModel
    {
        public string AgentID { get; set; }
        public double Amount { get; set; }
        public int Enabled { get; set; }
    } 
    public class AgentSpendingLimitIDModel
    {
        public string ID { get; set; }
    }

    public class AgentSpendingLimitResult : WEBModelResult
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string CodeID { get; set; }
        public string AgentID { get; set; }
        public double Amount { get; set; }
    }
    
    public class PayData  
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public string AgentID { get; set; }
        public string CodeID { get; set; }
        public string Title { get; set; }
        public bool State { get; set; }
        public string PaymentDate { get; set; }

    }
}