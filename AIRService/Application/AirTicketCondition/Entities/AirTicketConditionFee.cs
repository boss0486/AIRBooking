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
    [Dapper.Table("App_AirTicketConditionFee")]
    public class AirTicketConditionFee : WEBModel
    {
        public AirTicketConditionFee()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public string Summary { get; set; }
        public string ConditionID { get; set; }
        public int PlaneNoFrom { get; set; }
        public int PlaneNoTo { get; set; }
        public DateTime ?TimeStart { get; set; }
        public DateTime ?TimeEnd { get; set; }
        public bool IsApplied { get; set; }
    }

    public class AirTicketConditionFeeIDModel
    {
        public string ID { get; set; }
    }

    public class AirTicketConditionFeeConfigModel
    {
        public int PlaneNoFrom { get; set; }
        public int PlaneNoTo { get; set; }
        public string TimeStart { get; set; }
        public string TimeEnd { get; set; }
    }
    public class AirTicketConditionEventEndModel
    {
        public string ConditionID { get; set; }
    }
}
