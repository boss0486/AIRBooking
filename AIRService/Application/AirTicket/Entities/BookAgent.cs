using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;
using System.Collections.Generic;
using WebCore.Model.Entities;
using WebCore.Services;

namespace WebCore.Entities
{
    [ConnectionString(DbConnect.ConnectionString.CMS)]
    [Table("App_BookAgent")]
    public partial class BookAgent
    {
        public BookAgent()
        {
            ID = Guid.NewGuid().ToString();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string BookOrderID { get; set; }
        public string AgentID { get; set; }
        public string AgentCode { get; set; }
        public string AgentName { get; set; }
        public string TicketingID { get; set; }
        public string TicketingName { get; set; } 
        public string TicketingPhone { get; set; } 
        public string TicketingEmail { get; set; } 
        public double AgentFee { get; set; }
        public double AgentPrice { get; set; }
        public string ProviderName { get; set; }
        public double ProviderFee { get; set; }
    }
    // model
    public class BookAgentCreateModel
    {
        public string AgentID { get; set; }
        public string AgentName { get; set; }
        public string ProviderName { get; set; }
        public double ProviderFee { get; set; }
        public double AgentFee { get; set; }
        public double AgentPrice { get; set; }
    }
    public class BookAgentUpdateModel : BookAgentCreateModel
    {
        public string ID { get; set; }
    }
    public class BookAgentIDModel
    {
        public string ID { get; set; }
    }
}