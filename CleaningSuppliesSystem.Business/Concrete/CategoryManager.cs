using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.CategoryDtos;
using CleaningSuppliesSystem.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using static CleaningSuppliesSystem.Business.Validators.Validators;

namespace CleaningSuppliesSystem.Business.Concrete
{
    public class CategoryManager : GenericManager<Category>, ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly ISubCategoryRepository _subCategoryRepository;
        private readonly IMapper _mapper;
        public CategoryManager(IRepository<Category> repository, ICategoryRepository categoryRepository, ISubCategoryRepository subCategoryRepository, 
            IBrandRepository brandRepository, IMapper mapper)
        : base(repository)
        {
            _categoryRepository = categoryRepository;
            _brandRepository = brandRepository;
            _subCategoryRepository = subCategoryRepository;
            _mapper = mapper;
        }
        public async Task<(bool IsSuccess, string Message, int CreatedId)> TCreateCategoryAsync(CreateCategoryDto createCategoryDto)
        {
            var validator = new CreateCategoryValidator();
            var validationResult = await validator.ValidateAsync(createCategoryDto);
            if (!validationResult.IsValid)
            {
                var message = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
                return (false, message, 0);
            }
            var category = _mapper.Map<Category>(createCategoryDto);
            category.CreatedDate = DateTime.Now;
            category.IsShown = true;
            await _categoryRepository.CreateAsync(category);
            return (true, "Ürün Grubu / Kategori başarıyla oluşturuldu.", category.Id);
        }

        public async Task<(bool IsSuccess, string Message, int UpdatedId)> TUpdateCategoryAsync(UpdateCategoryDto updateCategoryDto)
        {
            var validator = new UpdateCategoryValidator();
            var validationResult = await validator.ValidateAsync(updateCategoryDto);

            if (!validationResult.IsValid)
            {
                var message = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
                return (false, message, 0);
            }
            var category = await _categoryRepository.GetByIdAsync(updateCategoryDto.Id);
            if (category == null)
                return (false, "Marka bulunamadı.", 0);

            _mapper.Map(updateCategoryDto, category);
            category.UpdatedDate = DateTime.Now;
            await _categoryRepository.UpdateAsync(category);
            return (true, "Marka başarıyla güncellendi.", category.Id);
        }

        public async Task<(bool IsSuccess, string Message, int SoftDeletedId)> TSoftDeleteCategoryAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return (false, "Kategori bulunamadı.", 0);

            if (category.IsDeleted)
                return (false, "Kategori zaten silinmiş.", category.Id);

            // Marka ilişkisi kontrolü
            var isUsedInBrands = await _brandRepository.AnyAsync(x => x.CategoryId == id && !x.IsDeleted);
            if (isUsedInBrands)
                return (false, $"'{category.Name}' kategorisi bazı markalarda kullanılıyor. Silme işlemi engellendi.", category.Id);

            category.DeletedDate = DateTime.Now;
            category.IsDeleted = true;
            await _categoryRepository.UpdateAsync(category);

