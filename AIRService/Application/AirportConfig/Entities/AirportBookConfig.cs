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
    [Dapper.Table("App_AirportConfig")]
    public class AirportConfig : WEBModel
    {
        public AirportConfig()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string AirportID { get; set; }
        public string IATACode { get; set; }
        public int VoidTicketTime { get; set; }
        public double AxFee { get; set; }
    }

    public class AirportConfigIDModel
    {
        public string ID { get; set; }
    }

    public class AirportConfig_SettingModel
    {
        public string AirportID { get; set; }
        public int TypeID { get; set; }
        public string Value { get; set; }
    }

    public class AirportConfigResult : WEBModelResult
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string IATACode { get; set; }
        public string AirportID { get; set; }
        public int VoidTicketTime { get; set; }
        public double AxFee { get; set; }
    }
     
}
