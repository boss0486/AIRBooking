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
using System.Web.Configuration;
using System.Data;
using Helper.File;
using Helper.Page;
using WebCore.Model.Entities;
using System.Globalization;
using WebCore.Model.Enum;
using WebCore.ENM;

namespace WebCore.Services
{
    public interface ICustomerService : IEntityService<AirAgent> { }
    public class AirAgentService : EntityService<AirAgent>, ICustomerService
    {
        public AirAgentService() : base() { }
        public AirAgentService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public ActionResult DataList(AirAgentSearchModel model)
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
            if (Helper.Current.UserLogin.IsAgentLogged())
            {
                // get customerid
                string clientId = GetAgentIDByUserID(Helper.Current.UserLogin.IdentifierID);
                whereCondition += " AND Path LIKE N'" + clientId + "%'";
            }

            string sqlQuery = @"SELECT * FROM App_AirAgent WHERE (dbo.Uni2NONE(Title) LIKE N'%'+ @Query +'%' OR CodeID LIKE N'%'+ @Query +'%') " + whereCondition + " ORDER BY [CreatedDate]";
            var dtList = _connection.Query<AirAgentResult>(sqlQuery, new { Query = Helper.Page.Library.FormatNameToUni2NONE(query) }).ToList();
            if (dtList.Count == 0)
                return Notifization.NotFound(MessageText.NotFound);
            //     
            var result = dtList.ToPagedList(page, Helper.Pagination.Paging.PAGESIZE).ToList();
            if (dtList.Count == 0 && page > 1)
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
            return Notifization.Data(MessageText.Success + sqlQuery, data: result, role: RoleActionSettingService.RoleListForUser(), paging: pagingModel);
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Create(AirAgentCreateModel model)
        {
            _connection.Open();
            using (var _transaction = _connection.BeginTransaction())
            {
                try
                {
                    if (model == null)
                        return Notifization.Invalid();
                    //
                    string agentId = model.AgentID;
                    string codeId = model.CodeID;
                    string parentId = model.ParentID;
                    string title = model.Title;
                    string summary = model.Summary;
                    string address = model.Address;
                    string compPhone = model.CompanyPhone;
                    string taxCode = model.TaxCode;
                    //
                    string contactName = model.ContactName;
                    string contactEmail = model.ContactEmail;
                    string contactPhone = model.ContactPhone;
                    //
                    double depositAmount = model.DepositAmount;
                    int termPayment = model.TermPayment;
                    //
                    string accountId = model.AccountID;
                    string password = model.Password;
                    string phone = model.Phone;
                    string email = model.Email;
                    //
                    int enabled = model.Enabled;

                    string path = string.Empty;
                    AirAgentService agentService = new AirAgentService(_connection);

                    string alias = Helper.Page.Library.FormatToUni2NONE(title);
                    if (model == null)
                        return Notifization.Invalid();
                    //
                    ClientLoginService clientLoginService = new ClientLoginService(_connection);
                    //return Notifization.TEST(":::" + Helper.Current.UserLogin.IdentifierID.ToLower());
                    string currentUserId = Helper.Current.UserLogin.IdentifierID;
                    UserService userService = new UserService(_connection);
                    if (Helper.Current.UserLogin.IsClientInApplication())
                    {
                        ClientLogin clientLogin = clientLoginService.GetAlls(m => m.UserID == currentUserId, transaction: _transaction).FirstOrDefault();
                        if (clientLogin == null)
                            return Notifization.Invalid("Đại lý không xác định");
                        //
                        agentId = clientLogin.AgentID;
                    }
                    //
                    if (!string.IsNullOrWhiteSpace(agentId))
                    {
                        var customer1 = agentService.GetAlls(m => m.ID == agentId, transaction: _transaction).FirstOrDefault();
                        if (customer1 == null)
                            return Notifization.Invalid("Đại lý cung cấp không hợp lệ");
                        //
                        parentId = customer1.ID;
                        // path
                        if (string.IsNullOrWhiteSpace(customer1.Path))
                            path = customer1.ID;
                        else
                            path = customer1.Path;
                        //
                        if (!string.IsNullOrWhiteSpace(customer1.ParentID))
                            return Notifization.Invalid("Cấp đại lý đã đạt giới hạn");
                        //
                    }
                    //
                    if (string.IsNullOrWhiteSpace(codeId))
                        return Notifization.Invalid("Không được để trống mã đại lý");
                    codeId = codeId.Trim();
                    if (codeId.Length != 3)
                        return Notifization.Invalid("Mã đại lý bao gồm 3 ký tự");

                    if (!Validate.TestAlphabet(codeId))
                        return Notifization.Invalid("Mã đại lý không hợp lệ");
                    // 
                    if (string.IsNullOrWhiteSpace(title))
                        return Notifization.Invalid("Không được để trống tên đại lý");
                    title = title.Trim();
                    if (!Validate.TestText(title))
                        return Notifization.Invalid("Tên đại lý không hợp lệ");
                    //
                    if (title.Length < 2 || title.Length > 80)
                        return Notifization.Invalid("Tên đại lý giới hạn 2-80 ký tự");
                    // summary valid               
                    if (!string.IsNullOrWhiteSpace(summary))
                    {
                        summary = summary.Trim();
                        if (!Validate.TestText(summary))
                            return Notifization.Invalid("Mô tả không hợp lệ");
                        if (summary.Length < 1 || summary.Length > 120)
                            return Notifization.Invalid("Mô tả giới hạn từ 1-> 120 ký tự");
                    }
                    //  tax code
                    if (string.IsNullOrWhiteSpace(taxCode))
                        return Notifization.Invalid("Không được để trống mã số thuế");
                    taxCode = taxCode.Trim();
                    if (!Validate.TestAlphabet(taxCode))
                        return Notifization.Invalid("Mã số thuế không hợp lệ");
                    //
                    if (string.IsNullOrWhiteSpace(compPhone))
                        return Notifization.Invalid("Không được để trống số đ.thoại công ty");
                    compPhone = compPhone.Trim();
                    if (!Validate.TestPhone(compPhone))
                        return Notifization.Invalid("Số đ.thoại công ty không hợp lệ");
                    //
                    if (string.IsNullOrWhiteSpace(address))
                        return Notifization.Invalid("Không được để trống địa chỉ");
                    address = address.Trim();
                    if (address.Length < 1 || address.Length > 255)
                        return Notifization.Invalid("Địa chỉ giới hạn từ 1-> 255 ký tự");
                    // contact name
                    if (string.IsNullOrWhiteSpace(contactName))
                        return Notifization.Invalid("Không được để trống họ tên");

                    contactName = contactName.Trim();
                    if (!Validate.TestText(contactName))
                        return Notifization.Invalid("Họ tên không hợp lệ");

                    if (contactName.Length < 2 || contactName.Length > 30)
                        return Notifization.Invalid("Họ tên giới hạn 2-30 ký tự");

                    // email
                    if (string.IsNullOrWhiteSpace(contactEmail))
                        return Notifization.Invalid("Không được để trống e-mail liên hệ");
                    //
                    contactEmail = contactEmail.Trim();
                    if (!Validate.TestEmail(contactEmail))
                        return Notifization.Invalid("E-mail liên hệ không hợp lệ");

                    if (string.IsNullOrWhiteSpace(contactPhone))
                        return Notifization.Invalid("Không được để trống số đ.thoại liên hệ");
                    contactPhone = contactPhone.Trim();
                    if (!Validate.TestPhone(contactPhone))
                        return Notifization.Invalid("Số đ.thoại liên hệ không hợp lệ");
                    //

                    // account
                    if (string.IsNullOrWhiteSpace(accountId))
                        return Notifization.Invalid("Không được để trống tài khoản");
                    //
                    accountId = accountId.Trim();
                    if (!Validate.TestUserName(accountId))
                        return Notifization.Invalid("Tài khoản không hợp lệ");
                    if (accountId.Length < 4 || accountId.Length > 16)
                        return Notifization.Invalid("Tài khoản giới hạn [4-16] ký tự");
                    // password
                    if (string.IsNullOrWhiteSpace(password))
                        return Notifization.Invalid("Không được để trống mật khẩu");
                    if (!Validate.TestPassword(password))
                        return Notifization.Invalid("Yêu cầu mật khẩu bảo mật hơn");
                    if (password.Length < 4 || password.Length > 16)
                        return Notifization.Invalid("Mật khẩu giới hạn [4-16] ký tự");
                    //
                    if (string.IsNullOrWhiteSpace(phone))
                        return Notifization.Invalid("Không được để trống số đ.thoại nhận OTP");
                    phone = phone.Trim();
                    if (!Validate.TestPhone(phone))
                        return Notifization.Invalid("Số đ.thoại nhận OTP không hợp lệ");
                    //
                    if (depositAmount <= 0)
                        return Notifization.Invalid("Số tiền đặt cọc phải > 0");
                    // 
                    if (termPayment < 0)
                        return Notifization.Invalid("Thời hạn thanh toán phải > 0");
                    // 
                    var agent = agentService.GetAlls(m => !string.IsNullOrWhiteSpace(m.CodeID) && m.CodeID.ToLower() == codeId.ToLower(), transaction: _transaction).FirstOrDefault();
                    if (agent != null)
                        return Notifization.Invalid("Mã đại lý đã được sử dụng");
                    //
                    var customerTitle = agentService.GetAlls(m => !string.IsNullOrWhiteSpace(m.Title) && m.Title.ToLower() == title.ToLower(), transaction: _transaction).FirstOrDefault();
                    if (customerTitle != null)
                        return Notifization.Invalid("Tên đại lý đã được sử dụng");
                    //
                    // account
                    string sqlQuery = @"SELECT TOP(1) ID FROM View_CMSUserLogin WHERE LoginID = @LoginID 
                                        UNION 
                                        SELECT TOP(1) ID FROM View_UserLogin WHERE LoginID = @LoginID ";
                    //
                    var userLogin = _connection.Query<UserIDModel>(sqlQuery, new { LoginID = accountId }, transaction: _transaction).FirstOrDefault();
                    if (userLogin != null)
                        return Notifization.Invalid("Tài khoản đã được sử dụng");
                    //
                    //
                    sqlQuery = @"SELECT TOP(1) ID FROM View_CMSUserLogin WHERE Email = @Email 
                                  UNION 
                                  SELECT TOP(1) ID FROM View_UserLogin WHERE Email = @Email ";
                    //
                    userLogin = _connection.Query<UserIDModel>(sqlQuery, new { Email = email }, transaction: _transaction).FirstOrDefault();
                    if (userLogin != null)
                        return Notifization.Error("Địa chỉ email đã được sử dụng");
                    //
                    UserLoginService userLoginService = new UserLoginService(_connection);
                    UserInfoService userInfoService = new UserInfoService(_connection);
                    UserSettingService userSettingService = new UserSettingService(_connection);
                    UserRoleService userRoleService = new UserRoleService(_connection);
                    WalletAgentService balanceCustomerService = new WalletAgentService(_connection);
                    LanguageService languageService = new LanguageService(_connection);
                    // *******  account login
                    string languageId = Helper.Page.Default.LanguageID;
                    string userId = userLoginService.Create<string>(new UserLogin()
                    {
                        LoginID = accountId.ToLower(),
                        Password = Helper.Security.Library.Encryption256(model.Password),
                        TokenID = null,
                        OTPCode = null
                    }, transaction: _transaction);
                    //
                    userInfoService.Create<string>(new UserInfo()
                    {
                        UserID = userId,
                        ImageFile = "",
                        FullName = codeId.ToUpper(),
                        Birthday = DateTime.Now,
                        Email = email,
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
                        IsBlock = false,
                        Enabled = enabled,
                        LanguageID = languageId,
                    }, transaction: _transaction);
                    //
                    agentId = agentService.Create<string>(new AirAgent()
                    {
                        CodeID = codeId,
                        ParentID = parentId,
                        Title = title,
                        Alias = alias,
                        Summary = summary,
                        Address = address,
                        CompanyPhone = compPhone,
                        TaxCode = taxCode,
                        //
                        ContactName = contactName,
                        ContactEmail = contactEmail,
                        ContactPhone = contactPhone,
                        //
                        DepositAmount = depositAmount,
                        TermPayment = termPayment,
                        //
                        RegisterID = userId,
                        //
                        Enabled = enabled,
                    }, transaction: _transaction);
                    //
                    var clientId = clientLoginService.Create<string>(new ClientLogin()
                    {
                        ClientType = (int)ClientLoginEnum.ClientType1.AGENT,
                        AgentID = agentId,
                        UserID = userId,
                        IsSuper = true
                    }, transaction: _transaction);

                    // update path
                    if (!string.IsNullOrWhiteSpace(path))
                        path += "/" + agentId;
                    else
                        path += agentId;
                    //
                    agent = agentService.GetAlls(m => m.ID == agentId, transaction: _transaction).FirstOrDefault();
                    agent.Path = path;
                    agentService.Update(agent, transaction: _transaction);
                    // commit
                    _transaction.Commit();
                    return Notifization.Success(MessageText.CreateSuccess);
                }
                catch (Exception ex)
                {
                    _transaction.Rollback();
                    return Notifization.TEST("::" + ex);
                }
            }
        }

