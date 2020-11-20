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

namespace Helper.Language
{
    public class Resource
    {
        public class MenuText
        {
            public string Account = "Tài khoản";
            public string MenuArea = "Phân vùng";
            public string MenuCategory = "Danh mục menu";
            public string MenuItem = "Menu quản lý";
            public string DataList = "Danh sách";
            public string Details = "Chi tiết";
            public string ChangePassword = "Thay đổi mật khẩu";
            public string Pincode = "Đăng nhập với mã pin";
            public string Setting = "Thiết lập";
        }
        public class Static
        {

        }
        public class Label
        {
            public static string BtnCreate { get; set; } = "Thêm mới";
            public static string BtnUpdate { get; set; } = "Cập nhật";
            public static string BtnDelete { get; set; } = "Xóa";
            public static string BtnSearch { get; set; } = "Tìm kiếm";
            public static string BtnClose { get; set; } = "Đóng";
            public static string BtnOpen { get; set; } = "Mở";
            public static string BtnReset { get; set; } = "Làm mới";
            public static string BtnCancel { get; set; } = "Hủy bỏ";
            public static string HtmlText { get; set; } = "Nội dung";
            public static string Summary { get; set; } = "Mô tả";
            public static string HtmlNote { get; set; } = "Mô tả (html)";
            public static string Active { get; set; } = "Kích hoạt";
            public static string NoActive { get; set; } = "Không kích hoạt";
            public static string IDNo { get; set; } = "IDNo.";
            public static string DataList { get; set; } = "Danh sách";
            public static string Display { get; set; } = "Hiển thị";
            public static string Hide { get; set; } = "Ẩn";
            public static string Category { get; set; } = "Danh mục";
            public static string Status { get; set; } = "Trạng thái";
            public static string State { get; set; } = "Tình trạng";
            public static string Option { get; set; } = "Lựa chọn";
            public static string OptionNew { get; set; } = "Tạo mới";
            public static string Photo { get; set; } = "Hình ảnh";
            public static string Title { get; set; } = "Tiêu đề";
            public static string Alias { get; set; } = "Đường dẫn";
            public static string Area { get; set; } = "Phân vùng";
            public static string Control { get; set; } = "Điều khiển";
            public static string GoBack { get; set; } = "Trở lại";
        }
        public class Field
        {

        }
        public class Text
        {

        }
    }

