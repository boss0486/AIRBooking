using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;
using System.Collections.Generic;
using WebCore.Model.Entities;

namespace WebCore.Entities
{
    [ConnectionString(DbConnect.ConnectionString.CMS)]
    [Table("App_BookOrder")]
    public partial class BookOrder : WEBModel
    {
        public BookOrder()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string PNR { get; set; }
        [IgnoreInsert]
        [IgnoreUpdate]
        public int IndenID { get; set; }
        public string CodeID { get; set; }
        public string AirlineID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
        public string TicketingID { get; set; }
        public string TicketingName { get; set; }
        public string ClientID { get; set; }
        public string ClientCode { get; set; }
        public int Status { get; set; }
    }

    // model
    public class BookOrderCreateModel
    {
        public string OrderID { get; set; }
        public string AirlineID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public int Status { get; set; }
        public int Enabled { get; set; }
    }
    public class BookOrderUpdateModel : BookOrderCreateModel
    {
        public string ID { get; set; }
    }
    public class BookOrderIDModel
    {
        public string ID { get; set; }
    }
    public partial class BookOrderResult : WEBModelResult
    {
        public string ID { get; set; }
        public string PNR { get; set; }
        public int IndenID { get; set; }
        public string CodeID { get; set; }
        public string AirlineID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
        public string TicketingID { get; set; }
        public string TicketingName { get; set; }
        public string ClientID { get; set; }
        public string ClientCode { get; set; }
        public double TotalAmount { get; set; }
        public string ContactName { get; set; }
        public int Status { get; set; }
    }
    
    public class BookOrderOption
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
    }

    public class BookOrderSerch : SearchModel
    {
        
    }

    //details ***************************************************************************************************************
    public partial class ViewBookOrder : WEBModelResult
    {
        public string ID { get; set; }
        public string PNR { get; set; }
        public int IndenID { get; set; }
        public string CodeID { get; set; }
        public string AirlineID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string TicketingID { get; set; }
        public string TicketingName { get; set; }
        public string ClientID { get; set; }
        public string ClientCode { get; set; }
        public int Status { get; set; }
        public List<BookTicket> BookTickets { get; set; }
        public List<BookPassenger> BookPassengers { get; set; }
        public List<BookContact> BookContacts { get; set; }
        public List<BookPrice> BookPrices { get; set; }
        public List<BookTax> BookTaxs { get; set; }
    }

}