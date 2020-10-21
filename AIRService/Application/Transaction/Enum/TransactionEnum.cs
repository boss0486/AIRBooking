﻿namespace WebCore.ENM
{
    public class TransactionEnum
    {
        public enum TransactionType
        {
            IN = 1,
            OUT = 2
        }
        public enum TransactionOriginal
        {
            NONE = 0,
            DEPOSIT = 1,
            SPENDING = 2
        }
    }
}