using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;
using WebCore.Model.Entities;

namespace WebCore.Entities
{
    public class WalletHistoryMessageModel
    {
        public bool Status { get; set; }
        public string Message { get; set; }
    }
}