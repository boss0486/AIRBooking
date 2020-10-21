using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;
using WebCore.Model.Entities;

namespace WebCore.Entities
{
    public class WalletUserMessageModel
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public double Balance { get; set; }
    }

    public class WalletClientMessageModel
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        //public string ClientID { get; set; }
        //public int ClientType { get; set; }
        public double InvestedAmount { get; set; }
        public double SpendingLimitBalance { get; set; }
        public double DepositBalance { get; set; }
        public double SpendingBalance { get; set; }
    }
}