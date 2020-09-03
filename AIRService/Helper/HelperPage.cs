using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Dapper;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Drawing;
using QRCoder;
using WebCore.Services;

namespace Helper.Page
{
    //###CLASS###################################################################################################################################################################################
    public class WebPage
    {
        public static string Domain
        {
            get
            {
                string host = HttpContext.Current.Request.Url.Host;
                string port = HttpContext.Current.Request.Url.Port.ToString();

                if (host.Contains("www"))
                    host = host.Replace("wwww.", "");

                if (host.IndexOf("localhost") >= 0)
                    return "http://" + host + ":" + port;
                else
                    return "http://" + host;
            }
        }
    }

    //###CLASS###################################################################################################################################################################################
    public class Validate
    {
        // regex string
        public static string RegexTextNoneUnicode = @"[a-zA-Z]+$";
        public static string RegexUserName = @"[a-zA-Z0-9]+$";
        public static string RegexPassword = @"[a-zA-Z0-9]+$"; // @"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#\$%\^&\*])(?=.{2,20})";
        public static string RegexPin = @"[0-9]+$";
        public static string RegexFName = @"^([a-zA-Z0-9 ẮẰẲẴẶĂẤẦẨẪẬÂÁÀÃẢẠĐẾỀỂỄỆÊÉÈẺẼẸÍÌỈĨỊỐỒỔỖỘÔỚỜỞỠỢƠÓÒÕỎỌỨỪỬỮỰƯÚÙỦŨỤÝỲỶỸỴắằẳẵặăấầẩẫậâáàãảạđếềểễệêéèẻẽẹíìỉĩịốồổỗộôớờởỡợơóòõỏọứừửữựưúùủũụýỳỷỹỵ]*)([a-zA-Z0-9 ẮẰẲẴẶĂẤẦẨẪẬÂÁÀÃẢẠĐẾỀỂỄỆÊÉÈẺẼẸÍÌỈĨỊỐỒỔỖỘÔỚỜỞỠỢƠÓÒÕỎỌỨỪỬỮỰƯÚÙỦŨỤÝỲỶỸỴắằẳẵặăấầẩẫậâáàãảạđếềểễệêéèẻẽẹíìỉĩịốồổỗộôớờởỡợơóòõỏọứừửữựưúùủũụýỳỷỹỵ]*)+$";
        public static string RegexText = @"([a-zA-Z0-9 ẮẰẲẴẶĂẤẦẨẪẬÂÁÀÃẢẠĐẾỀỂỄỆÊÉÈẺẼẸÍÌỈĨỊỐỒỔỖỘÔỚỜỞỠỢƠÓÒÕỎỌỨỪỬỮỰƯÚÙỦŨỤÝỲỶỸỴắằẳẵặăấầẩẫậâáàãảạđếềểễệêéèẻẽẹíìỉĩịốồổỗộôớờởỡợơóòõỏọứừửữựưúùủũụýỳỷỹỵ .:]*)([a-zA-Z0-9 ẮẰẲẴẶĂẤẦẨẪẬÂÁÀÃẢẠĐẾỀỂỄỆÊÉÈẺẼẸÍÌỈĨỊỐỒỔỖỘÔỚỜỞỠỢƠÓÒÕỎỌỨỪỬỮỰƯÚÙỦŨỤÝỲỶỸỴắằẳẵặăấầẩẫậâáàãảạđếềểễệêéèẻẽẹíìỉĩịốồổỗộôớờởỡợơóòõỏọứừửữựưúùủũụýỳỷỹỵ .:]*)+$";
        public static string RegexAlphabet = @"([a-zA-Z0-9 ẮẰẲẴẶĂẤẦẨẪẬÂÁÀÃẢẠĐẾỀỂỄỆÊÉÈẺẼẸÍÌỈĨỊỐỒỔỖỘÔỚỜỞỠỢƠÓÒÕỎỌỨỪỬỮỰƯÚÙỦŨỤÝỲỶỸỴắằẳẵặăấầẩẫậâáàãảạđếềểễệêéèẻẽẹíìỉĩịốồổỗộôớờởỡợơóòõỏọứừửữựưúùủũụýỳỷỹỵ .:]*)([a-zA-Z0-9 ẮẰẲẴẶĂẤẦẨẪẬÂÁÀÃẢẠĐẾỀỂỄỆÊÉÈẺẼẸÍÌỈĨỊỐỒỔỖỘÔỚỜỞỠỢƠÓÒÕỎỌỨỪỬỮỰƯÚÙỦŨỤÝỲỶỸỴắằẳẵặăấầẩẫậâáàãảạđếềểễệêéèẻẽẹíìỉĩịốồổỗộôớờởỡợơóòõỏọứừửữựưúùủũụýỳỷỹỵ .:]*)+$";
        public static string RegexPhone = @"((\+84)|(09|01[2|6|8|9]))+([0-9]{8,12})\b$";
        public static string RegexTel = @"^((\+\d{1,3}(-| )?\(?\d\)?(-| )?\d{1,3})|(\(?\d{2,3}\)?))(-| )?(\d{3,4})(-| )?(\d{4})(( x| ext)\d{1,5}){0,1}$";
        public static string RegexEmail = @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
        public static string RegexRoll = @"[a-zA-Z0-9]+$";
        public static string RegexDateVN = @"^([0]?[0-9]|[12][0-9]|[3][01])[./-]([0]?[1-9]|[1][0-2])[./-]([0-9]{4}|[0-9]{2})$";
        public static string RegexDate = @"^([12]\d{3}-(0[1-9]|1[0-2])-(0[1-9]|[12]\d|3[01]))$"; // yyyy-MM-dd
        public static string RegexDate2 = @"^(0?[1-9]|1[0-2])[-/](0?[1-9]|[12][0-9]|3[01])[-/](19[5-9][0-9]|20[0-4][0-9]|2050)$"; // yyyy-MM-dd
        public static string RegexDate_MMDDYYYY = @"^([0]?[1-9]|[1][0-2])[./-]([0]?[0-9]|[12][0-9]|[3][01])[./-]([0-9]{4}|[0-9]{2})$"; //mm/dd/yyyy
        public static string RegexTimekeeper = @"^([12]\d{3}-(0[1-9]|1[0-2])-(0[1-9]|[12]\d|3[01]))$"; // yyyy-MM-dd
        private const string RegexYoutube = "(?:.+?)?(?:\\/v\\/|watch\\/|\\?v=|\\&v=|youtu\\.be\\/|\\/v=|^youtu\\.be\\/)([a-zA-Z0-9_-]{11})+";
        // librari extensions file
        public static IList<string> FileExtensions = new List<string>
        {
            ".png", ".jpg", ".jpeg", ".svg", ".mp3", ".mp4", ".pdf", ".xls", ".xlsx", ".doc", ".docx", ".rar", ".zip", ".txt", ".ppt", ".pptx"
        };

