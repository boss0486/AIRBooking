using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;
using WebCore.Model.Entities;

namespace WebCore.Entities
{
    public class WalletHistoryEnum
    {
        public enum WalletHistoryTransactionType
        {
            None = 0,
            INPUT = 1,
            OUTPUT = 2
        }
        public enum WalletHistoryTransactionOriginal
        {
            DEPOSIT = 1,
            DIRECTLY = 2
        }
    }
}