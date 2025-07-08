using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.FinanceDtos;
using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Business.Concrete
{
    public class FinanceManager : GenericManager<Finance>, IFinanceService
    {
        private readonly IFinanceRepository _financeRepository;
        private readonly IMapper _mapper;

        public FinanceManager(IRepository<Finance> repository, IFinanceRepository financeRepository, IMapper mapper)
        : base(repository)
        {
            _financeRepository = financeRepository;
            _mapper = mapper;
        }
        public async Task<(bool IsSuccess, string Message)> TCreateFinanceAsync(CreateFinanceDto createFinanceDto)
        {
            var finance = _mapper.Map<Finance>(createFinanceDto);
            await _financeRepository.CreateAsync(finance);
            return (true, "Finans kaydı başarıyla oluşturuldu.");
        }
        public async Task<(bool IsSuccess, string Message)> TUpdateFinanceAsync(UpdateFinanceDto updateFinanceDto)
        {
            var finance = await _financeRepository.GetByIdAsync(updateFinanceDto.Id);
            if (finance == null)
                return (false, "Finans kaydı bulunamadı.");
            var originalCreatedDate = finance.Date;
            _mapper.Map(updateFinanceDto, finance);
            finance.Date = originalCreatedDate;
            await _financeRepository.UpdateAsync(finance);
            return (true, "Finans kaydı başarıyla güncellendi.");
        }
        public async Task<(bool IsSuccess, string Message)> TSoftDeleteFinanceAsync(int id)
        {
            var finance = await _financeRepository.GetByIdAsync(id);
            if (finance == null)
                return (false, "Finans kaydı bulunamadı.");

            if (finance.IsDeleted)
                return (false, "Finans kaydı zaten silinmiş durumda.");

            await _financeRepository.SoftDeleteAsync(finance);
            return (true, "Finans kaydı başarıyla silindi.");
        }
        public async Task<(bool IsSuccess, string Message)> TUndoSoftDeleteFinanceAsync(int id)
        {
            var finance = await _financeRepository.GetByIdAsync(id);
            if (finance == null)
                return (false, "Finans kaydı bulunamadı.");

            if (!finance.IsDeleted)
                return (false, "Finans kaydı zaten aktif durumda.");

            await _financeRepository.UndoSoftDeleteAsync(finance);
            return (true, "Finans kaydı başarıyla geri getirildi.");
        }
    }
}
