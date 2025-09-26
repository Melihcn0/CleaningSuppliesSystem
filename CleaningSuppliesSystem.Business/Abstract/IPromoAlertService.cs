using CleaningSuppliesSystem.DTO.DTOs.Home.BannerDtos;
using CleaningSuppliesSystem.DTO.DTOs.Home.PromoAlertDtos;
using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Business.Abstract
{
    public interface IPromoAlertService : IGenericService<PromoAlert>
    {
        Task<(bool IsSuccess, string Message, int CreatedId)> TCreatePromoAlertAsync(CreatePromoAlertDto createPromoAlertDto);
        Task<(bool IsSuccess, string Message, int UpdatedId)> TUpdatePromoAlertAsync(UpdatePromoAlertDto updatePromoAlertDto);
        Task<(bool IsSuccess, string Message)> TPermanentDeletePromoAlertAsync(int id);
        Task<bool> TTogglePromoAlertStatusAsync(int promoAlertId, bool newStatus);
        Task<List<ResultPromoAlertDto>> TGetAllPromoAlertAsync();
        Task<ResultPromoAlertDto> TGetFirstPromoAlertAsync();


    }
}
