using CleaningSuppliesSystem.DTO.DTOs.Developer.DeveloperProfileDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Business.Abstract
{
    public interface IDeveloperProfileService
    {
        Task<DeveloperProfileDto> TGetDeveloperProfileAsync();
        Task<UpdateDeveloperProfileDto> TGetUpdateDeveloperProfileAsync();
        Task<(bool IsSuccess, string Message, int UpdatedId)> TUpdateDeveloperProfileAsync(UpdateDeveloperProfileDto updateCustomerProfileDto);
    }
}