        //##############################################################################################################################################################################################################################################################
        public ActionResult Update(AirAgentUpdateModel model)
        {
            _connection.Open();
            using (var _transaction = _connection.BeginTransaction())
            {
                try
                {
                    if (model == null)
                        return Notifization.Invalid();
                    //
                    string id = model.ID.ToLower();
                    string codeId = model.CodeID;
                    string parentId = model.ParentID;
                    string title = model.Title;
                    string summary = model.Summary;
                    string address = model.Address;
                    string compPhone = model.CompanyPhone;
                    string taxCode = model.TaxCode;
                    //
                    string contactName = model.ContactName;
                    string contactEmail = model.ContactEmail;
                    string contactPhone = model.ContactPhone;
                    //
                    double depositAmount = model.DepositAmount;
                    int termPayment = model.TermPayment;
                    //
                    int enabled = model.Enabled;
                    //
                    string alias = Helper.Page.Library.FormatToUni2NONE(title);
                    if (model == null)
                        return Notifization.Invalid();
                    // 
                    //if (string.IsNullOrWhiteSpace(codeId))
                    //    return Notifization.Invalid("Không được để trống mã đại lý");
                    //codeId = codeId.Trim();
                    //if (codeId.Length != 3)
                    //    return Notifization.Invalid("Mã đại lý bao gồm 3 ký tự");

                    //if (!Validate.TestAlphabet(codeId))
                    //    return Notifization.Invalid("Mã đại lý không hợp lệ");
                    // 
                    if (string.IsNullOrWhiteSpace(title))
                        return Notifization.Invalid("Không được để trống tên đại lý");
                    title = title.Trim();
                    if (!Validate.TestText(title))
                        return Notifization.Invalid("Tên đại lý không hợp lệ");
                    //
                    if (title.Length < 2 || title.Length > 80)
                        return Notifization.Invalid("Tên đại lý giới hạn 2-80 ký tự");
                    // summary valid               
                    if (!string.IsNullOrWhiteSpace(summary))
                    {
                        summary = summary.Trim();
                        if (!Validate.TestText(summary))
                            return Notifization.Invalid("Mô tả không hợp lệ");
                        if (summary.Length < 1 || summary.Length > 120)
                            return Notifization.Invalid("Mô tả giới hạn từ 1-> 120 ký tự");
                    }
                    //  tax code
                    if (string.IsNullOrWhiteSpace(taxCode))
                        return Notifization.Invalid("Không được để trống mã số thuế");
                    taxCode = taxCode.Trim();
                    if (!Validate.TestAlphabet(taxCode))
                        return Notifization.Invalid("Mã số thuế không hợp lệ");
                    //
                    if (string.IsNullOrWhiteSpace(compPhone))
                        return Notifization.Invalid("Không được để trống số đ.thoại công ty");
                    compPhone = compPhone.Trim();
                    if (!Validate.TestPhone(compPhone))
                        return Notifization.Invalid("Số đ.thoại công ty không hợp lệ");
                    // 
                    if (string.IsNullOrWhiteSpace(address))
                        return Notifization.Invalid("Không được để trống địa chỉ");
                    address = address.Trim();
                    if (address.Length < 1 || address.Length > 255)
                        return Notifization.Invalid("Địa chỉ giới hạn từ 1-> 255 ký tự");
                    // contact name
                    if (string.IsNullOrWhiteSpace(contactName))
                        return Notifization.Invalid("Không được để trống họ tên");

                    contactName = contactName.Trim();
                    if (!Validate.TestText(contactName))
                        return Notifization.Invalid("Họ tên không hợp lệ");

                    if (contactName.Length < 2 || contactName.Length > 30)
                        return Notifization.Invalid("Họ tên giới hạn 2-30 ký tự");

                    // email
                    if (string.IsNullOrWhiteSpace(contactEmail))
                        return Notifization.Invalid("Không được để trống e-mail liên hệ");
                    //
                    contactEmail = contactEmail.Trim();
                    if (!Validate.TestEmail(contactEmail))
                        return Notifization.Invalid("E-mail không hợp lệ");

                    if (string.IsNullOrWhiteSpace(contactPhone))
                        return Notifization.Invalid("Không được để trống số đ.thoại liên hệ");
                    contactPhone = contactPhone.Trim();
                    if (!Validate.TestPhone(contactPhone))
                        return Notifization.Invalid("Số đ.thoại liên hệ không hợp lệ");
                    //
                    if (depositAmount <= 0)
                        return Notifization.Invalid("Số tiền đặt cọc phải > 0");
                    //
                    if (termPayment < 0)
                        return Notifization.Invalid("Thời hạn thanh toán phải > 0");
                    //
                    AirAgentService customerService = new AirAgentService(_connection);
                    var customer = customerService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.ID == id, transaction: _transaction).FirstOrDefault();
                    if (customer == null)
                        return Notifization.NotFound();
                    //
                    var modelTitle = customerService.GetAlls(m => !string.IsNullOrWhiteSpace(m.Title) && m.Title.ToLower() == title.ToLower() && m.ID != id, transaction: _transaction).FirstOrDefault();
                    if (modelTitle != null)
                        return Notifization.Invalid("Tên đại lý đã được sử dụng");

                    var customerId = customerService.GetAlls(m => !string.IsNullOrWhiteSpace(m.CodeID) && m.CodeID.ToLower() == codeId.ToLower() && m.ID != id, transaction: _transaction).FirstOrDefault();
                    if (customerId != null)
                        return Notifization.Invalid("Tên đại lý đã được sử dụng");

                    // update content
                    //customer.CodeID = codeId;
                    //customer.ParentID = parentId;
                    customer.Title = title;
                    customer.Alias = alias;
                    customer.Summary = summary;
                    customer.Address = address;
                    customer.CompanyPhone = compPhone;
                    customer.TaxCode = taxCode;
                    //
                    customer.ContactName = contactName;
                    customer.ContactEmail = contactEmail;
                    customer.ContactPhone = contactPhone;
                    //
                    customer.DepositAmount = depositAmount;
                    customer.TermPayment = termPayment;
                    //customer.Path = "";
                    //customer.AccountID = "",
                    customer.Enabled = enabled;
                    customerService.Update(customer, transaction: _transaction);
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
        public AirAgent GetAgentByID(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return null;
                string query = string.Empty;
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT TOP (1) * FROM App_AirAgent WHERE ID = @Query";
                var model = _connection.Query<AirAgent>(sqlQuery, new { Query = id }).FirstOrDefault();
                return model;
            }
            catch
            {
                return null;
            }
        }

        public AirAgentResult ViewCustomerByID(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return null;
                string query = string.Empty;
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT TOP (1) * FROM App_AirAgent WHERE ID = @Query";
                var model = _connection.Query<AirAgentResult>(sqlQuery, new { Query = id }).FirstOrDefault();
                return model;
            }
            catch
            {
                return null;
            }
        }

