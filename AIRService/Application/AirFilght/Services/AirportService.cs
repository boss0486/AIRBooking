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
    public interface IAirportService : IEntityService<Entities.Airport> { }
    public class AirportService : EntityService<Entities.Airport>, IAirportService
    {
        public AirportService() : base() { }
        public AirportService(System.Data.IDbConnection db) : base(db) { }

        //##############################################################################################################################################################################################################################################################
        public ActionResult DataList(AirportSearch model)
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
            string sqlQuery = $@"
            SELECT ap.*, anl.Title as 'AreaNational', al.Title as 'National',ai.Title as 'AreaInland' FROM App_Airport as ap 
            LEFT JOIN App_AreaInland as ai ON ai.ID = ap.AreaInlandID
            LEFT JOIN App_National al ON al.ID =  ai.NationalID 
            LEFT JOIN App_AreaNational anl ON anl.ID = al.AreaNationalID
            WHERE (dbo.Uni2NONE(ap.IATACode) LIKE N'%'+ @Query +'%' 
            OR dbo.Uni2NONE(ap.Title) LIKE N'%'+ @Query +'%' 
            OR dbo.Uni2NONE(anl.Title) LIKE N'%'+ @Query +'%' 
            OR dbo.Uni2NONE(al.Title) LIKE N'%'+ @Query +'%' 
            OR dbo.Uni2NONE(ai.Title) LIKE N'%'+ @Query +'%')
            {whereCondition}  ORDER BY ap.Title ASC";
            var dtList = _connection.Query<AirportResult>(sqlQuery, new { Query = Helper.Page.Library.FormatNameToUni2NONE(query) }).ToList();
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

        public ActionResult Setting(AirportSetting model)
        {
            if (model == null)
                return Notifization.Invalid(MessageText.Invalid);
            //
            string id = model.ID;
            int voidBookTime = model.VoidBookTime;
            int voidTicketTime = model.VoidTicketTime;
            double axfee = model.AxFee;
            //
            if (string.IsNullOrWhiteSpace(id))
                return Notifization.Invalid(MessageText.Invalid);
            //  
            id = id.ToLower();
            AirportConfigService airportConfigService = new AirportConfigService(_connection);
            AirportService flightService = new AirportService(_connection);
            Airport flight = flightService.GetAlls(m => m.ID == id).FirstOrDefault();
            if (flight == null)
                return Notifization.Invalid(MessageText.Invalid);
            //
            if (axfee < 0)
                return Notifization.Invalid("Phí sân bay không hợp lệ");
            //
            if (voidBookTime < 0)
                return Notifization.Invalid("T.gian hủy đặt chỗ không hợp lệ");
            //
            if (voidTicketTime < 0)
                return Notifization.Invalid("T.gian hủy đặt vé không hợp lệ");
            //
            AirportConfig airportConfig = airportConfigService.GetAlls(m => m.AirportID == id).FirstOrDefault();
            if (airportConfig == null)
            {
                airportConfigService.Create<string>(new AirportConfig
                {
                    AirportID = id,
                    AxFee = axfee,
                    VoidBookTime = voidBookTime,
                    VoidTicketTime = voidTicketTime
                });
            }
            // update
            airportConfig.AxFee = axfee;
            airportConfig.VoidBookTime = voidTicketTime;
            airportConfig.VoidTicketTime = voidTicketTime;
            airportConfigService.Update(airportConfig);
            //
            return Notifization.Success(MessageText.UpdateSuccess);
        }
        public ActionResult Create(AirportCreateModel model)
        {
            if (model == null)
                return Notifization.Invalid(MessageText.Invalid);
            //
            string nationalId = model.NationalID;
            string areaInlandId = model.AreaInlandID;
            string title = model.Title;
            //
            AirportService flightService = new AirportService(_connection);
            if (string.IsNullOrWhiteSpace(areaInlandId))
                return Notifization.Invalid("Vui lòng chọn vùng/miền");
            //
            nationalId = nationalId.ToLower();
            areaInlandId = areaInlandId.ToLower();
            // 
            var flight = flightService.GetAlls(m => m.Title.ToLower() == model.Title.ToLower()).FirstOrDefault();
            if (flight != null)
                return Notifization.Invalid("Tên chuyến bay đã được sử dụng");
            //
            flight = flightService.GetAlls(m => m.IATACode.ToLower() == model.IATACode.ToLower()).FirstOrDefault();
            if (flight != null)
                return Notifization.Invalid("Mã IATA đã được sử dụng");
            // 
            flightService.Create<string>(new Entities.Airport()
            {
                NationalID = nationalId,
                AreaInlandID = areaInlandId,
                Title = model.Title,
                Alias = Helper.Page.Library.FormatToUni2NONE(model.Title),
                Summary = model.Summary,
                IATACode = model.IATACode.ToUpper(),
                Enabled = model.Enabled
            });
            return Notifization.Success(MessageText.CreateSuccess);
        }
        public ActionResult Update(AirportUpdateModel model)
        {
            if (model == null)
                return Notifization.Invalid(MessageText.Invalid);
            //
            string id = model.ID;
            string nationalId = model.NationalID;
            string areaInlandId = model.AreaInlandID;
            string title = model.Title;
            //
            if (string.IsNullOrWhiteSpace(id))
                return Notifization.NotFound(MessageText.NotFound);
            //
            if (string.IsNullOrWhiteSpace(nationalId))
                return Notifization.Invalid("Vui lòng chọn quốc gia");
            //
            //
            if (string.IsNullOrWhiteSpace(nationalId))
                return Notifization.Invalid("Vui lòng chọn vùng/miền");
            //
            id = id.ToLower();
            nationalId = nationalId.ToLower();
            areaInlandId = areaInlandId.ToLower();
            AirportService flightService = new AirportService(_connection);
            var flight = flightService.GetAlls(m => m.ID == id).FirstOrDefault();
            if (flight == null)
                return Notifization.NotFound(MessageText.NotFound);
            //
            var flightValid = flightService.GetAlls(m => !string.IsNullOrWhiteSpace(m.Title) && m.Title.ToLower() == title.ToLower() && flight.ID != id).FirstOrDefault();
            if (flightValid != null)
                return Notifization.Invalid("Tên chuyến bay đã được sử dụng");
            //
            flightValid = flightService.GetAlls(m => !string.IsNullOrWhiteSpace(m.IATACode) && m.IATACode.ToLower() == model.IATACode.ToLower() && flight.ID != id).FirstOrDefault();
            if (flightValid != null)
                return Notifization.Invalid("Mã IATA đã được sử dụng");
            // 
            flight.AreaInlandID = model.AreaInlandID;
            flight.NationalID = model.NationalID;
            flight.Title = title;
            flight.Alias = Helper.Page.Library.FormatToUni2NONE(model.Title);
            flight.Summary = model.Summary;
            flight.IATACode = model.IATACode.ToUpper();
            flight.Enabled = model.Enabled;
            flightService.Update(flight);
            //
            return Notifization.Success(MessageText.UpdateSuccess);
        }
        public Airport GetAirAportModel(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;
            //
            string sqlQuery = @"SELECT TOP (1) * FROM App_Airport WHERE ID = @Query";
            Airport airAirport = _connection.Query<Airport>(sqlQuery, new { Query = id }).FirstOrDefault();
            //
            if (airAirport == null)
                return null;
            //
            return airAirport;
        }
        public AirportResult ViewAirAportModel(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;
            //
            string sqlQuery = @"SELECT TOP (1) * FROM App_Airport WHERE ID = @Query";
            AirportResult airAirport = _connection.Query<AirportResult>(sqlQuery, new { Query = id }).FirstOrDefault();
            //
            if (airAirport == null)
                return null;
            //
            return airAirport;
        }

        public AirportSetting GetAirportConfigModel(string id)
        {

            if (string.IsNullOrWhiteSpace(id))
                return null;
            //
            string sqlQuery = @"SELECT ap.ID, ap.Title,ap.IATACode, apf.AxFee, apf.VoidBookTime, apf.VoidTicketTime FROM App_Airport as ap
            LEFT JOIN App_AirportConfig as apf on apf.AirportID = ap.ID WHERE ap.ID = @Query";
            AirportSetting airAirport = _connection.Query<AirportSetting>(sqlQuery, new { Query = id }).FirstOrDefault();
            //
            if (airAirport == null)
                return null;
            //
            return airAirport;
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Delete(AirportIDModel model)
        {
            string id = model.ID;
            if (id == null)
                return Notifization.NotFound();
            //
            id = id.ToLower();
            AirportService flightService = new AirportService(_connection);
            var airport = flightService.GetAlls(m => m.ID == id).FirstOrDefault();
            if (airport == null)
                return Notifization.NotFound();
            //
            flightService.Remove(airport.ID);
            return Notifization.Success(MessageText.DeleteSuccess);
        }

        //##############################################################################################################################################################################################################################################################   
        public List<AirportOption> DataOption()
        {
            string sqlQuery = @"SELECT * FROM App_Airport ORDER BY Title ASC";
            List<AirportOption> flightOptions = _connection.Query<AirportOption>(sqlQuery).ToList();
            if (flightOptions.Count == 0)
                return new List<AirportOption>();
            //
            return flightOptions;
        }
        public static string DropdownList(string id)
        {
            try
            {
                AirportService flightService = new AirportService();
                var dtList = flightService.DataOption();
                if (dtList.Count > 0)
                {
                    string result = string.Empty;
                    foreach (var item in dtList)
                    {
                        string select = string.Empty;
                        if (!string.IsNullOrWhiteSpace(item.IATACode) && item.IATACode.ToLower() == id.ToLower())
                            select = "selected";
                        result += "<option value='" + item.IATACode + "'" + select + ">" + item.Title + " - " + item.IATACode + "</option>";
                    }
                    return result;
                }
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        public List<AirportFromToOption> FlightFromToData()
        {
            try
            {
                AirportService airportService = new AirportService();
                string sqlQuery = @"SELECT CONCAT(a1.IATACode,'-',a2.IATACode) AS ID, a1.Title as 'Departure', a2.Title AS 'Destination' FROM App_Airport a1 RIGHT JOIN App_Airport as a2 On a1.IATACode != a2.IATACode ORDER BY a1.IATACode";
                List<AirportFromToOption> flightOptions = _connection.Query<AirportFromToOption>(sqlQuery).ToList();
                if (flightOptions.Count == 0)
                    return new List<AirportFromToOption>();
                //
                return flightOptions;
            }
            catch
            {
                return null;
            }
        }
        public static string FlightFromToDropdownList(string id)
        {
            try
            {
                AirportService airportService = new AirportService();
                var dtList = airportService.FlightFromToData();
                if (dtList.Count > 0)
                {
                    string result = string.Empty;
                    foreach (var item in dtList)
                    {
                        string select = string.Empty;
                        if (!string.IsNullOrWhiteSpace(item.ID) && item.ID.ToLower() == id.ToLower())
                            select = "selected";
                        result += "<option value='" + item.ID + "' data = '" + item.ID + "'" + select + ">" + item.Departure + " -> " + item.Destination + " (x)</option>";
                    }
                    return result;
                }
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string DropdownListForNumber(string str = null)
        {
            string result = string.Empty;
            List<NumberModel> numberModels = new List<NumberModel>
            {
                new NumberModel {
                    Text = "01",
                    Value = 1
                },
                new NumberModel {
                    Text = "02",
                    Value = 2
                },
                new NumberModel
                {
                    Text = "03",
                    Value = 3
                },
                new NumberModel
                {
                    Text = "04",
                    Value = 4
                },
                new NumberModel
                {
                    Text = "05",
                    Value = 5
                },
                new NumberModel
                {
                    Text = "06",
                    Value = 6
                },
                new NumberModel
                {
                    Text = "07",
                    Value = 7
                },
                new NumberModel
                {
                    Text = "08",
                    Value = 8
                },
                new NumberModel {
                    Text = "09",
                    Value = 9
                }
            };


            foreach (var item in numberModels)
            {
                string attrActive = "";
                if (str != null && item.Value == Convert.ToInt32(str))
                {
                    attrActive = "selected";
                }
                result += "<option value=" + item.Value + "  " + attrActive + ">" + item.Text + "</option>";
            }
            return result;
        }


        //<option value="1">1</option>
        //                                   <option value="2">2</option>
        //                                   <option value="3">3</option>
        //                                   <option value="4">4</option>
        //                                   <option value="5">5</option>
        //                                   <option value="6">6</option>
        //                                   <option value="7">7</option>

    }

    public class NumberModel
    {
        public string Text { get; set; }
        public int Value { get; set; }
    }
}
