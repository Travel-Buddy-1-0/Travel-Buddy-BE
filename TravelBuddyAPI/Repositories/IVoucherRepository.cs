using BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IVoucherRepository
    {
        Task<List<Voucher>> GetActiveVouchersAsync();
        Task<Voucher?> GetByCodeAsync(string code);
        Task UpdateAsync(Voucher voucher);
    }
}
