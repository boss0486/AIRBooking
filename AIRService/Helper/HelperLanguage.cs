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
        public static LanguageCodeOption Abkhazian = new LanguageCodeOption { Title = "Abkhazian", ID = "ab" };
        public static LanguageCodeOption Afrikaans = new LanguageCodeOption { Title = "Afrikaans", ID = "af" };
        public static LanguageCodeOption Akan = new LanguageCodeOption { Title = "Akan", ID = "ak" };
        public static LanguageCodeOption Albanian = new LanguageCodeOption { Title = "Albanian", ID = "sq" };
        public static LanguageCodeOption Amharic = new LanguageCodeOption { Title = "Amharic", ID = "am" };
        public static LanguageCodeOption Arabic = new LanguageCodeOption { Title = "Arabic", ID = "ar" };
        public static LanguageCodeOption Aragonese = new LanguageCodeOption { Title = "Aragonese", ID = "an" };
        public static LanguageCodeOption Armenian = new LanguageCodeOption { Title = "Armenian", ID = "hy" };
        public static LanguageCodeOption Assamese = new LanguageCodeOption { Title = "Assamese", ID = "as" };
        public static LanguageCodeOption Avaric = new LanguageCodeOption { Title = "Avaric", ID = "av" };
        public static LanguageCodeOption Avestan = new LanguageCodeOption { Title = "Avestan", ID = "ae" };
        public static LanguageCodeOption Aymara = new LanguageCodeOption { Title = "Aymara", ID = "ay" };
        public static LanguageCodeOption Azerbaijani = new LanguageCodeOption { Title = "Azerbaijani", ID = "az" };
        public static LanguageCodeOption Bambara = new LanguageCodeOption { Title = "Bambara", ID = "bm" };
        public static LanguageCodeOption Bashkir = new LanguageCodeOption { Title = "Bashkir", ID = "ba" };
        public static LanguageCodeOption Basque = new LanguageCodeOption { Title = "Basque", ID = "eu" };
        public static LanguageCodeOption Belarusian = new LanguageCodeOption { Title = "Belarusian", ID = "be" };
        public static LanguageCodeOption Bengali = new LanguageCodeOption { Title = "Bengali (Bangla)", ID = "bn" };//(Bangla)
        public static LanguageCodeOption Bihari = new LanguageCodeOption { Title = "Bihari", ID = "bh" };
        public static LanguageCodeOption Bislama = new LanguageCodeOption { Title = "Bislama", ID = "bi" };
        public static LanguageCodeOption Bosnian = new LanguageCodeOption { Title = "Bosnian", ID = "bs" };
        public static LanguageCodeOption Breton = new LanguageCodeOption { Title = "Breton", ID = "br" };
        public static LanguageCodeOption Bulgarian = new LanguageCodeOption { Title = "Bulgarian", ID = "bg" };
        public static LanguageCodeOption Burmese = new LanguageCodeOption { Title = "Burmese", ID = "my" };
        public static LanguageCodeOption Catalan = new LanguageCodeOption { Title = "Catalan", ID = "ca" };
        public static LanguageCodeOption Chamorro = new LanguageCodeOption { Title = "Chamorro", ID = "ch" };
        public static LanguageCodeOption Chechen = new LanguageCodeOption { Title = "Chechen", ID = "ce" };
        public static LanguageCodeOption Chichewa_Chewa_Nyanja = new LanguageCodeOption { Title = "Chichewa, Chewa, Nyanja", ID = "ny" };
        public static LanguageCodeOption Chinese = new LanguageCodeOption { Title = "Chinese", ID = "zh" };
        public static LanguageCodeOption Chinese_Simplified = new LanguageCodeOption { Title = "Chinese (Simplified)", ID = "zh-Hans" };//Simplified
        public static LanguageCodeOption Chinese_Traditional = new LanguageCodeOption { Title = "Chinese (Traditional)", ID = "zh-Hant" };//Traditional
        public static LanguageCodeOption Chuvash = new LanguageCodeOption { Title = "Chuvash", ID = "cv" };
        public static LanguageCodeOption Cornish = new LanguageCodeOption { Title = "Cornish", ID = "kw" };
        public static LanguageCodeOption Corsican = new LanguageCodeOption { Title = "Corsican", ID = "co" };
        public static LanguageCodeOption Cree = new LanguageCodeOption { Title = "Cree", ID = "cr" };
        public static LanguageCodeOption Croatian = new LanguageCodeOption { Title = "Croatian", ID = "hr" };
        public static LanguageCodeOption Czech = new LanguageCodeOption { Title = "Czech", ID = "cs" };
        public static LanguageCodeOption Danish = new LanguageCodeOption { Title = "Danish", ID = "da" };
        public static LanguageCodeOption Divehi, Dhivehi, Maldivian = new LanguageCodeOption { Title = "Divehi, Dhivehi, Maldivian", ID = "dv" };
        public static LanguageCodeOption Dutch = new LanguageCodeOption { Title = "Dutch", ID = "nl" };
        public static LanguageCodeOption Dzongkha = new LanguageCodeOption { Title = "Dzongkha", ID = "dz" };
        public static LanguageCodeOption English = new LanguageCodeOption { Title = "English", ID = "en" };
        public static LanguageCodeOption Esperanto = new LanguageCodeOption { Title = "Esperanto", ID = "eo" };
        public static LanguageCodeOption Estonian = new LanguageCodeOption { Title = "Estonian", ID = "et" };
        public static LanguageCodeOption Ewe = new LanguageCodeOption { Title = "Ewe", ID = "ee" };
        public static LanguageCodeOption Faroese = new LanguageCodeOption { Title = "Faroese", ID = "fo" };
        public static LanguageCodeOption Fijian = new LanguageCodeOption { Title = "Fijian", ID = "fj" };
        public static LanguageCodeOption Finnish = new LanguageCodeOption { Title = "Finnish", ID = "fi" };
        public static LanguageCodeOption French = new LanguageCodeOption { Title = "French", ID = "fr" };
        public static LanguageCodeOption Fula, Fulah, Pulaar, Pular = new LanguageCodeOption { Title = "Fula, Fulah, Pulaar, Pular", ID = "ff" };
        public static LanguageCodeOption Galician = new LanguageCodeOption { Title = "Galician", ID = "gl" };
        public static LanguageCodeOption Gaelic_Scottish = new LanguageCodeOption { Title = "Gaelic (Scottish)", ID = "gd" };
        public static LanguageCodeOption Gaelic_Manx = new LanguageCodeOption { Title = "Gaelic (Manx)", ID = "gv" };
        public static LanguageCodeOption Georgian = new LanguageCodeOption { Title = "Georgian", ID = "ka" };
        public static LanguageCodeOption German = new LanguageCodeOption { Title = "German", ID = "de" };
        public static LanguageCodeOption Greek = new LanguageCodeOption { Title = "Greek", ID = "el" };
        public static LanguageCodeOption Greenlandic = new LanguageCodeOption { Title = "Greenlandic", ID = "kl" };
        public static LanguageCodeOption Guarani = new LanguageCodeOption { Title = "Guarani", ID = "gn" };
        public static LanguageCodeOption Gujarati = new LanguageCodeOption { Title = "Gujarati", ID = "gu" };
        public static LanguageCodeOption Haitian_Creole = new LanguageCodeOption { Title = "Haitian Creole", ID = "ht" };
        public static LanguageCodeOption Hausa = new LanguageCodeOption { Title = "Hausa", ID = "ha" };
        public static LanguageCodeOption Hebrew = new LanguageCodeOption { Title = "Hebrew", ID = "he" };
        public static LanguageCodeOption Herero = new LanguageCodeOption { Title = "Herero", ID = "hz" };
        public static LanguageCodeOption Hindi = new LanguageCodeOption { Title = "Hindi", ID = "hi" };
        public static LanguageCodeOption Hiri_Motu = new LanguageCodeOption { Title = "Hiri Motu", ID = "ho" };
        public static LanguageCodeOption Hungarian = new LanguageCodeOption { Title = "Hungarian", ID = "hu" };
        public static LanguageCodeOption Icelandic = new LanguageCodeOption { Title = "Icelandic", ID = "is" };
        public static LanguageCodeOption Ido = new LanguageCodeOption { Title = "Ido", ID = "io" };
        public static LanguageCodeOption Igbo = new LanguageCodeOption { Title = "Igbo", ID = "ig" };
        public static LanguageCodeOption Indonesian = new LanguageCodeOption { Title = "Indonesian", ID = "id" }; //id, in
        public static LanguageCodeOption Interlingua = new LanguageCodeOption { Title = "Interlingua", ID = "ia" };
        public static LanguageCodeOption Interlingue = new LanguageCodeOption { Title = "Interlingue", ID = "ie" };
        public static LanguageCodeOption Inuktitut = new LanguageCodeOption { Title = "Inuktitut", ID = "iu" };
        public static LanguageCodeOption Inupiak = new LanguageCodeOption { Title = "Inupiak", ID = "ik" };
        public static LanguageCodeOption Irish = new LanguageCodeOption { Title = "Irish", ID = "ga" };
        public static LanguageCodeOption Italian = new LanguageCodeOption { Title = "Italian", ID = "it" };
        public static LanguageCodeOption Japanese = new LanguageCodeOption { Title = "Japanese", ID = "ja" };
        public static LanguageCodeOption Javanese = new LanguageCodeOption { Title = "Javanese", ID = "jv" };
        public static LanguageCodeOption Kalaallisut_Greenlandic = new LanguageCodeOption { Title = "Kalaallisut, Greenlandic", ID = "kl" };
        public static LanguageCodeOption Kannada = new LanguageCodeOption { Title = "Kannada", ID = "kn" };
        public static LanguageCodeOption Kanuri = new LanguageCodeOption { Title = "Kanuri", ID = "kr" };
        public static LanguageCodeOption Kashmiri = new LanguageCodeOption { Title = "Kashmiri", ID = "ks" };
        public static LanguageCodeOption Kazakh = new LanguageCodeOption { Title = "Kazakh", ID = "kk" };
        public static LanguageCodeOption Khmer = new LanguageCodeOption { Title = "Khmer", ID = "km" };
        public static LanguageCodeOption Kikuyu = new LanguageCodeOption { Title = "Kikuyu", ID = "ki" };
        public static LanguageCodeOption Kinyarwanda_Rwanda = new LanguageCodeOption { Title = "Kinyarwanda (Rwanda)", ID = "rw" };
        public static LanguageCodeOption Kirundi = new LanguageCodeOption { Title = "Kirundi", ID = "rn" };
        public static LanguageCodeOption Kyrgyz = new LanguageCodeOption { Title = "Kyrgyz", ID = "ky" };
        public static LanguageCodeOption Komi = new LanguageCodeOption { Title = "Komi", ID = "kv" };
        public static LanguageCodeOption Kongo = new LanguageCodeOption { Title = "Kongo", ID = "kg" };
        public static LanguageCodeOption Korean = new LanguageCodeOption { Title = "Korean", ID = "ko" };
        public static LanguageCodeOption Kurdish = new LanguageCodeOption { Title = "Kurdish", ID = "ku" };
        public static LanguageCodeOption Kwanyama = new LanguageCodeOption { Title = "Kwanyama", ID = "kj" };
        public static LanguageCodeOption Lao = new LanguageCodeOption { Title = "Lao", ID = "lo" };
        public static LanguageCodeOption Latin = new LanguageCodeOption { Title = "Latin", ID = "la" };
        public static LanguageCodeOption Latvian_Lettish = new LanguageCodeOption { Title = "Latvian (Lettish)", ID = "lv" };
        public static LanguageCodeOption Limburgish_Limburger = new LanguageCodeOption { Title = "Limburgish ( Limburger)", ID = "li" };
        public static LanguageCodeOption Lingala = new LanguageCodeOption { Title = "Lingala", ID = "ln" };
        public static LanguageCodeOption Lithuanian = new LanguageCodeOption { Title = "Lithuanian", ID = "lt" };
        public static LanguageCodeOption Luga_Katanga = new LanguageCodeOption { Title = "Luga-Katanga", ID = "lu" };
        public static LanguageCodeOption Luganda_Ganda = new LanguageCodeOption { Title = "Luganda, Ganda", ID = "lg" };
        public static LanguageCodeOption Luxembourgish = new LanguageCodeOption { Title = "Luxembourgish", ID = "lb" };
        public static LanguageCodeOption Manx = new LanguageCodeOption { Title = "Manx", ID = "gv" };
        public static LanguageCodeOption Macedonian = new LanguageCodeOption { Title = "Macedonian", ID = "mk" };
        public static LanguageCodeOption Malagasy = new LanguageCodeOption { Title = "Malagasy", ID = "mg" };
        public static LanguageCodeOption Malay = new LanguageCodeOption { Title = "Malay", ID = "ms" };
        public static LanguageCodeOption Malayalam = new LanguageCodeOption { Title = "Malayalam", ID = "ml" };
        public static LanguageCodeOption Maltese = new LanguageCodeOption { Title = "Maltese", ID = "mt" };
        public static LanguageCodeOption Maori = new LanguageCodeOption { Title = "Maori", ID = "mi" };
        public static LanguageCodeOption Marathi = new LanguageCodeOption { Title = "Marathi", ID = "mr" };
        public static LanguageCodeOption Marshallese = new LanguageCodeOption { Title = "Marshallese", ID = "mh" };
        public static LanguageCodeOption Moldavian = new LanguageCodeOption { Title = "Moldavian", ID = "mo" };
        public static LanguageCodeOption Mongolian = new LanguageCodeOption { Title = "Mongolian", ID = "mn" };
        public static LanguageCodeOption Nauru = new LanguageCodeOption { Title = "Nauru", ID = "na" };
        public static LanguageCodeOption Navajo = new LanguageCodeOption { Title = "Navajo", ID = "nv" };
        public static LanguageCodeOption Ndonga = new LanguageCodeOption { Title = "Ndonga", ID = "ng" };
        public static LanguageCodeOption Northern_Ndebele = new LanguageCodeOption { Title = "Northern Ndebele", ID = "nd" };
        public static LanguageCodeOption Nepali = new LanguageCodeOption { Title = "Nepali", ID = "ne" };
        public static LanguageCodeOption Norwegian = new LanguageCodeOption { Title = "Norwegian", ID = "no" };
        public static LanguageCodeOption Norwegian_bokmal = new LanguageCodeOption { Title = "Norwegian bokmål", ID = "nb" };
        public static LanguageCodeOption Norwegian_nynorsk = new LanguageCodeOption { Title = "Norwegian nynorsk", ID = "nn" };
        public static LanguageCodeOption Nuosu = new LanguageCodeOption { Title = "Nuosu", ID = "ii" };
        public static LanguageCodeOption Occitan = new LanguageCodeOption { Title = "Occitan", ID = "oc" };
        public static LanguageCodeOption Ojibwe = new LanguageCodeOption { Title = "Ojibwe", ID = "oj" };
        public static LanguageCodeOption OldChurch_Slavonic_Or_OldBulgarian = new LanguageCodeOption { Title = "Old Church Slavonic, Old Bulgarian", ID = "cu" };
        public static LanguageCodeOption Oriya = new LanguageCodeOption { Title = "Oriya", ID = "or" };
        public static LanguageCodeOption Oromo_AfaanOromo = new LanguageCodeOption { Title = "Oromo (Afaan Oromo)", ID = "om" };
        public static LanguageCodeOption Ossetian = new LanguageCodeOption { Title = "Ossetian", ID = "os" };
        public static LanguageCodeOption Pāli = new LanguageCodeOption { Title = "Pāli", ID = "pi" };
        public static LanguageCodeOption Pashto, Pushto = new LanguageCodeOption { Title = "Pashto, Pushto", ID = "ps" };
        public static LanguageCodeOption Persian_Farsi = new LanguageCodeOption { Title = "Persian (Farsi)", ID = "fa" };
        public static LanguageCodeOption Polish = new LanguageCodeOption { Title = "Polish", ID = "pl" };
        public static LanguageCodeOption Portuguese = new LanguageCodeOption { Title = "Portuguese", ID = "pt" };
        public static LanguageCodeOption Punjabi_Eastern = new LanguageCodeOption { Title = "Punjabi (Eastern)", ID = "pa" };
        public static LanguageCodeOption Quechua = new LanguageCodeOption { Title = "Quechua", ID = "qu" };
        public static LanguageCodeOption Romansh = new LanguageCodeOption { Title = "Romansh", ID = "rm" };
        public static LanguageCodeOption Romanian = new LanguageCodeOption { Title = "Romanian", ID = "ro" };
        public static LanguageCodeOption Russian = new LanguageCodeOption { Title = "Russian", ID = "ru" };
        public static LanguageCodeOption Sami = new LanguageCodeOption { Title = "Sami", ID = "se" };
        public static LanguageCodeOption Samoan = new LanguageCodeOption { Title = "Samoan", ID = "sm" };
        public static LanguageCodeOption Sango = new LanguageCodeOption { Title = "Sango", ID = "sg" };
        public static LanguageCodeOption Sanskrit = new LanguageCodeOption { Title = "Sanskrit", ID = "sa" };
        public static LanguageCodeOption Serbian = new LanguageCodeOption { Title = "Serbian", ID = "sr" };
        public static LanguageCodeOption Serbo_Croatian = new LanguageCodeOption { Title = "Serbo-Croatian", ID = "sh" };
        public static LanguageCodeOption Sesotho = new LanguageCodeOption { Title = "Sesotho ", ID = "st" };
        public static LanguageCodeOption Setswana = new LanguageCodeOption { Title = "Setswana", ID = "tn" };
        public static LanguageCodeOption Shona = new LanguageCodeOption { Title = "Shona", ID = "sn" };
        public static LanguageCodeOption Sichuan_Yi = new LanguageCodeOption { Title = "Sichuan Yi", ID = "ii" };
        public static LanguageCodeOption Sindhi = new LanguageCodeOption { Title = "Sindhi", ID = "sd" };
        public static LanguageCodeOption Sinhalese = new LanguageCodeOption { Title = "Sinhalese", ID = "si" };
        public static LanguageCodeOption Siswati = new LanguageCodeOption { Title = "Siswati", ID = "ss" };
        public static LanguageCodeOption Slovak = new LanguageCodeOption { Title = "Slovak", ID = "sk" };
        public static LanguageCodeOption Slovenian = new LanguageCodeOption { Title = "Slovenian", ID = "sl" };
        public static LanguageCodeOption Somali = new LanguageCodeOption { Title = "Somali", ID = "so" };
        public static LanguageCodeOption Southern_Ndebele = new LanguageCodeOption { Title = "Southern Ndebele", ID = "nr" };
        public static LanguageCodeOption Spanish = new LanguageCodeOption { Title = "Spanish", ID = "es" };
        public static LanguageCodeOption Sundanese = new LanguageCodeOption { Title = "Sundanese", ID = "su" };
        public static LanguageCodeOption Swahili_Kiswahili = new LanguageCodeOption { Title = "Swahili (Kiswahili)", ID = "sw" };
        public static LanguageCodeOption Swati = new LanguageCodeOption { Title = "Swati", ID = "ss" };
        public static LanguageCodeOption Swedish = new LanguageCodeOption { Title = "Swedish", ID = "sv" };
        public static LanguageCodeOption Tagalog = new LanguageCodeOption { Title = "Tagalog", ID = "tl" };
        public static LanguageCodeOption Tahitian = new LanguageCodeOption { Title = "Tahitian", ID = "ty" };
        public static LanguageCodeOption Tajik = new LanguageCodeOption { Title = "Tajik", ID = "tg" };
        public static LanguageCodeOption Tamil = new LanguageCodeOption { Title = "Tamil", ID = "ta" };
        public static LanguageCodeOption Tatar = new LanguageCodeOption { Title = "Tatar", ID = "tt" };
        public static LanguageCodeOption Telugu = new LanguageCodeOption { Title = "Telugu", ID = "te" };
        public static LanguageCodeOption Thai = new LanguageCodeOption { Title = "Thai", ID = "th" };
        public static LanguageCodeOption Tibetan = new LanguageCodeOption { Title = "Tibetan", ID = "bo" };
        public static LanguageCodeOption Tigrinya = new LanguageCodeOption { Title = "Tigrinya", ID = "ti" };
        public static LanguageCodeOption Tonga = new LanguageCodeOption { Title = "Tonga", ID = "to" };
        public static LanguageCodeOption Tsonga = new LanguageCodeOption { Title = "Tsonga", ID = "ts" };
        public static LanguageCodeOption Turkish = new LanguageCodeOption { Title = "Turkish", ID = "tr" };
        public static LanguageCodeOption Turkmen = new LanguageCodeOption { Title = "Turkmen", ID = "tk" };
        public static LanguageCodeOption Twi = new LanguageCodeOption { Title = "Twi", ID = "tw" };
        public static LanguageCodeOption Uyghur = new LanguageCodeOption { Title = "Uyghur", ID = "ug" };
        public static LanguageCodeOption Ukrainian = new LanguageCodeOption { Title = "Ukrainian", ID = "uk" };
        public static LanguageCodeOption Urdu = new LanguageCodeOption { Title = "Urdu", ID = "ur" };
        public static LanguageCodeOption Uzbek = new LanguageCodeOption { Title = "Uzbek", ID = "uz" };
        public static LanguageCodeOption Venda = new LanguageCodeOption { Title = "Venda", ID = "ve" };
        public static LanguageCodeOption Vietnamese = new LanguageCodeOption { Title = "Vietnamese", ID = "vi" };
        public static LanguageCodeOption Volapük = new LanguageCodeOption { Title = "Volapük", ID = "vo" };
        public static LanguageCodeOption Wallon = new LanguageCodeOption { Title = "Wallon", ID = "wa" };
        public static LanguageCodeOption Welsh = new LanguageCodeOption { Title = "Welsh", ID = "cy" };
        public static LanguageCodeOption Wolof = new LanguageCodeOption { Title = "Wolof", ID = "wo" };
        public static LanguageCodeOption Western_Frisian = new LanguageCodeOption { Title = "Western Frisian", ID = "fy" };
        public static LanguageCodeOption Xhosa = new LanguageCodeOption { Title = "Xhosa", ID = "xh" };
        public static LanguageCodeOption Yiddish = new LanguageCodeOption { Title = "Yiddish", ID = "yi" }; //yi, ji
        public static LanguageCodeOption Yoruba = new LanguageCodeOption { Title = "Yoruba", ID = "yo" };
        public static LanguageCodeOption Zhuang, Chuang = new LanguageCodeOption { Title = "Zhuang, Chuang", ID = "za" };
        public static LanguageCodeOption Zulu = new LanguageCodeOption { Title = "Zulu", ID = "zu" };

    }



    public class LanguageCodeOption
    {


        public string Title { get; set; }
        public string ID { get; set; }
    }
}