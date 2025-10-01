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
    public class ContactRepository : GenericRepository<Contact>, IContactRepository
    {
        private readonly CleaningSuppliesSystemContext _context;

        public ContactRepository(CleaningSuppliesSystemContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<Contact>> GetActiveContactsAsync()
        {
            return await _context.Contacts.Where(b => !b.IsDeleted).ToListAsync();
        }
        public async Task<List<Contact>> GetDeletedContactsAsync()
        {
            return await _context.Contacts.Where(b => b.IsDeleted).ToListAsync();
        }

        public async Task SoftDeleteRangeAsync(List<int> ids)
        {
            var contacts = await _context.Contacts
                .Where(c => ids.Contains(c.Id) && !c.IsDeleted)
                .ToListAsync();

            foreach (var contact in contacts)
            {
                contact.IsDeleted = true;
            }

            _context.Contacts.UpdateRange(contacts);
            await _context.SaveChangesAsync();
        }
        public async Task UndoSoftDeleteRangeAsync(List<int> ids)
        {
            var contacts = await _context.Contacts
                .Where(c => ids.Contains(c.Id) && c.IsDeleted)
                .ToListAsync();

            foreach (var contact in contacts)
            {
                contact.IsDeleted = false;
            }

            _context.Contacts.UpdateRange(contacts);
            await _context.SaveChangesAsync();
        }
        public async Task PermanentDeleteRangeAsync(List<int> ids)
        {
            var contacts = await _context.Contacts
                .Where(c => ids.Contains(c.Id))
                .ToListAsync();

            _context.Contacts.RemoveRange(contacts);
            await _context.SaveChangesAsync();
        }

        public async Task<int> CountActiveContactsAsync()
        {
            return _context.Contacts.Count(b => !b.IsDeleted);
        }

        public async Task<int> CountDeletedContactsAsync()
        {
            return _context.Contacts.Count(b => b.IsDeleted);
        }
    }
}
