using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.DataAccess.Concrete;
using CleaningSuppliesSystem.DTO.DTOs.CategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerCorporateDtos;
using CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerIndivivualDtos;
using CleaningSuppliesSystem.Entity.Entities;

namespace CleaningSuppliesSystem.Business.Concrete
{
    public class CustomerCorporateAddressManager : ICustomerCorporateAddressService
    {
        private readonly ICustomerCorporateAddressRepository _customerCorporateAddressRepository;
        private readonly ICustomerIndividualAddressRepository _customerIndividualAddressRepository;
        private readonly IMapper _mapper;

        public CustomerCorporateAddressManager(ICustomerCorporateAddressRepository customerCorporateAddressRepository, ICustomerIndividualAddressRepository customerIndividualAddressRepository, IMapper mapper)
        {
            _customerCorporateAddressRepository = customerCorporateAddressRepository;
            _customerIndividualAddressRepository = customerIndividualAddressRepository;
            _mapper = mapper;
        }

        public async Task<List<CustomerCorporateAddressDto>> TGetAllAddressesAsync(int userId)
        {
            var addresses = await _customerCorporateAddressRepository.GetFilteredListAsync(a => a.AppUserId == userId);
            return _mapper.Map<List<CustomerCorporateAddressDto>>(addresses);
        }
        public async Task<CustomerCorporateAddressDto> TGetAddressByIdAsync(int id)
        {
            var address = await _customerCorporateAddressRepository.GetByFilterAsync(a => a.Id == id);
            if (address == null) return null;

            return _mapper.Map<CustomerCorporateAddressDto>(address);
        }

        public async Task<(bool IsSuccess, string Message, int CreatedId)> TCreateCustomerCorporateAddressAsync(CreateCustomerCorporateAddressDto createDto)
        {
            // Kullanıcının mevcut bireysel adreslerini al
            var individualAddresses = await _customerIndividualAddressRepository.GetFilteredListAsync(a => a.AppUserId == createDto.AppUserId);

            // Kullanıcının mevcut kurumsal adreslerini al
            var corporateAddresses = await _customerCorporateAddressRepository.GetFilteredListAsync(a => a.AppUserId == createDto.AppUserId);

            // Toplam adres sayısını hesapla
            var totalAddresses = individualAddresses.Count + corporateAddresses.Count;

            if (totalAddresses >= 5)
                return (false, "Bir müşteri en fazla 5 adres ekleyebilir.", 0);

            // DTO -> Entity map
            var entity = _mapper.Map<CustomerCorporateAddress>(createDto);

            // Repository’ye Entity gönder
            await _customerCorporateAddressRepository.CreateAsync(entity);

            return (true, "Adres başarıyla eklendi.", entity.Id);
        }


        public async Task<(bool IsSuccess, string Message, int UpdatedId)> TUpdateCustomerCorporateAddressAsync(UpdateCustomerCorporateAddressDto updateDto)
        {
            var entity = await _customerCorporateAddressRepository.GetByFilterAsync(a => a.Id == updateDto.Id);
            if (entity == null)
                return (false, "Adres bulunamadı.", 0);

            _mapper.Map(updateDto, entity);
            await _customerCorporateAddressRepository.UpdateAsync(entity);

            return (true, "Adres başarıyla güncellendi.", entity.Id);
        }

        public async Task<(bool IsSuccess, string Message)> TPermanentDeleteCustomerCorporateAsync(int id)
        {
            var address = await _customerCorporateAddressRepository.GetByIdAsync(id);
            if (address == null)
                return (false, "Kurumsal adres bulunamadı.");

            if (address.IsDefault)
                return (false, "Varsayılan adres silinemez. Lütfen önce başka bir adresi varsayılan yapın.");

            await _customerCorporateAddressRepository.DeleteAsync(address.Id);
            return (true, "Kurumsal adres kalıcı olarak silindi.");
        }
    }
}
