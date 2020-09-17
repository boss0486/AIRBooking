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

namespace WebCore.Services
{

    public interface IAccountService : IEntityService<DbConnection> { }
    public class AccountService : EntityService<DbConnection>, IAccountService
    {

        public AccountService() : base() { }
        public AccountService(System.Data.IDbConnection db) : base(db) { }
        //###############################################################################################################################
        public ActionResult Datalist(SearchModel model)
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
                Status = model.Status,
                TimeExpress = model.TimeExpress,
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
            using (var service = new AccountService())
            {
                string sqlQuery = @"SELECT * FROM View_User as vu  WHERE vu.ID NOT IN (SELECT UserID FROM App_ClientLogin) AND  dbo.Uni2NONE(FullName) LIKE N'%'+ @Query +'%' " + whereCondition + " ORDER BY FullName,CreatedDate";
                var dtList = service.Query<UserResult>(sqlQuery, new { Query = query }).ToList();
                if (dtList.Count == 0)
                    return Notifization.NotFound(MessageText.NotFound);
                //
                var result = dtList.ToPagedList(page, Helper.Pagination.Paging.PAGESIZE).ToList();
                if (result.Count <= 0 && page > 1)
                {
                    page -= 1;
                    result = dtList.ToPagedList(page, Helper.Pagination.Paging.PAGESIZE).ToList();
                }
                if (result.Count == 0)
                    return Notifization.NotFound(MessageText.NotFound);
                //
                Helper.Pagination.PagingModel pagingModel = new Helper.Pagination.PagingModel
                {
                    PageSize = Helper.Pagination.Paging.PAGESIZE,
                    Total = dtList.Count,
                    Page = page
                };
                //
                return Notifization.Data(MessageText.Success, data: result, role: RoleActionSettingService.RoleListForUser(), paging: pagingModel);
            }
        }

        //##############################################################################################################################################################################################################################################################

