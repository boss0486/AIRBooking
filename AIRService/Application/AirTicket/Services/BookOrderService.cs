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
        public ActionResult DataList(BookOrderSerch model)
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
            });
            if (searchResult != null)
            {
                if (searchResult.Status == 1)
                    whereCondition = searchResult.Message;
                else
                    return Notifization.Invalid(searchResult.Message);
            }
            //
            string areaId = model.AreaID;
            if (!string.IsNullOrWhiteSpace(areaId) && areaId != "-")
                whereCondition += " AND AreaID = @AreaID ";
            // query
            string sqlQuery = @"SELECT * 
            ,(SELECT SUM(Amount) FROM App_BookPrice WHERE PNR = o.PNR) as TotalAmount  
            ,(SELECT Top 1 Name FROM App_BookContact WHERE  App_BookContact.BookOrderID = o.ID) as ContactName 
            FROM App_BookOrder as o WHERE o.Title LIKE N'%'+ @Query +'%'" + whereCondition + " ORDER BY o.[Title] ASC";
            var dtList = _connection.Query<BookOrderResult>(sqlQuery, new { Query = Helper.Page.Library.FormatToUni2NONE(query), AreaID = areaId }).ToList();
            if (dtList.Count == 0)
                return Notifization.NotFound(MessageText.NotFound);
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

    }
}
