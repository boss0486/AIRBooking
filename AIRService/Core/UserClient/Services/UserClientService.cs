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
            using (var service = new UserClientService())
            {
                string sqlQuery = @"SELECT vu.*, c.ClientID, c.ClientType FROM View_User as vu LEFT JOIN dbo.App_ClientLogin as c ON vu.ID = c.UserID 
                                    WHERE vu.ID IN (select  c.UserID from App_ClientLogin as c group by c.UserID) AND dbo.Uni2NONE(vu.FullName) LIKE N'%'+ dbo.Uni2NONE(@Query) +'%' ORDER BY vu.FullName,vu.CreatedDate ";
                var dtList = _connection.Query<UserClientResult>(sqlQuery, new { Query = query }).ToList();
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
                return Notifization.Data(MessageText.Success + sqlQuery, data: result, role: roleAccountModel, paging: pagingModel);
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
                        string nickName = model.NickName;
                        string birthday = model.Birthday;
                        string imageFile = model.ImageFile;
                        string email = model.Email;
                        string phone = model.Phone;
                        string address = model.Address;
                        //
                        string loginId = model.LoginID;
                        string password = model.Password;
                        string roleId = model.RoleID;
                        //
                        int clientType = model.ClientType;
                        string clientId = model.ClientID;
                        //
                        string languageId = "";
                        bool isBlock = model.IsBlock;
                        int enabled = model.Enabled;
                        //
                        string crrUserId = Helper.Current.UserLogin.IdentifierID;
                        ClientLoginService clientLoginService = new ClientLoginService(_connection);
                        UserService userService = new UserService(_connection);
                        if (!userService.IsClientLogged(crrUserId, _connection, _transaction))
                        {
                            if (clientType != (int)WebCore.ENM.ClientLoginEnum.ClientType.Customer && clientType != (int)WebCore.ENM.ClientLoginEnum.ClientType.Supplier)
                                return Notifization.Invalid("Loại người dùng không hợp lệ");
                            //
                            string clientName = string.Empty;
                            if (clientType == (int)WebCore.ENM.ClientLoginEnum.ClientType.Customer)
                            {
                                if (string.IsNullOrWhiteSpace(clientId))
                                    return Notifization.Invalid("Vui lòng chọn khách hàng");
                                //
                                clientName = "Khách hàng";
                            }
                            //
                            if (clientType == (int)WebCore.ENM.ClientLoginEnum.ClientType.Customer)
                            {
                                if (string.IsNullOrWhiteSpace(clientId))
                                    return Notifization.Invalid("Vui lòng chọn nhà cung cấp");
                                //
                                clientName = "Nhà cung cấp";
                            }
                            // 
                            var client = clientLoginService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.ClientID.ToLower().Equals(clientId.ToLower()), transaction: _transaction).FirstOrDefault();
                            if (client == null)
                                return Notifization.Invalid(clientName + " không hợp lệ");
                        }
                        else
                        {
                            clientId = crrUserId;
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
                        RoleService roleService = new RoleService(_connection);
                        if (!string.IsNullOrWhiteSpace(roleId))
                        {
                            var role = roleService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.ID.ToLower().Equals(roleId.ToLower()), transaction: _transaction);
                            if (role == null)
                                return Notifization.Error("Nhóm quyền không hợp lệ");
                        }
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
                            LoginID = loginId,
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
                            Birthday = Helper.Time.TimeHelper.FormatToDateSQL(birthday),
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
                            RoleID = null,
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
                        // user for client
                        clientLoginService.Create<string>(new ClientLogin()
                        {
                            UserID = userId,
                            ClientID = clientId,
                            ClientType = clientType
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
                        string roleId = model.RoleID;
                        string languageId = model.LanguageID;
                        bool isBlock = model.IsBlock;
                        int enabled = model.Enabled;
                        //
                        if (string.IsNullOrWhiteSpace(userId))
                            return Notifization.Invalid(MessageText.Invalid);
                        userId = userId.Trim();

                        UserLoginService userLoginService = new UserLoginService(_connection);
                        UserInfoService userInfoService = new UserInfoService(_connection);
                        UserSettingService userSettingService = new UserSettingService(_connection);
                        UserRoleService userRoleService = new UserRoleService(_connection);

                        LanguageService languageService = new LanguageService(_connection);
                        //update user information
                        var userLogin = userLoginService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.ID.ToLower().Equals(userId.ToLower()), transaction: _transaction).FirstOrDefault();
                        if (userLogin == null)
                            return Notifization.Invalid(MessageText.Invalid);
                        //
                        RoleService roleService = new RoleService(_connection);
                        if (!string.IsNullOrWhiteSpace(roleId))
                        {
                            var role = roleService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.ID.ToLower().Equals(roleId.ToLower()), transaction: _transaction);
                            if (role == null)
                                return Notifization.Error("Nhóm quyền không hợp lệ");
                        }
                        var _uId = userLogin.ID;
                        var userInfo = userInfoService.GetAlls(m => !string.IsNullOrWhiteSpace(m.UserID) && m.UserID.ToLower().Equals(_uId.ToLower()), transaction: _transaction).FirstOrDefault();
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
                        userInfo.Birthday = Helper.Time.TimeHelper.FormatToDateSQL(model.Birthday);
                        //userInfo.Email = model.Email.ToLower();
                        userInfo.Phone = model.Phone;
                        userInfo.Address = model.Address;
                        userInfoService.Update(userInfo, transaction: _transaction);
                        //
                        var userSetting = userSettingService.GetAlls(m => !string.IsNullOrWhiteSpace(m.UserID) && m.UserID.ToLower().Equals(_uId.ToLower()), transaction: _transaction).FirstOrDefault();
                        if (userSetting == null)
                            return Notifization.Invalid(MessageText.Invalid);
                        //
                        //userSetting.AreaID = areaId;
                        userSetting.IsBlock = isBlock;
                        userSetting.Enabled = enabled;
                        userSetting.LanguageID = model.LanguageID;
                        userSettingService.Update(userSetting, transaction: _transaction);
                        //
                        if (!string.IsNullOrWhiteSpace(roleId))
                        {
                            // delete role cũ
                            var userRole = userRoleService.GetAlls(m => !string.IsNullOrWhiteSpace(m.RoleID) && m.UserID.ToLower().Equals(_uId.ToLower()), transaction: _transaction).FirstOrDefault();
                            if (userRole == null)
                            {
                                userRoleService.Create<string>(new UserRole()
                                {
                                    RoleID = roleId,
                                    UserID = _uId
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
                        AttachmentFile.DeleteFile(cmsUserResult.ImageFile, transaction: _transaction);

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

        public UserClientModel GetUserModel(string id)
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
                    var data = _connection.Query<UserClientModel>(sqlQuery, new { ID = id }).FirstOrDefault();
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

        public ActionResult GetUserIsHasRoleBooker(UserIDModel model)
        {
            if (model == null)
                return Notifization.Invalid(MessageText.Invalid);

            string clientId = model.ID;
            using (var service = new UserClientService())
            {
                var _connection = service._connection;
                if (string.IsNullOrWhiteSpace(clientId))
                    return Notifization.Invalid(MessageText.Invalid);
                //
                string sqlQuery = @"  SELECT uf.UserID, uf.FullName, ur.RoleID FROM UserInfo as uf
                                          INNER JOIN App_ClientLogin as client On client.UserID = uf.UserID
                                          LEFT JOIN UserRole as ur ON client.UserID =  ur.UserID
                                          INNER JOIN [Role] as r On r.ID = ur.RoleID
                                          where r.IsAllowSpend = 1 AND client.ClientType = @ClientType AND client.ClientID = @ClientID";
                var data = _connection.Query<UserIsHasRoleBookerModel>(sqlQuery, new { ClientID = clientId, ClientType = (int)ClientLoginEnum.ClientType.Customer }).ToList();
                if (data.Count == 0)
                    return Notifization.Invalid(MessageText.Invalid);
                //
                return Notifization.Data(MessageText.Success, data);
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
    }
}
