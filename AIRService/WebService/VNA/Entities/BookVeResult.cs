using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiPortalBooking.Models.VNA_WS_Model
{
    public class BookVeResult
    {
        public string PNR { get; set; }
        public double Total { get; set; }
        public List<PricingInPNR> lPricingInPNR { get; set; }
    }
    public class PricingInPNR
    {
        public  string DateOfBirth { get  ; set; }
        public string SurName { get; set; }
        public string GivenName { get; set; }
        public string type { get; set; }
        public double ServiceFee { get; set; }
        public double ScanFee { get; set; }
        public double TaxFee { get; set; }
        public double VAT { get; set; }
        public double Fare { get; set; }
        public double TotalTax { get; set; }
    }

    public class ResponsePNRInfoModel
    {
        public string DateOfBirth { get; set; }
        public string SurName { get; set; }
        public string GivenName { get; set; }
        public string type { get; set; }
        public double ServiceFee { get; set; }
        public double ScanFee { get; set; }
        public double TaxFee { get; set; }
        public double VAT { get; set; }
        public double Fare { get; set; }
        public double TotalTax { get; set; }
    }
}