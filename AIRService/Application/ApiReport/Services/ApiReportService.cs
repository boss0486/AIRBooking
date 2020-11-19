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
            string rpDate = model.ReportDate;

            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(rpDate))
                return Notifization.Invalid(MessageText.Invalid);
            //
            APIAuthenModel aPIAuthen = new APIAuthenModel();
            if (aPIAuthen.UserName != userName && aPIAuthen.Password != password)
                return Notifization.Invalid(MessageText.AccessDenied);
            //
            // var _conn = DbConnect.Connection.CMS;
            _connection.Open();
            using (var _transaction = _connection.BeginTransaction())
            {
                try
                {
                    // check report
                    VNA_TKT_AsrService vna_TKT_AsrService = new VNA_TKT_AsrService();
                    DateTime reportDate = Convert.ToDateTime(rpDate);
                    // delete old report
                    ReportSaleSummaryService reportSaleSummaryService = new ReportSaleSummaryService(_connection);
                    string sqlQuery = @"SELECT DocumentNumber FROM App_ReportSaleSummary WHERE cast(ReportDate as Date) = cast('" + reportDate + "' as Date)";
                    List<string> lstDocmummentNumber = reportSaleSummaryService.Query<string>(sqlQuery, new { ReportDate = reportDate }, transaction: _transaction).ToList();
                    if (lstDocmummentNumber.Count > 0)
                    {
                        reportSaleSummaryService.Execute("DELETE App_ReportSaleSummary WHERE ReportDate = @ReportDate AND DocumentNumber IN ('" + String.Join("','", lstDocmummentNumber) + "')", new { ReportDate = reportDate }, transaction: _transaction);
                        reportSaleSummaryService.Execute("DELETE App_ReportTicketingDocument_Coupon WHERE ReportDate = @ReportDate AND DocumentNumber IN ('" + String.Join("','", lstDocmummentNumber) + "')", new { ReportDate = reportDate }, transaction: _transaction);
                        reportSaleSummaryService.Execute("DELETE App_ReportTicketingDocument_Amount WHERE ReportDate = @ReportDate AND DocumentNumber IN ('" + String.Join("','", lstDocmummentNumber) + "')", new { ReportDate = reportDate }, transaction: _transaction);
                        reportSaleSummaryService.Execute("DELETE App_ReportTicketingDocument_Taxes WHERE ReportDate = @ReportDate AND DocumentNumber IN ('" + String.Join("','", lstDocmummentNumber) + "')", new { ReportDate = reportDate }, transaction: _transaction);
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
                            ReportDate = reportDate
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
                                ReportDate = reportDate,
                                EmpNumber = empNumber
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

                                        App_ReportSaleSummarySSFopService reportTransactionSSFopService = new App_ReportSaleSummarySSFopService(_connection);
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
                                            ReportDate = reportDate
                                        }, transaction: _transaction);
                                        ApiPortalBooking.Models.VNA_WS_Model.ReportSaleSummaryTransactionSSFop ePRReportTransactionSSFop = itemTrans.SaleSummaryTransactionSSFop;
                                        if (ePRReportTransactionSSFop != null)
                                        {
                                            await reportTransactionSSFopService.CreateAsync<string>(new ReportSaleSummarySSFop
                                            {
                                                Title = ePRReportTransactionSSFop.FopCode,
                                                ReportTransactionID = reportTransactionId,
                                                CurrencyCode = ePRReportTransactionSSFop.CurrencyCode,
                                                FareAmount = ePRReportTransactionSSFop.FareAmount,
                                                TaxAmount = ePRReportTransactionSSFop.TaxAmount,
                                                TotalAmount = ePRReportTransactionSSFop.TotalAmount,
                                            }, transaction: _transaction);
                                        }
                                        // save tiketting with  documment type value = tkt
                                        string docummentNumber = itemTrans.DocumentNumber;
                                        if (itemTrans.DocumentType == ("TKT") && !string.IsNullOrWhiteSpace(docummentNumber))
                                        {
                                            VNA_ReportSaleSummaryTicketing vna_ReportSaleSummaryTicketing = vna_TKT_AsrService.GetSaleReportTicketByDocNumber(docummentNumber, new TransactionModel { TranactionState = true, TokenModel = tokenModel });
                                            if (vna_ReportSaleSummaryTicketing != null)
                                            {
                                                List<VNA_ReportSaleSummaryTicketingDocument> vna_ReportSaleSummaryTicketingDocuments = vna_ReportSaleSummaryTicketing.SaleSummaryTicketingDocument;
                                                VNA_ReportSaleSummaryTicketingDocumentAmount vna_ReportSaleSummaryTicketingDocumentAmount = vna_ReportSaleSummaryTicketing.SaleSummaryTicketingDocumentAmount;
                                                List<VNA_ReportSaleSummaryTicketingDocumentTaxes> vna_ReportSaleSummaryTicketingDocumentTaxes = vna_ReportSaleSummaryTicketing.SaleSummaryTicketingDocumentTaxes;
                                                //
                                                if (vna_ReportSaleSummaryTicketingDocuments != null && vna_ReportSaleSummaryTicketingDocuments.Count > 0)
                                                {
                                                    ReportTicketingDocumentCouponService reportTicketingDocumentCouponService = new ReportTicketingDocumentCouponService(_connection);
                                                    foreach (var item in vna_ReportSaleSummaryTicketingDocuments)
                                                    {
                                                        reportTicketingDocumentCouponService.Create<string>(new ReportTicketingDocumentCoupon
                                                        {
                                                            ReportDate = reportDate,
                                                            DocumentNumber = docummentNumber,
                                                            MarketingFlightNumber = item.MarketingFlightNumber,
                                                            ClassOfService = item.ClassOfService,
                                                            FareBasis = item.FareBasis,
                                                            StartLocation = item.StartLocation,
                                                            EndLocation = item.EndLocation,
                                                            StartDateTime = item.StartDateTime,
                                                            EndDateTime = item.EndDateTime,
                                                            BookingStatus = item.BookingStatus,
                                                            CurrentStatus = item.CurrentStatus,
                                                            SystemDateTime = item.SystemDateTime,
                                                            FlownCoupon_DepartureDateTime = item.FlownCoupon_DepartureDateTime
                                                        }, transaction: _transaction);
                                                    }
                                                }
                                                //
                                                if (vna_ReportSaleSummaryTicketingDocumentAmount != null)
                                                {
                                                    ReportTicketingDocumentAmountService reportTicketingDocumentAmountService = new ReportTicketingDocumentAmountService(_connection);
                                                    string unit = vna_ReportSaleSummaryTicketingDocumentAmount.Unit;
                                                    double baseAmount = vna_ReportSaleSummaryTicketingDocumentAmount.BaseAmount;
                                                    double totalTax = vna_ReportSaleSummaryTicketingDocumentAmount.TotalTax;
                                                    double total = vna_ReportSaleSummaryTicketingDocumentAmount.Total;
                                                    double nonRefundable = vna_ReportSaleSummaryTicketingDocumentAmount.NonRefundable;
                                                    //
                                                    reportTicketingDocumentAmountService.Create<string>(new ReportTicketingDocumentAmount
                                                    {
                                                        ReportDate = reportDate,
                                                        DocumentNumber = docummentNumber,
                                                        BaseAmount = baseAmount,
                                                        TotalTax = totalTax,
                                                        Total = total,
                                                        NonRefundable = nonRefundable,
                                                        Unit = unit
                                                    }, transaction: _transaction);
                                                }
                                                //
                                                if (vna_ReportSaleSummaryTicketingDocumentTaxes != null && vna_ReportSaleSummaryTicketingDocumentTaxes.Count > 0)
                                                {
                                                    ReportTicketingDocumentTaxesService reportTicketingDocumentTaxes = new ReportTicketingDocumentTaxesService(_connection);
                                                    foreach (var item in vna_ReportSaleSummaryTicketingDocumentTaxes)
                                                    {
                                                        reportTicketingDocumentTaxes.Create<string>(new ReportTicketingDocumentTaxes
                                                        {
                                                            ReportDate = reportDate,
                                                            DocumentNumber = docummentNumber,
                                                            TaxCode = item.TaxCode,
                                                            Amount = item.Amount,
                                                            Unit = item.Unit
                                                        }, transaction: _transaction);
                                                    }
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                        }

                        _transaction.Commit();
                        return Notifization.Success(MessageText.UpdateSuccess + "11");
                    }
                }
                catch (Exception ex)
                {
                    _transaction.Rollback();
                    return Notifization.Error(MessageText.NotService + ex);
                }
            }
        }




        public async Task<ActionResult> APITest()
        {
            try
            {


                string reportDate = "2020-09-22";
                // Update port # in the following line.
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Accept.Clear();
                // var param = "{'UserName': 'api-booking','Password': '***********', 'ReportDate': '" + reportDate + "' }";

                APIDailyReportModel apiDailyReportModel = new APIDailyReportModel
                {
                    UserName = "api-booking",
                    Password = "***********",
                    ReportDate = reportDate,
                };
                var param = JsonConvert.SerializeObject(apiDailyReportModel);
                client.BaseAddress = new Uri("https://localhost:44334/");
                HttpContent httpContent = new StringContent(param, UTF8Encoding.UTF8, "application/json");
                var response = await client.PostAsync("/APIBooking/Report/Action/EPR-Rpdata", httpContent);
                if (response.IsSuccessStatusCode)
                {
                    var a = response.Content.ReadAsStringAsync().Result;

                    return Notifization.Data("OK" + Convert.ToString(response.Content.ReadAsStringAsync().Result), null);

                    //string httpResponse = httpResponseMessage.Content.ReadAsStringAsync().Result;
                    // return Notifization.Data("OK" + response.Content.ReadAsStringAsync().Result, null);
                }
                return Notifization.TEST("::111" + response.IsSuccessStatusCode);

            }
            catch (Exception ex)
            {

                return Notifization.TEST("::" + ex);

            }
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