    public class LanguageCode
    {
        public static LanguageCodeOption Abkhazian = new LanguageCodeOption { Name = "Abkhazian", ID = "ab" };
        public static LanguageCodeOption Afrikaans = new LanguageCodeOption { Name = "Afrikaans", ID = "af" };
        public static LanguageCodeOption Akan = new LanguageCodeOption { Name = "Akan", ID = "ak" };
        public static LanguageCodeOption Albanian = new LanguageCodeOption { Name = "Albanian", ID = "sq" };
        public static LanguageCodeOption Amharic = new LanguageCodeOption { Name = "Amharic", ID = "am" };
        public static LanguageCodeOption Arabic = new LanguageCodeOption { Name = "Arabic", ID = "ar" };
        public static LanguageCodeOption Aragonese = new LanguageCodeOption { Name = "Aragonese", ID = "an" };
        public static LanguageCodeOption Armenian = new LanguageCodeOption { Name = "Armenian", ID = "hy" };
        public static LanguageCodeOption Assamese = new LanguageCodeOption { Name = "Assamese", ID = "as" };
        public static LanguageCodeOption Avaric = new LanguageCodeOption { Name = "Avaric", ID = "av" };
        public static LanguageCodeOption Avestan = new LanguageCodeOption { Name = "Avestan", ID = "ae" };
        public static LanguageCodeOption Aymara = new LanguageCodeOption { Name = "Aymara", ID = "ay" };
        public static LanguageCodeOption Azerbaijani = new LanguageCodeOption { Name = "Azerbaijani", ID = "az" };
        public static LanguageCodeOption Bambara = new LanguageCodeOption { Name = "Bambara", ID = "bm" };
        public static LanguageCodeOption Bashkir = new LanguageCodeOption { Name = "Bashkir", ID = "ba" };
        public static LanguageCodeOption Basque = new LanguageCodeOption { Name = "Basque", ID = "eu" };
        public static LanguageCodeOption Belarusian = new LanguageCodeOption { Name = "Belarusian", ID = "be" };
        public static LanguageCodeOption Bengali = new LanguageCodeOption { Name = "Bengali (Bangla)", ID = "bn" };//(Bangla)
        public static LanguageCodeOption Bihari = new LanguageCodeOption { Name = "Bihari", ID = "bh" };
        public static LanguageCodeOption Bislama = new LanguageCodeOption { Name = "Bislama", ID = "bi" };
        public static LanguageCodeOption Bosnian = new LanguageCodeOption { Name = "Bosnian", ID = "bs" };
        public static LanguageCodeOption Breton = new LanguageCodeOption { Name = "Breton", ID = "br" };
        public static LanguageCodeOption Bulgarian = new LanguageCodeOption { Name = "Bulgarian", ID = "bg" };
        public static LanguageCodeOption Burmese = new LanguageCodeOption { Name = "Burmese", ID = "my" };
        public static LanguageCodeOption Catalan = new LanguageCodeOption { Name = "Catalan", ID = "ca" };
        public static LanguageCodeOption Chamorro = new LanguageCodeOption { Name = "Chamorro", ID = "ch" };
        public static LanguageCodeOption Chechen = new LanguageCodeOption { Name = "Chechen", ID = "ce" };
        public static LanguageCodeOption Chichewa_Chewa_Nyanja = new LanguageCodeOption { Name = "Chichewa, Chewa, Nyanja", ID = "ny" };
        public static LanguageCodeOption Chinese = new LanguageCodeOption { Name = "Chinese", ID = "zh" };
        public static LanguageCodeOption Chinese_Simplified = new LanguageCodeOption { Name = "Chinese (Simplified)", ID = "zh-Hans" };//Simplified
        public static LanguageCodeOption Chinese_Traditional = new LanguageCodeOption { Name = "Chinese (Traditional)", ID = "zh-Hant" };//Traditional
        public static LanguageCodeOption Chuvash = new LanguageCodeOption { Name = "Chuvash", ID = "cv" };
        public static LanguageCodeOption Cornish = new LanguageCodeOption { Name = "Cornish", ID = "kw" };
        public static LanguageCodeOption Corsican = new LanguageCodeOption { Name = "Corsican", ID = "co" };
        public static LanguageCodeOption Cree = new LanguageCodeOption { Name = "Cree", ID = "cr" };
        public static LanguageCodeOption Croatian = new LanguageCodeOption { Name = "Croatian", ID = "hr" };
        public static LanguageCodeOption Czech = new LanguageCodeOption { Name = "Czech", ID = "cs" };
        public static LanguageCodeOption Danish = new LanguageCodeOption { Name = "Danish", ID = "da" };
        public static LanguageCodeOption Divehi, Dhivehi, Maldivian = new LanguageCodeOption { Name = "Divehi, Dhivehi, Maldivian", ID = "dv" };
        public static LanguageCodeOption Dutch = new LanguageCodeOption { Name = "Dutch", ID = "nl" };
        public static LanguageCodeOption Dzongkha = new LanguageCodeOption { Name = "Dzongkha", ID = "dz" };
        public static LanguageCodeOption English = new LanguageCodeOption { Name = "English", ID = "en" };
        public static LanguageCodeOption Esperanto = new LanguageCodeOption { Name = "Esperanto", ID = "eo" };
        public static LanguageCodeOption Estonian = new LanguageCodeOption { Name = "Estonian", ID = "et" };
        public static LanguageCodeOption Ewe = new LanguageCodeOption { Name = "Ewe", ID = "ee" };
        public static LanguageCodeOption Faroese = new LanguageCodeOption { Name = "Faroese", ID = "fo" };
        public static LanguageCodeOption Fijian = new LanguageCodeOption { Name = "Fijian", ID = "fj" };
        public static LanguageCodeOption Finnish = new LanguageCodeOption { Name = "Finnish", ID = "fi" };
        public static LanguageCodeOption French = new LanguageCodeOption { Name = "French", ID = "fr" };
        public static LanguageCodeOption Fula, Fulah, Pulaar, Pular = new LanguageCodeOption { Name = "Fula, Fulah, Pulaar, Pular", ID = "ff" };
        public static LanguageCodeOption Galician = new LanguageCodeOption { Name = "Galician", ID = "gl" };
        public static LanguageCodeOption Gaelic_Scottish = new LanguageCodeOption { Name = "Gaelic (Scottish)", ID = "gd" };
        public static LanguageCodeOption Gaelic_Manx = new LanguageCodeOption { Name = "Gaelic (Manx)", ID = "gv" };
        public static LanguageCodeOption Georgian = new LanguageCodeOption { Name = "Georgian", ID = "ka" };
        public static LanguageCodeOption German = new LanguageCodeOption { Name = "German", ID = "de" };
        public static LanguageCodeOption Greek = new LanguageCodeOption { Name = "Greek", ID = "el" };
        public static LanguageCodeOption Greenlandic = new LanguageCodeOption { Name = "Greenlandic", ID = "kl" };
        public static LanguageCodeOption Guarani = new LanguageCodeOption { Name = "Guarani", ID = "gn" };
        public static LanguageCodeOption Gujarati = new LanguageCodeOption { Name = "Gujarati", ID = "gu" };
        public static LanguageCodeOption Haitian_Creole = new LanguageCodeOption { Name = "Haitian Creole", ID = "ht" };
        public static LanguageCodeOption Hausa = new LanguageCodeOption { Name = "Hausa", ID = "ha" };
        public static LanguageCodeOption Hebrew = new LanguageCodeOption { Name = "Hebrew", ID = "he" };
        public static LanguageCodeOption Herero = new LanguageCodeOption { Name = "Herero", ID = "hz" };
        public static LanguageCodeOption Hindi = new LanguageCodeOption { Name = "Hindi", ID = "hi" };
        public static LanguageCodeOption Hiri_Motu = new LanguageCodeOption { Name = "Hiri Motu", ID = "ho" };
        public static LanguageCodeOption Hungarian = new LanguageCodeOption { Name = "Hungarian", ID = "hu" };
        public static LanguageCodeOption Icelandic = new LanguageCodeOption { Name = "Icelandic", ID = "is" };
        public static LanguageCodeOption Ido = new LanguageCodeOption { Name = "Ido", ID = "io" };
        public static LanguageCodeOption Igbo = new LanguageCodeOption { Name = "Igbo", ID = "ig" };
        public static LanguageCodeOption Indonesian = new LanguageCodeOption { Name = "Indonesian", ID = "id" }; //id, in
        public static LanguageCodeOption Interlingua = new LanguageCodeOption { Name = "Interlingua", ID = "ia" };
        public static LanguageCodeOption Interlingue = new LanguageCodeOption { Name = "Interlingue", ID = "ie" };
        public static LanguageCodeOption Inuktitut = new LanguageCodeOption { Name = "Inuktitut", ID = "iu" };
        public static LanguageCodeOption Inupiak = new LanguageCodeOption { Name = "Inupiak", ID = "ik" };
        public static LanguageCodeOption Irish = new LanguageCodeOption { Name = "Irish", ID = "ga" };
        public static LanguageCodeOption Italian = new LanguageCodeOption { Name = "Italian", ID = "it" };
        public static LanguageCodeOption Japanese = new LanguageCodeOption { Name = "Japanese", ID = "ja" };
        public static LanguageCodeOption Javanese = new LanguageCodeOption { Name = "Javanese", ID = "jv" };
        public static LanguageCodeOption Kalaallisut_Greenlandic = new LanguageCodeOption { Name = "Kalaallisut, Greenlandic", ID = "kl" };
        public static LanguageCodeOption Kannada = new LanguageCodeOption { Name = "Kannada", ID = "kn" };
        public static LanguageCodeOption Kanuri = new LanguageCodeOption { Name = "Kanuri", ID = "kr" };
        public static LanguageCodeOption Kashmiri = new LanguageCodeOption { Name = "Kashmiri", ID = "ks" };
        public static LanguageCodeOption Kazakh = new LanguageCodeOption { Name = "Kazakh", ID = "kk" };
        public static LanguageCodeOption Khmer = new LanguageCodeOption { Name = "Khmer", ID = "km" };
        public static LanguageCodeOption Kikuyu = new LanguageCodeOption { Name = "Kikuyu", ID = "ki" };
        public static LanguageCodeOption Kinyarwanda_Rwanda = new LanguageCodeOption { Name = "Kinyarwanda (Rwanda)", ID = "rw" };
        public static LanguageCodeOption Kirundi = new LanguageCodeOption { Name = "Kirundi", ID = "rn" };
        public static LanguageCodeOption Kyrgyz = new LanguageCodeOption { Name = "Kyrgyz", ID = "ky" };
        public static LanguageCodeOption Komi = new LanguageCodeOption { Name = "Komi", ID = "kv" };
        public static LanguageCodeOption Kongo = new LanguageCodeOption { Name = "Kongo", ID = "kg" };
        public static LanguageCodeOption Korean = new LanguageCodeOption { Name = "Korean", ID = "ko" };
        public static LanguageCodeOption Kurdish = new LanguageCodeOption { Name = "Kurdish", ID = "ku" };
        public static LanguageCodeOption Kwanyama = new LanguageCodeOption { Name = "Kwanyama", ID = "kj" };
        public static LanguageCodeOption Lao = new LanguageCodeOption { Name = "Lao", ID = "lo" };
        public static LanguageCodeOption Latin = new LanguageCodeOption { Name = "Latin", ID = "la" };
        public static LanguageCodeOption Latvian_Lettish = new LanguageCodeOption { Name = "Latvian (Lettish)", ID = "lv" };
        public static LanguageCodeOption Limburgish_Limburger = new LanguageCodeOption { Name = "Limburgish ( Limburger)", ID = "li" };
        public static LanguageCodeOption Lingala = new LanguageCodeOption { Name = "Lingala", ID = "ln" };
        public static LanguageCodeOption Lithuanian = new LanguageCodeOption { Name = "Lithuanian", ID = "lt" };
        public static LanguageCodeOption Luga_Katanga = new LanguageCodeOption { Name = "Luga-Katanga", ID = "lu" };
        public static LanguageCodeOption Luganda_Ganda = new LanguageCodeOption { Name = "Luganda, Ganda", ID = "lg" };
        public static LanguageCodeOption Luxembourgish = new LanguageCodeOption { Name = "Luxembourgish", ID = "lb" };
        public static LanguageCodeOption Manx = new LanguageCodeOption { Name = "Manx", ID = "gv" };
        public static LanguageCodeOption Macedonian = new LanguageCodeOption { Name = "Macedonian", ID = "mk" };
        public static LanguageCodeOption Malagasy = new LanguageCodeOption { Name = "Malagasy", ID = "mg" };
        public static LanguageCodeOption Malay = new LanguageCodeOption { Name = "Malay", ID = "ms" };
        public static LanguageCodeOption Malayalam = new LanguageCodeOption { Name = "Malayalam", ID = "ml" };
        public static LanguageCodeOption Maltese = new LanguageCodeOption { Name = "Maltese", ID = "mt" };
        public static LanguageCodeOption Maori = new LanguageCodeOption { Name = "Maori", ID = "mi" };
        public static LanguageCodeOption Marathi = new LanguageCodeOption { Name = "Marathi", ID = "mr" };
        public static LanguageCodeOption Marshallese = new LanguageCodeOption { Name = "Marshallese", ID = "mh" };
        public static LanguageCodeOption Moldavian = new LanguageCodeOption { Name = "Moldavian", ID = "mo" };
        public static LanguageCodeOption Mongolian = new LanguageCodeOption { Name = "Mongolian", ID = "mn" };
        public static LanguageCodeOption Nauru = new LanguageCodeOption { Name = "Nauru", ID = "na" };
        public static LanguageCodeOption Navajo = new LanguageCodeOption { Name = "Navajo", ID = "nv" };
        public static LanguageCodeOption Ndonga = new LanguageCodeOption { Name = "Ndonga", ID = "ng" };
        public static LanguageCodeOption Northern_Ndebele = new LanguageCodeOption { Name = "Northern Ndebele", ID = "nd" };
        public static LanguageCodeOption Nepali = new LanguageCodeOption { Name = "Nepali", ID = "ne" };
        public static LanguageCodeOption Norwegian = new LanguageCodeOption { Name = "Norwegian", ID = "no" };
        public static LanguageCodeOption Norwegian_bokmal = new LanguageCodeOption { Name = "Norwegian bokmål", ID = "nb" };
        public static LanguageCodeOption Norwegian_nynorsk = new LanguageCodeOption { Name = "Norwegian nynorsk", ID = "nn" };
        public static LanguageCodeOption Nuosu = new LanguageCodeOption { Name = "Nuosu", ID = "ii" };
        public static LanguageCodeOption Occitan = new LanguageCodeOption { Name = "Occitan", ID = "oc" };
        public static LanguageCodeOption Ojibwe = new LanguageCodeOption { Name = "Ojibwe", ID = "oj" };
        public static LanguageCodeOption OldChurch_Slavonic_Or_OldBulgarian = new LanguageCodeOption { Name = "Old Church Slavonic, Old Bulgarian", ID = "cu" };
        public static LanguageCodeOption Oriya = new LanguageCodeOption { Name = "Oriya", ID = "or" };
        public static LanguageCodeOption Oromo_AfaanOromo = new LanguageCodeOption { Name = "Oromo (Afaan Oromo)", ID = "om" };
        public static LanguageCodeOption Ossetian = new LanguageCodeOption { Name = "Ossetian", ID = "os" };
        public static LanguageCodeOption Pāli = new LanguageCodeOption { Name = "Pāli", ID = "pi" };
        public static LanguageCodeOption Pashto, Pushto = new LanguageCodeOption { Name = "Pashto, Pushto", ID = "ps" };
        public static LanguageCodeOption Persian_Farsi = new LanguageCodeOption { Name = "Persian (Farsi)", ID = "fa" };
        public static LanguageCodeOption Polish = new LanguageCodeOption { Name = "Polish", ID = "pl" };
        public static LanguageCodeOption Portuguese = new LanguageCodeOption { Name = "Portuguese", ID = "pt" };
        public static LanguageCodeOption Punjabi_Eastern = new LanguageCodeOption { Name = "Punjabi (Eastern)", ID = "pa" };
        public static LanguageCodeOption Quechua = new LanguageCodeOption { Name = "Quechua", ID = "qu" };
        public static LanguageCodeOption Romansh = new LanguageCodeOption { Name = "Romansh", ID = "rm" };
        public static LanguageCodeOption Romanian = new LanguageCodeOption { Name = "Romanian", ID = "ro" };
        public static LanguageCodeOption Russian = new LanguageCodeOption { Name = "Russian", ID = "ru" };
        public static LanguageCodeOption Sami = new LanguageCodeOption { Name = "Sami", ID = "se" };
        public static LanguageCodeOption Samoan = new LanguageCodeOption { Name = "Samoan", ID = "sm" };
        public static LanguageCodeOption Sango = new LanguageCodeOption { Name = "Sango", ID = "sg" };
        public static LanguageCodeOption Sanskrit = new LanguageCodeOption { Name = "Sanskrit", ID = "sa" };
        public static LanguageCodeOption Serbian = new LanguageCodeOption { Name = "Serbian", ID = "sr" };
        public static LanguageCodeOption Serbo_Croatian = new LanguageCodeOption { Name = "Serbo-Croatian", ID = "sh" };
        public static LanguageCodeOption Sesotho = new LanguageCodeOption { Name = "Sesotho ", ID = "st" };
        public static LanguageCodeOption Setswana = new LanguageCodeOption { Name = "Setswana", ID = "tn" };
        public static LanguageCodeOption Shona = new LanguageCodeOption { Name = "Shona", ID = "sn" };
        public static LanguageCodeOption Sichuan_Yi = new LanguageCodeOption { Name = "Sichuan Yi", ID = "ii" };
        public static LanguageCodeOption Sindhi = new LanguageCodeOption { Name = "Sindhi", ID = "sd" };
        public static LanguageCodeOption Sinhalese = new LanguageCodeOption { Name = "Sinhalese", ID = "si" };
        public static LanguageCodeOption Siswati = new LanguageCodeOption { Name = "Siswati", ID = "ss" };
        public static LanguageCodeOption Slovak = new LanguageCodeOption { Name = "Slovak", ID = "sk" };
        public static LanguageCodeOption Slovenian = new LanguageCodeOption { Name = "Slovenian", ID = "sl" };
        public static LanguageCodeOption Somali = new LanguageCodeOption { Name = "Somali", ID = "so" };
        public static LanguageCodeOption Southern_Ndebele = new LanguageCodeOption { Name = "Southern Ndebele", ID = "nr" };
        public static LanguageCodeOption Spanish = new LanguageCodeOption { Name = "Spanish", ID = "es" };
        public static LanguageCodeOption Sundanese = new LanguageCodeOption { Name = "Sundanese", ID = "su" };
        public static LanguageCodeOption Swahili_Kiswahili = new LanguageCodeOption { Name = "Swahili (Kiswahili)", ID = "sw" };
        public static LanguageCodeOption Swati = new LanguageCodeOption { Name = "Swati", ID = "ss" };
        public static LanguageCodeOption Swedish = new LanguageCodeOption { Name = "Swedish", ID = "sv" };
        public static LanguageCodeOption Tagalog = new LanguageCodeOption { Name = "Tagalog", ID = "tl" };
        public static LanguageCodeOption Tahitian = new LanguageCodeOption { Name = "Tahitian", ID = "ty" };
        public static LanguageCodeOption Tajik = new LanguageCodeOption { Name = "Tajik", ID = "tg" };
        public static LanguageCodeOption Tamil = new LanguageCodeOption { Name = "Tamil", ID = "ta" };
        public static LanguageCodeOption Tatar = new LanguageCodeOption { Name = "Tatar", ID = "tt" };
        public static LanguageCodeOption Telugu = new LanguageCodeOption { Name = "Telugu", ID = "te" };
        public static LanguageCodeOption Thai = new LanguageCodeOption { Name = "Thai", ID = "th" };
        public static LanguageCodeOption Tibetan = new LanguageCodeOption { Name = "Tibetan", ID = "bo" };
        public static LanguageCodeOption Tigrinya = new LanguageCodeOption { Name = "Tigrinya", ID = "ti" };
        public static LanguageCodeOption Tonga = new LanguageCodeOption { Name = "Tonga", ID = "to" };
        public static LanguageCodeOption Tsonga = new LanguageCodeOption { Name = "Tsonga", ID = "ts" };
        public static LanguageCodeOption Turkish = new LanguageCodeOption { Name = "Turkish", ID = "tr" };
        public static LanguageCodeOption Turkmen = new LanguageCodeOption { Name = "Turkmen", ID = "tk" };
        public static LanguageCodeOption Twi = new LanguageCodeOption { Name = "Twi", ID = "tw" };
        public static LanguageCodeOption Uyghur = new LanguageCodeOption { Name = "Uyghur", ID = "ug" };
        public static LanguageCodeOption Ukrainian = new LanguageCodeOption { Name = "Ukrainian", ID = "uk" };
        public static LanguageCodeOption Urdu = new LanguageCodeOption { Name = "Urdu", ID = "ur" };
        public static LanguageCodeOption Uzbek = new LanguageCodeOption { Name = "Uzbek", ID = "uz" };
        public static LanguageCodeOption Venda = new LanguageCodeOption { Name = "Venda", ID = "ve" };
        public static LanguageCodeOption Vietnamese = new LanguageCodeOption { Name = "Vietnamese", ID = "vi" };
        public static LanguageCodeOption Volapük = new LanguageCodeOption { Name = "Volapük", ID = "vo" };
        public static LanguageCodeOption Wallon = new LanguageCodeOption { Name = "Wallon", ID = "wa" };
        public static LanguageCodeOption Welsh = new LanguageCodeOption { Name = "Welsh", ID = "cy" };
        public static LanguageCodeOption Wolof = new LanguageCodeOption { Name = "Wolof", ID = "wo" };
        public static LanguageCodeOption Western_Frisian = new LanguageCodeOption { Name = "Western Frisian", ID = "fy" };
        public static LanguageCodeOption Xhosa = new LanguageCodeOption { Name = "Xhosa", ID = "xh" };
        public static LanguageCodeOption Yiddish = new LanguageCodeOption { Name = "Yiddish", ID = "yi" }; //yi, ji
        public static LanguageCodeOption Yoruba = new LanguageCodeOption { Name = "Yoruba", ID = "yo" };
        public static LanguageCodeOption Zhuang, Chuang = new LanguageCodeOption { Name = "Zhuang, Chuang", ID = "za" };
        public static LanguageCodeOption Zulu = new LanguageCodeOption { Name = "Zulu", ID = "zu" };
    }
    public class LanguageCodeOption
    {
        public string Name { get; set; }
        public string ID { get; set; }
    }
    // function ****************************************************************************
    public class LanguagePage
    {
        public static string GetLanguageCode
        {
            get
            {
                Helper.Language.LanguageCodeOption languageCodeOption = Helper.Security.Cookies.GetCookiForLanguage();
                if (languageCodeOption != null)
                    return languageCodeOption.ID;
                // set default
                return string.Empty;
            }
        } 
    }

}