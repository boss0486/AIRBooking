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

namespace Helper
{
    public class Web
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
        public static string FuncNote
        {
            get
            {
                string result = string.Empty;
                List<ModelNote> lstModalNote = new List<ModelNote>
                {
                    new ModelNote("ic-add.png", "Tạo mới"),
                    new ModelNote("ic-edit.png", "Cập nhật"),
                    new ModelNote("ic-delete-document.png", "Xóa"),
                    new ModelNote("ic-view-details.png", "Chi tiết"),
                    new ModelNote("ic-barcode.png", "Mã code"),
                    new ModelNote("ic-no-access.png", "Không có quyền truy cập"),
                    new ModelNote("ic-filter.png", "Lọc"),
                    new ModelNote("ic-setting.png", "Cài đặt"),
                    new ModelNote("ic-approval.png", "Chấp nhận"),
                    new ModelNote("ic-checked.png", "Đã chọn"),
                    new ModelNote("ic-unchecked.png", "Chưa chọn"),
                    new ModelNote("ic-location.png", "Vị trí"),
                    new ModelNote("ic-checkout.png", "Trả sách")
                };
                result += "<div class='col-md-12 note-func'><div><b>Chú giải:</b></div><hr /><div class='row'>";
                foreach (var item in lstModalNote.OrderBy(m => m.IcName))
                {
                    result += "<div class='col-md-4'><div class='form-group'><label><img src='/_Library/image/icon/" + item.IcImg + "' /> </label>&nbsp;<span class='notetext'> " + item.IcName + "</span></div></div>";
                }
                result += "</div></div>";
                return result;
            }
        }
        public class ModelNote
        {
            public string IcName;
            public string IcImg;

