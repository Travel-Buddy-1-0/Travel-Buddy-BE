using BusinessObject.Data;
using BusinessObject.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class VoucherRepository : IVoucherRepository
    {
        private readonly AppDbContext _context;

        public VoucherRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Voucher>> GetActiveVouchersAsync()
        {
            var now = DateTime.UtcNow;

            // Thêm log ở đây
            System.Diagnostics.Debug.WriteLine("--- Đang vào VoucherRepository.GetActiveVouchersAsync ---");

            var result = await _context.Vouchers
                .Where(v => v.IsActive == true &&
                            v.StartDate <= now &&
                            v.EndDate >= now &&
                            v.CurrentUsageCount < v.MaxUsageCount)
                .ToListAsync();

            // Thêm log ở đây
            if (result == null)
            {
                System.Diagnostics.Debug.WriteLine("--- LỖI: ToListAsync() trả về null! ---");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"--- Sắp trả về {result.Count} vouchers ---");
            }

            return result;
        }

        public async Task<Voucher?> GetByCodeAsync(string code)
        {
            return await _context.Vouchers.FirstOrDefaultAsync(v => v.Code == code);
        }

        public async Task UpdateAsync(Voucher voucher)
        {
            _context.Vouchers.Update(voucher);
            await _context.SaveChangesAsync();
        }
    }
}
