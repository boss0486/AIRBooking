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
        public string Summary { get; set; }
        public double Amount { get; set; }
        public int Status { get; set; }
        public DateTime IssueDate { get; set; }
        public int MailStatus { get; set; }
        public int OrderStatus { get; set; }
        public string ProviderCode { get; set; }
        public DateTime? ExportDate { get; set; }

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
    public class BookPassengerIDModel
    {
        public string ID { get; set; }
        public int TicketType { get; set; }
    }


    public partial class BookOrderResult //: WEBModelResult
    {
        public string ID { get; set; }
        public string BookOrderID { get; set; }
        public string TicketNumber { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public int ItineraryType { get; set; }
        public int CustomerType { get; set; }
        public string ContactName { get; set; }
        public string CompanyID { get; set; }
        public string CompanyCode { get; set; }
        public string AgentCode { get; set; }
        public string TicketingID { get; set; }
        public string TicketingName { get; set; }
        public double AgentFee { get; set; }
        public string ProviderCode { get; set; }
        public double ProviderFee { get; set; }
        public double AgentPrice { get; set; }
        public string AirlineID { get; set; }
        public string PNR { get; set; }
        public double Amount { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExportDate { get; set; }
        public double FareBasic { get; set; }
        public double FareTax { get; set; }
        public double VAT { get; set; }
        public string Unit { get; set; }

        [NotMapped]
        public double TotalAmount => FareBasic + FareTax + VAT + ProviderFee + AgentPrice + AgentFee;
        [NotMapped]
        public string IssueDateText => TimeFormat.FormatToViewDate(IssueDate, Helper.Language.LanguagePage.GetLanguageCode);
        [NotMapped]
        public string ExportDateText => TimeFormat.FormatToViewDate(ExportDate, Helper.Language.LanguagePage.GetLanguageCode);
        [NotMapped]
        public string CustomerTypeText => CustomerTypeService.GetNameByID(CustomerType);
        [NotMapped]
        public string ItineraryText => BookOrderService.ViewOrderItineraryTypeText(ItineraryType);
    }
    public partial class BookingResult //: WEBModelResult
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
        public int CustomerType { get; set; }
        public string ContactName { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyID { get; set; }
        public int Status { get; set; }
        public DateTime IssueDate { get; set; }
        private int ItineraryType { get; set; }
        public int MailStatus { get; set; }
        public int OrderStatus { get; set; }
        [NotMapped]
        public double TotalAmount => FareBasic + FareTax + ProviderFee + AgentPrice + AgentFee;
        [NotMapped]
        public string IssueDateText => TimeFormat.FormatToViewDate(IssueDate, Helper.Language.LanguagePage.GetLanguageCode);
        [NotMapped]
        public string CustomerTypeText => CustomerTypeService.GetNameByID(CustomerType);
        [NotMapped]
        public string ItineraryText => BookOrderService.ViewOrderItineraryTypeText(ItineraryType);
        [NotMapped]
        private string TimeBooking => TimeFormat.FormatToViewDate(Convert.ToDateTime(IssueDate), LanguageCode.Vietnamese.ID);

    }

    public partial class BookingItineraryResult //: WEBModelResult
    {
        public string ID { get; set; }
        public string PNR { get; set; }
        public string CodeID { get; set; }
        public string AirlineID { get; set; }
        //public string Title { get; set; }
        //public string Summary { get; set; }
        //public string Alias { get; set; }
        //public string TicketingID { get; set; }
        public string TicketingName { get; set; }
        public string AgentCode { get; set; }
        public string OriginLocation { get; set; }
        public string DestinationLocation { get; set; }
        //public double AgentFee { get; set; }
        //public double ProviderFee { get; set; }
        //public double AgentPrice { get; set; }
        //public double Amount { get; set; }
        //public double FareBasic { get; set; }
        //public double FareTax { get; set; }
        //public int CustomerType { get; set; }
        //public string ContactName { get; set; }
        //public string CompanyCode { get; set; }
        //public string CompanyID { get; set; }
        //public int Status { get; set; }
        public DateTime IssueDate { get; set; }
        private int ItineraryType { get; set; }
        public int MailStatus { get; set; }
        public int OrderStatus { get; set; }
        public string IssueDateText => TimeFormat.FormatToViewDate(IssueDate, Helper.Language.LanguagePage.GetLanguageCode);
        [NotMapped]
        public string ItineraryText => BookOrderService.ViewOrderItineraryTypeText(ItineraryType);
    }

    public class BookOrderOption
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
    }

    public class BookOrderSearch : SearchModel
    {
        public int OrderStatus { get; set; }
        public int ItineraryType { get; set; }
        public string AgentID { get; set; }
        public string CompanyID { get; set; }
        public int CustomerType { get; set; }
    }
    public class BookChart
    {
        public string AgentID { get; set; }
        public string AirlineID { get; set; }
        public string TimeZoneLocal { get; set; }
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
        private int ItineraryType { get; set; }
        public int OrderStatus { get; set; }
        [NotMapped]
        public string ItineraryText => AirItineraryService.GetNameByID(ItineraryType);
        public int Status { get; set; }
        public BookAgent BookAgent { get; set; }
        public List<BookTicket> BookTickets { get; set; }
        public List<BookPassenger> BookPassengers { get; set; }
        public List<BookCustomer> BookCustomers { get; set; }
        public List<BookPrice> BookPrices { get; set; }
        public List<BookTax> BookTaxs { get; set; }
    }

    public class ChartLine
    {
        public List<string> Labels { get; set; }
        public List<ChartData> Datas { get; set; }
    }
     public class ChartData
    {
        public string Code { get; set; }
        public List<int> Quantities { get; set; }
    }

}