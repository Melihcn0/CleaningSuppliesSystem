using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.DataAccess.Context;
using CleaningSuppliesSystem.DataAccess.Repositories;
using CleaningSuppliesSystem.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DataAccess.Concrete
{
    public class CompanyBankRepository : GenericRepository<CompanyBank>, ICompanyBankRepository
    {
        private readonly CleaningSuppliesSystemContext _context;
        public CompanyBankRepository(CleaningSuppliesSystemContext context) : base(context)
        {
            _context = context;
        }
        public async Task<CompanyBank?> GetByIdAsync(int id)
        {
            return await _context.CompanyBanks
                .Include(c => c.AppUser)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<AppUser?> GetUserWithCompanyBankAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.CompanyBank)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }
        public async Task<CompanyBank?> GetFirstCompanyBankAsync()
        {
            return await _context.CompanyBanks.FirstOrDefaultAsync();
        }

    }
}