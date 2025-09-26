using CleaningSuppliesSystem.DTO.DTOs.Admin.AdminProfileDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Business.Abstract
{
    public interface IAdminProfileService
    {
        Task<AdminProfileDto> TGetAdminProfileAsync();
        Task<UpdateAdminProfileDto> TGetUpdateAdminProfileAsync();
        Task<(bool IsSuccess, string Message, int UpdatedId)> TUpdateAdminProfileAsync(UpdateAdminProfileDto updateAdminProfileDto);
    }
}