            public ModelNote(string v1, string v2)
            {
                this.IcImg = v1;
                this.IcName = v2;
            }
        }
        public class PageMessage
        {
            public static string ForbiddenPage = "/page/forbidden.html";
            public static string NotFound = "/page/not-found.html";
            public static string NotService = "/page/not-service.html";
        }
        public class IFile
        {
            public static string NoImangeFile = "/files/default/00000000-0000-0000-0000-000000000000.gif";
            public static string NoImangeUser = "/files/default/00000000-0000-0000-0000-000000000000.gif";

        }
        public class ContenText
        {
            public static string TextLength(int _length, string _text)
            {
                if (!string.IsNullOrEmpty(_text) && _length > 0 && _text.Length > _length)
                    return _text.Substring(0, _length);
                return _text;
            }
            public static string TitleLength(string _text)
            {
                if (!string.IsNullOrEmpty(_text) && _text.Length > 65)
                    return _text.Substring(0, 65) + "...";
                return _text;
            }
            public static string SummaryLength(string _text)
            {
                if (!string.IsNullOrEmpty(_text) && _text.Length > 65)
                    return _text.Substring(0, 65) + "...";
                return _text;
            }
            public static string FileLength(string _text)
            {
                if (!string.IsNullOrEmpty(_text) && _text.Length > 30)
                    return _text.Substring(0, 20) + "..." + _text.Substring(_text.Length - 10, 10);
                return _text;
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
    //
    public class Validate
    {
        public static string RegexUserName = @"[a-zA-Z0-9]+$";
        //public static string RegexPassword = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#\$%\^&\*])(?=.{2,20})";
        public static string RegexPassword = @"[a-zA-Z0-9]+$";
        public static string RegexPin = @"[0-9]+$";
        public static string RegexFName = @"^([a-zA-Z0-9 ẮẰẲẴẶĂẤẦẨẪẬÂÁÀÃẢẠĐẾỀỂỄỆÊÉÈẺẼẸÍÌỈĨỊỐỒỔỖỘÔỚỜỞỠỢƠÓÒÕỎỌỨỪỬỮỰƯÚÙỦŨỤÝỲỶỸỴắằẳẵặăấầẩẫậâáàãảạđếềểễệêéèẻẽẹíìỉĩịốồổỗộôớờởỡợơóòõỏọứừửữựưúùủũụýỳỷỹỵ]*)([a-zA-Z0-9 ẮẰẲẴẶĂẤẦẨẪẬÂÁÀÃẢẠĐẾỀỂỄỆÊÉÈẺẼẸÍÌỈĨỊỐỒỔỖỘÔỚỜỞỠỢƠÓÒÕỎỌỨỪỬỮỰƯÚÙỦŨỤÝỲỶỸỴắằẳẵặăấầẩẫậâáàãảạđếềểễệêéèẻẽẹíìỉĩịốồổỗộôớờởỡợơóòõỏọứừửữựưúùủũụýỳỷỹỵ]*)+$";
        public static string RegexFormatText = @"([a-zA-Z0-9 ẮẰẲẴẶĂẤẦẨẪẬÂÁÀÃẢẠĐẾỀỂỄỆÊÉÈẺẼẸÍÌỈĨỊỐỒỔỖỘÔỚỜỞỠỢƠÓÒÕỎỌỨỪỬỮỰƯÚÙỦŨỤÝỲỶỸỴắằẳẵặăấầẩẫậâáàãảạđếềểễệêéèẻẽẹíìỉĩịốồổỗộôớờởỡợơóòõỏọứừửữựưúùủũụýỳỷỹỵ .:]*)([a-zA-Z0-9 ẮẰẲẴẶĂẤẦẨẪẬÂÁÀÃẢẠĐẾỀỂỄỆÊÉÈẺẼẸÍÌỈĨỊỐỒỔỖỘÔỚỜỞỠỢƠÓÒÕỎỌỨỪỬỮỰƯÚÙỦŨỤÝỲỶỸỴắằẳẵặăấầẩẫậâáàãảạđếềểễệêéèẻẽẹíìỉĩịốồổỗộôớờởỡợơóòõỏọứừửữựưúùủũụýỳỷỹỵ .:]*)+$";
        public static string RegexAlphabet = @"([a-zA-Z0-9 ẮẰẲẴẶĂẤẦẨẪẬÂÁÀÃẢẠĐẾỀỂỄỆÊÉÈẺẼẸÍÌỈĨỊỐỒỔỖỘÔỚỜỞỠỢƠÓÒÕỎỌỨỪỬỮỰƯÚÙỦŨỤÝỲỶỸỴắằẳẵặăấầẩẫậâáàãảạđếềểễệêéèẻẽẹíìỉĩịốồổỗộôớờởỡợơóòõỏọứừửữựưúùủũụýỳỷỹỵ .:]*)([a-zA-Z0-9 ẮẰẲẴẶĂẤẦẨẪẬÂÁÀÃẢẠĐẾỀỂỄỆÊÉÈẺẼẸÍÌỈĨỊỐỒỔỖỘÔỚỜỞỠỢƠÓÒÕỎỌỨỪỬỮỰƯÚÙỦŨỤÝỲỶỸỴắằẳẵặăấầẩẫậâáàãảạđếềểễệêéèẻẽẹíìỉĩịốồổỗộôớờởỡợơóòõỏọứừửữựưúùủũụýỳỷỹỵ .:]*)+$";
        public static string RegexPhone = @"((\+84)|(09|01[2|6|8|9]))+([0-9]{8,12})\b$";
        public static string RegexTel = @"^((\+\d{1,3}(-| )?\(?\d\)?(-| )?\d{1,3})|(\(?\d{2,3}\)?))(-| )?(\d{3,4})(-| )?(\d{4})(( x| ext)\d{1,5}){0,1}$";
        public static string RegexEmail = @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
        public static string RegexRollNumber = @"[a-zA-Z0-9]+$";
        public static string RegexDateVN = @"^([0]?[0-9]|[12][0-9]|[3][01])[./-]([0]?[1-9]|[1][0-2])[./-]([0-9]{4}|[0-9]{2})$";
        public static string RegexDate = @"^([12]\d{3}-(0[1-9]|1[0-2])-(0[1-9]|[12]\d|3[01]))$"; // yyyy-MM-dd
        public static string RegexDate_MMDDYYYY = @"^([0]?[1-9]|[1][0-2])[./-]([0]?[0-9]|[12][0-9]|[3][01])[./-]([0-9]{4}|[0-9]{2})$"; //mm/dd/yyyy
        public static string RegexDate2 = @"^(0?[1-9]|1[0-2])[-/](0?[1-9]|[12][0-9]|3[01])[-/](19[5-9][0-9]|20[0-4][0-9]|2050)$"; // yyyy-MM-dd
        // /^(0?[1-9]|1[0-2])[-/](0?[1-9]|[12][0-9]|3[01])[-/](19[5-9][0-9]|20[0-4][0-9]|2050)$/
        public static string RegexTimekeeper = @"^([12]\d{3}-(0[1-9]|1[0-2])-(0[1-9]|[12]\d|3[01]))$"; // yyyy-MM-dd
        public static bool FormatUserName(string param)
        {
            var reg = new System.Text.RegularExpressions.Regex(RegexUserName);
            bool result = reg.IsMatch(param);
            if (result)
                return true;
            else
                return false;
        }
        public static bool FormatPassword(string param)
        {
            var reg = new System.Text.RegularExpressions.Regex(RegexPassword);
            bool result = reg.IsMatch(param);
            if (result)
                return true;
            else
                return false;
        }
        public static bool FormatPin(string param)
        {
            var reg = new System.Text.RegularExpressions.Regex(RegexPin);
            bool result = reg.IsMatch(param);
            if (result)
                return true;
            else
                return false;
        }
        public static bool FormatLastName(string param)
        {
            var reg = new System.Text.RegularExpressions.Regex(RegexFName);
            bool result = reg.IsMatch(param);
            if (result)
                return true;
            else
                return false;
        }
        public static bool FormatEmail(string param)
        {
            var reg = new System.Text.RegularExpressions.Regex(RegexEmail);
            bool result = reg.IsMatch(param);
            if (result)
                return true;
            else
                return false;
        }
        public static bool FormatRollNumber(string param)
        {
            var reg = new System.Text.RegularExpressions.Regex(RegexRollNumber);
            bool result = reg.IsMatch(param);
            if (result)
                return true;
            else
                return false;
        }
        public static bool FormatText(string param)
        {
            var reg = new System.Text.RegularExpressions.Regex(RegexFormatText);
            bool result = reg.IsMatch(param);
            if (result)
                return true;
            else
                return false;
        }
        public static bool FormatAlphabet(string param)
        {
            var reg = new System.Text.RegularExpressions.Regex(RegexAlphabet);
            bool result = reg.IsMatch(param);
            if (result)
                return true;
            else
                return false;
        }
        public static bool FormatPhone(string param)
        {
            var reg = new System.Text.RegularExpressions.Regex(RegexPhone); //  allow spaces
            bool result = reg.IsMatch(param);
            if (result)
                return true;
            else
                return false;

            //return System.Text.RegularExpressions.Regex.Match(number, @"^(\+[0-9]{9})$").Success;
        }
        public static bool FormatTel(string number)
        {
            return System.Text.RegularExpressions.Regex.Match(number, RegexTel).Success;
        }
        public static bool FormatDateVN(string param)
        {
            return System.Text.RegularExpressions.Regex.Match(param, RegexDateVN).Success;
        }
        public static bool FormatDateSQL(string param)
        {
            return System.Text.RegularExpressions.Regex.Match(param, RegexDate).Success;
        }
        public static bool FormatDate_MMDDYYYY(string param)
        {
            return System.Text.RegularExpressions.Regex.Match(param, RegexDate_MMDDYYYY).Success;
        }
        public static bool FormatDateTime(string text)
        {

            // Check for empty string.
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            bool isDateTime = DateTime.TryParse(text, out _);

            return isDateTime;
        }
        public static bool FormatImageFile(string extension)
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
        public static bool FormatFile(string extension)
        {
            bool rs = false;
            try
            {
                if (string.IsNullOrEmpty(extension))
                    return false;
                extension = extension.ToLower();
                return FormatFileExtensions.Contains(extension);
            }
            catch (Exception)
            {
                rs = false;
            }
            return rs;
        }
        public static int FormatOfficeExcelExtension(string Extension)
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
        public static int FormatVideoExtension(string Extension)
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

        public static IList<string> FormatFileExtensions = new List<string>
        {
            ".png", ".jpg", ".jpeg", ".svg", ".mp3", ".mp4", ".pdf", ".xls", ".xlsx", ".doc", ".docx", ".rar", ".zip", ".txt", ".ppt", ".pptx"
        };
        public static IList<string> ImgExtensions = new List<string>
        {
            ".png", ".jpg", ".jpeg",".gif"
        };

        public static bool FormatGuid(string text)
        {
            return (new Regex(@"(\A{[a-z\d]{8}(-[a-z\d]{4}){3}-[a-z\d]{12}}\z)|(\A[a-z\d]{8}(-[a-z\d]{4}){3}-[a-z\d]{12}\z)", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.CultureInvariant)).IsMatch(text);
        }
        private const string YoutubeLinkRegex = "(?:.+?)?(?:\\/v\\/|watch\\/|\\?v=|\\&v=|youtu\\.be\\/|\\/v=|^youtu\\.be\\/)([a-zA-Z0-9_-]{11})+";
        private static readonly System.Text.RegularExpressions.Regex regexExtractId = new System.Text.RegularExpressions.Regex(YoutubeLinkRegex, System.Text.RegularExpressions.RegexOptions.Compiled);
        private static readonly string[] validAuthorities = { "youtube.com", "www.youtube.com", "youtu.be", "www.youtu.be" };
        public static string ExtractVideoIdFromUri(string _path)
        {
            try
            {
                System.Uri uri = new System.Uri(_path);
                string authority = new UriBuilder(uri).Uri.Authority.ToLower();

                //check if the url is a youtube url
                if (!validAuthorities.Contains(authority))
                    return string.Empty;
                //and extract the id
                var regRes = regexExtractId.Match(uri.ToString());
                if (regRes.Success)
                    return regRes.Groups[1].Value;

                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
        public static bool FormatNumeric(string number)
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
    }
    public class Library
    {
        public static string ConvertTodDouble(double number)
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
        public static string ConvertToNumeric(double number)
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
        public static string Uni2NONE(string strText)
        {
            if (string.IsNullOrEmpty(strText))
                return string.Empty;

            string text = strText.ToLower().Trim();
            //a
            text = text.Replace("á", "a");
            text = text.Replace("à", "a");
            text = text.Replace("ả", "a");
            text = text.Replace("ã", "a");
            text = text.Replace("ạ", "a");
            //ă
            text = text.Replace("ă", "a");
            text = text.Replace("ắ", "a");
            text = text.Replace("ằ", "a");
            text = text.Replace("ẳ", "a");
            text = text.Replace("ẵ", "a");
            text = text.Replace("ặ", "a");
            //â
            text = text.Replace("â", "a");
            text = text.Replace("ấ", "a");
            text = text.Replace("ầ", "a");
            text = text.Replace("ẩ", "a");
            text = text.Replace("ẫ", "a");
            text = text.Replace("ậ", "a");
            //đ
            text = text.Replace("đ", "d");
            //e
            text = text.Replace("é", "e");
            text = text.Replace("è", "e");
            text = text.Replace("ẻ", "e");
            text = text.Replace("ẽ", "e");
            text = text.Replace("ẹ", "e");

            //ê
            text = text.Replace("ê", "e");
            text = text.Replace("ế", "e");
            text = text.Replace("ề", "e");
            text = text.Replace("ể", "e");
            text = text.Replace("ễ", "e");
            text = text.Replace("ệ", "e");
            //i
            text = text.Replace("í", "i");
            text = text.Replace("ì", "i");
            text = text.Replace("ỉ", "i");
            text = text.Replace("ĩ", "i");
            text = text.Replace("ị", "i");
            //o
            text = text.Replace("ó", "o");
            text = text.Replace("ò", "o");
            text = text.Replace("ỏ", "o");
            text = text.Replace("õ", "o");
            text = text.Replace("ọ", "o");
            //ô
            text = text.Replace("ô", "o");
            text = text.Replace("ố", "o");
            text = text.Replace("ồ", "o");
            text = text.Replace("ổ", "o");
            text = text.Replace("ỗ", "o");
            text = text.Replace("ộ", "o");
            //ơ
            text = text.Replace("ơ", "o");
            text = text.Replace("ớ", "o");
            text = text.Replace("ờ", "o");
            text = text.Replace("ở", "o");
            text = text.Replace("ỡ", "o");
            text = text.Replace("ợ", "o");
            //u
            text = text.Replace("ú", "u");
            text = text.Replace("ù", "u");
            text = text.Replace("ủ", "u");
            text = text.Replace("ũ", "u");
            text = text.Replace("ụ", "u");
            //ư
            text = text.Replace("ư", "u");
            text = text.Replace("ứ", "u");
            text = text.Replace("ừ", "u");
            text = text.Replace("ử", "u");
            text = text.Replace("ữ", "u");
            text = text.Replace("ự", "u");
            //y
            text = text.Replace("ý", "y");
            text = text.Replace("ỳ", "y");
            text = text.Replace("ỷ", "y");
            text = text.Replace("ỹ", "y");
            text = text.Replace("ỵ", "y");
            //---------------------------------Upper-------------------------------------------------

            //ký tựta
            text = text.Replace("/", "");
            text = text.Replace("\"", "");
            text = text.Replace(",", "");
            text = text.Replace("&", "");
            text = text.Replace("$", "");
            text = text.Replace("~", "");
            text = text.Replace("*", "");
            text = text.Replace("(", "");
            text = text.Replace(")", "");
            text = text.Replace("{", "");
            text = text.Replace("}", "");
            text = text.Replace("|", "");
            text = text.Replace("'", "''");
            text = text.Replace(" ", "-");
            text = text.Replace("?", "");
            text = text.Replace("%", " phan-tram ");
            return text.ToLower();
        }
        public static string RemoveUnicode(string text)
        {
            string[] arr1 = new string[] { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ", "đ", "é", "è", "ẻ", "ẽ", "ẹ", "ê", "ế", "ề", "ể", "ễ", "ệ", "í", "ì", "ỉ", "ĩ", "ị", "ó", "ò", "ỏ", "õ", "ọ", "ô", "ố", "ồ", "ổ", "ỗ", "ộ", "ơ", "ớ", "ờ", "ở", "ỡ", "ợ", "ú", "ù", "ủ", "ũ", "ụ", "ư", "ứ", "ừ", "ử", "ữ", "ự", "ý", "ỳ", "ỷ", "ỹ", "ỵ", };
            string[] arr2 = new string[] { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "d", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "i", "i", "i", "i", "i", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "u", "u", "u", "u", "u", "u", "u", "u", "u", "u", "u", "y", "y", "y", "y", "y", };
            for (int i = 0; i < arr1.Length; i++)
            {
                text = text.Replace(arr1[i], arr2[i]);
                text = text.Replace(arr1[i].ToUpper(), arr2[i].ToUpper());
            }
            text = text.Replace(" ", "");

            return text;
        }
        public static string ConvertDateVN(DateTime dateTime, string ext = null)
        {
            try
            {
                if (ext == null)
                {
                    ext = "/";
                }
                //DateTime dt = new DateTime(dtime.Year, dtime.Month, dtime.Day, dtime.Hour, dtime.Minute, dtime.Second, dtime.Millisecond);
                string _month = Convert.ToString(dateTime.Month);
                string _day = Convert.ToString(dateTime.Day);
                string _hour = Convert.ToString(dateTime.Hour);
                string _minute = Convert.ToString(dateTime.Minute);
                string _second = Convert.ToString(dateTime.Second);
                string _millisecond = Convert.ToString(dateTime.Millisecond);
                if (_month.Length == 1)
                    _month = "0" + _month;
                if (_day.Length == 1)
                    _day = "0" + _day;
                if (_hour.Length == 1)
                    _hour = "0" + _hour;
                if (_minute.Length == 1)
                    _minute = "0" + _minute;
                if (_second.Length == 1)
                    _second = "0" + _second;
                return _day + ext + _month + ext + dateTime.Year;
            }
            catch (Exception)
            {

                return "00" + ext + "00" + ext + "0000";

            }
        }
        public static string FormatDateTime(DateTime dtime)
        {
            try
            {
                //DateTime dt = new DateTime(dtime.Year, dtime.Month, dtime.Day, dtime.Hour, dtime.Minute, dtime.Second, dtime.Millisecond);
                string _month = Convert.ToString(dtime.Month);
                string _day = Convert.ToString(dtime.Day);
                string _hour = Convert.ToString(dtime.Hour);
                string _minute = Convert.ToString(dtime.Minute);
                string _second = Convert.ToString(dtime.Second);
                string _millisecond = Convert.ToString(dtime.Millisecond);
                if (_month.Length == 1)
                    _month = "0" + _month;
                if (_day.Length == 1)
                    _day = "0" + _day;
                if (_hour.Length == 1)
                    _hour = "0" + _hour;
                if (_minute.Length == 1)
                    _minute = "0" + _minute;
                if (_second.Length == 1)
                    _second = "0" + _second;
                return _day + "/" + _month + "/" + dtime.Year + " " + _hour + ":" + _minute + ":" + _second;
            }
            catch (Exception)
            {
                string _month = "00";
                string _day = "00";
                string _hour = "00";
                string _minute = "00";
                string _second = "00";
                return _day + "/" + _month + "/" + dtime.Year + " " + _hour + ":" + _minute + ":" + _second;

            }
        }
        public static string FormatDate(DateTime dtime)
        {
            try
            {
                //DateTime dt = new DateTime(dtime.Year, dtime.Month, dtime.Day, dtime.Hour, dtime.Minute, dtime.Second, dtime.Millisecond);
                string _month = Convert.ToString(dtime.Month);
                string _day = Convert.ToString(dtime.Day);
                if (_month.Length == 1)
                    _month = "0" + _month;
                if (_day.Length == 1)
                    _day = "0" + _day;
                return _day + "/" + _month + "/" + dtime.Year;
            }
            catch (Exception)
            {
                dtime = DateTime.Now;
                string _month = Convert.ToString(dtime.Month);
                string _day = Convert.ToString(dtime.Day);
                string _hour = Convert.ToString(dtime.Hour);
                string _minute = Convert.ToString(dtime.Minute);
                string _second = Convert.ToString(dtime.Second);
                string _millisecond = Convert.ToString(dtime.Millisecond);
                if (_month.Length == 1)
                    _month += "0" + _month;
                if (_day.Length == 1)
                    _day += "0" + _day;
                return _day + "/" + _month + "/" + dtime.Year;

            }
        }
        public static string FormatDate(string _strDate)
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
        public static string FormatNewsDate(DateTime dtime)
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
        public static string FormatSQLDateTime(DateTime dtime, string ext = null)
        {
            try
            {
                //DateTime dt = new DateTime(dtime.Year, dtime.Month, dtime.Day, dtime.Hour, dtime.Minute, dtime.Second, dtime.Millisecond);
                string _month = Convert.ToString(dtime.Month);
                string _day = Convert.ToString(dtime.Day);
                string _hour = Convert.ToString(dtime.Hour);
                string _minute = Convert.ToString(dtime.Minute);
                string _second = Convert.ToString(dtime.Second);
                string _millisecond = Convert.ToString(dtime.Millisecond);
                if (_month.Length == 1)
                    _month = "0" + _month;
                if (_day.Length == 1)
                    _day = "0" + _day;
                if (_hour.Length == 1)
                    _hour = "0" + _hour;
                if (_minute.Length == 1)
                    _minute = "0" + _minute;
                if (_second.Length == 1)
                    _second = "0" + _second;
                return dtime.Year + ext + _month + ext + _day + " " + _hour + ":" + _minute + ":" + _second;
            }
            catch (Exception)
            {
                dtime = DateTime.Now;
                string _month = Convert.ToString(dtime.Month);
                string _day = Convert.ToString(dtime.Day);
                string _hour = Convert.ToString(dtime.Hour);
                string _minute = Convert.ToString(dtime.Minute);
                string _second = Convert.ToString(dtime.Second);
                string _millisecond = Convert.ToString(dtime.Millisecond);
                if (_month.Length == 1)
                    _month += "0" + _month;
                if (_day.Length == 1)
                    _day += "0" + _day;
                if (_hour.Length == 1)
                    _hour += "0" + _hour;
                if (_minute.Length == 1)
                    _minute += "0" + _minute;
                if (_second.Length == 1)
                    _second += "0" + _second;
                return dtime.Year + ext + _month + ext + _day + " " + _hour + ":" + _minute + ":" + _second;
            }
        }
        public static string FormatSQLDate(DateTime dtime, string ext)
        {
            try
            {
                //DateTime dt = new DateTime(dtime.Year, dtime.Month, dtime.Day, dtime.Hour, dtime.Minute, dtime.Second, dtime.Millisecond);
                string _month = Convert.ToString(dtime.Month);
                string _day = Convert.ToString(dtime.Day);
                string _hour = Convert.ToString(dtime.Hour);
                string _minute = Convert.ToString(dtime.Minute);
                string _second = Convert.ToString(dtime.Second);
                string _millisecond = Convert.ToString(dtime.Millisecond);
                if (_month.Length == 1)
                    _month = "0" + _month;
                if (_day.Length == 1)
                    _day = "0" + _day;
                return dtime.Year + ext + _month + ext + _day;
            }
            catch (Exception)
            {
                dtime = DateTime.Now;
                string _month = Convert.ToString(dtime.Month);
                string _day = Convert.ToString(dtime.Day);
                string _hour = Convert.ToString(dtime.Hour);
                string _minute = Convert.ToString(dtime.Minute);
                string _second = Convert.ToString(dtime.Second);
                string _millisecond = Convert.ToString(dtime.Millisecond);
                if (_month.Length == 1)
                    _month += "0" + _month;
                if (_day.Length == 1)
                    _day += "0" + _day;
                return dtime.Year + ext + _month + ext + _day;

            }
        }
        public static DateTime FormatSQLDate(string dtime)
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
        public static string FormatUTCDateTime(DateTime dtime, string ext = "-")
        {
            try
            {
                string _month = Convert.ToString(dtime.Month);
                string _day = Convert.ToString(dtime.Day);
                string _hour = Convert.ToString(dtime.Hour);
                string _minute = Convert.ToString(dtime.Minute);
                string _second = Convert.ToString(dtime.Second);
                string _millisecond = Convert.ToString(dtime.Millisecond);
                if (_month.Length == 1)
                    _month = "0" + _month;
                if (_day.Length == 1)
                    _day = "0" + _day;
                if (_hour.Length == 1)
                    _hour = "0" + _hour;
                if (_minute.Length == 1)
                    _minute = "0" + _minute;
                if (_second.Length == 1)
                    _second = "0" + _second;
                return dtime.Year + ext + _month + ext + _day + " " + _hour + ":" + _minute + ":" + _second;
            }
            catch (Exception)
            {
                return "0000" + ext + "00" + ext + "" + "00" + "00" + ":" + "00" + ":" + "00";
            }
        }
        public static string FormatUTCDateTimeMonthYear(DateTime dtime)
        {
            try
            {
                string _month = Convert.ToString(dtime.Month);
                if (_month.Length == 1)
                    _month = "0" + _month;
                return dtime.Year + "-" + _month;
            }
            catch (Exception)
            {
                return "0000" + "-" + "00";
            }
        }
        public static string ConvertToUpper(string strText)
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
                result =result.Replace(",", " ");
                return result.Trim();
            }
            catch (Exception)
            {
                return string.Empty;
            }
            //
           
        }




        //###CLASS###################################################################################################################################################################################

    }
    //###CLASS###################################################################################################################################################################################
    public class Security
    {
        private static byte[] LEVEL1_SALT = new byte[] { 56, 3, 5, 64, 95, 6, 6, 2, 54, 3, 54, 86, 4, 98, 65, 46, 48, 64, 6, 87, 46 };
        private static byte[] LEVEL2_SALT = new byte[] { 121, 219, 95, 76, 5, 36, 9, 22, 3, 8, 64, 31, 8, 64, 172, 199, 100, 200 };
        public static string Encryption256Hashed(string strRaw)
        {
            var sha256Hash = System.Security.Cryptography.SHA256.Create();
            var hashArray = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(strRaw).Concat(LEVEL1_SALT).Concat(LEVEL2_SALT).ToArray());
            string result = "";
            foreach (var byteHash in hashArray)
            {
                result += string.Format("{0:x2}", byteHash);
            }
            return result;
        }

        public static string OTPCode
        {
            get
            {
                return GenerateRandomString(8);
            }
        }
        public static string GenerateRandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static string KeyCode
        {
            get
            {
                var now = Current.Now;

                string _month = now.Month + "";
                if (_month.Length < 2)
                    _month = "0" + _month;

                string _day = now.Day + "";
                if (_day.Length < 2)
                    _day = "0" + _day;

                string _hour = now.Hour + "";
                if (_hour.Length < 2)
                    _hour = "0" + _hour;

                string _minute = now.Minute + "";
                if (_minute.Length < 2)
                    _minute = "0" + _minute;

                string _second = now.Second + "";
                if (_second.Length < 2)
                    _second = "0" + _second;

                string _msecond = now.Millisecond + "";
                if (_msecond.Length < 3)
                    _msecond = "0" + _msecond;
                else if (_msecond.Length < 2)
                    _msecond = "00" + _msecond;

                string _millisecond = now.Millisecond.ToString();
                if (_millisecond.Length == 1)
                    _millisecond = "00" + _millisecond;
                if (_millisecond.Length == 2)
                    _millisecond = "0" + _millisecond;

                return now.Year + "" + _month + "-" + _day + "" + _hour + "" + _minute + "-" + _second + "" + _millisecond;

            }
        }


        public static string QRCodeGenerate(string key)
        {
            try
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(key, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                Bitmap qrCodeImage = qrCode.GetGraphic(20);

                using (Bitmap bitmap = qrCode.GetGraphic(20))
                {
                    using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
                    {
                        bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Bmp);
                        byte[] byteImage = memoryStream.ToArray();
                        return "data:image/Bmp; base64," + Convert.ToBase64String(byteImage);
                    }
                }
            }
            catch (Exception)
            {

                return string.Empty;
            }
        }


        public class Token
        {
            public static string GenerateToken()
            {
                byte[] bytes = System.Text.Encoding.Unicode.GetBytes(Guid.NewGuid().ToString());
                SHA384Managed hashstring = new SHA384Managed();
                byte[] hash = hashstring.ComputeHash(bytes);
                string hashString = string.Empty;
                foreach (byte x in hash)
                {
                    hashString += String.Format("{0:x2}", x);
                }
                return hashString;
            }

            public static string EncryptSH384(string value)
            {
                byte[] bytes = System.Text.Encoding.Unicode.GetBytes(value);
                SHA384Managed hashstring = new SHA384Managed();
                byte[] hash = hashstring.ComputeHash(bytes);
                string hashString = string.Empty;
                foreach (byte x in hash)
                {
                    hashString += String.Format("{0:x2}", x);
                }
                return hashString;
            }
            public static string Create(string userId, string guid)
            {
                try
                {
                    string strGuid = new Guid().ToString();
                    var handler = new JwtSecurityTokenHandler();
                    ClaimsIdentity identity = new ClaimsIdentity(
                        new System.Security.Principal.GenericIdentity(userId, "TokenAuthenzation"),
                        new[] {
                              new Claim("TokenID", userId),
                              new Claim("TokenKey", strGuid),
                              new Claim("TokenTime", DateTime.Now.ToString())
                        });
                    ClaimsPrincipal.Current.AddIdentity(identity);
                    var securityToken = handler.CreateToken(new SecurityTokenDescriptor
                    {
                        SigningCredentials = new SigningCredentials(new RsaSecurityKey(new RSACryptoServiceProvider(2048)), SecurityAlgorithms.RsaSha256Signature),
                        Subject = identity
                    });
                    string strToken = handler.WriteToken(securityToken);
                    return strToken;
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
        }
    }

    public class Cookies
    {
        public static void SetCookiForAuthenzation(string _uid, string _pid)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies.Get("Authenzation");
            if (cookie != null)
            {
                cookie.Values["UID"] = _uid;
                cookie.Values["PID"] = _pid;
                cookie.Expires = DateTime.Now.AddDays(10);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
            else
            {
                cookie = new HttpCookie("Authenzation");
                cookie.Values["UID"] = _uid;
                cookie.Values["PID"] = _pid;
                cookie.Expires = DateTime.Now.AddDays(10);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }
        public static string GetCookiForAuthenzation()
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies.Get("Authenzation");
            if (cookie != null)
                return cookie.Value;
            else
                return string.Empty;
        }
    }
    public class Default
    {
        public static string LanguageID = "en";
    }

    public class HServer
    {
        public static DateTime TimeNow(IDbTransaction transaction = null)
        {
            try
            {
                using (var _connectDb = DbConnect.Connection.CMS)
                {
                    string sqlQuery = "SELECT GETDATE() AS [DTime]";
                    var dt = _connectDb.Query<TimeModel>(sqlQuery, transaction: transaction).FirstOrDefault();
                    return dt.DTime;
                }
            }
            catch (Exception)
            {
                return DateTime.Now;
            }
        }
    }
    public class TimeModel
    {
        public DateTime DTime { get; set; }
    }

    //###CLASS###################################################################################################################################################################################
    public class Navigate
    {
        public static void PageNotFound()
        {
            System.Web.HttpContext.Current.Response.Redirect("/page/not-found.html");
        }
        public static void PageNotService()
        {
            HttpContext.Current.Response.Redirect("/page/not-service.html");
        }

        public class BackEnd
        {
            public static string URLCreate
            {
                get
                {
                    try
                    {
                        var meta = new MetaSEO();
                        string _url = "/backend/" + MetaSEO.ControllerText + "/create";
                        return _url.ToLower();
                    }
                    catch (Exception)
                    {
                        return string.Empty;
                    }
                }
            }
        }
        public class Manager
        {
            public static string URLCreate
            {
                get
                {
                    try
                    {
                        var meta = new MetaSEO();
                        string _url = "/adm/" + MetaSEO.ControllerText + "/create";
                        return _url.ToLower();
                    }
                    catch (Exception)
                    {
                        return string.Empty;
                    }
                }
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
        public static string PageTitle
        {
            get
            {
                try
                {
                    string _controller = MetaSEO.ControllerText;
                    string _action = MetaSEO.ActionText;
                    using (var _service = new MenuItemService())
                    {
                        string strTitle = _service.GetMenuName(_controller, _action);
                        if (string.IsNullOrWhiteSpace(strTitle))
                            return string.Empty;
                        return strTitle.ToUpper();
                    }
                }
                catch (Exception ex)
                {
                    return "HRM | MANAGE & " + ex;
                }
            }
        }
    }
}