using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using WebCore.Model.Entities;
using WebCore.Services;

namespace WebCore.Entities
{

    [ConnectionString(DbConnect.ConnectionString.CMS)]
    [Table("App_Supplier")]
    public partial class Supplier : WEBModel
    {
        public Supplier()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string TypeID { get; set; }
        public string CodeID { get; set; }
        public string NotationID { get; set; }
        public string ParentID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public string Summary { get; set; }
        public string Address { get; set; }
        public string CompanyPhone { get; set; }
        public string TaxCode { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string Path { get; set; }
        public string RegisterID { get; set; }
    }

    public class SupplierResult : WEBModelResult
    {
        public string ID { get; set; }
        public string TypeID { get; set; }
        public string CodeID { get; set; }
        public string NotationID { get; set; }
        public string ParentID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public string Summary { get; set; }
        public string Address { get; set; }
        public string PhoneCompany { get; set; }
        public string TaxCode { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string Path { get; set; }
    }

    public class SupplierCreateModel
    {
        public string TypeID { get; set; }
        public string CodeID { get; set; }
        public string NotationID { get; set; }
        public string ParentID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Address { get; set; }
        public string CompanyPhone { get; set; }
        public string TaxCode { get; set; }
        // contact
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        //
        public string AccountID { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        //
        public int Enabled { get; set; }
    }

    public class SupplierUpdateModel : SupplierCreateModel
    {
        public string ID { get; set; }
    }
    public class SupplierOptionModel
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
    }
    public class SupplierIDModel
    {
        public string ID { get; set; }
    }
    public class SupplierSearchModel : SearchModel
    {
        public string TypeID { get; set; }
    }

    public class SupplierOption
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string CodeID { get; set; }
    }
}