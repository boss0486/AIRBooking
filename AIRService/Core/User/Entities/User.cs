using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCore.Entities
{
    public class Logged
    {
        public string ID { get; set; }
        public string LoginID { get; set; }
        public string Password { get; set; }
        public bool IsBlock { get; set; }
        public int Enabled { get; set; }
        public bool IsCMSUser { get; set; }
        public bool IsAdministrator { get; set; }
    }
    public class CookiModel
    {
        public string LoginID { get; set; }
        public string Password { get; set; }
        public bool IsRemember { get; set; }

    }
    public class LoginInForModel

    {
        public string ID { get; set; }
        public string UserID { get; set; }
        public string ImageFile { get; set; }
        public string Birthday { get; set; }
        public string NickName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
    }
    public class LoginQRModel
    {
        public string ID { get; set; }
        public string LoginID { get; set; }
        public string PinCode { get; set; }
        public string TokenID { get; set; }
        public string OTPCode { get; set; }
        public int Enabled { get; set; }
        public bool IsBlock { get; set; }
        public bool IsAdmintrator { get; set; }
    }
    public class LoginReqestModel
    {
        public string LoginID { get; set; }
        public string Password { get; set; }
        public bool IsRemember { get; set; }
        public string Url { get; set; }
    }
    public class LoginQRReqestModel
    {
        public string LoginID { get; set; }
        public string PinCode { get; set; }
        public bool IsRemember { get; set; }
        public string Url { get; set; }
    }

    public class ClientLogged
    {
        public bool IsClient { get; set; }
        public int ClientType { get; set; }
    }
}
