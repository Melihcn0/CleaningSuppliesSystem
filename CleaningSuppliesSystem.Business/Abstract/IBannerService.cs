using CleaningSuppliesSystem.DTO.DTOs.Home.BannerDtos;
using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Business.Abstract
{
    public interface IBannerService : IGenericService<Banner>
    {
        Task<(bool IsSuccess, string Message, int CreatedId)> TCreateBannerAsync(CreateBannerDto createBannerDto);
        Task<(bool IsSuccess, string Message, int UpdatedId)> TUpdateBannerAsync(UpdateBannerDto updateBannerDto);
        Task<(bool IsSuccess, string Message, int SoftDeletedId)> TSoftDeleteBannerAsync(int id);
        Task<(bool IsSuccess, string Message, int UndoSoftDeletedId)> TUndoSoftDeleteBannerAsync(int id);
        Task<(bool IsSuccess, string Message)> TPermanentDeleteBannerAsync(int id);
        Task<List<ResultBannerDto>> TGetActiveBannersAsync();
        Task<List<ResultBannerDto>> TGetDeletedBannersAsync();
        Task<bool> ToggleBannerStatusAsync(int bannerId, bool newStatus);
    }
}
