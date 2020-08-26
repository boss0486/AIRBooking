using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;
using WebCore.Model.Entities;

namespace WebCore.Entities
{
    [ConnectionString(DbConnect.ConnectionString.CMS)]
    [Table("App_TransactionPaymentHistory")]
    public partial class TransactionPaymentHistory : WEBModel
    {
        public TransactionPaymentHistory()
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
        public double Amount { get; set; }
        public int TransactionType { get; set; }
        public int TransactionOriginal { get; set; }
        public int Status { get; set; }

    }

    // model
    public class TransactionPaymentHistoryCreateModel
    {
        public string CustomerID { get; set; }
        public double Amount { get; set; }
        public double NewBalance { get; set; }
        public int TransactionType { get; set; }
        public int TransactionOriginal { get; set; }
    }
    public class TransactionPaymentHistoryUpdateModel : TransactionPaymentHistoryCreateModel
    {
        public string ID { get; set; }
    }
    public class TransactionPaymentHistoryIDModel
    {
        public string ID { get; set; }
    }
    //
    public partial class TransactionPaymentHistoryResult : WEBModelResult
    {
        public string ID { get; set; }
        public string CustomerID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
        public double Amount { get; set; }
        public int TransactionType { get; set; }
        public int TransactionOriginal { get; set; }
        public int Status { get; set; }
    }
}