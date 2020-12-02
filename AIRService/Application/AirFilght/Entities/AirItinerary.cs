using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;
using WebCore.Model.Entities;

namespace WebCore.Entities
{
    public class AirItineraryOption
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string CodeID { get; set; }
    }
}