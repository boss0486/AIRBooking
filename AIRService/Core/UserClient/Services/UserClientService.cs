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
using Helper.File;
using WebCore.Model.Entities;
using System.Data;
using System.Web.Security;
using System.Runtime.CompilerServices;
using WebCore.ENM;
using Helper.TimeData;

namespace WebCore.Services
{
    public interface IUserClientService : IEntityService<DbConnection> { }
    public class UserClientService : EntityService<DbConnection>, IUserClientService
    {

        public UserClientService() : base() { }
        public UserClientService(System.Data.IDbConnection db) : base(db) { }


        //###############################################################################################################################
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
            string userId = Helper.Current.UserLogin.IdentifierID;
            //
            if (Helper.Current.UserLogin.IsClientInApplication())
            {
                string agentId = AirAgentService.GetAgentIDByUserID(userId);
                // show all with admin application
                if (Helper.Current.UserLogin.IsAdminAgentLogged())
                {
                    whereCondition += " AND c.AgentID = '" + agentId + "'";
                }
                else
                {
                    whereCondition += " AND c.AgentID = '" + agentId + "' AND  c.UserID = '" + userId + "'";
                }
            }
            else
            if (!Helper.Current.UserLogin.IsCMSUser && !Helper.Current.UserLogin.IsAdminInApplication)
            {
                return Notifization.AccessDenied(MessageText.AccessDenied);
            }
            //
            string langID = Helper.Current.UserLogin.LanguageID;
            using (var service = new UserClientService())
            {
                string sqlQuery = $@"
                SELECT vu.ID, vu.LoginID, vu.FullName, vu.Nickname, vu.Birthday, vu.Email, vu.LanguageID, vu.Status, vu.SiteID, vu.Enabled, vu.CreatedBy, vu.CreatedDate,	 
                (CASE
                    WHEN c.ClientType = 1 THEN (select CodeID from App_AirAgent where id = c.AgentID)   
                    ELSE (select CodeID from App_Company where id = c.AgentID) 
                END)as 'ClientCode',
                c.ClientType 
                FROM View_User as vu 
                INNER JOIN dbo.App_ClientLogin as c ON vu.ID = c.UserID
                WHERE (dbo.Uni2NONE(vu.LoginID) LIKE N'%'+ @Query +'%' OR dbo.Uni2NONE(vu.FullName) LIKE N'%'+ @Query +'%') {whereCondition}";
                //
                var dtList = _connection.Query<UserClientResult>(sqlQuery, new { Query = Helper.Page.Library.FormatNameToUni2NONE(query) }).OrderBy(m => m.ClientCode).OrderBy(m => m.LoginID).OrderBy(m => m.CreatedDate).ToList();
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
                //
                Helper.Pagination.PagingModel pagingModel = new Helper.Pagination.PagingModel
                {
                    PageSize = Helper.Pagination.Paging.PAGESIZE,
                    Total = dtList.Count,
                    Page = page
                };
                //
                return Notifization.Data(MessageText.Success + "::" + query, data: result, role: RoleActionSettingService.RoleListForUser(), paging: pagingModel);
            }
        }

        //##############################################################################################################################################################################################################################################################

