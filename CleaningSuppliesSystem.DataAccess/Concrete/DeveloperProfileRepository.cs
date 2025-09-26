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
    public class DeveloperProfileRepository : GenericRepository<AppUser>, IDeveloperProfileRepository
    {
        private readonly CleaningSuppliesSystemContext _context;
        public DeveloperProfileRepository(CleaningSuppliesSystemContext context) : base(context)
        {
            _context = context;
        }

    }
}
