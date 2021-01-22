using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using Helper;
using Helper.Email;
using Helper.Language;
using Helper.TimeData;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PagedList;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebCore.ENM;
using WebCore.Entities;
using WebCore.Model.Entities;
using WebCore.Model.Enum;
using static WebCore.ENM.BookOrderEnum;

namespace WebCore.Services
{
    public interface IBookOrderService : IEntityService<BookOrder> { }
    public class BookOrderService : EntityService<BookOrder>, IBookOrderService
    {
        public BookOrderService() : base() { }
        public BookOrderService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public List<BookOrderResult> OrderData(BookOrderSearch model)
        {
            List<BookOrderResult> bookOrderResult = new List<BookOrderResult>();
            if (model == null)
                return bookOrderResult;
            //
            int page = model.Page;
            string query = model.Query;
            if (string.IsNullOrWhiteSpace(query))
                query = "";
            //
            string whereCondition = string.Empty;
            SearchResult searchResult = WebCore.Model.Services.ModelService.SearchDefault(new SearchModel
            {
                Query = model.Query,
                TimeExpress = model.TimeExpress,
                Status = (int)WebCore.Model.Enum.ModelEnum.Enabled.ENABLED,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Page = model.Page,
                AreaID = model.AreaID,
                TimeZoneLocal = model.TimeZoneLocal
            }, dateColumn: "IssueDate");
            if (searchResult != null)
            {
                if (searchResult.Status == 1)
                    whereCondition = searchResult.Message;
                else
                    return bookOrderResult;
            }
            // 
            if (model.ItineraryType != (int)WebCore.ENM.BookOrderEnum.BookItineraryType.None)
                whereCondition += " AND o.ItineraryType = @ItineraryType";
            // 
            string compId = model.CompanyID;
            int customerType = model.CustomerType;
            //
            if (customerType != (int)CustomerEnum.CustomerType.NONE)
            {
                whereCondition += " AND c.CustomerType = " + customerType + " ";
                //
                if (customerType == (int)CustomerEnum.CustomerType.COMP)
                {
                    if (!string.IsNullOrWhiteSpace(compId))
                        whereCondition += " AND c.CompanyID = @CompanyID";
                }
            }

            string agentId = model.AgentID;
            // limit data
            //if (Helper.Current.UserLogin.IsClientInApplication())
            //{
            //    string userId = Helper.Current.UserLogin.IdentifierID;
            //    agentId = ClientLoginService.GetClientIDByUserID(userId);


            //} 
            //if (!string.IsNullOrWhiteSpace(agentId))
            //{
            //    whereCondition += " AND o.AgentID = @AgentID";
            //}
            // query 
            string sqlQuery = @"SELECT bp.ID, bp.BookOrderID, bp.TicketNumber, bp.FullName, bp.Gender, 
            c.CustomerType, c.Name as 'ContactName', c.CompanyID, c.CompanyCode,
            a.AgentCode,a.TicketingID, a.TicketingName, a.AgentFee , a.ProviderFee, a.AgentPrice,
			o.ProviderCode, o.AirlineID, o.ItineraryType, o.PNR, o.Amount, o.IssueDate, o.ExportDate, o.Unit,
            (SELECT SUM (Amount) FROM App_BookPrice WHERE BookOrderID = bp.BookOrderID AND PassengerType = bp.PassengerType) as FareBasic,
            (SELECT SUM (Amount) FROM App_BookTax WHERE BookOrderID = bp.BookOrderID AND PassengerType = bp.PassengerType AND TaxCode !='UE3') as FareTax,
            (SELECT Amount FROM App_BookTax WHERE BookOrderID = bp.BookOrderID AND PassengerType = bp.PassengerType AND TaxCode ='UE3') as VAT
            FROM App_BookPassenger as bp
            LEFT JOIN App_BookAgent as a ON a.BookOrderID = bp.BookOrderID
            LEFT JOIN App_BookCustomer as c ON c.BookOrderID = bp.BookOrderID
			LEFT JOIN App_BookOrder as o ON o.ID = bp.BookOrderID
            WHERE (bp.TicketNumber LIKE N'%'+ @Query +'%' OR o.PNR LIKE N'%'+ @Query +'%' OR bp.FullName LIKE N'%'+ @Query +'%') " + whereCondition + "  AND o.OrderStatus = @OrderStatus ORDER BY o.IssueDate, bp.TicketNumber, bp.FullName  ASC";
            bookOrderResult = _connection.Query<BookOrderResult>(sqlQuery, new { Query = Helper.Page.Library.FormatNameToUni2NONE(query), OrderStatus = (int)WebCore.ENM.BookOrderEnum.BookOrderStatus.Exported, ItineraryType = model.ItineraryType, AgentID = agentId, CompanyID = compId }).ToList();
            return bookOrderResult;
        }
        public List<BookingResult> BookingData(BookOrderSearch model)
        {
            List<BookingResult> bookOrderResult = new List<BookingResult>();
            if (model == null)
                return bookOrderResult;
            //
            int page = model.Page;
            string query = model.Query;
            if (string.IsNullOrWhiteSpace(query))
                query = "";
            //
            string whereCondition = string.Empty;
            SearchResult searchResult = WebCore.Model.Services.ModelService.SearchDefault(new SearchModel
            {
                Query = model.Query,
                TimeExpress = model.TimeExpress,
                Status = (int)WebCore.Model.Enum.ModelEnum.Enabled.ENABLED,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Page = model.Page,
                AreaID = model.AreaID,
                TimeZoneLocal = model.TimeZoneLocal
            }, dateColumn: "IssueDate");
            if (searchResult != null)
            {
                if (searchResult.Status == 1)
                    whereCondition = searchResult.Message;
                else
                    return bookOrderResult;
            }
            // 
            whereCondition += " AND o.OrderStatus = @OrderStatus";
            //
            if (model.ItineraryType != (int)WebCore.ENM.BookOrderEnum.BookItineraryType.None)
                whereCondition += " AND o.ItineraryType = @ItineraryType";
            // 
            string compId = model.CompanyID;
            int customerType = model.CustomerType;
            //
            if (customerType != (int)CustomerEnum.CustomerType.NONE)
            {
                whereCondition += " AND c.CustomerType = " + customerType + " ";
                //
                if (customerType == (int)CustomerEnum.CustomerType.COMP)
                {
                    if (!string.IsNullOrWhiteSpace(compId))
                        whereCondition += " AND c.CompanyID = @CompanyID";
                }
            }

            string agentId = model.AgentID;



            // limit data
            //if (Helper.Current.UserLogin.IsClientInApplication())
            //{
            //    string userId = Helper.Current.UserLogin.IdentifierID;
            //    agentId = ClientLoginService.GetClientIDByUserID(userId);


            //} 
            //if (!string.IsNullOrWhiteSpace(agentId))
            //{
            //    whereCondition += " AND o.AgentID = @AgentID";
            //}


            // query
            string sqlQuery = @"SELECT o.*,
            c.CustomerType, c.Name as 'ContactName', c.CompanyID, c.CompanyCode,
            a.AgentCode,a.TicketingID, a.TicketingName, a.AgentFee , a.ProviderFee, a.AgentPrice,
            (SELECT SUM (Amount) FROM App_BookPrice WHERE BookOrderID = o.ID) as FareBasic,
            (SELECT SUM (Amount) FROM App_BookTax WHERE BookOrderID = o.ID) as FareTax
            FROM App_BookOrder as o 
            LEFT JOIN App_BookCustomer as c ON c.BookOrderID = o.ID
            LEFT JOIN App_BookAgent as a ON a.BookOrderID = o.ID
            WHERE (o.PNR LIKE N'%'+ @Query +'%') " + whereCondition + " ORDER BY o.IssueDate ASC";
            bookOrderResult = _connection.Query<BookingResult>(sqlQuery, new { Query = Helper.Page.Library.FormatNameToUni2NONE(query), OrderStatus = (int)BookOrderStatus.Booking, ItineraryType = model.ItineraryType, AgentID = agentId, CompanyID = compId }).ToList();
            // reusult
            return bookOrderResult;
        }

