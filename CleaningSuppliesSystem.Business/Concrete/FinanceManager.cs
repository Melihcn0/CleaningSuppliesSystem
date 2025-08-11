using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.DataAccess.Concrete;
using CleaningSuppliesSystem.DTO.DTOs.CategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.FinanceDtos;
using CleaningSuppliesSystem.DTO.DTOs.TopCategoryDtos;
using CleaningSuppliesSystem.Entity.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CleaningSuppliesSystem.Business.Validators.Validators;

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
        public async Task<(bool IsSuccess, string Message, int CreatedId)> TCreateFinanceAsync(CreateFinanceDto createfinanceDto)
        {
            var validator = new CreateFinanceValidator();
            var validationResult = await validator.ValidateAsync(createfinanceDto);
            if (!validationResult.IsValid)
            {
                var message = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
                return (false, message, 0);
            }
            var finance = _mapper.Map<Finance>(createfinanceDto);
            finance.CreatedDate = DateTime.Now;
            await _financeRepository.CreateAsync(finance);
            return (true, "Finans kaydı başarıyla oluşturuldu.", finance.Id);
        }

        public async Task<(bool IsSuccess, string Message, int UpdatedId)> TUpdateFinanceAsync(UpdateFinanceDto updatefinanceDto)
        {
            var validator = new UpdateFinanceValidator();
            var validationResult = await validator.ValidateAsync(updatefinanceDto);
            if (!validationResult.IsValid)
            {
                var message = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
                return (false, message, 0);
            }

            var finance = await _financeRepository.GetByIdAsync(updatefinanceDto.Id);
            if (finance == null)
                return (false, "Finans kaydı bulunamadı.", 0);

            _mapper.Map(updatefinanceDto, finance);
            finance.UpdatedDate = DateTime.Now;
            await _financeRepository.UpdateAsync(finance);
            return (true, "Finans kaydı başarıyla güncellendi.", finance.Id);
        }

        public async Task<(bool IsSuccess, string Message, int SoftDeletedId)> TSoftDeleteFinanceAsync(int id)
        {
            var finance = await _financeRepository.GetByIdAsync(id);
            if (finance == null)
                return (false, "Finans kaydı bulunamadı.", 0);

            if (finance.IsDeleted)
                return (false, "Finans kaydı zaten silinmiş.", finance.Id);

            finance.DeletedDate = DateTime.Now;
            finance.IsDeleted = true;
            await _financeRepository.UpdateAsync(finance);
            return (true, "Finans kaydı başarıyla silindi.", finance.Id);
        }

        public async Task<(bool IsSuccess, string Message, int UndoSoftDeletedId)> TUndoSoftDeleteFinanceAsync(int id)
        {
            var finance = await _financeRepository.GetByIdAsync(id);
            if (finance == null)
                return (false, "Finans kaydı bulunamadı.", 0);

            if (!finance.IsDeleted)
                return (false, "Finans kaydı zaten aktif.", finance.Id);

            finance.DeletedDate = null;
            finance.IsDeleted = false;
            await _financeRepository.UpdateAsync(finance);
            return (true, "Finans kaydı başarıyla geri getirildi.", finance.Id);
        }
        public async Task<(bool IsSuccess, string Message)> TPermanentDeleteFinanceAsync(int id)
        {
            var finance = await _financeRepository.GetByIdAsync(id);
            if (finance == null)
                return (false, "Finans Kaydı bulunamadı.");

            if (!finance.IsDeleted)
                return (false, "Finans kaydı soft silinmemiş. Önce soft silmeniz gerekir.");

            await _financeRepository.DeleteAsync(finance.Id);
            return (true, "Finans kaydı kalıcı olarak silindi.");
        }
        public async Task<List<ResultFinanceDto>> TGetActiveFinancesAsync()
        {
            var entities = await _financeRepository.GetActiveFinancesAsync();
            return _mapper.Map<List<ResultFinanceDto>>(entities);
        }
        public async Task<List<ResultFinanceDto>> TGetDeletedFinancesAsync()
        {
            var entities = await _financeRepository.GetDeletedFinancesAsync();
            return _mapper.Map<List<ResultFinanceDto>>(entities);
        }
        public async Task<List<(int Id, bool IsSuccess, string Message)>> TSoftDeleteRangeFinanceAsync(List<int> ids)
        {
            var results = new List<(int Id, bool IsSuccess, string Message)>();
            var validItems = new List<Finance>();

            var alreadyDeleted = new List<string>();
            var notFound = new List<int>();

            foreach (var id in ids)
            {
                var finance = await _financeRepository.GetByIdAsync(id);
                if (finance == null)
                {
                    notFound.Add(id);
                    continue;
                }

                if (finance.IsDeleted)
                {
                    alreadyDeleted.Add(finance.Title);
                    continue;
                }

                validItems.Add(finance);
            }

            if (alreadyDeleted.Any() || notFound.Any())
            {
                if (alreadyDeleted.Any())
                    results.Add((0, false, $"{string.Join(", ", alreadyDeleted)} finans kayıtları zaten silinmiş."));

                if (notFound.Any())
                    results.Add((0, false, $"ID {string.Join(", ", notFound)} finans kayıtları bulunamadı."));

                return results;
            }

            foreach (var finance in validItems)
            {
                finance.IsDeleted = true;
                finance.DeletedDate = DateTime.UtcNow;
                await _financeRepository.UpdateAsync(finance);
                results.Add((finance.Id, true, $"'{finance.Title}' finans kaydı başarıyla silindi."));
            }

            return results;
        }
        public async Task<List<(int Id, bool IsSuccess, string Message)>> TUndoSoftDeleteRangeFinanceAsync(List<int> ids)
        {
            var results = new List<(int Id, bool IsSuccess, string Message)>();
            var validItems = new List<Finance>();

            var alreadyActive = new List<string>();
            var notFound = new List<int>();

            foreach (var id in ids)
            {
                var finance = await _financeRepository.GetByIdAsync(id);
                if (finance == null)
                {
                    notFound.Add(id);
                    continue;
                }

                if (!finance.IsDeleted)
                {
                    alreadyActive.Add(finance.Title);
                    continue;
                }

                validItems.Add(finance);
            }

            if (alreadyActive.Any() || notFound.Any())
            {
                if (alreadyActive.Any())
                    results.Add((0, false, $"{string.Join(", ", alreadyActive)} finans kayıtları zaten aktif durumda."));

                if (notFound.Any())
                    results.Add((0, false, $"ID {string.Join(", ", notFound)} finans kayıtları bulunamadı."));

                return results;
            }

            foreach (var finance in validItems)
            {
                finance.IsDeleted = false;
                finance.DeletedDate = null;
                await _financeRepository.UpdateAsync(finance);
                results.Add((finance.Id, true, $"'{finance.Title}' finans kaydı başarıyla geri getirildi."));
            }

            return results;
        }
        public async Task TPermanentDeleteRangeFinanceAsync(List<int> ids)
        {
            await _financeRepository.PermanentDeleteRangeAsync(ids);
        }

    }
}
