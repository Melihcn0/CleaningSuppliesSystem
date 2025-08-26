using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerProfileDtos;
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
    public class CustomerProfileManager : ICustomerProfileService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ICustomerProfileRepository _customerProfileRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public CustomerProfileManager(UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor, ICustomerProfileRepository customerProfileRepository, IMapper mapper)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _customerProfileRepository = customerProfileRepository;
            _mapper = mapper;
        }

        public async Task<CustomerProfileDto?> TGetProfileAsync()
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            if (user == null)
                return null;

            return _mapper.Map<CustomerProfileDto>(user);
        }
        public async Task<UpdateCustomerProfileDto?> TGetUpdateCustomerProfileAsync()
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            if (user == null)
                return null;

            return _mapper.Map<UpdateCustomerProfileDto>(user);
        }
        public async Task<(bool IsSuccess, string Message, int UpdatedId)> TUpdateCustomerProfileAsync(UpdateCustomerProfileDto updateCustomerProfileDto)
        {
            // Validator ile kontrol
            var validator = new UpdateCustomerProfileValidator();
            var validationResult = await validator.ValidateAsync(updateCustomerProfileDto);

            if (!validationResult.IsValid)
            {
                var message = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
                return (false, message, 0);
            }

            // Mevcut müşteri profiliyi veritabanından getir
            var customerProfile = await _customerProfileRepository.GetByIdAsync(updateCustomerProfileDto.Id);
            if (customerProfile == null)
                return (false, "Müşteri profili bulunamadı.", 0);

            // DTO'dan entity'e maple
            _mapper.Map(updateCustomerProfileDto, customerProfile);

            // Güncelle
            await _customerProfileRepository.UpdateAsync(customerProfile);

            return (true, "Müşteri profili başarıyla güncellendi.", customerProfile.Id);
        }



    }
}
