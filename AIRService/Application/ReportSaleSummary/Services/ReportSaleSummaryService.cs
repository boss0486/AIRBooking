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
using Helper.TimeData;
using AIRService.WS.Service;
using ApiPortalBooking.Models.VNA_WS_Model;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.IO;

namespace WebCore.Services
{
    public interface IReportSaleSummaryService : IEntityService<ReportSaleSummary> { }
    public class ReportSaleSummaryService : EntityService<ReportSaleSummary>, IReportSaleSummaryService
    {
        public ReportSaleSummaryService() : base() { }
        public ReportSaleSummaryService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################

        // search date by date report
        public ActionResult DataList(SearchModel model)
        {
            #region
            if (model == null)
                return Notifization.Invalid(MessageText.Invalid);
            //
            int page = model.Page;
            string query = model.Query;
            if (string.IsNullOrWhiteSpace(query))
                query = "";
            //
            string whereCondition = string.Empty;
            //
            SearchResult searchResult = WebCore.Model.Services.ModelService.SearchDefault(new SearchModel
            {
                Query = model.Query,
                TimeExpress = model.TimeExpress,
                Status = model.Status,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Page = model.Page,
                AreaID = model.AreaID,
                TimeZoneLocal = model.TimeZoneLocal
            }, dateColumn: "ReportDate");
            if (searchResult != null)
            {
                if (searchResult.Status == 1)
                    whereCondition = searchResult.Message;
                else
                    return Notifization.Invalid(searchResult.Message);
            }
            #endregion
            //

            if (!Helper.Current.UserLogin.IsCMSUser && !Helper.Current.UserLogin.IsAdminInApplication && !Helper.Current.UserLogin.IsSupplierLogged())
                return Notifization.AccessDenied(MessageText.AccessDenied);
            //
            string typeId = "";
            string sqlQuery = @"
            SELECT rps.*,rpsf.FareAmount, rpsf.TaxAmount, rpsf.TotalAmount FROM App_ReportSaleSummary as rps 
            LEFT JOIN App_ReportSaleSummarySSFop as rpsf 
            ON rpsf.ReportTransactionID =  rps.ID
            WHERE (EmployeeNumber LIKE N'%'+ @Query +'%' OR DocumentNumber LIKE N'%'+ @Query +'%' OR dbo.Uni2NONE(PassengerName) LIKE N'%'+ @Query +'%')"
            + whereCondition + " ORDER BY rps.[CreatedDate]";
            var dtList = _connection.Query<ReportSaleSummaryResult>(sqlQuery, new { Query = Helper.Page.Library.FormatNameToUni2NONE(query), TypeID = typeId }).ToList();
            if (dtList.Count == 0)
                return Notifization.NotFound(MessageText.NotFound);
            //     
            var result = dtList.ToPagedList(page, Helper.Pagination.Paging.PAGESIZE).ToList();
            if (dtList.Count == 0 && page > 1)
            {
                page -= 1;
                result = dtList.ToPagedList(page, Helper.Pagination.Paging.PAGESIZE).ToList();
            }
            if (result.Count == 0)
                return Notifization.NotFound(MessageText.NotFound);

            Helper.Pagination.PagingModel pagingModel = new Helper.Pagination.PagingModel
            {
                PageSize = Helper.Pagination.Paging.PAGESIZE,
                Total = dtList.Count,
                Page = page
            };
            //
            return Notifization.Data(MessageText.Success, data: result, role: RoleActionSettingService.RoleListForUser(), paging: pagingModel);
        }
        //
        // search date by departure day

