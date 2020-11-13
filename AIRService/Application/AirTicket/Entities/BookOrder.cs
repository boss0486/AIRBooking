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
        public int ItineraryType { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
        public string TicketingID { get; set; }
        public string TicketingName { get; set; }
        public string AgentID { get; set; }
        public string AgentCode { get; set; }
        public double AgentFee { get; set; }
        public double Amount { get; set; }
        public int Status { get; set; }
        public DateTime OrderDate { get; set; }
        public int MailStatus { get; set; }
        public int OrderStatus { get; set; }
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
        public string CodeID { get; set; }
        public string AirlineID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
        public string TicketingName { get; set; }
        public string AgentCode { get; set; }
        public double AgentFee { get; set; }
        public double Amount { get; set; }
        [NotMapped]
        public double TotalAmount => AgentFee + Amount;
        public string ContactName { get; set; }
        public int Status { get; set; }
        private string _orderDate;
        public string OrderDate
        {
            get
            {
                if (_orderDate == null)
                    return "../" + "../" + "..";
                return Helper.Time.TimeHelper.FormatToDate(Convert.ToDateTime(_orderDate), Helper.Language.LanguageCode.Vietnamese.ID);
            }
            set
            {
                _orderDate = value;
            }
        }
        private int ItineraryType { get; set; }
        public int MailStatus { get; set; }
        public int OrderStatus { get; set; }

        [NotMapped]
        public string CustomerTypeText => BookOrderService.ViewOrderCustomerType(BookContactService.GetBookContactTypeByOrderID(ID));
        [NotMapped]
        public string ItineraryText => BookOrderService.ViewOrderItineraryTypeText(ItineraryType);
        private string TimeBooking
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_orderDate))
                    return "../" + "../" + "..";
                return Helper.Time.TimeHelper.FormatToDate(Convert.ToDateTime(_orderDate), Helper.Language.LanguageCode.Vietnamese.ID);
            }
        }
    }

    public class BookOrderOption
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
    }

    public class BookOrderSerch : SearchModel
    {
        public int OrderStatus { get; set; }
        public int ItineraryType { get; set; }
        public string AgentID { get; set; }
        public string CompanyID { get; set; }
    }
    public class BookAgentID
    {
        public string AgentID { get; set; }
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
        public string AgentID { get; set; }
        public string AgentCode { get; set; }
        public double AgentFee { get; set; }
        private int ItineraryType { get; set; }
        public int OrderStatus { get; set; }
        [NotMapped]
        public string ItineraryText => BookOrderService.ViewOrderCustomerType(ItineraryType);
        public int Status { get; set; }
        public List<BookTicket> BookTickets { get; set; }
        public List<BookPassenger> BookPassengers { get; set; }
        public List<BookContact> BookContacts { get; set; }
        public List<BookPrice> BookPrices { get; set; }
        public List<BookTax> BookTaxs { get; set; }
    }
}