            return (true, "Kategori başarıyla çöp kutusuna taşındı.", category.Id);
        }

        public async Task<(bool IsSuccess, string Message, int UndoSoftDeletedId)> TUndoSoftDeleteCategoryAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return (false, "Kategori bulunamadı.", 0);

            if (!category.IsDeleted)
                return (false, "Kategori zaten aktif.", category.Id);

            // Aynı isimde aktif başka bir kategori var mı?
            var isDuplicate = await _categoryRepository.AnyAsync(x =>
                x.Name == category.Name &&
                x.IsDeleted == false &&
                x.Id != category.Id);
            if (isDuplicate)
                return (false, $"'{category.Name}' isminde aktif bir kategori mevcut. Geri alma yapılamadı.", category.Id);

            category.IsDeleted = false;
            category.DeletedDate = null;
            await _categoryRepository.UpdateAsync(category);
            return (true, "Kategori başarıyla geri getirildi.", category.Id);
        }


        public async Task<(bool isSuccess, string message)> TPermanentDeleteCategoryAsync(int id)
        {
            // Kategori al
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return (false, "Kategori bulunamadı.");

            if (!category.IsDeleted)
                return (false, "Kategori soft silinmemiş. Önce soft silmeniz gerekir.");

            var hasBrands = await _brandRepository.AnyIncludeSoftDeletedAsync(b => b.CategoryId == id);
            if (hasBrands)
                return (false, "Kategoriye bağlı markalar var. Önce onları kalıcı olarak silmelisiniz.");

            await _categoryRepository.DeleteAsync(category.Id);
            return (true, "Kategori ve bağlı markalar çöp kutusundan kalıcı olarak silindi.");
        }



        public async Task<(bool IsSuccess, string Message)> TSetCategoryDisplayOnHomeAsync(int categoryId, bool isShown)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null)
                return (false, "Kategori bulunamadı.");

            category.IsShown = isShown;
            await _categoryRepository.UpdateAsync(category);
            var status = isShown ? "gösterilecek" : "gösterilmeyecek";
            return (true, $"Kategori ana sayfada {status} şekilde ayarlandı.");
        }
        public async Task<List<ResultCategoryDto>> TGetActiveCategoriesAsync()
        {
            var entities = await _categoryRepository.GetActiveCategoriesAsync();
            return _mapper.Map<List<ResultCategoryDto>>(entities);
        }
        public async Task<List<ResultCategoryDto>> TGetDeletedCategoriesAsync()
        {
            var entities = await _categoryRepository.GetDeletedCategoriesAsync();
            return _mapper.Map<List<ResultCategoryDto>>(entities);
        }
        public async Task<List<ResultCategoryDto>> TGetActiveBySubCategoryIdAsync(int subCategoryId)
        {
            var entities = await _categoryRepository.GetActiveBySubCategoryIdAsync(subCategoryId);
            return _mapper.Map<List<ResultCategoryDto>>(entities);
        }

        public async Task<List<(int Id, bool IsSuccess, string Message)>> TSoftDeleteRangeCategoryAsync(List<int> ids)
        {
            var results = new List<(int Id, bool IsSuccess, string Message)>();
            var categories = new List<Category>();

            var usedInBrands = new List<string>();
            var notFound = new List<int>();

            foreach (var id in ids)
            {
                var category = await _categoryRepository.GetByIdAsync(id);
                if (category == null)
                {
                    notFound.Add(id);
                    continue;
                }

                var isUsed = await _brandRepository.AnyAsync(x => x.CategoryId == id && !x.IsDeleted);
                if (isUsed)
                {
                    usedInBrands.Add(category.Name);
                    continue;
                }

                categories.Add(category); // uygun kategori
            }

            // Eğer herhangi bir engel varsa, işlem yapılmasın
            if (usedInBrands.Any() || notFound.Any())
            {
                if (usedInBrands.Any())
                    results.Add((0, false, $"{string.Join(", ", usedInBrands)} kategorileri bazı markalarda kullanıldığı için silinemedi."));

                if (notFound.Any())
                    results.Add((0, false, $"ID {string.Join(", ", notFound)} kategorileri bulunamadı."));

                return results;
            }

            // Hatalı yoksa silme işlemlerini yap
            foreach (var category in categories)
            {
                category.IsDeleted = true;
                category.DeletedDate = DateTime.Now;
                await _categoryRepository.UpdateAsync(category);
                results.Add((category.Id, true, $"'{category.Name}' kategorisi başarıyla silindi."));
            }

            return results;
        }

        public async Task<List<(int Id, bool IsSuccess, string Message)>> TUndoSoftDeleteRangeCategoryAsync(List<int> ids)
        {
            var results = new List<(int Id, bool IsSuccess, string Message)>();
            var validCategories = new List<Category>();

            var nameConflicts = new List<string>();
            var alreadyActive = new List<string>();
            var notFound = new List<int>();

            foreach (var id in ids)
            {
                var category = await _categoryRepository.GetByIdAsync(id);
                if (category == null)
                {
                    notFound.Add(id);
                    continue;
                }

                if (!category.IsDeleted)
                {
                    alreadyActive.Add(category.Name);
                    continue;
                }

                var isDuplicate = await _categoryRepository.AnyAsync(x =>
                    x.Name == category.Name && !x.IsDeleted && x.Id != category.Id);

                if (isDuplicate)
                {
                    nameConflicts.Add(category.Name);
                    continue;
                }

                validCategories.Add(category); // geri alınabilir
            }

            // Eğer herhangi bir hata varsa işlem iptal edilir
            if (nameConflicts.Any() || alreadyActive.Any() || notFound.Any())
            {
                if (nameConflicts.Any())
                    results.Add((0, false, $"{string.Join(", ", nameConflicts)} kategorileri isim çakışması nedeniyle geri alınamadı."));

                if (alreadyActive.Any())
                    results.Add((0, false, $"{string.Join(", ", alreadyActive)} kategorileri zaten aktif durumda."));

                if (notFound.Any())
                    results.Add((0, false, $"ID {string.Join(", ", notFound)} kategorileri bulunamadı."));

                return results;
            }

            // Tüm kategoriler geri alınabilir
            foreach (var category in validCategories)
            {
                category.IsDeleted = false;
                category.DeletedDate = null;
                await _categoryRepository.UpdateAsync(category);
                results.Add((category.Id, true, $"'{category.Name}' kategorisi başarıyla geri getirildi."));
            }

            return results;
        }

        public async Task<List<(int Id, bool IsSuccess, string Message)>> TPermanentDeleteRangeCategoryAsync(List<int> ids)
        {
            var results = new List<(int Id, bool IsSuccess, string Message)>();
            var idsToDelete = new List<int>();

            foreach (var id in ids)
            {
                var category = await _categoryRepository.GetByIdAsync(id);
                if (category == null)
                {
                    results.Add((id, false, "Kategori bulunamadı."));
                    continue;
                }

                if (!category.IsDeleted)
                {
                    results.Add((id, false, "Kategori soft silinmemiş. Önce soft silmeniz gerekir."));
                    continue;
                }

                var hasRelatedBrands = await _brandRepository.AnyAsync(b => b.CategoryId == id);
                if (hasRelatedBrands)
                {
                    results.Add((id, false, "Kategoriye bağlı markalar var. Önce onları kalıcı olarak silmelisiniz."));
                    continue;
                }

                idsToDelete.Add(id);
            }

            if (idsToDelete.Any())
            {
                await _categoryRepository.PermanentDeleteRangeAsync(idsToDelete);
                foreach (var deletedId in idsToDelete)
                {
                    results.Add((deletedId, true, "Kategori kalıcı olarak silindi."));
                }
            }

            return results;
        }

        public async Task<List<Category>> TGetByIdsAsync(List<int> ids)
        {
            return await _categoryRepository.GetByIdsAsync(ids);
        }



    }
}
