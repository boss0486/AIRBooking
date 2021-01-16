using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using Helper;
using Helper.Page;
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
    public interface IAirResBookDesigService : IEntityService<Entities.AirResBookDesig> { }
    public class AirResBookDesigService : EntityService<Entities.AirResBookDesig>, IAirResBookDesigService
    {
        public AirResBookDesigService() : base() { }
        public AirResBookDesigService(System.Data.IDbConnection db) : base(db) { }

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
            string sqlQuery = $@"SELECT * FROM App_AirResBookDesig 
            WHERE (dbo.Uni2NONE(Title) LIKE N'%'+ @Query +'%' 
            OR CodeID LIKE N'%'+ @Query +'%') {whereCondition}  ORDER BY Title ASC";
            //
            var dtList = _connection.Query<AirResBookDesigResult>(sqlQuery, new { Query = Helper.Page.Library.FormatNameToUni2NONE(query) }).ToList();
            if (dtList.Count == 0)
                return Notifization.NotFound(MessageText.NotFound);
            //
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

        public ActionResult Setting(AirResBookDesigSetting model)
        {
            if (model == null)
                return Notifization.Invalid(MessageText.Invalid);
            //
            string id = model.ID;
            int voidBookTime = model.VoidBookTime;
            //
            if (string.IsNullOrWhiteSpace(id))
                return Notifization.Invalid(MessageText.Invalid);
            //  
            id = id.ToLower();
            AirResBookDesigService AirResBookDesigService = new AirResBookDesigService(_connection);
            AirResBookDesigService airResBookDesigService = new AirResBookDesigService(_connection);
            AirResBookDesig flight = airResBookDesigService.GetAlls(m => m.ID == id).FirstOrDefault();
            if (flight == null)
                return Notifization.Invalid(MessageText.Invalid);
            // 
            if (voidBookTime < 0)
                return Notifization.Invalid("T.gian hủy đặt chỗ không hợp lệ");

            AirResBookDesig AirResBookDesig = AirResBookDesigService.GetAlls(m => m.ID == id).FirstOrDefault();
            if (AirResBookDesig == null)
                return Notifization.Invalid(MessageText.Invalid);
            // update
            AirResBookDesig.VoidBookTime = voidBookTime;
            AirResBookDesigService.Update(AirResBookDesig);
            //
            return Notifization.Success(MessageText.UpdateSuccess);
        }
        public ActionResult Create(AirResBookDesigCreateModel model)
        {
            if (model == null)
                return Notifization.Invalid(MessageText.Invalid);
            // 
            string title = model.Title;
            string summary = model.Summary;
            int enabled = model.Enabled;
            string codeId = model.CodeID;
            int voidBookTime = model.VoidBookTime;
            // 
            if (string.IsNullOrWhiteSpace(title))
                return Notifization.Invalid("Không được để trống tiêu đề");
            title = title.Trim();
            if (!Validate.TestText(title))
                return Notifization.Invalid("Tiêu đề không hợp lệ");
            if (title.Length < 2 || title.Length > 80)
                return Notifization.Invalid("Tiêu đề giới hạn 2-80 ký tự");
            //
            if (string.IsNullOrWhiteSpace(codeId))
                return Notifization.Invalid("Không được để trống hạng đặt chỗ");
            codeId = codeId.ToUpper().Trim();
            if (!Validate.TestRoll(codeId))
                return Notifization.Invalid("Hạng đặt chỗ không hợp lệ");
            if (codeId.Length != 1)
                return Notifization.Invalid("Hạng đặt chỗ giới hạn 1 ký tự");
            // summary valid               
            if (!string.IsNullOrWhiteSpace(summary))
            {
                summary = summary.Trim();
                if (!Validate.TestText(summary))
                    return Notifization.Invalid("Mô tả không hợp lệ");
                if (summary.Length < 1 || summary.Length > 120)
                    return Notifization.Invalid("Mô tả giới hạn từ 1-> 120 ký tự");
            }
            // 
            AirResBookDesigService airResBookDesigService = new AirResBookDesigService(_connection);
            AirResBookDesig airResBookDesigTitle = airResBookDesigService.GetAlls(m => m.Title.ToLower() == model.Title.ToLower()).FirstOrDefault();
            if (airResBookDesigTitle != null)
                return Notifization.Invalid("Tiêu đề đã được sử dụng");
            //
            AirResBookDesig airResBookDesigCode = airResBookDesigService.GetAlls(m => !string.IsNullOrWhiteSpace(m.CodeID) && m.CodeID.ToUpper() == codeId).FirstOrDefault();
            if (airResBookDesigCode != null)
                return Notifization.Invalid("Hạng đặt chỗ đã được sử dụng");
            //
            if (voidBookTime < 0)
                return Notifization.Invalid("T.gian hủy đặt chỗ không hợp lệ");
            // 
            airResBookDesigService.Create<string>(new AirResBookDesig()
            {
                Title = title,
                Alias = Helper.Page.Library.FormatToUni2NONE(title),
                Summary = summary,
                CodeID = codeId,
                Enabled = enabled
            });
            return Notifization.Success(MessageText.CreateSuccess);
        }
        public ActionResult Update(AirResBookDesigUpdateModel model)
        {
            if (model == null)
                return Notifization.Invalid(MessageText.Invalid);
            // 
            string id = model.ID;
            string title = model.Title;
            string summary = model.Summary;
            int enabled = model.Enabled;
            string codeId = model.CodeID;
            int voidBookTime = model.VoidBookTime;
            // 
            if (string.IsNullOrWhiteSpace(title))
                return Notifization.Invalid("Không được để trống tiêu đề");
            title = title.Trim();
            if (!Validate.TestText(title))
                return Notifization.Invalid("Tiêu đề không hợp lệ");
            if (title.Length < 2 || title.Length > 80)
                return Notifization.Invalid("Tiêu đề giới hạn 2-80 ký tự");
            //
            if (string.IsNullOrWhiteSpace(codeId))
                return Notifization.Invalid("Không được để trống hạng đặt chỗ");
            codeId = codeId.ToUpper().Trim();
            if (!Validate.TestRoll(codeId))
                return Notifization.Invalid("Hạng đặt chỗ không hợp lệ");
            if (codeId.Length != 1)
                return Notifization.Invalid("Hạng đặt chỗ giới hạn 1 ký tự");
            // summary valid               
            if (!string.IsNullOrWhiteSpace(summary))
            {
                summary = summary.Trim();
                if (!Validate.TestText(summary))
                    return Notifization.Invalid("Mô tả không hợp lệ");
                if (summary.Length < 1 || summary.Length > 120)
                    return Notifization.Invalid("Mô tả giới hạn từ 1-> 120 ký tự");
            }
            //  
            if (voidBookTime < 0)
                return Notifization.Invalid("T.gian hủy đặt chỗ không hợp lệ");
            // 
            AirResBookDesigService airResBookDesigService = new AirResBookDesigService(_connection);
            AirResBookDesig airResBookDesig = airResBookDesigService.GetAlls(m => m.ID == id).FirstOrDefault();
            if (airResBookDesig == null)
                return Notifization.Invalid(MessageText.Invalid);
            //
            AirResBookDesig airResBookDesigTitle = airResBookDesigService.GetAlls(m => !string.IsNullOrWhiteSpace(m.Title) && m.Title.ToLower() == title.ToLower() && m.ID != id).FirstOrDefault();
            if (airResBookDesigTitle != null)
                return Notifization.Invalid("Tên chuyến bay đã được sử dụng");
            //
            AirResBookDesig airResBookDesigCode = airResBookDesigService.GetAlls(m => !string.IsNullOrWhiteSpace(m.CodeID) && m.CodeID.ToUpper() == codeId && m.ID != id).FirstOrDefault();
            if (airResBookDesigCode != null)
                return Notifization.Invalid("Mã hạng đặt chỗ đã được sử dụng");
            //  
            airResBookDesig.Title = title;
            airResBookDesig.Alias = Helper.Page.Library.FormatToUni2NONE(title);
            airResBookDesig.Summary = summary;
            airResBookDesig.CodeID = codeId;
            airResBookDesig.VoidBookTime = voidBookTime;
            airResBookDesig.Enabled = model.Enabled;
            airResBookDesigService.Update(airResBookDesig);
            //
            return Notifization.Success(MessageText.UpdateSuccess);
        }
        public AirResBookDesig GetAirAportModel(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;
            //
            string sqlQuery = @"SELECT TOP (1) * FROM App_AirResBookDesig WHERE ID = @Query";
            AirResBookDesig airAirResBookDesig = _connection.Query<AirResBookDesig>(sqlQuery, new { Query = id }).FirstOrDefault();
            //
            if (airAirResBookDesig == null)
                return null;
            //
            return airAirResBookDesig;
        }
        public AirResBookDesigResult ViewAirAportModel(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;
            //
            string sqlQuery = @"SELECT TOP (1) * FROM App_AirResBookDesig WHERE ID = @Query";
            AirResBookDesigResult airAirResBookDesig = _connection.Query<AirResBookDesigResult>(sqlQuery, new { Query = id }).FirstOrDefault();
            //
            if (airAirResBookDesig == null)
                return null;
            //
            return airAirResBookDesig;
        }

        public AirResBookDesigSetting GetAirResBookDesigModel(string id)
        {

            if (string.IsNullOrWhiteSpace(id))
                return null;
            //
            string sqlQuery = @"SELECT ap.ID, ap.Title,ap.IATACode, apf.AxFee, apf.VoidBookTime, apf.VoidTicketTime FROM App_AirResBookDesig as ap
            LEFT JOIN App_AirResBookDesig as apf on apf.AirResBookDesigID = ap.ID WHERE ap.ID = @Query";
            AirResBookDesigSetting airAirResBookDesig = _connection.Query<AirResBookDesigSetting>(sqlQuery, new { Query = id }).FirstOrDefault();
            //
            if (airAirResBookDesig == null)
                return null;
            //
            return airAirResBookDesig;
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Delete(AirResBookDesigIDModel model)
        {
            string id = model.ID;
            if (id == null)
                return Notifization.NotFound();
            //
            id = id.ToLower();
            AirResBookDesigService airResBookDesigService = new AirResBookDesigService(_connection);
            var AirResBookDesig = airResBookDesigService.GetAlls(m => m.ID == id).FirstOrDefault();
            if (AirResBookDesig == null)
                return Notifization.NotFound();
            //
            airResBookDesigService.Remove(AirResBookDesig.ID);
            return Notifization.Success(MessageText.DeleteSuccess);
        }

        //##############################################################################################################################################################################################################################################################   
        public List<AirResBookDesigOption> DataOption()
        {
            string sqlQuery = @"SELECT * FROM App_AirResBookDesig ORDER BY Title ASC";
            List<AirResBookDesigOption> flightOptions = _connection.Query<AirResBookDesigOption>(sqlQuery).ToList();
            if (flightOptions.Count == 0)
                return new List<AirResBookDesigOption>();
            //
            return flightOptions;
        }
        public static string DropdownList(string id)
        {
            try
            {
                AirResBookDesigService airResBookDesigService = new AirResBookDesigService();
                var dtList = airResBookDesigService.DataOption();
                if (dtList.Count > 0)
                {
                    string result = string.Empty;
                    foreach (var item in dtList)
                    {
                        string select = string.Empty;
                        if (!string.IsNullOrWhiteSpace(item.ID) && item.ID.ToLower() == id.ToLower())
                            select = "selected";
                        //
                        result += $"<option value='{item.ID}' codeid='{item.CodeID}' {select}>{item.Title}</option>";
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
    }
}
