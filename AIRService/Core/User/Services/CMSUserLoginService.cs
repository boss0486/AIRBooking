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

namespace WebCore.Services
{
    public interface ICMSUserLoginService : IEntityService<CMSUserLogin> { }
    public class CMSUserLoginService : EntityService<CMSUserLogin>, ICMSUserLoginService
    {
        public CMSUserLoginService() : base() { }
        public CMSUserLoginService(System.Data.IDbConnection db) : base(db) { }

        //##############################################################################################################################################################################################################################################################
        //public ActionResult Datalist(string strQuery, int page)
        //{
        //    string query = string.Empty;
        //    if (string.IsNullOrWhiteSpace(strQuery))
        //        query = "";
        //    else
        //        query = strQuery;
        //    //
        //    string langID = Helper.Current.UserLogin.LanguageID;
        //    string sqlQuery = @"SELECT * FROM View_CMSUser  
        //                        WHERE dbo.Uni2NONE(FirstName) LIKE N'%'+ dbo.Uni2NONE(@Query) +'%' OR dbo.Uni2NONE(LastName) LIKE N'%'+ dbo.Uni2NONE(@Query) +'%'
        //                        ORDER BY [CreatedDate]";
        //    var dtList = _connection.Query<CMSUserResult>(sqlQuery, new { Query = query }).ToList();
        //    if (dtList.Count == 0)
        //        return Notifization.NotFound(MessageText.NotFound);
        //    //
        //    var result = dtList.ToPagedList(page, Helper.Pagination.Paging.PAGESIZE).ToList();
        //    if (result.Count <= 0 && page > 1)
        //    {
        //        page -= 1;
        //        result = dtList.ToPagedList(page, Helper.Pagination.Paging.PAGESIZE).ToList();
        //    }
        //    if (result.Count <= 0)
        //        return Notifization.NotFound(MessageText.NotFound);
        //    //
        //    Helper.Pagination.PagingModel pagingModel = new Helper.Pagination.PagingModel
        //    {
        //        PageSize = Helper.Pagination.Paging.PAGESIZE,
        //        Total = dtList.Count,
        //        Page = page
        //    };
        //    Helper.Model.RoleAccountModel roleAccountModel = new Helper.Model.RoleAccountModel
        //    {
        //        Create = true,
        //        Update = true,
        //        Details = true,
        //        Delete = true,
        //        Block = true,
        //        Active = true,
        //    };
        //    return Notifization.Data(MessageText.Success, data: result, role: roleAccountModel, paging: pagingModel);
        //}
        ////##############################################################################################################################################################################################################################################################
        //public ActionResult Create(UserCreateModel model)
        //{
        //    _connection.Open();
        //    using (var transaction = _connection.BeginTransaction())
        //    {
        //        try
        //        {
        //            string fullName = model.FullName;
        //            string nickName = model.NickName;
        //            string email = model.Email;
        //            //
        //            string loginId = model.LoginID;
        //            string password = model.Password;
        //            string rePass = model.RePassword;
        //            //
        //            string langID = model.LanguageID;
        //            int Enabled = model.Enabled;
        //            // firt name valid
        //            if (string.IsNullOrWhiteSpace(fullName))
        //                return Notifization.Invalid("Không được để trống họ/tên");
        //            if (fullName.Length < 2 || fullName.Length > 30)
        //                return Notifization.Invalid("Họ/tên giới hạn [2-30] ký tự");
        //            if (!Validate.TestAlphabet(fullName))
        //                return Notifization.Invalid("Họ/tên không hợp lệ");