        public ActionResult OrederList(BookOrderSearch model)
        {
            List<BookOrderResult> dtList = OrderData(model);
            int page = model.Page;
            if (dtList.Count == 0)
                return Notifization.NotFound(MessageText.NotFound);
            //
            var result = dtList.ToPagedList(page, Helper.Pagination.Paging.PAGESIZE).ToList();
            if (result.Count == 0 && page > 1)
            {
                page -= 1;
                result = dtList.ToPagedList(page, Helper.Pagination.Paging.PAGESIZE).ToList();
            }
            if (result.Count == 0)
                return Notifization.NotFound(MessageText.NotFound);
            // Pagination
            Helper.Pagination.PagingModel pagingModel = new Helper.Pagination.PagingModel
            {
                PageSize = Helper.Pagination.Paging.PAGESIZE,
                Total = dtList.Count,
                Page = page
            };
            // reusult
            return Notifization.Data(MessageText.Success, data: result, role: RoleActionSettingService.RoleListForUser(), paging: pagingModel);
        }
        public ActionResult BookingList(BookOrderSearch model)
        {
            List<BookingResult> dtList = BookingData(model);
            int page = model.Page;
            if (dtList.Count == 0)
                return Notifization.NotFound(MessageText.NotFound);
            //
            var result = dtList.ToPagedList(page, Helper.Pagination.Paging.PAGESIZE).ToList();
            if (result.Count == 0 && page > 1)
            {
                page -= 1;
                result = dtList.ToPagedList(page, Helper.Pagination.Paging.PAGESIZE).ToList();
            }
            if (result.Count == 0)
                return Notifization.NotFound(MessageText.NotFound);
            // Pagination
            Helper.Pagination.PagingModel pagingModel = new Helper.Pagination.PagingModel
            {
                PageSize = Helper.Pagination.Paging.PAGESIZE,
                Total = dtList.Count,
                Page = page
            };
            // reusult
            return Notifization.Data(MessageText.Success, data: result, role: RoleActionSettingService.RoleListForUser(), paging: pagingModel);
        }
        //


