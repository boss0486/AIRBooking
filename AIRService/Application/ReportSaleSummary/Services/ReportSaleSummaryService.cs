using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using System;
using Dapper;
using System.Linq;
using PagedList;
using System.Web.Mvc;
using System.Collections.Generic;
using WebCore.Entities;
using Helper;
using System.Web;
using WebCore.Core;
using WebCore.Model.Entities;
using WebCore.Model.Enum;
using WebCore.ENM;
using Helper.File;

namespace WebCore.Services
{
    public interface IReportSaleSummaryService : IEntityService<ReportSaleSummary> { }
    public class ReportSaleSummaryService : EntityService<ReportSaleSummary>, IReportSaleSummaryService
    {
        public ReportSaleSummaryService() : base() { }
        public ReportSaleSummaryService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################

        // search date by date report
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
            }, dateColumn: "ReportDate");
            if (searchResult != null)
            {
                if (searchResult.Status == 1)
                    whereCondition = searchResult.Message;
                else
                    return Notifization.Invalid(searchResult.Message);
            }
            #endregion
            //

            if (!Helper.Current.UserLogin.IsCMSUser && !Helper.Current.UserLogin.IsAdminInApplication && !Helper.Current.UserLogin.IsSupplierLogged())
                return Notifization.AccessDenied(MessageText.AccessDenied);
            //
            string typeId = "";
            string sqlQuery = @"
            SELECT rps.*,rpsf.FareAmount, rpsf.TaxAmount, rpsf.TotalAmount FROM App_ReportSaleSummary as rps 
            LEFT JOIN App_ReportSaleSummarySSFop as rpsf 
            ON rpsf.ReportTransactionID =  rps.ID
            WHERE (dbo.Uni2NONE(EmployeeNumber) LIKE N'%'+ @Query +'%' OR DocumentNumber LIKE N'%'+ @Query +'%') " + whereCondition + " ORDER BY [CreatedDate]";
            var dtList = _connection.Query<ReportSaleSummaryResult>(sqlQuery, new { Query = Helper.Page.Library.FormatToUni2NONE(query), TypeID = typeId }).ToList();
            if (dtList.Count == 0)
                return Notifization.NotFound(MessageText.NotFound);
            //     
            var result = dtList.ToPagedList(page, Helper.Pagination.Paging.PAGESIZE).ToList();
            if (dtList.Count == 0 && page > 1)
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
            //
            return Notifization.Data(MessageText.Success, data: result, role: RoleActionSettingService.RoleListForUser(), paging: pagingModel);
        }
        //
        // search date by departure day
        public ActionResult DataListByDeparture(SearchModel model)
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
            SearchResult searchResult = SearchReport(new SearchModel
            {
                Query = model.Query,
                TimeExpress = model.TimeExpress,
                Status = model.Status,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Page = model.Page,
                AreaID = model.AreaID,
                TimeZoneLocal = model.TimeZoneLocal
            }, columName1: "StartDateTime", columName2: "EndDateTime");
            //
            if (searchResult != null)
            {
                if (searchResult.Status == 1)
                    whereCondition = searchResult.Message;
                else
                    return Notifization.Invalid(searchResult.Message);
            }
            #endregion
            //
            if (!Helper.Current.UserLogin.IsCMSUser && !Helper.Current.UserLogin.IsAdminInApplication && !Helper.Current.UserLogin.IsSupplierLogged())
                return Notifization.AccessDenied(MessageText.AccessDenied);
            //
            string typeId = "";
            string sqlQuery = @"
             SELECT rps.*,rtdc.SystemDateTime, rtdc.EndDateTime  FROM App_ReportSaleSummary as rps 
             INNER JOIN App_ReportTicketingDocument_Coupon as rtdc ON rtdc.DocumentNumber =  rps.DocumentNumber   
             WHERE (dbo.Uni2NONE(rps.DocumentNumber) LIKE N'%'+ @Query +'%' OR DocumentNumber LIKE N'%'+ @Query +'%') " + whereCondition + " ORDER BY [CreatedDate]";
            var dtList = _connection.Query<ReportSaleSummaryResult>(sqlQuery, new { Query = Helper.Page.Library.FormatToUni2NONE(query), TypeID = typeId }).ToList();
            if (dtList.Count == 0)
                return Notifization.NotFound(MessageText.NotFound);
            //     
            var result = dtList.ToPagedList(page, Helper.Pagination.Paging.PAGESIZE).ToList();
            if (dtList.Count == 0 && page > 1)
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
            //
            return Notifization.Data(MessageText.Success, data: result, role: RoleActionSettingService.RoleListForUser(), paging: pagingModel);
        }

        public ReportSaleSummaryModel GetReportSaleSummaryByDocumentNumber(string id)
        {

            if (string.IsNullOrWhiteSpace(id))
                return new ReportSaleSummaryModel();
            //
            id = id.ToLower();



            ReportSaleSummaryService reportSaleSummaryService = new ReportSaleSummaryService();
            ReportSaleSummary reportSaleSummary = reportSaleSummaryService.GetAlls(m => m.ID == id).FirstOrDefault();
            string docNumber = reportSaleSummary.DocumentNumber;
            //

            ReportTicketingDocumentCouponService reportTicketingDocumentCouponService = new ReportTicketingDocumentCouponService(_connection);
            ReportTicketingDocumentAmountService reportTicketingDocumentAmount = new ReportTicketingDocumentAmountService(_connection);
            ReportTicketingDocumentTaxesService reportTicketingDocumentTaxes = new ReportTicketingDocumentTaxesService(_connection);
            // reuslt
            ReportSaleSummaryModel reportSaleSummaryTicketing = new ReportSaleSummaryModel
            {
                ReportSaleSummary = reportSaleSummary,
                TicketingDocumentCoupons = reportTicketingDocumentCouponService.GetAlls(m => m.DocumentNumber == docNumber).ToList(),
                TicketingDocumentAmount = reportTicketingDocumentAmount.GetAlls(m => m.DocumentNumber == docNumber).FirstOrDefault(),
                TicketingDocumentTaxes = reportTicketingDocumentTaxes.GetAlls(m => m.DocumentNumber == docNumber).ToList()
            };

            return reportSaleSummaryTicketing;
        }

        //
        public SearchResult SearchReport(SearchModel model, string columName1 = "CreatedDate", string columName2 = null)
        {
            string whereCondition = string.Empty;
            int status = model.Status;
            int timeExpress = model.TimeExpress;
            string startDate = model.StartDate;
            string endDate = model.EndDate;
            string timeZoneLocal = model.TimeZoneLocal;
            //
            string clientTime = Helper.Time.TimeHelper.GetDateByTimeZone(timeZoneLocal);
            //
            string whereConditionSub1 = string.Empty;
            if (timeExpress != 0 && !string.IsNullOrWhiteSpace(clientTime))
            {
                // client time
                DateTime today = Convert.ToDateTime(clientTime);
                if (timeExpress == 1)
                {
                    string strDate = Helper.Time.TimeHelper.FormatToSQLDate(today);
                    string dtime = Convert.ToDateTime(strDate).ToString("yyyy-MM-dd");
                    if (string.IsNullOrWhiteSpace(columName2))
                        whereConditionSub1 = " OR cast(" + columName2 + " as Date) = cast('" + dtime + "' as Date)";
                    //
                    whereCondition = " AND (cast(" + columName1 + " as Date) = cast('" + dtime + "' as Date) " + whereConditionSub1 + ")";
                }
                // Yesterday
                if (timeExpress == 2)
                {
                    DateTime dtime = today.AddDays(-1);
                    if (string.IsNullOrWhiteSpace(columName2))
                        whereConditionSub1 = " OR cast(" + columName2 + " as Date) = cast('" + dtime + "' as Date)";
                    //
                    whereCondition = " AND (cast(" + columName1 + " as Date) = cast('" + dtime + "' as Date) " + whereConditionSub1 + ")";
                }
                // ThreeDayAgo
                if (timeExpress == 3)
                {
                    DateTime dtime = today.AddDays(-3);
                    if (string.IsNullOrWhiteSpace(columName2))
                        whereConditionSub1 = " OR cast(" + columName2 + " as Date) = cast('" + dtime + "' as Date)";
                    //
                    whereCondition = " AND (cast(" + columName1 + " as Date) = cast('" + dtime + "' as Date) " + whereConditionSub1 + ")";
                }
                // SevenDayAgo
                if (timeExpress == 4)
                {
                    DateTime dtime = today.AddDays(-7);
                    if (string.IsNullOrWhiteSpace(columName2))
                        whereConditionSub1 = " OR cast(" + columName2 + " as Date) = cast('" + dtime + "' as Date)";
                    //
                    whereCondition = " AND (cast(" + columName1 + " as Date) = cast('" + dtime + "' as Date) " + whereConditionSub1 + ")";
                }
                // OneMonthAgo
                if (timeExpress == 5)
                {
                    DateTime dtime = today.AddMonths(-1);
                    if (string.IsNullOrWhiteSpace(columName2))
                        whereConditionSub1 = " OR cast(" + columName2 + " as Date) = cast('" + dtime + "' as Date)";
                    //
                    whereCondition = " AND (cast(" + columName1 + " as Date) = cast('" + dtime + "' as Date) " + whereConditionSub1 + ")";
                }

                // ThreeMonthAgo
                if (timeExpress == 6)
                {
                    DateTime dtime = today.AddMonths(-3);
                    if (string.IsNullOrWhiteSpace(columName2))
                        whereConditionSub1 = " OR cast(" + columName2 + " as Date) = cast('" + dtime + "' as Date)";
                    //
                    whereCondition = " AND (cast(" + columName1 + " as Date) = cast('" + dtime + "' as Date) " + whereConditionSub1 + ")";
                }
                // SixMonthAgo
                if (timeExpress == 7)
                {
                    DateTime dtime = today.AddMonths(-6);
                    if (string.IsNullOrWhiteSpace(columName2))
                        whereConditionSub1 = " OR cast(" + columName2 + " as Date) = cast('" + dtime + "' as Date)";
                    //
                    whereCondition = " AND (cast(" + columName1 + " as Date) = cast('" + dtime + "' as Date) " + whereConditionSub1 + ")";
                }
                // OneYearAgo
                if (timeExpress == 8)
                {
                    DateTime dtime = today.AddYears(-1);
                    if (string.IsNullOrWhiteSpace(columName2))
                        whereConditionSub1 = " OR cast(" + columName2 + " as Date) = cast('" + dtime + "' as Date)";
                    //
                    whereCondition = " AND (cast(" + columName1 + " as Date) = cast('" + dtime + "' as Date) " + whereConditionSub1 + ")";
                }
                //
                return new SearchResult()
                {
                    Status = 1,
                    Message = whereCondition
                };
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(startDate))
                {
                    DateTime dtime = Convert.ToDateTime(startDate);
                    if (string.IsNullOrWhiteSpace(columName2))
                        whereConditionSub1 = " OR cast(" + columName2 + " as Date) = cast('" + dtime + "' as Date)";
                    //
                    whereCondition += " AND cast(" + columName1 + " as Date) >= cast('" + dtime + "' as Date) " + whereConditionSub1 + ")";
                }
                //
                if (!string.IsNullOrWhiteSpace(endDate))
                {
                    if (Convert.ToDateTime(endDate) < Convert.ToDateTime(startDate))
                        return new SearchResult()
                        {
                            Status = -1,
                            Message = "Thời gian kết thúc không hợp lệ"
                        };
                    //
                    DateTime dtime = Convert.ToDateTime(endDate);
                    if (string.IsNullOrWhiteSpace(columName2))
                        whereConditionSub1 = " OR cast(" + columName2 + " as Date) = cast('" + dtime + "' as Date)";
                    //
                    whereCondition += " AND cast(" + columName1 + " as Date) >= cast('" + dtime + "' as Date) " + whereConditionSub1 + ")";
                }
                //
                return new SearchResult()
                {
                    Status = 1,
                    Message = whereCondition
                };
            }
        }












    }
}