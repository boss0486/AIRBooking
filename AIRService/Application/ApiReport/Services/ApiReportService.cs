using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using System;
using Dapper;
using System.Linq;
using PagedList;
using System.Web.Mvc;
using System.Collections.Generic;
using WebCore.Entities;
using Helper;
using System.Web;
using WebCore.Core;
using WebCore.Model.Entities;
using WebCore.Model.Enum;
using WebCore.ENM;
using Helper.File;
using AIRService.WS.Service;
using ApiPortalBooking.Models.VNA_WS_Model;
using AIRService.Models;
using AIR.Helper.Session;
using ApiPortalBooking.Models;



using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Web.Helpers;
using AIRService.WebService.VNA.Authen;
using Helper.Language;
using Helper.TimeData;

namespace WebCore.Services
{
    public interface IApiReportService : IEntityService<DbConnection> { }
    public class ApiReportService : EntityService<DbConnection>, IApiReportService
    {

        public ApiReportService() : base() { }
        public ApiReportService(System.Data.IDbConnection db) : base(db) { }
        //###############################################################################################################################
        public async Task<ActionResult> APIReportDataAsync(APIDailyReportModel model)
        {
            if (model == null)
                return Notifization.Invalid(MessageText.Invalid);
            //
            string userName = model.UserName;
            string password = model.Password;
            string _rpDate = model.ReportDate;

            APIAuthenModel aPIAuthen = new APIAuthenModel();
            if (aPIAuthen.UserName != userName && aPIAuthen.Password != password)
                return Notifization.Invalid(MessageText.AccessDenied);

            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(_rpDate))
                return Notifization.Invalid(MessageText.Invalid);
            //
            if (!Helper.Page.Validate.TestDate(_rpDate))
                return Notifization.Invalid(MessageText.Invalid);

