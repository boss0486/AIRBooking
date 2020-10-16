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
    [Table("App_TransactionCustomerDepositHistory")]
    public partial class TrasactionCustomerDepositHistory : WEBModel
    {
        public TrasactionCustomerDepositHistory()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
        public string SenderID { get; set; }
        public string CustomerID { get; set; }
        public double Amount { get; set; }
        public int TransactionType { get; set; }
        public int TransactionOriginalType { get; set; }
        public string TransactionOriginalID { get; set; }
        public int Status { get; set; }

    }

    // model
    public class TransactionCustomerDepositHistoryCreateModel
    {
        public string SenderID { get; set; }
        public string CustomerID { get; set; }
        public double Amount { get; set; }
        public double NewBalance { get; set; }
        public int TransactionType { get; set; }
        public int TransactionOriginalType { get; set; }
        public string TransactionOriginalID { get; set; }
    }
    public class WalletCustomerDepositHistoryUpdateModel : TransactionCustomerDepositHistoryCreateModel
    {
        public string ID { get; set; }
    }
    public class TrasactionCustomerDepositHistoryIDModel
    {
        public string ID { get; set; }
    }
    //
    public partial class TransactionCustomerDepositHistoryResult : WEBModelResult
    {
        public string ID { get; set; }
        public string SenderID { get; set; }
        public string CustomerID { get; set; }
        [NotMapped]
        public string CustomerCodeID
        {
            get
            {
                return CustomerService.GetCustomerCodeID(CustomerID);
            }
        }

        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
        public double Amount { get; set; }
        public int TransactionType { get; set; } 
        [NotMapped]
        public string TransactionTypeText
        {
            get
            {
                return TransactionCustomerDepositHistoryService.TransactionTypeText(TransactionType);
            }
        }

        public int TransactionOriginal { get; set; }
        [NotMapped]
        public string TransactionOriginalText
        {
            get
            {
                return TransactionCustomerDepositHistoryService.TransactionOriginalText(TransactionType);
            }
        } 
        public string TransactionOriginalID { get; set; }
        public int Status { get; set; }
    }
}