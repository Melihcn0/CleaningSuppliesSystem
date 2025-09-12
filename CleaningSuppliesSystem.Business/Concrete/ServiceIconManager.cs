using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.DataAccess.Concrete;
using CleaningSuppliesSystem.DTO.DTOs.Home.ServiceIconDtos;
using CleaningSuppliesSystem.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static CleaningSuppliesSystem.Business.Validators.Validators;

namespace CleaningSuppliesSystem.Business.Concrete
{
    public class ServiceIconManager : GenericManager<ServiceIcon>, IServiceIconService
    {
        private readonly IServiceIconRepository _serviceIconRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IMapper _mapper;

        public ServiceIconManager(IServiceIconRepository serviceIconRepository, IServiceRepository serviceRepository, IMapper mapper)
            : base(serviceIconRepository)
        {
            _serviceIconRepository = serviceIconRepository;
            _serviceRepository = serviceRepository;
            _mapper = mapper;
        }

        public async Task<(bool IsSuccess, string Message, int CreatedId)> TCreateServiceIconAsync(CreateServiceIconDto createDto)
        {
            var validator = new CreateServiceIconValidator();
            var validationResult = await validator.ValidateAsync(createDto);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
                return (false, errors, 0);
            }

            var entity = _mapper.Map<ServiceIcon>(createDto);
            entity.CreatedDate = DateTime.Now;
            entity.IsDeleted = false;
            entity.IsShown = true;

            await _serviceIconRepository.CreateAsync(entity);
            return (true, "Icon başarıyla oluşturuldu.", entity.Id);
        }

        public async Task<(bool IsSuccess, string Message, int UpdatedId)> TUpdateServiceIconAsync(UpdateServiceIconDto updateDto)
        {
            var validator = new UpdateServiceIconValidator();
            var validationResult = await validator.ValidateAsync(updateDto);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
                return (false, errors, 0);
            }

            var entity = await _serviceIconRepository.GetByIdAsync(updateDto.Id);
            if (entity == null)
                return (false, "Icon bulunamadı.", 0);

            _mapper.Map(updateDto, entity);
            entity.UpdatedDate = DateTime.Now;

            await _serviceIconRepository.UpdateAsync(entity);
            return (true, "Icon başarıyla güncellendi.", entity.Id);
        }

        public async Task<(bool IsSuccess, string Message)> TPermanentDeleteServiceIconAsync(int id)
        {
            var entity = await _serviceIconRepository.GetByIdAsync(id);
            if (entity == null)
                return (false, "İkon bulunamadı.");

            if (!entity.IsDeleted)
                return (false, "Önce silmeniz gerekir.");

            await _serviceIconRepository.DeleteAsync(id);
            return (true, "İkon çöp kutusundan kalıcı olarak silindi.");
        }

        public async Task<List<ResultServiceIconDto>> TGetActiveServiceIconsAsync()
        {
            var entities = await _serviceIconRepository.GetActiveServiceIconsAsync();
            return _mapper.Map<List<ResultServiceIconDto>>(entities);
        }

        public async Task<List<ResultServiceIconDto>> TGetDeletedServiceIconsAsync()
        {
            var entities = await _serviceIconRepository.GetDeletedServiceIconsAsync();
            return _mapper.Map<List<ResultServiceIconDto>>(entities);
        }

        public async Task<(bool IsSuccess, string Message, int SoftDeletedId)> TSoftDeleteServiceIconAsync(int id)
        {
            var entity = await _serviceIconRepository.GetByIdAsync(id);
            if (entity == null)
                return (false, "İkon bulunamadı.", 0);

            if (entity.IsDeleted)
                return (false, "İkon zaten silinmiş.", id);

            var isUsed = await _serviceRepository.AnyAsync(s => s.ServiceIconId == id && !s.IsDeleted);
            if (isUsed)
                return (false, "Bu ikon serviste kullanılıyor, silinemez.", id);

            entity.IsDeleted = true;
            entity.DeletedDate = DateTime.Now;
            await _serviceIconRepository.UpdateAsync(entity);

            return (true, "İkon çöp kutusuna başarıyla taşındı.", id);
        }

        public async Task<(bool IsSuccess, string Message, int UndoSoftDeletedId)> TUndoSoftDeleteServiceIconAsync(int id)
        {
            var entity = await _serviceIconRepository.GetByIdAsync(id);
            if (entity == null)
                return (false, "İkon bulunamadı.", 0);

            if (!entity.IsDeleted)
                return (false, "İkon zaten aktif.", id);

            entity.IsDeleted = false;
            entity.DeletedDate = null;
            await _serviceIconRepository.UpdateAsync(entity);
            return (true, "İkon çöp kutusundan başarıyla geri getirildi.", id);
        }

        public async Task<List<ResultServiceIconDto>> TGetUnusedActiveServiceIconsAsync()
        {
            var unusedIcons = await _serviceIconRepository.GetUnusedActiveIconsAsync();
            return _mapper.Map<List<ResultServiceIconDto>>(unusedIcons);
        }


    }
}
