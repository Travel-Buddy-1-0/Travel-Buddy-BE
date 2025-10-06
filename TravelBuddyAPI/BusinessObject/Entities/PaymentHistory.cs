using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Entities
{
    public partial class PaymentHistory
    {
        public int PaymentId { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "VND";
        public string PaymentMethod { get; set; } = "PayOS";
        public long TransactionCode { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public virtual User User { get; set; } = null!;
    }

}
