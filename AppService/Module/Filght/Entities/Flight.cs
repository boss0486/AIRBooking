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
    [Dapper.Table("App_Flight")]
    public class Flight : Model.Entities.WEBModel
    {
        public Flight()
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

    public class FlightIDModel
    {
        public string ID { get; set; }
    }

    public class FlightCreateModel
    {
        public string Title { get; set; }
        public string Alias { get; set; }
        public string Summary { get; set; }
        public string IATACode { get; set; }
        public string AreaID { get; set; }
        public int Enabled { get; set; }
    }

    public class FlightUpdateModel : FlightCreateModel
    {
        public string ID { get; set; }
    }
    public class ViewFlight
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
                return AppAreaService.GetAreaName(_areaId); ;
            }
        }

        public int Enabled { get; set; }
        //
        private string _createdBy;
        public string CreatedBy
        {
            get
            {
                return CMSUserInfoService.GetLoginName(_createdBy); ;
            }
            set
            {
                _createdBy = value;
            }
        }
        //
        private string _createdDate;
        public string CreatedDate
        {
            get
            {
                return Helper.Library.FormatDate(_createdDate); ;
            }
            set
            {
                _createdDate = value;
            }
        }
    }
    public class FlightOption
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string IATACode { get; set; }
    }

    public class FlightSerch : SearchModel
    {
        public string AreaID { get; set; }
        public string ProviceID { get; set; }
    }


}
