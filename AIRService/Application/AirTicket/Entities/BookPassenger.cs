using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;
using System.Collections.Generic;
using WebCore.Model.Entities;
using WebCore.Services;

namespace WebCore.Entities
{
    [ConnectionString(DbConnect.ConnectionString.CMS)]
    [Table("App_BookPassenger")]
    public partial class BookPassenger  
    {
        public BookPassenger()
        {
            ID = Guid.NewGuid().ToString();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string PassengerType { get; set; }
        public string PNR { get; set; }
        public string FullName { get; set; }
        public int Gender { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public  DateTime DateOfBirth { get; set; }
    }
    // model
    public class BookPassengerCreateModel
    {
        public string PassengerType { get; set; }
        public string FullName { get; set; }
        public int Gender { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
    public class BookPassengerUpdateModel : BookPassengerCreateModel
    {
        public string ID { get; set; }
    }
    public class BookPassengerIDModel
    {
        public string ID { get; set; }
    }

    public class RequestBookPassengerModel
    {
        public string PassengerType { get; set; }
        public string FullName { get; set; }
        public int Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
    }












}