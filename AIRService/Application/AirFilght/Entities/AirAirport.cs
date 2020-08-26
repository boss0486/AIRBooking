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
    [Dapper.Table("App_AirAirport")]
    public class AirAirport : WEBModel
    {
        public AirAirport()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public string Summary { get; set; }
        public string IATACode { get; set; }
        public string AreaID { get; set; }
    }

    public class AirportIDModel
    {
        public string ID { get; set; }
    }

    public class AirportCreateModel
    {
        public string Title { get; set; }
        public string Alias { get; set; }
        public string Summary { get; set; }
        public string IATACode { get; set; }
        public string AreaID { get; set; }
        public int Enabled { get; set; }
    }

    public class AirportUpdateModel : AirportCreateModel
    {
        public string ID { get; set; }
    }
    public class AirportResult : WEBModelResult
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public string Summary { get; set; }
        public string IATACode { get; set; }
        //
        private string _areaId;
        public string AreaID
        {
            get
            {
                return _areaId;
            }
            set
            {
                _areaId = value;
            }
        }
        [NotMapped]
        public string AreaName
        {
            get
            {
                return AreaGeographicalService.GetAreaName(_areaId); ;
            }
        }
    }
    public class AirportOption
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string IATACode { get; set; }
    }

    public class AirportSearch : SearchModel
    {
        public string ProviceID { get; set; }
    }


}
