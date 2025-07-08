using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.ProductDtos;
using CleaningSuppliesSystem.Entity.Entities;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Business.Concrete
{
    public class ProductManager : GenericManager<Product>, IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductManager(IRepository<Product> repository, IProductRepository productRepository, IMapper mapper)
        : base(repository)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }
        public async Task<List<Product>> TGetProductsWithCategoriesAsync()
        {
            return await _productRepository.GetProductsWithCategoriesAsync();
        }       
        public async Task<Product> TGetByIdAsyncWithCategory(int id)
        {
            return await _productRepository.GetByIdAsyncWithCategory(id);
        }
        public async Task<(bool IsSuccess, List<string> Errors)> TApplyDiscountAsync(UpdateProductDto dto)
        {
            var validator = new Validators.Validators.DiscountValidator();
            var validationResult = await validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                return (false, validationResult.Errors.Select(e => e.ErrorMessage).ToList());

            var product = await _productRepository.GetByIdAsync(dto.Id);
            if (product == null)
                return (false, new List<string> { "Ürün bulunamadı." });

            product.DiscountRate = dto.DiscountRate;
            await _productRepository.UpdateAsync(product);

            return (true, new List<string>());
        }
        public async Task<UpdateProductDto> TGetProductForDiscountAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return null;

            return new UpdateProductDto
            {
                Id = product.Id,
                UnitPrice = product.UnitPrice,
                DiscountRate = product.DiscountRate
            };
        }
        public async Task<(bool IsSuccess, string Message)> TCreateProductAsync(CreateProductDto createProductDto)
        {
            var product = _mapper.Map<Product>(createProductDto);
            await _productRepository.CreateAsync(product);
            return (true, "Ürün başarıyla oluşturuldu.");
        }
        public async Task<(bool IsSuccess, string Message)> TUpdateProductAsync(UpdateProductDto updateProductDto)
        {
            var product = await _productRepository.GetByIdAsync(updateProductDto.Id);
            if (product == null)
                return (false, "Ürün bulunamadı.");
            var originalCreatedDate = product.CreatedAt;
            _mapper.Map(updateProductDto, product);
            product.CreatedAt = originalCreatedDate;
            await _productRepository.UpdateAsync(product);
            return (true, "Ürün başarıyla güncellendi.");
        }
        public async Task<(bool IsSuccess, string Message)> TSoftDeleteProductAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return (false, "Ürün bulunamadı.");

            if (product.IsDeleted)
                return (false, "Ürün zaten silinmiş durumda.");

            await _productRepository.SoftDeleteAsync(product);
            return (true, "Ürün başarıyla silindi.");
        }
        public async Task<(bool IsSuccess, string Message)> TUndoSoftDeleteProductAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return (false, "Ürün bulunamadı.");

            if (!product.IsDeleted)
                return (false, "Ürün zaten aktif durumda.");

            await _productRepository.UndoSoftDeleteAsync(product);
            return (true, "Ürün başarıyla geri getirildi.");
        }
    }
}