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
    }
}