using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using WebCore.Entities;
using System;
using Dapper;
using System.Linq;
using PagedList;
using Helper;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Globalization;
using CMSCore.Entities;
using WebCore.Core;
using CMSCore.Services;

namespace WebCore.Services
{
    public interface IUserLoginService : IEntityService<UserLogin> { }
    public class UserLoginService : EntityService<UserLogin>, IUserLoginService
    {
        public UserLoginService() : base() { }
        public UserLoginService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Datalist(string strQuery, int page)
        {
            string query = string.Empty;
            if (string.IsNullOrWhiteSpace(strQuery))
                query = "";
            else
                query = strQuery;
            //
            string langID = Current.LanguageID;
            string sqlQuery = @"SELECT 
                                      *
                                     FROM View_User  
                                     WHERE dbo.Uni2NONE(FirstName) LIKE N'%'+ dbo.Uni2NONE(@Query) +'%' OR dbo.Uni2NONE(LastName) LIKE N'%'+ dbo.Uni2NONE(@Query) +'%'
                                     ORDER BY [CreatedDate]
                                    ";
            var dtList = _connection.Query<UserViewModal>(sqlQuery, new { Query = query }).ToList();
            if (dtList.Count <= 0)
                return Notifization.NotFound(NotifizationText.NotFound);
            List<RsUserModel> resultData = new List<RsUserModel>();
            foreach (var item in dtList)
            {
                string _workshitName = string.Empty;
                resultData.Add(new RsUserModel(item.ID, item.LoginID, item.ImageFile, item.FirstName, item.LastName, item.Nickname, item.Birthday, item.Email, item.Phone, item.Address, item.IdentifierID, item.TimekepingID, _workshitName, item.DepartmentID, item.DepartmentPartID, item.DepartmentName, item.DepartmentPartName, item.LanguageID, item.SiteID, item.CreatedBy, item.IsBlock, item.Enabled, item.CreatedDate));
            }
            //
            var result = resultData.ToPagedList(page, Library.Paging.PAGESIZE).ToList();
            if (result.Count <= 0 && page > 1)
            {
                page -= 1;
                result = resultData.ToPagedList(page, Library.Paging.PAGESIZE).ToList();
            }
            if (result.Count <= 0)
                return Notifization.NotFound(NotifizationText.NotFound);
            //
            Library.PagingModel pagingModel = new Library.PagingModel
            {
                PageSize = Library.Paging.PAGESIZE,
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
            return Notifization.DATALIST(NotifizationText.Success, data: result, role: roleAccountModel, paging: pagingModel);
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Create(UserCreateModel model)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    UserLoginService userLoginService = new UserLoginService(_connection);
                    UserInfoService userInfoService = new UserInfoService(_connection);
                    UserSettingService userSettingService = new UserSettingService(_connection);
                    LanguageService languageService = new LanguageService(_connection);
                    string loginID = model.LoginID;

                    var userLogin = userLoginService.GetAlls(m => m.LoginID.Equals(loginID), transaction: transaction);
                    if (userLogin.Count > 0)
                        return Notifization.ERROR("Tài khoản đã được sử dụng");
                    //
                    var userInfo = userInfoService.GetAlls(m => m.Email.ToLower().Equals(model.Email.ToLower()), transaction: transaction);
                    if (userInfo.Count > 0)
                        return Notifization.ERROR("Địa chỉ email đã được sử dụng");
                    //  check department

                    //
                    //var lang = languageService.GetAlls(m => m.LanguageID == model.LanguageID, transaction: transaction).FirstOrDefault();
                    //if (lang == null)
                    //    return Notifization.NOTFOUND("Ngôn ngữ không hợp lệ");
                    //
                    string languageID = Helper.Default.LanguageID;

                    HttpPostedFileBase file = model.DocumentFile;
                    string fileId = string.Empty;
                    if (file != null)
                    {
                        fileId = WebCore.AttachmentFile.SaveFile(file, null, null, true, 0, 0, transaction: transaction);
                        if (!Helper.Validate.FormatGuid(fileId) && !string.IsNullOrWhiteSpace(fileId))
                            return Notifization.NotFound(fileId);
                    }
                    if (string.IsNullOrWhiteSpace(fileId))
                        fileId = "no-image.gif";
                    //  create user
                    string userId = userLoginService.Create<string>(new UserLogin()
                    {
                        LoginID = loginID,
                        PassID = Security.Encryption256Hashed(model.PassID),
                        TokenID = null,
                        OTPCode = null
                    }, transaction: transaction);
                    //
                    userInfoService.Create<string>(new UserInfo()
                    {
                        UserID = userId,
                        ImageFile = fileId,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        NickName = model.NickName,
                        Birthday = Helper.Library.FormatSQLDate(model.Birthday),
                        Email = model.Email.ToLower(),
                        Phone = model.Phone,
                        Address = model.Address
                    }, transaction: transaction);
                    //
                    string indentity = string.Empty;
                    int strMax = 0;
                    string sqlMaxID = "SELECT MAX(REPLACE(IdentifierID,'ID','')) AS MaxID FROM [UserSetting]";
                    var userMax = _connection.Query<UserMaxIDModel>(sqlMaxID, transaction: transaction).FirstOrDefault();
                    if (userMax != null)
                        strMax = userMax.MaxID;

                    string smax = "100000";
                    int strLen = smax.Length - strMax.ToString().Length;
                    if (strLen <= 0)
                        return Notifization.ERROR("Nhân viên đã đạt giới hạn 100000");
                    for (int i = 0; i < strLen; i++)
                        indentity += "0";
                    //
                    userSettingService.Create<string>(new UserSetting()
                    {
                        UserID = userId,
                        IdentifierID = "ID" + indentity + (strMax + 1),
                        TimekepingID = string.Empty,
                        SecurityPassword = null,
                        AuthenticateID = null,
                        IsBlock = false,
                        Enabled = model.Enabled,
                        DepartmentID = model.DepartmentID,
                        DepartmentPartID = model.DepartmentPartID,
                        WorkShiftID = model.WorkShiftID,
                        LanguageID = languageID,
                    }, transaction: transaction);
                    //
                    transaction.Commit();
                    return Notifization.Success(NotifizationText.CREATE_SUCCESS);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Notifization.NotService;
                }
            }
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Update(UserUpdateModel model)
        {
            _connection.Open();
            using (var _transaction = _connection.BeginTransaction())
            {
                try
                {
                    UserLoginService userLoginService = new UserLoginService(_connection);
                    UserInfoService userInfoService = new UserInfoService(_connection);
                    UserSettingService userSettingService = new UserSettingService(_connection);
                    LanguageService languageService = new LanguageService(_connection);
                    string Id = model.ID.ToLower();
                    var logIn = userLoginService.GetAlls(m => m.ID.Equals(Id), transaction: _transaction).FirstOrDefault();
                    if (logIn == null)
                        return Notifization.NotFound(NotifizationText.NotFound);
                    //
                    string loginID = model.LoginID;

                    var userLogin = userLoginService.GetAlls(m => m.LoginID.Equals(loginID) && !m.ID.Equals(Id), transaction: _transaction);
                    if (userLogin.Count > 0)
                        return Notifization.ERROR("Tài khoản đã được sử dụng");
                    //
                    var userInfo = userInfoService.GetAlls(m => m.UserID == logIn.ID, transaction: _transaction).FirstOrDefault();
                    if (userInfo == null)
                        return Notifization.NotFound(NotifizationText.NotFound);

                    var userEmail = userInfoService.GetAlls(m => m.Email.ToLower().Equals(model.Email.ToLower()) && !m.UserID.ToLower().Equals(Id), transaction: _transaction).FirstOrDefault();
                    if (userEmail != null)
                        return Notifization.ERROR("Địa chỉ email đã được sử dụng");

                    if (!string.IsNullOrWhiteSpace(model.LanguageID))
                    {
                        var lang = languageService.GetAlls(m => m.LanguageID.ToLower().Equals(model.LanguageID.ToLower()), transaction: _transaction).FirstOrDefault();
                        if (lang == null)
                            return Notifization.NotFound(NotifizationText.NotFound);
                    }
                    //
                    HttpPostedFileBase file = model.DocumentFile;
                    string fileId = userInfo.ImageFile;
                    if (file != null)
                    {
                        // update file
                        fileId = WebCore.AttachmentFile.SaveFile(file, null, null, true, 0, 0, transaction: _transaction, connection: _connection);
                        if (!Helper.Validate.FormatGuid(fileId) && !string.IsNullOrWhiteSpace(fileId))
                            return Notifization.NotFound(fileId);
                        // delete file
                        var strDelete = WebCore.AttachmentFile.DeleteFile(userInfo.ImageFile, transaction: _transaction, connection: _connection);
                        if (!string.IsNullOrWhiteSpace(strDelete))
                            return Notifization.ERROR(strDelete);
                    }
                    // update user login
                    logIn = userLoginService.GetAlls(m => m.ID.Equals(Id), transaction: _transaction).FirstOrDefault();
                    logIn.LoginID = model.LoginID;
                    userLoginService.Update(logIn, transaction: _transaction);
                    //update user information
                    userInfo.ImageFile = fileId;
                    userInfo.FirstName = model.FirstName;
                    userInfo.LastName = model.LastName;
                    userInfo.NickName = model.NickName;
                    userInfo.Birthday = Helper.Library.FormatSQLDate(model.Birthday);
                    userInfo.Email = model.Email.ToLower();
                    userInfo.Phone = model.Phone;
                    userInfo.Address = model.Address;
                    userInfoService.Update(userInfo, transaction: _transaction);
                    var userSetting = userSettingService.GetAlls(m => m.UserID.ToLower().Equals(logIn.ID.ToLower()), transaction: _transaction).FirstOrDefault();
                    if (userSetting == null)
                        return Notifization.NotFound(NotifizationText.NotFound);

                    userSetting.DepartmentID = model.DepartmentID;
                    userSetting.DepartmentPartID = model.DepartmentPartID;
                    userSetting.LanguageID = model.LanguageID;
                    //userSetting.IdentifierID = model.IdentifierID;
                    userSetting.WorkShiftID = model.WorkShiftID;
                    userSettingService.Update(userSetting, transaction: _transaction);

                    _transaction.Commit();
                    return Notifization.Success(NotifizationText.UPDATE_SUCCESS);
                }
                catch (Exception ex)
                {
                    _transaction.Rollback();
                    return Notifization.NotService;
                }
            }
        }

