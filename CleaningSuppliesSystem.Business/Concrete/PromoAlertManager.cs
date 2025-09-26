using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.DataAccess.Concrete;
using CleaningSuppliesSystem.DTO.DTOs.Home.BannerDtos;
using CleaningSuppliesSystem.DTO.DTOs.Home.PromoAlertDtos;
using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CleaningSuppliesSystem.Business.Validators.Validators;

namespace CleaningSuppliesSystem.Business.Concrete
{
    public class PromoAlertManager : GenericManager<PromoAlert>, IPromoAlertService
    {
        private readonly IPromoAlertRepository _promoAlertRepository;
        private readonly IMapper _mapper;

        public PromoAlertManager(IRepository<PromoAlert> repository, IPromoAlertRepository promoAlertRepository, IMapper mapper) : base(repository)
        {
            _promoAlertRepository = promoAlertRepository;
            _mapper = mapper;
        }
        public async Task<(bool IsSuccess, string Message, int CreatedId)> TCreatePromoAlertAsync(CreatePromoAlertDto createPromoAlertDto)
        {
            var validator = new CreatePromoAlertValidator();
            var validationResult = await validator.ValidateAsync(createPromoAlertDto);
            if (!validationResult.IsValid)
            {
                var message = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
                return (false, message, 0);
            }
            var promoAlert = _mapper.Map<PromoAlert>(createPromoAlertDto);
            promoAlert.CreatedDate = DateTime.Now;
            promoAlert.IsShown = false;
            await _promoAlertRepository.CreateAsync(promoAlert);
            return (true, "Ön gösterim bildirimi başarıyla oluşturuldu.", promoAlert.Id);
        }

        public async Task<(bool IsSuccess, string Message, int UpdatedId)> TUpdatePromoAlertAsync(UpdatePromoAlertDto updatePromoAlertDto)
        {
            var validator = new UpdatePromoAlertValidator();
            var validationResult = await validator.ValidateAsync(updatePromoAlertDto);

            if (!validationResult.IsValid)
            {
                var message = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
                return (false, message, 0);
            }
            var promoAlert = await _promoAlertRepository.GetByIdAsync(updatePromoAlertDto.Id);
            if (promoAlert == null)
                return (false, "Ön gösterim bulunamadı.", 0);

            _mapper.Map(updatePromoAlertDto, promoAlert);
            promoAlert.UpdatedDate = DateTime.Now;
            await _promoAlertRepository.UpdateAsync(promoAlert);
            return (true, "Ön gösterim başarıyla güncellendi.", promoAlert.Id);
        }
        public async Task<(bool IsSuccess, string Message)> TPermanentDeletePromoAlertAsync(int id)
        {
            var promoAlert = await _promoAlertRepository.GetByIdAsync(id);
            if (promoAlert == null)
                return (false, "Ön gösterim bulunamadı.");

            if (promoAlert.IsShown)
                return (false, "Ön gösterim şu an gösterimde. Silmeniz için gösterim durumunu değiştirin.");

            await _promoAlertRepository.DeleteAsync(promoAlert.Id);
            return (true, "Ön gösterim kalıcı olarak silindi.");
        }
        public async Task<(bool IsSuccess, string Message)> SetAsShownAsync(int id)
        {
            var promoAlert = await _promoAlertRepository.GetByIdAsync(id);
            if (promoAlert == null)
                return (false, "Ön gösterim bulunamadı.");

            if (promoAlert.IsShown)
                return (false, "Bu ön gösterim zaten aktif durumda.");

            var allPromoAlerts = await _promoAlertRepository.GetListAsync();
            var activePromoAlerts = allPromoAlerts
                .Where(x => x.IsShown && x.Id != id)
                .ToList();

            foreach (var b in activePromoAlerts)
            {
                b.IsShown = false;
                await _promoAlertRepository.UpdateAsync(b);
            }

            promoAlert.IsShown = true;
            await _promoAlertRepository.UpdateAsync(promoAlert);

            return (true, "Ön gösterim başarıyla aktif hale getirildi.");
        }

        public async Task<bool> TTogglePromoAlertStatusAsync(int promoAlertId, bool newStatus)
        {
            var promoAlert = await _promoAlertRepository.GetByIdAsync(promoAlertId);
            if (promoAlert == null)
                return false;

            if (newStatus)
            {
                var allPromoAlerts = await _promoAlertRepository.GetListAsync();
                var shownPromoAlerts = allPromoAlerts.Where(b => b.IsShown && b.Id != promoAlertId).ToList();

                foreach (var b in shownPromoAlerts)
                {
                    b.IsShown = false;
                    await _promoAlertRepository.UpdateAsync(b);
                }
            }

            promoAlert.IsShown = newStatus;
            await _promoAlertRepository.UpdateAsync(promoAlert);

            return true;
        }
        public async Task<List<ResultPromoAlertDto>> TGetAllPromoAlertAsync()
        {
            var entities = await _promoAlertRepository.GetAllPromoAlertAsync();
            return _mapper.Map<List<ResultPromoAlertDto>>(entities);
        }

        public async Task<ResultPromoAlertDto> TGetFirstPromoAlertAsync()
        {
            var entities = await _promoAlertRepository.GetFirstPromoAlertAsync();
            return _mapper.Map<ResultPromoAlertDto>(entities);
        }

    }
}
