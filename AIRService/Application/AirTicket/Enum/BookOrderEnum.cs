namespace WebCore.ENM
{
    public class BookOrderEnum
    {
        public enum BookOrderStatus
        {
            Booking = -1,
            None = 0,
            Exported = 1,
            Cancel = 2
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

        public enum BookFlightType
        {
            None = 0,
            FlightGo = 1,
            FlightReturn = 2
        }
    }
}