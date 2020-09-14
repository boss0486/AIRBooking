using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using System;
using Dapper;
using System.Linq;
using PagedList;
using System.Web.Mvc;
using System.Collections.Generic;
using Helper;
using System.Web;
using WebCore.Entities;
using AIRService.WebService.VNA.Enum;

namespace WebCore.Services
{
    public interface IOrderTicketService : IEntityService<OrderTicket> { }
    public class OrderTicketService : EntityService<OrderTicket>, IOrderTicketService
    {
        public OrderTicketService() : base() { }
        public OrderTicketService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        //public ActionResult Datalist(string strQuery, int page)
        //{
        //    string query = string.Empty;
        //    if (string.IsNullOrEmpty(strQuery))
        //        query = "";
        //    else
        //        query = strQuery;
        //    string langID = Current.LanguageID;
        //    string whereCondition = string.Empty;

        //    string sqlQuery = @"SELECT * FROM App_BookTicket WHERE ID != '' " + whereCondition + " ORDER BY [CreatedDate]";
        //    var dtList = _connection.Query<OrderTicket>(sqlQuery, new { Query = query }).ToList();
        //    if (dtList.Count <= 0)
        //        return Notifization.NotFound(NotifizationText.NotFound);
        //    //    
        //    var result = dtList.ToPagedList(page, Library.Paging.PAGESIZE).ToList();
        //    if (result.Count <= 0 && page > 1)
        //    {
        //        page -= 1;
        //        result = dtList.ToPagedList(page, Library.Paging.PAGESIZE).ToList();
        //    }
        //    if (result.Count <= 0)
        //        return Notifization.NotFound(NotifizationText.NotFound);

        //    Library.PagingModel pagingModel = new Library.PagingModel
        //    {
        //        PageSize = Library.Paging.PAGESIZE,
        //        Total = dtList.Count,
        //        Page = page
        //    };
        //    Helper.Model.RoleAccountModel roleAccountModel = new Helper.Model.RoleAccountModel
        //    {
        //        Create = true,
        //        Update = true,
        //        Details = true,
        //        Delete = true,
        //        Block = true,
        //        Active = true,
        //    };
        //    return Notifization.DATALIST(NotifizationText.Success, data: result, role: roleAccountModel, paging: pagingModel);
        //}

        public string BookOrder(Request_OrderTicketModel model)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    OrderCodeService orderCodeService = new OrderCodeService(_connection);
                    OrderTicketService orderTicketService = new OrderTicketService(_connection);
                    OrderTaxService orderTaxService = new OrderTaxService(_connection);
                    OrderPriceService orderPriceService = new OrderPriceService(_connection);
                    var flights = model.Flights;
                    var direction = (int)VNAEnum.FlightDirection.None;
                    var orderTickId = "";
                    int cnt = 0;
                    var priceFares = model.PriceFare;
                    var taxFares = model.TaxFare;
                    string keyCode = Helper.Security.Library.KEYCode;
                    string orderCodeId = orderCodeService.Create<string>(new OrderCode
                    {
                        AirlineID = "VNA",
                        Title = "VNA-" + keyCode,
                        Alias = "VNA-" + keyCode,
                        Summary = "Viet nam Airline",
                        Status = (int)ENM.BookTicketEnum.BookPNRCodeStatus.Booking,
                        Enabled = (int)Model.Enum.ModelEnum.Enabled.ENABLED,
                        SiteID = "-"
                    }, transaction: transaction);

                    foreach (var flight in flights)
                    {
                        if (cnt == 0)
                            direction = (int)VNAEnum.FlightDirection.FlightGo;
                        if (cnt == 1)
                            direction = (int)VNAEnum.FlightDirection.FlightReturn;
                        // flight
                        orderTickId = orderTicketService.Create<string>(new OrderTicket
                        {
                            CategoryID = orderCodeId,
                            Summary = "",
                            ADT = flight.ADT,
                            CNN = flight.CNN,
                            INF = flight.INF,
                            Direction = direction,
                            NumberInParty = flight.NumberInParty,
                            OriginLocation = flight.OriginLocation,
                            DestinationLocation = flight.DestinationLocation,
                            DepartureDateTime = flight.DepartureDateTime,
                            ArrivalDateTime = flight.ArrivalDateTime,
                            ResBookDesigCode = flight.ResBookDesigCode,
                            FlightNumber = flight.FlightNumber,
                            AirEquipType = flight.AirEquipType,
                            ReturnID = orderTickId
                        }, transaction: transaction);
                        // price 
                        foreach (var price in priceFares)
                        {
                            if (price.FlightType == direction)
                            {
                                var orderPriceId = orderPriceService.Create<string>(new OrderPrice
                                {
                                    FlightType = price.FlightType,
                                    TicketID = orderTickId,
                                    PassengerType = price.PassengerType,
                                    Title = "Fare Price",
                                    Amount = price.Amount,
                                    Unit = "VND"
                                }, transaction: transaction);
                            }
                        }
                        cnt++;
                    }
                    // tax

                    if (taxFares.Count > 0)
                    {
                        foreach (var item in taxFares)
                        {
                            var orderTaxId = orderTaxService.Create<string>(new OrderTax
                            {
                                PassengerType = item.PassengerType,
                                Title = item.Title,
                                TaxCode = item.TaxCode,
                                Amount = item.Amount,
                                Unit = "VND"
                            }, transaction: transaction);
                        }
                    }
                    //
                    transaction.Commit();
                    _connection.Close();
                    return orderTickId;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _connection.Close();
                    return string.Empty + ex;
                }
            }
        }
    }


}
