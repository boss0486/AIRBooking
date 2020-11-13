using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using Helper;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            string sqlQuery = @"SELECT o.*,c.ContactType, c.Name as 'ContactName'  
            FROM App_BookOrder as o LEFT JOIN App_BookContact as c ON c.BookOrderID = o.ID
            WHERE (o.Title LIKE N'%'+ @Query +'%' OR o.PNR LIKE N'%'+ @Query +'%')  
            " + whereCondition + " ORDER BY o.OrderDate, o.[Title] ASC";
            var dtList = _connection.Query<BookOrderResult>(sqlQuery, new { Query = Helper.Page.Library.FormatToUni2NONE(query), OrderStatus = orderStatus, ItineraryType = model.ItineraryType, AgentID = agentId, CompanyID = compId }).ToList();
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
    }
}
