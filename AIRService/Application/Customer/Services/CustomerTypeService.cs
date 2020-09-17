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
using WebCore.Model.Enum;
using WebCore.Core;
using WebCore.Entities;
using WebCore.Services;
using WebCore.Model.Entities;
using Helper.Page;
using WebCore.ENM;

namespace WebCore.Services
{
    public class CustomerTypeService
    {
        public static string DropdownList(string id)
        {
            try
            {
                string result = string.Empty;
                var service = new CustomerTypeService();
                var dtList = service.DataOption();
                if (dtList.Count > 0)
                {
                    foreach (var item in dtList)
                    {
                        string select = string.Empty;
                        if (!string.IsNullOrWhiteSpace(item.ID) && item.ID == id.ToLower())
                            select = "selected";
                        result += "<option value='" + item.ID + "' " + select + ">" + item.Title + "</option>";
                    }
                }
                return result;

            }
            catch
            {
                return string.Empty;
            }
        }
        public List<CustomerTypeOption> DataOption()
        {
            try
            {
                List<CustomerTypeOption> customerTypeOptions = new List<CustomerTypeOption>
                {
                    new CustomerTypeOption()
                    {
                        ID = "AGENT",
                        Alias = "AGENT",
                        Title = "Đại lý",
                        Type =  1,
                    },
                    new CustomerTypeOption()
                    {
                        ID = "COMP",
                        Alias = "comp",
                        Title = "Công ty",
                        Type =  2,
                    }
                };
                return customerTypeOptions;
            }
            catch
            {
                return new List<CustomerTypeOption>();
            }
        }
        //##############################################################################################################################################################################################################################################################
        public static string GetNameByID(string id)
        {
            try
            {
                var service = new CustomerTypeService();
                id = id.ToLower();
                var data = service.DataOption().Where(m => m.ID == id).FirstOrDefault();
                return data.Title;
            }
            catch
            {
                return string.Empty;
            }
        }
        public static string GetNameByType(int type)
        {
            try
            {
                var service = new CustomerTypeService();
                var data = service.DataOption().Where(m => !string.IsNullOrWhiteSpace(m.ID) && m.Type == type).FirstOrDefault();
                return data.Title;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static int GetCustomerType(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return (int)CustomerEnum.CustomerType.NONE;
                //
                id = id.ToLower();
                var service = new CustomerTypeService();
                var data = service.DataOption().Where(m => m.ID == id).FirstOrDefault();
                //
                return data.Type;
            }
            catch
            {
                return (int)CustomerEnum.CustomerType.NONE;
            }
        }

        public static string GetCustomerTypeIDByType(int _typeEnum)
        {
            try
            {
                var service = new CustomerTypeService();
                var data = service.DataOption().Where(m => m.Type == _typeEnum).FirstOrDefault();

                return data.ID;
            }
            catch
            {
                return string.Empty;
            }
        }
        public static List<CustomerTypeOption> GetCustomerTypeByTypeID(int typeId)
        {
            try
            {
                CustomerTypeService service = new CustomerTypeService();
                List<CustomerTypeOption> data = service.DataOption().Where(m => m.Type == typeId).ToList();
                if (data.Count == 0)
                    return new List<CustomerTypeOption>();
                //
                return data;
            }
            catch
            {
                return new List<CustomerTypeOption>();
            }
        }

    }
}
