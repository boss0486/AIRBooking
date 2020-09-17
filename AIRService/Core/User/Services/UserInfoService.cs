using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using WebCore.Entities;
using System;
using Dapper;
using System.Linq;
using PagedList;
using Helper;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Web;

namespace WebCore.Services
{
    public interface IUserInfoService : IEntityService<UserInfo> { }
    public class UserInfoService : EntityService<UserInfo>, IUserInfoService
    {
        public UserInfoService() : base() { }
        public UserInfoService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################


        // function ###########################################################################################################################################################################################
        public string GetFullName(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return string.Empty;
                //
                using (var service = new UserInfoService(_connection))
                {
                    var data = service.GetAlls(m => m.UserID == id.ToLower()).FirstOrDefault();
                    //
                    if (data == null)
                        return string.Empty;
                    //
                    return data.FullName;
                }
            }
            catch
            {
                return string.Empty;
            }
        }


    }
}