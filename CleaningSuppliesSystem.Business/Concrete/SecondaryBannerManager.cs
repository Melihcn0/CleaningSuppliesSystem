using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.Home.SecondaryBannerDtos;
using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CleaningSuppliesSystem.Business.Validators.Validators;

namespace CleaningSuppliesSystem.Business.Concrete
{
    public class SecondaryBannerManager : GenericManager<SecondaryBanner>, ISecondaryBannerService
    {
        private readonly ISecondaryBannerRepository _secondaryBannerRepository;
        private readonly IMapper _mapper;

        public SecondaryBannerManager(IRepository<SecondaryBanner> repository, ISecondaryBannerRepository secondaryBannerRepository, IMapper mapper)
            : base(repository)
        {
            _secondaryBannerRepository = secondaryBannerRepository;
            _mapper = mapper;
        }

        public async Task<(bool IsSuccess, string Message, int CreatedId)> TCreateSecondaryBannerAsync(CreateSecondaryBannerDto createSecondaryBannerDto)
        {
            var validator = new CreateSecondaryBannerValidator();
            var validationResult = await validator.ValidateAsync(createSecondaryBannerDto);
            if (!validationResult.IsValid)
            {
                var message = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
                return (false, message, 0);
            }
            var secondaryBanner = _mapper.Map<SecondaryBanner>(createSecondaryBannerDto);
            secondaryBanner.CreatedDate = DateTime.Now;
            secondaryBanner.IsShown = false;
            await _secondaryBannerRepository.CreateAsync(secondaryBanner);
            return (true, "İkincil banner başarıyla oluşturuldu", secondaryBanner.Id);
        }

        public async Task<(bool IsSuccess, string Message, int UpdatedId)> TUpdateSecondaryBannerAsync(UpdateSecondaryBannerDto updateSecondaryBannerDto)
        {
            var validator = new UpdateSecondaryBannerValidator();
            var validationResult = await validator.ValidateAsync(updateSecondaryBannerDto);

            if (!validationResult.IsValid)
            {
                var message = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
                return (false, message, 0);
            }
            var secondaryBanner = await _secondaryBannerRepository.GetByIdAsync(updateSecondaryBannerDto.Id);
            if (secondaryBanner == null)
                return (false, "İkincil banner bulunamadı.", 0);

            _mapper.Map(updateSecondaryBannerDto, secondaryBanner);
            secondaryBanner.UpdatedDate = DateTime.Now;
            await _secondaryBannerRepository.UpdateAsync(secondaryBanner);
            return (true, "İkincil banner başarıyla güncellendi.", secondaryBanner.Id);
        }

        public async Task<(bool IsSuccess, string Message, int SoftDeletedId)> TSoftDeleteSecondaryBannerAsync(int id)
        {
            var secondaryBanner = await _secondaryBannerRepository.GetByIdAsync(id);
            if (secondaryBanner == null)
                return (false, "İkincil banner bulunamadı.", 0);

            if (secondaryBanner.IsDeleted)
                return (false, "İkincil banner zaten silinmiş.", secondaryBanner.Id);

            if (secondaryBanner.IsShown)
                return (false, "Şu anda gösterimde olan bir ikincil banner çöp kutusuna taşınamaz. Lütfen başka bir banneri aktif hale getirin.", secondaryBanner.Id);

            secondaryBanner.DeletedDate = DateTime.Now;
            secondaryBanner.IsDeleted = true;
            secondaryBanner.IsShown = false;
            await _secondaryBannerRepository.UpdateAsync(secondaryBanner);
            return (true, "İkincil banner başarıyla çöp kutusuna taşındı.", secondaryBanner.Id);
        }

        public async Task<(bool IsSuccess, string Message, int UndoSoftDeletedId)> TUndoSoftDeleteSecondaryBannerAsync(int id)
        {
            var secondaryBanner = await _secondaryBannerRepository.GetByIdAsync(id);
            if (secondaryBanner == null)
                return (false, "İkincil banner bulunamadı.", 0);

            if (!secondaryBanner.IsDeleted)
                return (false, "İkincil banner zaten aktif.", secondaryBanner.Id);

            secondaryBanner.DeletedDate = null;
            secondaryBanner.IsDeleted = false;
            await _secondaryBannerRepository.UpdateAsync(secondaryBanner);
            return (true, "İkincil banner başarıyla geri getirildi.", secondaryBanner.Id);
        }

        public async Task<(bool IsSuccess, string Message)> TPermanentDeleteSecondaryBannerAsync(int id)
        {
            var secondaryBanner = await _secondaryBannerRepository.GetByIdAsync(id);
            if (secondaryBanner == null)
                return (false, "İkincil banner bulunamadı.");

            if (!secondaryBanner.IsDeleted)
                return (false, "İkincil banner silinmemiş. Önce silmeniz gerekir.");

            await _secondaryBannerRepository.DeleteAsync(secondaryBanner.Id);
            return (true, "İkincil banner çöp kutusundan kalıcı olarak silindi.");
        }

        public async Task<List<ResultSecondaryBannerDto>> TGetActiveSecondaryBannersAsync()
        {
            var entities = await _secondaryBannerRepository.GetActiveSecondaryBannersAsync();
            return _mapper.Map<List<ResultSecondaryBannerDto>>(entities);
        }
        public async Task<List<ResultSecondaryBannerDto>> TGetDeletedSecondaryBannersAsync()
        {
            var entities = await _secondaryBannerRepository.GetDeletedSecondaryBannersAsync();
            return _mapper.Map<List<ResultSecondaryBannerDto>>(entities);
        }

        public async Task<(bool IsSuccess, string Message)> SetAsShownAsync(int id)
        {
            var secondaryBanner = await _secondaryBannerRepository.GetByIdAsync(id);
            if (secondaryBanner == null)
                return (false, "İkincil banner bulunamadı.");

            if (secondaryBanner.IsShown)
                return (false, "Bu İkincil banner zaten aktif durumda.");

            var allSecondaryBanners = await _secondaryBannerRepository.GetAllAsync();
            var activeSecondaryBanners = allSecondaryBanners
                .Where(x => x.IsShown && !x.IsDeleted && x.Id != id)
                .ToList();

            foreach (var sb in activeSecondaryBanners)
            {
                sb.IsShown = false;
                await _secondaryBannerRepository.UpdateAsync(sb);
            }

            secondaryBanner.IsShown = true;
            await _secondaryBannerRepository.UpdateAsync(secondaryBanner);

            return (true, "İkincil banner başarıyla aktif hale getirildi.");
        }


        public async Task<bool> ToggleSecondaryBannerStatusAsync(int secondaryBannerId, bool newStatus)
        {
            var secondaryBanner = await _secondaryBannerRepository.GetByIdAsync(secondaryBannerId);
            if (secondaryBanner == null)
                return false;

            if (newStatus)
            {
                var allSecondaryBanners = await _secondaryBannerRepository.GetAllAsync();
                var shownSecondaryBanners = allSecondaryBanners.Where(b => b.IsShown && b.Id != secondaryBannerId).ToList();

                foreach (var ssb in shownSecondaryBanners)
                {
                    ssb.IsShown = false;
                    await _secondaryBannerRepository.UpdateAsync(ssb);
                }
            }

            secondaryBanner.IsShown = newStatus;
            await _secondaryBannerRepository.UpdateAsync(secondaryBanner);

            return true;
        }


    }
}
