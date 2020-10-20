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

    public class WalletCustomerMessageModel
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public double SpendingLimitBalance { get; set; }
        public double DepositBalance { get; set; }
        public double SpendingBalance { get; set; }
    }

    public class WalletCustomerModel
    {
        public double SpendingTotal { get; set; }
        public double DepositTotal { get; set; }
    }
    public class WalletUserModel
    {
        public double SpendingTotal { get; set; }
    }
}