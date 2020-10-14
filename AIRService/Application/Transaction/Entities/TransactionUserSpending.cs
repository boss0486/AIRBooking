using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;
using WebCore.Model.Entities;

namespace WebCore.Entities
{
    [ConnectionString(DbConnect.ConnectionString.CMS)]
    [Table("App_TransactionUserSpending")]
    public partial class TransactionUserSpending : WEBModel
    {
        public TransactionUserSpending()
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
        public string SendUserID { get; set; }
        public string ReceivedUserID { get; set; }
        public double Amount { get; set; }
        public int Status { get; set; }
    }

    // model
    public class TransactionUserSpendingCreateModel
    {
        public string CustomerID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string ReceivedUserID { get; set; }
        public double Amount { get; set; }
        public int Enabled { get; set; }

    }
    public class TransactionUserSpendingUpdateModel : TransactionUserSpendingCreateModel
    {
        public string ID { get; set; }
    }
    public class TransactionUserSpendingIDModel
    {
        public string ID { get; set; }
    }
    public class TransactionUserSpendingResult : WEBModelResult
    {

        public string ID { get; set; }
        public string CustomerID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
        public string SendUserID { get; set; }
        public string ReceivedUserID { get; set; }
        public double Amount { get; set; }
        public bool Status { get; set; }
    }
    public class TransactionUserSpendingOption
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
    }
}