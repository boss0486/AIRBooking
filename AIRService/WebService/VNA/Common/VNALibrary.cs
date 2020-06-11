using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AIRService.WS.Helper
{
    public static class VNALibrary
    {
        public static int ConvertToInt32(string value, int defaultValue = 0)
        {
            try
            {
                return int.Parse(value);
            }
            catch (Exception ex)
            {
                return defaultValue;
            }

        }
        public static double ConvertToDouble(string value, double defaultValue = 0f)
        {
            try
            {
                return double.Parse(value);
            }
            catch (Exception ex)
            {
                return defaultValue;
            }

        }
        public static bool ConvertToBool(string value, bool defaultValue = false)
        {
            try
            {
                return bool.Parse(value);
            }
            catch (Exception ex)
            {
                return defaultValue;
            }

        }
        public static DateTime ConvertToDateTime(string value, DateTime? defaultValue = null)
        {
            try
            {
                return DateTime.Parse(value);
            }
            catch (Exception ex)
            {
                if (defaultValue != null)
                {
                    return defaultValue.Value;
                }
                return DateTime.MinValue;

            }

        }
        public static string ParseDateTimeToFullStringDate(DateTime date)
        {
            return date.ToString("yyyy-MM-dd'T'HH:mm:ss");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="gender">true là nam | false là nữ</param>
        /// <returns></returns>
        public static string TitleGenerator(string type, string gender)
        {
            bool flag = false;
            if (gender == "M")
                flag = true;
            //
            if (type == "ADT")
            {
                if (flag)
                    return "MR";
                else
                    return "MS";
            }
            else
            {
                if (flag)
                    return "MSTR";
                else
                    return "MISS";
            }
        }
    }
    public class XMLHelper
    {
        // url webservice
        public static string URL_WS = "https://webservices-as.havail.sabre.com/vn/websvc";
        public static HttpWebRequest CreateWebRequest(string url)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Headers.Add(@"SOAP:Action");
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }
    }
}
