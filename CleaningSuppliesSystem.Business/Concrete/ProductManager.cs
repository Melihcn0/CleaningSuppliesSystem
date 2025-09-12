using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.DataAccess.Concrete;
using CleaningSuppliesSystem.DTO.DTOs.CategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.DiscountDtos;
using CleaningSuppliesSystem.DTO.DTOs.ProductDtos;
using CleaningSuppliesSystem.DTO.DTOs.StockDtos;
using CleaningSuppliesSystem.DTO.DTOs.SubCategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.TopCategoryDtos;
using CleaningSuppliesSystem.Entity.Entities;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using static CleaningSuppliesSystem.Business.Validators.Validators;

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
        public async Task<(bool IsSuccess, List<string> Errors)> TApplyDiscountAsync(UpdateDiscountDto dto)
        {
            var validator = new UpdateDiscountValidator();
            var validationResult = await validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return (false, errors);
            }

            var product = await _productRepository.GetByIdAsync(dto.Id);
            if (product == null)
                return (false, new List<string> { "Ürün bulunamadı." });

            // İskonto güncelle
            product.DiscountRate = dto.DiscountRate;

            await _productRepository.UpdateAsync(product);
            return (true, new List<string>());
        }

        public async Task<(bool IsSuccess, string Message, int CreatedId)> TCreateProductAsync(CreateProductDto createProductDto)
        {
            var validator = new CreateProductValidator();
            var validationResult = await validator.ValidateAsync(createProductDto);
            if (!validationResult.IsValid)
            {
                var message = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
                return (false, message, 0);
            }

            var product = _mapper.Map<Product>(createProductDto);
            product.IsShown = true;
            product.CreatedDate = DateTime.Now;

            await _productRepository.CreateAsync(product);

            return (true, "Ürün başarıyla oluşturuldu.", product.Id);
        }

        public async Task<(bool IsSuccess, string Message, int UpdatedId)> TUpdateProductAsync(UpdateProductDto updateProductDto)
        {
            var validator = new UpdateProductValidator();
            var validationResult = await validator.ValidateAsync(updateProductDto);

            if (!validationResult.IsValid)
            {
                var message = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
                return (false, message, 0);
            }

            var product = await _productRepository.GetByIdAsync(updateProductDto.Id);
            if (product == null)
                return (false, "Ürün bulunamadı.", 0);

            _mapper.Map(updateProductDto, product);
            product.UpdatedDate = DateTime.Now;

            await _productRepository.UpdateAsync(product);

            return (true, "Ürün başarıyla güncellendi.", product.Id);
        }
        public async Task<(bool IsSuccess, string Message, int SoftDeletedId)> TSoftDeleteProductAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return (false, "Ürün bulunamadı.", 0);
            if (product.IsDeleted)
                return (false, "Ürün zaten silinmiş durumda.", product.Id);
            product.IsDeleted = true;
            product.DeletedDate = DateTime.UtcNow;
            await _productRepository.UpdateAsync(product);
            return (true, "Ürün başarıyla soft silindi.", product.Id);
        }

        public async Task<(bool IsSuccess, string Message, int UndoSoftDeletedId)> TUndoSoftDeleteProductAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return (false, "Ürün bulunamadı.", 0);

            if (!product.IsDeleted)
                return (false, "Ürün zaten aktif durumda.", product.Id);

            product.IsDeleted = false;
            product.DeletedDate = null;
            await _productRepository.UpdateAsync(product);
            return (true, "Ürün başarıyla geri getirildi.", product.Id);
        }

        public async Task<(bool IsSuccess, string Message)> TPermanentDeleteProductAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return (false, "Ürün bulunamadı.");

            if (!product.IsDeleted)
                return (false, "Ürün soft silinmemiş. Önce soft silmeniz gerekir.");

            await _productRepository.DeleteAsync(product.Id);
            return (true, "Ürün kalıcı olarak silindi.");
        }

        public async Task<List<ResultProductDto>> TGetActiveProductsAsync()
        {
            var entities = await _productRepository.GetActiveProductsAsync();
            return _mapper.Map<List<ResultProductDto>>(entities);
        }
        public async Task<List<ResultProductDto>> TGetDeletedProductsAsync()
        {
            var entities = await _productRepository.GetDeletedProductAsync();
            return _mapper.Map<List<ResultProductDto>>(entities);
        }

        public async Task<(bool IsSuccess, string Message)> TSetProductDisplayOnHomeAsync(int productId, bool isShown)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
                return (false, "Ürün bulunamadı.");

            product.IsShown = isShown;
            await _productRepository.UpdateAsync(product);

            var status = isShown ? "gösterilecek" : "gösterilmeyecek";
            return (true, $"Ürün ana sayfa görünürlüğü {status} olarak ayarlandı.");
        }
        public async Task<List<ResultProductDto>> TGetActiveByBrandIdAsync(int brandId)
        {
            var entities = await _productRepository.GetActiveByBrandsIdAsync(brandId);
            return _mapper.Map<List<ResultProductDto>>(entities);
        }
        public async Task<List<(int Id, bool IsSuccess, string Message)>> TSoftDeleteRangeProductAsync(List<int> ids)
        {
            var results = new List<(int Id, bool IsSuccess, string Message)>();
            var validProducts = new List<Product>();

            var alreadyDeleted = new List<string>();
            var notFound = new List<int>();

            foreach (var id in ids)
            {
                var product = await _productRepository.GetByIdAsync(id);
                if (product == null)
                {
                    notFound.Add(id);
                    continue;
                }

                if (product.IsDeleted)
                {
                    alreadyDeleted.Add(product.Name);
                    continue;
                }

                validProducts.Add(product);
            }

            if (alreadyDeleted.Any() || notFound.Any())
            {
                if (alreadyDeleted.Any())
                    results.Add((0, false, $"{string.Join(", ", alreadyDeleted)} ürünleri zaten silinmiş durumda."));

                if (notFound.Any())
                    results.Add((0, false, $"ID {string.Join(", ", notFound)} ürünleri bulunamadı."));

                return results;
            }

            foreach (var product in validProducts)
            {
                product.IsDeleted = true;
                product.DeletedDate = DateTime.UtcNow;
                await _productRepository.UpdateAsync(product);
                results.Add((product.Id, true, $"'{product.Name}' ürünü başarıyla soft silindi."));
            }

            return results;
        }
        public async Task<List<(int Id, bool IsSuccess, string Message)>> TUndoSoftDeleteRangeProductAsync(List<int> ids)
        {
            var results = new List<(int Id, bool IsSuccess, string Message)>();
            var validProducts = new List<Product>();

            var alreadyActive = new List<string>();
            var notFound = new List<int>();

            foreach (var id in ids)
            {
                var product = await _productRepository.GetByIdAsync(id);
                if (product == null)
                {
                    notFound.Add(id);
                    continue;
                }

                if (!product.IsDeleted)
                {
                    alreadyActive.Add(product.Name);
                    continue;
                }

                validProducts.Add(product);
            }

            if (alreadyActive.Any() || notFound.Any())
            {
                if (alreadyActive.Any())
                    results.Add((0, false, $"{string.Join(", ", alreadyActive)} ürünleri zaten aktif durumda."));

                if (notFound.Any())
                    results.Add((0, false, $"ID {string.Join(", ", notFound)} ürünleri bulunamadı."));

                return results;
            }

            foreach (var product in validProducts)
            {
                product.IsDeleted = false;
                product.DeletedDate = null;
                await _productRepository.UpdateAsync(product);
                results.Add((product.Id, true, $"'{product.Name}' ürünü başarıyla geri getirildi."));
            }

            return results;
        }
        public async Task TPermanentDeleteRangeProductAsync(List<int> ids)
        {
            await _productRepository.PermanentDeleteRangeAsync(ids);
        }
        public async Task TDecreaseStockAsync(IEnumerable<OrderItem> orderItems)
        {
            foreach (var item in orderItems)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null)
                    throw new Exception($"Ürün bulunamadı: Id={item.ProductId}");

                if (product.StockQuantity < item.Quantity)
                    throw new Exception($"Yeterli stok yok: {product.Name}");

                product.StockQuantity -= item.Quantity;
                product.UpdatedDate = DateTime.Now;
                await _productRepository.UpdateAsync(product);
            }
        }




    }
}