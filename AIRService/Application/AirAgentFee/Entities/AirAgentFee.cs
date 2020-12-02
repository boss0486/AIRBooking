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
        public int ItineraryID { get; set; }
        public string NationalID { get; set; }
        public string AgentID { get; set; }
        public string AirlineID { get; set; }
        public double FeeAmount { get; set; }
    }

    public class AirFeeAgentIDModel
    {
        public string ID { get; set; }
    }

    public class AirAgentFeeConfigModel
    {
        public int ItineraryType { get; set; }
        public string NationalID { get; set; }
        public string AgentID { get; set; }
        public List<AirAgentFee_AirlineFee> AirlineFees { get; set; }

    }
    public class AirAgentFee_AirlineFee
    {
        public string AirlineID { get; set; }
        public string AirlineCode { get; set; }
        public string GeographicalID { get; set; }
        public double Amount { get; set; }

    }
    public class AirAgentFeeUpdateModel : AirAgentFeeConfigModel
    {
        public string ID { get; set; }
    }
    public class AirAgentFeeResult : WEBModelResult
    {
        public string ID { get; set; }
        public int ItineraryID { get; set; }
        public string NationalID { get; set; }
        public string AgentID { get; set; }
        public string AirlineID { get; set; }
        // airline
        public string Title { get; set; }
        public string CodeID { get; set; }
        // agent
        public string AgentName { get; set; }
        public string AgentCode { get; set; }
        //
        public float InlandFee { get; set; }
        public float InternationalFee { get; set; }
    }


    public class AirAgentFeeModel
    {
        public string AirlineID { get; set; }
        public double FeeAmount { get; set; }
    }

    public class AirAgentFee_AgentModel
    {
        public string AgentID { get; set; }
    }
    public class AirAgentFee_NationalIDModel
    {
        public string NationalID { get; set; }
        public string AgentID { get; set; }
    }
}
