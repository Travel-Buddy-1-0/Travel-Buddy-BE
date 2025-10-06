using BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IPaymentHistoryService
    {
        Task<IEnumerable<PaymentHistory>> GetAllAsync();
        Task<PaymentHistory?> GetByIdAsync(int id);
        Task<IEnumerable<PaymentHistory>> GetByUserIdAsync(int userId);
        Task AddAsync(PaymentHistory paymentHistory);
        Task UpdateAsync(PaymentHistory paymentHistory);
        Task DeleteAsync(int id);
        Task<PaymentHistory?> GetByOrderCodeAsync(long orderCode);
    }
}
