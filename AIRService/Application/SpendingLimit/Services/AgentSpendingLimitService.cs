using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using System;
using Dapper;
using System.Linq;
using PagedList;
using Helper;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Web;
using System.IdentityModel.Tokens.Jwt;
using WebCore.Model.Enum;
using WebCore.Core;
using WebCore.Entities;
using WebCore.Services;
using Helper.Page;
using WebCore.Model.Entities;
using WebCore.ENM;
using System.ComponentModel;
using Helper.Language;

namespace WebCore.Services
{
    public interface IAgentSpendingLimitService : IEntityService<AgentSpendingLimit> { }
    public class AgentSpendingLimitService : EntityService<AgentSpendingLimit>, IAgentSpendingLimitService
    {
        public AgentSpendingLimitService() : base() { }
        public AgentSpendingLimitService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public ActionResult DataList(SearchModel model)
        {
            #region
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
            #endregion
            //
            
            //
            string langID = Helper.Current.UserLogin.LanguageID;
            string sqlQuery = @"
            SELECT a.ID, a.CodeID, a.Title, asl.Amount, asl.Enabled,  asl.CreatedBy, asl.CreatedDate FROM App_AirAgent as a   
            LEFT JOIN App_AgentSpendingLimit as asl ON asl.AgentID = a.ID 
            WHERE a.ParentID IS NOT NULL AND a.TypeID ='agent' AND (dbo.Uni2NONE(a.Title) LIKE N'%'+ @Query +'%' OR a.CodeID LIKE N'%'+ @Query +'%') " + whereCondition + " ORDER BY a.Title, a.CodeID";
            //
            var dtList = _connection.Query<AgentSpendingLimitResult>(sqlQuery, new { Query = Helper.Page.Library.FormatNameToUni2NONE(query) }).ToList();
            if (dtList.Count == 0)
                return Notifization.NotFound(MessageText.NotFound);
            //
            var result = dtList.ToPagedList(page, Helper.Pagination.Paging.PAGESIZE).ToList();
            if (result.Count <= 0 && page > 1)
            {
                page -= 1;
                result = dtList.ToPagedList(page, Helper.Pagination.Paging.PAGESIZE).ToList();
            }
            if (result.Count <= 0)
                return Notifization.NotFound(MessageText.NotFound);

            Helper.Pagination.PagingModel pagingModel = new Helper.Pagination.PagingModel
            {
                PageSize = Helper.Pagination.Paging.PAGESIZE,
                Total = dtList.Count,
                Page = page
            };
            //
            return Notifization.Data(MessageText.Success, data: result, role: RoleActionSettingService.RoleListForUser(), paging: pagingModel);
        } 
        //##############################################################################################################################################################################################################################################################
        public ActionResult Setting(AgentSpendingLimitSettingModel model)
        {
            if (model == null)
                return Notifization.Invalid(MessageText.Invalid);
            // 
            string agentId = model.AgentID;
            double amount = model.Amount;
            int enabled = model.Enabled;
            //
            if (string.IsNullOrWhiteSpace(agentId))
                return Notifization.Invalid("Đại lý không hợp lệ");
            //
            if (amount < 0)
                return Notifization.Invalid("Hạn mức không hợp lệ");
            //
            agentId = agentId.ToLower().Trim();
            AgentSpendingLimitService agentSpendingLimitService = new AgentSpendingLimitService(_connection);
            AgentSpendingLimit agentSpendingLimit = agentSpendingLimitService.GetAlls(m => m.AgentID == agentId).FirstOrDefault();

            if (agentSpendingLimit == null)
            {
                agentSpendingLimitService.Create<string>(new AgentSpendingLimit
                {
                    AgentID = agentId,
                    Amount = amount,
                    Enabled = enabled
                });
                return Notifization.Success(MessageText.UpdateSuccess);
            }
            // update
            agentSpendingLimit.Amount = amount;
            agentSpendingLimit.Enabled = enabled;
            agentSpendingLimitService.Update(agentSpendingLimit);
            return Notifization.Success(MessageText.UpdateSuccess);
        } 
        public AgentSpendingLimitResult ViewgentSpendingLimit(string agentId)
        {
            if (agentId == null)
                return new AgentSpendingLimitResult();
            //
            string langID = Helper.Current.UserLogin.LanguageID;
            string sqlQuery = @"
            SELECT TOP 1 a.ID, a.CodeID, a.Title, asl.Amount, asl.Enabled,  asl.CreatedBy, asl.CreatedDate FROM App_AirAgent as a   
            LEFT JOIN App_AgentSpendingLimit as asl ON asl.AgentID = a.ID WHERE a.ID = @ID";
            //
            AgentSpendingLimitResult agentSpendingLimit = _connection.Query<AgentSpendingLimitResult>(sqlQuery, new { ID = agentId }).FirstOrDefault();
            if (agentSpendingLimit == null)
                return new AgentSpendingLimitResult();
            //
            return agentSpendingLimit;

        }
        //##############################################################################################################################################################################################################################################################
    }
}
