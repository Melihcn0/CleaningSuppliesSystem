using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.DataAccess.Concrete;
using CleaningSuppliesSystem.DTO.DTOs.CategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.ProductDtos;
using CleaningSuppliesSystem.DTO.DTOs.TopCategoryDtos;
using CleaningSuppliesSystem.Entity.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using static CleaningSuppliesSystem.Business.Validators.Validators;

namespace CleaningSuppliesSystem.Business.Concrete
{
    public class TopCategoryManager : GenericManager<TopCategory>, ITopCategoryService
    {
        private readonly ITopCategoryRepository _topCategoryRepository;
        private readonly ISubCategoryRepository _subCategoryRepository;
        private readonly IMapper _mapper;

        public TopCategoryManager(
            IRepository<TopCategory> repository,
            ITopCategoryRepository topCategoryRepository,
            ISubCategoryRepository subCategoryRepository,
            IMapper mapper)
            : base(repository)
        {
            _topCategoryRepository = topCategoryRepository;
            _subCategoryRepository = subCategoryRepository;
            _mapper = mapper;
        }

        public async Task<(bool IsSuccess, string Message, int CreatedId)> TCreateTopCategoryAsync(CreateTopCategoryDto createTopCategoryDto)
        {
            var validator = new CreateTopCategoryValidator();
            var validationResult = await validator.ValidateAsync(createTopCategoryDto);
            if (!validationResult.IsValid)
            {
                var message = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
                return (false, message, 0);
            }
            var topCategory = _mapper.Map<TopCategory>(createTopCategoryDto);
            topCategory.CreatedDate = DateTime.Now;
            await _topCategoryRepository.CreateAsync(topCategory);
            return (true, "Üst kategori başarıyla eklendi.", topCategory.Id);
        }

        public async Task<(bool IsSuccess, string Message, int UpdatedId)> TUpdateTopCategoryAsync(UpdateTopCategoryDto updateTopCategoryDto)
        {
            var validator = new UpdateTopCategoryValidator();
            var validationResult = await validator.ValidateAsync(updateTopCategoryDto);

            if (!validationResult.IsValid)
            {
                var message = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
                return (false, message, 0);
            }

            var topCategory = await _topCategoryRepository.GetByIdAsync(updateTopCategoryDto.Id);
            if (topCategory == null)
                return (false, "Üst kategori bulunamadı.", 0);
            topCategory.UpdatedDate = DateTime.Now;
            _mapper.Map(updateTopCategoryDto, topCategory);
            await _topCategoryRepository.UpdateAsync(topCategory);
            return (true, "Üst kategori başarıyla güncellendi.", topCategory.Id);
        }
        public async Task<(bool IsSuccess, string Message, int SoftDeletedId)> TSoftDeleteTopCategoryAsync(int id)
        {
            var category = await _topCategoryRepository.GetByIdAsync(id);
            if (category == null)
                return (false, $"ID {id} kategorisi bulunamadı.", 0);

            if (category.IsDeleted)
                return (false, $"{category.Name} kategorisi zaten çöp kutusunda", category.Id);

            var isUsedInSubCategories = await _subCategoryRepository.AnyAsync(x => x.TopCategoryId == id && !x.IsDeleted);
            if (isUsedInSubCategories)
                return (false, $"{category.Name} kategorisi alt kategorilerde kullanılıyor. Silme işlemi yapılamaz.", category.Id);

            category.IsDeleted = true;
            category.DeletedDate = DateTime.Now;
            await _topCategoryRepository.UpdateAsync(category);

            return (true, $"{category.Name} kategorisi başarıyla çöp kutusuna taşındı.", category.Id);
        }


        public async Task<(bool IsSuccess, string Message, int UndoSoftDeletedId)> TUndoSoftDeleteTopCategoryAsync(int id)
        {
            var topCategory = await _topCategoryRepository.GetByIdAsync(id);
            if (topCategory == null)
                return (false, "Üst kategori bulunamadı.", 0);

            if (!topCategory.IsDeleted)
                return (false, "Üst kategori zaten aktif durumda.", topCategory.Id);

            var isDuplicate = await _topCategoryRepository.AnyAsync(x =>
                x.Name == topCategory.Name &&
                x.IsDeleted == false &&
                x.Id != topCategory.Id);

            if (isDuplicate)
                return (false, $"{topCategory.Name} isminde aktif bir üst kategori mevcut. Lütfen ismini değiştirerek tekrar deneyin.", topCategory.Id);

            topCategory.IsDeleted = false;
            topCategory.DeletedDate = null;
            await _topCategoryRepository.UpdateAsync(topCategory);
            return (true, "Üst kategori çöp kutusundan başarıyla geri getirildi.", topCategory.Id);
        }
        public async Task<(bool IsSuccess, string Message)> TPermanentDeleteTopCategoryAsync(int id)
        {
            var topCategory = await _topCategoryRepository.GetByIdAsync(id);
            if (topCategory == null)
                return (false, "Üst kategori bulunamadı.");

            if (!topCategory.IsDeleted)
                return (false, "Üst kategori silinmemiş. Önce silmeniz gerekir.");

            await _topCategoryRepository.DeleteAsync(topCategory.Id);
            return (true, "Üst kategori kalıcı olarak silindi.");
        }

