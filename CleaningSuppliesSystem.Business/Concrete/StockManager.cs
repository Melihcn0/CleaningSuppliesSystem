using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.DataAccess.Concrete;
using CleaningSuppliesSystem.DTO.DTOs.ProductDtos;
using CleaningSuppliesSystem.DTO.DTOs.StockDtos;
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
    public class StockManager : IStockService
    {
        private readonly IStockRepository _stockRepository;
        private readonly IMapper _mapper;
        public StockManager(IStockRepository stockRepository, IMapper mapper)
        {
            _stockRepository = stockRepository;
            _mapper = mapper;
        }

        public async Task<(bool IsSuccess, string Message)> AssignStockAsync(CreateStockOperationDto dto)
        {
            var validator = new CreateStockOperationValidator();
            var validationResult = await validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
                return (false, $"Girdiğiniz bilgilerde hatalar var: {errors}");
            }

            var product = await _stockRepository.GetByIdAsync(dto.ProductId);
            if (product == null)
                return (false, "İşlem yapmak istediğiniz ürün stokta bulunamadı.");

            int currentStock = product.StockQuantity ?? 0;
            int requestedQuantity = dto.Quantity;

            if (dto.TransactionType == false)
            {
                if (requestedQuantity > currentStock)
                {
                    return (false, $"Yetersiz stok mevcut. Mevcut stok: {currentStock}, talep edilen miktar: {requestedQuantity}.");
                }
            }

            var result = await _stockRepository.AssignStockAsync(dto.ProductId, dto.Quantity, dto.TransactionType ?? false);
            if (!result)
                return (false, "Stok işlemi gerçekleştirilemedi. Lütfen tekrar deneyiniz.");

            return (true, "Stok işlemi başarıyla tamamlandı.");
        }


        public async Task<List<ResultStockOperationDto>> TGetActiveProductsAsync()
        {
            var entities = await _stockRepository.GetActiveProductsAsync();
            return _mapper.Map<List<ResultStockOperationDto>>(entities);
        }
    }

}
