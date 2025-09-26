using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.Developer.DeveloperProfileDtos;
using CleaningSuppliesSystem.Entity.Entities;
using FluentValidation;
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
    public class DeveloperProfileManager(UserManager<AppUser> _userManager, IHttpContextAccessor _httpContextAccessor, IDeveloperProfileRepository _developerProfileRepository, IMapper _mapper) : IDeveloperProfileService
    {
        public async Task<DeveloperProfileDto?> TGetDeveloperProfileAsync()
        {
            // Giriş yapan kullanıcıyı al
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            if (user == null) return null;
            return _mapper.Map<DeveloperProfileDto>(user);
        }

        public async Task<UpdateDeveloperProfileDto?> TGetUpdateDeveloperProfileAsync()
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            if (user == null)
                return null;

            return _mapper.Map<UpdateDeveloperProfileDto>(user);
        }
        public async Task<(bool IsSuccess, string Message, int UpdatedId)> TUpdateDeveloperProfileAsync(UpdateDeveloperProfileDto dto)
        {
            var validator = new UpdateDeveloperProfileValidator();
            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                var message = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
                return (false, message, 0);
            }

            // Mevcut kullanıcıyı getir
            var user = await _userManager.FindByIdAsync(dto.Id.ToString());
            if (user == null)
                return (false, "Geliştirici profili bulunamadı.", 0);

            // DTO'dan AppUser entity'e maple
            _mapper.Map(dto, user);

            // Güncelle
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
                return (false, errors, 0);
            }

            return (true, "Geliştirici profili başarıyla güncellendi.", user.Id);
        }

    }
}
