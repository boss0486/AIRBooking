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
    public interface IAirportService : IEntityService<Entities.AirAirport> { }
    public class AirportService : EntityService<Entities.AirAirport>, IAirportService
    {
        public AirportService() : base() { }
        public AirportService(System.Data.IDbConnection db) : base(db) { }

        //##############################################################################################################################################################################################################################################################
        public ActionResult DataList(AirportSearch model)
        {
            if (model == null)
                return Notifization.Invalid(MessageText.Invalid);
            //
            string query = string.Empty;
            if (string.IsNullOrWhiteSpace(model.Query))
                query = "";
            else
                query = model.Query;
            //
            int page = model.Page;

            string areaId = model.AreaID;
            string _whereCondition = string.Empty;

            if (!string.IsNullOrWhiteSpace(areaId) && areaId != "-")
                _whereCondition += " AND AreaID = @AreaID ";

            string sqlQuery = @"SELECT * FROM App_AirAirport WHERE Title LIKE N'%'+ @Query +'%'" + _whereCondition + "ORDER BY [Title] ASC";
            var dtList = _connection.Query<AirportResult>(sqlQuery, new { Query = query, AreaID = areaId }).ToList();
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

            Helper.Pagination.PagingModel pagingModel = new Helper.Pagination.PagingModel
            {
                PageSize = Helper.Pagination.Paging.PAGESIZE,
                Total = dtList.Count,
                Page = page
            };
            Helper.Model.RoleAccountModel roleAccountModel = new Helper.Model.RoleAccountModel
            {
                Create = true,
                Update = true,
                Details = true,
                Delete = true,
            };

            return Notifization.Data(MessageText.Success, data: result, role: roleAccountModel, paging: pagingModel);
        }

        //##############################################################################################################################################################################################################################################################
        public ActionResult Create(AirportCreateModel model)
        {
            AirportService flightService = new AirportService(_connection);
            var flight = flightService.GetAlls(m => m.Title.ToLower() == model.Title.ToLower()).FirstOrDefault();
            if (flight != null)
                return Notifization.Invalid("Tên chuyến bay đã được sử dụng");
            //
            flight = flightService.GetAlls(m => m.IATACode.ToLower() == model.IATACode.ToLower()).FirstOrDefault();
            if (flight != null)
                return Notifization.Invalid("Mã IATA đã được sử dụng");
            //
            flightService.Create<string>(new Entities.AirAirport()
            {
                Title = model.Title,
                Alias = Helper.Page.Library.FormatToUni2NONE(model.Title),
                Summary = model.Summary,
                IATACode = model.IATACode.ToUpper(),
                AreaID = model.AreaID,
                Enabled = model.Enabled
            });
            return Notifization.Success(MessageText.CreateSuccess);
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Update(AirportUpdateModel model)
        {
            string id = model.ID;
            if (string.IsNullOrWhiteSpace(id))
                return Notifization.NotFound(MessageText.NotFound);
            id = id.ToLower();
            AirportService flightService = new AirportService(_connection);
            var flight = flightService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.ID.ToLower().Equals(id)).FirstOrDefault();
            if (flight == null)
                return Notifization.NotFound(MessageText.NotFound);
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
            flight.Alias = Helper.Page.Library.FormatToUni2NONE(model.Title);
            flight.Summary = model.Summary;
            flight.IATACode = model.IATACode.ToUpper();
            flight.AreaID = model.AreaID;
            flight.Enabled = model.Enabled;
            flightService.Update(flight);
            //
            return Notifization.Success(MessageText.UpdateSuccess);
        }
        public AirAirport GetAirAportModel(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;
            //
            string sqlQuery = @"SELECT TOP (1) * FROM App_AirAirport WHERE ID = @Query";
            AirAirport airAirport = _connection.Query<AirAirport>(sqlQuery, new { Query = id }).FirstOrDefault();
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
            AirportService flightService = new AirportService(_connection);
            var banner = flightService.GetAlls(m => m.ID.Equals(id.ToLower())).FirstOrDefault();
            if (banner == null)
                return Notifization.NotFound();
            //
            flightService.Remove(banner.ID);
            return Notifization.Success(MessageText.DeleteSuccess);
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;
            //
            var viewFilght = GetAirAportModel(id);
            if (viewFilght == null)
                return Notifization.NotFound(MessageText.NotFound);
            //
            return Notifization.Data(MessageText.Success, data: viewFilght, role: null, paging: null);
        }
        //##############################################################################################################################################################################################################################################################   
        public List<AirportOption> DataOption()
        {
            string sqlQuery = @"SELECT * FROM App_AirAirport ORDER BY Title ASC";
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
