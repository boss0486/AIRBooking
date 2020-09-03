
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper.Time
{

    public class TimeHelper
    {
        public static string GetDateByTimeZone(string zoneId)
        {
            try
            {
               
                var timezone = TimeZoneInfo.FindSystemTimeZoneById(zoneId);
                if (timezone== null)
                {
                    return string.Empty;
                }
                var dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timezone);
                string date = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
                return date;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        // date time
        public static string FormatToDate(DateTime dtime, string languageCode)
        {
            try
            {
                if (languageCode == Language.LanguageCode.Vietnamese.ID)
                    return dtime.ToString("dd-MM-yyyy");
                else
                    return dtime.ToString("yyyy-MM-dd");

            }
            catch (Exception)
            {
                return string.Empty;

            }
        }
        public static string FormatToDate(string _strDate)
        {
            try
            {
                // Check for empty string.
                if (string.IsNullOrEmpty(_strDate))
                    return string.Empty;

                bool isDateTime = DateTime.TryParse(_strDate, out _);
                if (isDateTime)
                {
                    DateTime dateTime = Convert.ToDateTime(_strDate);
                    string _month = Convert.ToString(dateTime.Month);
                    string _day = Convert.ToString(dateTime.Day);
                    if (_month.Length == 1)
                        _month = "0" + _month;
                    if (_day.Length == 1)
                        _day = "0" + _day;
                    return _day + "/" + _month + "/" + dateTime.Year;
                }
                return "00/00/0000";
            }
            catch (Exception)
            {
                return "00/00/0000";
            }
        }
        public static string FormatToDateTime(DateTime dtime, string languageCode)
        {
            try
            {
                if (languageCode == Language.LanguageCode.Vietnamese.ID)
                    return dtime.ToString("dd-MM-yyyy HH:mm:ss");
                else
                    return dtime.ToString("yyyy-MM-dd HH:mm:ss");

            }
            catch (Exception)
            {
                return string.Empty;

            }
        }
        public static string FormatToNewsDate(DateTime dtime)
        {
            try
            {
                DateTime dt = new DateTime(dtime.Year, dtime.Month, dtime.Day, dtime.Hour, dtime.Minute, dtime.Second, dtime.Millisecond);
                return String.Format("{0:MMMM dd, yyyy}", dt);
            }
            catch (Exception)
            {
                dtime = DateTime.Now;
                DateTime dt = new DateTime(dtime.Year, dtime.Month, dtime.Day, dtime.Hour, dtime.Minute, dtime.Second, dtime.Millisecond);
                return String.Format("{0:MMMM dd, yyyy}", dt);

            }
        }
        public static string FormatToDateTimeSQL(DateTime dtime, string ext = null)
        {
            try
            {
                return dtime.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch (Exception)
            {
                dtime = DateTime.Now;
                return dtime.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
        public static string FormatToDateSQL(DateTime dtime)
        {
            try
            {
                return dtime.ToString("yyyy-MM-dd");
            }
            catch (Exception)
            {
                dtime = DateTime.Now;
                return dtime.ToString("yyyy-MM-dd");

            }
        }
        public static DateTime FormatToDateSQL(string dtime)
        {
            if (!string.IsNullOrWhiteSpace(dtime))
            {
                dtime = dtime.Trim();
                if (dtime.Contains('-'))
                {
                    string[] arrtime = dtime.Split('-');
                    string date = arrtime[2] + "-" + arrtime[1] + "-" + arrtime[0];
                    return Convert.ToDateTime(date);
                }
                if (dtime.Contains('/'))
                {
                    dtime = dtime.Replace("/", "-");
                    string[] arrtime = dtime.Split('-');
                    string date = arrtime[2] + "-" + arrtime[1] + "-" + arrtime[0];
                    return Convert.ToDateTime(date);
                }
            }
            // 19-05-2019
            return DateTime.Now;
        }
        public static string FormatToUTCDateTime(DateTime dtime, string ext = "-")
        {
            try
            {
                return dtime.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        public static string FormatDateTimeToYYMM(DateTime dtime)
        {
            try
            {
                return dtime.ToString("yyyy-MM");
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        // get data
        public static DateTime TimeNow(IDbTransaction transaction = null)
        {
            try
            {
                using (var _connectDb = DbConnect.Connection.CMS)
                {
                    string sqlQuery = "SELECT GETDATE() AS [DTime]";
                    var data = _connectDb.Query<string>(sqlQuery, transaction: transaction).FirstOrDefault();
                    return Convert.ToDateTime(data);
                }
            }
            catch (Exception)
            {
                return DateTime.Now;
            }
        }
    }
    //
    public class TimeZoneID
    {
        public static string vn_zonetime = "SE Asia Standard Time";
        public static string us_zonetime = "Central America Standard Time";
    }
}
