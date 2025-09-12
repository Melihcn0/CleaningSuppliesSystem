using CleaningSuppliesSystem.DTO.DTOs.Home.SecondaryBannerDtos;
using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Business.Abstract
{
    public interface ISecondaryBannerService : IGenericService<SecondaryBanner>
    {
        Task<(bool IsSuccess, string Message, int CreatedId)> TCreateSecondaryBannerAsync(CreateSecondaryBannerDto createSecondaryBannerDto);
        Task<(bool IsSuccess, string Message, int UpdatedId)> TUpdateSecondaryBannerAsync(UpdateSecondaryBannerDto updateSecondaryBannerDto);
        Task<(bool IsSuccess, string Message, int SoftDeletedId)> TSoftDeleteSecondaryBannerAsync(int id);
        Task<(bool IsSuccess, string Message, int UndoSoftDeletedId)> TUndoSoftDeleteSecondaryBannerAsync(int id);
        Task<(bool IsSuccess, string Message)> TPermanentDeleteSecondaryBannerAsync(int id);
        Task<List<ResultSecondaryBannerDto>> TGetActiveSecondaryBannersAsync();
        Task<List<ResultSecondaryBannerDto>> TGetDeletedSecondaryBannersAsync();
        Task<bool> ToggleSecondaryBannerStatusAsync(int secondaryBannerId, bool newStatus);
    }
}
