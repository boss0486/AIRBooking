using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using System;
using Dapper;
using System.Linq;
using PagedList;
using Helper;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Web;
using System.IdentityModel.Tokens.Jwt;
using WebCore.Model.Enum;
using WebCore.Core;
using WebCore.Entities;
using WebCore.Services;
using Helper.Page;
using Helper.File;
using WebCore.Model.Entities;

namespace WebCore.Services
{
    public interface IUserLoginService : IEntityService<UserLogin> { }
    public class UserLoginService : EntityService<UserLogin>, IUserLoginService
    {
        public UserLoginService() : base() { }
        public UserLoginService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################

    }
}