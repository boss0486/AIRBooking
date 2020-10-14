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
    [Dapper.Table("App_AirTicketCondition05")]
    public class AirTicketCondition05 : WEBModel
    {
        public AirTicketCondition05()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public string Summary { get; set; }
        public string FlightLocationID { get; set; }
        public string ResBookDesigCode { get; set; }
        public int TimePlaceHolder { get; set; }
        public int TimeBookHolder { get; set; }
        public bool IsApplied { get; set; }
    }
    public class AirTicketCondition05ConfigModel
    {
        public string FlightLocationID { get; set; }
        public List<string> ResBookDesigCode { get; set; } = new List<string>();
        public int TimePlaceHolder { get; set; }
        public int TimeBookHolder { get; set; }
    }
    public class AirTicketConditionID05Model
    {
        public string ID { get; set; }
    }public class AirTicketConditionFlightLocationID05Model
    {
        public string FlightLocationID { get; set; }
    }
}
