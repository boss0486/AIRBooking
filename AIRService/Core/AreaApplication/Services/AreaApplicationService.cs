﻿using AL.NetFrame.Attributes;
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
using WebCore.Model.Entities;
using WebCore.ENM;

namespace WebCore.Services
{
    public class AreaApplicationService
    {

        //##############################################################################################################################################################################################################################################################

        public static string GetAreaKeyByID(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return string.Empty;
                //
                var service = new AreaApplicationService();
                var data = service.DataOption().Where(m => m.ID.ToLower().Equals(id.ToLower())).FirstOrDefault();
                return data.KeyID;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static int GetAreaTypeByID(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return (int)AreaApplicationEnum.AreaType.NONE;
                //
                var service = new AreaApplicationService();
                var data = service.DataOption().Where(m => m.ID.ToLower().Equals(id.ToLower())).FirstOrDefault();
                return data.Type;
            }
            catch
            {
                return (int)AreaApplicationEnum.AreaType.NONE;
            }
        }

        public static string GetAreaID(string keyId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyId))
                    return string.Empty;
                //
                var service = new AreaApplicationService();
                var data = service.DataOption().Where(m => m.KeyID.ToLower().Equals(keyId.ToLower())).FirstOrDefault();

                return data.ID;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string GetRouteAreaID(int type)
        {
            try
            {
                var service = new AreaApplicationService();
                var data = service.DataOption().Where(m => m.Type == type).FirstOrDefault();
                if (data == null)
                    return string.Empty;
                //
                return data.ID;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string DropdowList(string id)
        {
            try
            {
                string result = string.Empty;
                var service = new AreaApplicationService();
                var dtList = service.DataOption();
                if (dtList.Count > 0)
                {
                    int cnt = 0;
                    foreach (var item in dtList)
                    {
                        string select = string.Empty;
                        if (!string.IsNullOrEmpty(id) && item.ID.Equals(id.ToLower()))
                            select = "selected";
                        //
                        result += "<option value='" + item.ID + "'" + select + ">" + item.Title + "</option>";
                        cnt++;
                    }
                }
                return result;

            }
            catch
            {
                return string.Empty;
            }
        }
        public List<AreaOption> DataOption()
        {
            try
            {
                List<AreaOption> areaOptions = new List<AreaOption>
                {
                    new AreaOption()
                    {
                        ID = "Development",
                        Alias = "58c2d3c0-5f95-42cc-942e-1d1b014d30a4",
                        KeyID = "Development",
                        Title = "Development",
                        Type =  1,
                    },
                    new AreaOption()
                    {
                        ID = "Management",
                        Alias = "0d854581-edf9-49c0-8e3d-3d427a203c1b",
                        KeyID = "Management",
                        Title = "Management",
                        Type =  2,
                    }
                };
                return areaOptions;
            }
            catch
            {
                return new List<AreaOption>();
            }
        }


    }
}
