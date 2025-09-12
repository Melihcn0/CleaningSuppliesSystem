using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.Admin.CompanyAddressDtos;
using CleaningSuppliesSystem.Entity.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using static CleaningSuppliesSystem.Business.Validators.Validators;

namespace CleaningSuppliesSystem.Business.Concrete
{
    public class CompanyAddressManager : ICompanyAddressService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ICompanyAddressRepository _companyAddressRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public CompanyAddressManager(UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor, ICompanyAddressRepository companyAddressRepository, IMapper mapper)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _companyAddressRepository = companyAddressRepository;
            _mapper = mapper;
        }
        public async Task<CompanyAddressDto?> TGetCompanyAddressAsync()
        {
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            var userWithAddress = await _companyAddressRepository.GetUserWithCompanyAddressAsync(currentUser.Id);
            return _mapper.Map<CompanyAddressDto>(userWithAddress.CompanyAddress);
        }


        public async Task<UpdateCompanyAddressDto?> TGetUpdateCompanyAddressAsync()
        {
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            if (currentUser == null)
                return null;

            // Kullanıcıyı CompanyAddress dahil getir
            var userWithAddress = await _companyAddressRepository.GetUserWithCompanyAddressAsync(currentUser.Id);
            if (userWithAddress?.CompanyAddress == null)
                return null;

            return _mapper.Map<UpdateCompanyAddressDto>(userWithAddress.CompanyAddress);
        }


        public async Task<(bool IsSuccess, string Message, int UpdatedId)> TUpdateCompanyAddressAsync(UpdateCompanyAddressDto dto)
        {
            var validator = new UpdateCompanyAddressValidator();
            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
                return (false, string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage)), 0);

            // Mevcut entityyi getir
            var entity = await _companyAddressRepository.GetByIdAsync(dto.Id);

            if (entity == null)
            {
                // Yeni entity oluştur
                entity = _mapper.Map<CompanyAddress>(dto);

                // Id sıfırlanmalı
                entity.Id = 0;

                // FK kolonunu set et
                var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
                if (user == null)
                    return (false, "Yetkili kullanıcı bulunamadı.", 0);

                entity.AppUserId = user.Id;

                await _companyAddressRepository.CreateAsync(entity);
            }
            else
            {
                // Var olan entity'yi güncelle
                _mapper.Map(dto, entity);
                await _companyAddressRepository.UpdateAsync(entity);
            }

            return (true, "Şirket adresi başarıyla güncellendi.", entity.Id);
        }

    }
}