            // var _conn = DbConnect.Connection.CMS;
            _connection.Open();
            using (var _transaction = _connection.BeginTransaction())
            {
                try
                {
                    // check report
                    VNA_TKT_AsrService vna_TKT_AsrService = new VNA_TKT_AsrService();
                    DateTime dateTimeReport = Helper.TimeData.TimeFormat.FormatToServerDate(_rpDate);
                    // delete old report
                    ReportSaleSummaryService reportSaleSummaryService = new ReportSaleSummaryService(_connection);
                    App_ReportSaleSummarySSFopService reportTransactionSSFopService = new App_ReportSaleSummarySSFopService(_connection);
                    ReportTicketingDocumentCouponService reportTicketingDocumentCouponService = new ReportTicketingDocumentCouponService(_connection);
                    ReportTicketingDocumentAmountService reportTicketingDocumentAmountService = new ReportTicketingDocumentAmountService(_connection);
                    ReportTicketingDocumentTaxesService reportTicketingDocumentTaxes = new ReportTicketingDocumentTaxesService(_connection);
                    string sqlQuery = @"SELECT DocumentNumber FROM App_ReportSaleSummary WHERE cast(ReportDate as Date) = cast('" + dateTimeReport + "' as Date)";
                    List<string> lstDocmummentNumber = reportSaleSummaryService.Query<string>(sqlQuery, new { ReportDate = dateTimeReport }, transaction: _transaction).ToList();
                    if (lstDocmummentNumber.Count > 0)
                    {
                        reportSaleSummaryService.Execute("DELETE App_ReportSaleSummary WHERE cast(ReportDate as Date) = cast('" + dateTimeReport + "' as Date)", transaction: _transaction);
                        reportSaleSummaryService.Execute("DELETE App_ReportTicketingDocument_Coupon WHERE cast(ReportDate as Date) = cast('" + dateTimeReport + "' as Date)", transaction: _transaction);
                        reportSaleSummaryService.Execute("DELETE App_ReportTicketingDocument_Amount WHERE cast(ReportDate as Date) = cast('" + dateTimeReport + "' as Date)", transaction: _transaction);
                        reportSaleSummaryService.Execute("DELETE App_ReportTicketingDocument_Taxes WHERE cast(ReportDate as Date) = cast('" + dateTimeReport + "' as Date)", transaction: _transaction);
                    }
                    // 
                    TokenModel tokenModel = VNA_AuthencationService.GetSession();
                    using (var sessionService = new VNA_SessionService(tokenModel))
                    {

                        DesignatePrinterLLSModel designatePrinter = new DesignatePrinterLLSModel
                        {
                            ConversationID = tokenModel.ConversationID,
                            Token = tokenModel.Token
                        };
                        VNA_DesignatePrinterLLSRQService wSDesignatePrinterLLSRQService = new VNA_DesignatePrinterLLSRQService();
                        var printer = wSDesignatePrinterLLSRQService.DesignatePrinterLLS(designatePrinter);
                        // model
                        VNA_EmpReportModel empReportModel = new VNA_EmpReportModel
                        {
                            Token = tokenModel.Token,
                            ConversationID = tokenModel.ConversationID,
                            ReportDate = dateTimeReport
                        };
                        // 
                        var empData = vna_TKT_AsrService.GetEmployeeNumber(empReportModel);
                        if (empData == null || empData.Count() == 0)
                            return Notifization.NotFound("Employee is not found");
                        //
                        foreach (var employee in empData)
                        {
                            string empNumber = employee.IssuingAgentEmployeeNumber;
                            VNA_ReportSaleSummaryResult reportSaleSummaryResult = await vna_TKT_AsrService.ReportSaleSummaryReportAsync(new VNA_ReportModel
                            {
                                ReportDate = dateTimeReport,
                                EmpNumber = empNumber,
                            }, new TransactionModel { TranactionState = true, TokenModel = tokenModel });
                            //get tickketing
                            List<string> docummentNunberForDetails = new List<string>();
                            if (reportSaleSummaryResult != null)
                            {
                                List<ReportSaleSummaryTransaction> reportSaleSummaryTransactions = reportSaleSummaryResult.SaleSummaryTransaction;
                                if (reportSaleSummaryTransactions.Count() > 0)
                                {
                                    foreach (var itemTrans in reportSaleSummaryTransactions)
                                    {
                                        // save sale summary
                                        string reportTransactionId = reportSaleSummaryService.Create<string>(new ReportSaleSummary
                                        {
                                            Title = empNumber,
                                            EmployeeNumber = empNumber,
                                            DocumentType = itemTrans.DocumentType,
                                            DocumentNumber = itemTrans.DocumentNumber,
                                            PassengerName = itemTrans.PassengerName,
                                            PnrLocator = itemTrans.PnrLocator,
                                            TicketPrinterLniata = itemTrans.TicketPrinterLniata,
                                            TransactionTime = itemTrans.TransactionTime,
                                            ExceptionItem = itemTrans.ExceptionItem,
                                            DecoupleItem = itemTrans.DecoupleItem,
                                            TicketStatusCode = itemTrans.TicketStatusCode,
                                            IsElectronicTicket = itemTrans.IsElectronicTicket,
                                            ReportDate = dateTimeReport
                                        }, transaction: _transaction);
                                        ApiPortalBooking.Models.VNA_WS_Model.ReportSaleSummaryTransactionSSFop ePRReportTransactionSSFop = itemTrans.SaleSummaryTransactionSSFop;
                                        if (ePRReportTransactionSSFop != null)
                                        {
                                            await reportTransactionSSFopService.CreateAsync<string>(new ReportSaleSummarySSFop
                                            {
                                                FopCode = ePRReportTransactionSSFop.FopCode,
                                                ReportTransactionID = reportTransactionId,
                                                CurrencyCode = ePRReportTransactionSSFop.CurrencyCode,
                                                FareAmount = ePRReportTransactionSSFop.FareAmount,
                                                TaxAmount = ePRReportTransactionSSFop.TaxAmount,
                                                TotalAmount = ePRReportTransactionSSFop.TotalAmount,
                                            }, transaction: _transaction);
                                        }
                                    }
                                    // group doc 
                                    List<string> docList = reportSaleSummaryTransactions.Where(m => m.DocumentType == "TKT").Select(m => m.DocumentNumber).ToList();
                                    VNA_TKT_AsrService asrService = new VNA_TKT_AsrService();
                                    List<XMLObject.ReportSaleSummay.GetTicketingDocumentRS> getTicketingDocumentRs = asrService.GetTicketingDocumentGroup(docList, new TransactionModel { TranactionState = true, TokenModel = tokenModel });
                                    // **************************************************************************************************************************************************************
                                    if (getTicketingDocumentRs == null)
                                        return Notifization.NotFound(MessageText.NotFound);
                                    // 
                                    foreach (var itemTicketingDocument in getTicketingDocumentRs)
                                    {
                                        List<XMLObject.ReportSaleSummay.Details> details = itemTicketingDocument.Details;
                                        if (details == null)
                                            continue;
                                        //
                                        foreach (var itemDetail in details)
                                        {
                                            string docNumber = itemDetail.Ticket.Number;
                                            XMLObject.ReportSaleSummay.ServiceCoupon serviceCoupon = itemDetail.Ticket.ServiceCoupon;
                                            string departureDateTime = string.Empty;
                                            if (itemDetail.Ticket.ServiceCoupon.FlownCoupon != null)
                                                departureDateTime = itemDetail.Ticket.ServiceCoupon.FlownCoupon.DepartureDateTime.Text;
                                            //
                                            reportTicketingDocumentCouponService.Create<string>(new ReportTicketingDocumentCoupon
                                            {
                                                ReportDate = dateTimeReport,
                                                DocumentNumber = docNumber,
                                                MarketingFlightNumber = serviceCoupon.MarketingFlightNumber.Text,
                                                ClassOfService = serviceCoupon.ClassOfService.Text,
                                                FareBasis = serviceCoupon.FareBasis.Text,
                                                StartLocation = serviceCoupon.StartLocation.Text,
                                                EndLocation = serviceCoupon.EndLocation.Text,
                                                StartDateTime = serviceCoupon.StartDateTime.Text,
                                                EndDateTime = serviceCoupon.EndDateTime.Text,
                                                BookingStatus = serviceCoupon.BookingStatus.Text,
                                                CurrentStatus = serviceCoupon.CurrentStatus.Text,
                                                SystemDateTime = itemDetail.TransactionInfo.SystemDateTime.Text,
                                                FlownCoupon_DepartureDateTime = departureDateTime
                                            }, transaction: _transaction);
                                            //
                                            XMLObject.ReportSaleSummay.Amounts amount = itemDetail.Ticket.Amounts;

                                            double baseAmount = 0;
                                            double totalTax = 0;
                                            double total = 0;
                                            double nonRefundable = 0;

                                            if (amount.New.Base != null)
                                                baseAmount = Convert.ToDouble(amount.New.Base.Amount.Text);
                                            //
                                            if (amount.New.TotalTax != null)
                                                totalTax = Convert.ToDouble(amount.New.TotalTax.Amount.Text);
                                            //
                                            if (amount.New.Total != null)
                                                total = Convert.ToDouble(amount.New.Total.Amount.Text);
                                            //
                                            if (amount.Other != null && amount.Other.NonRefundable != null)
                                                nonRefundable = Convert.ToDouble(amount.Other.NonRefundable.Amount.Text);
                                            //  
                                            reportTicketingDocumentAmountService.Create<string>(new ReportTicketingDocumentAmount
                                            {
                                                ReportDate = dateTimeReport,
                                                DocumentNumber = docNumber,
                                                BaseAmount = baseAmount,
                                                Unit = amount.New.Base.Amount.CurrencyCode,
                                                TotalTax = totalTax,
                                                Total = total,
                                                NonRefundable = nonRefundable,
                                            }, transaction: _transaction);
                                            //
                                            List<XMLObject.ReportSaleSummay.Tax> taxes = itemDetail.Ticket.Taxes.New.Tax;
                                            foreach (var itemTax in taxes)
                                            {
                                                double amountTax = 0;
                                                if (itemTax.Amount != null)
                                                    amountTax = Convert.ToDouble(itemTax.Amount.Text);
                                                //
                                                reportTicketingDocumentTaxes.Create<string>(new ReportTicketingDocumentTaxes
                                                {
                                                    ReportDate = dateTimeReport,
                                                    DocumentNumber = docNumber,
                                                    TaxCode = itemTax.Code,
                                                    Amount = amountTax,
                                                    Unit = itemTax.Amount.CurrencyCode
                                                }, transaction: _transaction);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        _transaction.Commit();
                        return Notifization.Success(MessageText.UpdateSuccess);
                    }
                }
                catch (Exception ex)
                {
                    _transaction.Rollback();
                    return Notifization.Error(MessageText.NotService + ex);
                }
            }
        }

        public ActionResult APIReportDate(SearchModel model)
        {
            if (model == null)
                return Notifization.Invalid(MessageText.Invalid);
            //
            string _startDate = model.StartDate;
            string _endDate = model.EndDate;
            DateTime dateTimeStart = TimeFormat.FormatToServerDateTime(_startDate, LanguagePage.GetLanguageCode);
            DateTime dateTimeEnd = TimeFormat.FormatToServerDateTime(_endDate, LanguagePage.GetLanguageCode);
            if (dateTimeEnd < dateTimeStart)
                return Notifization.Invalid(MessageText.Invalid);
            //
            if (dateTimeEnd > Convert.ToDateTime(TimeHelper.GetUtcDateTimeTx))
                return Notifization.Invalid("T.gian kết thúc phải < t.gian hiện tại");
            //
            List<DateTime> dateTimes = Enumerable.Range(0, 1 + dateTimeEnd.Subtract(dateTimeStart).Days).Select(offset => dateTimeStart.AddDays(offset)).ToList();
            List<string> result = new List<string>();
            //
            foreach (var item in dateTimes)
            {
                result.Add(TimeFormat.FormatToViewDate(item, LanguagePage.GetLanguageCode));
            }
            return Notifization.Data(MessageText.Success, result);
        }

        //public async Task<string> APITest()
        //{
        //    string reportDate = "2020-09-25";
        //    //
        //    using (var client = new HttpClient())
        //    {
        //        client.DefaultRequestHeaders.Accept.Clear();
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        client.BaseAddress = new Uri("http://localhost:44334/");
        //        //
        //        var param = "{'UserName': 'api-booking','Password': '***********', 'ReportDate': '" + reportDate + "' }";
        //        //
        //        HttpContent httpContent = new StringContent(param, Encoding.UTF8, "application/json");
        //        HttpResponseMessage result = await client.PostAsync("APIBooking/Report/Action/EPR-Rpdata", httpContent);
        //        var a = result;
        //        if (result.IsSuccessStatusCode)
        //        {
        //            string statusCode = result.StatusCode.ToString();
        //            string content = result.Content.ToString();
        //            return "";
        //        }
        //        return "";
        //    }
        //}

    }
}