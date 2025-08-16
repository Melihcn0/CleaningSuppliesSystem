using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.Entity.Entities;
using CleaningSuppliesSystem.DataAccess.Abstract;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerProfileDtos;

namespace CleaningSuppliesSystem.Business.Concrete
{
    public class CustomerAddressManager : ICustomerAddressService
    {
        private readonly ICustomerAddressRepository _customerAddressRepository;
        private readonly IMapper _mapper;

        public CustomerAddressManager(ICustomerAddressRepository customerAddressRepository, IMapper mapper)
        {
            _customerAddressRepository = customerAddressRepository;
            _mapper = mapper;
        }

        public async Task<List<CustomerAddressDto?>> TGetAllAddressesAsync(int userId)
        {
            var addresses = await _customerAddressRepository.GetFilteredListAsync(a => a.AppUserId == userId);
            return _mapper.Map<List<CustomerAddressDto?>>(addresses);
        }

        public async Task<CustomerAddressDto> TGetAddressByIdAsync(int id)
        {
            var address = await _customerAddressRepository.GetByFilterAsync(a => a.Id == id);
            if (address == null)
                return null;
            return _mapper.Map<CustomerAddressDto>(address);
        }

        public async Task<(bool IsSuccess, string Message, int CreatedId)> TCreateCustomerAddressAsync(CreateCustomerAddressDto createCustomerAddressDto)
        {
            var entity = _mapper.Map<CustomerAddress>(createCustomerAddressDto);
            await _customerAddressRepository.CreateAsync(entity);
            return (true, "Adres başarıyla eklendi.", entity.Id);
        }

        public async Task<(bool IsSuccess, string Message, int UpdatedId)> TUpdateCustomerAddressAsync(UpdateCustomerAddressDto updateCustomerAddressDto)
        {
            var entity = await _customerAddressRepository.GetByFilterAsync(a => a.Id == updateCustomerAddressDto.Id);
            if (entity == null)
                return (false, "Adres bulunamadı.", 0);

            _mapper.Map(updateCustomerAddressDto, entity);
            await _customerAddressRepository.UpdateAsync(entity);
            return (true, "Adres başarıyla güncellendi.", entity.Id);
        }
    }
}