        public ActionResult AccountUpdate(UserUpdateModel model)
        {
            if (model == null)
                return Notifization.NotFound();
            //
            string userId = Helper.Current.IdentifierID;

            string firstName = model.FirstName;
            string lastName = model.LastName;
            string nickName = model.NickName;
            string email = model.Email;
            //
            string langID = model.LanguageID;
            int Enabled = model.Enabled;
            // pay
            string bank = model.BankID;
            string accountNunber = model.AccountNunber;
            // firt name valid
            if (string.IsNullOrWhiteSpace(firstName))
                return Notifization.ERROR("Không được để trống họ/tên đệm");
            if (firstName.Length < 2 || firstName.Length > 30)
                return Notifization.ERROR("Họ/tên đệm giới hạn [2-30] ký tự");
            if (!Validate.FormatAlphabet(firstName))
                return Notifization.ERROR("Họ/tên đệm không hợp lệ");

            if (string.IsNullOrWhiteSpace(lastName))
                return Notifization.ERROR("Không được để trống tên");
            if (lastName.Length < 2 || lastName.Length > 30)
                return Notifization.ERROR("Tên giới hạn [2-30] ký tự");
            if (!Validate.FormatAlphabet(lastName))
                return Notifization.ERROR("Tên không hợp lệ");

            // middleName
            if (!string.IsNullOrWhiteSpace(nickName))
            {
                if (nickName.Length < 2 || nickName.Length > 30)
                    return Notifization.ERROR("Biệt danh giới hạn [2-30] ký tự");
                if (!Validate.FormatAlphabet(nickName))
                    return Notifization.ERROR("Biệt danh không hợp lệ");
            }

            // birthday
            string birthday = model.Birthday;
            if (!string.IsNullOrWhiteSpace(birthday))
            {
                if (!Validate.FormatDateVN(birthday))
                    return Notifization.ERROR("Ngày sinh không hợp lệ");
            }
            else
            {
                birthday = String.Format("{0:dd-MM-yyyy}", DateTime.Now);
            }
            // email address
            if (string.IsNullOrWhiteSpace(email))
                return Notifization.ERROR("Không được để trống địa chỉ email");
            if (!Validate.FormatEmail(email))
                return Notifization.ERROR("Địa chỉ email không hợp lệ");

            // phone number
            string phone = model.Phone;
            if (!string.IsNullOrWhiteSpace(phone))
            {
                if (!Validate.FormatPhone(phone))
                    return Notifization.ERROR("Số điện thoại không hợp lệ");
            }
            // address
            string address = model.Address;
            if (!string.IsNullOrWhiteSpace(address))
            {
                if (!Validate.FormatAlphabet(address))
                    return Notifization.ERROR("Địa chỉ không hợp lệ");
            }
            //if (string.IsNullOrWhiteSpace(langID))
            //    return Notifization.ERROR("Vui lòng chọn ngôn ngữ");
            _connection.Open();
            using (var _transaction = _connection.BeginTransaction())
            {
                try
                {
                    var languageService = new LanguageService(_connection);
                    if (Current.IsCMSUser)
                    {
                        var cmsUserLoginService = new CMSUserLoginService(_connection);
                        var cmsUserInfoService = new CMSUserInfoService(_connection);
                        var cmsUserSettingService = new CMSUserSettingService(_connection);
                        //var userBankService = new UserBankService(_connection);
                        //                           
                        var cmsUserInfo = cmsUserInfoService.GetAlls(m => m.UserID.ToLower().Equals(userId.ToLower()), transaction: _transaction).FirstOrDefault();
                        if (cmsUserInfo == null)
                            return Notifization.NotFound(NotifizationText.NotFound);

                        var cmsUserEmail = cmsUserInfoService.GetAlls(m => m.Email.ToLower().Equals(model.Email.ToLower()) && !m.UserID.ToLower().Equals(userId.ToLower()), transaction: _transaction).FirstOrDefault();
                        if (cmsUserEmail != null)
                            return Notifization.ERROR("Địa chỉ email đã được sử dụng");

                        var userInfoService = new UserInfoService(_connection);
                        var userEmail = userInfoService.GetAlls(m => m.Email.ToLower().Equals(model.Email.ToLower()) && !m.UserID.ToLower().Equals(userId.ToLower()), transaction: _transaction).FirstOrDefault();
                        if (userEmail != null)
                            return Notifization.ERROR("Địa chỉ email đã được sử dụng");

                        //if (!string.IsNullOrWhiteSpace(model.LanguageID))
                        //{
                        //    var lang = languageService.GetAlls(m => m.LanguageID.ToLower().Equals(model.LanguageID.ToLower()), transaction: _transaction).FirstOrDefault();
                        //    if (lang == null)
                        //        return Notifization.NotFound(NotifizationText.NotFound);
                        //}
                        //                        
                        // update user information
                        cmsUserInfo.ImageFile = model.ImageFile;
                        cmsUserInfo.FirstName = model.FirstName;
                        cmsUserInfo.LastName = model.LastName;
                        cmsUserInfo.NickName = model.NickName;
                        cmsUserInfo.Birthday = Helper.Library.FormatSQLDate(model.Birthday);
                        cmsUserInfo.Email = model.Email.ToLower();
                        cmsUserInfo.Phone = model.Phone;
                        cmsUserInfo.Address = model.Address;
                        cmsUserInfoService.Update(cmsUserInfo, transaction: _transaction);

                        var cmsUserSetting = cmsUserSettingService.GetAlls(m => m.UserID.ToLower().Equals(userId), transaction: _transaction).FirstOrDefault();
                        if (cmsUserSetting == null)
                            return Notifization.NotFound(NotifizationText.NotFound + "---");

                        cmsUserSetting.LanguageID = model.LanguageID;
                        cmsUserSettingService.Update(cmsUserSetting, transaction: _transaction);
                        //update bank
                        //var userBank = userBankService.GetAlls(m => m.UserID.ToLower().Equals(userId), transaction: _transaction).FirstOrDefault();
                        //if (userBank == null)
                        //    return Notifization.NotFound(NotifizationText.NotFound);
                        //userBank.BankID = model.BankID;
                        //userBank.AccountNumber = model.AccountNunber;
                        // commit
                        _transaction.Commit();
                        return Notifization.Success(NotifizationText.UPDATE_SUCCESS);
                    }
                    else
                    {
                        var userLoginService = new UserLoginService(_connection);
                        var userInfoService = new UserInfoService(_connection);
                        var userSettingService = new UserSettingService(_connection);
                        //var userBankService = new UserBankService(_connection);
                        //
                        var userInfo = userInfoService.GetAlls(m => m.UserID.ToLower().Equals(userId.ToLower()), transaction: _transaction).FirstOrDefault();
                        if (userInfo == null)
                            return Notifization.NotFound(NotifizationText.NotFound);
                        //
                        var userEmail = userInfoService.GetAlls(m => m.Email.ToLower().Equals(model.Email.ToLower()) && !m.UserID.ToLower().Equals(userId.ToLower()), transaction: _transaction).FirstOrDefault();
                        if (userEmail != null)
                            return Notifization.ERROR("Địa chỉ email đã được sử dụng");
                        // check cms
                        var cmsUserInfoService = new CMSUserInfoService(_connection);
                        var cmsUserEmail = cmsUserInfoService.GetAlls(m => m.Email.ToLower().Equals(model.Email.ToLower()) && !m.UserID.ToLower().Equals(userId.ToLower()), transaction: _transaction).FirstOrDefault();
                        if (cmsUserEmail != null)
                            return Notifization.ERROR("Địa chỉ email đã được sử dụng");
                        //
                        if (!string.IsNullOrWhiteSpace(model.LanguageID))
                        {
                            var lang = languageService.GetAlls(m => m.LanguageID.ToLower().Equals(model.LanguageID.ToLower()), transaction: _transaction).FirstOrDefault();
                            if (lang == null)
                                return Notifization.NotFound(NotifizationText.NotFound);
                        }
                        //update user information
                        userInfo.ImageFile = model.ImageFile;
                        userInfo.FirstName = model.FirstName;
                        userInfo.LastName = model.LastName;
                        userInfo.NickName = model.NickName;
                        userInfo.Birthday = Helper.Library.FormatSQLDate(model.Birthday);
                        userInfo.Email = model.Email.ToLower();
                        userInfo.Phone = model.Phone;
                        userInfo.Address = model.Address;
                        userInfoService.Update(userInfo, transaction: _transaction);

                        var userSetting = userSettingService.GetAlls(m => m.UserID.ToLower().Equals(userId), transaction: _transaction).FirstOrDefault();
                        if (userSetting == null)
                            return Notifization.NotFound(NotifizationText.NotFound);
                        //userSetting.DepartmentID = model.DepartmentID;
                        //userSetting.DepartmentPartID = model.DepartmentPartID;                      
                        //userSetting.WorkShiftID = model.WorkShiftID;
                        userSetting.LanguageID = model.LanguageID;
                        userSettingService.Update(userSetting, transaction: _transaction);
                        //bank
                        //var userBank = userBankService.GetAlls(m => m.UserID.ToLower().Equals(userId), transaction: _transaction).FirstOrDefault();
                        //if (userBank == null)
                        //    return Notifization.NotFound(NotifizationText.NotFound);
                        //userBank.BankID = model.BankID;
                        //userBank.AccountNumber = model.AccountNunber;
                        //commit
                        _transaction.Commit();
                        return Notifization.Success(NotifizationText.UPDATE_SUCCESS);
                    }
                }
                catch (Exception ex)
                {
                    _transaction.Rollback();
                    return Notifization.TEST("::" + ex.ToString());
                }
            }
        }
        public UserViewModal GetUserModel(string id)
        {
            try
            {
                var userViewModal = new UserViewModal();
                if (string.IsNullOrWhiteSpace(id))
                    return null;

                string sqlQuery = @"SELECT TOP 1 * FROM View_User WHERE ID = @Query";
                if (Helper.Current.IsCMSUser)
                {
                    sqlQuery = @"SELECT TOP 1 * FROM View_CMSUser WHERE ID = @Query";
                }
                userViewModal = _connection.Query<UserViewModal>(sqlQuery, new { Query = id }).FirstOrDefault();
                return userViewModal;
            }
            catch
            {
                return null;
            }
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Delete(string Id = null)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    if (Current.IsCMSUser)
                    {
                        var cmsUserLoginService = new CMSUserLoginService(_connection);
                        var cmsUserInfoService = new CMSUserInfoService(_connection);
                        var cmsUserSettingService = new CMSUserSettingService(_connection);
                        var cmsUserLogin = cmsUserLoginService.GetAlls(m => m.ID.Equals(Id.ToLower()), transaction: transaction).FirstOrDefault();
                        if (cmsUserLogin == null)
                            return Notifization.ERROR("Tài khoản không tồn tại");
                        var cmsUserInfo = cmsUserInfoService.GetAlls(m => m.UserID.ToLower().Equals(Id.ToLower()), transaction: transaction).FirstOrDefault();
                        if (cmsUserInfo == null)
                            return Notifization.ERROR("Tài khoản không tồn tại");
                        var cmsUserSetting = cmsUserSettingService.GetAlls(m => m.UserID.ToLower().Equals(Id.ToLower()), transaction: transaction).FirstOrDefault();
                        if (cmsUserSetting == null)
                            return Notifization.ERROR("Tài khoản không tồn tại");
                        // delete
                        AttachmentFile.DeleteFile(cmsUserInfo.ImageFile, transaction: transaction);
                        cmsUserInfoService.Remove(cmsUserInfo.ID, transaction: transaction);
                        cmsUserSettingService.Remove(cmsUserSetting.ID, transaction: transaction);
                        cmsUserInfoService.Remove(cmsUserLogin.ID, transaction: transaction);

                        transaction.Commit();
                        return Notifization.Success("Đã xóa tài khoản");
                    }
                    UserLoginService userLoginService = new UserLoginService(_connection);
                    UserInfoService userInfoService = new UserInfoService(_connection);
                    UserSettingService userSettingService = new UserSettingService(_connection);
                    var userLogin = userLoginService.GetAlls(m => m.ID.Equals(Id.ToLower()), transaction: transaction).FirstOrDefault();
                    if (userLogin == null)
                        return Notifization.ERROR("Tài khoản không tồn tại");
                    var userInfo = userInfoService.GetAlls(m => m.UserID.ToLower().Equals(Id.ToLower()), transaction: transaction).FirstOrDefault();
                    if (userInfo == null)
                        return Notifization.ERROR("Tài khoản không tồn tại");
                    var userSetting = userSettingService.GetAlls(m => m.UserID.ToLower().Equals(Id.ToLower()), transaction: transaction).FirstOrDefault();
                    if (userInfo == null)
                        return Notifization.ERROR("Tài khoản không tồn tại");
                    // delete
                    AttachmentFile.DeleteFile(userInfo.ImageFile, transaction: transaction);
                    userInfoService.Remove(userInfo.ID, transaction: transaction);
                    userSettingService.Remove(userSetting.ID, transaction: transaction);
                    userLoginService.Remove(userLogin.ID, transaction: transaction);

                    transaction.Commit();
                    return Notifization.Success("Đã xóa tài khoản");
                }
                catch
                {
                    transaction.Rollback();
                    return Notifization.NotService;
                }
            }
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult ChangePassword(UserChangePasswordModel model)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    string loginId = Helper.Current.IdentifierID;
                    if (string.IsNullOrWhiteSpace(loginId))
                        return Notifization.ERROR("Bạn cần đăng nhập trước");
                    string passId = Helper.Security.Encryption256Hashed(model.Password);
                    // check account system
                    if (Current.IsCMSUser)
                    {
                        var cmsUserLoginService = new CMSUserLoginService(_connection);
                        var cmsUserLogin = cmsUserLoginService.GetAlls(m => m.ID.Equals(loginId), transaction: transaction).FirstOrDefault();
                        if (cmsUserLogin == null)
                            return Notifization.NotFound(NotifizationText.NotFound);

                        if (!cmsUserLogin.PassID.Equals(passId))
                            return Notifization.NotFound("Mật khẩu cũ chưa đúng");
                        // update
                        cmsUserLogin.PassID = Helper.Security.Encryption256Hashed(model.NewPassword);
                        cmsUserLoginService.Update(cmsUserLogin, transaction: transaction);
                        transaction.Commit();
                        return Notifization.Success(NotifizationText.UPDATE_SUCCESS);
                    }
                    // check account user
                    UserLoginService userLoginService = new UserLoginService(_connection);
                    var userLogin = userLoginService.GetAlls(m => m.ID.Equals(loginId), transaction: transaction).FirstOrDefault();
                    if (userLogin == null)
                        return Notifization.NotFound(NotifizationText.NotFound);

