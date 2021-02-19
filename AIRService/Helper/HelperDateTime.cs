
using Dapper;
using Helper.Language;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebCore.Model.Enum;

namespace Helper.TimeData
{
    public class TimeHelper
    {
        /// <summary>
        /// Fomat: yyyy-MM-dd HH:mm:ss
        /// </summary>
        public static DateTime UtcDateTime => Convert.ToDateTime(TimeHelper.GetUtcDateTimeTx);

        public static string GetUtcDateTimeTx
        {
            get
            {
                string timeZoon = string.Empty;
                string lgCode = Helper.Language.LanguagePage.GetLanguageCode;
                // Vietnamese
                if (lgCode == Language.LanguageCode.Vietnamese.ID)
                    timeZoon = TimeZoneID.vn_zonetime;
                // English
                if (lgCode == Language.LanguageCode.English.ID)
                    timeZoon = TimeZoneID.us_zonetime;
                //*************************************************************
                if (string.IsNullOrWhiteSpace(timeZoon))
                    return string.Empty;
                //
                string strDateTime = TimeFormat.GetDateTimeByTimeZone(timeZoon);
                return strDateTime;
            }
        }
        //
        public static string GetViewDate
        {
            get
            {
                string strdTime = TimeHelper.GetUtcDateTimeTx;
                if (string.IsNullOrWhiteSpace(strdTime))
                    return string.Empty;
                //
                DateTime dateTime = Convert.ToDateTime(strdTime);
                return TimeFormat.FormatToViewDate(dateTime, Helper.Language.LanguagePage.GetLanguageCode);
            }
        }
        public static string GetViewTime
        {
            get
            {
                string strdTime = TimeHelper.GetUtcDateTimeTx;
                if (string.IsNullOrWhiteSpace(strdTime))
                    return string.Empty;
                //
                DateTime dateTime = Convert.ToDateTime(strdTime);
                return dateTime.ToString("HH:MM:ss");
            }
        }
    }