        //########################################################################tttt######################################################################################################################################################################################
        public ActionResult Delete(AirAgentIDModel model)
        {
            _connection.Open();
            using (var _transaction = _connection.BeginTransaction())
            {
                try
                {
                    if (model == null)
                        return Notifization.Error(MessageText.Invalid + "01");
                    //
                    string id = model.ID.ToLower();
                    AirAgentService customerService = new AirAgentService(_connection);
                    var customer = customerService.GetAlls(m => m.ID == id, transaction: _transaction).FirstOrDefault();
                    if (customer == null)
                        return Notifization.NotFound();
                    //
                    string sqlCustomer = @"SELECT ID FROM App_AirAgent WHERE ParentID = @ParentID";
                    var customerId = _connection.Query<AirAgentIDModel>(sqlCustomer, new { ParentID = id }, transaction: _transaction).ToList();
                    if (customerId.Count > 0)
                        return Notifization.Error("Vui lòng xóa tất cả đại lý trước");
                    //
                    string sqlAgentFee = @"SELECT ID FROM App_AirAgentFee WHERE AgentID = @AgentID";
                    List<AirAgentIDModel> customerFee = _connection.Query<AirAgentIDModel>(sqlAgentFee, new { AgentID = id }, transaction: _transaction).ToList();
                    if (customerFee.Count > 0)
                    {
                        sqlAgentFee = @"DELETE App_AirAgentFee WHERE AgentID = @AgentID";
                        _connection.Execute(sqlAgentFee, new { AgentID = id }, transaction: _transaction);
                    }
                    // 
                    ClientLoginService clientLoginService = new ClientLoginService(_connection);
                    var clientLogin = clientLoginService.GetAlls(m => m.AgentID == id, transaction: _transaction).ToList();
                    if (clientLogin.Count > 0)
                    {
                        string sqlQuery = @"SELECT * FROM View_User WHERE ID IN ('" + String.Join("','", clientLogin.Select(m => m.UserID)) + "')";
                        var userResult = _connection.Query<UserResult>(sqlQuery, transaction: _transaction).ToList();
                        if (userResult.Count > 0)
                        {
                            // get all file
                            List<string> lstImgFile = userResult.Select(m => m.ImageFile).ToList();
                            if (lstImgFile.Count > 0)
                            {
                                foreach (var item in lstImgFile)
                                {
                                    AttachmentFile.DeleteFile(item, dbTransaction: _transaction);
                                }
                            }
                        }
                        _connection.Execute("DELETE App_ClientLogin WHERE UserID IN ('" + String.Join("','", clientLogin.Select(m => m.UserID)) + "')", transaction: _transaction);
                        _connection.Execute("DELETE UserInfo WHERE UserID IN ('" + String.Join("','", clientLogin.Select(m => m.UserID)) + "')", transaction: _transaction);
                        _connection.Execute("DELETE UserSetting WHERE UserID IN ('" + String.Join("','", clientLogin.Select(m => m.UserID)) + "')", transaction: _transaction);
                        _connection.Execute("DELETE UserLogin WHERE ID IN('" + String.Join("', '", clientLogin.Select(m => m.UserID)) + "')", transaction: _transaction);
                        _connection.Execute("DELETE UserRole WHERE UserID IN('" + String.Join("', '", clientLogin.Select(m => m.UserID)) + "')", transaction: _transaction);
                        //
                    }

                    // client wallet **************************************************************************************************
                    string sqlWallet = @"DELETE App_WalletAgent WHERE AgentID = @AgentID";
                    _connection.Execute(sqlWallet, new { AgentID = customer.ID }, transaction: _transaction);
                    //
                    string sqlWalletDeposit = @"DELETE App_WalletDepositHistory WHERE AgentID = @AgentID";
                    _connection.Execute(sqlWalletDeposit, new { AgentID = customer.ID }, transaction: _transaction);
                    //
                    string sqlWalletSpending = @"DELETE App_WalletSpendingHistory WHERE AgentID = @AgentID";
                    _connection.Execute(sqlWalletSpending, new { AgentID = customer.ID }, transaction: _transaction);
                    // 
                    // user wallet **************************************************************************************************
                    //string sqlWalletUser = @"DELETE App_TiketingSpendingLimit WHERE AgentID = @AgentID";
                    //_connection.Execute(sqlWalletUser, new { AgentID = customer.ID }, transaction: _transaction);
                    ////
                    //string sqlWalletUserSpending = @"DELETE App_WalletUserSpendingHistory WHERE AgentID = @AgentID";
                    //_connection.Execute(sqlWalletUserSpending, new { AgentID = customer.ID }, transaction: _transaction);
                    //
                    customerService.Remove(id, transaction: _transaction);
                    _transaction.Commit();
                    return Notifization.Success(MessageText.DeleteSuccess);
                }
                catch (Exception ex)
                {
                    _transaction.Rollback();
                    return Notifization.TEST("::" + ex);
                }
            }
        }

