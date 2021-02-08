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
    public interface ICompanyService : IEntityService<Company> { }
    public class CompanyService : EntityService<Company>, ICompanyService
    {
        public CompanyService() : base() { }
        public CompanyService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
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
            }, ectColumn: "c.");
            if (searchResult != null)
            {
                if (searchResult.Status == 1)
                    whereCondition = searchResult.Message;
                else
                    return Notifization.Invalid(searchResult.Message);
            }
            #endregion
            // 
            string sqlQuery = @"SELECT c.*, a.CodeID as 'AgentCode', a.Title as 'AgentName' FROM App_Company as c LEFT JOIN App_AirAgent as a ON a.ID = c.AgentID  
            WHERE (dbo.Uni2NONE(c.Title) LIKE N'%'+ @Query +'%' OR c.CodeID LIKE N'%'+ @Query +'%') " + whereCondition + " ORDER BY c.CreatedDate";
            List<CompanyResult> dtList = _connection.Query<CompanyResult>(sqlQuery, new { Query = Helper.Page.Library.FormatNameToUni2NONE(query) }).ToList();
            if (dtList.Count == 0)
                return Notifization.NotFound(MessageText.NotFound);
            //     
            List<CompanyResult> result = dtList.ToPagedList(page, Helper.Pagination.Paging.PAGESIZE).ToList();
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
        public ActionResult Create(CompanyCreateModel model)
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
                    int termPayment = model.TermPayment;
                    //
                    string accountId = model.AccountID;
                    string password = model.Password;
                    string phone = model.Phone;
                    string email = model.Email;
                    //
                    int enabled = model.Enabled;
                    string path = string.Empty;
                    CompanyService CompanyService = new CompanyService(_connection);
                    string alias = Helper.Page.Library.FormatToUni2NONE(title);
                    if (model == null)
                        return Notifization.Invalid();
                    //
                    ClientLoginService clientLoginService = new ClientLoginService(_connection);
                    //return Notifization.TEST(":::" + Helper.Current.UserLogin.IdentifierID.ToLower());
                    string currentUserId = Helper.Current.UserLogin.IdentifierID;
                    UserService userService = new UserService(_connection);
                    //  
                    if (Helper.Current.UserLogin.IsClientInApplication())
                    {
                        ClientLogin clientLogin = clientLoginService.GetAlls(m => m.UserID == currentUserId, transaction: _transaction).FirstOrDefault();
                        if (clientLogin == null)
                            return Notifization.Invalid("Đại lý không xác định");
                        //
                        agentId = clientLogin.AgentID;
                    }
                    //
                    if (String.IsNullOrWhiteSpace(agentId))
                        return Notifization.Invalid("Vui lòng chọn đại lý");
                    //
                    if (string.IsNullOrWhiteSpace(codeId))
                        return Notifization.Invalid("Không được để trống mã công ty");
                    codeId = codeId.Trim();
                    if (codeId.Length != 3)
                        return Notifization.Invalid("Mã công ty bao gồm 3 ký tự");

                    if (!Validate.TestAlphabet(codeId))
                        return Notifization.Invalid("Mã công ty không hợp lệ");
                    // 
                    if (string.IsNullOrWhiteSpace(title))
                        return Notifization.Invalid("Không được để trống tên công ty");
                    //
                    title = title.Trim();
                    if (!Validate.TestText(title))
                        return Notifization.Invalid("Tên công ty không hợp lệ");
                    //
                    if (title.Length < 2 || title.Length > 80)
                        return Notifization.Invalid("Tên công ty giới hạn 2-80 ký tự");
                    // summary valid               
                    if (!string.IsNullOrWhiteSpace(summary))
                    {
                        summary = summary.Trim();
                        if (!Validate.TestText(summary))
                            return Notifization.Invalid("Mô tả không hợp lệ");
                        //
                        if (summary.Length < 1 || summary.Length > 120)
                            return Notifization.Invalid("Mô tả giới hạn từ 1-> 120 ký tự");
                    }
                    //  tax code
                    if (string.IsNullOrWhiteSpace(taxCode))
                        return Notifization.Invalid("Không được để trống mã số thuế");
                    //
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
                    //
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
                    if (termPayment < 0)
                        return Notifization.Invalid("Thời hạn thanh toán phải > 0");
                    // 
                    Company Company = CompanyService.GetAlls(m => !string.IsNullOrWhiteSpace(m.CodeID) && m.CodeID.ToLower() == codeId.ToLower(), transaction: _transaction).FirstOrDefault();
                    if (Company != null)
                        return Notifization.Invalid("Mã công ty đã được sử dụng");
                    //
                    Company CompanyTitle = CompanyService.GetAlls(m => !string.IsNullOrWhiteSpace(m.Title) && m.Title.ToLower() == title.ToLower(), transaction: _transaction).FirstOrDefault();
                    if (CompanyTitle != null)
                        return Notifization.Invalid("Tên công ty đã được sử dụng");
                    // account
                    string sqlQuery = @"SELECT TOP(1) ID FROM View_CMSUserLogin WHERE LoginID = @LoginID 
                                        UNION 
                                        SELECT TOP(1) ID FROM View_UserLogin WHERE LoginID = @LoginID ";
                    //
                    var userLogin = _connection.Query<UserIDModel>(sqlQuery, new { LoginID = accountId }, transaction: _transaction).FirstOrDefault();
                    if (userLogin != null)
                        return Notifization.Error("Tài khoản đã được sử dụng");
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
                    AgentWalletService balanceCompanyService = new AgentWalletService(_connection);
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
                    string CompanyId = CompanyService.Create<string>(new Company()
                    {
                        AgentID = agentId,
                        CodeID = codeId,
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
                        TermPayment = termPayment,
                        //
                        RegisterID = userId,
                        //
                        Enabled = enabled,
                    }, transaction: _transaction);
                    //
                    var clientId = clientLoginService.Create<string>(new ClientLogin()
                    {
                        ClientType = (int)ClientLoginEnum.ClientType1.COMP,
                        AgentID = CompanyId,
                        UserID = userId,
                        IsSuper = true
                    }, transaction: _transaction);
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
        public ActionResult Update(CompanyUpdateModel model)
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
                    int termPayment = model.TermPayment;
                    //
                    int enabled = model.Enabled;
                    //
                    string alias = Helper.Page.Library.FormatToUni2NONE(title);
                    if (model == null)
                        return Notifization.Invalid();
                    // 
                    if (string.IsNullOrWhiteSpace(title))
                        return Notifization.Invalid("Không được để trống tên công ty");
                    title = title.Trim();
                    if (!Validate.TestText(title))
                        return Notifization.Invalid("Tên công ty không hợp lệ");
                    //
                    if (title.Length < 2 || title.Length > 80)
                        return Notifization.Invalid("Tên công ty giới hạn 2-80 ký tự");
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
                    if (termPayment < 0)
                        return Notifization.Invalid("Thời hạn thanh toán phải > 0");
                    //
                    CompanyService CompanyService = new CompanyService(_connection);
                    Company Company = CompanyService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.ID == id, transaction: _transaction).FirstOrDefault();
                    if (Company == null)
                        return Notifization.NotFound();
                    //
                    Company modelTitle = CompanyService.GetAlls(m => !string.IsNullOrWhiteSpace(m.Title) && m.Title.ToLower() == title.ToLower() && m.ID != id, transaction: _transaction).FirstOrDefault();
                    if (modelTitle != null)
                        return Notifization.Invalid("Tên công ty đã được sử dụng");

                    Company modelCode = CompanyService.GetAlls(m => !string.IsNullOrWhiteSpace(m.CodeID) && m.CodeID.ToLower() == codeId.ToLower() && m.ID != id, transaction: _transaction).FirstOrDefault();
                    if (modelCode != null)
                        return Notifization.Invalid("Mã công ty đã được sử dụng");
                    // update content
                    //Company.CodeID = codeId;
                    Company.Title = title;
                    Company.Alias = alias;
                    Company.Summary = summary;
                    Company.Address = address;
                    Company.CompanyPhone = compPhone;
                    Company.TaxCode = taxCode;
                    //
                    Company.ContactName = contactName;
                    Company.ContactEmail = contactEmail;
                    Company.ContactPhone = contactPhone;
                    //
                    Company.TermPayment = termPayment;
                    Company.Enabled = enabled;
                    CompanyService.Update(Company, transaction: _transaction);
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
        public Company GetCompanyByID(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return null;
                string query = string.Empty;
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT TOP (1) * FROM App_Company WHERE ID = @Query";
                var model = _connection.Query<Company>(sqlQuery, new { Query = id }).FirstOrDefault();
                return model;
            }
            catch
            {
                return null;
            }
        }

        public CompanyResult ViewCompanyByID(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return null;
                string query = string.Empty;
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT TOP (1) * FROM App_Company WHERE ID = @Query";
                var model = _connection.Query<CompanyResult>(sqlQuery, new { Query = id }).FirstOrDefault();
                return model;
            }
            catch
            {
                return null;
            }
        }

        //########################################################################tttt######################################################################################################################################################################################
        public ActionResult Delete(CompanyIDModel model)
        {
            _connection.Open();
            using (var _transaction = _connection.BeginTransaction())
            {
                try
                {
                    if (model == null)
                        return Notifization.Invalid(MessageText.Invalid + "01");
                    //
                    string id = model.ID.ToLower();
                    CompanyService CompanyService = new CompanyService(_connection);
                    Company Company = CompanyService.GetAlls(m => m.ID == id, transaction: _transaction).FirstOrDefault();
                    if (Company == null)
                        return Notifization.NotFound();
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
                    //
                    CompanyService.Remove(id, transaction: _transaction);
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
        //##############################################################################################################################################################################################################################################################
        public static string DropdownList(string id)
        {
            string result = string.Empty;
            using (var service = new CompanyService())
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
        public List<CompanyOption> DataOption()
        {
            // limit data 
            string whereCondition = string.Empty;
            if (Helper.Current.UserLogin.IsAdminInApplication)
                whereCondition = string.Empty;
            else if (Helper.Current.UserLogin.IsAdminAgentLogged() || Helper.Current.UserLogin.IsAgentLogged())
                whereCondition = $" AND AgentID = '{AirAgentService.GetAgentIDByUserID(Helper.Current.UserLogin.IdentifierID)}'"; 
            else
                return new List<CompanyOption>();
            // **********************************************************************************************************************************
            string sqlQuery = $@"SELECT ID, Title, CodeID FROM App_Company WHERE Enabled = 1 {whereCondition} ORDER BY Title ASC";
            return _connection.Query<CompanyOption>(sqlQuery).ToList();
        }

        public static string DropdownList(string id, int typeEnum)
        {
            string result = string.Empty;
            using (var service = new CompanyService())
            {
                var dtList = service.DataOption();
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

        //##############################################################################################################################################################################################################################################################
        public static string GetCompanyCodeID(string id)
        {

            if (string.IsNullOrWhiteSpace(id))
                return string.Empty;
            //
            id = id.ToLower();
            using (var service = new CompanyService())
            {
                var Company = service.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.ID == id).FirstOrDefault();
                if (Company == null)
                    return string.Empty;
                //
                return Company.CodeID;

            }

        }
        public static string GetCompanyName(string id)
        {

            if (string.IsNullOrWhiteSpace(id))
                return string.Empty;
            //
            id = id.ToLower();
            using (var service = new CompanyService())
            {
                var Company = service.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.ID == id).FirstOrDefault();
                if (Company == null)
                    return string.Empty;
                //
                return Company.Title;

            }

        }
        public List<ClientOption> GetCompByAgentID(string agentId)
        {
            string sqlQuery = @"SELECT c.ID,c.CodeID, c.Title FROM App_Company as c WHERE c.Enabled = 1 AND (ParentID = @AgentID OR SupplierID = @AgentID) ORDER BY c.CodeID";
            List<ClientOption> clientOptions = _connection.Query<ClientOption>(sqlQuery, new { AgentID = agentId }).ToList();
            //
            if (clientOptions.Count == 0)
                return new List<ClientOption>();
            //
            return clientOptions;
        }
    }
}