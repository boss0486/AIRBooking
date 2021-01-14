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
    [Dapper.Table("App_AirportBookConfig")]
    public class AirportBookConfig : WEBModel
    {
        public AirportBookConfig()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string AirportID { get; set; }
        public int VoidBookTime { get; set; }
        public int VoidTicketTime { get; set; }
        public double AxFee { get; set; }
    }

    public class AirportBookConfigIDModel
    {
        public string ID { get; set; }
    }

    public class AirportBookConfig_SettingModel
    {
        public string AirportID { get; set; }
        public int TypeID { get; set; }
        public string Value { get; set; }
    }

    public class AirportBookConfigResult
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string IATACode { get; set; }
        public string AirportID { get; set; }
        public int VoidBookTime { get; set; }
        public int VoidTicketTime { get; set; }
        public double AxFee { get; set; }
    }
}
