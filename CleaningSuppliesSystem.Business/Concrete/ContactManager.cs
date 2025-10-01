using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.BrandDtos;
using CleaningSuppliesSystem.DTO.DTOs.Home.ContactDtos;
using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CleaningSuppliesSystem.Business.Validators.Validators;

namespace CleaningSuppliesSystem.Business.Concrete
{
    public class ContactManager : GenericManager<Contact>, IContactService
    {
        private readonly IContactRepository _contactRepository;
        private readonly IMapper _mapper;
        
        public ContactManager(IRepository<Contact> repository, IContactRepository contactRepository, IMapper mapper) : base(repository)
        {
            _contactRepository = contactRepository;
            _mapper = mapper;
        }

        public async Task<(bool IsSuccess, string Message, int CreatedId)> TCreateContactAsync(CreateContactDto createContactDto)
        {
            var validator = new CreateContactValidator();
            var validationResult = await validator.ValidateAsync(createContactDto);
            if (!validationResult.IsValid)
            {
                var message = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
                return (false, message, 0);
            }
            var contact = _mapper.Map<Contact>(createContactDto);
            contact.CreatedDate = DateTime.Now;
            await _contactRepository.CreateAsync(contact);
            return (true, "Mesaj başarıyla oluşturuldu.", contact.Id);
        }
        public async Task<(bool IsSuccess, string Message, int SoftDeletedId)> TSoftDeleteContactAsync(int id)
        {
            var contact = await _contactRepository.GetByIdAsync(id);
            if (contact == null)
                return (false, "Mesaj bulunamadı.", 0);

            if (contact.IsDeleted)
                return (false, "Mesaj zaten silinmiş.", contact.Id);

            contact.DeletedDate = DateTime.Now;
            contact.IsDeleted = true;
            await _contactRepository.UpdateAsync(contact);
            return (true, "Mesaj başarıyla çöp kutusuna taşındı.", contact.Id);
        }

        public async Task<(bool IsSuccess, string Message, int UndoSoftDeletedId)> TUndoSoftDeleteContactAsync(int id)
        {
            var contact = await _contactRepository.GetByIdAsync(id);
            if (contact == null)
                return (false, "Mesaj bulunamadı.", 0);

            if (!contact.IsDeleted)
                return (false, "Mesaj zaten aktif.", contact.Id);

            contact.DeletedDate = null;
            contact.IsDeleted = false;
            await _contactRepository.UpdateAsync(contact);
            return (true, "Mesaj başarıyla geri getirildi.", contact.Id);
        }

        public async Task<(bool IsSuccess, string Message)> TPermanentDeleteContactAsync(int id)
        {
            var contact = await _contactRepository.GetByIdAsync(id);
            if (contact == null)
                return (false, "Mesaj bulunamadı.");

            if (!contact.IsDeleted)
                return (false, "Mesaj silinmemiş. Önce silmeniz gerekir.");

            await _contactRepository.DeleteAsync(contact.Id);
            return (true, "Mesaj çöp kutusundan kalıcı olarak silindi.");
        }
        public async Task<List<ResultContactDto>> TGetActiveContactsAsync()
        {
            var entities = await _contactRepository.GetActiveContactsAsync();
            return _mapper.Map<List<ResultContactDto>>(entities);
        }
        public async Task<List<ResultContactDto>> TGetDeletedContactsAsync()
        {
            var entities = await _contactRepository.GetDeletedContactsAsync();
            return _mapper.Map<List<ResultContactDto>>(entities);
        }
        public async Task<List<(int Id, bool IsSuccess, string Message)>> TSoftDeleteRangeContactAsync(List<int> ids)
        {
            var results = new List<(int Id, bool IsSuccess, string Message)>();

            foreach (var id in ids)
            {
                var contact = await _contactRepository.GetByIdAsync(id);
                if (contact == null)
                {
                    results.Add((id, false, "Mesaj bulunamadı."));
                    continue;
                }

                if (contact.IsDeleted)
                {
                    results.Add((id, false, "Mesaj zaten silinmiş."));
                    continue;
                }

                contact.IsDeleted = true;
                contact.DeletedDate = DateTime.UtcNow;
                await _contactRepository.UpdateAsync(contact);
                results.Add((id, true, "Mesaj başarıyla silindi."));
            }

            return results;
        }
        public async Task<List<(int Id, bool IsSuccess, string Message)>> TUndoSoftDeleteRangeContactAsync(List<int> ids)
        {
            var results = new List<(int Id, bool IsSuccess, string Message)>();

            foreach (var id in ids)
            {
                var contact = await _contactRepository.GetByIdAsync(id);

                if (contact == null)
                {
                    results.Add((id, false, "Mesaj bulunamadı."));
                    continue;
                }

                if (!contact.IsDeleted)
                {
                    results.Add((id, false, "Mesaj zaten aktif."));
                    continue;
                }

                contact.IsDeleted = false;
                contact.DeletedDate = null;
                await _contactRepository.UpdateAsync(contact);

                results.Add((id, true, "Mesaj başarıyla geri getirildi."));
            }

            return results;
        }
        public async Task TPermanentDeleteRangeContactAsync(List<int> ids)
        {
            await _contactRepository.PermanentDeleteRangeAsync(ids);
        }
        public async Task<int> TGetActiveContactsCountAsync()
        {
            return await _contactRepository.CountActiveContactsAsync();
        }

        public async Task<int> TGetDeletedContactsCountAsync()
        {
            return await _contactRepository.CountDeletedContactsAsync();
        }
    }
}
