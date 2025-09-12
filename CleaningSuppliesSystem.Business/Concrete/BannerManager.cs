using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.DataAccess.Concrete;
using CleaningSuppliesSystem.DTO.DTOs.BrandDtos;
using CleaningSuppliesSystem.DTO.DTOs.CategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.Home.BannerDtos;
using CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.Home.BannerDto;
using CleaningSuppliesSystem.Entity.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Business.Concrete
{
    public class BannerManager : GenericManager<Banner>, IBannerService
    {
        private readonly IBannerRepository _bannerRepository;
        private readonly IMapper _mapper;

        public BannerManager(IRepository<Banner> repository, IBannerRepository bannerRepository, IMapper mapper)
            : base(repository)
        {
            _bannerRepository = bannerRepository;
            _mapper = mapper;
        }

        public async Task<(bool IsSuccess, string Message, int CreatedId)> TCreateBannerAsync(CreateBannerDto createBannerDto)
        {
            var validator = new CreateBannerValidator();
            var validationResult = await validator.ValidateAsync(createBannerDto);
            if (!validationResult.IsValid)
            {
                var message = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
                return (false, message, 0);
            }
            var banner = _mapper.Map<Banner>(createBannerDto);
            banner.CreatedDate = DateTime.Now;
            banner.IsShown = false;
            await _bannerRepository.CreateAsync(banner);
            return (true, "Banner başarıyla oluşturuldu", banner.Id);
        }

        public async Task<(bool IsSuccess, string Message, int UpdatedId)> TUpdateBannerAsync(UpdateBannerDto updateBannerDto)
        {
            var validator = new UpdateBannerValidator();
            var validationResult = await validator.ValidateAsync(updateBannerDto);

            if (!validationResult.IsValid)
            {
                var message = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
                return (false, message, 0);
            }
            var banner = await _bannerRepository.GetByIdAsync(updateBannerDto.Id);
            if (banner == null)
                return (false, "Banner bulunamadı.", 0);

            _mapper.Map(updateBannerDto, banner);
            banner.UpdatedDate = DateTime.Now;
            await _bannerRepository.UpdateAsync(banner);
            return (true, "Banner başarıyla güncellendi.", banner.Id);
        }

        public async Task<(bool IsSuccess, string Message, int SoftDeletedId)> TSoftDeleteBannerAsync(int id)
        {
            var banner = await _bannerRepository.GetByIdAsync(id);
            if (banner == null)
                return (false, "Banner bulunamadı.", 0);

            if (banner.IsDeleted)
                return (false, "Banner zaten silinmiş.", banner.Id);

            if (banner.IsShown)
                return (false, "Şu anda gösterimde olan bir banner soft silinemez. Lütfen başka bir banneri aktif hale getirin.", banner.Id);

            banner.DeletedDate = DateTime.Now;
            banner.IsDeleted = true;
            banner.IsShown = false;
            await _bannerRepository.UpdateAsync(banner);
            return (true, "Banner başarıyla çöp kutusuna taşındı.", banner.Id);
        }

        public async Task<(bool IsSuccess, string Message, int UndoSoftDeletedId)> TUndoSoftDeleteBannerAsync(int id)
        {
            var banner = await _bannerRepository.GetByIdAsync(id);
            if (banner == null)
                return (false, "Banner bulunamadı.", 0);

            if (!banner.IsDeleted)
                return (false, "Banner zaten aktif.", banner.Id);

            banner.DeletedDate = null;
            banner.IsDeleted = false;
            await _bannerRepository.UpdateAsync(banner);
            return (true, "Banner çöp kutusundan başarıyla geri getirildi.", banner.Id);
        }

        public async Task<(bool IsSuccess, string Message)> TPermanentDeleteBannerAsync(int id)
        {
            var banner = await _bannerRepository.GetByIdAsync(id);
            if (banner == null)
                return (false, "Banner bulunamadı.");

            if (!banner.IsDeleted)
                return (false, "Banner silinmemiş. Önce silmeniz gerekir.");

            await _bannerRepository.DeleteAsync(banner.Id);
            return (true, "Banner çöp kutusundan kalıcı olarak silindi.");
        }

        public async Task<List<ResultBannerDto>> TGetActiveBannersAsync()
        {
            var entities = await _bannerRepository.GetActiveBannersAsync();
            return _mapper.Map<List<ResultBannerDto>>(entities);
        }
        public async Task<List<ResultBannerDto>> TGetDeletedBannersAsync()
        {
            var entities = await _bannerRepository.GetDeletedBannersAsync();
            return _mapper.Map<List<ResultBannerDto>>(entities);
        }
        public async Task<(bool IsSuccess, string Message)> SetAsShownAsync(int id)
        {
            var banner = await _bannerRepository.GetByIdAsync(id);
            if (banner == null)
                return (false, "Banner bulunamadı.");

            if (banner.IsShown)
                return (false, "Bu banner zaten aktif durumda.");

            var allBanners = await _bannerRepository.GetAllAsync();
            var activeBanners = allBanners
                .Where(x => x.IsShown && !x.IsDeleted && x.Id != id)
                .ToList();

            foreach (var b in activeBanners)
            {
                b.IsShown = false;
                await _bannerRepository.UpdateAsync(b);
            }

            banner.IsShown = true;
            await _bannerRepository.UpdateAsync(banner);

            return (true, "Banner başarıyla aktif hale getirildi.");
        }

        public async Task<bool> ToggleBannerStatusAsync(int bannerId, bool newStatus)
        {
            var banner = await _bannerRepository.GetByIdAsync(bannerId);
            if (banner == null)
                return false;

            if (newStatus)
            {
                var allBanners = await _bannerRepository.GetAllAsync();
                var shownBanners = allBanners.Where(b => b.IsShown && b.Id != bannerId).ToList();

                foreach (var b in shownBanners)
                {
                    b.IsShown = false;
                    await _bannerRepository.UpdateAsync(b);
                }
            }

            banner.IsShown = newStatus;
            await _bannerRepository.UpdateAsync(banner);

            return true;
        }


    }
}