        public static IList<string> ImgExtensions = new List<string>
        {
            ".png", ".jpg", ".jpeg",".gif"
        };
        public static IList<string> YoutubeList = new List<string>
        {
            "youtube.com", "www.youtube.com", "youtu.be", "www.youtu.be"
        };
        // function check
        public static bool TestNumeric(string number)
        {
            try
            {
                bool isNumeric = int.TryParse(number, out int n);
                return isNumeric;
            }
            catch
            {
                return false;
            }
        }
        public static bool TestUserName(string param)
        {
            var reg = new System.Text.RegularExpressions.Regex(RegexUserName);
            bool result = reg.IsMatch(param);
            if (result)
                return true;
            else
                return false;
        }
        public static bool TestPassword(string param)
        {
            var reg = new System.Text.RegularExpressions.Regex(RegexPassword);
            bool result = reg.IsMatch(param);
            if (result)
                return true;
            else
                return false;
        }
        public static bool TestPinCode(string param)
        {
            var reg = new System.Text.RegularExpressions.Regex(RegexPin);
            bool result = reg.IsMatch(param);
            if (result)
                return true;
            else
                return false;
        }
        public static bool TestLastName(string param)
        {
            var reg = new System.Text.RegularExpressions.Regex(RegexFName);
            bool result = reg.IsMatch(param);
            if (result)
                return true;
            else
                return false;
        }
        public static bool TestEmail(string param)
        {
            var reg = new System.Text.RegularExpressions.Regex(RegexEmail);
            bool result = reg.IsMatch(param);
            if (result)
                return true;
            else
                return false;
        }
        public static bool TestRoll(string param)
        {
            var reg = new System.Text.RegularExpressions.Regex(RegexRoll);
            bool result = reg.IsMatch(param);
            if (result)
                return true;
            else
                return false;
        }
        public static bool TestText(string param)
        {
            var reg = new System.Text.RegularExpressions.Regex(RegexText);
            bool result = reg.IsMatch(param);
            if (result)
                return true;
            else
                return false;
        }
        public static bool TestAlphabet(string param)
        {
            var reg = new System.Text.RegularExpressions.Regex(RegexAlphabet);
            bool result = reg.IsMatch(param);
            if (result)
                return true;
            else
                return false;
        }
        public static bool TestTextNoneUnicode(string param)
        {
            var reg = new System.Text.RegularExpressions.Regex(RegexTextNoneUnicode);
            bool result = reg.IsMatch(param);
            if (result)
                return true;
            else
                return false;
        }
        public static bool TestPhone(string param)
        {
            var reg = new System.Text.RegularExpressions.Regex(RegexPhone); //  allow spaces
            bool result = reg.IsMatch(param);
            if (result)
                return true;
            else
                return false;

            //return System.Text.RegularExpressions.Regex.Match(number, @"^(\+[0-9]{9})$").Success;
        }
        public static bool TestTel(string number)
        {
            return System.Text.RegularExpressions.Regex.Match(number, RegexTel).Success;
        }
        public static bool TestDateVN(string param)
        {
            return System.Text.RegularExpressions.Regex.Match(param, RegexDateVN).Success;
        }
        public static bool TestDateSQL(string param)
        {
            return System.Text.RegularExpressions.Regex.Match(param, RegexDate).Success;
        }
        public static bool TestDateTime(string text)
        {

            // Check for empty string.
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            bool isDateTime = DateTime.TryParse(text, out _);

            return isDateTime;
        }

