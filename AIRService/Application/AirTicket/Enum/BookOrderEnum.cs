namespace WebCore.ENM
{
    public class BookOrderEnum
    {
        public enum BookOrderStatus
        {
            Booking = -1,
            None = 0,
            Cancel = 2,
            ExTicket = 3
        }
        public enum BookMailStatus
        {
            None = 0,
            Sent = 1,
        }

        public enum BookItineraryType
        {
            None = 0,
            Inland = 1,
            National = 2,
        } 
    }
}