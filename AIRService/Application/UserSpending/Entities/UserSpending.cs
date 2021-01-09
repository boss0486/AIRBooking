using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;
using WebCore.Model.Entities;

namespace WebCore.Entities
{
    [ConnectionString(DbConnect.ConnectionString.CMS)]
    [Table("App_UserSpending")]
    public partial class UserSpending : WEBModel
    {
        public UserSpending()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string Summary { get; set; }
        public string AgentID { get; set; }
        public string TicketingID { get; set; } 
        public double Amount { get; set; }
        public int Status { get; set; }
    }

    // model
    public class UserSpendingCreateModel
    {
        public string Title { get; set; }
        public string Summary { get; set; }
        public string AgentID { get; set; }
        public string TicketingID { get; set; }
        public double Amount { get; set; }
        public int Enabled { get; set; }

    }
    public class UserSpendingUpdateModel : UserSpendingCreateModel
    {
        public string ID { get; set; }
    }
    public class UserSpendingIDModel
    {
        public string ID { get; set; }
    }
    public class UserSpendingResult : WEBModelResult
    {

        public string ID { get; set; }
        public string CustomerID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
        public string SenderID { get; set; }
        public string SendUserID { get; set; }
        public string ReceivedID { get; set; }
        public string ReceivedUserID { get; set; }
        public double Amount { get; set; }
        public bool Status { get; set; }
    }
    public class UserSpendingOption
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
    }
}