                    if (!userLogin.PassID.Equals(passId))
                        return Notifization.NotFound("Mật khẩu cũ chưa đúng");
                    // update
                    userLogin.PassID = Helper.Security.Encryption256Hashed(model.NewPassword);
                    userLoginService.Update(userLogin, transaction: transaction);
                    transaction.Commit();
                    _connection.Close();
                    return Notifization.Success(NotifizationText.UPDATE_SUCCESS);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _connection.Close();
                    return Notifization.NotService;
                }
            }
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult ChangePinCode(UserChangePinCodeModel model)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    string loginId = Helper.Current.IdentifierID;
                    if (string.IsNullOrWhiteSpace(loginId))
                        return Notifization.ERROR("Bạn cần đăng nhập trước");

                    string newPinCode = model.NewPinCode;
                    string reNewPinCode = model.ReNewPinCode;
                    //
                    if (string.IsNullOrWhiteSpace(newPinCode))
                        return Notifization.ERROR("Không được để trống mã pin");
                    //
                    if (!newPinCode.ToLower().Equals(reNewPinCode))
                        return Notifization.ERROR("Xác nhận mã pin chưa đúng");
                    // 
                    if (newPinCode.Length != 8)
                        return Notifization.ERROR("Mã pin giới hạn 8 kí tự");

                    // check pin
                    string pincode = Helper.Security.Encryption256Hashed(model.PinCode);
                    if (Current.IsCMSUser)
                    {
                        var cmsUserLoginService = new CMSUserLoginService(_connection);
                        var cmsUserLogin = cmsUserLoginService.GetAlls(m => m.ID.Equals(loginId), transaction: transaction).FirstOrDefault();
                        if (cmsUserLogin == null)
                            return Notifization.NotFound(NotifizationText.NotFound);

                        if (!cmsUserLogin.PinCode.ToLower().Equals(pincode))
                            return Notifization.NotFound("Mã pin chưa đúng");
                        // update
                        cmsUserLogin.PinCode = Helper.Security.Encryption256Hashed(newPinCode);
                        cmsUserLoginService.Update(cmsUserLogin, transaction: transaction);
                        transaction.Commit();
                        return Notifization.Success(NotifizationText.UPDATE_SUCCESS);
                    }

                    var userLoginService = new UserLoginService(_connection);
                    var userLogin = userLoginService.GetAlls(m => m.ID.Equals(loginId), transaction: transaction).FirstOrDefault();
                    if (userLogin == null)
                        return Notifization.NotFound(NotifizationText.NotFound);

                    if (!userLogin.PinCode.Equals(pincode))
                        return Notifization.NotFound("Mã pin chưa đúng");
                    // update

                    userLogin.PinCode = Helper.Security.Encryption256Hashed(newPinCode);
                    userLoginService.Update(userLogin, transaction: transaction);
                    transaction.Commit();
                    return Notifization.Success(NotifizationText.UPDATE_SUCCESS);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Notifization.NotService;
                }
            }
        }
        //##############################################################################################################################################################################################################################################################
       
        //##############################################################################################################################################################################################################################################################
        public string CreateID(System.Data.IDbConnection _connectDb, System.Data.IDbTransaction transaction = null)
        {
            UserLoginService userLoginService = new UserLoginService(_connectDb);
            string result = string.Empty;
            int counter = 1;
            do
            {
                result = new Guid().ToString();
                var accountID = userLoginService.Get(result, transaction: transaction);
                if (accountID == null)
                    return result;
                else
                    result = string.Empty;
                counter++;
            } while (result == string.Empty && counter < 20);
            return result;
        }
        public string CreateID()
        {
            UserLoginService userLoginService = new UserLoginService(_connection);
            string result = string.Empty;
            int counter = 1;
            do
            {
                result = new Guid().ToString();
                var accountID = userLoginService.Get(result);
                if (accountID == null)
                    return result;
                else
                    result = string.Empty;
                counter++;
            } while (result == string.Empty && counter < 20);
            return result;
        }
        //##############################################################################################################################################################################################################################################################
        public string GetLanguageID
        {
            get
            {
                string result = string.Empty;
                try
                {
                    string userID = Convert.ToString(HttpContext.Current.Session["LoginID"]);
                    UserViewModal userModal = new UserViewModal();
                    if (string.IsNullOrWhiteSpace(userID))
                        return Default.LanguageID;
                    string sqlQuery = @"SELECT 
                                     [ID]
                                    ,[LoginID]
                                    ,[LastName]
                                    ,[Nickname]
                                    ,[Birthday]
                                    ,[Email]                                   
                                    ,[Phone]  
                                    ,PermissionID 
                                    ,[IsBlock]
                                    ,[Enabled]
                                    ,[LanguageID]
                                    ,[CreatedDate]  
                                     FROM View_User   
                                     WHERE ID = @UserID
                                    ";
                    userModal = _connection.Query<UserViewModal>(sqlQuery, new { UserID = userID }).FirstOrDefault();
                    if (userModal == null)
                        return Default.LanguageID;
                    return userModal.LanguageID;
                }
                catch
                {
                    return Default.LanguageID;
                }
            }
        }
        //##############################################################################################################################################################################################################################################################
        public string CurrentSiteIDByLoginID
        {
            get
            {
                try
                {
                    string loginId = Convert.ToString(HttpContext.Current.Session["LoginID"]);
                    if (string.IsNullOrWhiteSpace(loginId))
                        return "EM-SITE"; ;
                    string sqlQuery = @"SELECT  * FROM View_User WHERE ID = @UserID ";
                    var model = _connection.Query<UserViewModal>(sqlQuery, new { UserID = loginId }).FirstOrDefault();
                    if (model == null)
                        return "EM-SITE";
                    if (model.LoginID == "administrator")
                        return "AD-SITE";
                    return model.SiteID;
                }
                catch
                {
                    return "AD-SITE";
                }
            }
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult GenEMPID()
        {
            string sqlQuery = @"
                WITH temp1(ID)
                as (
                     SELECT ROW_NUMBER() OVER (ORDER BY ID DESC) AS Ranhk FROM dbo.UserLogin   
                )
                SELECT ISNULL(MAX(ID),1)  AS MaxNumber  FROM temp1";

            var maxRows = _connection.Query<MaxRowNumber>(sqlQuery).FirstOrDefault();
            int maxNumber = maxRows.MaxNumber;
            sqlQuery = @"SELECT * FROM View_User WHERE IdentifierID IS NULL  OR IdentifierID ='' ORDER BY IdentityID ASC";
            var dtList = _connection.Query<UserViewModal>(sqlQuery).ToList();
            List<EmpID> empIDs = new List<EmpID>();

            foreach (var item in dtList)
            {
                int max = 6;
                int strLen = max - maxNumber.ToString().Length;
                string indentity = string.Empty;
                for (int j = 0; j < strLen; j++)
                    indentity += "0";


                //  update
                //update user setting

                //var userSettingService = new UserSettingService(_connection);
                //var userSetting = userSettingService.GetAlls(m => m.UserID.ToLower().Equals(item.ID)).FirstOrDefault();
                //if (userSetting != null)
                //{
                //    userSetting.IdentifierID = "ID" + indentity + maxNumber;
                //    userSettingService.Update(userSetting);
                //}
                empIDs.Add(new EmpID
                {
                    ID = "ID" + indentity + maxNumber,
                    Name = strLen.ToString() + "|" + indentity
                });

                maxNumber++;
            }
            return Notifization.DATALIST(NotifizationText.Success, data: empIDs);
        }
        //##############################################################################################################################################################################################################################################################

        public ActionResult ChuanName()
        {
            try
            {
                string langID = Current.LanguageID;
                string sqlQuery = @"SELECT * FROM View_User";

                var dtList = _connection.Query<UserViewModal>(sqlQuery, new { }).ToList();
                if (dtList.Count <= 0)
                    return Notifization.NotFound(NotifizationText.NotFound);
                List<RsUserModel> resultData = new List<RsUserModel>();
                string fname = string.Empty;
                string lastname = string.Empty;

                UserInfoService userInfoService = new UserInfoService();

                foreach (var item in dtList)
                {
                    string name = item.FirstName;
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        name = name.Trim();
                        if (name.Contains(' '))
                        {
                            string[] rs = name.Split(' ');
                            lastname += Convert.ToString(rs[rs.Length - 1]) + ",";

                            var userinfor = userInfoService.GetAlls(m => m.UserID.ToLower().Equals(item.ID)).FirstOrDefault();
                            userinfor.LastName = Convert.ToString(rs[rs.Length - 1]);
                            userInfoService.Update(userinfor);
                            if (rs.Length > 0)
                            {
                                for (int i = 0; i < rs.Length - 1; i++)
                                {
                                    fname += rs[i] + ",";
                                    userinfor = userInfoService.GetAlls(m => m.UserID.ToLower().Equals(item.ID)).FirstOrDefault();
                                    userinfor.FirstName = Convert.ToString(rs[i]);
                                    userInfoService.Update(userinfor);
                                }
                            }
                        }

                    }
                    //resultData.Add(new RsUserModel(item.ID, item.LoginID, item.ImageFile, item.FirstName, item.LastName, item.Nickname, item.Birthday, item.Email, item.Phone, item.Address, item.IdentifierID, item.TimekepingID, item.DepartmentID, item.DepartmentPartID, item.LanguageID, item.SiteID, item.CreatedBy, item.IsBlock, item.Enabled, item.CreatedDate));
                }
                return Notifization.TEST("F:" + fname + " L:" + lastname);
            }
            catch (Exception ex)
            {
                return Notifization.TEST(":" + ex);
            }
        }


    }
    class EmpID
    {
        public string ID { get; set; }
        public string Name { get; set; }

    }
}