        public List<AirPassengerReult> DepartureData(ReportEprSearchModel model)
        {
            #region
            if (model == null)
                return new List<AirPassengerReult>();
            // 
            string query = model.Query;
            if (string.IsNullOrWhiteSpace(query))
                query = "";
            // 
            string currentStatus = model.CurrentStatus;
            string whereCondition = string.Empty;
            SearchResult searchResult = SearchReport(new SearchModel
            {
                Query = model.Query,
                TimeExpress = model.TimeExpress,
                Status = model.Status,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Page = model.Page,
                AreaID = "",
                TimeZoneLocal = model.TimeZoneLocal
            }, columName1: "StartDateTime");
            //
            if (searchResult != null)
            {
                if (searchResult.Status == 1)
                    whereCondition = searchResult.Message;
                else
                    return new List<AirPassengerReult>();
            }
            if (!string.IsNullOrWhiteSpace(currentStatus))
            {
                whereCondition += " AND CurrentStatus = '" + currentStatus + "'";
            }
            #endregion
            //

            string sqlQuery = @"
             SELECT rtdc.ID, rtdc.MarketingFlightNumber, rtdc.ClassOfService, rtdc.ClassOfService, rtdc.FareBasis, rtdc.StartLocation, rtdc.EndLocation, rtdc.StartLocation, rtdc.StartDateTime, rtdc.EndDateTime, rtdc.BookingStatus, rtdc.CurrentStatus  
             ,rps.ID as 'ReportSaleSummaryID', rps.EmployeeNumber, rps.DocumentType, rps.DocumentNumber, rps.PassengerName, rps.PnrLocator, rps.TicketPrinterLniata, rps.TransactionTime, rps.ExceptionItem, rps.DecoupleItem, rps.TicketStatusCode, rps.IsElectronicTicket, rps.ReportDate 
             FROM App_ReportSaleSummary as rps 
             INNER JOIN App_ReportTicketingDocument_Coupon as rtdc ON rtdc.DocumentNumber =  rps.DocumentNumber   
             WHERE (dbo.Uni2NONE(rps.PassengerName) LIKE N'%'+ @Query +'%' OR rps.DocumentNumber LIKE N'%'+ @Query +'%') " + whereCondition + " ORDER BY rps.[CreatedDate]";
            var dtList = _connection.Query<AirPassengerReult>(sqlQuery, new { Query = Helper.Page.Library.FormatNameToUni2NONE(query) }).ToList();
            return dtList;

        }
        public ActionResult DataListByDeparture(ReportEprSearchModel model)
        {
            if (!Helper.Current.UserLogin.IsCMSUser && !Helper.Current.UserLogin.IsAdminInApplication && !Helper.Current.UserLogin.IsSupplierLogged())
                return Notifization.Invalid(MessageText.AccessDenied);
            //
            List<AirPassengerReult> dtList = DepartureData(model);
            if (dtList.Count == 0)
                return Notifization.NotFound(MessageText.NotFound);
            //     
            int page = model.Page;
            var result = dtList.ToPagedList(page, Helper.Pagination.Paging.PAGESIZE).ToList();
            if (dtList.Count == 0 && page > 1)
            {
                page -= 1;
                result = dtList.ToPagedList(page, Helper.Pagination.Paging.PAGESIZE).ToList();
            }
            if (result.Count == 0)
                return Notifization.NotFound(MessageText.NotFound);
            //
            Helper.Pagination.PagingModel pagingModel = new Helper.Pagination.PagingModel
            {
                PageSize = Helper.Pagination.Paging.PAGESIZE,
                Total = dtList.Count,
                Page = page
            };
            //
            return Notifization.Data(MessageText.Success, data: result, role: RoleActionSettingService.RoleListForUser(), paging: pagingModel);
        }