        //            if (string.IsNullOrWhiteSpace(fullName))
        //                return Notifization.Invalid("Không được để trống họ/tên");
        //            if (fullName.Length < 2 || fullName.Length > 30)
        //                return Notifization.Invalid("Họ tên giới hạn [2-30] ký tự");
        //            if (!Validate.TestAlphabet(fullName))
        //                return Notifization.Invalid("Họ tên không hợp lệ");
        //            // nick name
        //            if (!string.IsNullOrWhiteSpace(nickName))
        //            {
        //                if (nickName.Length < 2 || nickName.Length > 30)
        //                    return Notifization.Invalid("Biệt danh giới hạn [2-30] ký tự");
        //                if (!Validate.TestAlphabet(nickName))
        //                    return Notifization.Invalid("Biệt danh không hợp lệ");
        //            }
        //            // birthday
        //            string birthday = model.Birthday;
        //            if (!string.IsNullOrWhiteSpace(birthday))
        //            {
        //                if (!Validate.TestDateVN(birthday))
        //                    return Notifization.Invalid("Ngày sinh không hợp lệ");
        //            }
        //            else
        //            {
        //                birthday = String.Format("{0:dd-MM-yyyy}", DateTime.Now);
        //            }
        //            //  email
        //            if (string.IsNullOrWhiteSpace(email))
        //                return Notifization.Invalid("Không được để trống địa chỉ email");
        //            // 
        //            email = email.Trim();
        //            if (!Validate.TestEmail(email))
        //                return Notifization.Invalid("Địa chỉ email không hợp lệ");
        //            // phone number
        //            string phone = model.Phone;
        //            if (!string.IsNullOrWhiteSpace(phone))
        //            {
        //                if (!Validate.TestPhone(phone))
        //                    return Notifization.Invalid("Số điện thoại không hợp lệ");
        //            }
        //            string address = model.Address;
        //            if (!string.IsNullOrWhiteSpace(address))
        //            {
        //                if (!Validate.TestAlphabet(address))
        //                    return Notifization.Invalid("Địa chỉ không hợp lệ");
        //            }
        //            // account
        //            if (string.IsNullOrWhiteSpace(loginId))
        //                return Notifization.Invalid("Không được để trống tài khoản");
        //            //
        //            loginId = loginId.Trim();
        //            if (!Validate.TestUserName(loginId))
        //                return Notifization.Invalid("Tài khoản không hợp lệ");
        //            if (loginId.Length < 4 || loginId.Length > 16)
        //                return Notifization.Invalid("Tài khoản giới hạn [4-16] ký tự");
        //            // password
        //            if (string.IsNullOrWhiteSpace(password))
        //                return Notifization.Invalid("Không được để trống mật khẩu");
        //            if (!Validate.TestPassword(password))
        //                return Notifization.Invalid("Yêu cầu mật khẩu bảo mật hơn");
        //            if (password.Length < 4 || password.Length > 16)
        //                return Notifization.Invalid("Mật khẩu giới hạn [4-16] ký tự");
        //            //
        //            //if (string.IsNullOrWhiteSpace(langID))
        //            //    return Notifization.Invalid("Vui lòng chọn ngôn ngữ");
        //            // call service
        //            CMSUserLoginService cmsUserLoginService = new CMSUserLoginService(_connection);
        //            CMSUserInfoService cmsUserInfoService = new CMSUserInfoService(_connection);
        //            CMSUserSettingService cmsUserSettingService = new CMSUserSettingService(_connection);
        //            LanguageService languageService = new LanguageService(_connection);

        //            string sqlQuerry = @"SELECT TOP (1) ID FROM View_CMSUserLogin WHERE LoginID = @LoginID 
        //                                 UNION 
        //                                 SELECT TOP (1) ID FROM View_UserLogin WHERE LoginID = @LoginID ";

        //            var userLogin = _connection.Query<UserIDModel>(sqlQuerry, new { LoginID = loginId }, transaction: transaction).FirstOrDefault();
        //            if (userLogin != null)
        //                return Notifization.Error("Tài khoản đã được sử dụng");


        //            sqlQuerry = @"SELECT TOP (1) ID FROM View_CMSUserLogin WHERE Email = @Email 
        //                          UNION 
        //                          SELECT TOP (1) ID FROM View_UserLogin WHERE Email = @Email ";

        //            userLogin = _connection.Query<UserIDModel>(sqlQuerry, new { Email = email }, transaction: transaction).FirstOrDefault();
        //            if (userLogin != null)
        //                return Notifization.Error("Địa chỉ email đã được sử dụng");
        //            //
        //            //var lang = languageService.GetAlls(m => m.LanguageID == model.LanguageID, transaction: transaction).FirstOrDefault();
        //            //if (lang == null)
        //            //    return Notifization.NOTFOUND("Ngôn ngữ không hợp lệ");
        //            //
        //            string languageID = Helper.Page.Default.LanguageID;
        //            //  create user
        //            string userId = cmsUserLoginService.Create<string>(new CMSUserLogin()
        //            {
        //                LoginID = loginId,
        //                Password = Helper.Security.Library.Encryption256(model.Password),
        //                TokenID = null,
        //                OTPCode = null
        //            }, transaction: transaction);
        //            //
        //            cmsUserInfoService.Create<string>(new CMSUserInfo()
        //            {
        //                UserID = userId,
        //                ImageFile = "",
        //                FullName = model.FullName,
        //                Birthday = Helper.Page.Library.FormatToDateSQL(model.Birthday),
        //                Email = model.Email.ToLower(),
        //                Phone = model.Phone,
        //                Address = model.Address
        //            }, transaction: transaction);
        //            //
        //            string indentity = string.Empty;
        //            //
        //            cmsUserSettingService.Create<string>(new CMSUserSetting()
        //            {
        //                UserID = userId,
        //                SecurityPassword = null,
        //                AuthenType = null,
        //                IsBlock = false,
        //                Enabled = model.Enabled,
        //                LanguageID = languageID,
        //            }, transaction: transaction);
        //            //
        //            transaction.Commit();
        //            return Notifization.Success(MessageText.CreateSuccess);
        //        }
        //        catch (Exception ex)
        //        {
        //            transaction.Rollback();
        //            return Notifization.NotService;
        //        }
        //    }
        //}
        ////##############################################################################################################################################################################################################################################################
        //public ActionResult Update(UserUpdateModel model)
        //{
        //    _connection.Open();
        //    using (var _transaction = _connection.BeginTransaction())
        //    {
        //        try
        //        {
        //            CMSUserLoginService cmsUserLoginService = new CMSUserLoginService(_connection);
        //            CMSUserInfoService cmsUserInfoService = new CMSUserInfoService(_connection);
        //            CMSUserSettingService cmsUserSettingService = new CMSUserSettingService(_connection);
        //            LanguageService languageService = new LanguageService(_connection);
        //            string Id = model.ID.ToLower();
        //            var logIn = cmsUserLoginService.GetAlls(m => m.ID.Equals(Id), transaction: _transaction).FirstOrDefault();
        //            if (logIn == null)
        //                return Notifization.NotFound(MessageText.NotFound);
        //            //
        //            string loginID = model.LoginID;

