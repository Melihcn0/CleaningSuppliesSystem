using CleaningSuppliesSystem.DTO.DTOs.Home.ServiceDtos;
using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Business.Abstract
{
    public interface IServiceService : IGenericService<Service>
    {
        Task<(bool IsSuccess, string Message, int CreatedId)> TCreateServiceAsync(CreateServiceDto createServiceDto);
        Task<(bool IsSuccess, string Message, int UpdatedId)> TUpdateServiceAsync(UpdateServiceDto updateServiceDto);
        Task<(bool IsSuccess, string Message, int SoftDeletedId)> TSoftDeleteServiceAsync(int id);
        Task<(bool IsSuccess, string Message, int UndoSoftDeletedId)> TUndoSoftDeleteServiceAsync(int id);
        Task<(bool IsSuccess, string Message)> TPermanentDeleteServiceAsync(int id);
        Task<List<ResultServiceDto>> TGetActiveServicesAsync();
        Task<List<ResultServiceDto>> TGetDeletedServicesAsync();
        Task<(bool IsSuccess, string Message)> ToggleServiceStatusAsync(int serviceId, bool newStatus);

        Task<int> TGetActiveServiceCountAsync();

    }
}
