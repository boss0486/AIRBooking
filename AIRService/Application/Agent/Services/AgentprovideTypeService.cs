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
    public class AgentProvideTypeService
    {
        public static string DropdownList(string id)
        {
            try
            {
                string result = string.Empty;
                var service = new AgentProvideTypeService();
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
        public List<AgentProvideTypeOption> DataOption()
        {
            try
            {
                List<AgentProvideTypeOption> customerTypeOptions = new List<AgentProvideTypeOption>
                {
                    new AgentProvideTypeOption()
                    {
                        ID = "agent",
                        Alias = "agent",
                        Title = "Đại lý",
                        Type =  1,
                    },
                    new AgentProvideTypeOption()
                    {
                        ID = "comp",
                        Alias = "comp",
                        Title = "Công ty",
                        Type =  2,
                    }
                };
                return customerTypeOptions;
            }
            catch
            {
                return new List<AgentProvideTypeOption>();
            }
        }
        //##############################################################################################################################################################################################################################################################
        public static string GetNameByID(string id)
        {
            try
            {
                var service = new AgentProvideTypeService();
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
                var service = new AgentProvideTypeService();
                var data = service.DataOption().Where(m => !string.IsNullOrWhiteSpace(m.ID) && m.Type == type).FirstOrDefault();
                return data.Title;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static int GetAgentProvideType(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return (int)AgentProvideEnum.AgentProvideType.NONE;
                //
                id = id.ToLower();
                var service = new AgentProvideTypeService();
                var data = service.DataOption().Where(m => m.ID == id).FirstOrDefault();
                //
                return data.Type;
            }
            catch
            {
                return (int)AgentProvideEnum.AgentProvideType.NONE;
            }
        }

        public static string GetAgentProvideTypeIDByType(int _typeEnum)
        {
            try
            {
                var service = new AgentProvideTypeService();
                var data = service.DataOption().Where(m => m.Type == _typeEnum).FirstOrDefault();

                return data.ID;
            }
            catch
            {
                return string.Empty;
            }
        }
        public static List<AgentProvideTypeOption> GetAgentProvideTypeByTypeID(int typeId)
        {
            try
            {
                AgentProvideTypeService service = new AgentProvideTypeService();
                List<AgentProvideTypeOption> data = service.DataOption().Where(m => m.Type == typeId).ToList();
                if (data.Count == 0)
                    return new List<AgentProvideTypeOption>();
                //
                return data;
            }
            catch
            {
                return new List<AgentProvideTypeOption>();
            }
        }

    }

    public class CustomerTypeService
    {
        public static string DropdownList(int id = -1)
        {
            try
            {
                string result = string.Empty;
                CustomerTypeService service = new CustomerTypeService();
                List<AgentTypeOption> dtList = service.DataOption();
                if (dtList.Count > 0)
                {
                    foreach (var item in dtList)
                    {
                        string select = string.Empty;
                        if (item.ID == id)
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
        public List<AgentTypeOption> DataOption()
        {
            try
            {
                List<AgentTypeOption> customerTypeOptions = new List<AgentTypeOption>
                {
                    new AgentTypeOption()
                    {
                        ID =  1,
                        Title = "Công ty"
                    },
                    new AgentTypeOption()
                    {
                       ID =  2,
                        Title = "Khách lẻ"
                    }
                };
                return customerTypeOptions;
            }
            catch
            {
                return new List<AgentTypeOption>();
            }
        }
        //##############################################################################################################################################################################################################################################################
        public static string GetNameByID(int id)
        {
            try
            {
                CustomerTypeService service = new CustomerTypeService();
                AgentTypeOption data = service.DataOption().Where(m => m.ID == id).FirstOrDefault();
                return data.Title;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
