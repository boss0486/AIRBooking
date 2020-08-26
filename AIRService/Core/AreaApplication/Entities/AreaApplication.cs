using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;
using System.Collections.Generic;
using WebCore.Model.Entities;

namespace WebCore.Entities
{
    public class AreaIDRequestModel
    {
        public string RouteArea { get; set; }
    }
    public class AreaResult : WEBModelResult
    {
        public string ID { get; set; }
        public string KeyID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public int KeyEnum { get; set; }

    }
    public class AreaOption
    {
        public string ID { get; set; }
        public string KeyID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public int Type { get; set; }
    }
}