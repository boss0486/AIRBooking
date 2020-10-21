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
    [Table("App_WalletDepositHistory")]
    public partial class WalletDepositHistory : WEBModel
    {
        public WalletDepositHistory()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string SenderUserID { get; set; }
        public string SenderID { get; set; }
        public string ReceivedID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
        public double Amount { get; set; }
        public int TransactionType { get; set; }
        public int TransactionOriginal { get; set; }
        public string TransactionOriginalID { get; set; }
        public int Status { get; set; }

    }

    // model
    public class WalletDepositHistoryCreateModel
    {
        public string SenderUserID { get; set; }
        public string SenderID { get; set; }
        public string ReceivedID { get; set; }
        public double Amount { get; set; }
        public double NewBalance { get; set; }
        public int TransactionType { get; set; }
        public int TransactionOriginal { get; set; }
        public string TransactionOriginalID { get; set; }
    }
    public class WalletDepositHistoryUpdateModel : WalletDepositHistoryCreateModel
    {
        public string ID { get; set; }
    }
    public class WalletDepositHistoryIDModel
    {
        public string ID { get; set; }
    }
    //
    public partial class WalletDepositHistoryResult : WEBModelResult
    {
        public string ID { get; set; }

        public string SenderUserID { get; set; }
        public string SenderName
        {
            get
            {
                return Helper.User.InFormation.GetFullName(SenderUserID);
            }
        }
 
        string _senderId { get; set; }
        public string SenderID
        {
            get
            {
                return SupplierService.GetSupplierCodeID(_senderId);
            }
            set
            {
                _senderId = value;
            }
        }

        string _receivedId { get; set; }
        public string ReceivedID
        {
            get
            {
                return CustomerService.GetCustomerCodeID(_receivedId);
            }
            set
            {
                _receivedId = value;
            }
        }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
        public double Amount { get; set; }
        public int TransactionType { get; set; }
        public int TransactionOriginal { get; set; }
        public string TransactionOriginalID { get; set; }
        public int Status { get; set; }
    }
}