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
    public interface IFlightService : IEntityService<Entities.Flight> { }
    public class FlightService : EntityService<Entities.Flight>, IFlightService
    {
        public FlightService() : base() { }
        public FlightService(System.Data.IDbConnection db) : base(db) { }

        //##############################################################################################################################################################################################################################################################
        public ActionResult Datalist(FlightSerch model)
        {
            if (model == null)
                return Notifization.Invalid(NotifizationText.Invalid);
            //
            string query = string.Empty;
            if (string.IsNullOrWhiteSpace(model.Query))
                query = "";
            else
                query = model.Query;
            //
            int page = model.Page;
            string langID = Current.LanguageID;

            string areaId = model.AreaID;
            string _whereCondition = string.Empty;

            if (!string.IsNullOrWhiteSpace(areaId) && areaId != "-")
                _whereCondition += " AND AreaID = @AreaID ";

            string sqlQuery = @"SELECT * FROM View_App_Flight WHERE Title LIKE N'%'+ @Query +'%'" + _whereCondition + "ORDER BY [Title] ASC";
            var dtList = _connection.Query<ViewFlight>(sqlQuery, new { Query = query, AreaID = areaId }).ToList();
            if (dtList.Count == 0)
                return Notifization.NotFound(NotifizationText.NotFound);
            var result = dtList.ToPagedList(page, Library.Paging.PAGESIZE).ToList();
            if (result.Count <= 0 && page > 1)
            {
                page -= 1;
                result = dtList.ToPagedList(page, Library.Paging.PAGESIZE).ToList();
            }
            if (result.Count <= 0)
                return Notifization.NotFound(NotifizationText.NotFound);

            Library.PagingModel pagingModel = new Library.PagingModel
            {
                PageSize = Library.Paging.PAGESIZE,
                Total = dtList.Count,
                Page = page
            };
            Helper.Model.RoleAccountModel roleAccountModel = new Helper.Model.RoleAccountModel
            {
                Create = true,
                Update = true,
                Details = true,
                Delete = true,
                Block = true,
                Active = true,
            };

            return Notifization.DATALIST(NotifizationText.Success, data: result, role: roleAccountModel, paging: pagingModel);
        }

        //##############################################################################################################################################################################################################################################################
        public ActionResult Create(FlightCreateModel model)
        {
            FlightService flightService = new FlightService(_connection);
            var flight = flightService.GetAlls(m => m.Title.ToLower() == model.Title.ToLower()).FirstOrDefault();
            if (flight != null)
                return Notifization.Invalid("Tên chuyến bay đã được sử dụng");
            //
            flight = flightService.GetAlls(m => m.IATACode.ToLower() == model.IATACode.ToLower()).FirstOrDefault();
            if (flight != null)
                return Notifization.Invalid("Mã IATA đã được sử dụng");
            //
            flightService.Create<string>(new Entities.Flight()
            {
                Title = model.Title,
                Alias = Helper.Library.Uni2NONE(model.Title),
                Summary = model.Summary,
                IATACode = model.IATACode.ToUpper(),
                AreaID = model.AreaID,
                Enabled = model.Enabled
            });
            return Notifization.Success(NotifizationText.CREATE_SUCCESS);
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Update(FlightUpdateModel model)
        {
            string id = model.ID;
            if (string.IsNullOrWhiteSpace(id))
                return Notifization.NotFound(NotifizationText.NotFound);
            id = id.ToLower();
            FlightService flightService = new FlightService(_connection);
            var flight = flightService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.ID.ToLower().Equals(id)).FirstOrDefault();
            if (flight == null)
                return Notifization.NotFound(NotifizationText.NotFound);
            //
            string title = model.Title;
            var flightValid = flightService.GetAlls(m => m.Title.ToLower().Equals(title.ToLower()) && !flight.ID.ToLower().Equals(id)).FirstOrDefault();
            if (flightValid != null)
                return Notifization.Invalid("Tên chuyến bay đã được sử dụng");
            //
            flightValid = flightService.GetAlls(m => m.IATACode.ToLower().Equals(model.IATACode.ToLower()) && !flight.ID.ToLower().Equals(id)).FirstOrDefault();
            if (flightValid != null)
                return Notifization.Invalid("Mã IATA đã được sử dụng");
            //
            flight.Title = title;
            flight.Alias = Helper.Library.Uni2NONE(title);
            flight.Summary = model.Summary;
            flight.IATACode = model.IATACode.ToUpper();
            flight.AreaID = model.AreaID;
            flight.Enabled = model.Enabled;
            flightService.Update(flight);
            //
            return Notifization.Success(NotifizationText.UPDATE_SUCCESS);
        }
        public ViewFlight UpdateForm(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;
            //
            string sqlQuery = @"SELECT TOP (1) * FROM View_App_Flight WHERE ID = @Query";
            var viewFilght = _connection.Query<ViewFlight>(sqlQuery, new { Query = id }).FirstOrDefault();
            //
            if (viewFilght == null)
                return null;
            //
            return viewFilght;
        }
        //########################################################################tttt######################################################################################################################################################################################
        public ActionResult Delete(FlightIDModel model)
        {
            string id = model.ID;
            if (id == null)
                return Notifization.NotFound();
            //
            FlightService flightService = new FlightService(_connection);
            var banner = flightService.GetAlls(m => m.ID.Equals(id.ToLower())).FirstOrDefault();
            if (banner == null)
                return Notifization.NotFound();
            //
            flightService.Remove(banner.ID);
            return Notifization.Success(NotifizationText.DELETE_SUCCESS);
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;
            //
            var viewFilght = UpdateForm(id);
            if (viewFilght == null)
                return Notifization.NotFound(NotifizationText.NotFound);
            //
            return Notifization.DATALIST(NotifizationText.Success, data: viewFilght, role: null, paging: null);
        }
        //##############################################################################################################################################################################################################################################################   
        public List<FlightOption> DataOption()
        {
            string sqlQuery = @"SELECT * FROM View_App_Flight ORDER BY Title ASC";
            List<FlightOption> flightOptions = _connection.Query<FlightOption>(sqlQuery).ToList();
            if (flightOptions.Count == 0)
                return new List<FlightOption>();
            //
            return flightOptions;
        }
        public static string DropdownListOption(string id)
        {
            try
            {
                FlightService flightService = new FlightService();
                var dtList = flightService.DataOption();
                if (dtList.Count > 0)
                {
                    string result = string.Empty;
                    foreach (var item in dtList)
                    {
                        string select = string.Empty;
                        if (!string.IsNullOrWhiteSpace(item.IATACode) && item.IATACode.ToLower().Equals(id.ToLower()))
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

        public static string DDLNumber(string str = null)
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
