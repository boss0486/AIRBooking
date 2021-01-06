using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;
using WebCore.Model.Entities;

namespace WebCore.Entities
{
    [ConnectionString(DbConnect.ConnectionString.CMS)]
    [Table("App_TiketingSpendingLimit")]
    public partial class TiketingSpendingLimit
    {
        public TiketingSpendingLimit()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string AgentID { get; set; }
        public string UserID { get; set; }
        public double Amount { get; set; }
        public int Enabled { get; set; }
    }

    // model
    public class TiketingSpendingLimitSettingModel
    {
        public string AgentID { get; set; }
        public string UserID { get; set; }
        public double Amount { get; set; }
        public int Enabled { get; set; }

    }
    public class TiketingSpendingLimitResult : WEBModelResult
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string CodeID { get; set; }
        public string AgentID { get; set; }
        public string UserID { get; set; }
        public double Amount { get; set; }
    }
    public class TiketingSpendingLimitIDModel
    {
        public string ID { get; set; }
    }
}