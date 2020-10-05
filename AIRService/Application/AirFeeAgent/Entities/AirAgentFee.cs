using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AL.NetFrame.Attributes;
using Dapper;
using WebCore.Model.Entities;
using WebCore.Services;

namespace WebCore.Entities
{
    [ConnectionString(DbConnect.ConnectionString.CMS)]
    [Dapper.Table("App_AirAgentFee")]
    public class AirAgentFee : WEBModel
    {
        public AirAgentFee()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public string Summary { get; set; }
        public string AgentID { get; set; }
        public float Amount { get; set; }
    }

    public class AirFeeAgentIDModel
    {
        public string ID { get; set; }
    }

    public class AirFeeAgentConfigModel
    {
        public string AgentID { get; set; }
        public float Amount { get; set; }
    }

    public class AirFeeAgentUpdateModel : AirFeeAgentConfigModel
    {
        public string ID { get; set; }
    }
    public class AirFeeAgentResult : WEBModelResult
    {
        public string ID { get; set; }
        public string AgentID { get; set; }
        public float Amount { get; set; }

    }
}