        public List<ClientOption> GetAgentLimit()
        {
            string userId = Helper.Current.UserLogin.IdentifierID;
            List<ClientOption> clientOptions = new List<ClientOption>();
            string sqlQuery;
            if (Helper.Current.UserLogin.IsCMSUser || Helper.Current.UserLogin.IsAdminInApplication)
            {
                sqlQuery = @"SELECT c.ID,c.CodeID, c.Title FROM App_AirAgent as c WHERE c.Enabled = 1 AND CHARINDEX('/', Path) < 1 ORDER BY c.CodeID";
                //
                clientOptions = _connection.Query<ClientOption>(sqlQuery, new { }).ToList();
            }
            else if (Helper.Current.UserLogin.IsAgentLogged())
            {
                string clientId = GetAgentIDByUserID(userId);
                sqlQuery = @"SELECT c.ID,c.CodeID, c.Title FROM App_AirAgent as c WHERE c.Enabled = 1 AND CHARINDEX('/', Path) < 1 AND ParentID = @ClientID ORDER BY c.CodeID";
                clientOptions = _connection.Query<ClientOption>(sqlQuery, new { ClientID = clientId }).ToList();
            }
            //
            if (clientOptions.Count == 0)
                return new List<ClientOption>();
            //
            return clientOptions;
        }



