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

namespace WebCore.Services
{
    public interface IUserSettingService : IEntityService<UserSetting> { }
    public class UserSettingService : EntityService<UserSetting>, IUserSettingService
    {
        public UserSettingService() : base() { }
        public UserSettingService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public ActionResult UserBlock(string id)
        {
            if (string.IsNullOrEmpty(id))
                return Notifization.NotFound("Dữ liệu không hợp lệ");
            id = id.ToLower();
            UserSettingService userSettingService = new UserSettingService(_connection);
            var userSetting = userSettingService.GetAlls(m => m.UserID == id).FirstOrDefault();
            if (userSetting == null)
                return Notifization.NotFound("Dữ liệu không hợp lệ");
            userSetting.IsBlock = true;
            userSettingService.Update(userSetting);
            return Notifization.Success(MessageText.UpdateSuccess);
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult UserUnlock(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return Notifization.NotFound("Dữ liệu không hợp lệ");
            id = id.ToLower();
            UserSettingService userSettingService = new UserSettingService(_connection);
            var userSetting = userSettingService.GetAlls(m => m.UserID == id).FirstOrDefault();
            if (userSetting == null)
                return Notifization.NotFound("Dữ liệu không hợp lệ");
            userSetting.IsBlock = false;
            userSettingService.Update(userSetting);
            return Notifization.Success(MessageText.UpdateSuccess);
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult UserActive(string id)
        {
            if (string.IsNullOrEmpty(id))
                return Notifization.NotFound("Dữ liệu không hợp lệ");
            id = id.ToLower();
            UserSettingService userSettingService = new UserSettingService(_connection);
            var userSetting = userSettingService.GetAlls(m => m.UserID == id).FirstOrDefault();
            if (userSetting == null)
                return Notifization.NotFound("Dữ liệu không hợp lệ");
            userSetting.Enabled = (int)ModelEnum.Enabled.ENABLED;
            userSettingService.Update(userSetting);
            return Notifization.Success(MessageText.UpdateSuccess);
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult UserUnActive(string id)
        {
            if (string.IsNullOrEmpty(id))
                return Notifization.NotFound("Dữ liệu không hợp lệ");
            id = id.ToLower();
            UserSettingService userSettingService = new UserSettingService(_connection);
            var userSetting = userSettingService.GetAlls(m => m.UserID == id).FirstOrDefault();
            if (userSetting == null)
                return Notifization.NotFound("Dữ liệu không hợp lệ");
            userSetting.Enabled = (int)ModelEnum.Enabled.DISABLE;
            userSettingService.Update(userSetting);
            return Notifization.Success(MessageText.UpdateSuccess);
        }
        //##############################################################################################################################################################################################################################################################
    }
}