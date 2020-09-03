﻿using AL.NetFrame.Attributes;
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
using WebCore.ENM;

namespace WebCore.Services
{
    public interface IUserService : IEntityService<DbConnection> { }
    public class UserService : EntityService<DbConnection>, IUserService
    {

        public UserService() : base() { }
        public UserService(System.Data.IDbConnection db) : base(db) { }
        //###############################################################################################################################
        public ActionResult Login(LoginReqestModel model)
        {
            using (var services = new UserService())
            {
                if (model == null)
                    return Notifization.Invalid(MessageText.Invalid);
                //
                string loginId = model.LoginID;
                string password = model.Password;
                if (string.IsNullOrWhiteSpace(loginId) || string.IsNullOrWhiteSpace(password))
                    return Notifization.Invalid(MessageText.Invalid);
                //   
                var sqlHelper = DbConnect.Connection.CMS;
                string sqlQuerry = @"SELECT TOP 1 *,IsCMSUser = 1 FROM View_CMSUserLogin WHERE LoginID = @LoginID AND Password = @Password
                                 UNION 
                                 SELECT TOP 1 *,IsCMSUser = 0 FROM View_UserLogin WHERE LoginID = @LoginID AND Password = @Password";
                var login = sqlHelper.Query<Logged>(sqlQuerry, new { model.LoginID, Password = Helper.Security.Library.Encryption256(password) }).FirstOrDefault();
                //
                if (login == null)
                    return Notifization.Error("Sai thông tin tài khoản hoặc mật khẩu");
                //
                if (login.Enabled != (int)ModelEnum.Enabled.ENABLED)
                    return Notifization.Error("Tài khoản chưa được kích hoạt");
                //
                if (login.IsBlock)
                    return Notifization.Error("Tài khoản của bạn bị khóa");
                //
                HttpContext.Current.Session["IdentifyID"] = login.ID;
                HttpContext.Current.Session["LoginID"] = login.LoginID;

                // delete cookies
                HttpContext.Current.Response.Cookies[FormsAuthentication.FormsCookieName].Expires = DateTime.Now.AddDays(-1);
                // set cooki
                string _uData = "{'LoginID': '', 'Password': '', 'IsRemember': 0} ";
                if (model.IsRemember)
                    _uData = "{'LoginID': '" + model.LoginID + "', 'Password': '" + model.Password + "', 'IsRemember': 1} ";

                bool isPersistent = false;
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
                    model.LoginID,
                    DateTime.Now,
                    DateTime.Now.AddDays(10),
                    isPersistent,
                    _uData,
                    FormsAuthentication.FormsCookiePath
                );
                // Encrypt the ticket.
                string encTicket = FormsAuthentication.Encrypt(ticket);
                // Create the cookie.
                HttpContext.Current.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));

                //navigate by account type
                // get pre-page 


                string prePath = Helper.Page.Navigate.NavigateByParam(model.Url);
                // Development
                if (login.IsCMSUser)
                {
                    // check permission
                    // something here

                    // default
                    if (string.IsNullOrWhiteSpace(prePath))
                        prePath = "/Development/Home/index";
                    //retun
                    prePath = prePath.ToLower();
                    return Notifization.Success("Đăng nhập thành công", prePath);
                }
                // Application
                // check permission
                // something here

