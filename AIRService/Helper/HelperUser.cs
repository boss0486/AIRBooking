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

namespace Helper.User
{
    public class InFormation
    {
        public static string GetFullName(string id)
        {
            UserInfoService userInfoService = new UserInfoService();
            string fullName = userInfoService.GetFullName(id);
            if (!string.IsNullOrWhiteSpace(fullName))
                return fullName;
            ///
            CMSUserInfoService cMSUserInfoService = new CMSUserInfoService();
            fullName = cMSUserInfoService.GetFullName(id);
            if (!string.IsNullOrWhiteSpace(fullName))
                return fullName;
            //
            return string.Empty;
        }
        public static string GetInfCreateBy(string id)
        {
            UserInfoService userInfoService = new UserInfoService();
            string fullName = userInfoService.GetFullName(id);
            if (!string.IsNullOrWhiteSpace(fullName))
                return fullName;
            ///
            CMSUserInfoService cMSUserInfoService = new CMSUserInfoService();
            fullName = cMSUserInfoService.GetFullName(id);
            if (!string.IsNullOrWhiteSpace(fullName))
                return "*:" + fullName;
            //
            return string.Empty;
        }
    }
    public class Access
    {

        public static bool IsLogin()
        {
            try
            {
                UserService userService = new UserService();
                return userService.IsLogin();
            }
            catch
            {
                return false;
            }
        }
        // static function ()

    }

    //
}