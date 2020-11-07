using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using System;
using Dapper;
using System.Linq;
using PagedList;
using System.Web.Mvc;
using System.Collections.Generic;
using WebCore.Entities;
using Helper;
using System.Web;
using System.Web.Configuration;
using System.Data;

namespace WebCore.Services
{
    public interface IClientCompService : IEntityService<ClientComp> { }
    public class ClientCompService : EntityService<ClientComp>, IClientCompService
    {
        public ClientCompService() : base() { }
        public ClientCompService(System.Data.IDbConnection db) : base(db) { }
    }
}