        public ActionResult GetAgentForCustomerType(string customerType)
        {
            if (string.IsNullOrWhiteSpace(customerType))
                return Notifization.NotFound(MessageText.NotFound);
            //
            customerType = customerType.Trim().ToLower();
            int typeEnum = AgentProvideTypeService.GetAgentProvideType(customerType);
            string whereCondition = string.Empty;
            //
            if (typeEnum == (int)AgentProvideEnum.AgentProvideType.AGENT)
                whereCondition += " AND (ParentID IS NULL OR datalength(ParentID)=0)";
            //
            if (typeEnum == (int)AgentProvideEnum.AgentProvideType.COMP)
                whereCondition += " AND TypeID = 'agent'";
            //
            string sqlQuery = @"SELECT c.ID,c.CodeID, c.Title FROM App_AirAgent as c WHERE c.Enabled = 1 " + whereCondition + "  ORDER BY c.CodeID";
            List<ClientOption> clientOptions = _connection.Query<ClientOption>(sqlQuery, new { }).ToList();
            if (clientOptions.Count == 0)
                return Notifization.NotFound(MessageText.NotFound);
            //
            return Notifization.Option(MessageText.Success, clientOptions);
        }


        public List<ClientOption> GetCompanyByLogin()
        {
            string userId = Helper.Current.UserLogin.IdentifierID;
            List<ClientOption> clientOptions = new List<ClientOption>();
            string sqlQuery;
            if (Helper.Current.UserLogin.IsCMSUser || Helper.Current.UserLogin.IsAdminInApplication)
            {
                sqlQuery = @"SELECT c.ID,c.CodeID, c.Title FROM App_AirAgent as c WHERE c.Enabled = 1 ORDER BY c.CodeID";
                //
                clientOptions = _connection.Query<ClientOption>(sqlQuery, new { }).ToList();
            }
            else if (Helper.Current.UserLogin.IsAgentLogged())
            {
                string clientId = GetAgentIDByUserID(userId);
                sqlQuery = @"SELECT c.ID,c.CodeID, c.Title FROM App_AirAgent as c WHERE c.Enabled = 1 AND ParentID = @ClientID ORDER BY c.CodeID";
                clientOptions = _connection.Query<ClientOption>(sqlQuery, new { ClientID = clientId }).ToList();
            }
            //
            if (clientOptions.Count == 0)
                return new List<ClientOption>();
            //
            return clientOptions;
        }


