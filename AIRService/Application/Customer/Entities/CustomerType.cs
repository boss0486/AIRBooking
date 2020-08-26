using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;
using WebCore.Model.Entities;

namespace WebCore.Entities
{
    public class CustomerTypeOption
    {
        public string ID { get; set; }
        public string KeyID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public int Type { get; set; }
    }
}