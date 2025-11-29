using BusinessObject.Entities;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class PaymentHistoryService : IPaymentHistoryService
    {
        private readonly IPaymentHistoryRepository _repository;

        public PaymentHistoryService(IPaymentHistoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<PaymentHistory>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<PaymentHistory?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<PaymentHistory?> GetByOrderCodeAsync(long orderCode)
        {
            return await _repository.GetByOrderCodeAsync(orderCode);
        }

        public async Task<IEnumerable<PaymentHistory>> GetByUserIdAsync(int userId)
        {
            return await _repository.GetByUserIdAsync(userId);
        }

        public async Task AddAsync(PaymentHistory paymentHistory)
        {
            await _repository.AddAsync(paymentHistory);
        }

        public async Task UpdateAsync(PaymentHistory paymentHistory)
        {
            await _repository.UpdateAsync(paymentHistory);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
