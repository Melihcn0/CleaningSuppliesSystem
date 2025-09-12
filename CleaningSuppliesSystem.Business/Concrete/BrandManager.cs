using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.Entity.Entities;
using CleaningSuppliesSystem.DTO.DTOs.BrandDtos;
using System;
using System.Threading.Tasks;
using CleaningSuppliesSystem.DTO.DTOs.ProductDtos;
using static CleaningSuppliesSystem.Business.Validators.Validators;
using CleaningSuppliesSystem.DataAccess.Concrete;
using CleaningSuppliesSystem.DTO.DTOs.TopCategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.SubCategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.FinanceDtos;

namespace CleaningSuppliesSystem.Business.Concrete
{
    public class BrandManager : GenericManager<Brand>, IBrandService
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public BrandManager(IRepository<Brand> repository, IBrandRepository brandRepository, IProductRepository productRepository, IMapper mapper)
            : base(repository)
        {
            _brandRepository = brandRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<(bool IsSuccess, string Message, int CreatedId)> TCreateBrandAsync(CreateBrandDto createBrandDto)
        {
            var validator = new CreateBrandValidator();
            var validationResult = await validator.ValidateAsync(createBrandDto);
            if (!validationResult.IsValid)
            {
                var message = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
                return (false, message, 0);
            }
            var brand = _mapper.Map<Brand>(createBrandDto);
            brand.CreatedDate = DateTime.Now;
            await _brandRepository.CreateAsync(brand);
            return (true, "Marka başarıyla oluşturuldu.", brand.Id);
        }

        public async Task<(bool IsSuccess, string Message, int UpdatedId)> TUpdateBrandAsync(UpdateBrandDto updateBrandDto)
        {
            var validator = new UpdateBrandValidator();
            var validationResult = await validator.ValidateAsync(updateBrandDto);

            if (!validationResult.IsValid)
            {
                var message = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
                return (false, message, 0);
            }
            var brand = await _brandRepository.GetByIdAsync(updateBrandDto.Id);
            if (brand == null)
                return (false, "Marka bulunamadı.", 0);

            _mapper.Map(updateBrandDto, brand);
            brand.UpdatedDate = DateTime.Now;
            await _brandRepository.UpdateAsync(brand);
            return (true, "Marka başarıyla güncellendi.", brand.Id);
        }

        public async Task<(bool IsSuccess, string Message, int SoftDeletedId)> TSoftDeleteBrandAsync(int id)
        {
            var brand = await _brandRepository.GetByIdAsync(id);
            if (brand == null)
                return (false, "Marka bulunamadı.", 0);

            if (brand.IsDeleted)
                return (false, "Marka zaten silinmiş.", brand.Id);

            // Marka ürünlerde kullanılıyor mu?
            var isUsedInProducts = await _productRepository.AnyAsync(x => x.BrandId == id && !x.IsDeleted);
            if (isUsedInProducts)
                return (false, "Bu marka bazı ürünlerde aktif olarak kullanılıyor. Soft silme yapılamaz.", brand.Id);

            brand.DeletedDate = DateTime.Now;
            brand.IsDeleted = true;
            await _brandRepository.UpdateAsync(brand);
            return (true, "Marka başarıyla çöp kutusuna taşındı.", brand.Id);
        }

        public async Task<(bool IsSuccess, string Message, int UndoSoftDeletedId)> TUndoSoftDeleteBrandAsync(int id)
        {
            var brand = await _brandRepository.GetByIdAsync(id);
            if (brand == null)
                return (false, "Marka bulunamadı.", 0);

            if (!brand.IsDeleted)
                return (false, "Marka zaten aktif.", brand.Id);

            // Aynı isimde aktif başka marka var mı?
            var isDuplicate = await _brandRepository.AnyAsync(x =>
                x.Name == brand.Name &&
                x.IsDeleted == false &&
                x.Id != brand.Id);
            if (isDuplicate)
                return (false, $"'{brand.Name}' isminde aktif bir marka mevcut. Geri alma yapılamadı.", brand.Id);

            brand.DeletedDate = null;
            brand.IsDeleted = false;
            await _brandRepository.UpdateAsync(brand);
            return (true, "Marka başarıyla geri getirildi.", brand.Id);
        }

        public async Task<(bool IsSuccess, string Message)> TPermanentDeleteBrandAsync(int id)
        {
            var brand = await _brandRepository.GetByIdAsync(id);
            if (brand == null)
                return (false, "Marka bulunamadı.");

            if (!brand.IsDeleted)
                return (false, "Marka silinmemiş. Önce silmeniz gerekir.");

            await _brandRepository.DeleteAsync(brand.Id);
            return (true, "Marka çöp kutusundan kalıcı olarak silindi.");
        }
        public async Task<List<ResultBrandDto>> TGetActiveBrandsAsync()
        {
            var entities = await _brandRepository.GetActiveBrandsAsync();
            return _mapper.Map<List<ResultBrandDto>>(entities);
        }
        public async Task<List<ResultBrandDto>> TGetDeletedBrandsAsync()
        {
            var entities = await _brandRepository.GetDeletedBrandsAsync();
            return _mapper.Map<List<ResultBrandDto>>(entities);
        }
        public async Task<List<ResultBrandDto>> TGetActiveByCategoryIdAsync(int categoryId)
        {
            var entities = await _brandRepository.GetActiveByCategoryIdAsync(categoryId);
            return _mapper.Map<List<ResultBrandDto>>(entities);
        }
        public async Task<List<(int Id, bool IsSuccess, string Message)>> TSoftDeleteRangeBrandAsync(List<int> ids)
        {
            var results = new List<(int Id, bool IsSuccess, string Message)>();

            foreach (var id in ids)
            {
                var brand = await _brandRepository.GetByIdAsync(id);
                if (brand == null)
                {
                    results.Add((id, false, "Marka bulunamadı."));
                    continue;
                }

                if (brand.IsDeleted)
                {
                    results.Add((id, false, "Marka zaten silinmiş."));
                    continue;
                }

                var isUsedInProducts = await _productRepository.AnyAsync(x => x.BrandId == id && !x.IsDeleted);
                if (isUsedInProducts)
                {
                    results.Add((id, false, "Bu marka bazı ürünlerde aktif olarak kullanılıyor. Soft silme yapılamadı."));
                    continue;
                }

                brand.IsDeleted = true;
                brand.DeletedDate = DateTime.UtcNow;
                await _brandRepository.UpdateAsync(brand);
                results.Add((id, true, "Marka başarıyla soft silindi."));
            }

            return results;
        }
        public async Task<List<(int Id, bool IsSuccess, string Message)>> TUndoSoftDeleteRangeBrandAsync(List<int> ids)
        {
            var results = new List<(int Id, bool IsSuccess, string Message)>();
            var brandsToUpdate = new List<Brand>();
            var softDeletedNameCounts = new Dictionary<string, int>();

            foreach (var id in ids)
            {
                var brand = await _brandRepository.GetByIdAsync(id);

                if (brand == null)
                {
                    results.Add((id, false, "Marka bulunamadı."));
                    continue;
                }

                if (!brand.IsDeleted)
                {
                    results.Add((id, false, "Marka zaten aktif."));
                    continue;
                }

                var isDuplicate = await _brandRepository.AnyAsync(x =>
                    x.Name == brand.Name && !x.IsDeleted && x.Id != brand.Id);

                if (isDuplicate)
                {
                    results.Add((id, false, $"'{brand.Name}' isminde aktif bir marka mevcut."));
                    continue;
                }

                softDeletedNameCounts[brand.Name] = softDeletedNameCounts.ContainsKey(brand.Name)
                    ? softDeletedNameCounts[brand.Name] + 1
                    : 1;

                brandsToUpdate.Add(brand);
                results.Add((id, true, "Geri alma için uygun."));
            }

            // Aynı isimle birden fazla soft-deleted varsa: işlemi durdur
            var conflictedNames = softDeletedNameCounts
                .Where(x => x.Value > 1)
                .Select(x => x.Key)
                .ToList();

            if (conflictedNames.Any())
            {
                results = results.Select(r =>
                {
                    var brand = brandsToUpdate.FirstOrDefault(b => b.Id == r.Id);
                    if (brand != null && conflictedNames.Contains(brand.Name))
                    {
                        return (r.Id, false, $"'{brand.Name}' isminde birden fazla silinmiş marka bulundu. Lütfen sadece birini seçin.");
                    }
                    return r;
                }).ToList();
            }

            if (results.Any(r => !r.IsSuccess))
            {
                return results;
            }

            foreach (var brand in brandsToUpdate)
            {
                brand.IsDeleted = false;
                brand.DeletedDate = null;
                await _brandRepository.UpdateAsync(brand);
            }

            results = results.Select(r => (r.Id, true, "Marka başarıyla geri getirildi.")).ToList();
            return results;
        }

        public async Task TPermanentDeleteRangeBrandAsync(List<int> ids)
        {
            await _brandRepository.PermanentDeleteRangeAsync(ids);
        }

    }
}
