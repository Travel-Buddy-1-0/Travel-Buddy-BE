using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class CreatePaymentRequest
    {
        public int UserId { get; set; }
        public string Description { get; set; }
        public int Amount { get; set; }
    }
}
