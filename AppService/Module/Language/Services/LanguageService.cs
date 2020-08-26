using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using Helper;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebCore.Entities;
using WebCore.Model.Enum;

namespace WebCore.Services
{
    public interface ILanguageService : IEntityService<Language> { }
    public class LanguageService : EntityService<Language>, ILanguageService
    {
        public LanguageService() : base() { }
        public LanguageService(System.Data.IDbConnection db) : base(db) { }

        //##############################################################################################################################################################################################################################################################
        public ActionResult Datalist(string strQuery, int page, string langID)
        {
            string query = string.Empty;
            if (string.IsNullOrEmpty(strQuery))
                query = "";
            else
                query = strQuery;

            if (string.IsNullOrEmpty(langID))
                langID = Default.LanguageID;

            string sqlQuery = @"SELECT 
                                     [ID]
                                    ,[Title]
                                    ,[LanguageID]
                                    ,[LangID]
                                    ,[Enabled]
                                    ,[CreatedDate]
                                     FROM Language 
                                     WHERE (Title LIKE N'%'+ dbo.Uni2NONE(@Query) +'%')  
                                     AND LangID = @LangID ORDER BY Title ASC";
            var dtList = _connection.Query<LanguageModel>(sqlQuery, new { Query = query, @LangID = langID }).ToList();
            if (dtList.Count <= 0)
                return Notifization.NotFound(NotifizationText.NotFound);

            List<RsLanguage> resultData = new List<RsLanguage>();
            foreach (var item in dtList)
            {
                string isBlock = string.Empty;
                string isEnable = string.Empty;
                if (item.Enabled == (int)ModelEnum.Enabled.ENABLED)
                    isEnable = "Actived";
                else
                    isEnable = "Not active";

                resultData.Add(new RsLanguage(item.ID, item.Title, item.LanguageID, item.LangID, isEnable, Convert.ToString(item.CreatedDate)));
            }
            var result = resultData.ToPagedList(page, Library.Paging.PAGESIZE).ToList();
            if (result.Count <= 0 && page > 1)
            {
                page -= 1;
                result = resultData.ToPagedList(page, Library.Paging.PAGESIZE).ToList();
            }
            if (result.Count <= 0)
                return Notifization.NotFound(NotifizationText.NotFound);
            Library.PagingModel pagingModel = new Library.PagingModel
            {
                PageSize = Library.Paging.PAGESIZE,
                Total = dtList.Count,
                Page = page
            };
            return Notifization.DATALIST(NotifizationText.Success, data: result, paging: pagingModel);
        }
        //##############################################################################################################################################################################################################################################################
        public static string DdlLanguage(string langID, string selected)
        {
            try
            {
                string result = string.Empty;
                using (LanguageService languageService = new LanguageService())
                {
                    List<LanguageOption> dtList = languageService.DataOption(langID);
                    if (dtList.Count > 0)
                    {
                        foreach (var item in dtList)
                        {
                            string select = string.Empty;
                            if (item.LanguageID.ToLower().Equals(selected.ToLower()))
                                select = "selected";
                            result += "<option value='" + item.LanguageID + "'" + select + ">" + item.Title + "</option>";
                        }
                    }
                    return result;
                }
            }
            catch
            {
                return string.Empty;
            }
        }
        //##############################################################################################################################################################################################################################################################
        public List<LanguageOption> DataOption(string langID)
        {
            try
            {
                List<LanguageOption> optionList = new List<LanguageOption>();
                string sqlQuery = @"SELECT [LanguageID],[Title] FROM Language ORDER BY Title ASC";
                optionList = _connection.Query<LanguageOption>(sqlQuery, new { LangID = langID }).ToList();
                return optionList;
            }
            catch
            {
                return new List<LanguageOption>();
            }
        }
    }
    //##############################################################################################################################################################################################################################################################
    public class Resource
    {
        public class Static
        {
        }
        public class Label
        {
            public static string BtnCreate { get; set; } = "Thêm mới";
            public static string BtnUpdate { get; set; } = "Cập nhật";
            public static string BtnDelete { get; set; } = "Xóa";
            public static string BtnSearch { get; set; } = "Tìm kiếm";
            public static string BtnClose { get; set; } = "Đóng";
            public static string BtnOpen { get; set; } = "Mở";
            public static string BtnReset { get; set; } = "Làm mới";
            public static string BtnCancel { get; set; } = "Hủy bỏ";
            public static string HtmlText { get; set; } = "Nội dung";
            public static string Summary { get; set; } = "Mô tả";
            public static string HtmlNote { get; set; } = "Mô tả (html)";
            public static string Active { get; set; } = "Kích hoạt";
            public static string NoActive { get; set; } = "Không kích hoạt";
            public static string IDNo { get; set; } = "IDNo.";
            public static string DataList { get; set; } = "Danh sách";
            public static string Display { get; set; } = "Hiển thị";
            public static string Hide { get; set; } = "Ẩn";
            public static string Category { get; set; } = "Danh mục";
            public static string Status { get; set; } = "Trạng thái";
            public static string State { get; set; } = "Tình trạng";
            public static string Option { get; set; } = "Lựa chọn";
            public static string Photo { get; set; } = "Hình ảnh";
            public static string Title { get; set; } = "Tiêu đề";
            public static string Alias { get; set; } = "Đường dẫn";
            public static string Search { get; set; } = "Tìm kiếm";
        }
        public class Field
        {

        }
        public class Text
        {

        }
    }
    //##############################################################################################################################################################################################################################################################
}