        public ActionResult BookingExport(BookOrderSearch model)
        {

            List<BookingResult> dtList = BookingData(model);
            if (dtList.Count == 0)
                return Notifization.NotFound();
            //     
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                DateTime dateMin = dtList.Min(m => m.IssueDate);
                DateTime dateMax = dtList.Max(m => m.IssueDate);

                string fileName = "DỮ LIỆU ĐẶT CHỖ";
                string alias = Helper.Page.Library.FormatToUni2NONE(fileName);
                // set month / sheet
                List<string> sheetData = dtList.Select(m => m.IssueDate.ToString("yyyy-MM")).ToList();
                sheetData = sheetData.GroupBy(m => m).Select(m => m.Key).ToList();
                foreach (var sheet in sheetData)
                {
                    ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets.Add("T:" + sheet);
                    workSheet.TabColor = System.Drawing.Color.Black;
                    workSheet.DefaultRowHeight = 12;
                    //Header of table   
                    workSheet.Cells["A1:N1"].Merge = true;
                    workSheet.Cells["A1:N1"].Value = fileName + " " + TimeFormat.FormatToViewDate(dateMin, LanguagePage.GetLanguageCode, (int)ModelEnum.DateExtension.SLASH) + " - " + TimeFormat.FormatToViewDate(dateMax, LanguagePage.GetLanguageCode, (int)ModelEnum.DateExtension.SLASH);
                    workSheet.Cells["A1:N1"].Style.Font.Name = "Arial Unicode MS";
                    workSheet.Cells["A1:N1"].Style.Font.Size = 16;
                    workSheet.Row(1).Height = 35;
                    workSheet.Cells["A1:N1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Cells["A1:N1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    workSheet.Cells["A1:N1"].Style.Font.Bold = true;
                    workSheet.Cells["A1:N1"].Style.Font.Color.SetColor(Color.White);
                    workSheet.Cells["A1:N1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    workSheet.Cells["A1:N1"].Style.Fill.BackgroundColor.SetColor(Color.Teal);
                    workSheet.Cells["A1:N1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A1:N1"].Style.Border.Bottom.Color.SetColor(Color.Gray);
                    //title of table  
                    workSheet.Row(2).Height = 20;
                    workSheet.Cells[2, 1].Value = "#";
                    workSheet.Cells[2, 2].Value = "Ngày đặt";
                    workSheet.Cells[2, 3].Value = "Loại khách hàng";
                    workSheet.Cells[2, 4].Value = "Mã DL";
                    workSheet.Cells[2, 5].Value = "Nhân viên đặt chỗ";
                    workSheet.Cells[2, 6].Value = "Loại chuyến bay";
                    workSheet.Cells[2, 7].Value = "Hãng hàng không";
                    workSheet.Cells[2, 8].Value = "Mã PNR";
                    workSheet.Cells[2, 9].Value = "Gửi mail";
                    workSheet.Cells[2, 10].Value = "Giá hãng";
                    workSheet.Cells[2, 11].Value = "Giá DL (chênh)";
                    workSheet.Cells[2, 12].Value = "Phí xuất vé";
                    workSheet.Cells[2, 13].Value = "Phí đại lý";
                    workSheet.Cells[2, 14].Value = "Tổng chi phí";


                    //Body of table  
                    workSheet.Cells["A2:N2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    workSheet.Cells["A2:N2"].Style.Fill.BackgroundColor.SetColor(Color.Teal);
                    workSheet.Cells["A2:N2"].Style.Font.Color.SetColor(Color.White);
                    workSheet.Cells["A2:N2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    //
                    int recordIndex = 3;
                    int cnt = 1;
                    // / data to sheet
                    List<BookingResult> dtListForSheet = dtList.Where(m => m.IssueDate.ToString("yyyy-MM") == sheet).ToList();
                    if (dtListForSheet.Count == 0)
                        continue;
                    //
                    foreach (var item in dtListForSheet)
                    {
                        workSheet.Cells[recordIndex, 1].Value = cnt;
                        workSheet.Cells[recordIndex, 2].Value = item.IssueDateText;
                        workSheet.Cells[recordIndex, 3].Value = item.CustomerTypeText;
                        workSheet.Cells[recordIndex, 4].Value = item.AgentCode;
                        workSheet.Cells[recordIndex, 5].Value = item.TicketingName;
                        workSheet.Cells[recordIndex, 6].Value = item.ItineraryText;
                        workSheet.Cells[recordIndex, 7].Value = item.AirlineID;
                        workSheet.Cells[recordIndex, 8].Value = item.PNR;
                        workSheet.Cells[recordIndex, 9].Value = item.MailStatus;
                        workSheet.Cells[recordIndex, 10].Value = item.Amount;
                        workSheet.Cells[recordIndex, 11].Value = item.AgentPrice;
                        workSheet.Cells[recordIndex, 12].Value = item.ProviderFee;
                        workSheet.Cells[recordIndex, 13].Value = item.AgentFee;
                        workSheet.Cells[recordIndex, 14].Value = item.TotalAmount;
                        ////// attribute 
                        workSheet.Cells[recordIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        workSheet.Cells[recordIndex, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        workSheet.Cells[recordIndex, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        workSheet.Cells[recordIndex, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        workSheet.Cells[recordIndex, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        //
                        workSheet.Cells[recordIndex, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[recordIndex, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        //
                        workSheet.Cells[recordIndex, 8].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        //
                        workSheet.Cells[recordIndex, 10].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[recordIndex, 11].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[recordIndex, 12].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[recordIndex, 13].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[recordIndex, 14].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        //
                        workSheet.Cells[recordIndex, 6].Style.Fill.BackgroundColor.SetColor(Color.PaleGreen);
                        workSheet.Cells[recordIndex, 7].Style.Fill.BackgroundColor.SetColor(Color.PaleGreen);
                        //
                        workSheet.Cells[recordIndex, 8].Style.Fill.BackgroundColor.SetColor(Color.LightPink);
                        //
                        workSheet.Cells[recordIndex, 10].Style.Fill.BackgroundColor.SetColor(Color.LightYellow);
                        workSheet.Cells[recordIndex, 11].Style.Fill.BackgroundColor.SetColor(Color.LightYellow);
                        workSheet.Cells[recordIndex, 12].Style.Fill.BackgroundColor.SetColor(Color.LightYellow);
                        workSheet.Cells[recordIndex, 13].Style.Fill.BackgroundColor.SetColor(Color.LightYellow);
                        workSheet.Cells[recordIndex, 14].Style.Fill.BackgroundColor.SetColor(Color.LightYellow);
                        ////
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
                string outFolder = "/Files/Export/AirBooking/";
                string pathFile = Helper.File.AttachmentFile.AttachmentExls(alias, excelPackage, outFolder);
                if (string.IsNullOrWhiteSpace(pathFile))
                    return null;
                //
                return Notifization.DownLoadFile(MessageText.DownLoad, pathFile);
            }
        }
        public ActionResult OrderExport(BookOrderSearch model)
        {

            List<BookOrderResult> dtList = OrderData(model);
            if (dtList.Count == 0)
                return Notifization.NotFound();
            //     
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                DateTime dateMin = dtList.Min(m => m.IssueDate);
                DateTime dateMax = dtList.Max(m => m.IssueDate);

                string fileName = "DỮ LIỆU XUẤT VÉ";
                string alias = Helper.Page.Library.FormatToUni2NONE(fileName);
                // set month / sheet
                List<string> sheetData = dtList.Select(m => m.IssueDate.ToString("yyyy-MM")).ToList();
                sheetData = sheetData.GroupBy(m => m).Select(m => m.Key).ToList();
                BookTicketService bookTicketService = new BookTicketService(_connection);


                foreach (var sheet in sheetData)
                {
                    ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets.Add("T:" + sheet);
                    workSheet.TabColor = System.Drawing.Color.Black;
                    workSheet.DefaultRowHeight = 12;
                    //Header of table   
                    workSheet.Cells["A1:V1"].Merge = true;
                    workSheet.Cells["A1:V1"].Value = fileName + " " + TimeFormat.FormatToViewDate(dateMin, LanguagePage.GetLanguageCode, (int)ModelEnum.DateExtension.SLASH) + " - " + TimeFormat.FormatToViewDate(dateMax, LanguagePage.GetLanguageCode, (int)ModelEnum.DateExtension.SLASH);
                    workSheet.Cells["A1:V1"].Style.Font.Name = "Arial Unicode MS";
                    workSheet.Cells["A1:V1"].Style.Font.Size = 16;
                    workSheet.Row(1).Height = 35;
                    workSheet.Cells["A1:V1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Cells["A1:V1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    workSheet.Cells["A1:V1"].Style.Font.Bold = true;
                    workSheet.Cells["A1:V1"].Style.Font.Color.SetColor(Color.White);
                    workSheet.Cells["A1:V1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    workSheet.Cells["A1:V1"].Style.Fill.BackgroundColor.SetColor(Color.Teal);
                    workSheet.Cells["A1:V1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells["A1:V1"].Style.Border.Bottom.Color.SetColor(Color.Gray);
                    //title of table  
                    workSheet.Row(2).Height = 20;
                    workSheet.Cells[2, 01].Value = "#";
                    workSheet.Cells[2, 02].Value = "Ngày xuất vé";
                    workSheet.Cells[2, 03].Value = "Số vé";
                    workSheet.Cells[2, 04].Value = "Tên hành khách";
                    workSheet.Cells[2, 05].Value = "Nhà cung cấp";
                    workSheet.Cells[2, 06].Value = "Mã đại lý";
                    workSheet.Cells[2, 07].Value = "Loại khách hàng";
                    workSheet.Cells[2, 08].Value = "NV. Phụ trách";
                    //
                    workSheet.Cells[2, 09].Value = "L.hành trình";
                    workSheet.Cells[2, 10].Value = "Hãng HK";
                    workSheet.Cells[2, 11].Value = "Hành trình";
                    workSheet.Cells[2, 12].Value = "Ngày khởi hành";
                    workSheet.Cells[2, 13].Value = "Loại vé";
                    workSheet.Cells[2, 14].Value = "Hạng vé";
                    //
                    workSheet.Cells[2, 15].Value = "Giá vé(1)";
                    workSheet.Cells[2, 16].Value = "Thuế(2)";
                    workSheet.Cells[2, 17].Value = "VAT(3)";
                    workSheet.Cells[2, 18].Value = "Giá DL(4)";
                    workSheet.Cells[2, 19].Value = "Phí xuất vé(5)";
                    workSheet.Cells[2, 20].Value = "Phí đại lý(6)";
                    workSheet.Cells[2, 21].Value = "Tổng chi phí";
                    workSheet.Cells[2, 22].Value = "Đơn vị";
                    //Body of table  
                    workSheet.Cells["A2:V2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    workSheet.Cells["A2:V2"].Style.Fill.BackgroundColor.SetColor(Color.Teal);
                    workSheet.Cells["A2:V2"].Style.Font.Color.SetColor(Color.White);
                    workSheet.Cells["A2:V2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    //
                    int recordIndex = 3;
                    int cnt = 1;
                    // / data to sheet
                    List<BookOrderResult> dtListForSheet = dtList.Where(m => m.IssueDate.ToString("yyyy-MM") == sheet).ToList();
                    if (dtListForSheet.Count == 0)
                        continue;
                    //
                    foreach (var item in dtListForSheet)
                    {
                        string itinerary = string.Empty;
                        string departureDate = string.Empty;
                        string ticketType = string.Empty;
                        string resBookDesigCode = string.Empty;
                        string direction = string.Empty;
                        BookTicket bookTicket = bookTicketService.GetAlls(m => m.BookOrderID == item.BookOrderID).FirstOrDefault();
                        if (bookTicket != null)
                        {
                            itinerary = "1:" + bookTicket.OriginLocation + "-" + bookTicket.DestinationLocation;
                            departureDate = Helper.TimeData.TimeFormat.FormatToViewDate(bookTicket.DepartureDateTime, LanguagePage.GetLanguageCode);
                            ticketType = BookTicketService.TicketTypeData().Where(m => m.Value == bookTicket.TicketType).FirstOrDefault().Text;
                            resBookDesigCode = bookTicket.ResBookDesigCode;
                        }
                        workSheet.Cells[recordIndex, 01].Value = cnt;
                        workSheet.Cells[recordIndex, 02].Value = item.ExportDateText;
                        workSheet.Cells[recordIndex, 03].Value = item.TicketNumber;
                        workSheet.Cells[recordIndex, 04].Value = item.FullName.ToUpper();
                        workSheet.Cells[recordIndex, 05].Value = item.ProviderCode;
                        workSheet.Cells[recordIndex, 06].Value = item.AgentCode;
                        workSheet.Cells[recordIndex, 07].Value = item.CustomerTypeText;
                        workSheet.Cells[recordIndex, 08].Value = item.TicketingName;
                        //
                        workSheet.Cells[recordIndex, 09].Value = item.ItineraryText;
                        workSheet.Cells[recordIndex, 10].Value = item.AirlineID;
                        workSheet.Cells[recordIndex, 11].Value = itinerary;
                        workSheet.Cells[recordIndex, 12].Value = departureDate;
                        workSheet.Cells[recordIndex, 13].Value = ticketType;
                        workSheet.Cells[recordIndex, 14].Value = resBookDesigCode;
                        //
                        workSheet.Cells[recordIndex, 15].Value = item.FareBasic;
                        workSheet.Cells[recordIndex, 16].Value = item.FareTax;
                        workSheet.Cells[recordIndex, 17].Value = item.VAT;
                        workSheet.Cells[recordIndex, 18].Value = item.AgentPrice;
                        workSheet.Cells[recordIndex, 19].Value = item.ProviderFee;
                        workSheet.Cells[recordIndex, 20].Value = item.AgentFee;
                        workSheet.Cells[recordIndex, 21].Value = item.TotalAmount;
                        workSheet.Cells[recordIndex, 22].Value = item.Unit;
                        ////// attribute 
                        workSheet.Cells[recordIndex, 01].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        //
                        workSheet.Cells[recordIndex, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        //
                        workSheet.Cells[recordIndex, 15].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        workSheet.Cells[recordIndex, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        workSheet.Cells[recordIndex, 17].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        workSheet.Cells[recordIndex, 18].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        workSheet.Cells[recordIndex, 19].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        workSheet.Cells[recordIndex, 20].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        workSheet.Cells[recordIndex, 20].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        //
                        workSheet.Cells[recordIndex, 22].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        //
                        workSheet.Cells[recordIndex, 09].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[recordIndex, 10].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[recordIndex, 11].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[recordIndex, 12].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[recordIndex, 13].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        //
                        workSheet.Cells[recordIndex, 14].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        //
                        workSheet.Cells[recordIndex, 15].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[recordIndex, 16].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[recordIndex, 17].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[recordIndex, 18].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[recordIndex, 19].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[recordIndex, 20].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[recordIndex, 21].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[recordIndex, 22].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        //
                        workSheet.Cells[recordIndex, 09].Style.Fill.BackgroundColor.SetColor(Color.PaleGreen);
                        workSheet.Cells[recordIndex, 10].Style.Fill.BackgroundColor.SetColor(Color.PaleGreen);
                        workSheet.Cells[recordIndex, 11].Style.Fill.BackgroundColor.SetColor(Color.PaleGreen);
                        workSheet.Cells[recordIndex, 12].Style.Fill.BackgroundColor.SetColor(Color.PaleGreen);
                        workSheet.Cells[recordIndex, 13].Style.Fill.BackgroundColor.SetColor(Color.PaleGreen);
                        workSheet.Cells[recordIndex, 14].Style.Fill.BackgroundColor.SetColor(Color.PaleGreen);
                        //
                        workSheet.Cells[recordIndex, 15].Style.Fill.BackgroundColor.SetColor(Color.LightYellow);
                        workSheet.Cells[recordIndex, 16].Style.Fill.BackgroundColor.SetColor(Color.LightYellow);
                        workSheet.Cells[recordIndex, 17].Style.Fill.BackgroundColor.SetColor(Color.LightYellow);
                        workSheet.Cells[recordIndex, 18].Style.Fill.BackgroundColor.SetColor(Color.LightYellow);
                        workSheet.Cells[recordIndex, 19].Style.Fill.BackgroundColor.SetColor(Color.LightYellow);
                        workSheet.Cells[recordIndex, 20].Style.Fill.BackgroundColor.SetColor(Color.LightYellow);
                        workSheet.Cells[recordIndex, 21].Style.Fill.BackgroundColor.SetColor(Color.LightYellow);
                        //
                        workSheet.Cells[recordIndex, 22].Style.Fill.BackgroundColor.SetColor(Color.Aquamarine);
                        ////
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
                string outFolder = "/Files/Export/AirBooking/";
                string pathFile = Helper.File.AttachmentFile.AttachmentExls(alias, excelPackage, outFolder);
                if (string.IsNullOrWhiteSpace(pathFile))
                    return null;
                //
                return Notifization.DownLoadFile(MessageText.DownLoad, pathFile);
            }
        }
        //

        public ViewBookOrder ViewBookOrderByID(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;
            //
            string sqlQuery = @"SELECT TOP (1) * FROM App_BookOrder WHERE ID = @ID";
            ViewBookOrder data = _connection.Query<ViewBookOrder>(sqlQuery, new { ID = id }).FirstOrDefault();
            if (data == null)
                return null;
            //
            BookAgentService bookAgentService = new BookAgentService(_connection);
            BookAgent bookAgent = bookAgentService.GetAlls(m => m.BookOrderID == data.ID).FirstOrDefault();
            data.BookAgent = bookAgent;
            //
            BookTicketService bookTicketService = new BookTicketService(_connection);
            List<BookTicket> bookTickets = bookTicketService.GetAlls(m => m.BookOrderID == data.ID).ToList();
            data.BookTickets = bookTickets;
            // 
            BookPassengerService bookPassengerService = new BookPassengerService(_connection);
            data.BookPassengers = bookPassengerService.GetAlls(m => m.BookOrderID == data.ID).ToList();
            //
            BookPriceService bookPriceService = new BookPriceService(_connection);
            data.BookPrices = bookPriceService.GetAlls(m => m.BookOrderID == data.ID).ToList();
            //
            BookTaxService bookTaxService = new BookTaxService(_connection);
            data.BookTaxs = bookTaxService.GetAlls(m => m.BookOrderID == data.ID).ToList();
            //
            BookCustomerService bookContactService = new BookCustomerService(_connection);
            data.BookCustomers = bookContactService.GetAlls(m => m.BookOrderID == data.ID).ToList();
            //
            return data;
        }
        public ViewBookOrder ViewTicketByID(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;
            //
            BookPassengerService bookPassengerService = new BookPassengerService(_connection);
            BookPassenger bookPassenger = bookPassengerService.GetAlls(m => m.ID == id).FirstOrDefault();
            if (bookPassenger == null)
                return null;
            //
            string orderId = bookPassenger.BookOrderID;
            // 
            string sqlQuery = @"SELECT TOP (1) * FROM App_BookOrder WHERE ID = @ID";
            ViewBookOrder data = _connection.Query<ViewBookOrder>(sqlQuery, new { ID = orderId }).FirstOrDefault();
            if (data == null)
                return null;
            BookAgentService bookAgentService = new BookAgentService(_connection);
            BookAgent bookAgent = bookAgentService.GetAlls(m => m.BookOrderID == orderId).FirstOrDefault();
            data.BookAgent = bookAgent;
            //
            BookTicketService bookTicketService = new BookTicketService(_connection);
            List<BookTicket> bookTickets = bookTicketService.GetAlls(m => m.BookOrderID == orderId).ToList();
            data.BookTickets = bookTickets;
            // 
            data.BookPassengers = bookPassengerService.GetAlls(m => m.BookOrderID == orderId).ToList();
            //
            BookPriceService bookPriceService = new BookPriceService(_connection);
            data.BookPrices = bookPriceService.GetAlls(m => m.BookOrderID == orderId).ToList();
            //
            BookTaxService bookTaxService = new BookTaxService(_connection);
            data.BookTaxs = bookTaxService.GetAlls(m => m.BookOrderID == orderId).ToList();
            //
            BookCustomerService bookContactService = new BookCustomerService(_connection);
            data.BookCustomers = bookContactService.GetAlls(m => m.BookOrderID == orderId).ToList();
            //

            return data;

        }

        public static string ViewOrderContactType(int state)
        {
            string result = string.Empty;
            switch (state)
            {
                case 1:
                    result = "Công ty";
                    break;
                case 2:
                    result = "Khách lẻ";
                    break;
                default:
                    break;
            }
            return result;
        }
        public static string ViewOrderItineraryTypeText(int state)
        {
            string result = string.Empty;
            switch (state)
            {
                case 1:
                    result = "Nội địa";
                    break;
                case 2:
                    result = "Quốc tế";
                    break;
                default:
                    break;
            }
            return result;
        }
        public List<BookPassengerResult> ViewBookPassenger(string bookId)
        {
            if (string.IsNullOrWhiteSpace(bookId))
                return null;
            //
            bookId = bookId.Trim().ToLower();
            string sqlQuery = @"SELECT * FROM App_BookPassenger WHERE BookOrderID = @ID ORDER BY PassengerType";
            List<BookPassengerResult> data = _connection.Query<BookPassengerResult>(sqlQuery, new { ID = bookId }).ToList();
            if (data.Count == 0)
                return new List<BookPassengerResult>();
            //
            return data;
        }
        public List<RequestBookFareBasic> ViewBookFareBasic(string bookId)
        {
            if (string.IsNullOrWhiteSpace(bookId))
                return null;
            //
            bookId = bookId.Trim().ToLower();
            string sqlQuery = @"SELECT * FROM App_BookPassenger WHERE BookOrderID = @BookOrderID";
            List<BookPassenger> dataPassengers = _connection.Query<BookPassenger>(sqlQuery, new { BookOrderID = bookId }).ToList();
            if (dataPassengers.Count == 0)
                return null;
            //
            var dataGroup = dataPassengers.GroupBy(m => new { m.PassengerType }).Select(m => new { m.Key.PassengerType }).ToList();
            BookPriceService bookPriceService = new BookPriceService(_connection);
            BookTaxService bookTaxService = new BookTaxService(_connection);
            List<RequestBookFareBasic> requestBookFareBasics = new List<RequestBookFareBasic>();
            foreach (var item in dataGroup)
            {
                //
                double amount = 0;
                double tax = 0;
                List<BookPrice> bookPrices = bookPriceService.GetAlls(m => m.PassengerType == item.PassengerType && m.BookOrderID == bookId).ToList();
                if (bookPrices.Count > 0)
                    amount = bookPrices.Sum(m => m.Amount);
                // 
                List<BookTax> bookTaxes = bookTaxService.GetAlls(m => m.PassengerType == item.PassengerType && m.BookOrderID == bookId).ToList();
                if (bookTaxes.Count > 0)
                    tax = bookTaxes.Sum(m => m.Amount);
                // 
                RequestBookFareBasic requestBookFareBasic = new RequestBookFareBasic
                {
                    PassengerType = item.PassengerType,
                    Quantity = dataPassengers.Where(m => m.PassengerType == item.PassengerType).Count(),
                    Amount = amount,
                    TaxAmount = tax
                };
                requestBookFareBasics.Add(requestBookFareBasic);
            }
            return requestBookFareBasics;
        }
        //  change order ******************************************************************************************************
        public ActionResult BookEditPrice(BookEditPriceModel model)
        {
            if (model == null)
                return Notifization.Invalid(MessageText.Invalid);
            //
            string bookOrderId = model.ID;
            string ticktingId = model.TicktingID;
            double amount = model.Amount;
            if (string.IsNullOrWhiteSpace(bookOrderId) || string.IsNullOrWhiteSpace(ticktingId))
                return Notifization.Invalid(MessageText.Invalid);
            //
            BookOrderService bookOrderService = new BookOrderService(_connection);
            BookAgentService bookAgentService = new BookAgentService(_connection);
            BookOrder bookOrder = bookOrderService.GetAlls(m => m.ID == bookOrderId).FirstOrDefault();
            if (bookOrder == null)
                return Notifization.Invalid(MessageText.Invalid);
            //
            BookAgent bookAgent = bookAgentService.GetAlls(m => m.BookOrderID == bookOrderId && m.TicketingID == ticktingId).FirstOrDefault();
            if (bookAgent == null)
                return Notifization.Invalid(MessageText.Invalid);
            //
            double oldVal = bookAgent.AgentPrice;
            if (oldVal == amount)
                return Notifization.Success(MessageText.UpdateSuccess);
            //
            bookAgent.AgentPrice = amount;
            bookAgentService.Update(bookAgent);
            return Notifization.Success(MessageText.UpdateSuccess);
        }
        public ActionResult BookEditAgentFee(BookEditFeeModel model)
        {
            if (model == null)
                return Notifization.Invalid(MessageText.Invalid);
            //
            string bookOrderId = model.ID;
            string ticktingId = model.TicktingID;
            double amount = model.Amount;
            if (string.IsNullOrWhiteSpace(bookOrderId) || string.IsNullOrWhiteSpace(ticktingId))
                return Notifization.Invalid(MessageText.Invalid);
            //
            BookOrderService bookOrderService = new BookOrderService(_connection);
            BookAgentService bookAgentService = new BookAgentService(_connection);
            BookOrder bookOrder = bookOrderService.GetAlls(m => m.ID == bookOrderId).FirstOrDefault();
            if (bookOrder == null)
                return Notifization.Invalid(MessageText.Invalid);
            //
            BookAgent bookAgent = bookAgentService.GetAlls(m => m.BookOrderID == bookOrderId && m.TicketingID == ticktingId).FirstOrDefault();
            if (bookAgent == null)
                return Notifization.Invalid(MessageText.Invalid);
            //
            double oldVal = bookAgent.AgentFee;
            if (oldVal == amount)
                return Notifization.Success(MessageText.UpdateSuccess);
            //
            bookAgent.AgentFee = amount;
            bookAgentService.Update(bookAgent);
            return Notifization.Success(MessageText.UpdateSuccess);
        }
        public ActionResult BookEmail(string orderId)
        {
            if (string.IsNullOrWhiteSpace(orderId))
                return null;
            //
            orderId = orderId.Trim().ToLower();
            BookOrderService bookOrderService = new BookOrderService(_connection);
            BookOrder bookOrder = bookOrderService.GetAlls(m => m.ID == orderId).FirstOrDefault();
            if (bookOrder == null)
                return Notifization.Invalid(MessageText.Invalid);
            //    
            BookCustomerService bookCustomerService = new BookCustomerService(_connection);
            BookCustomer bookCustomer = bookCustomerService.GetAlls(m => m.BookOrderID == orderId).FirstOrDefault();
            if (bookCustomer == null)
                return Notifization.Invalid(MessageText.Invalid);
            //  
            BookAgentService bookAgentService = new BookAgentService(_connection);
            BookAgent bookAgent = bookAgentService.GetAlls(m => m.BookOrderID == orderId).FirstOrDefault();
            if (bookAgent == null)
                return Notifization.Invalid(MessageText.Invalid);
            //
            string emailTo = bookCustomer.Email;
            int mailStaus = MailService.SendBooking_TicketingInfo(emailTo, bookAgent.AgentID, "/ExportFile/ExOrder/" + orderId);
            if (mailStaus == 1)
            {
                bookOrder.MailStatus += 1;
                bookOrderService.Update(bookOrder);
                //
                return Notifization.Success("Thư của bạn đã được gửi tới khách hàng");
            }
            return Notifization.Invalid(MessageText.Invalid);

        }

        //##############################################################################################################################################################################################################################################################

        public static string DropdownListBookOrderStatus(int id)
        {
            try
            {
                BookOrderService bookOrderService = new BookOrderService();
                List<StatusModel> orderStatusModels = bookOrderService.OrderStatusData();
                string result = string.Empty;
                foreach (var item in orderStatusModels)
                {
                    string selected = string.Empty;
                    if (item.ID == id)
                        selected = "selected";
                    result += "<option value='" + item.ID + "' " + selected + ">" + item.Title + "</option>";
                }
                return result;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static string DropdownListOrderItinerary(int id)
        {
            try
            {
                BookOrderService bookOrderService = new BookOrderService();
                List<StatusModel> dataList = bookOrderService.OrderItineraryData();
                string result = string.Empty;
                foreach (var item in dataList)
                {
                    string selected = string.Empty;
                    if (item.ID == id)
                        selected = "selected";
                    result += "<option value='" + item.ID + "' " + selected + ">" + item.Title + "</option>";
                }
                return result;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        //##############################################################################################################################################################################################################################################################
        public List<StatusModel> OrderStatusData()
        {
            return new List<StatusModel>{
                    new StatusModel(-1, "Giữ chỗ"),
                    new StatusModel(2, "Hủy chỗ"),
                    new StatusModel(3, "Xuất vé"),
                };
        }

        public List<StatusModel> OrderItineraryData()
        {
            return new List<StatusModel>{
                    new StatusModel(1, "Nội địa"),
                    new StatusModel(2, "Quốc tế")
                };
        }

        //##############################################################################################################################################################################################################################################################


        public class DownLoadTest
        {
            public string GuidID { get; set; }
            public string FileName { get; set; }
            public byte[] DataFile { get; set; }
        }
    }
}
