//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;

//namespace AIRService.Helper
//{
//    class ValidData
//    {
//        public static string RegexUserName = @"[a-zA-Z0-9]+$";
//        //public static string RegexPassword = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#\$%\^&\*])(?=.{2,20})";
//        public static string RegexPassword = @"[a-zA-Z0-9]+$";
//        public static string RegexPin = @"[0-9]+$";
//        public static string RegexFName = @"^([a-zA-Z0-9 ẮẰẲẴẶĂẤẦẨẪẬÂÁÀÃẢẠĐẾỀỂỄỆÊÉÈẺẼẸÍÌỈĨỊỐỒỔỖỘÔỚỜỞỠỢƠÓÒÕỎỌỨỪỬỮỰƯÚÙỦŨỤÝỲỶỸỴắằẳẵặăấầẩẫậâáàãảạđếềểễệêéèẻẽẹíìỉĩịốồổỗộôớờởỡợơóòõỏọứừửữựưúùủũụýỳỷỹỵ]*)([a-zA-Z0-9 ẮẰẲẴẶĂẤẦẨẪẬÂÁÀÃẢẠĐẾỀỂỄỆÊÉÈẺẼẸÍÌỈĨỊỐỒỔỖỘÔỚỜỞỠỢƠÓÒÕỎỌỨỪỬỮỰƯÚÙỦŨỤÝỲỶỸỴắằẳẵặăấầẩẫậâáàãảạđếềểễệêéèẻẽẹíìỉĩịốồổỗộôớờởỡợơóòõỏọứừửữựưúùủũụýỳỷỹỵ]*)+$";
//        public static string RegexFormatText = @"([a-zA-Z0-9 ẮẰẲẴẶĂẤẦẨẪẬÂÁÀÃẢẠĐẾỀỂỄỆÊÉÈẺẼẸÍÌỈĨỊỐỒỔỖỘÔỚỜỞỠỢƠÓÒÕỎỌỨỪỬỮỰƯÚÙỦŨỤÝỲỶỸỴắằẳẵặăấầẩẫậâáàãảạđếềểễệêéèẻẽẹíìỉĩịốồổỗộôớờởỡợơóòõỏọứừửữựưúùủũụýỳỷỹỵ .:]*)([a-zA-Z0-9 ẮẰẲẴẶĂẤẦẨẪẬÂÁÀÃẢẠĐẾỀỂỄỆÊÉÈẺẼẸÍÌỈĨỊỐỒỔỖỘÔỚỜỞỠỢƠÓÒÕỎỌỨỪỬỮỰƯÚÙỦŨỤÝỲỶỸỴắằẳẵặăấầẩẫậâáàãảạđếềểễệêéèẻẽẹíìỉĩịốồổỗộôớờởỡợơóòõỏọứừửữựưúùủũụýỳỷỹỵ .:]*)+$";
//        public static string RegexAlphabet = @"([a-zA-Z0-9 ẮẰẲẴẶĂẤẦẨẪẬÂÁÀÃẢẠĐẾỀỂỄỆÊÉÈẺẼẸÍÌỈĨỊỐỒỔỖỘÔỚỜỞỠỢƠÓÒÕỎỌỨỪỬỮỰƯÚÙỦŨỤÝỲỶỸỴắằẳẵặăấầẩẫậâáàãảạđếềểễệêéèẻẽẹíìỉĩịốồổỗộôớờởỡợơóòõỏọứừửữựưúùủũụýỳỷỹỵ .:]*)([a-zA-Z0-9 ẮẰẲẴẶĂẤẦẨẪẬÂÁÀÃẢẠĐẾỀỂỄỆÊÉÈẺẼẸÍÌỈĨỊỐỒỔỖỘÔỚỜỞỠỢƠÓÒÕỎỌỨỪỬỮỰƯÚÙỦŨỤÝỲỶỸỴắằẳẵặăấầẩẫậâáàãảạđếềểễệêéèẻẽẹíìỉĩịốồổỗộôớờởỡợơóòõỏọứừửữựưúùủũụýỳỷỹỵ .:]*)+$";
//        public static string RegexPhone = @"((\+84)|(09|01[2|6|8|9]))+([0-9]{8,12})\b$";
//        public static string RegexTel = @"^((\+\d{1,3}(-| )?\(?\d\)?(-| )?\d{1,3})|(\(?\d{2,3}\)?))(-| )?(\d{3,4})(-| )?(\d{4})(( x| ext)\d{1,5}){0,1}$";
//        public static string RegexEmail = @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
//        public static string RegexRollNumber = @"[a-zA-Z0-9]+$";
//        public static string RegexDateVN = @"^([0]?[0-9]|[12][0-9]|[3][01])[./-]([0]?[1-9]|[1][0-2])[./-]([0-9]{4}|[0-9]{2})$";
//        public static string RegexDate = @"^([12]\d{3}-(0[1-9]|1[0-2])-(0[1-9]|[12]\d|3[01]))$"; // yyyy-MM-dd
//        public static string RegexDate2 = @"^(0?[1-9]|1[0-2])[-/](0?[1-9]|[12][0-9]|3[01])[-/](19[5-9][0-9]|20[0-4][0-9]|2050)$"; // yyyy-MM-dd
//        // /^(0?[1-9]|1[0-2])[-/](0?[1-9]|[12][0-9]|3[01])[-/](19[5-9][0-9]|20[0-4][0-9]|2050)$/
//        public static string RegexTimekeeper = @"^([12]\d{3}-(0[1-9]|1[0-2])-(0[1-9]|[12]\d|3[01]))$"; // yyyy-MM-dd
//        public static bool FormatUserName(string param)
//        {
//            var reg = new System.Text.RegularExpressions.Regex(RegexUserName);
//            bool result = reg.IsMatch(param);
//            if (result)
//                return true;
//            else
//                return false;
//        }
//        public static bool FormatPassword(string param)
//        {
//            var reg = new System.Text.RegularExpressions.Regex(RegexPassword);
//            bool result = reg.IsMatch(param);
//            if (result)
//                return true;
//            else
//                return false;
//        }
//        public static bool FormatPin(string param)
//        {
//            var reg = new System.Text.RegularExpressions.Regex(RegexPin);
//            bool result = reg.IsMatch(param);
//            if (result)
//                return true;
//            else
//                return false;
//        }
//        public static bool FormatLastName(string param)
//        {
//            var reg = new System.Text.RegularExpressions.Regex(RegexFName);
//            bool result = reg.IsMatch(param);
//            if (result)
//                return true;
//            else
//                return false;
//        }
//        public static bool FormatEmail(string param)
//        {
//            var reg = new System.Text.RegularExpressions.Regex(RegexEmail);
//            bool result = reg.IsMatch(param);
//            if (result)
//                return true;
//            else
//                return false;
//        }
//        public static bool FormatRollNumber(string param)
//        {
//            var reg = new System.Text.RegularExpressions.Regex(RegexRollNumber);
//            bool result = reg.IsMatch(param);
//            if (result)
//                return true;
//            else
//                return false;
//        }
//        public static bool FormatText(string param)
//        {
//            var reg = new System.Text.RegularExpressions.Regex(RegexFormatText);
//            bool result = reg.IsMatch(param);
//            if (result)
//                return true;
//            else
//                return false;
//        }
//        public static bool FormatAlphabet(string param)
//        {
//            var reg = new System.Text.RegularExpressions.Regex(RegexAlphabet);
//            bool result = reg.IsMatch(param);
//            if (result)
//                return true;
//            else
//                return false;
//        }
//        public static bool FormatPhone(string param)
//        {
//            var reg = new System.Text.RegularExpressions.Regex(RegexPhone); //  allow spaces
//            bool result = reg.IsMatch(param);
//            if (result)
//                return true;
//            else
//                return false;

