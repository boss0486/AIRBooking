using AIRService.WS.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
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

        public static List<VNAResbookDesigCode> ListVNAResbookDesigCode()
        {
            List<VNAResbookDesigCode> vnaResbookDesigCode = new List<VNAResbookDesigCode>
            {
                new VNAResbookDesigCode { ID = 01, Title = "J" }, // 1
                new VNAResbookDesigCode { ID = 02, Title = "C" }, // 1
                new VNAResbookDesigCode { ID = 03, Title = "D" }, // 1
                new VNAResbookDesigCode { ID = 04, Title = "I" },
                new VNAResbookDesigCode { ID = 05, Title = "O" },
                new VNAResbookDesigCode { ID = 06, Title = "Y" },
                new VNAResbookDesigCode { ID = 07, Title = "B" },
                new VNAResbookDesigCode { ID = 08, Title = "M" }, // 2
                new VNAResbookDesigCode { ID = 09, Title = "S" }, // 4
                new VNAResbookDesigCode { ID = 10, Title = "H" }, // 4
                new VNAResbookDesigCode { ID = 11, Title = "K" }, // 4
                new VNAResbookDesigCode { ID = 12, Title = "L" }, // 4
                new VNAResbookDesigCode { ID = 13, Title = "Q" }, // 6
                new VNAResbookDesigCode { ID = 14, Title = "N" }, // 6
                new VNAResbookDesigCode { ID = 15, Title = "R" }, // 6
                new VNAResbookDesigCode { ID = 16, Title = "T" }, // 6
                new VNAResbookDesigCode { ID = 17, Title = "E" }, // 4
                new VNAResbookDesigCode { ID = 18, Title = "A" },
                new VNAResbookDesigCode { ID = 19, Title = "G" },
                new VNAResbookDesigCode { ID = 20, Title = "P" },
                new VNAResbookDesigCode { ID = 21, Title = "X" },
                new VNAResbookDesigCode { ID = 22, Title = "V" }
            };
            return vnaResbookDesigCode;
        }

        public static int GetResbookDesigCodeIDByKey(string key)
        {
            VNAResbookDesigCode vnaResbookDesigCode = VNALibrary.ListVNAResbookDesigCode().Where(m => m.Title == key).FirstOrDefault();
            if (vnaResbookDesigCode != null)
                return vnaResbookDesigCode.ID;
            else
                return -1;
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
        public static T Deserialize2<T>(string input) where T : class
        {
            var serializer = new XmlSerializer(typeof(T));
            using (TextReader reader = new StringReader(input))
            {
                return (T)serializer.Deserialize(reader);
            }
        }

        public static T ConvertNode<T>(XmlNode node, string xmlRootElement) where T : class
        {
            MemoryStream stm = new MemoryStream();

            StreamWriter stw = new StreamWriter(stm);
            stw.Write(node.OuterXml);
            stw.Flush();

            stm.Position = 0;
            XmlRootAttribute xRoot = new XmlRootAttribute("callArgs")
            {
                ElementName = "name",
                Namespace = "http://webservice.api.cabaret.com/",
                IsNullable = true
            };
            XmlSerializer ser = new XmlSerializer(typeof(T), xRoot);
            T result = (ser.Deserialize(stm) as T);

            return result;
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

        public static string GetXMLFromObject(object o)
        {
            StringWriter sw = new StringWriter();
            XmlTextWriter tw = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(o.GetType());
                tw = new XmlTextWriter(sw);
                serializer.Serialize(tw, o);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                sw.Close();
                if (tw != null)
                {
                    tw.Close();
                }
            }
            return sw.ToString();
        }


        //Helper.XMLHelper.WriteXml(Helper.XMLHelper.RandomString(8) + "search2.xml", soapEnvelopeXml);
        public static void WriteXml(string fileName, string inpXml)
        {
            XmlDocument soapEnvelopeXml = new XmlDocument();
            soapEnvelopeXml.LoadXml(inpXml);
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true
            };
            var urlFile = HttpContext.Current.Server.MapPath(@"~/Team/" + fileName);
            XmlWriter writer = XmlWriter.Create(urlFile, settings);
            soapEnvelopeXml.Save(writer);
        }

        public static void WriteXml(string fileName, XmlDocument soapEnvelopeXml)
        {
            // Save the document to a file and auto-indent the output.
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true
            };
            var urlFile = HttpContext.Current.Server.MapPath(@"~/Team/" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-") + fileName);
            XmlWriter writer = XmlWriter.Create(urlFile, settings);
            soapEnvelopeXml.Save(writer);
        }

        public static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string result = new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
            return result;
        }
    }
}
