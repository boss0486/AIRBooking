using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;
using WebCore.Model.Entities;

namespace WebCore.Entities
{
    public class AirportTypeOption
    {
        public int ID { get; set; }
        public string Title { get; set; }
    }
}