//            //return System.Text.RegularExpressions.Regex.Match(number, @"^(\+[0-9]{9})$").Success;
//        }
//        public static bool FormatTel(string number)
//        {
//            return System.Text.RegularExpressions.Regex.Match(number, RegexTel).Success;
//        }
//        public static bool FormatDateVN(string param)
//        {
//            return System.Text.RegularExpressions.Regex.Match(param, RegexDateVN).Success;
//        }
//        public static bool FormatDateSQL(string param)
//        {
//            return System.Text.RegularExpressions.Regex.Match(param, RegexDate).Success;
//        }
//        public static bool FormatDateTime(string text)
//        {

//            // Check for empty string.
//            if (string.IsNullOrEmpty(text))
//            {
//                return false;
//            }

//            bool isDateTime = DateTime.TryParse(text, out _);

//            return isDateTime;
//        }
//        public static bool FormatImageFile(string extension)
//        {
//            try
//            {
//                if (string.IsNullOrEmpty(extension))
//                    return false;
//                extension = extension.ToLower();

//                return true;
//                // return ImgExtensions.Contains(extension);
//            }
//            catch (Exception)
//            {
//                return false;
//            }

//        }
//        public static bool FormatFile(string extension)
//        {
//            bool rs = false;
//            try
//            {
//                if (string.IsNullOrEmpty(extension))
//                    return false;
//                extension = extension.ToLower();
//                return FormatFileExtensions.Contains(extension);
//            }
//            catch (Exception)
//            {
//                rs = false;
//            }
//            return rs;
//        }
//        public static int FormatOfficeExcelExtension(string Extension)
//        {
//            int rs = -1;
//            try
//            {
//                if (Extension.Length > 0 && Extension.ToLower() != "")
//                {
//                    switch (Extension.ToLower())
//                    {
//                        case ".xlsx":
//                            rs = 1;
//                            break;
//                        default:
//                            rs = -1;
//                            break;
//                    }
//                }
//            }
//            catch (Exception)
//            {
//                rs = -1;
//            }
//            return rs;
//        }
//        public static int FormatVideoExtension(string Extension)
//        {
//            int rs = -1;
//            try
//            {
//                if (Extension.Length > 0 && Extension.ToLower() != "")
//                {
//                    switch (Extension.ToLower())
//                    {
//                        case ".avi":
//                            rs = 1;
//                            break;
//                        case ".3gp":
//                            rs = 1;
//                            break;
//                        case ".flv":
//                            rs = 1;
//                            break;
//                        case ".mp4":
//                            rs = 1;
//                            break;
//                        default:
//                            rs = -1;
//                            break;
//                    }
//                }
//            }
//            catch (Exception)
//            {
//                rs = -1;
//            }
//            return rs;
//        }