        //            var userLogin = cmsUserLoginService.GetAlls(m => m.LoginID.Equals(loginID) && !m.ID.Equals(Id), transaction: _transaction);
        //            if (userLogin.Count > 0)
        //                return Notifization.Error("Tài khoản đã được sử dụng");
        //            //
        //            var cmsUserInfo = cmsUserInfoService.GetAlls(m => m.UserID == logIn.ID, transaction: _transaction).FirstOrDefault();
        //            if (cmsUserInfo == null)
        //                return Notifization.NotFound(MessageText.NotFound);

        //            var userEmail = cmsUserInfoService.GetAlls(m => m.Email.ToLower().Equals(model.Email.ToLower()) && !m.UserID.ToLower().Equals(Id), transaction: _transaction).FirstOrDefault();
        //            if (userEmail != null)
        //                return Notifization.Error("Địa chỉ email đã được sử dụng");

        //            if (!string.IsNullOrWhiteSpace(model.LanguageID))
        //            {
        //                var lang = languageService.GetAlls(m => m.LanguageID.ToLower().Equals(model.LanguageID.ToLower()), transaction: _transaction).FirstOrDefault();
        //                if (lang == null)
        //                    return Notifization.NotFound(MessageText.NotFound);
        //            }
        //            //
        //            string fileId = cmsUserInfo.ImageFile;
        //            // update user login
        //            logIn = cmsUserLoginService.GetAlls(m => m.ID.Equals(Id), transaction: _transaction).FirstOrDefault();
        //            logIn.LoginID = model.LoginID;
        //            cmsUserLoginService.Update(logIn, transaction: _transaction);
        //            //update user information
        //            cmsUserInfo.ImageFile = fileId;
        //            cmsUserInfo.FullName = model.FullName;
        //            cmsUserInfo.NickName = model.NickName;
        //            cmsUserInfo.Birthday = Library.FormatToDateSQL(model.Birthday);
        //            cmsUserInfo.Email = model.Email.ToLower();
        //            cmsUserInfo.Phone = model.Phone;
        //            cmsUserInfo.Address = model.Address;
        //            cmsUserInfoService.Update(cmsUserInfo, transaction: _transaction);
        //            //
        //            var cmsUserSetting = cmsUserSettingService.GetAlls(m => m.UserID.ToLower().Equals(logIn.ID.ToLower()), transaction: _transaction).FirstOrDefault();
        //            if (cmsUserSetting == null)
        //                return Notifization.Invalid(MessageText.Invalid);
        //            //
        //            cmsUserSetting.LanguageID = model.LanguageID;
        //            cmsUserSettingService.Update(cmsUserSetting, transaction: _transaction);
        //            //
        //            _transaction.Commit();
        //            return Notifization.Success(MessageText.UpdateSuccess);
        //        }
        //        catch (Exception ex)
        //        {
        //            _transaction.Rollback();
        //            return Notifization.NotService;
        //        }
        //    }
        //}
        //public CMSUserResult GetUserModel(string id)
        //{
        //    try
        //    {
        //        var userViewModal = new CMSUserResult();
        //        if (string.IsNullOrWhiteSpace(id))
        //            return userViewModal;
        //        //
        //        string sqlQuery = sqlQuery = @"SELECT TOP 1 * FROM View_CMSUser WHERE ID = @ID";
        //        userViewModal = _connection.Query<CMSUserResult>(sqlQuery, new { ID = id }).FirstOrDefault();
        //        return userViewModal;
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}
        ////##############################################################################################################################################################################################################################################################
        //public ActionResult Delete(string id)
        //{
        //    _connection.Open();
        //    using (var transaction = _connection.BeginTransaction())
        //    {
        //        try
        //        {
        //            if (string.IsNullOrWhiteSpace(id))
        //                return Notifization.Invalid(MessageText.Invalid);
        //            //
        //            string sqlQuery = sqlQuery = @"SELECT TOP 1 * FROM View_CMSUser WHERE ID = @ID";
        //            var userViewModal = _connection.Query<CMSUserResult>(sqlQuery, new { ID = id }).FirstOrDefault();
        //            if (userViewModal == null)
        //                return Notifization.Error(MessageText.Invalid);
        //            // delete
        //            AttachmentFile.DeleteFile(userViewModal.ImageFile, transaction: transaction);
        //            _connection.Execute("DELETE CMSUserInfo WHERE UserID = @UserID", new { UserID = id }, transaction: transaction);
        //            _connection.Execute("DELETE CMSUserSetting WHERE UserID = @UserID", new { UserID = id }, transaction: transaction);
        //            _connection.Execute("DELETE CMSUserLogin WHERE ID = @ID", new { ID = id }, transaction: transaction);
        //            transaction.Commit();
        //            return Notifization.Success(MessageText.DeleteSuccess);
        //        }
        //        catch
        //        {
        //            transaction.Rollback();
        //            return Notifization.NotService;
        //        }
        //    }
        //}
        ////##############################################################################################################################################################################################################################################################

