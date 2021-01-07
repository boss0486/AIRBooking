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
        //search date by date report ##############################################################################################################################################################################################################################################################
        public List<ReportSaleSummaryResult> ReportData(SearchModel model)
        {
            #region
            if (model == null)
                return new List<ReportSaleSummaryResult>();
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
                    return new List<ReportSaleSummaryResult>();
            }
            #endregion
            //

            if (!Helper.Current.UserLogin.IsCMSUser && !Helper.Current.UserLogin.IsAdminInApplication)
                return new List<ReportSaleSummaryResult>();
            //
            string typeId = "";
            string sqlQuery = @"
            SELECT rps.*,rpsf.FareAmount, rpsf.TaxAmount, rpsf.TotalAmount FROM App_ReportSaleSummary as rps 
            LEFT JOIN App_ReportSaleSummarySSFop as rpsf 
            ON rpsf.ReportTransactionID =  rps.ID
            WHERE (EmployeeNumber LIKE N'%'+ @Query +'%' OR DocumentNumber LIKE N'%'+ @Query +'%' OR dbo.Uni2NONE(PassengerName) LIKE N'%'+ @Query +'%')"
            + whereCondition + " ORDER BY rps.[CreatedDate]";
            List<ReportSaleSummaryResult> dtList = _connection.Query<ReportSaleSummaryResult>(sqlQuery, new { Query = Helper.Page.Library.FormatNameToUni2NONE(query), TypeID = typeId }).ToList();
            if (dtList.Count == 0)
                return new List<ReportSaleSummaryResult>();
            //
            return dtList;
        }
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
        //
        public ActionResult DataList(SearchModel model)
        {
            List<ReportSaleSummaryResult> dtList = ReportData(model);
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

            Helper.Pagination.PagingModel pagingModel = new Helper.Pagination.PagingModel
            {
                PageSize = Helper.Pagination.Paging.PAGESIZE,
                Total = dtList.Count,
                Page = page
            };
            //
            return Notifization.Data(MessageText.Success, data: result, role: RoleActionSettingService.RoleListForUser(), paging: pagingModel);
        }
        public ActionResult ExportReport(SearchModel model)
        {
            List<ReportSaleSummaryResult> dtList = ReportData(model);
            if (dtList.Count == 0)
                return null;
            //      
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                string fileName = "DANH SÁCH BÁO CÁO HẰNG NGÀY";
                string alias = Helper.Page.Library.FormatToUni2NONE(fileName);
                // set month / sheet
                List<string> sheetData = dtList.Select(m => m.ReportDate.ToString("yyyy-MM")).ToList();
                sheetData = sheetData.GroupBy(m => m).Select(m => m.Key).ToList();
                foreach (var sheet in sheetData)
                {
                    ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets.Add("T:" + sheet);
                    workSheet.TabColor = System.Drawing.Color.Black;
                    workSheet.DefaultRowHeight = 12;
                    //Header of table   
                    workSheet.Cells["A1:J1"].Merge = true;
                    workSheet.Cells["A1:J1"].Value = fileName;
                    workSheet.Cells["A1:J1"].Style.Font.Name = "Arial Narrow";
                    workSheet.Cells["A1:J1"].Style.Font.Size = 16;
                    workSheet.Row(1).Height = 35;
                    workSheet.Cells["A1:J1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Cells["A1:J1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    workSheet.Cells["A1:J1"].Style.Font.Bold = true;
                    workSheet.Cells["A1:J1"].Style.Font.Color.SetColor(Color.White);
                    workSheet.Cells["A1:J1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    workSheet.Cells["A1:J1"].Style.Fill.BackgroundColor.SetColor(Color.Teal);
                    workSheet.Cells["A1:J1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A1:J1"].Style.Border.Bottom.Color.SetColor(Color.Gray);
                    //title of table  
                    workSheet.Row(2).Height = 25;
                    workSheet.Cells[2, 1].Value = "#";
                    workSheet.Cells[2, 2].Value = "Mã nhân viên";
                    workSheet.Cells[2, 3].Value = "Doc Number";
                    workSheet.Cells[2, 4].Value = "Mã PNA";
                    workSheet.Cells[2, 5].Value = "Tên hành khách";
                    workSheet.Cells[2, 6].Value = "Ng.Báo cáo";
                    workSheet.Cells[2, 7].Value = "T.Gian khởi hành";
                    workSheet.Cells[2, 8].Value = "FareAmount";
                    workSheet.Cells[2, 9].Value = "TaxAmount";
                    workSheet.Cells[2, 10].Value = "TotalAmount";
                    //Body of table  
                    workSheet.Cells["A2:J2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    workSheet.Cells["A2:J2"].Style.Fill.BackgroundColor.SetColor(Color.Teal);
                    workSheet.Cells["A2:J2"].Style.Font.Color.SetColor(Color.White);
                    workSheet.Cells["A2:J2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    //
                    int recordIndex = 3;
                    int cnt = 1;
                    // / data to sheet
                    List<ReportSaleSummaryResult> dtListForSheet = dtList.Where(m => m.ReportDate.ToString("yyyy-MM") == sheet).ToList();
                    if (dtListForSheet.Count == 0)
                        continue;
                    //
                    foreach (var _data in dtListForSheet)
                    {
                        workSheet.Cells[recordIndex, 1].Value = cnt;
                        workSheet.Cells[recordIndex, 2].Value = _data.EmployeeNumber;
                        workSheet.Cells[recordIndex, 3].Value = _data.DocumentNumber;
                        workSheet.Cells[recordIndex, 4].Value = _data.PnrLocator;
                        workSheet.Cells[recordIndex, 5].Value = _data.PassengerName;
                        workSheet.Cells[recordIndex, 6].Value = _data.ReportDateText;
                        workSheet.Cells[recordIndex, 7].Value = _data.TransactionTime;
                        workSheet.Cells[recordIndex, 8].Value = _data.FareAmount;
                        workSheet.Cells[recordIndex, 9].Value = _data.TaxAmount;
                        workSheet.Cells[recordIndex, 10].Value = _data.TotalAmount;
                        ////// attribute
                        workSheet.Cells[recordIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        //
                        recordIndex++;
                        cnt++;
                    }
                    //
                    for (int i = 1; i <= 10; i++)
                    {
                        workSheet.Column(i).AutoFit();
                    }
                    //
                    workSheet.Cells[workSheet.Dimension.Address].AutoFitColumns();
                    workSheet.Cells[workSheet.Dimension.Address].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[workSheet.Dimension.Address].Style.Border.Top.Color.SetColor(Color.LightGray);
                    workSheet.Cells[workSheet.Dimension.Address].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[workSheet.Dimension.Address].Style.Border.Left.Color.SetColor(Color.LightGray);
                    workSheet.Cells[workSheet.Dimension.Address].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[workSheet.Dimension.Address].Style.Border.Right.Color.SetColor(Color.LightGray);
                    workSheet.Cells[workSheet.Dimension.Address].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[workSheet.Dimension.Address].Style.Border.Bottom.Color.SetColor(Color.LightGray);
                }
                // create file
                string outFolder = "/Files/Export/VnaReport/";
                string pathFile = Helper.File.AttachmentFile.AttachmentExls(alias, excelPackage, outFolder);
                if (string.IsNullOrWhiteSpace(pathFile))
                    return null;
                //
                return Notifization.DownLoadFile(MessageText.DownLoad, pathFile);
            }
        }
        //search date by departure day ##############################################################################################################################################################################################################################################################

        public ActionResult DataListByDeparture(ReportEprSearchModel model)
        {
            if (!Helper.Current.UserLogin.IsCMSUser && !Helper.Current.UserLogin.IsAdminInApplication)
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

            List<AirPassengerReult> dtList = DepartureData(model).OrderBy(m => m.StartDateTime).ToList();
            if (dtList.Count == 0)
                return null;
            //     
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                string fileName = "DANH SÁCH BÁO CÁO NGÀY BAY";
                string alias = Helper.Page.Library.FormatToUni2NONE(fileName);
                // set month / sheet
                List<string> sheetData = dtList.Select(m => m.StartDateTime.ToString("yyyy-MM")).ToList();
                sheetData = sheetData.GroupBy(m => m).Select(m => m.Key).ToList();
                foreach (var sheet in sheetData)
                {
                    ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets.Add("T:" + sheet);
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
                    //
                    int recordIndex = 3;
                    int cnt = 1;
                    // / data to sheet
                    List<AirPassengerReult> dtListForSheet = dtList.Where(m => m.StartDateTime.ToString("yyyy-MM") == sheet).ToList();
                    if (dtListForSheet.Count == 0)
                        continue;
                    //
                    foreach (var item in dtListForSheet)
                    {
                        workSheet.Cells[recordIndex, 1].Value = cnt;
                        workSheet.Cells[recordIndex, 2].Value = item.PnrLocator;
                        workSheet.Cells[recordIndex, 3].Value = item.FareBasis;
                        workSheet.Cells[recordIndex, 4].Value = item.PassengerName;
                        workSheet.Cells[recordIndex, 5].Value = item.DocumentNumber;
                        workSheet.Cells[recordIndex, 6].Value = item.StartDateTimeText;
                        workSheet.Cells[recordIndex, 7].Value = item.BookingStatus;
                        workSheet.Cells[recordIndex, 8].Value = item.CurrentStatus;
                        ////// attribute 
                        workSheet.Cells[recordIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        //
                        workSheet.Cells[recordIndex, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[recordIndex, 7].Style.Fill.BackgroundColor.SetColor(BookingStatus(item.BookingStatus));
                        workSheet.Cells[recordIndex, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        //
                        workSheet.Cells[recordIndex, 8].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[recordIndex, 8].Style.Fill.BackgroundColor.SetColor(CurrStatus(item.CurrentStatus));
                        workSheet.Cells[recordIndex, 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        //
                        recordIndex++;
                        cnt++;
                    }
                    //
                    for (int i = 1; i < 9; i++)
                    {
                        workSheet.Column(i).AutoFit();
                    }
                    //
                    workSheet.Cells[workSheet.Dimension.Address].AutoFitColumns();
                    workSheet.Cells[workSheet.Dimension.Address].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[workSheet.Dimension.Address].Style.Border.Top.Color.SetColor(Color.LightGray);
                    workSheet.Cells[workSheet.Dimension.Address].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[workSheet.Dimension.Address].Style.Border.Left.Color.SetColor(Color.LightGray);
                    workSheet.Cells[workSheet.Dimension.Address].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[workSheet.Dimension.Address].Style.Border.Right.Color.SetColor(Color.LightGray);
                    workSheet.Cells[workSheet.Dimension.Address].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[workSheet.Dimension.Address].Style.Border.Bottom.Color.SetColor(Color.LightGray);
                }
                // create file
                string outFolder = "/Files/Export/VnaReport/";
                string pathFile = Helper.File.AttachmentFile.AttachmentExls(alias, excelPackage, outFolder);
                if (string.IsNullOrWhiteSpace(pathFile))
                    return null;
                //
                return Notifization.DownLoadFile(MessageText.DownLoad, pathFile);
            }
        }
        //
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
        public SearchResult SearchReport(SearchModel model, string columName1 = "CreatedDate")
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
            //string whereConditionSub1 = string.Empty;
            if (timeExpress != 0 && !string.IsNullOrWhiteSpace(clientTime))
            {
                // client time
                DateTime today = Convert.ToDateTime(clientTime);
                if (timeExpress == 1)
                {
                    string strDate = TimeFormat.FormatToServerDate(today);
                    string dtime = Convert.ToDateTime(strDate).ToString("yyyy-MM-dd");
                    //if (!string.IsNullOrWhiteSpace(columName2))
                    //    whereConditionSub1 = " OR cast(" + columName2 + " as Date) = cast('" + dtime + "' as Date)";
                    ////
                    whereCondition = " AND cast(" + columName1 + " as Date) = cast('" + dtime + "' as Date)";
                }
                // Yesterday
                if (timeExpress == 2)
                {
                    DateTime dtime = today.AddDays(-1);
                    //if (!string.IsNullOrWhiteSpace(columName2))
                    //    whereConditionSub1 = " OR cast(" + columName2 + " as Date) = cast('" + dtime + "' as Date)";
                    ////
                    whereCondition = " AND cast(" + columName1 + " as Date) = cast('" + dtime + "' as Date)";
                }
                // ThreeDayAgo
                if (timeExpress == 3)
                {
                    DateTime dtime = today.AddDays(-3);
                    //if (!string.IsNullOrWhiteSpace(columName2))
                    //    whereConditionSub1 = " OR cast(" + columName2 + " as Date) >= cast('" + dtime + "' as Date)";
                    ////
                    whereCondition = " AND cast(" + columName1 + " as Date) >= cast('" + dtime + "' as Date)";
                    whereCondition += " AND cast(" + columName1 + " as Date) <= cast('" + today.ToString("yyyy-MM-dd") + "' as Date)";
                }
                // SevenDayAgo
                if (timeExpress == 4)
                {
                    DateTime dtime = today.AddDays(-7);
                    //if (!string.IsNullOrWhiteSpace(columName2))
                    //    whereConditionSub1 = " OR cast(" + columName2 + " as Date) >= cast('" + dtime + "' as Date)";
                    ////
                    whereCondition = " AND cast(" + columName1 + " as Date) >= cast('" + dtime + "' as Date)";
                    whereCondition += " AND cast(" + columName1 + " as Date) <= cast('" + today.ToString("yyyy-MM-dd") + "' as Date)";
                }
                // OneMonthAgo
                if (timeExpress == 5)
                {
                    DateTime dtime = today.AddMonths(-1);
                    //if (!string.IsNullOrWhiteSpace(columName2))
                    //    whereConditionSub1 = " OR cast(" + columName2 + " as Date) >= cast('" + dtime + "' as Date)";
                    ////
                    whereCondition = " AND cast(" + columName1 + " as Date) >= cast('" + dtime + "' as Date)";
                    whereCondition += " AND cast(" + columName1 + " as Date) <= cast('" + today.ToString("yyyy-MM-dd") + "' as Date)";
                }

                // ThreeMonthAgo
                if (timeExpress == 6)
                {
                    DateTime dtime = today.AddMonths(-3);
                    //if (!string.IsNullOrWhiteSpace(columName2))
                    //    whereConditionSub1 = " OR cast(" + columName2 + " as Date) >= cast('" + dtime + "' as Date)";
                    ////
                    whereCondition = " AND cast(" + columName1 + " as Date) >= cast('" + dtime + "' as Date)";
                    whereCondition += " AND cast(" + columName1 + " as Date) <= cast('" + today.ToString("yyyy-MM-dd") + "' as Date)";
                }
                // SixMonthAgo
                if (timeExpress == 7)
                {
                    DateTime dtime = today.AddMonths(-6);
                    //if (!string.IsNullOrWhiteSpace(columName2))
                    //    whereConditionSub1 = " OR cast(" + columName2 + " as Date) >= cast('" + dtime + "' as Date)";
                    ////
                    whereCondition = " AND cast(" + columName1 + " as Date) >= cast('" + dtime + "' as Date)";
                    whereCondition += " AND cast(" + columName1 + " as Date) <= cast('" + today.ToString("yyyy-MM-dd") + "' as Date)";
                }
                // OneYearAgo
                if (timeExpress == 8)
                {
                    DateTime dtime = today.AddYears(-1);
                    //if (!string.IsNullOrWhiteSpace(columName2))
                    //    whereConditionSub1 = " OR cast(" + columName2 + " as Date) >= cast('" + dtime + "' as Date)";
                    ////
                    whereCondition = " AND cast(" + columName1 + " as Date) >= cast('" + dtime + "' as Date)";
                    whereCondition += " AND cast(" + columName1 + " as Date) <= cast('" + today.ToString("yyyy-MM-dd") + "' as Date)";
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
                    //if (!string.IsNullOrWhiteSpace(columName2))
                    //    whereConditionSub1 = " OR cast(" + columName2 + " as Date) = cast('" + dateTimeStart + "' as Date)";
                    ////
                    whereCondition += " AND cast(" + columName1 + " as Date) >= cast('" + dateTimeStart + "' as Date)";
                }
                //
                if (!string.IsNullOrWhiteSpace(strTimeEnd))
                {
                    DateTime dateTimeStart = Helper.TimeData.TimeFormat.FormatToServerDate(strTimeStart);
                    DateTime dateTimeEnd = Helper.TimeData.TimeFormat.FormatToServerDate(strTimeEnd);
                    if (dateTimeStart > dateTimeEnd)
                        return new SearchResult() { Status = -1, Message = "Thời gian kết thúc không hợp lệ" };
                    //
                    //if (!string.IsNullOrWhiteSpace(columName2))
                    //    whereConditionSub1 = " OR cast(" + columName2 + " as Date) = cast('" + dateTimeEnd + "' as Date)";
                    ////
                    whereCondition += " AND cast(" + columName1 + " as Date) <= cast('" + dateTimeEnd + "' as Date)";
                }
                //
                return new SearchResult()
                {
                    Status = 1,
                    Message = whereCondition
                };
            }
        }


        public Color CurrStatus(string _status)
        {
            Color rsColor = Color.White;
            switch (_status)
            {
                case "OK":
                    rsColor = Color.GreenYellow;
                    break;
                case "CKIN":
                    rsColor = Color.Yellow;
                    break;
                case "LFTD":
                    rsColor = Color.Green;
                    break;
                case "USED":
                    rsColor = Color.DodgerBlue;
                    break;
                case "NOGO":
                    rsColor = Color.Red;
                    break;
                case "VOID":
                    rsColor = Color.Orange;
                    break;
                case "RFND":
                    rsColor = Color.Aqua;
                    break;
                case "EXCH":
                    rsColor = Color.DeepPink;
                    break;
                default:
                    rsColor = Color.White;
                    break;
            }
            return rsColor;
        }

        public Color BookingStatus(string _status)
        {
            Color rsColor = Color.White;
            switch (_status)
            {
                case "OK":
                    rsColor = Color.GreenYellow;
                    break;
                case "HK":
                    rsColor = Color.Yellow;
                    break;
                case "KL":
                    rsColor = Color.Green;
                    break;
                case "UC":
                    rsColor = Color.DodgerBlue;
                    break;
                case "GN":
                    rsColor = Color.Gray;
                    break;
                case "JL":
                    rsColor = Color.Red;
                    break;
                case "HL":
                    rsColor = Color.LightSkyBlue;
                    break;
                case "WK":
                    rsColor = Color.Orange;
                    break;
                case "SC":
                    rsColor = Color.DeepPink;
                    break;
                case "NS":
                    rsColor = Color.Aqua;
                    break;
                case "RQ":
                    rsColor = Color.Violet;
                    break;
                default:
                    rsColor = Color.White;
                    break;
            }
            return rsColor;
        }

    }
}
