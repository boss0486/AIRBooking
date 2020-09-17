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
    public class CMSUserService : IDisposable
    {
        private IDbConnection _connecion;
        private bool _disposed = false;

        public CMSUserService(IDbConnection dbConnection = null)
        {

            if (dbConnection != null)
                _connecion = dbConnection;
            else
            {
                _connecion = DbConnect.Connection.CMS;
                _connecion.Open();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                    _connecion.Close();
                //
                _disposed = true;
            }
        }

        ~CMSUserService()
        {
            Dispose(false);
        }

        //###############################################################################################################################
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
            string langID = Helper.Current.UserLogin.LanguageID;
            string sqlQuery = @"SELECT * FROM View_CMSUser WHERE dbo.Uni2NONE(FullName) LIKE N'%'+ @Query +'%' " + whereCondition + " ORDER BY FullName,CreatedDate";
            var dtList = _connecion.Query<UserResult>(sqlQuery, new { Query = query }).ToList();
            if (dtList.Count == 0)
                return Notifization.NotFound(MessageText.NotFound);
            //
            var result = dtList.ToPagedList(page, Helper.Pagination.Paging.PAGESIZE).ToList();
            if (result.Count == 0 && page > 1)
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
            // ressult data
            return Notifization.Data(MessageText.Success, data: result, role: RoleActionSettingService.RoleListForUser(), paging: pagingModel);
        }
        //##############################################################################################################################################################################################################################################################

        public ActionResult Create(UserCreateModel model)
        {
            using (var service = new CMSUserService())
            {
                var _connection = service._connecion;
                using (var transaction = _connection.BeginTransaction())
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
                        string rePassword = model.RePassword;
                        string languageId = model.LanguageID;
                        string roleId = model.RoleID;
                        bool isBlock = model.IsBlock;
                        int enabled = model.Enabled;
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
                        string sqlQuery = @"SELECT TOP(1) ID FROM View_CMSUserLogin WHERE LoginID = @LoginID 
                                         UNION 
                                         SELECT TOP(1) ID FROM View_UserLogin WHERE LoginID = @LoginID ";
                        //
                        var userLogin = _connection.Query<UserIDModel>(sqlQuery, new { LoginID = loginId }, transaction: transaction).FirstOrDefault();
                        if (userLogin != null)
                            return Notifization.Error("Tài khoản đã được sử dụng");
                        //
                        sqlQuery = @"SELECT TOP(1) ID FROM View_CMSUserLogin WHERE Email = @Email 
                                  UNION 
                                  SELECT TOP(1) ID FROM View_UserLogin WHERE Email = @Email ";
                        //
                        userLogin = _connection.Query<UserIDModel>(sqlQuery, new { Email = email }, transaction: transaction).FirstOrDefault();
                        if (userLogin != null)
                            return Notifization.Error("Địa chỉ email đã được sử dụng");
                        //
                        //var lang = languageService.GetAlls(m => m.LanguageID == model.LanguageID, transaction: transaction).FirstOrDefault();
                        //if (lang == null)
                        //    return Notifization.NOTFOUND("Ngôn ngữ không hợp lệ");
                        //
                        CMSUserLoginService cmsUserLoginService = new CMSUserLoginService(_connection);
                        CMSUserInfoService cmsUserInfoService = new CMSUserInfoService(_connection);
                        CMSUserSettingService cmsUserSettingService = new CMSUserSettingService(_connection);
                        CMSUserRoleService cmsUserRoleService = new CMSUserRoleService(_connection);
                        //CMSLanguageService languageService = new LanguageService(_connection);
                        string userId = cmsUserLoginService.Create<string>(new CMSUserLogin()
                        {
                            LoginID = loginId,
                            Password = Helper.Security.Library.Encryption256(model.Password),
                            TokenID = null,
                            OTPCode = null
                        }, transaction: transaction);
                        //
                        cmsUserInfoService.Create<string>(new CMSUserInfo()
                        {
                            UserID = userId,
                            ImageFile = "",
                            FullName = model.FullName,
                            Birthday = Helper.Time.TimeHelper.FormatToDateSQL(model.Birthday),
                            Email = model.Email.ToLower(),
                            Phone = model.Phone,
                            Address = model.Address
                        }, transaction: transaction);
                        //
                        string indentity = string.Empty;
                        //
                        cmsUserSettingService.Create<string>(new CMSUserSetting()
                        {
                            UserID = userId,
                            SecurityPassword = null,
                            AuthenType = null,
                            IsBlock = isBlock,
                            Enabled = enabled,
                            LanguageID = languageId,
                        }, transaction: transaction);
                        //
                        cmsUserRoleService.Create<string>(new CMSUserRole()
                        {
                            RoleID = roleId,
                        }, transaction: transaction);
                        //
                        transaction.Commit();
                        return Notifization.Success(MessageText.CreateSuccess);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return Notifization.NotService;
                    }
                }
            }
        }
        //##############################################################################################################################################################################################################################################################

        public ActionResult Update(UserUpdateModel model)
        {
            using (var service = new CMSUserService())
            {
                var _connection = service._connecion;
                using (var _transaction = _connection.BeginTransaction())
                {
                    try
                    {
                        if (model == null)
                            return Notifization.Invalid(MessageText.Invalid);
                        //
                        string id = model.ID;
                        string fullName = model.FullName;
                        string birthday = model.Birthday;
                        string imageFile = model.ImageFile;
                        string email = model.Email;
                        string phone = model.Phone;
                        string address = model.Address;
                        string nickName = model.NickName;
                        string loginId = model.LoginID;
                        string password = model.Password;
                        string rePassword = model.RePassword;
                        string languageId = model.LanguageID;
                        string roleId = model.RoleID;
                        bool isBlock = model.IsBlock;
                        int enabled = model.Enabled;
                        //
                        if (string.IsNullOrWhiteSpace(id))
                            return Notifization.Invalid(MessageText.Invalid + " -1");
                        id = id.Trim();
                        //  email
                        if (string.IsNullOrWhiteSpace(email))
                            return Notifization.Invalid("Không được để trống địa chỉ email");
                        if (!Validate.TestEmail(email))
                            return Notifization.Invalid("Địa chỉ email không hợp lệ");
                        //
                        string sqlQuery = @"SELECT TOP(1) ID FROM View_CMSUserLogin WHERE Email = @Email AND ID !=@ID 
                                  UNION 
                                  SELECT TOP(1) ID FROM View_UserLogin WHERE Email = @Email AND ID !=@ID ";
                        //
                        var userEmail = _connection.Query<UserIDModel>(sqlQuery, new { Email = email, ID = id }, transaction: _transaction).FirstOrDefault();
                        if (userEmail != null)
                            return Notifization.Error("Địa chỉ email đã được sử dụng");
                        //
                        CMSUserLoginService cmsUserLoginService = new CMSUserLoginService(_connection);
                        CMSUserInfoService cmsUserInfoService = new CMSUserInfoService(_connection);
                        CMSUserSettingService cmsUserSettingService = new CMSUserSettingService(_connection);
                        //CMSLanguageService languageService = new LanguageService(_connection);
                        var userLogin = cmsUserLoginService.GetAlls(m => m.ID == id, transaction: _transaction).FirstOrDefault();
                        if (userLogin == null)
                            return Notifization.Invalid(MessageText.Invalid);
                        //
                        var _uId = userLogin.ID;
                        var userInfo = cmsUserInfoService.GetAlls(m => m.UserID == _uId, transaction: _transaction).FirstOrDefault();
                        if (userInfo == null)
                            return Notifization.Invalid(MessageText.Invalid);

                        userInfo.ImageFile = imageFile;
                        userInfo.FullName = fullName;
                        userInfo.NickName = model.NickName;
                        userInfo.Birthday = Helper.Time.TimeHelper.FormatToDateSQL(model.Birthday);
                        userInfo.Email = model.Email.ToLower();
                        userInfo.Phone = model.Phone;
                        userInfo.Address = model.Address;
                        cmsUserInfoService.Update(userInfo, transaction: _transaction);
                        //
                        var userSetting = cmsUserSettingService.GetAlls(m => m.UserID == _uId, transaction: _transaction).FirstOrDefault();
                        if (userSetting == null)
                            return Notifization.Invalid(MessageText.Invalid);
                        // 
                        //userSetting.IsRootUser 
                        userSetting.IsBlock = isBlock;
                        userSetting.Enabled = enabled;
                        userSetting.LanguageID = model.LanguageID;
                        cmsUserSettingService.Update(userSetting, transaction: _transaction);
                        //
                        CMSUserRoleService cmsUserRoleService = new CMSUserRoleService(_connection);
                        var userRole = cmsUserRoleService.GetAlls(m => m.UserID == _uId, transaction: _transaction).FirstOrDefault();
                        if (userRole == null)
                        {
                            cmsUserRoleService.Create<string>(new CMSUserRole()
                            {
                                RoleID = roleId,
                            }, transaction: _transaction);
                        }
                        else
                        {
                            userRole.RoleID = roleId;
                            cmsUserRoleService.Update(userRole, transaction: _transaction);
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
            using (var service = new CMSUserService())
            {
                var _connection = service._connecion;
                using (var _transaction = _connection.BeginTransaction())
                {
                    try
                    {
                        if (model == null)
                            return Notifization.Invalid(MessageText.Invalid);
                        //
                        string id = model.ID;
                        if (string.IsNullOrWhiteSpace(id))
                            return Notifization.Invalid(MessageText.Invalid);
                        //
                        string sqlQuery = sqlQuery = @"SELECT TOP 1 * FROM View_CMSUser WHERE ID = @ID";
                        var cmsUserResult = _connection.Query<CMSUserResult>(sqlQuery, new { ID = id }, transaction: _transaction).FirstOrDefault();
                        if (cmsUserResult == null)
                            return Notifization.Error(MessageText.Invalid);
                        // delete
                        AttachmentFile.DeleteFile(cmsUserResult.ImageFile, transaction: _transaction);
                        _connection.Execute("DELETE CMSUserInfo WHERE UserID = @UserID", new { UserID = id }, transaction: _transaction);
                        _connection.Execute("DELETE CMSUserSetting WHERE UserID = @UserID", new { UserID = id }, transaction: _transaction);
                        _connection.Execute("DELETE CMSUserLogin WHERE ID = @ID", new { ID = id }, transaction: _transaction);
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
                using (var service = new CMSUserService())
                {
                    var _connection = service._connecion;
                    if (string.IsNullOrWhiteSpace(id))
                        return null;
                    //
                    string sqlQuery = @"SELECT TOP (1) * FROM View_CMSUser WHERE ID = @ID";
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
            string loginId = Helper.Current.UserLogin.IdentifierID;
            if (string.IsNullOrWhiteSpace(loginId))
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
            string passId = Helper.Security.Library.Encryption256(model.Password);
            // check account system
            CMSUserLoginService cmsUserLoginService = new CMSUserLoginService();
            var cmsUserLogin = cmsUserLoginService.GetAlls(m => m.ID == loginId).FirstOrDefault();
            if (cmsUserLogin == null)
                return Notifization.NotFound(MessageText.NotFound);
            //
            if (cmsUserLogin.Password != passId)
                return Notifization.NotFound("Mật khẩu cũ chưa đúng");
            // update
            cmsUserLogin.Password = Helper.Security.Library.Encryption256(model.NewPassword);
            cmsUserLoginService.Update(cmsUserLogin);
            return Notifization.Success(MessageText.UpdateSuccess);
        }
        //##############################################################################################################################################################################################################################################################

        public static string GetLoginIDByID(string id)
        {
            try
            {
                using (var service = new CMSUserService())
                {
                    var _connection = service._connecion;
                    if (string.IsNullOrWhiteSpace(id))
                        return string.Empty;
                    //
                    string sqlQuery = @"SELECT TOP (1) * FROM View_CMSUser WHERE ID = @ID";
                    var data = _connection.Query<UserModel>(sqlQuery, new { ID = id }).FirstOrDefault();
                    if (data == null)
                        return string.Empty;
                    //
                    return data.LoginID;
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        //##############################################################################################################################################################################################################################################################
        public string GetLanguageID
        {
            get
            {
                string result = Helper.Page.Default.LanguageID;
                try
                {
                    string userId = Helper.Current.UserLogin.IdentifierID;
                    if (string.IsNullOrWhiteSpace(userId))
                        return result;
                    //
                    UserSettingService service = new UserSettingService();
                    var userModal = service.GetAlls(m => m.UserID == userId).FirstOrDefault();
                    if (userModal == null)
                        return result;
                    //
                    return userModal.LanguageID;
                }
                catch
                {
                    return result;
                }
            }
        }
        //##############################################################################################################################################################################################################################################################


    }
}
