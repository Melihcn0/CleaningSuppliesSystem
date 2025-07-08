using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DataAccess.Abstract
{
    public interface IFinanceRepository : IRepository<Finance>
    {
        Task<Finance> GetByIdAsync(int id);
        Task CreateAsync(Finance finance);
        Task UpdateAsync(Finance finance);
        Task SoftDeleteAsync(Finance finance);
        Task UndoSoftDeleteAsync(Finance finance);
    }
}
