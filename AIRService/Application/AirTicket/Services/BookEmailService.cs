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
    public interface IBookEmailService : IEntityService<BookEmail> { }
    public class BookEmailService : EntityService<BookEmail>, IBookEmailService
    {
        public BookEmailService() : base() { }
        public BookEmailService(System.Data.IDbConnection db) : base(db) { }



        //##############################################################################################################################################################################################################################################################
    }
}
