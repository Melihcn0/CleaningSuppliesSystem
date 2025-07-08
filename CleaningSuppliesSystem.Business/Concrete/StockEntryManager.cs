using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.StockEntryDtos;
using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Business.Concrete
{
    public class StockEntryManager : GenericManager<StockEntry> , IStockEntryService
    {
        private readonly IStockEntryRepository _stockEntryRepository;
        private readonly IMapper _mapper;

        public StockEntryManager(IRepository<StockEntry> repository, IStockEntryRepository stockEntryRepository, IMapper mapper) : base(repository)
        {
            _stockEntryRepository = stockEntryRepository;
            _mapper = mapper;
        }
        public async Task<List<StockEntry>> TGetStockEntryWithProductsandCategoriesAsync()
        {
            return await _stockEntryRepository.GetStockEntryWithProductsandCategoriesAsync();
        }
        public async Task<StockEntry> TGetByIdAsyncWithProductsandCategories(int id)
        {
            return await _stockEntryRepository.GetByIdAsyncWithProductsandCategories(id);
        }
        public async Task<(bool IsSuccess, string Message)> TCreateStockEntryAsync(CreateStockEntryDto createStockEntryDto)
        {
            var stockEntry = _mapper.Map<StockEntry>(createStockEntryDto);
            await _stockEntryRepository.CreateAsync(stockEntry);
            return (true, "Stok başarıyla oluşturuldu.");
        }
        public async Task<(bool IsSuccess, string Message)> TUpdateStockEntryAsync(UpdateStockEntryDto updateStockEntryDto)
        {
            var stockEntry = await _stockEntryRepository.GetByIdAsync(updateStockEntryDto.Id);
            if (stockEntry == null)
                return (false, "Stok bulunamadı.");
            var originalCreatedDate = stockEntry.EntryDate;
            _mapper.Map(updateStockEntryDto, stockEntry);
            stockEntry.EntryDate = originalCreatedDate;
            await _stockEntryRepository.UpdateAsync(stockEntry);
            return (true, "Stok başarıyla güncellendi.");
        }
        public async Task<(bool IsSuccess, string Message)> TSoftDeleteStockEntryAsync(int id)
        {
            var stockEntry = await _stockEntryRepository.GetByIdAsync(id);
            if (stockEntry == null)
                return (false, "Stok bulunamadı.");

            if (stockEntry.IsDeleted)
                return (false, "Stok zaten silinmiş durumda.");

            await _stockEntryRepository.SoftDeleteAsync(stockEntry);
            return (true, "Stok başarıyla silindi.");
        }
        public async Task<(bool IsSuccess, string Message)> TUndoSoftDeleteStockEntryAsync(int id)
        {
            var stockEntry = await _stockEntryRepository.GetByIdAsync(id);
            if (stockEntry == null)
                return (false, "Stok bulunamadı.");

            if (!stockEntry.IsDeleted)
                return (false, "Stok zaten aktif durumda.");

            await _stockEntryRepository.UndoSoftDeleteAsync(stockEntry);
            return (true, "Stok başarıyla geri getirildi.");
        }
    }
}
