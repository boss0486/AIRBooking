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
    [Table("App_Customer")]
    public partial class Customer : WEBModel
    {
        public Customer()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string SupplierID { get; set; }
        public string TypeID { get; set; }
        public string CodeID { get; set; }
        public string NotationID { get; set; }
        public string ParentID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public string Summary { get; set; }
        public string Path { get; set; }
        public string Address { get; set; }
        public string CompanyPhone { get; set; }
        public string TaxCode { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public double DepositAmount { get; set; }
        public double SpendingLimit { get; set; }
        public int TermPayment { get; set; }
        public string RegisterID { get; set; }
    }

    public class CustomerResult : WEBModelResult
    {
        public string ID { get; set; }
        private string ParentID { get; set; }
        private string SupplierID { get; set; }
        [NotMapped]
        public string SupplierCode => CustomerService.GetCustomerCodeID(SupplierID);
        private string _typeId;
        public string TypeID
        {
            get
            {
                return CustomerTypeService.GetNameByID(_typeId);
            }
            set
            {
                _typeId = value;
            }
        }
        //[NotMapped]
        //public int TypeLevel
        //{
        //    get
        //    {
        //        return CustomerService.GetLevelByPath(Path);
        //    }
        //}
        public string CodeID { get; set; }
        public string NotationID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public string Summary { get; set; }
        public string Address { get; set; }
        public string CompanyPhone { get; set; }
        public string TaxCode { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public double DepositAmount { get; set; }
        public double SpendingLimit { get; set; }
        public int TermPayment { get; set; }

    }

    public class CustomerCreateModel
    {
        public string SupplierID { get; set; }
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
        public double DepositAmount { get; set; }
        public int TermPayment { get; set; }
        // 
        public string AccountID { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        //
        public int Enabled { get; set; }
    }

    public class CustomerUpdateModel : CustomerCreateModel
    {
        public string ID { get; set; }
    }
    public class CustomerOptionModel
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
    }
    public class CustomerIDModel
    {
        public string ID { get; set; }
    }
    public class CustomerTypeModel
    {
        public string CustomerType { get; set; }
    }
    public class CustomerSearchModel : SearchModel
    {
        public string TypeID { get; set; }
    }

    public class CustomerOption
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string CodeID { get; set; }
        public string ParentID { get; set; }
    }
}