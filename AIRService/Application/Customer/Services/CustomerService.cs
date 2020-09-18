﻿using AL.NetFrame.Attributes;
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
    public interface ICustomerService : IEntityService<Customer> { }
    public class CustomerService : EntityService<Customer>, ICustomerService
    {
        public CustomerService() : base() { }
        public CustomerService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public ActionResult DataList(CustomerSearchModel model)
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
            string typeId = model.TypeID;
            if (!string.IsNullOrWhiteSpace(typeId))
            {
                whereCondition += " AND TypeID = @TypeID";
            }
            //
            if (Helper.Current.UserLogin.IsCustomerLogged())
            {
                // get customerid
                string clientId = ClientLoginService.GetClientIDByUserID(Helper.Current.UserLogin.IdentifierID);
                whereCondition += " AND Path LIKE N'" + clientId + "%'";
            }
            else
            if (Helper.Current.UserLogin.IsSupplierLogged())
            {
                // get customerid
                string clientId = ClientLoginService.GetClientIDByUserID(Helper.Current.UserLogin.IdentifierID);
                whereCondition += " AND SupplierID = '" + clientId + "'";
            }

            string sqlQuery = @"SELECT * FROM App_Customer WHERE (dbo.Uni2NONE(Title) LIKE N'%'+ @Query +'%' OR CodeID LIKE N'%'+ @Query +'%') " + whereCondition + " ORDER BY [CreatedDate]";
            var dtList = _connection.Query<CustomerResultModel>(sqlQuery, new { Query = Helper.Page.Library.FormatToUni2NONE(query), TypeID = typeId }).ToList();
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
        public ActionResult Create(CustomerCreateModel model)
        {
            _connection.Open();
            using (var _transaction = _connection.BeginTransaction())
            {
                try
                {
                    if (model == null)
                        return Notifization.Invalid();
                    //
                    string supplierId = model.SupplierID;
                    string typeId = model.TypeID;
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
                    CustomerService customerService = new CustomerService(_connection);
                    SupplierService supplierService = new SupplierService(_connection);

                    string alias = Helper.Page.Library.FormatToUni2NONE(title);
                    if (model == null)
                        return Notifization.Invalid();
                    //
                    ClientLoginService clientLoginService = new ClientLoginService(_connection);
                    //return Notifization.TEST(":::" + Helper.Current.UserLogin.IdentifierID.ToLower());
                    string currentUserId = Helper.Current.UserLogin.IdentifierID;
                    var userService = new UserService();

                    bool customerLoginStatus = userService.IsCustomerLogged(currentUserId, dbConnection: _connection, _transaction);
                    bool supplierLoginStatus = userService.IsSupplierLogged(currentUserId, dbConnection: _connection, _transaction);
                    string suppId = string.Empty;
                    if (userService.IsCustomerLogged(currentUserId, dbConnection: _connection, _transaction))
                    {
                        var client = clientLoginService.GetAlls(m => m.UserID == currentUserId && m.ClientType == (int)ClientLoginEnum.ClientType.Customer, transaction: _transaction).FirstOrDefault();
                        if (client == null)
                            return Notifization.Invalid("Không thể xác định nhà cung cấp 01");
                        //
                        var customer1 = customerService.GetAlls(m => m.ID == client.ClientID, transaction: _transaction).FirstOrDefault();
                        if (customer1 == null)
                            return Notifization.Invalid("Không thể xác định nhà cung cấp 02");
                        //
                        supplierId = customer1.SupplierID;
                        parentId = customer1.ID;
                        // path
                        if (string.IsNullOrWhiteSpace(customer1.Path))
                            path = customer1.ID;
                        else
                            path = customer1.Path;
                    }
                    else if (userService.IsSupplierLogged(currentUserId, dbConnection: _connection, _transaction))
                    {
                        var client = clientLoginService.GetAlls(m => m.UserID == currentUserId && m.ClientType == (int)ClientLoginEnum.ClientType.Supplier, transaction: _transaction).FirstOrDefault();
                        if (client == null)
                            return Notifization.Invalid("Không thể xác định nhà cung cấp 03");
                        //
                        var supplier = supplierService.GetAlls(m => m.ID == client.ClientID, transaction: _transaction).FirstOrDefault();
                        if (supplier == null)
                            return Notifization.Invalid("Không thể xác định nhà cung cấp 04");
                        //
                        supplierId = supplier.ID;
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(supplierId))
                            return Notifization.Invalid("Vui lòng chọn nhà cung cấp 05");
                        //
                        supplierId = supplierId.Trim().ToLower();
                        //
                        var supplier = supplierService.GetAlls(m => m.ID == supplierId, transaction: _transaction).FirstOrDefault();
                        if (supplier == null)
                            return Notifization.Invalid("Không thể xác định nhà cung cấp 06");
                        //
                    }
                    // CHECK CREATE AGENT *************************************************************
                    // 1.neu phai admin, admin app, nha cung cap, khach hang se ko dc tao khach hang
                    // 1.neu la khach hang ko dc tao them dai ly, chỉ dc tạo comp
                    if (!Helper.Current.UserLogin.IsCMSUser && !Helper.Current.UserLogin.IsAdminInApplication && !Helper.Current.UserLogin.IsSupplierLogged() && !Helper.Current.UserLogin.IsCustomerLogged())
                        return Notifization.Invalid("Không thể tạo khách hàng");
                    //
                    if (Helper.Current.UserLogin.IsCustomerLogged() && CustomerTypeService.GetCustomerType(typeId) == (int)CustomerEnum.CustomerType.AGENT)
                        return Notifization.Invalid("Không thể tạo đại lý");
                    //
                    if (string.IsNullOrWhiteSpace(typeId))
                        return Notifization.Invalid("Vui lòng chọn loại khách hàng");
                    typeId = typeId.Trim();
                    //
                    if (string.IsNullOrWhiteSpace(codeId))
                        return Notifization.Invalid("Không được để trống mã khách hàng");
                    codeId = codeId.Trim();
                    if (codeId.Length != 3)
                        return Notifization.Invalid("Mã khách hàng bao gồm 3 ký tự");

                    if (!Validate.TestAlphabet(codeId))
                        return Notifization.Invalid("Mã khách hàng không hợp lệ");
                    // 
                    if (string.IsNullOrWhiteSpace(title))
                        return Notifization.Invalid("Không được để trống tên khách hàng");
                    title = title.Trim();
                    if (!Validate.TestText(title))
                        return Notifization.Invalid("Tên khách hàng không hợp lệ");
                    //
                    if (title.Length < 2 || title.Length > 80)
                        return Notifization.Invalid("Tên khách hàng giới hạn 2-80 ký tự");
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
                    if (CustomerTypeService.GetCustomerType(typeId) != (int)CustomerEnum.CustomerType.COMP)
                    {
                        if (depositAmount <= 0)
                            return Notifization.Invalid("Số tiền đặt cọc phải > 0");
                    }
                    else
                    {
                        depositAmount = 0;
                    }
                    // 
                    if (termPayment < 0)
                        return Notifization.Invalid("Thời hạn thanh toán phải > 0");
                    //
                    var customer = customerService.GetAlls(m => !string.IsNullOrWhiteSpace(m.CodeID) && m.CodeID.ToLower() == codeId.ToLower(), transaction: _transaction).FirstOrDefault();
                    if (customer != null)
                        return Notifization.Invalid("Mã khách hàng đã được sử dụng");

                    customer = customerService.GetAlls(m => !string.IsNullOrWhiteSpace(m.Title) && m.Title.ToLower() == title.ToLower(), transaction: _transaction).FirstOrDefault();
                    if (customer != null)
                        return Notifization.Invalid("Tên khách hàng đã được sử dụng");
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
                    WalletCustomerService balanceCustomerService = new WalletCustomerService(_connection);
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
                    var customerId = customerService.Create<string>(new Customer()
                    {
                        SupplierID = supplierId,
                        TypeID = typeId,
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

                    //******* create balance 
                    var balanceCustomer = balanceCustomerService.Create<string>(new WalletCustomer()
                    {
                        CustomerID = customerId,
                        SpendingAmount = 0,
                        DepositAmount = 0

                    }, transaction: _transaction);
                    //
                    var clientId = clientLoginService.Create<string>(new ClientLogin()
                    {
                        ClientID = customerId,
                        ClientType = (int)ClientLoginEnum.ClientType.Customer,
                        UserID = userId
                    }, transaction: _transaction);


                    // update path
                    if (!string.IsNullOrWhiteSpace(path))
                        path += "/" + customerId;
                    else
                        path += customerId;
                    //
                    customer = customerService.GetAlls(m => m.ID == customerId, transaction: _transaction).FirstOrDefault();
                    customer.Path = path;
                    customerService.Update(customer, transaction: _transaction);
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
        public ActionResult Update(CustomerUpdateModel model)
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
                    string typeId = model.TypeID;
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
                    //if (string.IsNullOrWhiteSpace(typeId))
                    //    return Notifization.Invalid("Vui lòng chọn loại khách hàng");
                    //typeId = typeId.Trim();
                    //
                    //if (string.IsNullOrWhiteSpace(codeId))
                    //    return Notifization.Invalid("Không được để trống mã khách hàng");
                    //codeId = codeId.Trim();
                    //if (codeId.Length != 3)
                    //    return Notifization.Invalid("Mã khách hàng bao gồm 3 ký tự");

                    //if (!Validate.TestAlphabet(codeId))
                    //    return Notifization.Invalid("Mã khách hàng không hợp lệ");
                    // 
                    if (string.IsNullOrWhiteSpace(title))
                        return Notifization.Invalid("Không được để trống tên khách hàng");
                    title = title.Trim();
                    if (!Validate.TestText(title))
                        return Notifization.Invalid("Tên khách hàng không hợp lệ");
                    //
                    if (title.Length < 2 || title.Length > 80)
                        return Notifization.Invalid("Tên khách hàng giới hạn 2-80 ký tự");
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
                    if (CustomerTypeService.GetCustomerType(typeId) != (int)CustomerEnum.CustomerType.COMP)
                    {
                        if (depositAmount <= 0)
                            return Notifization.Invalid("Số tiền đặt cọc phải > 0");
                    }
                    //
                    if (termPayment < 0)
                        return Notifization.Invalid("Thời hạn thanh toán phải > 0");
                    //
                    CustomerService customerService = new CustomerService(_connection);
                    var customer = customerService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.ID == id, transaction: _transaction).FirstOrDefault();
                    if (customer == null)
                        return Notifization.NotFound();
                    //
                    var modelTitle = customerService.GetAlls(m => !string.IsNullOrWhiteSpace(m.Title) && m.Title.ToLower() == title.ToLower() && m.ID != id, transaction: _transaction).FirstOrDefault();
                    if (modelTitle != null)
                        return Notifization.Invalid("Tên khách hàng đã được sử dụng");

                    var customerId = customerService.GetAlls(m => !string.IsNullOrWhiteSpace(m.CodeID) && m.CodeID.ToLower() == codeId.ToLower() && m.ID != id, transaction: _transaction).FirstOrDefault();
                    if (customerId != null)
                        return Notifization.Invalid("Tên khách hàng đã được sử dụng");

                    // update content
                    //customer.CustomerType = typeId;
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
        public Customer GetCustomerModel(string Id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Id))
                    return null;
                string query = string.Empty;
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT TOP (1) * FROM App_Customer WHERE ID = @Query";
                var model = _connection.Query<Customer>(sqlQuery, new { Query = Id }).FirstOrDefault();
                return model;
            }
            catch
            {
                return null;
            }
        }
        //########################################################################tttt######################################################################################################################################################################################
        public ActionResult Delete(CustomerIDModel model)
        {
            _connection.Open();
            using (var _transaction = _connection.BeginTransaction())
            {
                try
                {
                    if (model == null)
                        return Notifization.Error(MessageText.Invalid);
                    //
                    string id = model.ID.ToLower();
                    CustomerService customerService = new CustomerService(_connection);
                    var customer = customerService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.ID == id, transaction: _transaction).FirstOrDefault();
                    if (customer == null)
                        return Notifization.NotFound();
                    // get all user
                    ClientLoginService clientLoginService = new ClientLoginService(_connection);
                    var clientLogin = clientLoginService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ClientID) && m.ClientID == id && m.ClientType == (int)ClientLoginEnum.ClientType.Customer, transaction: _transaction).ToList();
                    if (clientLogin.Count == 0)
                        return Notifization.Error(MessageText.Invalid);
                    //
                    string sqlQuery = @"SELECT * FROM View_User WHERE ID IN ('" + String.Join("','", clientLogin.Select(m => m.UserID)) + "')";
                    var userResult = _connection.Query<UserResult>(sqlQuery, transaction: _transaction).ToList();
                    if (userResult.Count == 0)
                        return Notifization.Error(MessageText.Invalid);
                    // check data reference
                    // #1. Customer
                    string sqlCustomer = @"SELECT ID FROM App_Customer WHERE ParentID = @ParentID";
                    var customerId = _connection.Query<CustomerIDModel>(sqlCustomer, new { ParentID = id }, transaction: _transaction).ToList();
                    if (customerId.Count > 0)
                        return Notifization.Error("Vui lòng xóa tất cả các khách hàng trước");
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
                    _connection.Execute("DELETE App_ClientLogin WHERE ClientType = @ClientType AND UserID IN ('" + String.Join("','", clientLogin.Select(m => m.UserID)) + "')", new { ClientType = (int)ClientLoginEnum.ClientType.Customer }, transaction: _transaction);
                    //
                    _connection.Execute("DELETE UserInfo WHERE UserID IN ('" + String.Join("','", clientLogin.Select(m => m.UserID)) + "')", transaction: _transaction);
                    _connection.Execute("DELETE UserSetting WHERE UserID IN ('" + String.Join("','", clientLogin.Select(m => m.UserID)) + "')", transaction: _transaction);
                    _connection.Execute("DELETE UserLogin WHERE ID IN('" + String.Join("', '", clientLogin.Select(m => m.UserID)) + "')", transaction: _transaction);
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
        //##############################################################################################################################################################################################################################################################
        public static string DropdownList(string id)
        {
            string result = string.Empty;
            using (var service = new CustomerService())
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
        public List<CustomerOption> DataOption()
        {
            string sqlQuery = @"SELECT ID, Title, CodeID FROM App_Customer WHERE Enabled = 1 ORDER BY Title ASC";
            return _connection.Query<CustomerOption>(sqlQuery, new { }).ToList();
        }

        public List<CustomerOption> GetCustomerBySupplierIDOption(string supplierId)
        {
            if (string.IsNullOrWhiteSpace(supplierId))
                return new List<CustomerOption>();
            //
            string sqlQuery = @"SELECT ID, Title, CodeID FROM App_Customer WHERE SupplierID = @SupplierID AND Enabled = 1 ORDER BY Title ASC";
            return _connection.Query<CustomerOption>(sqlQuery, new { SupplierID = supplierId }).ToList();
        }

        public static string DropdownListWithTypeID(int typeEnum, string id)
        {
            string result = string.Empty;
            using (var service = new CustomerService())
            {
                string typeId = CustomerTypeService.GetCustomerTypeIDByType(typeEnum);
                if (!Helper.Current.UserLogin.IsCMSUser && !Helper.Current.UserLogin.IsAdminInApplication && !Helper.Current.UserLogin.IsSupplierLogged())
                    return result;
                //
                // limit by user login
                string whereCondition = string.Empty;
                if (Helper.Current.UserLogin.IsSupplierLogged())
                {
                    string userId = Helper.Current.UserLogin.IdentifierID;
                    string supplierId = ClientLoginService.GetClientIDByUserID(userId);
                    whereCondition += " AND SupplierID = '" + supplierId + "'";
                }
                //
                string sqlQuery = @"SELECT * FROM App_Customer WHERE TypeID = @TypeID  " + whereCondition + " AND Enabled = 1 ORDER BY Title ASC";
                var dtList = service.Query<CustomerOption>(sqlQuery, new { TypeID = typeId }).ToList();
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


        //##############################################################################################################################################################################################################################################################
        public static string DropdownListCustomerChildByLogin(string id)
        {
            try
            {
                string result = string.Empty;
                using (var service = new CustomerService())
                {
                    string whereCondition = string.Empty;
                    if (Helper.Current.UserLogin.IsCustomerLogged())
                    {
                        // get customer
                        string sqlQuery = @"SELECT TOP 1 * FROM App_ClientLogin where UserID = @UserID AND ClientType = 1";
                        var clientLogin = service.Query<ClientLogin>(sqlQuery, new { }).FirstOrDefault();
                        if (clientLogin == null)
                            return string.Empty;
                        //
                        string customerId = clientLogin.ClientID;
                        sqlQuery = @"SELECT * FROM App_Customer WHERE ParentID = @ParentID AND Enabled = 1 ORDER BY Title ASC";
                        var customer = service.Query<CustomerOption>(sqlQuery, new { ParentID = customerId }).ToList();
                        if (customer.Count == 0)
                            return string.Empty;
                        //
                        foreach (var item in customer)
                        {
                            string select = string.Empty;
                            if (!string.IsNullOrWhiteSpace(id) && item.ID == id.ToLower())
                                select = "selected";
                            result += "<option value='" + item.ID + "' data-codeid= '" + item.CodeID + "' " + select + ">" + item.Title + "</option>";
                        }
                        return result;
                    }
                    else
                    {
                        string sqlQuery = @"SELECT * FROM App_Customer WHERE ParentID IS NULL AND Enabled = 1 ORDER BY Title ASC";
                        var customer = service.Query<CustomerOption>(sqlQuery, new { }).ToList();
                        if (customer.Count == 0)
                            return string.Empty;
                        //
                        foreach (var item in customer)
                        {
                            string select = string.Empty;
                            if (!string.IsNullOrWhiteSpace(id) && item.ID == id.ToLower())
                                select = "selected";
                            result += "<option value='" + item.ID + "' data-codeid= '" + item.CodeID + "' " + select + ">" + item.Title + "</option>";
                        }
                        return result;
                    }
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        //##############################################################################################################################################################################################################################################################
        public static string GetCustomerCodeID(string id)
        {

            if (string.IsNullOrWhiteSpace(id))
                return string.Empty;
            //
            id = id.ToLower();
            using (var service = new CustomerService())
            {
                var customer = service.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.ID == id).FirstOrDefault();
                if (customer == null)
                    return string.Empty;
                //
                return customer.CodeID;

            }

        }
        public static string GetInforType(string typeId, string path)
        {
            try
            {
                var service = new CustomerTypeService();
                string typeName = CustomerTypeService.GetNameByID(typeId);
                if (CustomerTypeService.GetCustomerType(typeId) == (int)CustomerEnum.CustomerType.AGENT)
                {
                    int level = CustomerService.GetLevelByPath(path);
                    string strLevel = level + "";
                    if (level < 10)
                        strLevel = "0" + level;
                    //
                    return typeName + " cấp: " + strLevel;
                }
                return typeName;
            }
            catch
            {
                return string.Empty;
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
                return customer.ClientID;

            }

        }
        //##############################################################################################################################################################################################################################################################

    }
}