        public ActionResult Create(UserClientCreateModel model)
        {

            using (var service = new UserClientService())
            {
                _connection.Open();
                using (var _transaction = _connection.BeginTransaction())
                {
                    try
                    {
                        if (model == null)
                            return Notifization.Invalid(MessageText.Invalid);
                        //
                        string fullName = model.FullName;
                        string birthday = model.Birthday;
                        string imageFile = model.ImageFile;
                        string email = model.Email;
                        string phone = model.Phone;
                        string address = model.Address;
                        //
                        string loginId = model.LoginID;
                        string password = model.Password;
                        //////string roleId = model.RoleID;
                        string agentId = model.AgentID;
                        //
                        string languageId = "";
                        bool isBlock = model.IsBlock;
                        int enabled = model.Enabled;
                        //
                        string crrUserId = Helper.Current.UserLogin.IdentifierID;
                        ClientLoginService clientLoginService = new ClientLoginService(_connection);
                        UserService userService = new UserService(_connection);
                        if (!userService.IsClientInApplication(crrUserId, _connection, _transaction))
                        {
                            //
                            var clientLogin = clientLoginService.GetAlls(m => m.AgentID == agentId, transaction: _transaction).FirstOrDefault();
                            if (clientLogin == null)
                                return Notifization.Invalid(MessageText.Invalid);
                            //
                        }
                        else
                        {
                            var clientLogin = clientLoginService.GetAlls(m => m.UserID == crrUserId, transaction: _transaction).FirstOrDefault();
                            if (clientLogin == null)
                                return Notifization.Invalid(MessageText.Invalid);
                            //
                            agentId = clientLogin.AgentID;
                        }
                        if (string.IsNullOrWhiteSpace(agentId))
                        {
                            return Notifization.Invalid(MessageText.Invalid);
                        }
                        // full name valid
                        if (string.IsNullOrWhiteSpace(fullName))
                            return Notifization.Invalid("Không được để trống họ/tên");

                        if (string.IsNullOrWhiteSpace(fullName))
                            return Notifization.Invalid("Không được để trống họ/tên");
                        //
                        if (fullName.Length < 2 || fullName.Length > 30)
                            return Notifization.Invalid("Họ/tên giới hạn [2-30] ký tự");
                        //
                        if (!Validate.TestAlphabet(fullName))
                            return Notifization.Invalid("Họ/tên không hợp lệ");
                        //
                        if (string.IsNullOrWhiteSpace(fullName))
                            return Notifization.Invalid("Không được để trống họ/tên");
                        //
                        if (fullName.Length < 2 || fullName.Length > 30)
                            return Notifization.Invalid("Họ tên giới hạn [2-30] ký tự");
                        //
                        if (!Validate.TestAlphabet(fullName))
                            return Notifization.Invalid("Họ tên không hợp lệ");
                        // birthday
                        if (!string.IsNullOrWhiteSpace(birthday))
                        {
                            if (!Validate.TestDate(birthday))
                                return Notifization.Invalid("Ngày sinh không hợp lệ");
                        }
                        else
                        {
                            birthday = String.Format("{0:dd-MM-yyyy}", DateTime.Now);
                        }
                        //  email
                        if (string.IsNullOrWhiteSpace(email))
                            return Notifization.Invalid("Không được để trống địa chỉ email");
                        //
                        if (!Validate.TestEmail(email))
                            return Notifization.Invalid("Địa chỉ email không hợp lệ");
                        // phone number
                        if (!string.IsNullOrWhiteSpace(phone))
                        {
                            if (!Validate.TestPhone(phone))
                                return Notifization.Invalid("Số điện thoại không hợp lệ");
                        }

                        if (!string.IsNullOrWhiteSpace(address))
                        {
                            if (!Validate.TestAlphabet(address))
                                return Notifization.Invalid("Địa chỉ không hợp lệ");
                        }
                        // account
                        if (string.IsNullOrWhiteSpace(loginId))
                            return Notifization.Invalid("Không được để trống tài khoản");
                        //
                        loginId = loginId.Trim();
                        if (!Validate.TestUserName(loginId))
                            return Notifization.Invalid("Tài khoản không hợp lệ");
                        if (loginId.Length < 4 || loginId.Length > 16)
                            return Notifization.Invalid("Tài khoản giới hạn [4-16] ký tự");
                        // password
                        if (string.IsNullOrWhiteSpace(password))
                            return Notifization.Invalid("Không được để trống mật khẩu");
                        if (!Validate.TestPassword(password))
                            return Notifization.Invalid("Yêu cầu mật khẩu bảo mật hơn");
                        if (password.Length < 4 || password.Length > 16)
                            return Notifization.Invalid("Mật khẩu giới hạn [4-16] ký tự");
                        //
                        //if (string.IsNullOrWhiteSpace(langID))
                        //    return Notifization.Invalid("Vui lòng chọn ngôn ngữ");
                        // call service

                        string sqlQuery = @"SELECT TOP(1) ID FROM CMSUserLogin WHERE LoginID = @LoginID 
                        UNION 
                        SELECT TOP(1) ID FROM UserLogin WHERE LoginID = @LoginID ";
                        //
                        var userLogin = _connection.Query<UserClientIDModel>(sqlQuery, new { LoginID = loginId }, transaction: _transaction).FirstOrDefault();
                        if (userLogin != null)
                            return Notifization.Error("Tài khoản đã được sử dụng");
                        //
                        sqlQuery = @"SELECT TOP(1) ID FROM CMSUserInfo WHERE Email = @Email 
                                     UNION 
                                     SELECT TOP(1) ID FROM UserInfo WHERE Email = @Email ";
                        //
                        userLogin = _connection.Query<UserClientIDModel>(sqlQuery, new { Email = email }, transaction: _transaction).FirstOrDefault();
                        if (userLogin != null)
                            return Notifization.Error("Địa chỉ email đã được sử dụng");
                        //
                        //////RoleService roleService = new RoleService(_connection);
                        //////if (!string.IsNullOrWhiteSpace(roleId))
                        //////{
                        //////    roleId = roleId.ToLower();
                        //////    var role = roleService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.ID == roleId, transaction: _transaction);
                        //////    if (role == null)
                        //////        return Notifization.Error("Nhóm quyền không hợp lệ");
                        //////}
                        //
                        UserLoginService userLoginService = new UserLoginService(_connection);
                        UserInfoService userInfoService = new UserInfoService(_connection);
                        UserSettingService userSettingService = new UserSettingService(_connection);
                        UserRoleService userRoleService = new UserRoleService(_connection);
                        LanguageService languageService = new LanguageService(_connection);
                        //
                        //var lang = languageService.GetAlls(m => m.LanguageID == model.LanguageID, transaction: transaction).FirstOrDefault();
                        //if (lang == null)
                        //    return Notifization.NOTFOUND("Ngôn ngữ không hợp lệ");
                        //
                        string userId = userLoginService.Create<string>(new UserLogin()
                        {
                            LoginID = loginId.ToLower(),
                            Password = Helper.Security.Library.Encryption256(model.Password),
                            PinCode = null,
                            TokenID = null,
                            OTPCode = null,
                            IsAdministrator = false
                        }, transaction: _transaction);
                        //
                        userInfoService.Create<string>(new UserInfo()
                        {
                            UserID = userId,
                            ImageFile = imageFile,
                            FullName = model.FullName,
                            Birthday = TimeFormat.FormatToServerDate(birthday),
                            Email = model.Email.ToLower(),
                            Phone = model.Phone,
                            Address = model.Address
                        }, transaction: _transaction);
                        //
                        userSettingService.Create<string>(new UserSetting()
                        {
                            UserID = userId,
                            SecurityPassword = null,
                            AuthenType = null,
                            IsBlock = isBlock,
                            Enabled = enabled,
                            LanguageID = languageId,
                        }, transaction: _transaction);
                        // role
                        //////if (!string.IsNullOrWhiteSpace(roleId))
                        //////{
                        //////    userRoleService.Create<string>(new UserRole()
                        //////    {
                        //////        RoleID = roleId,
                        //////        UserID = userId
                        //////    }, transaction: _transaction);
                        //////}
                        // user for client
                        clientLoginService.Create<string>(new ClientLogin()
                        {
                            UserID = userId,
                            AgentID = agentId,
                        }, transaction: _transaction);
                        //
                        _transaction.Commit();
                        return Notifization.Success(MessageText.CreateSuccess);
                    }
                    catch (Exception ex)
                    {
                        _transaction.Rollback();
                        return Notifization.TEST(">>" + ex);
                    }
                }
            }
        }
        //##############################################################################################################################################################################################################################################################

