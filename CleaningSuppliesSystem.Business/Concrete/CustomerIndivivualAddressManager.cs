using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerIndivivualDtos;
using CleaningSuppliesSystem.Entity.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Business.Concrete
{
    public class CustomerIndivivualAddressManager : ICustomerIndivivualAddressService
    {
        private readonly ICustomerIndivivualAddressRepository _customerAddressRepository;
        private readonly IMapper _mapper;

        public CustomerIndivivualAddressManager(ICustomerIndivivualAddressRepository customerAddressRepository, IMapper mapper)
        {
            _customerAddressRepository = customerAddressRepository;
            _mapper = mapper;
        }

        public async Task<List<CustomerIndivivualAddressDto>> TGetAllAddressesAsync(int userId)
        {
            var addresses = await _customerAddressRepository.GetFilteredListAsync(a => a.AppUserId == userId);
            return _mapper.Map<List<CustomerIndivivualAddressDto>>(addresses);
        }

        public async Task<CustomerIndivivualAddressDto> TGetAddressByIdAsync(int id)
        {
            var address = await _customerAddressRepository.GetByFilterAsync(a => a.Id == id);
            if (address == null) return null;

            return _mapper.Map<CustomerIndivivualAddressDto>(address);
        }

        public async Task<(bool IsSuccess, string Message, int CreatedId)> TCreateCustomerIndivivualAddressAsync(CreateCustomerIndivivualAddressDto createDto)
        {
            // Mevcut adres sayısını kontrol et
            var existingAddresses = await _customerAddressRepository.GetFilteredListAsync(a => a.AppUserId == createDto.AppUserId);
            if (existingAddresses.Count >= 5)
                return (false, "Bir müşteri en fazla 5 adres ekleyebilir.", 0);

            // DTO -> Entity
            var entity = _mapper.Map<CustomerIndivivualAddress>(createDto);
            await _customerAddressRepository.CreateAsync(entity);

            return (true, "Adres başarıyla eklendi.", entity.Id);
        }

        public async Task<(bool IsSuccess, string Message, int UpdatedId)> TUpdateCustomerIndivivualAddressAsync(UpdateCustomerIndivivualAddressDto updateDto)
        {
            var entity = await _customerAddressRepository.GetByFilterAsync(a => a.Id == updateDto.Id);
            if (entity == null)
                return (false, "Adres bulunamadı.", 0);

            // DTO -> Entity mapping
            _mapper.Map(updateDto, entity);
            await _customerAddressRepository.UpdateAsync(entity);

            return (true, "Adres başarıyla güncellendi.", entity.Id);
        }

        public async Task<bool> SetAsDefaultAsync(int addressId)
        {
            var address = await _customerAddressRepository.GetByIdAsync(addressId);
            if (address == null) return false;

            // Aynı kullanıcıya ait diğer default adresleri kapat
            var otherAddresses = await _customerAddressRepository.GetFilteredListAsync(
                x => x.AppUserId == address.AppUserId && x.Id != addressId && x.IsDefault
            );

            foreach (var addr in otherAddresses)
            {
                addr.IsDefault = false;
                await _customerAddressRepository.UpdateAsync(addr);
            }

            address.IsDefault = true;
            await _customerAddressRepository.UpdateAsync(address);

            return true;
        }

        public async Task<bool> TToggleCustomerIndivivualAddressStatusAsync(int addressId, bool newStatus)
        {
            if (newStatus)
                return await SetAsDefaultAsync(addressId);

            return false;
        }
    }
}
