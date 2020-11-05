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
    [Dapper.Table("App_Airport")]
    public class Airport : WEBModel
    {
        public Airport()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public string Summary { get; set; }
        public string CategoryID { get; set; }
        public string IATACode { get; set; }
        public double AxFee { get; set; }
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
        public string CategoryID { get; set; }
        public string IATACode { get; set; }
        public double AxFee { get; set; }
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
        public string CategoryID { get; set; }

        [NotMapped]
        public string AreaName
        {
            get
            {
                return AreaGeographicalService.GetAreaName(CategoryID); ;
            }
        }
        public string IATACode { get; set; }
        public double AxFee { get; set; }

    }
    public class AirportOption
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string IATACode { get; set; }
    }

    public class AirportFromToOption
    {
        public string ID { get; set; }
        public string Departure { get; set; }
        public string Destination { get; set; } 
    }

    public class AirportSearch : SearchModel
    {
        public string ProviceID { get; set; }
    }


}