        public ActionResult Update(UserClientUpdateModel model)
        {
            using (var service = new UserClientService())
            {
                _connection.Open();
                using (var _transaction = _connection.BeginTransaction())
                {
                    try
                    {

                        if (model == null)
                            return Notifization.Invalid(MessageText.Invalid);
                        //
                        string userId = model.ID;
                        string fullName = model.FullName;
                        string birthday = model.Birthday;
                        string imageFile = model.ImageFile;
                        string email = model.Email;
                        string phone = model.Phone;
                        string address = model.Address;
                        string nickName = model.NickName;
                        //
                        //string roleId = model.RoleID;
                        string languageId = model.LanguageID;
                        bool isBlock = model.IsBlock;
                        int enabled = model.Enabled;
                        //
                        if (string.IsNullOrWhiteSpace(userId))
                            return Notifization.Invalid(MessageText.Invalid);
                        userId = userId.Trim().ToLower();

                        UserLoginService userLoginService = new UserLoginService(_connection);
                        UserInfoService userInfoService = new UserInfoService(_connection);
                        UserSettingService userSettingService = new UserSettingService(_connection);
                        UserRoleService userRoleService = new UserRoleService(_connection);

                        LanguageService languageService = new LanguageService(_connection);
                        //update user information
                        var userLogin = userLoginService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.ID == userId, transaction: _transaction).FirstOrDefault();
                        if (userLogin == null)
                            return Notifization.Invalid(MessageText.Invalid);
                        //
                        //////RoleService roleService = new RoleService(_connection);
                        //////if (!string.IsNullOrWhiteSpace(roleId))
                        //////{
                        //////    roleId = roleId.ToLower();
                        //////    var role = roleService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.ID == roleId, transaction: _transaction);
                        //////    if (role == null)
                        //////        return Notifization.Error("Nhóm quyền không hợp lệ");
                        //////}
                        var _uId = userLogin.ID;
                        var userInfo = userInfoService.GetAlls(m => !string.IsNullOrWhiteSpace(m.UserID) && m.UserID == _uId, transaction: _transaction).FirstOrDefault();
                        if (userInfo == null)
                            return Notifization.Invalid(MessageText.Invalid);
                        //
                        //  email
                        if (string.IsNullOrWhiteSpace(email))
                            return Notifization.Invalid("Không được để trống địa chỉ email");
                        if (!Validate.TestEmail(email))
                            return Notifization.Invalid("Địa chỉ email không hợp lệ");
                        //
                        string sqlQuery = @"SELECT TOP(1) ID FROM CMSUserInfo WHERE Email = @Email AND UserID !=@ID 
                                  UNION 
                                  SELECT TOP(1) ID FROM UserInfo WHERE Email = @Email AND UserID !=@ID ";
                        //
                        var userEmail = _connection.Query<UserIDModel>(sqlQuery, new { Email = email, ID = userId }, transaction: _transaction).FirstOrDefault();
                        if (userEmail != null)
                            return Notifization.Error("Địa chỉ email đã được sử dụng");
                        //
                        userInfo.ImageFile = imageFile;
                        userInfo.FullName = fullName;
                        userInfo.NickName = model.NickName;
                        userInfo.Birthday = TimeFormat.FormatToServerDate(model.Birthday);
                        //userInfo.Email = model.Email.ToLower();
                        userInfo.Phone = model.Phone;
                        userInfo.Address = model.Address;
                        userInfoService.Update(userInfo, transaction: _transaction);
                        //
                        var userSetting = userSettingService.GetAlls(m => !string.IsNullOrWhiteSpace(m.UserID) && m.UserID == _uId, transaction: _transaction).FirstOrDefault();
                        if (userSetting == null)
                            return Notifization.Invalid(MessageText.Invalid);
                        //
                        //userSetting.AreaID = areaId;
                        userSetting.IsBlock = isBlock;
                        userSetting.Enabled = enabled;
                        userSetting.LanguageID = model.LanguageID;
                        userSettingService.Update(userSetting, transaction: _transaction);
                        //
                        //if (!string.IsNullOrWhiteSpace(roleId))
                        //{
                        //    // delete role cũ
                        //    var userRole = userRoleService.GetAlls(m => m.UserID == _uId, transaction: _transaction).FirstOrDefault();
                        //    if (userRole == null)
                        //    {
                        //        userRoleService.Create<string>(new UserRole()
                        //        {
                        //            RoleID = roleId,
                        //            UserID = _uId
                        //        }, transaction: _transaction);
                        //    }
                        //    else
                        //    {
                        //        userRole.RoleID = roleId;
                        //        userRoleService.Update(userRole, transaction: _transaction);
                        //    }
                        //}
                        _transaction.Commit();
                        return Notifization.Success(MessageText.UpdateSuccess);
                    }
                    catch (Exception ex)
                    {
                        _transaction.Rollback();
                        return Notifization.TEST(">>>>" + ex);
                    }
                }
            }
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Delete(UserIDModel model)
        {
            using (var service = new UserClientService())
            {
                _connection.Open();
                using (var _transaction = _connection.BeginTransaction())
                {
                    try
                    {
                        string id = model.ID;
                        if (string.IsNullOrWhiteSpace(id))
                            return Notifization.Invalid(MessageText.Invalid);
                        //
                        string sqlQuery = sqlQuery = @"SELECT TOP 1 * FROM View_User WHERE ID = @ID";
                        var cmsUserResult = _connection.Query<UserClientResult>(sqlQuery, new { ID = id }, transaction: _transaction).FirstOrDefault();
                        if (cmsUserResult == null)
                            return Notifization.Error(MessageText.Invalid);
                        // delete
                        AttachmentFile.DeleteFile(cmsUserResult.ImageFile, dbTransaction: _transaction);

                        _connection.Execute("DELETE App_ClientLogin WHERE UserID = @UserID", new { UserID = id }, transaction: _transaction);
                        _connection.Execute("DELETE UserRole WHERE UserID = @UserID ", new { UserID = id }, transaction: _transaction);
                        //
                        _connection.Execute("DELETE UserInfo WHERE UserID = @UserID", new { UserID = id }, transaction: _transaction);
                        _connection.Execute("DELETE UserSetting WHERE UserID = @UserID", new { UserID = id }, transaction: _transaction);
                        _connection.Execute("DELETE UserLogin WHERE ID = @ID", new { ID = id }, transaction: _transaction);
                        _transaction.Commit();
                        return Notifization.Success(MessageText.DeleteSuccess);
                    }
                    catch (Exception ex)
                    {
                        _transaction.Rollback();
                        return Notifization.TEST(">>" + ex);
                    }
                }
            }
        }

