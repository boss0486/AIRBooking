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
using System.Reflection;

namespace WebCore.Services
{
    public interface IMenuActionService : IEntityService<MenuAction> { }
    public class MenuActionService : EntityService<MenuAction>, IMenuActionService
    {
        public MenuActionService() : base() { }
        public MenuActionService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public static string DropdownList(string id, string categoryId)
        {
            try
            {
                string result = string.Empty;
                using (var service = new MenuActionService())
                {
                    var dtList = service.DataOption(categoryId);
                    if (string.IsNullOrWhiteSpace(categoryId))
                    {
                        dtList = dtList.Where(m => m.CategoryID == categoryId).ToList();
                    }

                    if (dtList.Count > 0)
                    {
                        int cnt = 0;
                        foreach (var item in dtList)
                        {
                            string select = string.Empty;
                            if (!string.IsNullOrWhiteSpace(id) && !string.IsNullOrWhiteSpace(item.ID) && item.ID.ToLower().Equals(id.ToLower()))
                                select = "selected";
                            //
                            result += "<option value='" + item.ID + "'" + select + ">" + item.KeyID + "</option>";
                            cnt++;
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

        public List<MvcActionOption> DataOption(string categoryId)
        {
            try
            {
                string sqlQuery = @"SELECT * FROM View_MenuAction WHERE CategoryID = @categoryId ORDER BY Title ASC";
                return _connection.Query<MvcActionOption>(sqlQuery, new { CategoryID = categoryId }).ToList();
            }
            catch
            {
                return new List<MvcActionOption>();
            }
        }

        //#######################################################################################################################################################################################
        public List<MenuAction> GetActionPageByCategoryID(ActionByCategoryModel model)
        {
            try
            {
                string id = model.ID;
                string categoryId = model.CategoryID;
                string result = string.Empty;
                using (var service = new MenuActionService())
                {
                    string sqlQuery = @"SELECT * FROM View_MenuAction WHERE CategoryID = @CategoryID AND APIAction = 1  ORDER BY Title ASC";
                    var dtList = service.Query<MenuAction>(sqlQuery, new { CategoryID = categoryId }).ToList();
                    if (dtList.Count == 0)
                        return new List<MenuAction>();
                    //
                    return dtList;
                }
            }
            catch
            {
                return new List<MenuAction>();
            }
        }


    }
}