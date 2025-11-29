using BusinessObject.Data;
using BusinessObject.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Implements
{
    public class CvRepository : ICvRepository
    {
        private readonly AppDbContext _context;

        public CvRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Template>> GetTemplatesAsync() => await _context.Templates.ToListAsync();

        public async Task<Template?> GetTemplateByIdAsync(int id) => await _context.Templates.FindAsync(id);

        public async Task<List<Cv>> GetCvsByUserIdAsync(int userId)
        {
            return await _context.Cvs.Where(c => c.UserId == userId)
                                     .OrderByDescending(c => c.UpdatedAt).ToListAsync();
        }

        public async Task<Cv?> GetCvByIdAsync(int cvId, int userId)
        {
            return await _context.Cvs.FirstOrDefaultAsync(c => c.Id == cvId && c.UserId == userId);
        }

        public async Task<Cv> AddCvAsync(Cv cv)
        {
            _context.Cvs.Add(cv);
            await _context.SaveChangesAsync();
            return cv;
        }

        public async Task UpdateCvAsync(Cv cv)
        {
            _context.Cvs.Update(cv);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCvAsync(Cv cv)
        {
            _context.Cvs.Remove(cv);
            await _context.SaveChangesAsync();
        }
    }
}
