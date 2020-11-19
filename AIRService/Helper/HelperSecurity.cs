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
using System.IO;
using System.Windows.Media;
using Helper.Language;
using ZXing;
using System.Windows;

namespace Helper.Security
{
    public class Library
    {
        private static readonly byte[] LEVEL1_SALT = new byte[] { 56, 3, 5, 64, 95, 6, 6, 2, 54, 3, 54, 86, 4, 98, 65, 46, 48, 64, 6, 87, 46 };
        private static readonly byte[] LEVEL2_SALT = new byte[] { 121, 219, 95, 76, 5, 36, 9, 22, 3, 8, 64, 31, 8, 64, 172, 199, 100, 200 };
        //  
        public static string InputKey(string _inp, int _len = 20)
        {
            if (string.IsNullOrWhiteSpace(_inp))
                return string.Empty;
            //
            if (_inp.Length > _len)
                return string.Empty;
            //
            int _ranLen = _len - _inp.Length;
            string result = "";
            for (int i = 0; i < _ranLen; i++)
                result += "0";
            // 
            return result + _inp;
        }
        //
        public static string IDCode(int lent = 6)
        {
            return RandomString(lent);
        }
        public static string OTPCode
        {
            get
            {
                return RandomString(8);
            }
        }
        public static string KEYCode
        {
            get
            {
                var dateTime = DateTime.Now;
                string _month = dateTime.Month + "";
                if (_month.Length < 2)
                    _month = "0" + _month;

                string _day = dateTime.Day + "";
                if (_day.Length < 2)
                    _day = "0" + _day;

                string _hour = dateTime.Hour + "";
                if (_hour.Length < 2)
                    _hour = "0" + _hour;

                string _minute = dateTime.Minute + "";
                if (_minute.Length < 2)
                    _minute = "0" + _minute;

                string _second = dateTime.Second + "";
                if (_second.Length < 2)
                    _second = "0" + _second;

                string _msecond = dateTime.Millisecond + "";
                if (_msecond.Length < 3)
                    _msecond = "0" + _msecond;
                else if (_msecond.Length < 2)
                    _msecond = "00" + _msecond;

                string _millisecond = dateTime.Millisecond.ToString();
                if (_millisecond.Length == 1)
                    _millisecond = "00" + _millisecond;
                if (_millisecond.Length == 2)
                    _millisecond = "0" + _millisecond;

                return dateTime.Year + "" + _month + "-" + _day + "" + _hour + "" + _minute + "-" + _second + "" + _millisecond;

            }
        }
        public static string KEYTEXT(string _val)
        {
            int lenDefault = 8;
            if (!string.IsNullOrWhiteSpace(_val))
            {
                int lenVal = _val.Length;
                if (lenDefault > lenVal)
                    return null;
                //
                int lenCode = lenDefault - lenVal;
                //
                string strCode = "";
                if (lenCode > 0)
                {
                    for (int i = 0; i < lenCode; i++)
                    {
                        strCode += "0";
                    }
                }
                return strCode + _val;
            }
            return null;
        }

        //
        public Bitmap GenerateQRCode(string text)
        {
            try
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData QRCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(QRCodeData);
                Bitmap bitmap = qrCode.GetGraphic(10);
                Bitmap bitmapRs = new Bitmap(bitmap, 300, 300);
                return bitmapRs;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public Bitmap GenerateQRCodeLogo(string text)
        {
            try
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData QRCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(QRCodeData);
                Bitmap bitmapIcon = new Bitmap($"{ HttpContext.Current.Server.MapPath("~/Files/default")}/logo.png");
                var _background = System.Drawing.Color.White;
                var _lightColor = System.Drawing.Color.Black;
                Bitmap bitmap = qrCode.GetGraphic(10, _background, _lightColor, bitmapIcon, 15, 15, true);
                Graphics graphics = Graphics.FromImage(bitmap);
                int _w = (bitmap.Width - bitmapIcon.Width) / 2;
                int _h = (bitmap.Height - bitmapIcon.Height) / 2;
                graphics.DrawImage(bitmapIcon, new System.Drawing.Point(_w, _h));
                Bitmap bitmapRs = new Bitmap(bitmap, 300, 300);
                return bitmapRs;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Image GenerateZXING_QRCode(string _text)
        {

            ZXing.BarcodeWriter barcodeWriter = new ZXing.BarcodeWriter();
            ZXing.Common.EncodingOptions encodingOptions = new ZXing.Common.EncodingOptions()
            {
                Width = 300,
                Height = 300,
                Margin = 5,
                PureBarcode = false
            };
            encodingOptions.Hints.Add(EncodeHintType.ERROR_CORRECTION, ZXing.QrCode.Internal.ErrorCorrectionLevel.H);
            barcodeWriter.Renderer = new ZXing.Rendering.BitmapRenderer();
            barcodeWriter.Options = encodingOptions;
            barcodeWriter.Format = ZXing.BarcodeFormat.QR_CODE;
            Bitmap bitmap = barcodeWriter.Write(_text);
            Graphics graphics = Graphics.FromImage(bitmap);
            Bitmap logo = new Bitmap($"{ HttpContext.Current.Server.MapPath("~/Files/default")}/delete-sign.png");
            int _w = (bitmap.Width - logo.Width) / 2;
            int _h = (bitmap.Height - logo.Height) / 2;
            graphics.DrawImage(logo, new System.Drawing.Point(_w, _h));
            return bitmap;

        }
        //
        public static string Encryption256(string strRaw)
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
        //
        public static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string result = new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
            return result;
        }
        //
        public static string FakeGuidID(string str)
        {
            str = str.ToLower();
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] hash = md5.ComputeHash(System.Text.Encoding.Default.GetBytes(str));
                Guid result = new Guid(hash);
                return result.ToString().ToLower();
            }
        }
    }
    public class HashToken
    {
        public static string NewToken()
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
        public static string Create(string tokeId)
        {
            try
            {
                string strGuid = new Guid().ToString();
                var handler = new JwtSecurityTokenHandler();
                ClaimsIdentity identity = new ClaimsIdentity(
                    new System.Security.Principal.GenericIdentity(tokeId, "TokenAuthenzation"),
                    new[] {
                              new Claim("TokenID", tokeId),
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
    public class Cookies
    {
        //Authen *****************************************************************************************************
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
        //Laguage *****************************************************************************************************
        public static void SetCookiForLanguage(string _name, string _code)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies.Get("Language");
            if (cookie != null)
            {
                cookie.Values["LanguageName"] = _name;
                cookie.Values["LanguageCode"] = _code;
                cookie.Expires = DateTime.Now.AddDays(10);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
            else
            {
                cookie = new HttpCookie("Language");
                cookie.Values["LanguageName"] = _name;
                cookie.Values["LanguageCode"] = _code;
                cookie.Expires = DateTime.Now.AddDays(10);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }
        public static LanguageCodeOption GetCookiForLanguage()
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies.Get("Language");
            LanguageCodeOption languageCodeOption = new LanguageCodeOption();
            if (cookie != null)
            {
                string id = string.Empty;
                string name = string.Empty;
                //
                if (HttpContext.Current.Request.Cookies["Language"] != null)
                {
                    id = cookie.Values["LanguageCode"];
                    name = cookie.Values["LanguageName"];
                }
                //
                if (!string.IsNullOrWhiteSpace(id))
                    languageCodeOption = new LanguageCodeOption { ID = id, Name = name };
                //
                return languageCodeOption;
            }
            //
            return new LanguageCodeOption();
        }
    }
}