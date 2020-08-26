using CMSCore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using WebCore.Entities;
using CMSCore.Entities;
using WebCore.Services;

namespace Helper
{
    public class Current : IHttpHandler, IRequiresSessionState
    {
        public static string LanguageID
        {
            get
            {
                using (var userLoginService = new UserLoginService())
                    return userLoginService.GetLanguageID;
            }
        }

        public static bool IsCMSUser
        {
            get
            {
                try
                {
                    using (var userLoginService = new UserLoginService())
                    {
                        string sqlQuerry = @"SELECT TOP 1 * FROM View_Login  WHERE LoginID = @LoginID";
                        LoginModel loginModel = userLoginService.Query<LoginModel>(sqlQuerry, new { Current.LoginID }).FirstOrDefault();
                        return loginModel.IsCMSUser;
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }


        public static LoginModel LoginUser
        {
            get
            {
                try
                {
                    LoginModel loginModel = new LoginModel();
                    using (var userLoginService = new UserLoginService())
                    {
                        string sqlQuerry = @"SELECT TOP 1 * FROM View_Login  WHERE LoginID  = @LoginID";
                        loginModel = userLoginService.Query<LoginModel>(sqlQuerry, new { Current.IdentifierID }).FirstOrDefault();
                    }
                    return loginModel;
                }
                catch (Exception)
                {
                    return new LoginModel();
                }
            }
        }
        //public static LoginModel LoginInFor(System.Data.IDbTransaction transaction)
        //{
        //    try
        //    {
        //        LoginModel loginModel = new LoginModel();
        //        using (var userLoginService = new UserLoginService())
        //        {
        //            string sqlQuerry = @"SELECT TOP 1 [ID] ,[LoginID] ,[PassID] ,[TokenID] ,[OTPCode] ,[SkipAuthorization] ,[ImageFile]
        //                              FROM View_CMSLogin   WHERE LOWER(ID) = @ID
        //	  UNION ALL SELECT TOP 1 [ID] ,[LoginID] ,[PassID] ,[TokenID] ,[OTPCode] ,[SkipAuthorization] ,[ImageFile]
        //                              FROM View_Login  WHERE LOWER(ID)  = @ID";
        //            loginModel = userLoginService.Query<LoginModel>(sqlQuerry, new { ID = Current.LoginID }).FirstOrDefault();
        //        }
        //        return loginModel;
        //    }
        //    catch (Exception)
        //    {
        //        HttpContext.Current.Response.Redirect("/");
        //        return new LoginModel();
        //    }
        //}
        // // 
        public static bool LoginState()
        {
            try
            {
                HttpCookie authCookie = HttpContext.Current.Request.Cookies[System.Web.Security.FormsAuthentication.FormsCookieName];
                if (authCookie !=null)
                    return true;
                return false;
            }
            catch
            {
                return false;
            }
        }
        public static string LoginState_test()
        {
            try
            {
                HttpCookie authCookie = HttpContext.Current.Request.Cookies[System.Web.Security.FormsAuthentication.FormsCookieName];
                if (authCookie != null)
                    return "111";
                return "9999";
            }
            catch
            {
                return "111";
            }
        }

        public static LoginInForModel LoginInFor
        {
            get
            {

                if (Current.IsCMSUser)
                {
                    using (var cMSUserLoginService = new CMSUserLoginService())
                    {
                        string sqlQuerry = @"SELECT TOP (1) * FROM View_CMSUserInformation WHERE UserID = 'ffc79712-cbc1-4fb8-abd7-939ed10c4e37'";
                        var loginModel = cMSUserLoginService.Query<LoginInForModel>(sqlQuerry, new { ID = Current.IdentifierID }).FirstOrDefault();
                        return loginModel;
                    }
                }
                else
                {
                    using (var userLoginService = new UserLoginService())
                    {
                        string sqlQuerry = @"SELECT TOP (1) * FROM View_UserInformation WHERE UserID = @ID ";
                        var loginModel = userLoginService.Query<LoginInForModel>(sqlQuerry, new { ID = Current.IdentifierID }).FirstOrDefault();
                        return loginModel;
                    }
                }
            }
        }
        public static string IdentifierID
        {
            get
            {
                try
                {
                    string loginId = HttpContext.Current.Session["identifyId"].ToString();
                    if (!string.IsNullOrWhiteSpace(loginId))
                        return loginId.ToLower();
                    return string.Empty;
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
        }
        public static string LoginID
        {
            get
            {
                try
                {
                    string identifyId = HttpContext.Current.Session["LoginID"].ToString();
                    if (!string.IsNullOrWhiteSpace(identifyId))
                        return identifyId.ToLower();
                    return string.Empty;
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
        }

        public static bool CheckSite
        {
            get
            {
                //    Current current = new Current();
                //    string host = current.HttpContext.Current.Request.Url.Host;
                //    string port = current.HttpContext.Current.Request.Url.Port.ToString();
                //    if (string.IsNullOrEmpty(host))
                //        return false;
                //    //
                //    using (var cmsSiteservice = new CMSSiteService())
                //    {
                //        var site = cmsSiteservice.GetAlls(m => m.Domain.ToLower().Equals(host.ToLower()) || m.Domain.ToLower().Equals("wwww." + host.ToLower())).FirstOrDefault();
                //        if (site != null)
                //            return false;
                //        if (site.SiteID.ToLower().Equals(SiteID.ToLower()))
                //            return true;
                //    }
                //    //  getsite fordomain
                return false;
            }

        }
        public static DateTime Now => DateTime.Now;

        public bool IsReusable => throw new NotImplementedException();

        public void ProcessRequest(HttpContext context)
        {
            throw new NotImplementedException();
        }
    }
}