        public ActionResult UserRoleSetting(UserRoleSettingModel model)
        {
            var service = new UserClientService();
            string roleId = model.RoleID;
            string userId = model.UserID;
            if (string.IsNullOrWhiteSpace(userId))
                return Notifization.Invalid(MessageText.Invalid);
            // 
            RoleService roleService = new RoleService(_connection);
            UserRoleService userRoleService = new UserRoleService(_connection);
            UserLoginService userLoginService = new UserLoginService(_connection);
            userId = userId.ToLower();
            UserLogin userLogin = userLoginService.GetAlls(m => m.ID == userId).FirstOrDefault();
            if (userLogin == null)
                return Notifization.NotFound(MessageText.NotFound);
            //
            UserRole userRole = userRoleService.GetAlls(m => m.UserID == userId).FirstOrDefault();
            if (userRole == null)
            {
                if (string.IsNullOrWhiteSpace(roleId))
                    return Notifization.Invalid(MessageText.Invalid);
                // 
                roleId = roleId.ToLower();
                Role role = roleService.GetAlls(m => m.ID == roleId).FirstOrDefault();
                if (role == null)
                    return Notifization.NotFound(MessageText.NotFound);
                //
                userRoleService.Create<string>(new UserRole()
                {
                    RoleID = roleId,
                    UserID = userId
                });
            }
            else
            {   // role empty => delete role of user
                if (string.IsNullOrWhiteSpace(roleId))
                {
                    userRoleService.Remove(userRole.ID);
                }
                else
                {
                    roleId = roleId.ToLower();
                    Role role = roleService.GetAlls(m => m.ID == roleId).FirstOrDefault();
                    if (role == null)
                        return Notifization.NotFound(MessageText.NotFound);
                    //
                    userRole.RoleID = roleId;
                    userRoleService.Update(userRole);
                }
            }
            return Notifization.Success(MessageText.UpdateSuccess);
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Details(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return Notifization.NotFound(MessageText.Invalid);
                //
                string langID = Helper.Current.UserLogin.LanguageID;
                var data = GetUserModel(id);
                if (data == null)
                    return Notifization.NotFound(MessageText.NotFound);
                return Notifization.Data(MessageText.Success, data: data, role: null, paging: null);
            }
            catch
            {
                return Notifization.NotService;
            }
        }

