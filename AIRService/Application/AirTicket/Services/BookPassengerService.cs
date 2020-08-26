using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using System;
using Dapper;
using System.Linq;
using PagedList;
using System.Web.Mvc;
using System.Collections.Generic;
using Helper;
using System.Web;
using WebCore.Entities;

namespace WebCore.Services
{
    public interface IBookPassengerService : IEntityService<BookPassenger> { }
    public class BookPassengerService : EntityService<BookPassenger>, IBookPassengerService
    {
        public BookPassengerService() : base() { }
        public BookPassengerService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
    }
}
