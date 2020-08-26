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

namespace WebCore.Services
{
    public interface IAppBookTicketService : IEntityService<AppBookTicket> { }
    public class AppBookTicketService : EntityService<AppBookTicket>, IAppBookTicketService
    {
        public AppBookTicketService() : base() { }
        public AppBookTicketService(System.Data.IDbConnection db) : base(db) { }
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

        //    string sqlQuery = @"SELECT * FROM View_App_BookTicket WHERE ID != '' " + whereCondition + " ORDER BY [CreatedDate]";
        //    var dtList = _connection.Query<AppBookTicket>(sqlQuery, new { Query = query }).ToList();
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

        public string BookOrder(BookOrder model)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    AppBookTicketService appBookTicketService = new AppBookTicketService(_connection);
                    AppBookPassengerService appBookPassengerService = new AppBookPassengerService(_connection);
                    AppBookFareService appBookFareService = new AppBookFareService(_connection);
                    var flight = model.BookOrderFlight;
                    var direction = flight.Direction;
                    var pnr = flight.PNR;
                    var bookTickId = appBookTicketService.Create<string>(new AppBookTicket
                    {
                        PNR = pnr,
                        Summary = flight.Summary,
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
                        Amount = flight.Amount,
                        FareBase = flight.FareBase,
                        ReturnID = flight.ReturnID,
                        Enabled = (int)Model.Enum.ModelEnum.Enabled.ENABLED,
                        SiteID = "66D1F8F4-57D3-4497-A5D8-C4CD3C591261"
                    }, transaction: transaction);

                    // chi luu thong tin fare va passenger  cho chieu di chieu ve ko luu, 
                    if (direction == 1)//int)FlightDirection.FlightGo
                    {
                        List<BookOrderPassenger> bookOrderPassengers = model.BookOrderPassengers;
                        if (bookOrderPassengers.Count > 0)
                        {
                            foreach (var passenger in bookOrderPassengers)
                            {
                                var passengerId = appBookPassengerService.Create<string>(new AppBookPassenger
                                {
                                    PNR = pnr,
                                    BookTicketID = bookTickId,
                                    PassengerType = passenger.PassengerType,
                                    FullName = passenger.FullName,
                                    Gender = passenger.Gender,
                                    DateOfBirth = passenger.DateOfBirth,
                                }, transaction: transaction);
                            }
                        }
                        //
                        var bookOrderFares = model.BookOrderFares;
                        if (bookOrderFares.Count > 0)
                        {
                            foreach (var item in bookOrderFares)
                            {
                                var fareId = appBookFareService.Create<string>(new AppBookFare
                                {
                                    BookTicketID = bookTickId,
                                    PassengerType = item.PassengerType,
                                    PNR = pnr,
                                    Title = item.Title,
                                    TaxCode = item.TaxCode,
                                    Amount = item.Amount
                                }, transaction: transaction);
                            }
                        }
                    }
                    //
                    transaction.Commit();
                    _connection.Close();
                    return bookTickId;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _connection.Close();
                    return string.Empty + ex;
                }
            }
        }

        public Response_AppBookDetailsModel BookOrderDetails(string id)
        {
            Response_AppBookDetailsModel response_AppBookDetails = new Response_AppBookDetailsModel();
            List<Response_AppBookTicketDetailsModel> response_AppBookTicketDetails = new List<Response_AppBookTicketDetailsModel>();
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return response_AppBookDetails;
                //
                id = id.ToLower();
                AppBookTicketService appBookTicketService = new AppBookTicketService(_connection);
                AppBookPassengerService appBookPassengerService = new AppBookPassengerService(_connection);
                AppBookFareService appBookFareService = new AppBookFareService(_connection);

                var appBookTickets = appBookTicketService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && (m.ID.ToLower().Equals(id) || (!string.IsNullOrWhiteSpace(m.ID) && m.ReturnID.ToLower().Equals(id)))).OrderBy(m => m.Direction).ToList();
                if (appBookTickets.Count == 0)
                    return response_AppBookDetails;

                List<Response_AppBookPassengerDetailsModel> response_AppBookPassengerDetails = new List<Response_AppBookPassengerDetailsModel>();
                foreach (var item in appBookTickets)
                {
                    var appBookPassengers = appBookPassengerService.GetAlls(m => !string.IsNullOrWhiteSpace(m.BookTicketID) && (m.BookTicketID.ToLower().Equals(item.ID.ToLower()))).ToList();
                    if (appBookPassengers.Count > 0)
                    {
                        foreach (var passenger in appBookPassengers)
                        {
                            response_AppBookPassengerDetails.Add(new WebCore.Entities.Response_AppBookPassengerDetailsModel
                            {
                                PassengerType = passenger.PassengerType,
                                FullName = passenger.FullName,
                                Gender = passenger.Gender,
                                DateOfBirth = passenger.DateOfBirth
                            });
                            // 
                        }
                    }

                    List<AppBookFare> response_AppBookFareDetails = new List<AppBookFare>();
                    var appBookFares = appBookFareService.GetAlls(m => !string.IsNullOrWhiteSpace(m.BookTicketID) && (m.BookTicketID.ToLower().Equals(item.ID.ToLower()) || m.BookTicketID.ToLower().Equals(item.ReturnID.ToLower())) ).ToList();

                    response_AppBookTicketDetails.Add(new Response_AppBookTicketDetailsModel
                    {
                        PNR = item.PNR,
                        Direction = item.Direction,
                        NumberInParty = item.NumberInParty,
                        OriginLocation = item.OriginLocation,
                        DestinationLocation = item.DestinationLocation,
                        DepartureDateTime = item.DepartureDateTime,
                        ArrivalDateTime = item.ArrivalDateTime,
                        ResBookDesigCode = item.ResBookDesigCode,
                        FlightNumber = item.FlightNumber,
                        AirEquipType = item.AirEquipType,
                        Amount = item.Amount, 
                         BookFares = appBookFares
                    });
                    // add ticket
                }
                response_AppBookDetails.AppBookTicketDetails = response_AppBookTicketDetails;
                response_AppBookDetails.AppBookPassengerDetails = response_AppBookPassengerDetails.OrderBy(m => m.PassengerType).ToList();
                return response_AppBookDetails;
            }
            catch (Exception)
            {
                return response_AppBookDetails;
            }
        }
        public static int ConvertGenderToNumber(string str)
        {
            int result = 0;
            try
            {
                switch (str)
                {
                    case "M":
                        result = 1;
                        break;
                    case "F":
                        result = 2;
                        break;
                }
                return result;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static string DDLGender(string str = null)
        {
            string result = string.Empty;
            List<GenderModel> genderModels = new List<GenderModel>
            {
                new GenderModel {
                    Text = "Nam",
                    Value = 1
                },
                new GenderModel {
                    Text = "Nữ",
                    Value = 2
                }
            };

            foreach (var item in genderModels)
            {
                string attrActive = "";
                if (str != null && item.Value == ConvertGenderToNumber(str))
                {
                    attrActive = "selected";
                }
                result += "<option value=" + item.Value + "  " + attrActive + ">" + item.Text + "</option>";
            }
            return result;
        }



    }


}