        public async Task<List<ResultTopCategoryDto>> TGetDeletedTopCategoriesAsync()
        {
            var entities = await _topCategoryRepository.GetDeletedTopCategoriesAsync();
            return _mapper.Map<List<ResultTopCategoryDto>>(entities);
        }

        public async Task<List<ResultTopCategoryDto>> TGetActiveTopCategoriesAsync()
        {
            var entities = await _topCategoryRepository.GetActiveTopCategoriesAsync();
            return _mapper.Map<List<ResultTopCategoryDto>>(entities);
        }

        public async Task<List<(int Id, bool IsSuccess, string Message)>> TSoftDeleteRangeTopCategoryAsync(List<int> ids)
        {
            var results = new List<(int Id, bool IsSuccess, string Message)>();
            var categoriesToDelete = new List<TopCategory>();

            var usedCategoryNames = new List<string>();
            var notFoundIds = new List<int>();

            foreach (var id in ids)
            {
                var category = await _topCategoryRepository.GetByIdAsync(id);
                if (category == null)
                {
                    notFoundIds.Add(id);
                    continue;
                }

                var isUsedInSubCategory = await _subCategoryRepository.AnyAsync(x => x.TopCategoryId == id && !x.IsDeleted);
                if (isUsedInSubCategory)
                {
                    usedCategoryNames.Add(category.Name);
                    continue;
                }

                if (category.IsDeleted)
                    continue; // Zaten silinmişse işle

                categoriesToDelete.Add(category); // Silinmeye uygun kategori
            }

            // Eğer hatalı veya engelli varsa işlem iptal edilsin
            if (usedCategoryNames.Any() || notFoundIds.Any())
            {
                foreach (var category in categoriesToDelete)
                {
                    results.Add((category.Id, false, $"{category.Name} silinmeye uygundu ancak işlem iptal edildi."));
                }

                if (usedCategoryNames.Any())
                    results.Add((0, false, $"{string.Join(", ", usedCategoryNames)} kategorileri alt kategorilerde kullanıldığı için işlem yapılamadı.."));

                if (notFoundIds.Any())
                    results.Add((0, false, $"ID {string.Join(", ", notFoundIds)} kategorileri bulunamadı."));

                return results;
            }

            // Her şey yolundaysa, silme işlemlerini yap
            foreach (var category in categoriesToDelete)
            {
                category.IsDeleted = true;
                category.DeletedDate = DateTime.Now;
                await _topCategoryRepository.UpdateAsync(category);

                results.Add((category.Id, true, $"{category.Name} kategorisi başarıyla çöp kutusuna taşındı."));
            }

            return results;
        }

        public async Task<List<(int Id, bool IsSuccess, string Message)>> TUndoSoftDeleteRangeTopCategoryAsync(List<int> ids)
        {
            var results = new List<(int Id, bool IsSuccess, string Message)>();
            var validTopCategories = new List<TopCategory>();

            var nameConflicts = new List<string>();
            var alreadyActive = new List<string>();
            var notFound = new List<int>();

            foreach (var id in ids)
            {
                var category = await _topCategoryRepository.GetByIdAsync(id);
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

                var isDuplicate = await _topCategoryRepository.AnyAsync(x =>
                    x.Name == category.Name && !x.IsDeleted && x.Id != category.Id);

                if (isDuplicate)
                {
                    nameConflicts.Add(category.Name);
                    continue;
                }

                validTopCategories.Add(category); // geri alınabilir
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
            foreach (var category in validTopCategories)
            {
                category.IsDeleted = false;
                category.DeletedDate = null;
                await _topCategoryRepository.UpdateAsync(category);
                results.Add((category.Id, true, $"{category.Name} kategorisi başarıyla geri getirildi."));
            }

            return results;
        }

        public async Task TPermanentDeleteRangeTopCategoryAsync(List<int> ids)
        {
            await _topCategoryRepository.PermanentDeleteRangeAsync(ids);
        }

    }
}
