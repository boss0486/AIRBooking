using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIRService.Models
{
    public class TokenModel
    {
        public string Status { get; set; }
        public string Token { get; set; }
        public string ConversationID { get; set; }
        public string LNIATA { get; set; }
    }
    public class TransactionModel  
    {
        public bool TranactionState { get; set; } = false;
        public TokenModel TokenModel { get; set; }

    }
}