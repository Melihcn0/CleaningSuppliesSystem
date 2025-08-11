using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.DataAccess.Concrete;
using CleaningSuppliesSystem.DTO.DTOs.Home.ServiceDtos;
using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CleaningSuppliesSystem.Business.Validators.Validators;

namespace CleaningSuppliesSystem.Business.Concrete
{
    public class ServiceManager : GenericManager<Service>, IServiceService
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IMapper _mapper;

        public ServiceManager(IRepository<Service> repository, IServiceRepository ServiceRepository, IMapper mapper)
        : base(repository)
        {
            _serviceRepository = ServiceRepository;
            _mapper = mapper;
        }
        public async Task<(bool IsSuccess, string Message, int CreatedId)> TCreateServiceAsync(CreateServiceDto createServiceDto)
        {
            var validator = new CreateServiceValidator();
            var validationResult = await validator.ValidateAsync(createServiceDto);
            if (!validationResult.IsValid)
            {
                var message = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
                return (false, message, 0);
            }

            var service = _mapper.Map<Service>(createServiceDto);
            service.CreatedDate = DateTime.Now;

            if (createServiceDto.IsShown)
            {
                var allServices = await _serviceRepository.GetAllAsync();
                int shownCount = allServices.Count(x => x.IsShown && !x.IsDeleted);

                if (shownCount >= 5)
                {
                    service.IsShown = false;
                    await _serviceRepository.CreateAsync(service);
                    return (true, "Hizmet oluşturuldu ancak gösterime alınmadı (maksimum 5 gösterim sınırı).", service.Id);
                }
            }

            service.IsShown = createServiceDto.IsShown;
            await _serviceRepository.CreateAsync(service);
            return (true, "Hizmet başarıyla oluşturuldu.", service.Id);
        }



        public async Task<(bool IsSuccess, string Message, int UpdatedId)> TUpdateServiceAsync(UpdateServiceDto updateServiceDto)
        {
            var validator = new UpdateServiceValidator();
            var validationResult = await validator.ValidateAsync(updateServiceDto);
            if (!validationResult.IsValid)
            {
                var message = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
                return (false, message, 0);
            }

            var Service = await _serviceRepository.GetByIdAsync(updateServiceDto.Id);
            if (Service == null)
                return (false, "Hizmet bulunamadı.", 0);

            _mapper.Map(updateServiceDto, Service);
            Service.UpdatedDate = DateTime.Now;
            await _serviceRepository.UpdateAsync(Service);
            return (true, "Hizmet başarıyla güncellendi.", Service.Id);
        }

        public async Task<(bool IsSuccess, string Message, int SoftDeletedId)> TSoftDeleteServiceAsync(int id)
        {
            var service = await _serviceRepository.GetByIdAsync(id);
            if (service == null)
                return (false, "Hizmet bulunamadı.", 0);

            if (service.IsDeleted)
                return (false, "Hizmet zaten silinmiş.", service.Id);

            if (service.IsShown)
                return (false, "Hizmet gösterimde silinemez.", 0);

            service.DeletedDate = DateTime.Now;
            service.IsDeleted = true;

            await _serviceRepository.UpdateAsync(service);
            return (true, "Hizmet başarıyla silindi.", service.Id);
        }


        public async Task<(bool IsSuccess, string Message, int UndoSoftDeletedId)> TUndoSoftDeleteServiceAsync(int id)
        {
            var Service = await _serviceRepository.GetByIdAsync(id);
            if (Service == null)
                return (false, "Hizmet bulunamadı.", 0);

            if (!Service.IsDeleted)
                return (false, "Hizmet zaten aktif.", Service.Id);

            Service.DeletedDate = null;
            Service.IsDeleted = false;
            await _serviceRepository.UpdateAsync(Service);
            return (true, "Hizmet başarıyla geri getirildi.", Service.Id);
        }
        public async Task<(bool IsSuccess, string Message)> TPermanentDeleteServiceAsync(int id)
        {
            var Service = await _serviceRepository.GetByIdAsync(id);
            if (Service == null)
                return (false, "Hizmet bulunamadı.");

            if (!Service.IsDeleted)
                return (false, "Hizmet soft silinmemiş. Önce soft silmeniz gerekir.");

            await _serviceRepository.DeleteAsync(Service.Id);
            return (true, "Hizmet kalıcı olarak silindi.");
        }
        public async Task<List<ResultServiceDto>> TGetActiveServicesAsync()
        {
            var entities = await _serviceRepository.GetActiveServicesAsync();
            return _mapper.Map<List<ResultServiceDto>>(entities);
        }
        public async Task<List<ResultServiceDto>> TGetDeletedServicesAsync()
        {
            var entities = await _serviceRepository.GetDeletedServicesAsync();
            return _mapper.Map<List<ResultServiceDto>>(entities);
        }

        public async Task<(bool IsSuccess, string Message)> SetAsShownAsync(int id)
        {
            var service = await _serviceRepository.GetByIdAsync(id);
            if (service == null)
                return (false, "Hizmet bulunamadı.");

            if (service.IsShown)
                return (false, "Bu hizmet zaten aktif durumda.");

            var allServices = await _serviceRepository.GetAllAsync();
            int activeCount = allServices.Count(s => s.IsShown && !s.IsDeleted);

            if (activeCount >= 5)
                return (false, "En fazla 5 hizmet gösterimde olabilir. Lütfen başka bir hizmeti pasif hale getirin.");

            service.IsShown = true;
            await _serviceRepository.UpdateAsync(service);

            return (true, "Hizmet başarıyla aktif hale getirildi.");
        }
        public async Task<(bool IsSuccess, string Message)> ToggleServiceStatusAsync(int serviceId, bool newStatus)
        {
            // Servisi veritabanından getir
            var service = await _serviceRepository.GetByIdAsync(serviceId);
            if (service == null)
                return (false, "Hizmet bulunamadı.");

            // Eğer hizmet aktif ediliyorsa aktif hizmet sayısı limiti kontrolü yap
            if (newStatus)
            {
                var activeServices = await _serviceRepository.GetAllAsync();
                var activeCount = activeServices.Count(s => s.IsShown && !s.IsDeleted);
                if (activeCount >= 5)
                    return (false, "Aktif hizmet sayısı 5'i geçemez.");

            }

            // Hizmetin aktif/pasif durumunu güncelle
            service.IsShown = newStatus;

            // Güncellemeyi kaydet
            await _serviceRepository.UpdateAsync(service);

            return (true, "Hizmet durumu başarıyla güncellendi.");
        }


        public async Task<int> TGetActiveServiceCountAsync()
        {
            var allServices = await _serviceRepository.GetAllAsync();
            return allServices.Count(s => s.IsShown && !s.IsDeleted);
        }



    }
}