        public List<AirAgent_DDLSpendingOption> GetAgentHasSpendingData()
        {
            using (var service = new AirAgentService())
            {
                string agentId = string.Empty;
                string whereCondition = string.Empty;
                if (Helper.Current.UserLogin.IsAgentLogged())
                {
                    agentId = AirAgentService.GetAgentIDByUserID(Helper.Current.UserLogin.IdentifierID);
                    whereCondition = " AND a.ID = @AgentID OR a.ParentID = @AgentID";
                }
                //
                string sqlQuery = $@"SELECT a.ID, a.ParentID, a.Title, a.CodeID, l.Amount as 'Spending', l.Amount - ISNULL((select SUM(Amount) from App_UserSpending where AgentID  = a.ID), 0)  as 'Remain' 
                                     FROM App_AirAgent as a
                                     LEFT JOIN App_AgentSpendingLimit as l ON l.AgentID =  a.ID  
                                     WHERE a.ID != '' {whereCondition} ORDER BY  a.ParentID, a.Title";
                //
                List<AirAgent_DDLSpendingOption> dtList = service._connection.Query<AirAgent_DDLSpendingOption>(sqlQuery, new { AgentID = agentId }).ToList();
                if (dtList.Count == 0)
                    return new List<AirAgent_DDLSpendingOption>();
                //
                return dtList;
            }
        }

