using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.DataAccess.Concrete;
using CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerIndivivualDtos;
using CleaningSuppliesSystem.DTO.DTOs.LocationDtos;
using CleaningSuppliesSystem.DTO.DTOs.SubCategoryDtos;
using CleaningSuppliesSystem.Entity.Entities;

namespace CleaningSuppliesSystem.Business.Concrete
{
    public class CustomerIndividualAddressManager : ICustomerIndividualAddressService
    {
        private readonly ICustomerIndividualAddressRepository _customerIndividualAddressRepository;
        private readonly ICustomerCorporateAddressRepository _customerCorporateAddressRepository;
        private readonly ILocationDistrictRepository _locationDistrictRepository;
        private readonly IMapper _mapper;

        public CustomerIndividualAddressManager(ICustomerIndividualAddressRepository customerIndividualAddressRepository, ICustomerCorporateAddressRepository customerCorporateAddressRepository, ILocationDistrictRepository locationDistrictRepository, IMapper mapper)
        {
            _customerIndividualAddressRepository = customerIndividualAddressRepository;
            _customerCorporateAddressRepository = customerCorporateAddressRepository;
            _locationDistrictRepository = locationDistrictRepository;
            _mapper = mapper;
        }

        public async Task<List<CustomerIndividualAddressDto>> TGetAllAddressesAsync(int userId)
        {
            var addresses = await _customerIndividualAddressRepository.GetFilteredListAsync(a => a.AppUserId == userId);
            return _mapper.Map<List<CustomerIndividualAddressDto>>(addresses);
        }

        public async Task<CustomerIndividualAddressDto> TGetAddressByIdAsync(int id)
        {
            var address = await _customerIndividualAddressRepository.GetByFilterAsync(a => a.Id == id);
            if (address == null) return null;

            return _mapper.Map<CustomerIndividualAddressDto>(address);
        }
        public async Task<List<ResultLocationDistrictDto>> TGetActiveByCityIdAsync(int cityId)
        {
            var entities = await _locationDistrictRepository.GetActiveByCityIdAsync(cityId);
            return _mapper.Map<List<ResultLocationDistrictDto>>(entities);
        }

        public async Task<(bool IsSuccess, string Message, int CreatedId)> TCreateCustomerIndividualAddressAsync(CreateCustomerIndividualAddressDto createDto)
        {
            // Kullanıcıya ait mevcut bireysel adresleri al
            var individualAddresses = await _customerIndividualAddressRepository.GetFilteredListAsync(a => a.AppUserId == createDto.AppUserId);

            // Kullanıcıya ait mevcut kurumsal adresleri al
            var corporateAddresses = await _customerCorporateAddressRepository.GetFilteredListAsync(a => a.AppUserId == createDto.AppUserId);

            // Toplam adres sayısını hesapla
            var totalAddresses = individualAddresses.Count + corporateAddresses.Count;

            if (totalAddresses >= 5)
                return (false, "Bir müşteri en fazla 5 adres ekleyebilir.", 0);

            // DTO -> Entity map
            var entity = _mapper.Map<CustomerIndividualAddress>(createDto);

            // Repository’ye Entity gönder
            await _customerIndividualAddressRepository.CreateAsync(entity);

            return (true, "Adres başarıyla eklendi.", entity.Id);
        }



        public async Task<(bool IsSuccess, string Message, int UpdatedId)> TUpdateCustomerIndividualAddressAsync(UpdateCustomerIndividualAddressDto updateDto)
        {
            var entity = await _customerIndividualAddressRepository.GetByFilterAsync(a => a.Id == updateDto.Id);
            if (entity == null)
                return (false, "Adres bulunamadı.", 0);

            // DTO -> Entity mapping
            _mapper.Map(updateDto, entity);
            await _customerIndividualAddressRepository.UpdateAsync(entity);

            return (true, "Adres başarıyla güncellendi.", entity.Id);
        }

