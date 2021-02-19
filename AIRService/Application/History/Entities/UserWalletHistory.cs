﻿using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;
using WebCore.Model.Entities;

namespace WebCore.Entities
{
    [ConnectionString(DbConnect.ConnectionString.CMS)]
    [Table("App_UserWalletHistory")]
    public partial class UserWalletHistory : WEBModel
    {
        public UserWalletHistory()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string AgentID { get; set; }
        public string UserID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
        public double Amount { get; set; }
        public int TransactionType { get; set; }
        public int Status { get; set; }
    }
    // model
    public class UserWalletHistoryCreateModel
    {
        public string AgentID { get; set; }
        public string UserID { get; set; }
        public string FullName { get; set; }
        public double Amount { get; set; }
        public int TransactionType { get; set; }
    } 
    //
    public partial class UserWalletHistoryResult : WEBModelResult
    {
        public string ID { get; set; }
        public string FullName { get; set; } 
        public string Summary { get; set; }
        public string AgentID { get; set; }
        public string UserID { get; set; }
        public double Amount { get; set; }
        public int TransactionType { get; set; }
    } 
}