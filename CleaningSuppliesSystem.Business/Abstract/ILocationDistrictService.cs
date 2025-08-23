using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleaningSuppliesSystem.DTO.DTOs.LocationDtos;
using CleaningSuppliesSystem.Entity.Entities;

namespace CleaningSuppliesSystem.Business.Abstract
{
    public interface ILocationDistrictService : IGenericService<LocationDistrict>
    {
        Task<(bool IsSuccess, string Message, int CreatedId)> TCreateLocationDistrictAsync(CreateLocationDistrictDto createLocationDistrictDto);
        Task<(bool IsSuccess, string Message, int SoftDeletedId)> TSoftDeleteLocationDistrictAsync(int id);
        Task<(bool IsSuccess, string Message, int UndoSoftDeletedId)> TUndoSoftDeleteLocationDistrictAsync(int id);
        Task<(bool IsSuccess, string Message)> TPermanentDeleteLocationDistrictAsync(int id);
        Task<List<ResultLocationDistrictDto>> TGetActiveLocationDistrictsAsync();
        Task<List<ResultLocationDistrictDto>> TGetDeletedLocationDistrictsAsync();
        Task<List<ResultLocationDistrictDto>> TGetActiveByCityIdAsync(int cityId);
    }
}