        public UserClientResult GetUserModel(string id)
        {
            try
            {

                using (var service = new UserClientService())
                {
                    var _connection = service._connection;
                    if (string.IsNullOrWhiteSpace(id))
                        return null;
                    //
                    string sqlQuery = @"SELECT TOP (1) * FROM View_User WHERE ID = @ID";
                    var data = _connection.Query<UserClientResult>(sqlQuery, new { ID = id }).FirstOrDefault();
                    if (data == null)
                        return null;
                    //
                    return data;
                }
            }
            catch
            {
                return null;
            }
        }

        public List<EmployeeModel> GetEmployeeByAgentLogin()
        {
            string agentId = string.Empty;
            string whereCondition = string.Empty;
            if (Helper.Current.UserLogin.IsAdminAgentLogged())
            {
                agentId = AirAgentService.GetAgentIDByUserID(Helper.Current.UserLogin.IdentifierID);
                whereCondition = " AND client.AgentID = @AgentID";
            }
            //
            string sqlQuery = $@"SELECT uf.UserID, uf.FullName, ur.RoleID, (select top 1 Amount from App_TiketingSpendingLimit where UserID = ur.UserID) as Spending  
                                FROM UserInfo as uf
                                INNER JOIN App_ClientLogin as client On client.UserID = uf.UserID
                                LEFT JOIN UserRole as ur ON client.UserID =  ur.UserID
                                INNER JOIN [Role] as r On r.ID = ur.RoleID
                                where r.IsAllowSpend = 1 {whereCondition} ORDER BY uf.FullName";
            //
            List<EmployeeModel> data = _connection.Query<EmployeeModel>(sqlQuery, new { AgentID = agentId }).ToList();
            if (data.Count == 0)
                return new List<EmployeeModel>();
            //
            return data;
        }
        public List<EmployeeModel> GetUserByAgentID(string agentId)
        {
            string sqlQuery = $@"SELECT uf.UserID, uf.FullName, ur.RoleID,  ISNULL((select top 1 Amount from App_UserSpending where TicketingID = ur.UserID),0) as 'Spending'   
                                FROM UserInfo as uf
                                INNER JOIN App_ClientLogin as client On client.UserID = uf.UserID
                                LEFT JOIN UserRole as ur ON client.UserID =  ur.UserID
                                INNER JOIN [Role] as r On r.ID = ur.RoleID
                                WHERE r.IsAllowSpend = 1 AND AgentID = @AgentID ORDER BY uf.FullName";
            //
            List<EmployeeModel> data = _connection.Query<EmployeeModel>(sqlQuery, new { AgentID = agentId }).ToList();
            if (data.Count == 0)
                return new List<EmployeeModel>();
            //
            return data;
        }
        //##############################################################################################################################################################################################################################################################
        public static string DropdownListEmployee(string id)
        {
            string result = string.Empty;
            using (var service = new AirAgentService())
            {
                UserClientService userClientService = new UserClientService();
                List<EmployeeModel> dtList = userClientService.GetEmployeeByAgentLogin();
                if (dtList.Count > 0)
                {
                    foreach (var item in dtList)
                    {
                        string strSelect = string.Empty;
                        if (!string.IsNullOrWhiteSpace(id) && item.UserID == id.ToLower())
                            strSelect = "selected";
                        else if (string.IsNullOrWhiteSpace(id) && item.UserID == dtList[0].UserID)
                            strSelect = "selected";
                        //
                        result += $"<option value='{item.UserID}' {strSelect} data-spending='{item.Spending }'>{item.FullName }</option>";
                    }
                }
                return result;
            }

        }

        //##############################################################################################################################################################################################################################################################

    }
}
