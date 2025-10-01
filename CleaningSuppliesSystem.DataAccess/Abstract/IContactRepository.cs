using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DataAccess.Abstract
{
    public interface IContactRepository : IRepository<Contact>
    {
        Task<List<Contact>> GetActiveContactsAsync();
        Task<List<Contact>> GetDeletedContactsAsync();
        Task SoftDeleteRangeAsync(List<int> ids);
        Task UndoSoftDeleteRangeAsync(List<int> ids);
        Task PermanentDeleteRangeAsync(List<int> ids);
        Task<int> CountActiveContactsAsync();
        Task<int> CountDeletedContactsAsync();
    }
}
