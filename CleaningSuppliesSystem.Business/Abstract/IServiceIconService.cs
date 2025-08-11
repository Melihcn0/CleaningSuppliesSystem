
using CleaningSuppliesSystem.DTO.DTOs.Home.ServiceIconDtos;
using CleaningSuppliesSystem.Entity.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Business.Abstract
{
    public interface IServiceIconService : IGenericService<ServiceIcon>
    {
        Task<(bool IsSuccess, string Message, int CreatedId)> TCreateServiceIconAsync(CreateServiceIconDto dto);
        Task<(bool IsSuccess, string Message, int UpdatedId)> TUpdateServiceIconAsync(UpdateServiceIconDto dto);
        Task<(bool IsSuccess, string Message, int SoftDeletedId)> TSoftDeleteServiceIconAsync(int id);
        Task<(bool IsSuccess, string Message, int UndoSoftDeletedId)> TUndoSoftDeleteServiceIconAsync(int id);
        Task<(bool IsSuccess, string Message)> TPermanentDeleteServiceIconAsync(int id);
        Task<List<ResultServiceIconDto>> TGetActiveServiceIconsAsync();
        Task<List<ResultServiceIconDto>> TGetDeletedServiceIconsAsync();
        Task<List<ResultServiceIconDto>> TGetUnusedActiveServiceIconsAsync();

    }
}
