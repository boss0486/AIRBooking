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
using WebCore.ENM;
using WebCore.Entities;
using WebCore.Model.Entities;

namespace WebCore.Services
{
    public interface IAirportBookConfigService : IEntityService<Entities.AirportBookConfig> { }
    public class AirportBookConfigService : EntityService<Entities.AirportBookConfig>, IAirportBookConfigService
    {
        public AirportBookConfigService() : base() { }
        public AirportBookConfigService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public ActionResult DataList(SearchModel model)
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
            }, ectColumn: "ap.");
            if (searchResult != null)
            {
                if (searchResult.Status == 1)
                    whereCondition = searchResult.Message;
                else
                    return Notifization.Invalid(searchResult.Message);
            }
            //
            string sqlQuery = $@"SELECT ap.ID, ap.AreaInlandID, ap.Title, ap.IATACode, ap.AxFee, apf.VoidBookTime, apf.VoidTicketTime FROM App_Airport as ap
            LEFT JOIN App_AirportBookConfig apf ON apf.AirportID = ap.ID 
            WHERE (dbo.Uni2NONE(ap.Title) LIKE N'%'+ @Query +'%' OR ap.IATACode LIKE N'%'+ @Query +'%') {whereCondition} ORDER BY ap.Title ASC";
            var dtList = _connection.Query<AirportBookConfigResult>(sqlQuery, new { Query = Helper.Page.Library.FormatNameToUni2NONE(query) }).ToList();
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

        public ActionResult AirportBookConfig_Setting(AirportBookConfig_SettingModel model)
        {
            if (model == null)
                return Notifization.Invalid(MessageText.Invalid);
            //
            string airportId = model.AirportID;
            string val = model.Value;
            if (string.IsNullOrWhiteSpace(airportId) || string.IsNullOrWhiteSpace(val))
                return Notifization.Invalid(MessageText.Invalid);
            //
            AirportBookConfigService airportBookConfigService = new AirportBookConfigService(_connection);
            // axFee 
            AirportBookConfig airportBookConfig = airportBookConfigService.GetAlls(m => m.AirportID == airportId).FirstOrDefault();
            if (model.TypeID == (int)AirportBookConfigEnum.AirportBookConfig_SettingType.AxFee)
            {
                if (airportBookConfig == null)
                {
                    airportBookConfigService.Create<string>(new AirportBookConfig
                    {
                        AirportID = airportId,
                        AxFee = Convert.ToDouble(val),
                        VoidBookTime = 0,
                        VoidTicketTime = 0
                    });
                }
                airportBookConfig.AxFee = Convert.ToDouble(val);
                airportBookConfigService.Update(airportBookConfig);
                return Notifization.Success(MessageText.UpdateSuccess);
            }
            //VoidBookTime
            if (model.TypeID == (int)AirportBookConfigEnum.AirportBookConfig_SettingType.VoidBookTime)
            {
                if (airportBookConfig == null)
                {
                    airportBookConfigService.Create<string>(new AirportBookConfig
                    {
                        AirportID = airportId,
                        AxFee = 0,
                        VoidBookTime = Convert.ToInt32(val),
                        VoidTicketTime = 0
                    });
                }

                airportBookConfig.VoidBookTime = Convert.ToInt32(val);
                airportBookConfigService.Update(airportBookConfig);
                return Notifization.Success(MessageText.UpdateSuccess);
            }
            //VoidTicketTime
            if (model.TypeID == (int)AirportBookConfigEnum.AirportBookConfig_SettingType.VoidTicketTime)
            {
                if (airportBookConfig == null)
                {
                    airportBookConfigService.Create<string>(new AirportBookConfig
                    {
                        AirportID = airportId,
                        AxFee = 0,
                        VoidBookTime = 0,
                        VoidTicketTime = Convert.ToInt32(val)
                    });
                }
                airportBookConfig.VoidTicketTime = Convert.ToInt32(val);
                airportBookConfigService.Update(airportBookConfig);
                return Notifization.Success(MessageText.UpdateSuccess);
            }
            return Notifization.Invalid(MessageText.Invalid);
        }

        //##############################################################################################################################################################################################################################################################
        public AirportBookConfig GetAirportBookConfig(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;
            //
            string sqlQuery = @"SELECT TOP (1) * FROM App_AirportBookConfig WHERE ID = @ID";
            AirportBookConfig airAirFeeAgent = _connection.Query<AirportBookConfig>(sqlQuery, new { AgentID = id }).FirstOrDefault();
            //
            if (airAirFeeAgent == null)
                return null;
            //
            return airAirFeeAgent;
        }
        public AirportBookConfigResult ViewAirportBookConfig(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;
            //
            string sqlQuery = @"SELECT TOP (1) * FROM App_AirportBookConfig WHERE ID = @ID";
            AirportBookConfigResult airAirFeeAgent = _connection.Query<AirportBookConfigResult>(sqlQuery, new { ID = id }).FirstOrDefault();
            //
            if (airAirFeeAgent == null)
                return null;
            //
            return airAirFeeAgent;
        }
    }
}
