using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleaningSuppliesSystem.DTO.DTOs.LocationDtos;
using CleaningSuppliesSystem.DTO.DTOs.BrandDtos;

namespace CleaningSuppliesSystem.Business.Abstract
{
    public interface ILocationCityService : IGenericService<LocationCity>
    {
        Task<(bool IsSuccess, string Message, int CreatedId)> TCreateLocationCityAsync(CreateLocationCityDto createLocationCityDto);
        Task<(bool IsSuccess, string Message, int SoftDeletedId)> TSoftDeleteLocationCityAsync(int id);
        Task<(bool IsSuccess, string Message, int UndoSoftDeletedId)> TUndoSoftDeleteLocationCityAsync(int id);
        Task<(bool IsSuccess, string Message)> TPermanentDeleteLocationCityAsync(int id);
        Task<List<ResultLocationCityDto>> TGetActiveLocationCitysAsync();
        Task<List<ResultLocationCityDto>> TGetDeletedLocationCitysAsync();
        Task<List<ResultLocationCityWithLocationDistrictDto>> TGetLocationCityWithLocationDistrictAsync();
    }
}
