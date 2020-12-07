using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using Helper;
using Helper.Email;
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
using WebCore.ENM;
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
            WHERE (o.Title LIKE N'%'+ @Query +'%' OR o.PNR LIKE N'%'+ @Query +'%') " + whereCondition + " ORDER BY o.OrderDate, o.[Title] ASC";
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
