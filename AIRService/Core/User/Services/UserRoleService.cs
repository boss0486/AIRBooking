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
    public interface IUserRole : IEntityService<UserRole> { }
    public class UserRoleService : EntityService<UserRole>, IUserRole
    {
        public UserRoleService() : base() { }
        public UserRoleService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
    }
}