        public ActionResult Create(UserCreateModel model)
        {
            using (var service = new AccountService())
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
                        string nickName = model.NickName;

                        string loginId = model.LoginID;
                        string password = model.Password;
                        string roleId = model.RoleID;

                        string rePassword = model.RePassword;
                        string languageId = model.LanguageID;
                        bool isBlock = model.IsBlock;
                        int enabled = model.Enabled;
                        //
                        // full name valid
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
                        // nick name
                        if (!string.IsNullOrWhiteSpace(nickName))
                        {
                            if (nickName.Length < 2 || nickName.Length > 30)
                                return Notifization.Invalid("Biệt danh giới hạn [2-30] ký tự");
                            //
                            if (!Validate.TestAlphabet(nickName))
                                return Notifization.Invalid("Biệt danh không hợp lệ");
                        }
                        // birthday
                        if (!string.IsNullOrWhiteSpace(birthday))
                        {
                            if (!Validate.TestDateVN(birthday))
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
                        string sqlQuery = @"SELECT TOP(1) ID FROM View_CMSUser WHERE LoginID = @LoginID 
                                         UNION 
                                         SELECT TOP(1) ID FROM View_User WHERE LoginID = @LoginID ";
                        //
                        var userLogin = _connection.Query<UserIDModel>(sqlQuery, new { LoginID = loginId }, transaction: _transaction).FirstOrDefault();
                        if (userLogin != null)
                            return Notifization.Error("Tài khoản đã được sử dụng");
                        //
                        sqlQuery = @"SELECT TOP(1) ID FROM View_CMSUser WHERE Email = @Email 
                                  UNION 
                                  SELECT TOP(1) ID FROM View_User WHERE Email = @Email ";
                        //
                        userLogin = _connection.Query<UserIDModel>(sqlQuery, new { Email = email }, transaction: _transaction).FirstOrDefault();
                        if (userLogin != null)
                            return Notifization.Error("Địa chỉ email đã được sử dụng");
                        //
                        //
                        RoleService roleService = new RoleService(_connection);
                        if (!string.IsNullOrWhiteSpace(roleId))
                        {
                            roleId = roleId.ToLower();
                            var role = roleService.GetAlls(m => m.ID == roleId, transaction: _transaction);
                            if (role == null)
                                return Notifization.Error("Nhóm quyền không hợp lệ");
                        }
                        //
                        //var lang = languageService.GetAlls(m => m.LanguageID == model.LanguageID, transaction: transaction).FirstOrDefault();
                        //if (lang == null)
                        //    return Notifization.NOTFOUND("Ngôn ngữ không hợp lệ");
                        //

                        UserLoginService userLoginService = new UserLoginService(_connection);
                        UserInfoService userInfoService = new UserInfoService(_connection);
                        UserSettingService userSettingService = new UserSettingService(_connection);
                        UserRoleService userRoleService = new UserRoleService(_connection);
                        LanguageService languageService = new LanguageService(_connection);
                        string userId = userLoginService.Create<string>(new UserLogin()
                        {
                            LoginID = loginId.ToLower(),
                            Password = Helper.Security.Library.Encryption256(model.Password),
                            PinCode = null,
                            TokenID = null,
                            OTPCode = null,
                            IsAdministrator = true
                        }, transaction: _transaction);
                        //
                        userInfoService.Create<string>(new UserInfo()
                        {
                            UserID = userId,
                            ImageFile = "",
                            FullName = model.FullName,
                            Birthday = Helper.Time.TimeHelper.FormatToDateSQL(model.Birthday),
                            Email = model.Email.ToLower(),
                            Phone = model.Phone,
                            Address = model.Address
                        }, transaction: _transaction);
                        //
                        string indentity = string.Empty;
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
                        if (!string.IsNullOrWhiteSpace(roleId))
                        {
                            userRoleService.Create<string>(new UserRole()
                            {
                                RoleID = roleId,
                                UserID = userId
                            }, transaction: _transaction);
                        }
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

        public ActionResult Update(UserUpdateModel model)
        {
            using (var service = new AccountService())
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
                        string roleId = model.RoleID;
                        //
                        string languageId = model.LanguageID;
                        bool isBlock = model.IsBlock;
                        int enabled = model.Enabled;
                        //
                        if (string.IsNullOrWhiteSpace(userId))
                            return Notifization.Invalid(MessageText.Invalid);
                        //
                        userId = userId.Trim().ToLower();
                        //
                        UserLoginService userLoginService = new UserLoginService(_connection);
                        UserInfoService userInfoService = new UserInfoService(_connection);
                        UserSettingService userSettingService = new UserSettingService(_connection);
                        UserRoleService userRoleService = new UserRoleService(_connection);
                        LanguageService languageService = new LanguageService(_connection);
                        //
                        var userLogin = userLoginService.GetAlls(m => m.ID == userId, transaction: _transaction).FirstOrDefault();
                        if (userLogin == null)
                            return Notifization.Invalid(MessageText.Invalid);
                        //  email
                        if (string.IsNullOrWhiteSpace(email))
                            return Notifization.Invalid("Không được để trống địa chỉ email");
                        if (!Validate.TestEmail(email))
                            return Notifization.Invalid("Địa chỉ email không hợp lệ");
                        //
                        string sqlQuery = @"SELECT TOP(1) ID FROM CMSUserInfo WHERE Email = @Email AND UserID !=@UserID 
                                  UNION 
                                  SELECT TOP(1) ID FROM UserInfo WHERE Email = @Email AND UserID !=@UserID ";
                        //
                        var userEmail = _connection.Query<UserIDModel>(sqlQuery, new { Email = email, UserID = userId }, transaction: _transaction).FirstOrDefault();
                        if (userEmail != null)
                            return Notifization.Error("Địa chỉ email đã được sử dụng");
                        //update user information
                        RoleService roleService = new RoleService(_connection);
                        if (!string.IsNullOrWhiteSpace(roleId))
                        {
                            roleId = roleId.ToLower();
                            var role = roleService.GetAlls(m => m.ID == roleId, transaction: _transaction);
                            if (role == null)
                                return Notifization.Error("Nhóm quyền không hợp lệ");
                        }
                        var userInfo = userInfoService.GetAlls(m => m.UserID == userId, transaction: _transaction).FirstOrDefault();
                        if (userInfo == null)
                            return Notifization.Invalid(MessageText.Invalid);
                        //
                        //
                        var userSetting = userSettingService.GetAlls(m => m.UserID == userId, transaction: _transaction).FirstOrDefault();
                        if (userSetting == null)
                            return Notifization.Invalid(MessageText.Invalid);

                        userInfo.ImageFile = imageFile;
                        userInfo.FullName = fullName;
                        userInfo.NickName = model.NickName;
                        userInfo.Birthday = Helper.Time.TimeHelper.FormatToDateSQL(model.Birthday);
                        //userInfo.Email = model.Email.ToLower();
                        userInfo.Phone = model.Phone;
                        userInfo.Address = model.Address;
                        userInfoService.Update(userInfo, transaction: _transaction);
                        //
                        //userSetting.AreaID = areaId;
                        //userSetting.IsRootUser 
                        userSetting.IsBlock = isBlock;
                        userSetting.Enabled = enabled;
                        userSetting.LanguageID = model.LanguageID;
                        userSettingService.Update(userSetting, transaction: _transaction);
                        //
                        if (!string.IsNullOrWhiteSpace(roleId))
                        {
                            // delete role cũ
                            var userRole = userRoleService.GetAlls(m => m.UserID == userId, transaction: _transaction).FirstOrDefault();
                            if (userRole == null)
                            {
                                userRoleService.Create<string>(new UserRole()
                                {
                                    RoleID = roleId,
                                    UserID = userId
                                }, transaction: _transaction);
                            }
                            else
                            {
                                userRole.RoleID = roleId;
                                userRoleService.Update(userRole, transaction: _transaction);
                            }
                        }
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
            using (var service = new AccountService())
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
                        var cmsUserResult = _connection.Query<CMSUserResult>(sqlQuery, new { ID = id }, transaction: _transaction).FirstOrDefault();
                        if (cmsUserResult == null)
                            return Notifization.Error(MessageText.Invalid);
                        // delete
                        AttachmentFile.DeleteFile(cmsUserResult.ImageFile, transaction: _transaction);
                        _connection.Execute("DELETE UserRole WHERE UserID = @UserID ", new { UserID = id }, transaction: _transaction);
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

        public UserResult GetUserModel(string id)
        {
            try
            {
                using (var service = new AccountService())
                {
                    _connection.Open();
                    if (string.IsNullOrWhiteSpace(id))
                        return null;
                    //
                    string sqlQuery = @"SELECT TOP (1) * FROM View_User WHERE ID = @ID";
                    var data = _connection.Query<UserResult>(sqlQuery, new { ID = id }).FirstOrDefault();
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
        //##############################################################################################################################################################################################################################################################
        public ActionResult ChangePassword(UserChangePasswordModel model)
        {
            string userId = Helper.Current.UserLogin.IdentifierID;
            if (string.IsNullOrWhiteSpace(userId))
                return Notifization.Error("Bạn cần đăng nhập trước");
            //
            if (model == null)
                return Notifization.Invalid();
            //
            string loginID = model.Password;
            string passID = model.NewPassword;
            string rePass = model.ReNewPassword;
            //
            if (string.IsNullOrEmpty(passID))
                return Notifization.Invalid("Không được để trống mật khẩu");
            if (!Validate.TestPassword(passID))
                return Notifization.Invalid("Yêu cầu mật khẩu bảo mật hơn");
            if (passID.Length < 2 || passID.Length > 30)
                return Notifization.Invalid("Mật khẩu giới hạn [2-30] ký tự");
            string passwordHash = Helper.Security.Library.Encryption256(model.Password);
            // check account system
            UserLoginService userLoginService = new UserLoginService();
            var user = userLoginService.GetAlls(m => m.ID == userId).FirstOrDefault();
            if (user == null)
                return Notifization.NotFound(MessageText.NotFound);
            //
            if (user.Password != passwordHash)
                return Notifization.NotFound("Mật khẩu cũ chưa đúng");
            // update
            user.Password = Helper.Security.Library.Encryption256(model.NewPassword);
            userLoginService.Update(user);
            return Notifization.Success(MessageText.UpdateSuccess);
        }
        //##############################################################################################################################################################################################################################################################
    }
}
