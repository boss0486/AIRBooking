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
    public interface IAirFeeAgentService : IEntityService<Entities.AirAgentFee> { }
    public class AirFeeAgentService : EntityService<Entities.AirAgentFee>, IAirFeeAgentService
    {
        public AirFeeAgentService() : base() { }
        public AirFeeAgentService(System.Data.IDbConnection db) : base(db) { }

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
            string sqlQuery = @"SELECT * FROM App_AirFeeAgent WHERE Title LIKE N'%'+ @Query +'%'" + whereCondition + " ORDER BY [Title] ASC";
            var dtList = _connection.Query<AirFeeAgentResult>(sqlQuery, new { Query = Helper.Page.Library.FormatToUni2NONE(query), AreaID = areaId }).ToList();
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
        //##############################################################################################################################################################################################################################################################
        public ActionResult AgentFeeConfig(AirFeeAgentConfigModel model)
        {
            if (model == null)
                return Notifization.Invalid(MessageText.Invalid + "1");
            //
            string agentId = model.AgentID;
            float amount = model.Amount;
            if (amount < 0 && amount > 100000000)
                return Notifization.Invalid("Số tiền giới hạn từ 0 - 100 000 000 đ");
            //
            if (string.IsNullOrWhiteSpace(agentId))
                return Notifization.Invalid(MessageText.Invalid + "2");
            //
            CustomerService customerService = new CustomerService(_connection);
            Customer customer = customerService.GetAlls(m => m.ID == agentId.ToLower()).FirstOrDefault();
            if (customer == null)
                return Notifization.Invalid(MessageText.Invalid + "3");
            //
            AirFeeAgentService airFeeAgentService = new AirFeeAgentService(_connection);
            var airFeeAgent = airFeeAgentService.GetAlls(m => m.AgentID == agentId).FirstOrDefault();
            if (airFeeAgent == null)
            {
                // create
                airFeeAgentService.Create<string>(new AirAgentFee
                {
                    Title = customer.CodeID,
                    AgentID = agentId,
                    Amount = amount,
                    Enabled = 1
                });
                return Notifization.Success(MessageText.CreateSuccess);
            }
            // update
            airFeeAgent.Amount = amount;
            airFeeAgentService.Update(airFeeAgent);
            return Notifization.Success(MessageText.UpdateSuccess);
        }
        //##############################################################################################################################################################################################################################################################
        public AirFeeAgentResult GetAirFeeAgentModel(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;
            //
            string sqlQuery = @"SELECT TOP (1) * FROM App_AirFeeAgent WHERE ID = @Query";
            AirFeeAgentResult airAirFeeAgent = _connection.Query<AirFeeAgentResult>(sqlQuery, new { Query = id }).FirstOrDefault();
            //
            if (airAirFeeAgent == null)
                return null;
            //
            return airAirFeeAgent;
        }
    }
}
