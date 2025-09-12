using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.DataAccess.Concrete;
using CleaningSuppliesSystem.DTO.DTOs.LocationDtos;
using CleaningSuppliesSystem.DTO.DTOs.SubCategoryDtos;
using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CleaningSuppliesSystem.Business.Validators.Validators;

namespace CleaningSuppliesSystem.Business.Concrete
{
    public class LocationDistrictManager : GenericManager<LocationDistrict>, ILocationDistrictService
    {
        private readonly ILocationDistrictRepository _locationDistrictRepository;
        private readonly ILocationCityRepository _locationCityRepository;
        private readonly IMapper _mapper;
        public LocationDistrictManager(IRepository<LocationDistrict> repository, ILocationDistrictRepository locationDistrictRepository, ILocationCityRepository locationCityRepository, IMapper mapper) : base(repository)
        {
            _locationDistrictRepository = locationDistrictRepository;
            _locationCityRepository = locationCityRepository;
            _mapper = mapper;
        }
        public async Task<(bool IsSuccess, string Message, int CreatedId)> TCreateLocationDistrictAsync(CreateLocationDistrictDto createLocationDistrictDto)
        {
            var validator = new CreateLocationDistrictValidator();
            var validationResult = await validator.ValidateAsync(createLocationDistrictDto);
            if (!validationResult.IsValid)
            {
                var message = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
                return (false, message, 0);
            }
            var LocationDistrict = _mapper.Map<LocationDistrict>(createLocationDistrictDto);
            LocationDistrict.CreatedDate = DateTime.Now;
            await _locationDistrictRepository.CreateAsync(LocationDistrict);
            return (true, "İlçe lokasyonu başarıyla oluşturuldu.", LocationDistrict.DistrictId);
        }

        public async Task<(bool IsSuccess, string Message, int SoftDeletedId)> TSoftDeleteLocationDistrictAsync(int id)
        {
            var LocationDistrict = await _locationDistrictRepository.GetByIdAsync(id);
            if (LocationDistrict == null)
                return (false, "İlçe lokasyonu bulunamadı.", 0);

            if (LocationDistrict.IsDeleted)
                return (false, "İlçe lokasyonu zaten silinmiş.", LocationDistrict.DistrictId);

            // Ürünlerde kullanım kontrolü (varsa)
            var isUsedInDistricts = await _locationCityRepository.AnyAsync(x => x.CityId == id);
            if (isUsedInDistricts)
                return (false, "Bu şehir lokasyonu bazı ilçe lokasyonlarında kullanılıyor. Silme yapılamaz.", LocationDistrict.DistrictId);

            LocationDistrict.DeletedDate = DateTime.Now;
            LocationDistrict.IsDeleted = true;
            await _locationDistrictRepository.UpdateAsync(LocationDistrict);
            return (true, "İlçe lokasyonu çöp kutusuna başarıyla taşındı.", LocationDistrict.DistrictId);
        }

        public async Task<(bool IsSuccess, string Message, int UndoSoftDeletedId)> TUndoSoftDeleteLocationDistrictAsync(int id)
        {
            var LocationDistrict = await _locationDistrictRepository.GetByIdAsync(id);
            if (LocationDistrict == null)
                return (false, "İlçe lokasyonu bulunamadı.", 0);

            if (!LocationDistrict.IsDeleted)
                return (false, "İlçe lokasyonu zaten aktif.", LocationDistrict.DistrictId);

            // Aynı isimde aktif başka ilçe var mı?
            var isDuplicate = await _locationDistrictRepository.AnyAsync(x =>
                x.DistrictName == LocationDistrict.DistrictName &&
                x.IsDeleted == false &&
                x.DistrictId != LocationDistrict.DistrictId);
            if (isDuplicate)
                return (false, $"'{LocationDistrict.DistrictName}' isminde aktif bir ilçe lokasyonu mevcut. Geri alma yapılamadı.", LocationDistrict.DistrictId);

            LocationDistrict.DeletedDate = null;
            LocationDistrict.IsDeleted = false;
            await _locationDistrictRepository.UpdateAsync(LocationDistrict);
            return (true, "İlçe lokasyonu çöp kutusundan başarıyla geri getirildi.", LocationDistrict.DistrictId);
        }

        public async Task<(bool IsSuccess, string Message)> TPermanentDeleteLocationDistrictAsync(int id)
        {
            var LocationDistrict = await _locationDistrictRepository.GetByIdAsync(id);
            if (LocationDistrict == null)
                return (false, "İlçe lokasyonu bulunamadı.");

            if (!LocationDistrict.IsDeleted)
                return (false, "İlçe lokasyonu silinmemiş. Önce silmeniz gerekir.");

            await _locationDistrictRepository.DeleteAsync(LocationDistrict.DistrictId);
            return (true, "İlçe lokasyonu çöp kutusundan kalıcı olarak silindi.");
        }

        public async Task<List<ResultLocationDistrictDto>> TGetDeletedLocationDistrictsAsync()
        {
            var LocationDistricts = await _locationDistrictRepository.GetDeletedLocationDistrictsAsync();

            return LocationDistricts.Select(b => new ResultLocationDistrictDto
            {
                Id = b.DistrictId,
                DistrictName = b.DistrictName,
                CreatedDate = b.CreatedDate,
                DeletedDate = b.DeletedDate
            }).ToList();
        }
        public async Task<List<ResultLocationDistrictDto>> TGetActiveLocationDistrictsAsync()
        {
            var LocationDistricts = await _locationDistrictRepository.GetActiveLocationDistrictsAsync();

            return LocationDistricts.Select(b => new ResultLocationDistrictDto
            {
                Id = b.DistrictId,
                DistrictName = b.DistrictName,
                CreatedDate = b.CreatedDate,
                DeletedDate = b.DeletedDate
            }).ToList();
        }
        public async Task<List<ResultLocationDistrictDto>> TGetActiveByCityIdAsync(int cityId)
        {
            var entities = await _locationDistrictRepository.GetActiveByCityIdAsync(cityId);
            return _mapper.Map<List<ResultLocationDistrictDto>>(entities);
        }
    }
}
