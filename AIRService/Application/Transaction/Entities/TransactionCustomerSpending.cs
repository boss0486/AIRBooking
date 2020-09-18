using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;
using WebCore.Model.Entities;
using WebCore.Services;

namespace WebCore.Entities
{
    [ConnectionString(DbConnect.ConnectionString.CMS)]
    [Table("App_TransactionCustomerSpending")]
    public partial class TransactionCustomerSpending : WEBModel
    {
        public TransactionCustomerSpending()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
        public string SupplierID { get; set; }
        public string SupplierUserID { get; set; }
        public string CustomerID { get; set; }
        public double Amount { get; set; }
        public int Status { get; set; }
    }

    // model
    public class TransactionCustomerSpendingCreateModel
    {
        public string SupplierID { get; set; }
        public string CustomerID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public double Amount { get; set; }
        public int Enabled { get; set; }
    }
    public class TransactionCustomerSpendingUpdateModel : TransactionCustomerSpendingCreateModel
    {
        public string ID { get; set; }
    }
    public class TransactionSpendingIDModel
    {
        public string ID { get; set; }
    }
    public class TransactionCustomerSpendingResult : WEBModelResult
    {

        public string ID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
        public string SupplierID { get; set; }
        [NotMapped]
        public string SupplierCodeID
        {
            get
            {
                return SupplierService.GetSupplierCodeID(SupplierID);
            }
        }
        public string SupplierUserID { get; set; }
        public string CustomerID { get; set; }
        [NotMapped]
        public string CustomerCodeID
        {
            get
            {
                return CustomerService.GetCustomerCodeID(CustomerID);
            }
        }
        public double Amount { get; set; }
        public bool Status { get; set; }
    }
    public class TransactionCustomerSpendingOption
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
    }
}