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
    public class AirportTypeService
    {
        public static string DropdownList(int id)
        {
            try
            {
                string result = string.Empty;
                var service = new AirportTypeService();
                var dtList = service.DataOption();
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
        public List<AirportTypeOption> DataOption()
        {
            try
            {
                List<AirportTypeOption> AirportTypeOptions = new List<AirportTypeOption>
                {
                    new AirportTypeOption()
                    {
                        ID = 1,
                        Title = "Nội địa",
                    },
                    new AirportTypeOption()
                    {
                        ID = 2,
                        Title = "Quốc tế",
                    }
                };
                return AirportTypeOptions;
            }
            catch
            {
                return new List<AirportTypeOption>();
            }
        }
        //##############################################################################################################################################################################################################################################################
        public static string GetNameByID(int id)
        {
            try
            {
                AirportTypeService service = new AirportTypeService();
                AirportTypeOption data = service.DataOption().Where(m => m.ID == id).FirstOrDefault();
                return data.Title;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
