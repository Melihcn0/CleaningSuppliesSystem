using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.DataAccess.Concrete;
using CleaningSuppliesSystem.DTO.DTOs.BrandDtos;
using CleaningSuppliesSystem.DTO.DTOs.FinanceDtos;
using CleaningSuppliesSystem.DTO.DTOs.LocationDtos;
using CleaningSuppliesSystem.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CleaningSuppliesSystem.Business.Validators.Validators;

namespace CleaningSuppliesSystem.Business.Concrete
{
    public class LocationCityManager : GenericManager<LocationCity>, ILocationCityService
    {
        private readonly ILocationCityRepository _locationCityRepository;
        private readonly ILocationDistrictRepository _locationDistrictRepository;
        private readonly IMapper _mapper;
        public LocationCityManager(IRepository<LocationCity> repository, ILocationCityRepository locationCityRepository, ILocationDistrictRepository locationDistrictRepository, IMapper mapper) : base(repository)
        {
            _locationCityRepository = locationCityRepository;
            _locationDistrictRepository = locationDistrictRepository;
            _mapper = mapper;
        }
        public async Task<(bool IsSuccess, string Message, int CreatedId)> TCreateLocationCityAsync(CreateLocationCityDto createLocationCityDto)
        {
            var validator = new CreateLocationCityValidator();
            var validationResult = await validator.ValidateAsync(createLocationCityDto);
            if (!validationResult.IsValid)
            {
                var message = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
                return (false, message, 0);
            }
            var LocationCity = _mapper.Map<LocationCity>(createLocationCityDto);
            LocationCity.CreatedDate = DateTime.Now;
            await _locationCityRepository.CreateAsync(LocationCity);
            return (true, "Şehir lokasyonu başarıyla oluşturuldu.", LocationCity.CityId);
        }

        public async Task<(bool IsSuccess, string Message, int SoftDeletedId)> TSoftDeleteLocationCityAsync(int id)
        {
            var locationCity = await _locationCityRepository.GetByIdAsync(id);
            if (locationCity == null)
                return (false, "Şehir lokasyonu bulunamadı.", 0);

            if (locationCity.IsDeleted)
                return (false, "Şehir lokasyonu zaten silinmiş.", locationCity.CityId);

            // Ürünlerde kullanım kontrolü (varsa)
            var isUsedInDistricts = await _locationDistrictRepository.AnyAsync(x => x.CityId == id);
            if (isUsedInDistricts)
                return (false, "Bu şehir lokasyonu bazı ilçe lokasyonlarında kullanılıyor. Soft silme yapılamaz.", locationCity.CityId);

            locationCity.DeletedDate = DateTime.Now;
            locationCity.IsDeleted = true;
            await _locationCityRepository.UpdateAsync(locationCity);

            return (true, "Şehir lokasyonu başarıyla soft silindi.", locationCity.CityId);
        }

        public async Task<(bool IsSuccess, string Message, int UndoSoftDeletedId)> TUndoSoftDeleteLocationCityAsync(int id)
        {
            var LocationCity = await _locationCityRepository.GetByIdAsync(id);
            if (LocationCity == null)
                return (false, "Şehir lokasyonu bulunamadı.", 0);

            if (!LocationCity.IsDeleted)
                return (false, "Şehir lokasyonu zaten aktif.", LocationCity.CityId);

            // Aynı isimde aktif başka marka var mı?
            var isDuplicate = await _locationCityRepository.AnyAsync(x =>
                x.CityName == LocationCity.CityName &&
                x.IsDeleted == false &&
                x.CityId != LocationCity.CityId);
            if (isDuplicate)
                return (false, $"'{LocationCity.CityName}' isminde aktif bir şehir lokasyonu mevcut. Geri alma yapılamadı.", LocationCity.CityId);

            LocationCity.DeletedDate = null;
            LocationCity.IsDeleted = false;
            await _locationCityRepository.UpdateAsync(LocationCity);
            return (true, "Şehir lokasyonu başarıyla geri getirildi.", LocationCity.CityId);
        }

        public async Task<(bool IsSuccess, string Message)> TPermanentDeleteLocationCityAsync(int id)
        {
            var LocationCity = await _locationCityRepository.GetByIdAsync(id);
            if (LocationCity == null)
                return (false, "Şehir lokasyonu bulunamadı.");

            if (!LocationCity.IsDeleted)
                return (false, "Şehir lokasyonu soft silinmemiş. Önce soft silmeniz gerekir.");

            await _locationCityRepository.DeleteAsync(LocationCity.CityId);
            return (true, "Şehir lokasyonu kalıcı olarak silindi.");
        }

        public async Task<List<ResultLocationCityDto>> TGetActiveLocationCitysAsync()
        {
            var entities = await _locationCityRepository.GetActiveLocationCitiesAsync();
            return _mapper.Map<List<ResultLocationCityDto>>(entities);
        }

        public async Task<List<ResultLocationCityDto>> TGetDeletedLocationCitysAsync()
        {
            var entities = await _locationCityRepository.GetDeletedLocationCitiesAsync();
            return _mapper.Map<List<ResultLocationCityDto>>(entities);
        }
        public async Task<List<ResultLocationCityWithLocationDistrictDto>> TGetLocationCityWithLocationDistrictAsync()
        {
            var entities = await _locationCityRepository.GetLocationCityWithLocationDistrictAsync();
            return _mapper.Map<List<ResultLocationCityWithLocationDistrictDto>>(entities);
        }
    }
}
