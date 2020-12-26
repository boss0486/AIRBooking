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
    public interface IAgentSpendingLimitPaymentService : IEntityService<AgentSpendingLimitPayment> { }
    public class AgentSpendingLimitPaymentService : EntityService<AgentSpendingLimitPayment>, IAgentSpendingLimitPaymentService
    {
        public AgentSpendingLimitPaymentService() : base() { }
        public AgentSpendingLimitPaymentService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public ActionResult ShowPay(SearchModel model)
        {
            if (model == null)
                return Notifization.Invalid(MessageText.Invalid);
            //
            int page = model.Page;
            string query = model.Query;
            if (string.IsNullOrWhiteSpace(query))
                query = "";
            //
            //string whereCondition = string.Empty;
            ////
            ///

            int timeExpress = model.TimeExpress;
            string strTimeStart = model.StartDate;
            string strTimeEnd = model.EndDate;
            string clientTime = model.TimeZoneLocal;
            //

            // today
            DateTime today = Helper.TimeData.TimeHelper.UtcDateTime;
            today = new DateTime(today.Year, today.Month, 1);

            DateTime dateTimeStart = today.AddMonths(-6);
            DateTime dateTimeEnd = today;
            //DateTime today = Convert.ToDateTime(clientTime);
            if (timeExpress != 0 && !string.IsNullOrWhiteSpace(clientTime))
            {
                // client time 
                if (timeExpress == 1)
                {
                    dateTimeStart = today;
                }
                // Yesterday
                if (timeExpress == 2)
                {
                    dateTimeStart = today.AddMonths(-1);
                    dateTimeEnd = today.AddMonths(-1);
                }
                // ThreeDayAgo
                if (timeExpress == 3)
                {
                    // lay ngay 01 / month 
                    dateTimeStart = today.AddMonths(-2);
                }
                // SevenDayAgo
                if (timeExpress == 4)
                {
                    dateTimeStart = today.AddMonths(-5);
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(strTimeStart))
                {
                    if (dateTimeStart < today.AddMonths(-6))
                        return Notifization.NotFound("T.G bắt đầu giới hạn trong 6 tháng");
                    //
                    dateTimeStart = Helper.TimeData.TimeFormat.FormatToServerDate(strTimeStart);
                }
                if (!string.IsNullOrWhiteSpace(strTimeEnd))
                {
                    if (dateTimeStart > dateTimeEnd)
                        return Notifization.NotFound("Thời gian kết thúc không hợp lệ");
                    //
                    if (dateTimeEnd > today)
                        return Notifization.NotFound("Thời gian kết thúc không hợp lệ");
                    //
                    dateTimeEnd = Helper.TimeData.TimeFormat.FormatToServerDate(strTimeEnd);
                }
            }
            //if (searchResult != null)
            //{
            //    if (searchResult.Status == 1)
            //        whereCondition = searchResult.Message;
            //    else
            //        return Notifization.Invalid(searchResult.Message);
            //}
            //#endregion
            ////
            //if (Helper.Current.UserLogin.IsSupplierLogged())
            //{

            //}
            //
            // tạo danh sách thanh toán
            //DateTime start = Helper.TimeData.TimeHelper.UtcDateTime.AddMonths(-6);
            //DateTime end = Helper.TimeData.TimeHelper.UtcDateTime;
            List<DateTime> dateTimes = Enumerable.Range(0, 1 + dateTimeEnd.Subtract(dateTimeStart).Days).Select(offset => dateTimeStart.AddDays(offset)).Where(m => m.Day == 1).ToList();
            //
            List<PayData> dtList = new List<PayData>();
            AirAgentService airAgentService = new AirAgentService(_connection);

            string sqlQuery = @"
            SELECT a.* FROM App_AirAgent as a     
            WHERE a.ParentID IS NOT NULL AND a.TypeID ='agent' AND (dbo.Uni2NONE(a.Title) LIKE N'%'+ @Query +'%' OR a.CodeID LIKE N'%'+ @Query +'%') ORDER BY a.Title, a.CodeID";
            //
            List<AirAgent> airAgents = _connection.Query<AirAgent>(sqlQuery, new { Query = Helper.Page.Library.FormatNameToUni2NONE(query) }).ToList();
            if (airAgents.Count == 0)
                return Notifization.NotFound(MessageText.NotFound);
            //
            AgentSpendingLimitPaymentService agentSpendingLimitPaymentService = new AgentSpendingLimitPaymentService(_connection);
            List<AgentSpendingLimitPayment> agentSpendingLimitPayments = agentSpendingLimitPaymentService.GetAlls().ToList();
            foreach (var time in dateTimes)
            {
                foreach (var agent in airAgents)
                {
                    bool _state = false;
                    string _payDate = string.Empty;
                    AgentSpendingLimitPayment agentSpendingLimitPayment = agentSpendingLimitPayments.Where(m => m.AgentID == agent.ID && m.Year == time.Year && m.Month == time.Month).FirstOrDefault();
                    if (agentSpendingLimitPayment != null)
                    {
                        _state = agentSpendingLimitPayment.State; // true
                        _payDate = Helper.TimeData.TimeFormat.FormatToViewDateTime(agentSpendingLimitPayment.PaymentDate, LanguagePage.GetLanguageCode);
                    }
                    //
                    dtList.Add(new PayData()
                    {
                        Month = time.Month,
                        Year = time.Year,
                        AgentID = agent.ID,
                        CodeID = agent.CodeID,
                        Title = agent.Title,
                        State = _state,
                        PaymentDate = _payDate
                    });
                }

            }
            //
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

        public ActionResult ExPayment(AgentSpendingLimitPaymentSettingModel model)
        {
            if (model == null)
                return Notifization.Invalid(MessageText.Invalid);
            // 
            string agentId = model.AgentID;
            int year = model.Year;
            int month = model.Month;
            // 
            if (string.IsNullOrWhiteSpace(agentId))
                return Notifization.Invalid("Đại lý không hợp lệ");
            //
            AirAgentService airAgentService = new AirAgentService(_connection);
            AirAgent airAgent = airAgentService.GetAlls(m => m.ID == agentId).FirstOrDefault();
            DateTime dateTime = Helper.TimeData.TimeHelper.UtcDateTime;

            if (year > dateTime.Year)
                return Notifization.Invalid(MessageText.Invalid);
            //
            if (month < 1 || month > 12)
                return Notifization.Invalid(MessageText.Invalid);
            // 

            AgentSpendingLimitPaymentService agentSpendingLimitPaymentService = new AgentSpendingLimitPaymentService(_connection);
            AgentSpendingLimitPayment agentSpendingLimitPayment = agentSpendingLimitPaymentService.GetAlls(m => m.AgentID == agentId && m.Year == year && m.Month == month).FirstOrDefault();
            if (agentSpendingLimitPayment != null)
                return Notifization.Invalid("Đại lý đã thanh toán");
            // 
            agentSpendingLimitPaymentService.Create<string>(new AgentSpendingLimitPayment
            {
                AgentID = agentId,
                Year = year,
                Month = month,
                State = true,
                PaymentDate = dateTime
            });
            return Notifization.Success(MessageText.UpdateSuccess);
        }

        //##############################################################################################################################################################################################################################################################
    }
}
