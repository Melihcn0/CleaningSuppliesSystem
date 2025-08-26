using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.Admin.CompanyAddresDtos;
using CleaningSuppliesSystem.DTO.DTOs.Customer.AdminProfileDtos;
using CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.AdminProfileValidatorDto;
using CleaningSuppliesSystem.Entity.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Business.Concrete
{
    public class AdminProfileManager : IAdminProfileService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ICompanyAddressRepository _companyAddressRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public AdminProfileManager(UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor, ICompanyAddressRepository companyAddressRepository, IMapper mapper)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _companyAddressRepository = companyAddressRepository;
            _mapper = mapper;
        }

        public async Task<AdminProfileDto?> TGetAdminProfileAsync()
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            if (user == null)
                return null;

            return _mapper.Map<AdminProfileDto>(user);
        }
        public async Task<UpdateAdminProfileDto?> TGetUpdateAdminProfileAsync()
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            if (user == null)
                return null;

            return _mapper.Map<UpdateAdminProfileDto>(user);
        }
        public async Task<(bool IsSuccess, string Message, int UpdatedId)> TUpdateAdminProfileAsync(UpdateAdminProfileDto dto)
        {
            // Validator ile kontrol
            var validator = new UpdateAdminProfileValidator();
            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                var message = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
                return (false, message, 0);
            }

            // Mevcut kullanıcıyı getir
            var user = await _userManager.FindByIdAsync(dto.Id.ToString());
            if (user == null)
                return (false, "Yetkili profili bulunamadı.", 0);

            // DTO'dan AppUser entity'e maple
            _mapper.Map(dto, user);

            // Güncelle
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
                return (false, errors, 0);
            }

            return (true, "Yetkili profili başarıyla güncellendi.", user.Id);
        }

    }
}