        public static bool TestDate_MMDDYYYY(string param)
        {
            return System.Text.RegularExpressions.Regex.Match(param, RegexDate_MMDDYYYY).Success;
        }
        public static bool TestGuid(string text)
        {
            return (new Regex(@"(\A{[a-z\d]{8}(-[a-z\d]{4}){3}-[a-z\d]{12}}\z)|(\A[a-z\d]{8}(-[a-z\d]{4}){3}-[a-z\d]{12}\z)", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.CultureInvariant)).IsMatch(text);
        }
        // format extension
        public static bool TestImageFile(string extension)
        {
            try
            {
                if (string.IsNullOrEmpty(extension))
                    return false;
                extension = extension.ToLower();

                return true;
                // return ImgExtensions.Contains(extension);
            }
            catch (Exception)
            {
                return false;
            }

        }
        public static bool TestFile(string extension)
        {
            try
            {
                if (string.IsNullOrEmpty(extension))
                    return false;
                extension = extension.ToLower();
                return FileExtensions.Contains(extension);
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static int TestExcelExtension(string Extension)
        {
            int rs = -1;
            try
            {
                if (Extension.Length > 0 && Extension.ToLower() != "")
                {
                    switch (Extension.ToLower())
                    {
                        case ".xlsx":
                            rs = 1;
                            break;
                        default:
                            rs = -1;
                            break;
                    }
                }
            }
            catch (Exception)
            {
                rs = -1;
            }
            return rs;
        }
        public static int TestVideoExtension(string Extension)
        {
            int rs = -1;
            try
            {
                if (Extension.Length > 0 && Extension.ToLower() != "")
                {
                    switch (Extension.ToLower())
                    {
                        case ".avi":
                            rs = 1;
                            break;
                        case ".3gp":
                            rs = 1;
                            break;
                        case ".flv":
                            rs = 1;
                            break;
                        case ".mp4":
                            rs = 1;
                            break;
                        default:
                            rs = -1;
                            break;
                    }
                }
            }
            catch (Exception)
            {
                rs = -1;
            }
            return rs;
        }
    }
    //###CLASS###################################################################################################################################################################################
    
    
    
    public class Library
    {
        // format text 
        public static string SubText(int _length, string _text)
        {
            if (!string.IsNullOrEmpty(_text) && _length > 0 && _text.Length > _length)
                return _text.Substring(0, _length);
            return _text;
        }
        public static string SubTextTitle(string _text)
        {
            if (!string.IsNullOrEmpty(_text) && _text.Length > 65)
                return _text.Substring(0, 65) + "...";
            return _text;
        }
        public static string SubTextSummary(string _text)
        {
            if (!string.IsNullOrEmpty(_text) && _text.Length > 65)
                return _text.Substring(0, 65) + "...";
            return _text;
        }
        public static string SubTextFileName(string _text)
        {
            if (!string.IsNullOrEmpty(_text) && _text.Length > 30)
                return _text.Substring(0, 20) + "..." + _text.Substring(_text.Length - 10, 10);
            return _text;
        }
        // data
        public static string FormatTodDouble(double number)
        {
            try
            {
                return string.Format("{0:n8}", Decimal.Parse(number.ToString(), System.Globalization.NumberStyles.Float));
            }
            catch
            {
                return "0";
            }
        }
        public static string FormatToNumeric(double number)
        {
            try
            {
                string dcma = Decimal.Parse(number.ToString(), System.Globalization.NumberStyles.Float).ToString();

                return dcma + "";
                //return Math.Round(number, 2).ToString("0." + new string('#', 339));
            }
            catch
            {
                return "0.00";
            }
        }
        public static string FormatToUni2NONE(string text)
        {
            string[] arr1 = new string[] { "/", "\",", ",", "&", "$", "~", "*", ")", "{", "}", "|", "'", " ", "?", "%", "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ", "đ", "é", "è", "ẻ", "ẽ", "ẹ", "ê", "ế", "ề", "ể", "ễ", "ệ", "í", "ì", "ỉ", "ĩ", "ị", "ó", "ò", "ỏ", "õ", "ọ", "ô", "ố", "ồ", "ổ", "ỗ", "ộ", "ơ", "ớ", "ờ", "ở", "ỡ", "ợ", "ú", "ù", "ủ", "ũ", "ụ", "ư", "ứ", "ừ", "ử", "ữ", "ự", "ý", "ỳ", "ỷ", "ỹ", "ỵ", };
            string[] arr2 = new string[] { "", "", "", "", "", "", "", "", "", "", "", "", "-", "", "phan-tram", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "d", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "i", "i", "i", "i", "i", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "u", "u", "u", "u", "u", "u", "u", "u", "u", "u", "u", "y", "y", "y", "y", "y", };
            for (int i = 0; i < arr1.Length; i++)
            {
                text = text.Replace(arr1[i], arr2[i]);
                text = text.Replace(arr1[i].ToUpper(), arr2[i].ToUpper());
            }
            text = text.Replace(" ", "");

            return text;
        }
        public static string FormatToUpper(string strText)
        {
            if (string.IsNullOrEmpty(strText))
                return strText;
            string result = string.Empty;
            string[] words = strText.Split(' ');
            foreach (string word in words)
            {
                if (word.Trim() != "")
                {
                    if (word.Length > 1)
                        result += word.Substring(0, 1).ToUpper() + word.Substring(1).ToLower() + " ";
                    else
                        result += word.ToUpper() + " ";
                }
            }
            return result.Trim();
        }
        public static string FormatThousands(double _amount)
        {
            try
            {
                string result = String.Format("{0:#,##0.##}", _amount);
                result = result.Replace(",", " ");
                return result.Trim() + ".0";
            }
            catch (Exception)
            {
                return string.Empty;
            }
            //

        }
    }
    //###CLASS###################################################################################################################################################################################
    public class Default
    {
        public static string LanguageID = "en";
        public static string FileNoImange = "/files/default/00000000-0000-0000-0000-000000000000.gif";
        public static string FileUserImange = "/files/default/00000000-0000-0000-0000-000000000000.gif";
    }
    //###CLASS###################################################################################################################################################################################
    public class Navigate
    {
        public static string PathForbidden = "/pages/forbidden.html";
        public static string PathNotFound = "/pages/not-found.html";
        public static string PathNotService = "/pages/not-service.html";
        // direct to page
        public static void RedirectToForbidden()
        {
            // System.Web.HttpContext.Current.Response.Redirect(PathForbidden);
        }
        public static void RedirectToNotFound()
        {
            HttpContext.Current.Response.Redirect(PathNotFound);
        }
        public static void RedirectToNotService()
        {
            HttpContext.Current.Response.Redirect(PathNotService);
        }
        // 
        public static string PathCreate()
        {
            try
            {
                var meta = new MetaSEO();
                string _url = string.Empty;
                // area
                if (!string.IsNullOrWhiteSpace(MetaSEO.AreaText))
                    _url += "/" + MetaSEO.AreaText;
                // controller
                if (!string.IsNullOrWhiteSpace(MetaSEO.ControllerText))
                    _url += "/" + MetaSEO.ControllerText;
                // action
                if (string.IsNullOrWhiteSpace(_url))
                    return string.Empty;
                //
                _url += "/create";
                return _url.ToLower();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        public static string PathDataList()
        {
            try
            {
                var meta = new MetaSEO();
                string _url = string.Empty;
                // area
                if (!string.IsNullOrWhiteSpace(MetaSEO.AreaText))
                    _url += "/" + MetaSEO.AreaText;
                // controller
                if (!string.IsNullOrWhiteSpace(MetaSEO.ControllerText))
                    _url += "/" + MetaSEO.ControllerText;
                // action
                if (string.IsNullOrWhiteSpace(_url))
                    return string.Empty;
                //
                _url += "/DataList";
                return _url.ToLower();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static string NavigateByParam(string _url)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_url))
                    return string.Empty;
                //
                Uri siteUri = new Uri(_url);
                string result = HttpUtility.ParseQueryString(siteUri.Query).Get("r");
                //
                return result;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
    public class MetaSEO
    {
        public static string MetaTitle
        {
            get
            {
                try
                {
                    string strTitle = "MANAGE " + MetaSEO.ControllerText; //+ " - " + meta.ConvertAction(meta.ActionText());
                    return strTitle.ToUpper();
                }
                catch (Exception)
                {
                    return "MANAGE";
                }
            }
        }
        public static string AreaText
        {
            get
            {
                try
                {
                    var routeData = HttpContext.Current.Request.RequestContext.RouteData;
                    if (routeData.DataTokens["area"] != null)
                        return routeData.DataTokens["area"].ToString();
                    //
                    return string.Empty;
                }
                catch (Exception)
                {

                    return string.Empty;
                }

            }
        }
        public static string ControllerText
        {
            get
            {
                try
                {
                    var routeValues = HttpContext.Current.Request.RequestContext.RouteData.Values;
                    if (routeValues.ContainsKey("controller"))
                        return (string)routeValues["controller"];
                    return string.Empty;
                }
                catch (Exception)
                {

                    return string.Empty;
                }

            }
        }
        public static string ActionText
        {
            get
            {
                var routeValues = HttpContext.Current.Request.RequestContext.RouteData.Values;
                if (routeValues.ContainsKey("action"))
                    return (string)routeValues["action"];
                return string.Empty;
            }
        }
        public static string PageTitle
        {
            get
            {
                try
                {
                    string _controller = MetaSEO.ControllerText;
                    string _action = MetaSEO.ActionText;
                    using (var _service = new MenuControllerService())
                    {

                        return _controller + "/" + _action;
                        //string strTitle = _service.GetMenuName(_controller, _action);
                        //if (string.IsNullOrWhiteSpace(strTitle))
                        //    return string.Empty;
                        //return strTitle.ToUpper();
                    }
                }
                catch (Exception ex)
                {
                    return "HRM | MANAGE & " + ex;
                }
            }
        }
        public string ConvertAction(string _param)
        {
            try
            {
                if (string.IsNullOrEmpty(_param))
                    return string.Empty;

                string result = string.Empty;
                switch (_param.ToLower())
                {
                    case "index":
                        result = "Danh sách";
                        break;
                    case "create":
                        result = "Tạo mới";
                        break;
                    case "update":
                        result = "Cập nhật";
                        break;
                }
                return result;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}