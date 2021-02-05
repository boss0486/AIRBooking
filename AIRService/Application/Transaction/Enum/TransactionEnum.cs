namespace WebCore.ENM
{
    public class TransactionEnum
    {
        public enum TransactionType
        {
            NONE = 0,
            IN = 1,
            OUT = 2,
        }
        public enum TransactionOriginal
        {
            NONE = 0,
            DEPOSIT = 1,
            SPENDING = 2,
            USER_SPENDING = 3
        }
    }
}