        public async Task<bool> SetAsDefaultAsync(int addressId)
        {
            // Önce Individual kontrol et
            var individual = await _customerIndividualAddressRepository.GetByIdAsync(addressId);
            if (individual != null)
            {
                // Tüm bireysel adresleri pasifleştir
                var allIndividualAddresses = await _customerIndividualAddressRepository.GetFilteredListAsync(
                    x => x.AppUserId == individual.AppUserId && x.Id != addressId && x.IsDefault
                );
                foreach (var addr in allIndividualAddresses)
                {
                    addr.IsDefault = false;
                    await _customerIndividualAddressRepository.UpdateAsync(addr);
                }

                // Tüm kurumsal adresleri de pasifleştir
                var allCorporateAddresses = await _customerCorporateAddressRepository.GetFilteredListAsync(
                    x => x.AppUserId == individual.AppUserId && x.IsDefault
                );
                foreach (var addr in allCorporateAddresses)
                {
                    addr.IsDefault = false;
                    await _customerCorporateAddressRepository.UpdateAsync(addr);
                }

                // Seçilen bireyseli default yap
                individual.IsDefault = true;
                await _customerIndividualAddressRepository.UpdateAsync(individual);

                return true;
            }

            // Eğer Individual değilse Corporate kontrol et
            var corporate = await _customerCorporateAddressRepository.GetByIdAsync(addressId);
            if (corporate != null)
            {
                // Tüm kurumsal adresleri pasifleştir
                var allCorporateAddresses = await _customerCorporateAddressRepository.GetFilteredListAsync(
                    x => x.AppUserId == corporate.AppUserId && x.Id != addressId && x.IsDefault
                );
                foreach (var addr in allCorporateAddresses)
                {
                    addr.IsDefault = false;
                    await _customerCorporateAddressRepository.UpdateAsync(addr);
                }

                // Tüm bireysel adresleri de pasifleştir
                var allIndividualAddresses = await _customerIndividualAddressRepository.GetFilteredListAsync(
                    x => x.AppUserId == corporate.AppUserId && x.IsDefault
                );
                foreach (var addr in allIndividualAddresses)
                {
                    addr.IsDefault = false;
                    await _customerIndividualAddressRepository.UpdateAsync(addr);
                }

                // Seçilen kurumsalı default yap
                corporate.IsDefault = true;
                await _customerCorporateAddressRepository.UpdateAsync(corporate);

                return true;
            }

            return false; // Ne Individual ne Corporate bulundu
        }

        public async Task<bool> TToggleCustomerAddressStatusAsync(int addressId, bool newStatus)
        {
            if (newStatus)
            {
                return await SetAsDefaultAsync(addressId);
            }
            else
            {
                // Switch kapalıysa sadece ilgili adresi default olmaktan çıkar
                var individual = await _customerIndividualAddressRepository.GetByIdAsync(addressId);
                if (individual != null)
                {
                    individual.IsDefault = false;
                    await _customerIndividualAddressRepository.UpdateAsync(individual);
                    return true;
                }

                var corporate = await _customerCorporateAddressRepository.GetByIdAsync(addressId);
                if (corporate != null)
                {
                    corporate.IsDefault = false;
                    await _customerCorporateAddressRepository.UpdateAsync(corporate);
                    return true;
                }

                return false;
            }
        }

        public async Task<(bool IsSuccess, string Message)> TPermanentDeleteCustomerIndividualAsync(int id)
        {
            var address = await _customerIndividualAddressRepository.GetByIdAsync(id);
            if (address == null)
                return (false, "Bireysel adres bulunamadı.");

            if (address.IsDefault)
                return (false, "Varsayılan adres silinemez. Lütfen önce başka bir adresi varsayılan yapın.");

            await _customerIndividualAddressRepository.DeleteAsync(address.Id);
            return (true, "Bireysel adres kalıcı olarak silindi.");
        }

    }
}
