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
    public interface ISupplierService : IEntityService<Supplier> { }
    public class SupplierService : EntityService<Supplier>, ISupplierService
    {
        public SupplierService() : base() { }
        public SupplierService(System.Data.IDbConnection db) : base(db) { }
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

            if (!Helper.Current.UserLogin.IsCMSUser && !Helper.Current.UserLogin.IsAdministratorInApplication && !Helper.Current.UserLogin.IsSupplierLogged())
                return Notifization.AccessDenied(MessageText.AccessDenied);
            //
            if (Helper.Current.UserLogin.IsSupplierLogged())
            {
                string supplierId = ClientLoginService.GetClientIDByUserID(Helper.Current.UserLogin.IdentifierID);

                whereCondition += " AND ID = '" + supplierId + "'";
            }
            //
            string typeId = "";
            string sqlQuery = @"SELECT * FROM App_Supplier WHERE (dbo.Uni2NONE(Title) LIKE N'%'+ @Query +'%' OR CodeID LIKE N'%'+ @Query +'%') " + whereCondition + " ORDER BY [CreatedDate]";
            var dtList = _connection.Query<SupplierResultModel>(sqlQuery, new { Query = Helper.Page.Library.FormatToUni2NONE(query), TypeID = typeId }).ToList();
            if (dtList.Count == 0)
                return Notifization.NotFound(MessageText.NotFound);
            //     
            var result = dtList.ToPagedList(page, Helper.Pagination.Paging.PAGESIZE).ToList();
            if (dtList.Count == 0 && page > 1)
            {
                page -= 1;
                result = dtList.ToPagedList(page, Helper.Pagination.Paging.PAGESIZE).ToList();
            }
            if (result.Count == 0)
                return Notifization.NotFound(MessageText.NotFound);

            Helper.Pagination.PagingModel pagingModel = new Helper.Pagination.PagingModel
            {
                PageSize = Helper.Pagination.Paging.PAGESIZE,
                Total = dtList.Count,
                Page = page
            };
            //
            return Notifization.Data(MessageText.Success, data: result, role: RoleActionSettingService.RoleListForUser(), paging: pagingModel);
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Create(SupplierCreateModel model)
        {
            _connection.Open();
            using (var _transaction = _connection.BeginTransaction())
            {
                try
                {
                    if (model == null)
                        return Notifization.Invalid();
                    //
                    //string typeId = model.TypeID;
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
                    string accountId = model.AccountID;
                    string password = model.Password;
                    string phone = model.Phone;
                    string email = model.Email;
                    //
                    int enabled = model.Enabled;
                    SupplierService supplierService = new SupplierService(_connection);

                    string alias = Helper.Page.Library.FormatToUni2NONE(title);
                    if (model == null)
                        return Notifization.Invalid();
                    //
                    //if (string.IsNullOrWhiteSpace(typeId))
                    //    return Notifization.Invalid("Vui lòng chọn loại nhà cung cấp");
                    //typeId = typeId.Trim();
                    //
                    if (string.IsNullOrWhiteSpace(codeId))
                        return Notifization.Invalid("Không được để trống mã nhà cung cấp");
                    codeId = codeId.Trim();
                    if (codeId.Length != 3)
                        return Notifization.Invalid("Mã nhà cung cấp bao gồm 3 ký tự");

                    if (!Validate.TestAlphabet(codeId))
                        return Notifization.Invalid("Mã nhà cung cấp không hợp lệ");
                    // 
                    if (string.IsNullOrWhiteSpace(title))
                        return Notifization.Invalid("Không được để trống tên nhà cung cấp");
                    title = title.Trim();
                    if (!Validate.TestText(title))
                        return Notifization.Invalid("Tên nhà cung cấp không hợp lệ");
                    //
                    if (title.Length < 2 || title.Length > 80)
                        return Notifization.Invalid("Tên nhà cung cấp giới hạn 2-80 ký tự");
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
                    var supplier = supplierService.GetAlls(m => !string.IsNullOrWhiteSpace(m.CodeID) && m.CodeID.ToLower().Equals(codeId.ToLower()), transaction: _transaction).FirstOrDefault();
                    if (supplier != null)
                        return Notifization.Invalid("Mã nhà cung cấp đã được sử dụng");

                    supplier = supplierService.GetAlls(m => !string.IsNullOrWhiteSpace(m.Title) && m.Title.ToLower().Equals(title.ToLower()), transaction: _transaction).FirstOrDefault();
                    if (supplier != null)
                        return Notifization.Invalid("Tên nhà cung cấp đã được sử dụng");
                    //
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
                    ClientLoginService clientLoginService = new ClientLoginService(_connection);
                    LanguageService languageService = new LanguageService(_connection);

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
                        FullName = accountId,
                        Birthday = DateTime.Now,
                        Email = email,
                        Phone = phone,
                        Address = address
                    }, transaction: _transaction);
                    //
                    string indentity = string.Empty;
                    //
                    userSettingService.Create<string>(new UserSetting()
                    {
                        UserID = userId,
                        SecurityPassword = null,
                        AuthenType = null,
                        RoleID = null,
                        IsBlock = false,
                        Enabled = enabled,
                        LanguageID = languageId,
                    }, transaction: _transaction);
                    //
                    var supplierId = supplierService.Create<string>(new Supplier()
                    {
                        CodeID = codeId,
                        //ParentID = parentId,
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
                        Enabled = enabled
                    }, transaction: _transaction);

                    var clientId = clientLoginService.Create<string>(new ClientLogin()
                    {
                        ClientID = supplierId,
                        ClientType = (int)ClientLoginEnum.ClientType.Supplier,
                        UserID = userId
                    }, transaction: _transaction);
                    // site id
                    string strPath = string.Empty;
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
        public ActionResult Update(SupplierUpdateModel model)
        {
            _connection.Open();
            using (var _transaction = _connection.BeginTransaction())
            {
                try
                {
                    if (model == null)
                        return Notifization.Invalid();
                    //
                    string id = model.ID;
                    //string typeId = model.TypeID;
                    string codeId = model.CodeID;
                    //string parentId = model.ParentID;
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
                    int enabled = model.Enabled;
                    //
                    string alias = Helper.Page.Library.FormatToUni2NONE(title);
                    if (model == null)
                        return Notifization.Invalid();
                    //
                    //if (string.IsNullOrWhiteSpace(typeId))
                    //    return Notifization.Invalid("Vui lòng chọn loại nhà cung cấp");
                    //typeId = typeId.Trim();
                    //
                    //if (string.IsNullOrWhiteSpace(codeId))
                    //    return Notifization.Invalid("Không được để trống mã nhà cung cấp");
                    //codeId = codeId.Trim();
                    //if (codeId.Length != 3)
                    //    return Notifization.Invalid("Mã nhà cung cấp bao gồm 3 ký tự");

                    //if (!Validate.TestAlphabet(codeId))
                    //    return Notifization.Invalid("Mã nhà cung cấp không hợp lệ");
                    // 
                    if (string.IsNullOrWhiteSpace(title))
                        return Notifization.Invalid("Không được để trống tên nhà cung cấp");
                    title = title.Trim();
                    if (!Validate.TestText(title))
                        return Notifization.Invalid("Tên nhà cung cấp không hợp lệ");
                    //
                    if (title.Length < 2 || title.Length > 80)
                        return Notifization.Invalid("Tên nhà cung cấp giới hạn 2-80 ký tự");
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

                    SupplierService supplierService = new SupplierService(_connection);
                    var supplier = supplierService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.ID.Equals(model.ID.ToLower()), transaction: _transaction).FirstOrDefault();
                    if (supplier == null)
                        return Notifization.NotFound();
                    //
                    var modelTitle = supplierService.GetAlls(m => !string.IsNullOrWhiteSpace(m.Title) && m.Title.ToLower().Equals(title.ToLower()) && !m.ID.Equals(model.ID.ToLower()), transaction: _transaction).FirstOrDefault();
                    if (modelTitle != null)
                        return Notifization.Invalid("Tên nhà cung cấp đã được sử dụng");

                    var supplierId = supplierService.GetAlls(m => !string.IsNullOrWhiteSpace(m.CodeID) && m.CodeID.ToLower().Equals(codeId.ToLower()) && !m.ID.Equals(model.ID.ToLower()), transaction: _transaction).FirstOrDefault();
                    if (supplierId != null)
                        return Notifization.Invalid("Mã nhà cung cấp đã được sử dụng");
                    //
                    if (enabled != (int)ModelEnum.Enabled.ENABLED)
                        return Notifization.Error("Xác nhận hoàn tất thủ tục");
                    //
                    //Supplier.SupplierType = typeId;
                    //supplier.CodeID = codeId;
                    //supplier.ParentID = parentId;
                    supplier.Title = title;
                    supplier.Alias = alias;
                    supplier.Summary = summary;
                    supplier.Address = address;
                    supplier.CompanyPhone = compPhone;
                    supplier.TaxCode = taxCode;
                    //
                    supplier.ContactName = contactName;
                    supplier.ContactEmail = contactEmail;
                    supplier.ContactPhone = contactPhone;
                    //
                    supplier.Enabled = enabled;
                    supplierService.Update(supplier, transaction: _transaction);
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
        public Supplier GetSupplierModel(string Id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Id))
                    return null;
                string query = string.Empty;
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT TOP (1) * FROM App_Supplier WHERE ID = @Query";
                var model = _connection.Query<Supplier>(sqlQuery, new { Query = Id }).FirstOrDefault();
                return model;
            }
            catch
            {
                return null;
            }
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Delete(SupplierIDModel model)
        {
            _connection.Open();
            using (var _transaction = _connection.BeginTransaction())
            {
                try
                {
                    if (model == null)
                        return Notifization.Error(MessageText.Invalid);
                    //
                    string id = model.ID;
                    SupplierService supplierService = new SupplierService(_connection);
                    var supplier = supplierService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.ID.ToLower().Equals(id.ToLower()), transaction: _transaction).FirstOrDefault();
                    if (supplier == null)
                        return Notifization.NotFound();
                    // get all user
                    ClientLoginService clientLoginService = new ClientLoginService(_connection);
                    var clientLogin = clientLoginService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.ClientID.ToLower().Equals(supplier.ID.ToLower()) && m.ClientType == (int)ClientLoginEnum.ClientType.Supplier, transaction: _transaction).ToList();
                    if (clientLogin.Count == 0)
                        return Notifization.Error(MessageText.Invalid);
                    //
                    string sqlQuery = @"SELECT * FROM View_User WHERE ID IN ('" + String.Join("','", clientLogin.Select(m => m.UserID)) + "')";
                    var userResult = _connection.Query<UserResult>(sqlQuery, transaction: _transaction).ToList();
                    if (userResult.Count == 0)
                        return Notifization.Error(MessageText.Invalid);
                    // check data reference
                    // #1. Customer
                    string sqlCustomer = @"SELECT ID FROM App_Customer WHERE SupplierID = @SupplierID";
                    var customerId = _connection.Query<CustomerIDModel>(sqlCustomer, new { SupplierID = id }, transaction: _transaction).ToList();
                    if (customerId.Count > 0)
                        return Notifization.Error("Vui lòng xóa tất cả khách hàng trước");
                    //
                    // get all file
                    List<string> lstImgFile = userResult.Select(m => m.ImageFile).ToList();
                    if (lstImgFile.Count > 0)
                    {
                        foreach (var item in lstImgFile)
                        {
                            AttachmentFile.DeleteFile(item, transaction: _transaction);
                        }
                    }
                    _connection.Execute("DELETE App_ClientLogin WHERE ClientType = @ClientType AND UserID IN ('" + String.Join("','", clientLogin.Select(m => m.UserID)) + "')", new { ClientType = (int)ClientLoginEnum.ClientType.Supplier }, transaction: _transaction);
                    //
                    _connection.Execute("DELETE UserInfo WHERE UserID IN ('" + String.Join("','", clientLogin.Select(m => m.UserID)) + "')", transaction: _transaction);
                    _connection.Execute("DELETE UserSetting WHERE UserID IN ('" + String.Join("','", clientLogin.Select(m => m.UserID)) + "')", transaction: _transaction);
                    _connection.Execute("DELETE UserLogin WHERE ID IN('" + String.Join("', '", clientLogin.Select(m => m.UserID)) + "')", transaction: _transaction);
                    //
                    supplierService.Remove(id, transaction: _transaction);

                    // delete in client table
                    // get all 
                    //string sqlLogin = "UPDATE UserSetting SET Enabled = -1 WHERE UserID IN ('" + String.Join("','", clientLogin.Select(m => m.UserID)) + "')";
                    //_connection.Execute(sqlLogin, transaction: _transaction);
                    //supplier.Enabled = (int)ModelEnum.Enabled.NONE;
                    //supplierService.Update(supplier, transaction: _transaction);
                    //
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
        public static string GetSupplierName(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return string.Empty;
                string query = string.Empty;
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT TOP (1) * FROM App_Supplier WHERE ID = @Query";
                SupplierService supplierService = new SupplierService();
                var model = supplierService.Query<Supplier>(sqlQuery, new { Query = id }).FirstOrDefault();
                return model.CodeID;
            }
            catch
            {
                return string.Empty;
            }
        }

        //##############################################################################################################################################################################################################################################################

        public static string DropdownList(string id)
        {
            string result = string.Empty;
            using (var service = new SupplierService())
            {
                var dtList = service.DataOption();
                if (dtList.Count > 0)
                {
                    foreach (var item in dtList)
                    {
                        string select = string.Empty;
                        if (!string.IsNullOrWhiteSpace(id) && item.ID.Equals(id.ToLower()))
                            select = "selected";
                        result += "<option value='" + item.ID + "' data-codeid='" + item.CodeID + "' " + select + ">" + item.CodeID + "-" + item.Title + "</option>";
                    }
                }
                return result;
            }

        }
        public List<SupplierOption> DataOption()
        {

            string sqlQuery = @"SELECT * FROM App_Supplier WHERE Enabled = 1 ORDER BY Title ASC";
            return _connection.Query<SupplierOption>(sqlQuery, new { }).ToList();

        }
        //##############################################################################################################################################################################################################################################################


        //##############################################################################################################################################################################################################################################################

    }
}