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
    [Table("App_TransactionDeposit")]
    public partial class TransactionDeposit : WEBModel
    {
        public TransactionDeposit()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string CustomerID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public string Summary { get; set; }
        public string TransactionID { get; set; }
        public string BankSent { get; set; }
        public string BankIDSent { get; set; }
        public string BankReceived { get; set; }
        public string BankIDReceived { get; set; }
        public DateTime ReceivedDate { get; set; }
        public double Amount { get; set; }
        public int Status { get; set; }
    }

    // model
    public class TransactionDepositCreateModel
    {
        public string CustomerID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string TransactionID { get; set; }
        public string BankSent { get; set; }
        public string BankIDSent { get; set; }
        public string BankReceived { get; set; }
        public string BankIDReceived { get; set; }
        public string ReceivedDate { get; set; }
        public double Amount { get; set; }
        public int Enabled { get; set; }

    }
    public class TransactionDepositUpdateModel : TransactionDepositCreateModel
    {
        public string ID { get; set; }
    }
    public class TransactionDepositIDModel
    {
        public string ID { get; set; }
    }
    public class TransactionDepositResult : WEBModelResult
    {

        public string ID { get; set; }
        private string _customerId;
        public string CustomerID
        {
            get
            {
                return CustomerService.GetCustomerCodeID(_customerId);
            }
            set
            {
                _customerId = value;
            }
        }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
        public string TransactionID { get; set; }
        public string BankSent { get; set; }
        public string BankIDSent { get; set; }
        public string BankReceived { get; set; }
        public string BankIDReceived { get; set; }
        private string _receivedDate;
        public string ReceivedDate
        {
            get
            {
                return Helper.Time.TimeHelper.FormatToDate(Convert.ToDateTime(_receivedDate), Helper.Language.LanguageCode.Vietnamese.ID);
            }
            set
            {
                _receivedDate = value;
            }
        }
        public double Amount { get; set; }
        public int Status { get; set; }
    }
    public class TransactionDepositOption
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
    }
}