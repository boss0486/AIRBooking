using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using System.Web.Security;
using WebCore.Core;
using WebCore.Entities;
using WebCore.Services;
using Helper.User;

namespace Helper.Current
{
    public class UserLogin : IHttpHandler, IRequiresSessionState
    {
        public static string LanguageID
        {
            get
            {
                UserService service = new UserService();
                return service.GetLanguageID;
            }
        }

        public static int UserRoleInApplication
        {
            get
            {
                try
                {
                    if (Helper.Current.UserLogin.IsCMSUser)
                        return 1;
                    if (Helper.Current.UserLogin.IsAdminInApplication)
                        return 2;
                    //
                    if (Helper.Current.UserLogin.IsAdminAgentLogged())
                        return 3;
                    //
                    if (Helper.Current.UserLogin.IsCustomerLogged())
                        return 4;
                    //
                    return 0;
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }
        public static bool IsCMSUser
        {
            get
            {
                try
                {
                    string loggedId = UserLogin.LoginID;
                    var service = new UserService();
                    var logged = service.LoggedModel();
                    if (logged == null)
                        return false;
                    //
                    return logged.IsCMSUser;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
        public static bool IsAdminInApplication
        {
            get
            {
                var service = new UserService();
                var logged = service.LoggedModel();
                if (logged != null)
                    return logged.IsAdministrator;
                //
                return false;
            }
        }

        public static bool IsClientInApplication()
        {
            try
            {
                string userId = Helper.Current.UserLogin.IdentifierID;
                var service = new UserService();
                return service.IsClientInApplication(userId);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IsAdminAgentLogged()
        {
            try
            {
                string userId = Helper.Current.UserLogin.IdentifierID;
                var service = new UserService();
                return service.IsAdminCustomerLogged(userId);
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static bool IsCustomerLogged()
        {
            try
            {
                string userId = Helper.Current.UserLogin.IdentifierID;
                var service = new UserService();
                return service.IsCustomerLogged(userId);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IsAdminSupplierLogged()
        {
            try
            {
                string userId = Helper.Current.UserLogin.IdentifierID;
                var service = new UserService();
                return service.IsAdminSupplierLogged(userId);
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static bool IsSupplierLogged()
        {
            try
            {
                string userId = Helper.Current.UserLogin.IdentifierID;
                var service = new UserService();
                return service.IsSupplierLogged(userId, dbConnection: service._connection);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static Logged LoggedModel
        {
            get
            {
                try
                { 
                    var service = new UserService();
                    var logged = service.LoggedModel();
                    return logged;
                }
                catch (Exception)
                {
                    return new Logged();
                }
            }
        }
        //
        public static LoginInForModel LoginInFor()
        {
            try
            {
                string userId = Helper.Current.UserLogin.IdentifierID;
                using (var service = new UserService())
                    return service.LoginInformation(userId);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static string IdentifierID
        {
            get
            {
                try
                {
                    using (var service = new UserService())
                        return service.GetIdentifierID();
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
                    using (var service = new UserService())
                        return service.LoginID();
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
        }
        // ###########################################################################################################################################################################################
        public static DateTime Now => DateTime.Now;
        // ###########################################################################################################################################################################################

        public bool IsReusable => throw new NotImplementedException();

        public void ProcessRequest(HttpContext context)
        {
            throw new NotImplementedException();
        }
    }
}
