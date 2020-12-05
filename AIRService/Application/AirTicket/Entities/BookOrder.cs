using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using Helper.Language;
using Helper.TimeData;
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
    public partial class BookOrderResult //: WEBModelResult
    {
        public string ID { get; set; }
        public string PNR { get; set; }
        public string CodeID { get; set; }
        public string AirlineID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
        public string TicketingID { get; set; }
        public string TicketingName { get; set; }
        public string AgentCode { get; set; }
        public double AgentFee { get; set; }
        public double ProviderFee { get; set; }
        public double AgentPrice { get; set; }
        public double Amount { get; set; }
        public double FareBasic { get; set; }
        public double FareTax { get; set; }


        [NotMapped]
        public double TotalAmount
        {
            get
            {
                double fareBasic = FareBasic;
                if (AgentPrice > FareBasic)
                    fareBasic = AgentPrice;
                //
                return fareBasic + FareTax + ProviderFee + AgentFee;
            }

        }
        public int CustomerType { get; set; }
        public string ContactName { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyID { get; set; }
        public int Status { get; set; }
        private string _orderDate;
        public string OrderDate
        {
            get
            {
                if (_orderDate == null)
                    return "../" + "../" + "..";
                return TimeFormat.FormatToViewDate(Convert.ToDateTime(_orderDate), LanguageCode.Vietnamese.ID);
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
        public string CustomerTypeText => CustomerTypeService.GetNameByID(CustomerType);
        [NotMapped]
        public string ItineraryText => BookOrderService.ViewOrderItineraryTypeText(ItineraryType);
        private string TimeBooking
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_orderDate))
                    return "../" + "../" + "..";
                return TimeFormat.FormatToViewDate(Convert.ToDateTime(_orderDate), LanguageCode.Vietnamese.ID);
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
        public int CustomerType { get; set; }
    }
    public class BookAgentID
    {
        public string AgentID { get; set; }
    }

    public class BookEditPriceModel
    {
        public string ID { get; set; }
        public string TicktingID { get; set; }
        public double Amount { get; set; }
    }
    public class BookEditFeeModel
    {
        public string ID { get; set; }
        public string TicktingID { get; set; }
        public double Amount { get; set; }
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
        public string ItineraryText => AirItineraryService.GetNameByID(ItineraryType);
        public int Status { get; set; }
        public List<BookTicket> BookTickets { get; set; }
        public List<BookPassenger> BookPassengers { get; set; }
        public List<BookCustomer> BookCustomers { get; set; }
        public List<BookPrice> BookPrices { get; set; }
        public List<BookTax> BookTaxs { get; set; }
    }
}