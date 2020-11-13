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
    public interface IBookContactService : IEntityService<BookContact> { }
    public class BookContactService : EntityService<BookContact>, IBookContactService
    {
        public BookContactService() : base() { }
        public BookContactService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public BookContact GetBookContactByID(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return null;
                string query = string.Empty;
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT TOP (1) * FROM App_BookContact WHERE ID = @Query";
                var model = _connection.Query<BookContact>(sqlQuery, new { Query = id }).FirstOrDefault();
                return model;
            }
            catch
            {
                return null;
            }
        }
        //##############################################################################################################################################################################################################################################################
        public static int GetBookContactTypeByOrderID(string orderId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(orderId))
                    return 0;
                //
                BookContactService bookContactService = new BookContactService();
                BookContact bookContact = bookContactService.GetAlls(m=>  m.BookOrderID == orderId).FirstOrDefault();
                if (bookContact == null)
                    return 0;
                //
                return bookContact.ContactType;
            }
            catch
            {
                return 0;
            }
        }

        //##############################################################################################################################################################################################################################################################
    }
}
