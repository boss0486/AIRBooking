using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;
using System.Collections.Generic;
using WebCore.Model.Entities;
using WebCore.Services;

namespace WebCore.Entities
{
    [ConnectionString(DbConnect.ConnectionString.CMS)]
    [Table("App_BookFare")]
    public partial class AppBookFare  
    {
        public AppBookFare()
        {
            ID = Guid.NewGuid().ToString();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string BookTicketID { get; set; }
        public string PassengerType { get; set; }
        public string PNR { get; set; }
        public string Title { get; set; }
        public string TaxCode { get; set; }
        public  double Amount { get; set; }
        public string Unit { get; set; }
    }
    // model
    public class AppBookFareCreateModel
    {
        public string BookTickID { get; set; }
        public string PassengerType { get; set; }
        public string PNR { get; set; }
        public string Title { get; set; }
        public string TaxCode { get; set; }
        public double Amount { get; set; }
        public string Unit { get; set; }
    }
    public class AppBookFareUpdateModel : AppBookFareCreateModel
    {
        public string ID { get; set; }
    }
    public class AppBookFareIDModel
    {
        public string ID { get; set; }
    }



    public class RequestAppBookFareModel
    {
        public string PassengerType { get; set; }
        public string Title { get; set; }
        public string TaxCode { get; set; }
        public double Amount { get; set; }
        public string Unit { get; set; }
    }




    //public class AppBookFareFeeBaseModel
    //{
    //    public  int  ADT { get; set; }
    //    public  int  CNN { get; set; }
    //    public  int  INF { get; set; }
    //    public List<AppBookFareFeeBaseSegment> Flights { get; set; }
    //}

    //public class AppBookFareFeeBaseSegment
    //{
    //    public DateTime DepartureDateTime { get; set; }
    //    public DateTime ArrivalDateTime { get; set; }
    //    public int FlightNumber { get; set; }
    //    public int NumberInParty { get; set; }
    //    public string ResBookDesigCode { get; set; }
    //    public int AirEquipType { get; set; }
    //    public string DestinationLocation { get; set; }
    //    public string OriginLocation { get; set; }
    //}
}