                // default
                if (string.IsNullOrWhiteSpace(prePath))
                    prePath = "/Management/Home/Index";
                //retun
                prePath = prePath.ToLower();
                return Notifization.Success("Đăng nhập thành công", prePath);
            }
        }

        public ActionResult LoginQR(LoginQRReqestModel model)
        {
            if (model == null)
                return Notifization.Invalid();
            //
            string loginId = model.LoginID;
            string pinCode = model.PinCode;
            if (string.IsNullOrWhiteSpace(loginId) && string.IsNullOrWhiteSpace(pinCode))
                return Notifization.Invalid();
            //   
            var sqlHelper = DbConnect.Connection.CMS;
            string sqlQuerry = @"SELECT TOP 1 *,IsCMSUser = 1 FROM View_CMSUserLogin WHERE LoginID = @LoginID AND PinCode = @PinCode
                                     UNION 
                                     SELECT TOP 1 *,IsCMSUser = 0 FROM View_UserLogin WHERE LoginID = @LoginID AND PinCode = @PinCode";
            var login = sqlHelper.Query<Logged>(sqlQuerry, new { model.LoginID, PinCode = Helper.Security.Library.Encryption256(pinCode) }).FirstOrDefault();
            //
            if (login == null)
                return Notifization.Error("Sai thông tin tài khoản hoặc mật khẩu");
            //
            if (login.Enabled != (int)ModelEnum.Enabled.ENABLED)
                return Notifization.Error("Tài khoản chưa được kích hoạt");
            //
            if (login.IsBlock)
                return Notifization.Error("Tài khoản của bạn bị khóa");
            //
            //HttpContext.Current.Session["IdentifyID"] = login.ID;
            //HttpContext.Current.Session["LoginID"] = login.LoginID;
            // delete cookies
            //HttpContext.Current.Response.Cookies[FormsAuthentication.FormsCookieName].Expires = DateTime.Now.AddDays(-1);
            //// set cooki
            //string _uData = "{'LoginID': '', 'Password': '', 'IsRemember': 0} ";
            //if (model.IsRemember)
            //    _uData = "{'LoginID': '" + model.LoginID + "', 'Password': '" + model.Pu + "', 'IsRemember': 1} ";
            //bool isPersistent = false;
            //FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
            //    model.LoginID,
            //    DateTime.Now,
            //    DateTime.Now.AddDays(10),
            //    isPersistent,
            //    _uData,
            //    FormsAuthentication.FormsCookiePath
            //);
            // Encrypt the ticket.
            //string encTicket = FormsAuthentication.Encrypt(ticket);
            // Create the cookie.
            //HttpContext.Current.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
            //navigate by account type
            // get pre-page 
            string prePath = Helper.Page.Navigate.NavigateByParam(model.Url);
            // Development
            if (login.IsCMSUser)
            {
                // check permission
                // something here
                // default
                if (string.IsNullOrWhiteSpace(prePath))
                    prePath = "/Development/Home/index";
                //retun
                prePath = prePath.ToLower();
                return Notifization.Success("Đăng nhập thành công", prePath);
            }
            // Application
            // check permission
            // something here

            // default
            if (string.IsNullOrWhiteSpace(prePath))
                prePath = "/Management/Home/Index";
            //retun
            prePath = prePath.ToLower();
            return Notifization.Success("Đăng nhập thành công", prePath);

        }

        public Logged LoggedModel(string loginId)
        {
            try
            {
                using (var service = new UserService())
                {
                    string sqlQuerry = @"SELECT TOP 1 *,IsCMSUser = 1 FROM View_CMSUserLogin WHERE LoginID = @LoginID 
                                         UNION 
                                         SELECT TOP 1 *,IsCMSUser = 0 FROM View_UserLogin WHERE LoginID = @LoginID ";
                    var logged = service.Query<Logged>(sqlQuerry, new { LoginID = loginId }).FirstOrDefault();
                    if (logged == null)
                        return null;
                    //
                    return logged;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }




        //###############################################################################################################################
        public ActionResult Datalist(SearchModel model)
        {
            if (model == null)
                return Notifization.Invalid();
            //
            string query = string.Empty;
            if (string.IsNullOrWhiteSpace(model.Query))
                query = "";
            else
                query = model.Query;
            //
            int page = model.Page;
            //
            string langID = Helper.Current.UserLogin.LanguageID;
            using (var service = new UserService(_connection))
            {
                string sqlQuery = @"SELECT * FROM View_User WHERE dbo.Uni2NONE(FullName) LIKE N'%'+ dbo.Uni2NONE(@Query) +'%' ORDER BY FullName,CreatedDate";
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
                if (result.Count <= 0)
                    return Notifization.NotFound(MessageText.NotFound);
                //
                Helper.Pagination.PagingModel pagingModel = new Helper.Pagination.PagingModel
                {
                    PageSize = Helper.Pagination.Paging.PAGESIZE,
                    Total = dtList.Count,
                    Page = page
                };
                Helper.Model.RoleAccountModel roleAccountModel = new Helper.Model.RoleAccountModel
                {
                    Create = true,
                    Update = true,
                    Details = true,
                    Delete = true,
                    Block = true,
                    Active = true,
                };
                return Notifization.Data(MessageText.Success, data: result, role: roleAccountModel, paging: pagingModel);
            }
        }

        //##############################################################################################################################################################################################################################################################

        public ActionResult Create(UserCreateModel model)
        {
            using (var service = new UserService())
            {
                _connection.Open();
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
                        var userLogin = _connection.Query<UserIDModel>(sqlQuery, new { LoginID = loginId }, transaction: transaction).FirstOrDefault();
                        if (userLogin != null)
                            return Notifization.Error("Tài khoản đã được sử dụng");
                        //
                        sqlQuery = @"SELECT TOP(1) ID FROM View_CMSUser WHERE Email = @Email 
                                  UNION 
                                  SELECT TOP(1) ID FROM View_User WHERE Email = @Email ";
                        //
                        userLogin = _connection.Query<UserIDModel>(sqlQuery, new { Email = email }, transaction: transaction).FirstOrDefault();
                        if (userLogin != null)
                            return Notifization.Error("Địa chỉ email đã được sử dụng");
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
                            LoginID = loginId,
                            Password = Helper.Security.Library.Encryption256(model.Password),
                            PinCode = null,
                            TokenID = null,
                            OTPCode = null,
                            IsAdministrator = true
                        }, transaction: transaction);
                        //
                        userInfoService.Create<string>(new UserInfo()
                        {
                            UserID = userId,
                            ImageFile = imageFile,
                            FullName = model.FullName,
                            Birthday = Helper.Time.TimeHelper.FormatToDateSQL(model.Birthday),
                            Email = model.Email.ToLower(),
                            Phone = model.Phone,
                            Address = model.Address
                        }, transaction: transaction);
                        //
                        userSettingService.Create<string>(new UserSetting()
                        {
                            UserID = userId,
                            SecurityPassword = null,
                            AuthenType = null,
                            RoleID = null,
                            IsBlock = isBlock,
                            Enabled = enabled,
                            LanguageID = languageId,
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
            using (var service = new UserService())
            {
                _connection.Open();
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
                        bool isBlock = model.IsBlock;
                        int enabled = model.Enabled;
                        //
                        if (string.IsNullOrWhiteSpace(id))
                            return Notifization.Invalid(MessageText.Invalid);
                        id = id.Trim();
                        //  email
                        if (string.IsNullOrWhiteSpace(email))
                            return Notifization.Invalid("Không được để trống địa chỉ email");
                        if (!Validate.TestEmail(email))
                            return Notifization.Invalid("Địa chỉ email không hợp lệ");
                        //
                        string sqlQuery = @"SELECT TOP(1) ID FROM View_CMSUser WHERE Email = @Email AND ID !=@ID 
                                  UNION 
                                  SELECT TOP(1) ID FROM View_User WHERE Email = @Email AND ID !=@ID ";
                        //
                        var userEmail = _connection.Query<UserIDModel>(sqlQuery, new { Email = email, ID = id }, transaction: _transaction).FirstOrDefault();
                        if (userEmail != null)
                            return Notifization.Error("Địa chỉ email đã được sử dụng");
                        //
                        UserLoginService userLoginService = new UserLoginService(_connection);
                        UserInfoService userInfoService = new UserInfoService(_connection);
                        UserSettingService userSettingService = new UserSettingService(_connection);
                        LanguageService languageService = new LanguageService(_connection);
                        //update user information
                        var userLogin = userLoginService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.ID.ToLower().Equals(id.ToLower()), transaction: _transaction).FirstOrDefault();
                        if (userLogin == null)
                            return Notifization.Invalid(MessageText.Invalid + id);
                        //
                        var _uId = userLogin.ID;
                        var userInfo = userInfoService.GetAlls(m => !string.IsNullOrWhiteSpace(m.UserID) && m.UserID.ToLower().Equals(_uId.ToLower()), transaction: _transaction).FirstOrDefault();
                        if (userInfo == null)
                            return Notifization.Invalid(MessageText.Invalid + id);
                        //
                        userInfo.ImageFile = imageFile;
                        userInfo.FullName = fullName;
                        userInfo.NickName = model.NickName;
                        userInfo.Birthday = Helper.Time.TimeHelper.FormatToDateSQL(model.Birthday);
                        userInfo.Email = model.Email.ToLower();
                        userInfo.Phone = model.Phone;
                        userInfo.Address = model.Address;
                        userInfoService.Update(userInfo, transaction: _transaction);
                        //
                        var userSetting = userSettingService.GetAlls(m => !string.IsNullOrWhiteSpace(m.UserID) && m.UserID.ToLower().Equals(_uId.ToLower()), transaction: _transaction).FirstOrDefault();
                        if (userSetting == null)
                            return Notifization.Invalid(MessageText.Invalid);
                        //
                        //userSetting.AreaID = areaId;
                        //userSetting.IsRootUser 
                        userSetting.IsBlock = isBlock;
                        userSetting.Enabled = enabled;
                        userSetting.LanguageID = model.LanguageID;
                        userSettingService.Update(userSetting, transaction: _transaction);
                        //
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
            using (var service = new UserService())
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

        public UserModel GetUserModel(string id)
        {
            try
            {
                using (var service = new UserService())
                {
                    if (string.IsNullOrWhiteSpace(id))
                        return null;
                    //
                    string sqlQuery = @"SELECT TOP (1) * FROM View_User WHERE ID = @ID";
                    var data = service.Query<UserModel>(sqlQuery, new { ID = id }).FirstOrDefault();
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
            UserLoginService userLoginService = new UserLoginService();
            var userLogin = userLoginService.GetAlls(m => m.ID.Equals(loginId)).FirstOrDefault();
            if (userLogin == null)
                return Notifization.NotFound(MessageText.NotFound);
            //
            if (!userLogin.Password.Equals(passId))
                return Notifization.NotFound("Mật khẩu cũ chưa đúng");
            // update
            userLogin.Password = Helper.Security.Library.Encryption256(model.NewPassword);
            userLoginService.Update(userLogin);
            return Notifization.Success(MessageText.UpdateSuccess);
        }
        //##############################################################################################################################################################################################################################################################

        public static string GetLoginIDByID(string id)
        {
            try
            {
                using (var service = new UserService())
                {
                    if (string.IsNullOrWhiteSpace(id))
                        return string.Empty;
                    //
                    string sqlQuery = @"SELECT TOP (1) * FROM View_User WHERE ID = @ID";
                    var data = service.Query<UserModel>(sqlQuery, new { ID = id }).FirstOrDefault();
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

        //FUNTION FOR CURRENT ##############################################################################################################################################################################################################################################################
        public LoginInForModel LoginInformation(string loginId)
        {
            var userId = loginId; // get lai =  userID
            using (var service = new UserService())
            {
                string sqlQuerry = @"SELECT TOP (1) * FROM View_CMSUserInfo WHERE UserID = @UserID                                         
                                     UNION SELECT TOP (1) * FROM View_UserInfo WHERE UserID = @UserID";
                var loginModel = service.Query<LoginInForModel>(sqlQuerry, new { UserID = userId }).FirstOrDefault();
                if (loginModel != null)
                    return loginModel;
                //
                return null;
            }
        }
        public bool IsLogin()
        {
            try
            {
                HttpCookie authCookie = HttpContext.Current.Request.Cookies[System.Web.Security.FormsAuthentication.FormsCookieName];
                if (authCookie != null)
                    return true;
                return false;
            }
            catch
            {
                return false;
            }
        }
        public string GetIdentifierID()
        {
            string loginId = Convert.ToString(HttpContext.Current.Session["identifyId"]);
            if (!string.IsNullOrWhiteSpace(loginId))
                return loginId;
            return string.Empty;
        }
        public string LoginID()
        {
            string identifyId = Convert.ToString(HttpContext.Current.Session["LoginID"]);
            if (!string.IsNullOrWhiteSpace(identifyId))
                return identifyId.ToLower();
            return string.Empty;
        }
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
                    var userModal = service.GetAlls(m => m.UserID.ToLower().Equals(userId)).FirstOrDefault();
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
        public bool IsClientLogged(string userId, IDbConnection dbConnection = null, IDbTransaction dbTransaction = null)
        {
            try
            {
                if (dbConnection == null)
                    dbConnection = DbConnect.Connection.CMS;
                //
                var service = new ClientLoginService(dbConnection);
                if (string.IsNullOrWhiteSpace(userId))
                    return false;
                var client = service.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.UserID.ToLower().Equals(userId.ToLower()), dbTransaction).FirstOrDefault();
                if (client == null)
                    return false;
                //
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool IsCustomerLogged(string userId, IDbConnection dbConnection = null, IDbTransaction dbTransaction = null)
        {
            try
            {
                if (dbConnection == null)
                    dbConnection = DbConnect.Connection.CMS;
                //
                var service = new ClientLoginService(dbConnection);
                if (string.IsNullOrWhiteSpace(userId))
                    return false;
                //
                var client = service.GetAlls(m => !string.IsNullOrWhiteSpace(m.UserID) && m.UserID.ToLower().Equals(userId.ToLower()) && m.ClientType == (int)ClientLoginEnum.ClientType.Customer, dbTransaction).FirstOrDefault();
                if (client == null)
                    return false;
                //
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool IsSuperCustomerLogged(string userId, IDbConnection dbConnection = null, IDbTransaction dbTransaction = null)
        {
            try
            {
                if (dbConnection == null)
                    dbConnection = this._connection;
                //
                var service = new ClientLoginService(dbConnection);
                if (string.IsNullOrWhiteSpace(userId))
                    return false;
                //
                var client = service.GetAlls(m => !string.IsNullOrWhiteSpace(m.UserID) && m.UserID.ToLower().Equals(userId.ToLower()) && m.ClientType == (int)ClientLoginEnum.ClientType.Customer, dbTransaction).FirstOrDefault();
                if (client == null)
                    return false;

                if (client.IsSuper)
                    return true;
                //
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool IsSupplierLogged(string userId, IDbConnection dbConnection = null, IDbTransaction dbTransaction = null)
        {
            try
            {
                var service = new ClientLoginService(dbConnection);
                if (string.IsNullOrWhiteSpace(userId))
                    return false;
                var client = service.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.UserID.ToLower().Equals(userId.ToLower()) && m.ClientType == (int)ClientLoginEnum.ClientType.Supplier, dbTransaction).FirstOrDefault();
                if (client == null)
                    return false;
                //
                return true;

            }
            catch
            {
                return false;
            }
        }
    }
}
