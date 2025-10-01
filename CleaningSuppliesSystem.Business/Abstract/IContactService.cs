using CleaningSuppliesSystem.DTO.DTOs.BrandDtos;
using CleaningSuppliesSystem.DTO.DTOs.Home.ContactDtos;
using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Business.Abstract
{
    public interface IContactService : IGenericService<Contact>
    {
        Task<(bool IsSuccess, string Message, int CreatedId)> TCreateContactAsync(CreateContactDto createContactDto);
        Task<(bool IsSuccess, string Message, int SoftDeletedId)> TSoftDeleteContactAsync(int id);
        Task<(bool IsSuccess, string Message, int UndoSoftDeletedId)> TUndoSoftDeleteContactAsync(int id);
        Task<(bool IsSuccess, string Message)> TPermanentDeleteContactAsync(int id);
        Task<List<ResultContactDto>> TGetActiveContactsAsync();
        Task<List<ResultContactDto>> TGetDeletedContactsAsync();
        Task<List<(int Id, bool IsSuccess, string Message)>> TSoftDeleteRangeContactAsync(List<int> ids);
        Task<List<(int Id, bool IsSuccess, string Message)>> TUndoSoftDeleteRangeContactAsync(List<int> ids);
        Task TPermanentDeleteRangeContactAsync(List<int> ids);
        Task<int> TGetActiveContactsCountAsync();
        Task<int> TGetDeletedContactsCountAsync();
    }
}
