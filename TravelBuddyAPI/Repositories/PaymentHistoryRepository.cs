using BusinessObject.Data;
using BusinessObject.Entities;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class PaymentHistoryRepository : IPaymentHistoryRepository
    {
        private readonly AppDbContext _context;

        public PaymentHistoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PaymentHistory>> GetAllAsync()
        {
            return await _context.PaymentHistories
                                 .Include(p => p.User)
                                 .ToListAsync();
        }

        public async Task<PaymentHistory?> GetByIdAsync(int id)
        {
            return await _context.PaymentHistories
                                 .Include(p => p.User)
                                 .FirstOrDefaultAsync(p => p.PaymentId == id);
        }

        public async Task<PaymentHistory?> GetByOrderCodeAsync(long orderCode)
        {
            return await _context.PaymentHistories
                                 .Include(p => p.User)
                                 .FirstOrDefaultAsync(p => p.TransactionCode == orderCode);
        }
        public async Task<IEnumerable<PaymentHistory>> GetByUserIdAsync(int userId)
        {
            return await _context.PaymentHistories
                             .Where(p => p.UserId == userId)
                             .OrderByDescending(p => p.CreatedAt) // Sắp xếp giao dịch mới nhất lên đầu
                             .ToListAsync();
        }

        public async Task AddAsync(PaymentHistory paymentHistory)
        {
            await _context.PaymentHistories.AddAsync(paymentHistory);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PaymentHistory paymentHistory)
        {
            _context.PaymentHistories.Update(paymentHistory);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.PaymentHistories.FindAsync(id);
            if (entity != null)
            {
                _context.PaymentHistories.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
