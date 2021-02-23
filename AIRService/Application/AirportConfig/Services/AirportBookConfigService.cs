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
    public interface IAirportConfigService : IEntityService<Entities.AirportConfig> { }
    public class AirportConfigService : EntityService<Entities.AirportConfig>, IAirportConfigService
    {
        public AirportConfigService() : base() { }
        public AirportConfigService(System.Data.IDbConnection db) : base(db) { } 
        //##############################################################################################################################################################################################################################################################
        public ActionResult ExSetting(SearchModel model)
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
            }, ectColumn: "apf.");
            if (searchResult != null)
            {
                if (searchResult.Status == 1)
                    whereCondition = searchResult.Message;
                else
                    return Notifization.Invalid(searchResult.Message);
            }
            //
            string sqlQuery = $@"SELECT ap.ID, ap.AreaInlandID, ap.Title, ap.IATACode, apf.AxFee, apf.VoidTicketTime, ISNULL(apf.Enabled,0), apf.CreatedDate FROM App_Airport as ap
            LEFT JOIN App_AirportConfig apf ON apf.AirportID = ap.ID 
            WHERE (dbo.Uni2NONE(ap.Title) LIKE N'%'+ @Query +'%' OR ap.IATACode LIKE N'%'+ @Query +'%') {whereCondition} ORDER BY ap.Title ASC";
            var dtList = _connection.Query<AirportConfigResult>(sqlQuery, new { Query = Helper.Page.Library.FormatNameToUni2NONE(query) }).ToList();

            if (dtList.Count == 0)
                return Notifization.NotFound(MessageText.NotFound + sqlQuery);
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

        public ActionResult AirportConfig_Setting(AirportConfig_SettingModel model)
        {
            if (model == null)
                return Notifization.Invalid(MessageText.Invalid);
            //
            string airportId = model.AirportID;
            string val = model.Value;
            if (string.IsNullOrWhiteSpace(airportId) || string.IsNullOrWhiteSpace(val))
                return Notifization.Invalid(MessageText.Invalid);
            //
            AirportConfigService airportConfigService = new AirportConfigService(_connection);
            // axFee 
            AirportConfig airportConfig = airportConfigService.GetAlls(m => m.AirportID == airportId).FirstOrDefault();
            if (model.TypeID == (int)AirportConfigEnum.AirportConfig_SettingType.AxFee)
            {
                if (airportConfig == null)
                {
                    airportConfigService.Create<string>(new AirportConfig
                    {
                        AirportID = airportId,
                        AxFee = Convert.ToDouble(val),
                        VoidTicketTime = 0
                    });
                }
                airportConfig.AxFee = Convert.ToDouble(val);
                airportConfigService.Update(airportConfig);
                return Notifization.Success(MessageText.UpdateSuccess);
            }
            //VoidBookTime
            if (model.TypeID == (int)AirportConfigEnum.AirportConfig_SettingType.VoidBookTime)
            {
                if (airportConfig == null)
                {
                    airportConfigService.Create<string>(new AirportConfig
                    {
                        AirportID = airportId,
                        AxFee = 0,
                        VoidTicketTime = 0
                    });
                }
                airportConfigService.Update(airportConfig);
                return Notifization.Success(MessageText.UpdateSuccess);
            }
            //VoidTicketTime
            if (model.TypeID == (int)AirportConfigEnum.AirportConfig_SettingType.VoidTicketTime)
            {
                if (airportConfig == null)
                {
                    airportConfigService.Create<string>(new AirportConfig
                    {
                        AirportID = airportId,
                        AxFee = 0,
                        VoidTicketTime = Convert.ToInt32(val)
                    });
                }
                airportConfig.VoidTicketTime = Convert.ToInt32(val);
                airportConfigService.Update(airportConfig);
                return Notifization.Success(MessageText.UpdateSuccess);
            }
            return Notifization.Invalid(MessageText.Invalid);
        }

        //##############################################################################################################################################################################################################################################################
        public AirportConfig GetAirportConfig(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;
            //
            string sqlQuery = @"SELECT TOP (1) * FROM App_AirportConfig WHERE ID = @ID";
            AirportConfig airAirFeeAgent = _connection.Query<AirportConfig>(sqlQuery, new { AgentID = id }).FirstOrDefault();
            //
            if (airAirFeeAgent == null)
                return null;
            //
            return airAirFeeAgent;
        }
        public AirportConfigResult ViewAirportConfig(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;
            //
            string sqlQuery = @"SELECT TOP (1) * FROM App_AirportConfig WHERE ID = @ID";
            AirportConfigResult airAirFeeAgent = _connection.Query<AirportConfigResult>(sqlQuery, new { ID = id }).FirstOrDefault();
            //
            if (airAirFeeAgent == null)
                return null;
            //
            return airAirFeeAgent;
        }
    }
}
