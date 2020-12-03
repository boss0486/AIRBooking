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
    public interface IAirAgentFeeService : IEntityService<Entities.AirAgentFee> { }
    public class AirAgentFeeService : EntityService<Entities.AirAgentFee>, IAirAgentFeeService
    {
        public AirAgentFeeService() : base() { }
        public AirAgentFeeService(System.Data.IDbConnection db) : base(db) { }
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
            // query
            string sqlQuery = @"SELECT af.*, al.Title, al.CodeID, cs.Title as 'AgentName', cs.CodeID as 'AgentCode' FROM App_AirAgentFee as af 
            LEFT JOIN App_Airline as al  ON  al.ID = af.AirlineID 
            LEFT JOIN App_AirAgent as cs  ON  cs.ID = af.AgentID 
            WHERE (dbo.Uni2NONE(al.Title) LIKE N'%'+ @Query +'%' OR al.CodeID LIKE N'%'+ @Query +'%' OR dbo.Uni2NONE(cs.Title) LIKE N'%'+ @Query +'%' OR cs.CodeID LIKE N'%'+ @Query +'%') ORDER BY af.AgentID, al.Title ASC";
            var dtList = _connection.Query<AirAgentFeeResult>(sqlQuery, new { Query = Helper.Page.Library.FormatNameToUni2NONE(query) }).ToList();
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


        public ActionResult GetFeeConfig(AirAgentFee_RequestModel model)
        {
            if (model == null)
                return Notifization.Invalid(MessageText.Invalid);
            //
            int itineraryId = model.ItineraryID;
            string nationalId = model.NationalID;
            string agentId = model.AgentID;
            //
            if (string.IsNullOrWhiteSpace(agentId))
                return Notifization.Invalid("Vui lòng chọn đại lý");
            //
            string whereCondition = string.Empty;
            if (itineraryId == (int)ItineraryEnum.ItineraryType.Inland)
            {
                whereCondition = " AND NationalID IS NULL AND ItineraryID = " + itineraryId;
            }
            if (itineraryId == (int)ItineraryEnum.ItineraryType.International)
            {
                if (string.IsNullOrWhiteSpace(nationalId))
                    return Notifization.Invalid(MessageText.Invalid);
                //
                whereCondition = " AND NationalID = @NationalID AND ItineraryID = " + itineraryId;
            }
            //
            string sqlQuery = @"SELECT ItineraryID, AirlineID, FeeAmount FROM App_AirAgentFee WHERE AgentID = @AgentID AND Enabled = 1" + whereCondition;
            List<AirAgentFee_ResultModel> airAgentFees = _connection.Query<AirAgentFee_ResultModel>(sqlQuery, new
            {
                NationalID = nationalId,
                AgentID = agentId
            }).ToList();
            if (airAgentFees.Count == 0)
                return Notifization.Data(MessageText.Success + "nationalId:" + nationalId + "AgentID:" + agentId, null);
            // 
            return Notifization.Data(MessageText.Success, airAgentFees);
        }

        public ActionResult ConfigFee(AirAgentFeeConfigModel model)
        {
            if (model == null)
                return Notifization.Invalid(MessageText.Invalid + "1");
            //
            string agentId = model.AgentID;
            if (string.IsNullOrWhiteSpace(agentId))
                return Notifization.Invalid("Vui lòng chọn đại lý");
            //
            List<AirAgentFee_AirlineFee> airlineFees = model.AirlineFees;
            if (string.IsNullOrWhiteSpace(agentId))
                return Notifization.Invalid("Đại lý không hợp lệ");
            //
            if (airlineFees.Count == 0)
                return Notifization.Invalid(MessageText.Invalid);
            //
            foreach (var item in airlineFees)
            {
                if (item.Amount <= 0 || item.Amount >= 100000000)
                    return Notifization.Invalid("Phí giới hạn từ [0-100 000 000] đ");
                //
            }
            // 
            AirAgentService customerService = new AirAgentService(_connection);
            AirAgentFeeService airFeeAgentService = new AirAgentFeeService(_connection);
            AirAgent customer = customerService.GetAlls(m => m.ID == agentId).FirstOrDefault();
            if (customer == null)
                return Notifization.Invalid("Đại lý không hợp lệ");
            //

            //  Inland
            if (model.ItineraryType == (int)ItineraryEnum.ItineraryType.Inland)
            {
                foreach (var item in airlineFees)
                {
                    AirAgentFee airFeeAgent = airFeeAgentService.GetAlls(m => m.AgentID == agentId && m.AirlineID == item.AirlineID && m.ItineraryID == model.ItineraryType).FirstOrDefault();
                    if (airFeeAgent == null)
                    {
                        double feeAmount = item.Amount;
                        // create  
                        airFeeAgentService.Create<string>(new AirAgentFee
                        {
                            AgentID = agentId,
                            ItineraryID = (int)ItineraryEnum.ItineraryType.Inland,
                            AirlineID = item.AirlineID,
                            FeeAmount = feeAmount,
                            Enabled = (int)Model.Enum.ModelEnum.Enabled.ENABLED
                        });
                    }
                    else
                    {
                        // update
                        airFeeAgent.FeeAmount = item.Amount;
                        airFeeAgentService.Update(airFeeAgent);
                    }
                }
                return Notifization.Success(MessageText.UpdateSuccess);
            }
            // International
            if (model.ItineraryType == (int)ItineraryEnum.ItineraryType.International)
            {
                string nationalId = model.NationalID;
                if (string.IsNullOrWhiteSpace(nationalId))
                    return Notifization.Invalid("Vui lòng chọn quốc gia");
                //
                nationalId = nationalId.Trim().ToLower();
                NationalService nationalService = new NationalService(_connection);
                National national = nationalService.GetAlls(m => m.ID == nationalId).FirstOrDefault();
                if (national == null)
                    return Notifization.Invalid("Quốc gia không hợp lệ");
                // 
                foreach (var item in airlineFees)
                {
                    AirAgentFee airFeeAgent = airFeeAgentService.GetAlls(m => m.AgentID == agentId && m.AirlineID == item.AirlineID && m.ItineraryID == model.ItineraryType && m.NationalID == nationalId).FirstOrDefault();
                    if (airFeeAgent == null)
                    {
                        double feeAmount = item.Amount;
                        // create  
                        airFeeAgentService.Create<string>(new AirAgentFee
                        {
                            NationalID = nationalId,
                            AgentID = agentId,
                            ItineraryID = (int)ItineraryEnum.ItineraryType.International,
                            AirlineID = item.AirlineID,
                            FeeAmount = feeAmount,
                            Enabled = (int)Model.Enum.ModelEnum.Enabled.ENABLED
                        });
                    }
                    else
                    {
                        // update
                        airFeeAgent.FeeAmount = item.Amount;
                        airFeeAgentService.Update(airFeeAgent);
                    }
                }







                return Notifization.Success(MessageText.UpdateSuccess);
            }
            return Notifization.Invalid(MessageText.Invalid);
        }

        public ActionResult GetAgentFee(AirAgentFee_AgentModel model)
        {
            if (model == null)
                return Notifization.Invalid(MessageText.Invalid);
            //
            string agentId = model.AgentID;
            if (string.IsNullOrWhiteSpace(agentId))
                return Notifization.Invalid(MessageText.Invalid);
            //
            string sqlQuery = @"SELECT TOP (1) * FROM App_AirAgentFee WHERE AgentID = @AgentID";
            AirAgentFeeResult airAirFeeAgent = _connection.Query<AirAgentFeeResult>(sqlQuery, new { AgentID = agentId.ToLower() }).FirstOrDefault();
            //
            if (airAirFeeAgent == null)
                return Notifization.Data("OK", null);
            //
            return Notifization.Data("OK", airAirFeeAgent);
        }
        //##############################################################################################################################################################################################################################################################
        public AirAgentFee GetAgentFee(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;
            //
            string sqlQuery = @"SELECT TOP (1) * FROM App_AirAgentFee WHERE AgentID = @AgentID";
            AirAgentFee airAirFeeAgent = _connection.Query<AirAgentFee>(sqlQuery, new { AgentID = id }).FirstOrDefault();
            //
            if (airAirFeeAgent == null)
                return null;
            //
            return airAirFeeAgent;
        }
        public AirAgentFeeResult ViewAgentFee(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;
            //
            string sqlQuery = @"SELECT TOP (1) * FROM App_AirAgentFee WHERE ID = @ID";
            AirAgentFeeResult airAirFeeAgent = _connection.Query<AirAgentFeeResult>(sqlQuery, new { ID = id }).FirstOrDefault();
            //
            if (airAirFeeAgent == null)
                return null;
            //
            return airAirFeeAgent;
        }
    }
}
