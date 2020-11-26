using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using Helper;
using OfficeOpenXml;
using PagedList;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebCore.Entities;
using WebCore.Model.Entities;

namespace WebCore.Services
{
    public interface IBookOrderService : IEntityService<BookOrder> { }
    public class BookOrderService : EntityService<BookOrder>, IBookOrderService
    {
        public BookOrderService() : base() { }
        public BookOrderService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public ActionResult BookOrder(BookOrderSerch model, int orderStatus = (int)ENM.BookOrderEnum.BookOrderStatus.None)
        {
            if (model == null)
                return Notifization.Invalid(MessageText.Invalid);
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
            }, dateColumn: "OrderDate");
            if (searchResult != null)
            {
                if (searchResult.Status == 1)
                    whereCondition = searchResult.Message;
                else
                    return Notifization.Invalid(searchResult.Message);
            }
            // 
            if (orderStatus == (int)WebCore.ENM.BookOrderEnum.BookOrderStatus.None && model.OrderStatus != (int)WebCore.ENM.BookOrderEnum.BookOrderStatus.None)
            {
                whereCondition += " AND o.OrderStatus = @OrderStatus";
                orderStatus = model.OrderStatus;
            }
            //
            if (model.ItineraryType != (int)WebCore.ENM.BookOrderEnum.BookItineraryType.None)
                whereCondition += " AND o.ItineraryType = @ItineraryType";
            //
            string agentId = model.AgentID;
            if (!string.IsNullOrWhiteSpace(agentId))
                whereCondition += " AND o.AgentID = @AgentID";
            //
            string compId = model.CompanyID;
            if (!string.IsNullOrWhiteSpace(compId))
                whereCondition += " AND c.CompanyID = @CompanyID";
            // query
            string sqlQuery = @"SELECT o.*,c.ContactType, c.Name as 'ContactName', c.CompanyID, c.CompanyCode  
            FROM App_BookOrder as o LEFT JOIN App_BookContact as c ON c.BookOrderID = o.ID
            WHERE (o.Title LIKE N'%'+ @Query +'%' OR o.PNR LIKE N'%'+ @Query +'%')  
            " + whereCondition + " ORDER BY o.OrderDate, o.[Title] ASC";
            var dtList = _connection.Query<BookOrderResult>(sqlQuery, new { Query = Helper.Page.Library.FormatNameToUni2NONE(query), OrderStatus = orderStatus, ItineraryType = model.ItineraryType, AgentID = agentId, CompanyID = compId }).ToList();
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
            return Notifization.Data(MessageText.Success + sqlQuery, data: result, role: RoleActionSettingService.RoleListForUser(), paging: pagingModel);
        }
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
            BookContactService bookContactService = new BookContactService(_connection);
            data.BookContacts = bookContactService.GetAlls(m => m.BookOrderID == data.ID).ToList();
            //
            return data;
        }
        public static string ViewOrderCustomerType(int state)
        {
            string result = string.Empty;
            switch (state)
            {
                case 1:
                    result = "Khách lẻ";
                    break;
                case 2:
                    result = "Công ty";
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
        //
        public string BookEmail(string bookId)
        {
            if (string.IsNullOrWhiteSpace(bookId))
                return null;
            //
            bookId = bookId.Trim().ToLower();






            return string.Empty;
        }

        public ActionResult GetPdf(string id)
        {
            //// Validate the Model is correct and contains valid data
            //// Generate your report output based on the model parameters
            //// This can be an Excel, PDF, Word file - whatever you need.

            //// As an example lets assume we've generated an EPPlus ExcelPackage

            //ExcelPackage workbook = new ExcelPackage();
            //// Do something to populate your workbook
            //ExcelWorksheet ws = workbook.Workbook.Worksheets.Add("testsheet");
            //// Generate a new unique identifier against which the file can be stored
            //string handle = Guid.NewGuid().ToString();

            //using (MemoryStream memoryStream = new MemoryStream())
            //{
            //    ws.Cells["B1"].Value = "Number of Used Agencies";
            //    ws.Cells["C1"].Value = "Active Agencies";
            //    ws.Cells["D1"].Value = "Inactive Agencies";
            //    ws.Cells["E1"].Value = "Total Hours Volunteered";
            //    ws.Cells["B1:E1"].Style.Font.Bold = true;
            //    workbook.SaveAs(memoryStream);
            //    memoryStream.Position = 0;
            //    // Note we are returning a filename as well as the handle

            //    return new DownLoadTest
            //    {
            //        GuidID = handle,
            //        FileName = "TestReportOutput.xlsx",
            //        DataFile = memoryStream.ToArray(),


            //    };
            //}
            if (string.IsNullOrWhiteSpace(id))
                return Notifization.Invalid(MessageText.Invalid);
            // 
            var converter = new HtmlToPdf();


            string urlPage = Helper.Page.WebPage.Domain + "/ExportFile/ExOrder/" + id;

            var doc = converter.ConvertUrl(urlPage);
            string fileFolderPath = HttpContext.Current.Server.MapPath(@"~/Files/Export/Order/");
            var fileName = string.Format("{0}_ticketing.pdf", Guid.NewGuid() + "_1");
            string pathFile = fileFolderPath + fileName;
            //List<string> files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith("_ticketing.pdf")).ToList();
            if (File.Exists(pathFile))
                File.Delete(pathFile);
            //
            doc.Save(pathFile);
            doc.Close();

            string mailMessage = Helper.Email.EMailService.MailExcuteTest("vietnt.itt@gmail.com", "Thông tin đặt vé", "Đặt vé", pathFile);

            return Notifization.DownLoadFile("ok:" + mailMessage, fileFolderPath);
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


        public class DownLoadTest
        {
            public string GuidID { get; set; }
            public string FileName { get; set; }
            public byte[] DataFile { get; set; }
        }
    }
}