        ////##############################################################################################################################################################################################################################################################
        //public ActionResult ChangePinCode(UserChangePinCodeModel model)
        //{
        //    string loginId = Helper.Current.UserLogin.IdentifierID;
        //    if (string.IsNullOrWhiteSpace(loginId))
        //        return Notifization.Error("Bạn cần đăng nhập trước");
        //    //
        //    string pincode = Helper.Security.Library.Encryption256(model.PinCode);
        //    var cmsUserLoginService = new CMSUserLoginService(_connection);
        //    var cmsUserLogin = cmsUserLoginService.GetAlls(m => m.ID.Equals(loginId)).FirstOrDefault();
        //    if (cmsUserLogin == null)
        //        return Notifization.NotFound(MessageText.NotFound);
        //    //
        //    if (!cmsUserLogin.PinCode.ToLower().Equals(pincode))
        //        return Notifization.NotFound("Mã pin chưa đúng");
        //    // update
        //    cmsUserLogin.PinCode = Helper.Security.Library.Encryption256(model.PinCode);
        //    cmsUserLoginService.Update(cmsUserLogin);
        //    return Notifization.Success(MessageText.UpdateSuccess);
        //}

        ////##############################################################################################################################################################################################################################################################
        //public string CreateID(System.Data.IDbConnection _connectDb, System.Data.IDbTransaction transaction = null)
        //{
        //    CMSUserLoginService cmsUserLoginService = new CMSUserLoginService(_connectDb);
        //    string result = string.Empty;
        //    int counter = 1;
        //    do
        //    {
        //        result = new Guid().ToString();
        //        var accountID = cmsUserLoginService.Get(result, transaction: transaction);
        //        if (accountID == null)
        //            return result;
        //        else
        //            result = string.Empty;
        //        counter++;
        //    } while (result == string.Empty && counter < 20);
        //    return result;
        //}
        //public string CreateID()
        //{
        //    CMSUserLoginService cmsUserLoginService = new CMSUserLoginService(_connection);
        //    string result = string.Empty;
        //    int counter = 1;
        //    do
        //    {
        //        result = new Guid().ToString();
        //        var accountID = cmsUserLoginService.Get(result);
        //        if (accountID == null)
        //            return result;
        //        else
        //            result = string.Empty;
        //        counter++;
        //    } while (result == string.Empty && counter < 20);
        //    return result;
        //}
        ////##############################################################################################################################################################################################################################################################
        //public string GetLanguageID
        //{
        //    get
        //    {
        //        string result = Helper.Page.Default.LanguageID;
        //        try
        //        {
        //            string userId = Helper.Current.UserLogin.IdentifierID;
        //            if (string.IsNullOrWhiteSpace(userId))
        //                return result;
        //            //
        //            CMSUserSettingService service = new CMSUserSettingService();
        //            var userModal = service.GetAlls(m => m.UserID.ToLower().Equals(userId)).FirstOrDefault();
        //            if (userModal == null)
        //                return result;
        //            //
        //            return userModal.LanguageID;
        //        }
        //        catch
        //        {
        //            return result;
        //        }
        //    }
        //}
    }
}