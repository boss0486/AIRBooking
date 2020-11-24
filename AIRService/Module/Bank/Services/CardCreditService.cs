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
using WebCore.Model.Enum;
using WebCore.Core;
using WebCore.Entities;
using WebCore.Services;
using Helper.Page;
using WebCore.Model.Entities;

namespace WebCore.Services
{
    public interface ICardCreditService : IEntityService<CardCredit> { }
    public class CardCreditService : EntityService<CardCredit>, ICardCreditService
    {
        public CardCreditService() : base() { }
        public CardCreditService(System.Data.IDbConnection db) : base(db) { }
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
            string langID = Helper.Current.UserLogin.LanguageID;
            string sqlQuery = @"SELECT * FROM App_CardCredit WHERE dbo.Uni2NONE(Title) LIKE N'%'+ @Query +'%' " + whereCondition + " ORDER BY [CreatedDate]";
            var dtList = _connection.Query<CardCreditResult>(sqlQuery, new { Query = Helper.Page.Library.FormatNameToUni2NONE(query) }).ToList();
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
        public ActionResult Create(CardCreditCreateModel model)
        {
            _connection.Open();
            using (var _transaction = _connection.BeginTransaction())
            {
                try
                {
                    if (model == null)
                        return Notifization.Invalid();
                    //
                    string title = model.Title;
                    string summary = model.Summary;
                    if (string.IsNullOrWhiteSpace(title))
                        return Notifization.Invalid("Không được để trống tiêu đề");
                    //
                    title = title.Trim();
                    if (!Validate.TestText(title))
                        return Notifization.Invalid("Tiêu đề không hợp lệ");
                    if (title.Length < 2 || title.Length > 80)
                        return Notifization.Invalid("Tiêu đề giới hạn 2-80 ký tự");
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
                    CardCreditService cardCreditService = new CardCreditService(_connection);
                    var cardCredit = cardCreditService.GetAlls(m => !string.IsNullOrWhiteSpace(title) && m.Title.ToLower() == title.ToLower(), transaction: _transaction).FirstOrDefault();
                    if (cardCredit != null)
                        return Notifization.Invalid("Tên thẻ tín dụng đã được sử dụng");
                    var Id = cardCreditService.Create<string>(new CardCredit()
                    {
                        Title = model.Title,
                        Alias = Helper.Page.Library.FormatToUni2NONE(model.Title),
                        Summary = model.Summary,
                        LanguageID = Helper.Current.UserLogin.LanguageID,
                        Enabled = model.Enabled,
                    }, transaction: _transaction);
                    string temp = string.Empty;
                    //sort
                    _transaction.Commit();
                    return Notifization.Success(MessageText.CreateSuccess);
                }
                catch
                {
                    _transaction.Rollback();
                    return Notifization.NotService;
                }
            }
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Update(CardCreditUpdateModel model)
        {
            _connection.Open();
            using (var _transaction = _connection.BeginTransaction())
            {
                try
                {
                    if (model == null)
                        return Notifization.Invalid();

                    string title = model.Title;
                    string summary = model.Summary;
                    if (string.IsNullOrWhiteSpace(title))
                        return Notifization.Invalid("Không được để trống tiêu đề");
                    title = title.Trim();
                    if (!Validate.TestText(title))
                        return Notifization.Invalid("Tiêu đề không hợp lệ");
                    if (title.Length < 2 || title.Length > 80)
                        return Notifization.Invalid("Tiêu đề giới hạn 2-80 ký tự");
                    // summary valid               
                    if (!string.IsNullOrWhiteSpace(summary))
                    {
                        summary = summary.Trim();
                        if (!Validate.TestText(summary))
                            return Notifization.Invalid("Mô tả không hợp lệ");
                        if (summary.Length < 1 || summary.Length > 120)
                            return Notifization.Invalid("Mô tả giới hạn từ 1-> 120 ký tự");
                    }
                    CardCreditService cardCreditService = new CardCreditService(_connection);
                    string id = model.ID.ToLower();
                    var cardCredit = cardCreditService.GetAlls(m => m.ID == id, transaction: _transaction).FirstOrDefault();
                    if (cardCredit == null)
                        return Notifization.NotFound(MessageText.NotFound);
                    //
                    cardCredit = cardCreditService.GetAlls(m => !string.IsNullOrWhiteSpace(m.Title) && m.Title.ToLower() == title.ToLower() && m.ID != id, transaction: _transaction).FirstOrDefault();
                    if (cardCredit != null)
                        return Notifization.Invalid("Tên thẻ tín dụng đã được sử dụng");
                    // update user information
                    cardCredit.Title = title;
                    cardCredit.Alias = Helper.Page.Library.FormatToUni2NONE(title);
                    cardCredit.Summary = model.Summary;
                    cardCredit.Enabled = model.Enabled;
                    cardCreditService.Update(cardCredit, transaction: _transaction);
                    _transaction.Commit();
                    return Notifization.Success(MessageText.UpdateSuccess);
                }
                catch
                {
                    _transaction.Rollback();
                    return Notifization.NotService;
                }
            }
        }
        public CardCreditResult GetCreditByID(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return null;
                string query = string.Empty;
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT TOP (1) * FROM App_CardCredit WHERE ID = @Query";
                return _connection.Query<CardCreditResult>(sqlQuery, new { Query = id }).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }
        //########################################################################tttt######################################################################################################################################################################################
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return Notifization.NotFound();
            //
            id = id.ToLower();
            using (var _connectDb = DbConnect.Connection.CMS)
            {
                _connectDb.Open();
                using (var _transaction = _connectDb.BeginTransaction())
                {
                    try
                    {
                        CardCreditService cardCreditService = new CardCreditService(_connectDb);
                        var cardCredit = cardCreditService.GetAlls(m => m.ID == id, transaction: _transaction).FirstOrDefault();
                        if (cardCredit == null)
                            return Notifization.NotFound();
                        cardCreditService.Remove(cardCredit.ID, transaction: _transaction);
                        // remover seo
                        _transaction.Commit();
                        return Notifization.Success(MessageText.DeleteSuccess);
                    }
                    catch
                    {
                        _transaction.Rollback();
                        return Notifization.NotService;
                    }
                }
            }
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Detail(string Id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Id))
                    return Notifization.NotFound(MessageText.Invalid);
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT * FROM App_CardCredit WHERE ID = @ID";
                var item = _connection.Query<CardCreditResult>(sqlQuery, new { ID = Id }).FirstOrDefault();
                if (item == null)
                    return Notifization.NotFound(MessageText.NotFound);
                //
                return Notifization.Data(MessageText.Success, data: item);
            }
            catch
            {
                return Notifization.NotService;
            }
        }
        //##############################################################################################################################################################################################################################################################
        public static string DropdownList(string id)
        {
            try
            {
                string result = string.Empty;
                using (var CardCreditService = new CardCreditService())
                {
                    var dtList = CardCreditService.DataOption(null);
                    if (dtList.Count > 0)
                    {
                        foreach (var item in dtList)
                        {
                            string select = string.Empty;
                            if (!string.IsNullOrWhiteSpace(id) && item.ID == id.ToLower())
                                select = "selected";
                            result += "<option value='" + item.ID + "'" + select + ">" + item.Title + "</option>";
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
        public List<CardCreditOption> DataOption(string languageId)
        {
            try
            {
                string sqlQuery = @"SELECT * FROM App_CardCredit WHERE Enabled = 1 ORDER BY Title ASC";
                return _connection.Query<CardCreditOption>(sqlQuery, new { LangID = languageId }).ToList();
            }
            catch
            {
                return new List<CardCreditOption>();
            }
        }
        //##############################################################################################################################################################################################################################################################
    }
}
