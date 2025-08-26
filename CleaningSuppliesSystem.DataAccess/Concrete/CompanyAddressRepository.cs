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
    public class CompanyAddressRepository : GenericRepository<CompanyAddress>, ICompanyAddressRepository
    {
        private readonly CleaningSuppliesSystemContext _context;
        public CompanyAddressRepository(CleaningSuppliesSystemContext context) : base(context)
        {
            _context = context;
        }

        public async Task<CompanyAddress?> GetByIdAsync(int id)
        {
            return await _context.CompanyAddresses
                .Include(c => c.AppUser)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<AppUser?> GetUserWithCompanyAddressAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.CompanyAddress)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

    }
}