        //##############################################################################################################################################################################################################################################################
        public static string DropdownList(string id)
        {
            string result = string.Empty;
            using (var service = new AirAgentService())
            {
                var dtList = service.DataOption();
                if (dtList.Count > 0)
                {
                    foreach (var item in dtList)
                    {
                        string select = string.Empty;
                        if (!string.IsNullOrWhiteSpace(id) && item.ID == id.ToLower())
                            select = "selected";
                        result += "<option value='" + item.ID + "' data-codeid= '" + item.CodeID + "' " + select + ">" + item.Title + "</option>";
                    }
                }
                return result;
            }
        }
        public static string DropdownList_HasSpend(string id)
        {
            string result = string.Empty;
            using (var service = new AirAgentService())
            {
                string agentId = string.Empty;
                List<AirAgent_DDLSpendingOption> dtList = service.GetAgentHasSpendingData();
                if (dtList.Count == 0)
                    return string.Empty;
                //
                //
                foreach (var item in dtList)
                {
                    string strSelect = string.Empty;
                    if (!string.IsNullOrWhiteSpace(id) && item.ID == id.ToLower())
                        strSelect = "selected";
                    //
                    int limited = 0;
                    if (string.IsNullOrEmpty(item.ParentID))
                        limited = 1;
                    //
                    result += $"<option value='{item.ID}' {strSelect} title='{item.Title}' data-unlimited='{limited}' data-code='{item.CodeID}' data-spending='{item.Spending }' data-remain='{item.Remain }'>{item.Title }</option>";
                }
                return result;
            }

        }

        public static string DropdownListAgentLimit(string id)
        {
            string result = string.Empty;
            using (var service = new AirAgentService())
            {
                var dtList = service.GetAgentLimit();
                if (dtList.Count > 0)
                {
                    foreach (var item in dtList)
                    {
                        string select = string.Empty;
                        if (!string.IsNullOrWhiteSpace(id) && item.ID == id.ToLower())
                            select = "selected";
                        result += "<option value='" + item.ID + "' data-codeid= '" + item.CodeID + "' " + select + ">" + item.Title + "</option>";
                    }
                }
                return result;
            }

        }
        public static string DropdownListAgentSub(string id, string parentId = null)
        {
            string result = string.Empty;
            using (var service = new AirAgentService())
            {
                AirAgentService customerService = new AirAgentService();


                string whereCondition = string.Empty;
                if (!string.IsNullOrWhiteSpace(parentId))
                {
                    whereCondition += " AND ParentID = @ParentID";
                }

                string sqlQuery = @"SELECT ID, Title, CodeID FROM App_AirAgent WHERE Enabled = 1 AND ParentID IS NOT NULL" + whereCondition + " ORDER BY Title ASC";
                List<AirAgentOption> dtList = customerService.Query<AirAgentOption>(sqlQuery, new { ParentID = parentId }).ToList();
                if (dtList.Count > 0)
                {
                    foreach (var item in dtList)
                    {
                        string select = string.Empty;
                        if (!string.IsNullOrWhiteSpace(id) && item.ID == id.ToLower())
                            select = "selected";
                        result += "<option value='" + item.ID + "' data-codeid= '" + item.CodeID + "' " + select + ">" + item.Title + "</option>";
                    }
                }
                return result;
            }

        }

        public List<AirAgentOption> DataOption()
        {
            // limit data
            string whereCondition = string.Empty;
            string agentId = string.Empty;
            if (Helper.Current.UserLogin.IsClientInApplication())
            {
                agentId = GetAgentIDByUserID(Helper.Current.UserLogin.IdentifierID);
                whereCondition = " AND ID = @ID";
            }
            else if (!Helper.Current.UserLogin.IsCMSUser && !Helper.Current.UserLogin.IsAdminInApplication)
            {
                return new List<AirAgentOption>();
            }
            //
            string sqlQuery = @"SELECT ID, Title, CodeID FROM App_AirAgent WHERE Enabled = 1 " + whereCondition + " ORDER BY Title ASC";
            return _connection.Query<AirAgentOption>(sqlQuery, new { ID = agentId }).ToList();
        }

