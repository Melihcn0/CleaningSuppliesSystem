using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.Admin.CompanyBankDtos;
using CleaningSuppliesSystem.Entity.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CleaningSuppliesSystem.Business.Validators.Validators;

namespace CleaningSuppliesSystem.Business.Concrete
{
    public class CompanyBankManager(UserManager<AppUser> _userManager, ICompanyBankRepository _companyBankRepository, IHttpContextAccessor _httpContextAccessor, IMapper _mapper) : ICompanyBankService
    {
        public async Task<CompanyBankDto?> TGetCompanyBankAsync()
        {
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            var userWithAddress = await _companyBankRepository.GetUserWithCompanyBankAsync(currentUser.Id);
            return _mapper.Map<CompanyBankDto>(userWithAddress.CompanyBank);
        }
        public async Task<UpdateCompanyBankDto?> TGetUpdateCompanyBankAsync()
        {
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            if (currentUser == null)
                return null;

            var userWithBank = await _companyBankRepository.GetUserWithCompanyBankAsync(currentUser.Id);
            if (userWithBank?.CompanyBank == null)
                return null;

            return _mapper.Map<UpdateCompanyBankDto>(userWithBank.CompanyBank);
        }
        public async Task<(bool IsSuccess, string Message, int UpdatedId)> TUpdateCompanyBankAsync(UpdateCompanyBankDto dto)
        {
            var validator = new UpdateCompanyBankValidator();
            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
                return (false, string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage)), 0);

            var entity = await _companyBankRepository.GetByIdAsync(dto.Id);

            if (entity == null)
            {
                entity = _mapper.Map<CompanyBank>(dto);
                entity.Id = 0;
                var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
                if (user == null)
                    return (false, "Yetkili kullanıcı bulunamadı.", 0);
                entity.AppUserId = user.Id;
                await _companyBankRepository.CreateAsync(entity);
            }
            else
            {
                _mapper.Map(dto, entity);
                await _companyBankRepository.UpdateAsync(entity);
            }

            return (true, "Şirket banka bilgisi başarıyla güncellendi.", entity.Id);
        }
        public async Task<CompanyBankDto?> TGetFirstCompanyBankAsync()
        {
            var bank = await _companyBankRepository.GetFirstCompanyBankAsync();
            return bank == null ? null : _mapper.Map<CompanyBankDto>(bank);
        }

    }
}