        public ActionResult EPRExportDeparture(ReportEprSearchModel model)
        {

            List<AirPassengerReult> dtList = DepartureData(model);
            if (dtList.Count == 0)
                return null;
            //     
            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                string fileName = "DANH SÁCH BÁO CÁO NGÀY BAY";
                string alias = Helper.Page.Library.FormatToUni2NONE(fileName);
                var workSheet = excelPackage.Workbook.Worksheets.Add(Helper.Page.Library.FormatToUni2NONE(fileName).ToUpper());
                workSheet.TabColor = System.Drawing.Color.Black;
                workSheet.DefaultRowHeight = 12;
                //Header of table   
                workSheet.Cells["A1:H1"].Merge = true;
                workSheet.Cells["A1:H1"].Value = fileName;
                workSheet.Cells["A1:H1"].Style.Font.Name = "Arial Narrow";
                workSheet.Cells["A1:H1"].Style.Font.Size = 16;
                workSheet.Row(1).Height = 35; 
                workSheet.Cells["A1:H1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Cells["A1:H1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells["A1:H1"].Style.Font.Bold = true;
                workSheet.Cells["A1:H1"].Style.Font.Color.SetColor(Color.White);
                workSheet.Cells["A1:H1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                workSheet.Cells["A1:H1"].Style.Fill.BackgroundColor.SetColor(Color.Teal);
                workSheet.Cells["A1:H1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                workSheet.Cells["A1:H1"].Style.Border.Bottom.Color.SetColor(Color.Gray);
                //title of table  
                workSheet.Row(2).Height = 20;
                workSheet.Cells[2, 1].Value = "#";
                workSheet.Cells[2, 2].Value = "PnrLocator";
                workSheet.Cells[2, 3].Value = "FareBasis";
                workSheet.Cells[2, 4].Value = "Tên hành khách";
                workSheet.Cells[2, 5].Value = "Document Number";
                workSheet.Cells[2, 6].Value = "T.Gian khởi hành";
                workSheet.Cells[2, 7].Value = "B-Status";
                workSheet.Cells[2, 8].Value = "C-Status";
                //Body of table  
                workSheet.Cells["A2:H2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                workSheet.Cells["A2:H2"].Style.Fill.BackgroundColor.SetColor(Color.Teal); 
                workSheet.Cells["A2:H2"].Style.Font.Color.SetColor(Color.White);
                workSheet.Cells["A2:H2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                int recordIndex = 3;
                int cnt = 1;
                foreach (var item in dtList)
                {
                    workSheet.Cells[recordIndex, 1].Value = cnt;
                    workSheet.Cells[recordIndex, 2].Value = item.PnrLocator;
                    workSheet.Cells[recordIndex, 3].Value = item.FareBasis;
                    workSheet.Cells[recordIndex, 4].Value = item.PassengerName;
                    workSheet.Cells[recordIndex, 5].Value = item.DocumentNumber;
                    workSheet.Cells[recordIndex, 6].Value = item.StartDateTime;
                    workSheet.Cells[recordIndex, 7].Value = item.BookingStatus;
                    workSheet.Cells[recordIndex, 8].Value = item.CurrentStatus;
                    ////// attribute
                    ////workSheet.Cells[recordIndex, 7].Style.Font.Color.SetColor(Color.White);
                    ////workSheet.Cells[recordIndex, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ////workSheet.Cells[recordIndex, 7].Style.Fill.BackgroundColor.SetColor(Color.SpringGreen);
                    //////
                    ////workSheet.Cells[recordIndex, 8].Style.Font.Color.SetColor(Color.White);
                    ////workSheet.Cells[recordIndex, 8].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ////workSheet.Cells[recordIndex, 8].Style.Fill.BackgroundColor.SetColor(Color.Azure);

                    //workSheet.Cells[recordIndex, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                    recordIndex++;
                    cnt++;
                }
                workSheet.Column(1).AutoFit();
                workSheet.Column(2).AutoFit();
                workSheet.Column(3).AutoFit();
                workSheet.Column(4).AutoFit();
                workSheet.Column(5).AutoFit();
                workSheet.Column(6).AutoFit();
                workSheet.Column(7).AutoFit();
                workSheet.Column(8).AutoFit();
                // create file
                string outFolder = "/Files/Export/VnaReport/";
                string pathFile = Helper.File.AttachmentFile.AttachmentExls(alias, excelPackage, outFolder);
                if (string.IsNullOrWhiteSpace(pathFile))
                    return null;
                //

                return Notifization.DownLoadFile(MessageText.DownLoad, pathFile);
                //return fileContentResult;

                //return Notifization.DownLoadFile(MessageText.DownLoad, pathFile);
                //using (var memoryStream = new MemoryStream())
                //{
                //    HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //    HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=" + excelName + ".xlsx");
                //    excel.SaveAs(memoryStream);
                //    memoryStream.WriteTo(HttpContext.Current.Response.OutputStream);
                //    HttpContext.Current.Response.Flush();
                //    HttpContext.Current.Response.End();


                //}

                //
            }
        }



        public ReportSaleSummaryModel GetReportSaleSummaryByDocumentNumber(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return new ReportSaleSummaryModel();
            //
            id = id.ToLower();
            ReportSaleSummaryService reportSaleSummaryService = new ReportSaleSummaryService();
            ReportSaleSummary reportSaleSummary = reportSaleSummaryService.GetAlls(m => m.ID == id).FirstOrDefault();
            if (reportSaleSummary == null)
                return new ReportSaleSummaryModel();
            //
            string docNumber = reportSaleSummary.DocumentNumber;
            // 
            ReportTicketingDocumentCouponService reportTicketingDocumentCouponService = new ReportTicketingDocumentCouponService(_connection);
            ReportTicketingDocumentAmountService reportTicketingDocumentAmount = new ReportTicketingDocumentAmountService(_connection);
            ReportTicketingDocumentTaxesService reportTicketingDocumentTaxes = new ReportTicketingDocumentTaxesService(_connection);
            VNA_TKT_AsrService asrService = new VNA_TKT_AsrService();
            List<VNA_ReportSaleSummaryTicketingDocument> vna_ReportSaleSummaryTicketingDocuments = asrService.GetTicketingDocumentStatusGroup(new List<string> {
            docNumber
            });

            // reuslt
            ReportSaleSummaryModel reportSaleSummaryTicketing = new ReportSaleSummaryModel
            {
                ReportSaleSummary = reportSaleSummary,
                TicketingDocumentCoupons = reportTicketingDocumentCouponService.GetAlls(m => m.DocumentNumber == docNumber).ToList(),
                TicketingDocumentAmount = reportTicketingDocumentAmount.GetAlls(m => m.DocumentNumber == docNumber).FirstOrDefault(),
                TicketingDocumentTaxes = reportTicketingDocumentTaxes.GetAlls(m => m.DocumentNumber == docNumber).ToList(),
                TicketingDocumentStatus = vna_ReportSaleSummaryTicketingDocuments
            };
            return reportSaleSummaryTicketing;
        }
        //
        public SearchResult SearchReport(SearchModel model, string columName1 = "CreatedDate", string columName2 = null)
        {
            string whereCondition = string.Empty;
            int status = model.Status;
            int timeExpress = model.TimeExpress;
            string strTimeStart = model.StartDate;
            string strTimeEnd = model.EndDate;
            string timeZoneLocal = model.TimeZoneLocal;
            //
            string clientTime = TimeFormat.GetDateByTimeZone(timeZoneLocal);
            //
            string whereConditionSub1 = string.Empty;
            if (timeExpress != 0 && !string.IsNullOrWhiteSpace(clientTime))
            {
                // client time
                DateTime today = Convert.ToDateTime(clientTime);
                if (timeExpress == 1)
                {
                    string strDate = TimeFormat.FormatToServerDate(today);
                    string dtime = Convert.ToDateTime(strDate).ToString("yyyy-MM-dd");
                    if (!string.IsNullOrWhiteSpace(columName2))
                        whereConditionSub1 = " OR cast(" + columName2 + " as Date) = cast('" + dtime + "' as Date)";
                    //
                    whereCondition = " AND (cast(" + columName1 + " as Date) = cast('" + dtime + "' as Date) " + whereConditionSub1 + ")";
                }
                // Yesterday
                if (timeExpress == 2)
                {
                    DateTime dtime = today.AddDays(-1);
                    if (!string.IsNullOrWhiteSpace(columName2))
                        whereConditionSub1 = " OR cast(" + columName2 + " as Date) = cast('" + dtime + "' as Date)";
                    //
                    whereCondition = " AND (cast(" + columName1 + " as Date) = cast('" + dtime + "' as Date) " + whereConditionSub1 + ")";
                }
                // ThreeDayAgo
                if (timeExpress == 3)
                {
                    DateTime dtime = today.AddDays(-3);
                    if (!string.IsNullOrWhiteSpace(columName2))
                        whereConditionSub1 = " OR cast(" + columName2 + " as Date) >= cast('" + dtime + "' as Date)";
                    //
                    whereCondition = " AND (cast(" + columName1 + " as Date) >= cast('" + dtime + "' as Date) " + whereConditionSub1 + ")";
                }
                // SevenDayAgo
                if (timeExpress == 4)
                {
                    DateTime dtime = today.AddDays(-7);
                    if (!string.IsNullOrWhiteSpace(columName2))
                        whereConditionSub1 = " OR cast(" + columName2 + " as Date) >= cast('" + dtime + "' as Date)";
                    //
                    whereCondition = " AND (cast(" + columName1 + " as Date) >= cast('" + dtime + "' as Date) " + whereConditionSub1 + ")";
                }
                // OneMonthAgo
                if (timeExpress == 5)
                {
                    DateTime dtime = today.AddMonths(-1);
                    if (!string.IsNullOrWhiteSpace(columName2))
                        whereConditionSub1 = " OR cast(" + columName2 + " as Date) >= cast('" + dtime + "' as Date)";
                    //
                    whereCondition = " AND (cast(" + columName1 + " as Date) >= cast('" + dtime + "' as Date) " + whereConditionSub1 + ")";
                }

                // ThreeMonthAgo
                if (timeExpress == 6)
                {
                    DateTime dtime = today.AddMonths(-3);
                    if (!string.IsNullOrWhiteSpace(columName2))
                        whereConditionSub1 = " OR cast(" + columName2 + " as Date) >= cast('" + dtime + "' as Date)";
                    //
                    whereCondition = " AND (cast(" + columName1 + " as Date) >= cast('" + dtime + "' as Date) " + whereConditionSub1 + ")";
                }
                // SixMonthAgo
                if (timeExpress == 7)
                {
                    DateTime dtime = today.AddMonths(-6);
                    if (!string.IsNullOrWhiteSpace(columName2))
                        whereConditionSub1 = " OR cast(" + columName2 + " as Date) >= cast('" + dtime + "' as Date)";
                    //
                    whereCondition = " AND (cast(" + columName1 + " as Date) >= cast('" + dtime + "' as Date) " + whereConditionSub1 + ")";
                }
                // OneYearAgo
                if (timeExpress == 8)
                {
                    DateTime dtime = today.AddYears(-1);
                    if (!string.IsNullOrWhiteSpace(columName2))
                        whereConditionSub1 = " OR cast(" + columName2 + " as Date) >= cast('" + dtime + "' as Date)";
                    //
                    whereCondition = " AND (cast(" + columName1 + " as Date) >= cast('" + dtime + "' as Date) " + whereConditionSub1 + ")";
                }
                //
                return new SearchResult()
                {
                    Status = 1,
                    Message = whereCondition
                };
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(strTimeStart))
                {
                    DateTime dateTimeStart = Helper.TimeData.TimeFormat.FormatToServerDate(strTimeStart);
                    if (!string.IsNullOrWhiteSpace(columName2))
                        whereConditionSub1 = " OR cast(" + columName2 + " as Date) = cast('" + dateTimeStart + "' as Date)";
                    //
                    whereCondition += " AND ( cast(" + columName1 + " as Date) >= cast('" + dateTimeStart + "' as Date) " + whereConditionSub1 + ")";
                }
                //
                if (!string.IsNullOrWhiteSpace(strTimeEnd))
                {
                    DateTime dateTimeStart = Helper.TimeData.TimeFormat.FormatToServerDate(strTimeStart);
                    DateTime dateTimeEnd = Helper.TimeData.TimeFormat.FormatToServerDate(strTimeEnd);
                    if (dateTimeStart > dateTimeEnd)
                        return new SearchResult() { Status = -1, Message = "Thời gian kết thúc không hợp lệ" };
                    //
                    if (!string.IsNullOrWhiteSpace(columName2))
                        whereConditionSub1 = " OR cast(" + columName2 + " as Date) = cast('" + dateTimeEnd + "' as Date)";
                    //
                    whereCondition += " AND ( cast(" + columName1 + " as Date) <= cast('" + dateTimeEnd + "' as Date) " + whereConditionSub1 + ")";
                }
                //
                return new SearchResult()
                {
                    Status = 1,
                    Message = whereCondition
                };
            }
        }












    }
}