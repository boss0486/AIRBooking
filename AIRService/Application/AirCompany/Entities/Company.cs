using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using WebCore.Model.Entities;
using WebCore.Services;

namespace WebCore.Entities
{

    [ConnectionString(DbConnect.ConnectionString.CMS)]
    [Table("App_Company")]
    public partial class Company : WEBModel
    {
        public Company()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string AgentID { get; set; }
        public string CodeID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public string Summary { get; set; }
        public string Address { get; set; }
        public string CompanyPhone { get; set; }
        public string TaxCode { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public int TermPayment { get; set; }
        public string RegisterID { get; set; }
    }
     
    public class CompanyCreateModel
    {
        public string AgentID { get; set; }
        public string CodeID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public string Summary { get; set; }
        public string Address { get; set; }
        public string CompanyPhone { get; set; }
        public string TaxCode { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public int TermPayment { get; set; }
        //
        public string AccountID { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        //
        public int Enabled { get; set; }
    }

    public class CompanyUpdateModel : CompanyCreateModel
    {
        public string ID { get; set; }
    }
 
    public class CompanyIDModel
    {
        public string ID { get; set; }
    }
    public class CompanyResult : WEBModelResult
    {
        public string ID { get; set; }
        public string AgentID { get; set; }
        public string AgentCode { get; set; }
        public string AgentName { get; set; }
        public string CodeID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public string Summary { get; set; }
        public string Address { get; set; }
        public string CompanyPhone { get; set; }
        public string TaxCode { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public int TermPayment { get; set; }
        public string RegisterID { get; set; }

    }

    public class CompanyOption
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string CodeID { get; set; }
        public string ParentID { get; set; }
    }
}