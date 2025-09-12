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
    public class AdminProfileRepository : GenericRepository<AppUser>, IAdminProfileRepository
    {
        private readonly CleaningSuppliesSystemContext _context;
        public AdminProfileRepository(CleaningSuppliesSystemContext context) : base(context)
        {
            _context = context;
        }
        public async Task<AppUser> GetUserWithCompanyBankAndAddressAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.CompanyBank)
                .Include(u => u.CompanyAddress)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

    }
}
