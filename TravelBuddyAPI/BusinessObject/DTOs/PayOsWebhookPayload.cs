using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class PayOsWebhookPayload
    {
        public long orderCode { get; set; }
        public int amount { get; set; }
        public string description { get; set; }
        public string status { get; set; } // "PAID", "CANCELLED", ...
        public DateTime? transactionDateTime { get; set; }
    }

}
