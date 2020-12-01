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
        public string AgentID { get; set; }
        public string AirlineID { get; set; }
        public float InlandFee { get; set; }
        public float InternationalFee { get; set; }


    }

    public class AirFeeAgentIDModel
    {
        public string ID { get; set; }
    }

    public class AirAgentFeeConfigModel
    {
        public string AgentID { get; set; }
        public string AirlineID { get; set; }

        public float InlandFee { get; set; }
        public float InternationalFee { get; set; }
    }

    public class AirAgentFeeUpdateModel : AirAgentFeeConfigModel
    {
        public string ID { get; set; }
    }
    public class AirAgentFeeResult : WEBModelResult
    {
        public string ID { get; set; }
        public string AgentID { get; set; }
        public string AirlineID { get; set; }

        [NotMapped]
        public string AgentName
        {
            get
            {
                return CustomerService.GetCustomerName(AgentID);
            }
        }
        public float InlandFee { get; set; }
        public float InternationalFee { get; set; }
    }

    public class AirAgentFeeModel
    {
        public string AgentID { get; set; }
    }
}