//        public static IList<string> FormatFileExtensions = new List<string>
//        {
//            ".png", ".jpg", ".jpeg", ".svg", ".mp3", ".mp4", ".pdf", ".xls", ".xlsx", ".doc", ".docx", ".rar", ".zip", ".txt", ".ppt", ".pptx"
//        };
//        public static IList<string> ImgExtensions = new List<string>
//        {
//            ".png", ".jpg", ".jpeg",".gif"
//        };

//        public static bool FormatGuid(string text)
//        {
//            return (new Regex(@"(\A{[a-z\d]{8}(-[a-z\d]{4}){3}-[a-z\d]{12}}\z)|(\A[a-z\d]{8}(-[a-z\d]{4}){3}-[a-z\d]{12}\z)", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.CultureInvariant)).IsMatch(text);
//        }
//        private const string YoutubeLinkRegex = "(?:.+?)?(?:\\/v\\/|watch\\/|\\?v=|\\&v=|youtu\\.be\\/|\\/v=|^youtu\\.be\\/)([a-zA-Z0-9_-]{11})+";
//        private static readonly System.Text.RegularExpressions.Regex regexExtractId = new System.Text.RegularExpressions.Regex(YoutubeLinkRegex, System.Text.RegularExpressions.RegexOptions.Compiled);
//        private static readonly string[] validAuthorities = { "youtube.com", "www.youtube.com", "youtu.be", "www.youtu.be" };
//        public static string ExtractVideoIdFromUri(string _path)
//        {
//            try
//            {
//                System.Uri uri = new System.Uri(_path);
//                string authority = new UriBuilder(uri).Uri.Authority.ToLower();

//                //check if the url is a youtube url
//                if (!validAuthorities.Contains(authority))
//                    return string.Empty;
//                //and extract the id
//                var regRes = regexExtractId.Match(uri.ToString());
//                if (regRes.Success)
//                    return regRes.Groups[1].Value;

//                return string.Empty;
//            }
//            catch
//            {
//                return string.Empty;
//            }
//        }
//        public static bool FormatNumeric(string number)
//        {
//            try
//            {
//                bool isNumeric = int.TryParse(number, out int n);
//                return isNumeric;
//            }
//            catch
//            {
//                return false;
//            }
//        }
//    }

//}
