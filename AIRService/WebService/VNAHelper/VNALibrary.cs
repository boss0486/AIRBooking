using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

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
        public static string GetGivenName(string fullName)
        {
            if (fullName.Contains(" "))
            {
                string[] strName = fullName.Split(' ');
                return strName[strName.Length - 1];
            } 
            return string.Empty;
        }

        public static string GetSurName(string fullName)
        {
            if (fullName.Contains(" "))
            {
                string[] strName = fullName.Split(' ');
                return strName[0];
            }
            return string.Empty;
        }

        public static string GetMiddleName(string fullName)
        {
            if (fullName.Contains(" "))
            {
                string[] names = fullName.Split(' ');
                string strTemp = string.Empty;
                if (names.Length > 2)
                {
                    for (int i = 0; i < names.Length; i++)
                    {
                        if (i > 0 && i < names.Length - 1)
                            strTemp += names[i] + " ";
                    }
                    if (!string.IsNullOrWhiteSpace(strTemp))
                        strTemp = strTemp.Trim();
                }
                //
                return strTemp;
            }
            return string.Empty;
        }

        public static string ConvertGender(int _gender)
        {
            string result = string.Empty;
            switch (_gender)
            {
                case 1:
                    result = "M";
                    break;
                case 2:
                    result = "F";
                    break;
            }
            return result;
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


        public static string RemoveAllNamespaces(string xmlDocument)
        {
            XElement xmlDocumentWithoutNs = RemoveAllNamespaces(XElement.Parse(xmlDocument));

            return xmlDocumentWithoutNs.ToString();
        }

        //Core recursion function
        public static XElement RemoveAllNamespaces(XElement xmlDocument)
        {
            if (!xmlDocument.HasElements)
            {
                XElement xElement = new XElement(xmlDocument.Name.LocalName);
                xElement.Value = xmlDocument.Value;

                foreach (XAttribute attribute in xmlDocument.Attributes())
                    xElement.Add(attribute);

                return xElement;
            }
            return new XElement(xmlDocument.Name.LocalName, xmlDocument.Elements().Select(el => RemoveAllNamespaces(el)));
        }


        public static T Deserialize<T>(string input) where T : class
        {
            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(T));

            using (StringReader sr = new StringReader(input))
            {
                return (T)ser.Deserialize(sr);
            }
        }

        public static string Serialize<T>(T ObjectToSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(ObjectToSerialize.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, ObjectToSerialize);
                return textWriter.ToString();
            }
        }
    }
}
