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
        Task<(bool IsSuccess, string Message, int CreatedId)> TCreateBannerAsync(CreateSecondaryBannerDto createSecondaryBannerDto);
        Task<(bool IsSuccess, string Message, int UpdatedId)> TUpdateBannerAsync(UpdateSecondaryBannerDto updateSecondaryBannerDto);
        Task<(bool IsSuccess, string Message, int SoftDeletedId)> TSoftDeleteBannerAsync(int id);
        Task<(bool IsSuccess, string Message, int UndoSoftDeletedId)> TUndoSoftDeleteBannerAsync(int id);
        Task<(bool IsSuccess, string Message)> TPermanentDeleteBannerAsync(int id);
        Task<List<ResultSecondaryBannerDto>> TGetActiveSecondaryBannersAsync();
        Task<List<ResultSecondaryBannerDto>> TGetDeletedSecondaryBannersAsync();
        Task<bool> ToggleSecondaryBannerStatusAsync(int secondaryBannerId, bool newStatus);
    }
}
