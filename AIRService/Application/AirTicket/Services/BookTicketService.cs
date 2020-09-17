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
    public interface IBookTicketService : IEntityService<BookTicket> { }
    public class BookTicketService : EntityService<BookTicket>, IBookTicketService
    {
        public BookTicketService() : base() { }
        public BookTicketService(System.Data.IDbConnection db) : base(db) { }
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

        public string BookTicket(BookTicketOrder model)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    BookPNRCodeService bookPNRCodeService = new BookPNRCodeService(_connection);
                    BookTicketService bookTicketService = new BookTicketService(_connection);
                    BookPassengerService bookPassengerService = new BookPassengerService(_connection);
                    BookTaxService bookFareService = new BookTaxService(_connection);
                    BookPriceService bookPriceService = new BookPriceService(_connection);
                    BookContactService bookContactService = new BookContactService(_connection);

                    var pnr = model.PNR;
                    var flights = model.Flights;
                    var direction = (int)VNAEnum.FlightDirection.None;
                    var passengers = model.Passengers;
                    var bookTickId = "";
                    int cnt = 0;
                    var fareFlights = model.FareFlights;
                    string pnrId = bookPNRCodeService.Create<string>(new BookPNRCode
                    {
                        OrderID = model.OrderID,
                        AirlineID = "VNA",
                        Title = pnr,
                        Alias = "VNA-" + pnr,
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
                        bookTickId = bookTicketService.Create<string>(new BookTicket
                        {

                            CategoryID = pnrId,
                            PNR = pnr,
                            Summary = "",
                            ADT = passengers.Where(m => m.PassengerType == "ADT").Count(),
                            CNN = passengers.Where(m => m.PassengerType == "CNN").Count(),
                            INF = passengers.Where(m => m.PassengerType == "INF").Count(),
                            Direction = direction,
                            NumberInParty = flight.NumberInParty,
                            OriginLocation = flight.OriginLocation,
                            DestinationLocation = flight.DestinationLocation,
                            DepartureDateTime = flight.DepartureDateTime,
                            ArrivalDateTime = flight.ArrivalDateTime,
                            ResBookDesigCode = flight.ResBookDesigCode,
                            FlightNumber = flight.FlightNumber,
                            AirEquipType = flight.AirEquipType,
                            Amount = 0,
                            FareBase = 0,
                            ReturnID = bookTickId,
                            Enabled = (int)Model.Enum.ModelEnum.Enabled.ENABLED
                        }, transaction: transaction);
                        // price 
                        foreach (var price in fareFlights)
                        {
                            if (price.FlightType == direction)
                            {
                                var fareId = bookPriceService.Create<string>(new BookPrice
                                {
                                    PNR = pnr,
                                    FlightType = price.FlightType,
                                    TicketID = bookTickId,
                                    PassengerType = price.PassengerType,
                                    Title = "Fare Price",
                                    Amount = price.Amount,
                                    Unit = "VND"
                                }, transaction: transaction);
                            }
                        }
                        cnt++;
                    }

                    // Passenger
                    if (passengers.Count > 0)
                    {
                        foreach (var passenger in passengers)
                        {
                            var passengerId = bookPassengerService.Create<string>(new BookPassenger
                            {
                                PNR = pnr,
                                PassengerType = passenger.PassengerType,
                                FullName = passenger.FullName,
                                Gender = passenger.Gender,
                                DateOfBirth = passenger.DateOfBirth,
                            }, transaction: transaction);
                        }
                    }
                    // tax
                    var booFareTaxs = model.FareTaxs;
                    if (booFareTaxs.Count > 0)
                    {
                        foreach (var item in booFareTaxs)
                        {
                            var fareId = bookFareService.Create<string>(new BookTax
                            {
                                PNR = pnr,
                                PassengerType = item.PassengerType,
                                Title = item.Text,
                                TaxCode = item.TaxCode,
                                Amount = item.Amount,
                                Unit = "VND"
                            }, transaction: transaction);
                        }
                    }
                    // contact
                    var bookContacts = model.Contacts;
                    if (bookContacts.Count > 0)
                    {
                        foreach (var item in bookContacts)
                        {
                            var fareId = bookContactService.Create<string>(new BookContact
                            {
                                PNR = pnr,
                                Name = item.Name,
                                Email = item.Email,
                                Phone = item.Phone,
                            }, transaction: transaction);
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

        public BookTicketOrderDetails BookTicketDetails(string pnr)
        {
            BookTicketOrderDetails bookTicketOrderDetails = new BookTicketOrderDetails();
            List<BookTicketDetails> bookTicketDetails = new List<BookTicketDetails>();
            //try
            //{
            if (string.IsNullOrWhiteSpace(pnr))
                return bookTicketOrderDetails;
            //
            pnr = pnr.ToLower();
            BookTicketService bookTicketService = new BookTicketService(_connection);
            BookPassengerService bookPassengerService = new BookPassengerService(_connection);
            BookTaxService bookFareService = new BookTaxService(_connection);
            BookContactService bookContactService = new BookContactService(_connection);


            BookPriceService bookPriceService = new BookPriceService(_connection);
            var bookTickets = bookTicketService.GetAlls(m => !string.IsNullOrWhiteSpace(m.PNR) && m.PNR.ToLower() == pnr).OrderBy(m => m.Direction).ToList();
            if (bookTickets.Count == 0)
                return bookTicketOrderDetails;

            List<BookPassengerDetails> bookPassengerDetails = new List<BookPassengerDetails>();
            foreach (var item in bookTickets)
            {
                bookTicketDetails.Add(new BookTicketDetails
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
                    BookFares = null
                });
                // add ticket
            }

            var bookPassengers = bookPassengerService.GetAlls(m => !string.IsNullOrWhiteSpace(m.PNR) && m.PNR.ToLower() == pnr).ToList();
            if (bookPassengers.Count > 0)
            {
                foreach (var item in bookPassengers)
                {
                    bookPassengerDetails.Add(new BookPassengerDetails
                    {
                        FullName = item.FullName,
                        PassengerType = item.PassengerType,
                        Gender = item.Gender,
                        DateOfBirth = Helper.Time.TimeHelper.FormatToDate(item.DateOfBirth, languageCode: Helper.Language.LanguageCode.Vietnamese.ID)
                    });
                }

            }
            //
            List<BookFareDetails> lstbookFareDetails = new List<BookFareDetails>();
            var passengerType = bookPassengerDetails.GroupBy(m => new { m.PassengerType }).Select(m => new { m.Key.PassengerType }).ToList();
            foreach (var item in passengerType)
            {
                int passengerQty = bookPassengerService.GetAlls(m => !string.IsNullOrWhiteSpace(m.PassengerType) && m.PassengerType == item.PassengerType).Count();
                double priceTotal = bookPriceService.GetAlls(m => !string.IsNullOrWhiteSpace(m.PNR) && m.PNR.ToLower() == pnr && m.PassengerType == item.PassengerType).Sum(m => m.Amount);


                List<BookTax> bookTaxs = bookFareService.GetAlls(m => !string.IsNullOrWhiteSpace(m.PNR) && m.PNR.ToLower() == pnr && m.PassengerType == item.PassengerType).ToList();
                double taxTotal = bookTaxs.Sum(m => m.Amount);
                List<BookTax> bookFareTaxs = new List<BookTax>();

                lstbookFareDetails.Add(new BookFareDetails
                {
                    PassengerType = item.PassengerType,
                    PassengerQty = passengerQty,
                    PriceTotal = priceTotal,
                    TaxTotal = taxTotal,
                    FareTaxs = bookTaxs
                });
            }
            // contact
            List<BookContacDetails> bookContacDetails = new List<BookContacDetails>();
            var contactList = bookContactService.GetAlls(m => !string.IsNullOrWhiteSpace(m.PNR) && m.PNR.ToLower() == pnr).ToList();
            if (contactList.Count > 0)
            {
                foreach (var item in contactList)
                {
                    bookContacDetails.Add(new BookContacDetails
                    {
                        Name = item.Name,
                        Email = item.Email,
                        Phone = item.Phone
                    });
                }

            }

            lstbookFareDetails = lstbookFareDetails.OrderBy(m => m.PassengerType).ToList();
            bookTicketOrderDetails.BookTickets = bookTicketDetails;
            bookTicketOrderDetails.BookFares = lstbookFareDetails;
            bookTicketOrderDetails.BookPassengers = bookPassengerDetails.OrderBy(m => m.PassengerType).ToList();
            bookTicketOrderDetails.Contacts = bookContacDetails;
            return bookTicketOrderDetails;
            //}
            //catch (Exception)
            //{
            //    return response_AppBookDetails;
            //}
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

        public static string ConvertToGenderName(int _gender)
        {
            string result = string.Empty;
            switch (_gender)
            {
                case 1:
                    result = "Nam";
                    break;
                case 2:
                    result = "Nữ";
                    break;
            }
            return result;
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
