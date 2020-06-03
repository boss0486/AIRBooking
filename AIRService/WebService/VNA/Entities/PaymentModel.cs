using AIRService.Models;
using ApiPortalBooking.Models.VNA_WS_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiPortalBooking.Models
{
    public class PaymentModel : TokenModel
    {
        public RevResult RevResult { get; set; }
        public string pnr { get; set; }
        public List<PaymentOrderDetail> PaymentOrderDetail { get; set; }
        public double Total { get; set; }

    }
    public class PaymentOrderDetail
    {
        public string PsgrType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public double BaseFare { get; set; }
        public double Taxes { get; set; }
        public double Fees { get; set; }

    }
}