    public class TimeFormat
    {
        public static string GetDateByTimeZone(string strUtc)
        {
            string zoneId = GetTimeZoonName(strUtc);
            if (string.IsNullOrWhiteSpace(zoneId))
                return DateTime.Now.ToString();
            // 
            var timezone = TimeZoneInfo.FindSystemTimeZoneById(zoneId);
            if (timezone == null)
                return DateTime.Now.ToString();
            //
            var dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timezone);
            string date = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
            return date;
        }
        public static string GetDateTime
        {
            get
            {
                string timeZoon = string.Empty;
                string lgCode = Helper.Language.LanguagePage.GetLanguageCode;
                // Vietnamese
                if (lgCode == Language.LanguageCode.Vietnamese.ID)
                    timeZoon = TimeZoneID.vn_zonetime;
                // English
                if (lgCode == Language.LanguageCode.English.ID)
                    timeZoon = TimeZoneID.us_zonetime;
                //*************************************************************
                if (string.IsNullOrWhiteSpace(timeZoon))
                    return string.Empty;
                //
                string strDateTime = TimeFormat.GetDateTimeByTimeZone(timeZoon);
                return strDateTime;
            }
        }
        //
        public static string GetDateTimeByTimeZone(string timeZoon)
        {
            if (string.IsNullOrWhiteSpace(timeZoon))
                return string.Empty;
            // 
            TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoon);
            if (timeZoneInfo == null)
                return string.Empty;
            //
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo);
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }
        // date time
        public static string FormatToViewDate(DateTime dtime, string languageCode, int extent = (int)ModelEnum.DateExtension.DASH)
        {
            try
            {
                string strExt = string.Empty;
                if (extent == (int)ModelEnum.DateExtension.NONE || extent == (int)ModelEnum.DateExtension.DASH)
                    strExt = "-";
                else
                    strExt = "/";
                //
                if (languageCode == Language.LanguageCode.Vietnamese.ID)
                    return dtime.ToString("dd" + strExt + "MM" + strExt + "yyyy");
                else
                    return dtime.ToString("yyyy" + strExt + "MM" + strExt + "dd");

            }
            catch (Exception)
            {
                return string.Empty;

            }
        }
        public static string FormatToViewDate(string _strDate, string languageCode)
        {
            try
            {
                // Check for empty string.
                if (string.IsNullOrEmpty(_strDate))
                    return string.Empty;
                //
                bool isDateTime = DateTime.TryParse(_strDate, out _);
                if (isDateTime)
                {
                    DateTime dateTime = Convert.ToDateTime(_strDate);
                    return TimeFormat.FormatToViewDate(dateTime, languageCode);
                }
                return string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        public static string FormatToViewDateTime(DateTime dtime, string languageCode)
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
        public static string FormatToViewYearMonth(DateTime dtime, string languageCode)
        {
            if (languageCode == Language.LanguageCode.Vietnamese.ID)
                return dtime.ToString("MM-yyyy");
            else
                return dtime.ToString("yyyy-MM");
        }
        public static string FormatToTime(DateTime dtime)
        {
            return dtime.ToString("HH:mm:ss");
        }

        public static string FormatToViewDateTime(string _strDate, string languageCode)
        {
            try
            {
                // Check for empty string.
                if (string.IsNullOrEmpty(_strDate))
                    return string.Empty;
                //
                bool isDateTime = DateTime.TryParse(_strDate, out _);
                if (isDateTime)
                {
                    DateTime dateTime = Convert.ToDateTime(_strDate);
                    return TimeFormat.FormatToViewDateTime(dateTime, languageCode);
                }
                return string.Empty;

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
        public static string FormatToServerDateTime(DateTime dtime)
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
        public static DateTime FormatToServerDateTime(string dtime, string languageCode)
        {

            if (!string.IsNullOrWhiteSpace(dtime))
            {
                dtime = dtime.Trim();
                string[] arrDtime = dtime.Split(' ');
                if (arrDtime.Length == 0)
                    return DateTime.Now;
                //
                string arrDate = arrDtime[0];
                string arrTime = string.Empty;
                if (arrDtime.Length > 1)
                    arrTime = arrDtime[1];
                // date
                if (languageCode == Helper.Language.LanguageCode.Vietnamese.ID)
                {
                    if (arrDate.Contains('-'))
                    {
                        string[] arrtime = arrDate.Split('-');
                        string dateTime = arrtime[2] + "-" + arrtime[1] + "-" + arrtime[0] + " " + arrTime;
                        return Convert.ToDateTime(dateTime);
                    }
                    if (arrDate.Contains('/'))
                    {
                        string[] arrtime = arrDate.Split('/');
                        string dateTime = arrtime[2] + "-" + arrtime[1] + "-" + arrtime[0] + " " + arrTime;
                        return Convert.ToDateTime(dateTime);
                    }
                }
                else
                {
                    return Convert.ToDateTime(dtime);
                }

            }
            return DateTime.Now;
        }
        public static string FormatToServerDate(DateTime dtime)
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
        public static DateTime FormatToServerDate(string dtime, string languageCode = null)
        {
            if (!string.IsNullOrWhiteSpace(dtime))
            {
                if (languageCode == null)
                    languageCode = Helper.Language.LanguagePage.GetLanguageCode;
                //
                dtime = dtime.Trim();
                string[] arrDtime = dtime.Split(' ');
                if (arrDtime.Length == 0)
                    return DateTime.Now;
                //
                string arrDate = arrDtime[0];
                string arrTime = string.Empty;
                if (arrDtime.Length > 1)
                    arrTime = arrDtime[1];
                // date
                if (languageCode == Helper.Language.LanguageCode.Vietnamese.ID)
                {
                    if (arrDate.Contains('-'))
                    {
                        string[] arrtime = arrDate.Split('-');
                        string dateTime = arrtime[2] + "-" + arrtime[1] + "-" + arrtime[0] + " " + arrTime;
                        return Convert.ToDateTime(dateTime);
                    }
                    if (arrDate.Contains('/'))
                    {
                        string[] arrtime = arrDate.Split('/');
                        string dateTime = arrtime[2] + "-" + arrtime[1] + "-" + arrtime[0] + " " + arrTime;
                        return Convert.ToDateTime(dateTime);
                    }
                }
                else
                {
                    return Convert.ToDateTime(dtime);
                }

            }
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

        public static string Format_TDateTimeToDateTime(string dtime)
        {
            try
            {
                var dateTime = DateTimeOffset.Parse(dtime);
                return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
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

        // utc:  "Asia/Saigon", "Asia/Vientiane",
        public static string GetTimeZoonName(string strUtc)
        {
            if (string.IsNullOrWhiteSpace(strUtc))
                return string.Empty;
            //
            string file = HttpContext.Current.Server.MapPath(@"/library/script/timezones.json");
            using (StreamReader r = new StreamReader(file))
            {
                string json = r.ReadToEnd();
                List<TimeZoonClientModel> timeZoonClientModels = JsonConvert.DeserializeObject<List<TimeZoonClientModel>>(json);
                if (timeZoonClientModels != null && timeZoonClientModels.Count > 0)
                {
                    foreach (var item in timeZoonClientModels)
                    {
                        List<string> lstUtc = item.Utc;
                        if (lstUtc.Contains(strUtc))
                            return item.Value;
                    }
                }
                return string.Empty;
            }
        }
    }
    //
    public class TimeZoneID
    {
        public static string vn_zonetime = "SE Asia Standard Time";
        public static string us_zonetime = "Central America Standard Time";
    }
    public class TimeZoonClientModel
    {
        public string Value { get; set; }
        public string Abbr { get; set; }
        public string Offset { get; set; }
        public string Isdst { get; set; }
        public string Text { get; set; }
        public List<string> Utc { get; set; }
    }
}