        public static string DropdownList(string id, int typeEnum)
        {
            string result = string.Empty;
            using (var service = new AirAgentService())
            {

                if (!Helper.Current.UserLogin.IsCMSUser && !Helper.Current.UserLogin.IsAdminInApplication && !Helper.Current.UserLogin.IsClientInApplication())
                    return result;
                //
                // limit by user login
                string whereCondition = string.Empty;
                if (Helper.Current.UserLogin.IsAdminAgentLogged())
                {
                    string userId = Helper.Current.UserLogin.IdentifierID;
                    string agentCode = GetAgentIDByUserID(userId);
                    whereCondition = " AND (ID = '" + agentCode + "' OR ParentID = '" + agentCode + "')";
                }
                if (Helper.Current.UserLogin.IsAgentLogged())
                {
                    string userId = Helper.Current.UserLogin.IdentifierID;
                    string agentCode = GetAgentIDByUserID(userId);
                    whereCondition = " AND (ID = '" + agentCode + "')";
                }
                //
                string sqlQuery = @"SELECT ID, Title, CodeID, ParentID FROM App_AirAgent WHERE Enabled = 1" + whereCondition + "   ORDER BY Title ASC";
                var dtList = service.Query<AirAgentOption>(sqlQuery).ToList();
                if (dtList.Count > 0)
                {
                    foreach (var item in dtList)
                    {
                        string select = string.Empty;
                        if (!string.IsNullOrWhiteSpace(id) && item.ID == id.ToLower())
                            select = "selected";
                        //
                        int isLimit = 1;
                        if (string.IsNullOrWhiteSpace(item.ParentID))
                            isLimit = 0;
                        //
                        result += "<option value='" + item.ID + "' data-codeid= '" + item.CodeID + "' data-limited = '" + isLimit + "' " + select + " title='" + item.Title + "'>" + item.CodeID + "</option>";
                    }
                }
                return result;
            }

        }

        public static string DropdownListProvider(string id)
        {
            string result = string.Empty;
            AirAgentService airAgentService = new AirAgentService();
            List<AirAgentOption> airAgentOptions = airAgentService.DataOption().Where(m => string.IsNullOrWhiteSpace(m.ParentID)).ToList();
            // 
            if (airAgentOptions.Count > 0)
            {
                foreach (var item in airAgentOptions)
                {
                    string select = string.Empty;
                    if (!string.IsNullOrWhiteSpace(id) && item.ID == id.ToLower())
                        select = "selected";
                    result += "<option value='" + item.ID + "' data-codeid= '" + item.CodeID + "' " + select + ">" + item.Title + "</option>";
                }
            }
            return result;

        }
        //##############################################################################################################################################################################################################################################################
        public static string GetAgentCodeID(string id)
        {

            if (string.IsNullOrWhiteSpace(id))
                return string.Empty;
            //
            id = id.ToLower();
            using (var service = new AirAgentService())
            {
                var customer = service.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.ID == id).FirstOrDefault();
                if (customer == null)
                    return string.Empty;
                //
                return customer.CodeID;

            }

        }
        public static string GetAgentName(string id)
        {

            if (string.IsNullOrWhiteSpace(id))
                return string.Empty;
            //
            id = id.ToLower();
            using (var service = new AirAgentService())
            {
                var customer = service.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.ID == id).FirstOrDefault();
                if (customer == null)
                    return string.Empty;
                //
                return customer.Title;

            }

        }
        public static string GetCustomerID(string id)
        {

            if (string.IsNullOrWhiteSpace(id))
                return string.Empty;
            //
            id = id.ToLower();
            using (var service = new AirAgentService())
            {
                var customer = service.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.ID == id).FirstOrDefault();
                if (customer == null)
                    return string.Empty;
                //
                return customer.Title;

            }

        }

        public static int GetLevelByPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return 0;
            //
            if (path.Contains("/"))
            {
                string[] arrPath = path.Split('/');
                return arrPath.Length;
            }
            //
            return 1;
        }

        public static string GetCustomerIDByUserID(string userId)
        {

            if (string.IsNullOrWhiteSpace(userId))
                return string.Empty;
            //
            using (var service = new ClientLoginService())
            {
                var customer = service.GetAlls(m => !string.IsNullOrWhiteSpace(m.UserID) && m.UserID.ToLower() == userId.ToLower()).FirstOrDefault();
                if (customer == null)
                    return string.Empty;
                //
                return customer.AgentID;

            }

        }
        //##############################################################################################################################################################################################################################################################
        public static string GetAgentIDByUserID(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return string.Empty;
            //
            userId = userId.ToLower();
            using (var service = new ClientLoginService())
            {
                var customer = service.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.UserID == userId).FirstOrDefault();
                if (customer == null)
                    return string.Empty;
                //
                return customer.AgentID;

            }
        }
    }
}