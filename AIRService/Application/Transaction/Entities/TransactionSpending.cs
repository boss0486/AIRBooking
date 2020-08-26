using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;
using WebCore.Model.Entities;

namespace WebCore.Entities
{
    [ConnectionString(DbConnect.ConnectionString.CMS)]
    [Table("App_TransactionSpending")]
    public partial class TransactionSpending : WEBModel
    {
        public TransactionSpending()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string CustomerID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
        public string UserIDSend { get; set; }
        public double Amount { get; set; }
        public int Status { get; set; }
    }

    // model
    public class TransactionSpendingCreateModel
    {
        public string CustomerID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public double Amount { get; set; }
        public int Enabled { get; set; }

    }
    public class TransactionSpendingUpdateModel : TransactionSpendingCreateModel
    {
        public string ID { get; set; }
    }
    public class TransactionSpendingIDModel
    {
        public string ID { get; set; }
    }
    public class TransactionSpendingResult : WEBModelResult
    {

        public string ID { get; set; }
        public string CustomerID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
        public string UserIDSend { get; set; }
        public double Amount { get; set; }
        public bool Status { get; set; }
    }
    public class TransactionSpendingOption
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
    }
}