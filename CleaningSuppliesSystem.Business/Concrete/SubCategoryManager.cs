using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.DataAccess.Concrete;
using CleaningSuppliesSystem.DTO.DTOs.ProductDtos;
using CleaningSuppliesSystem.DTO.DTOs.SubCategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.TopCategoryDtos;
using CleaningSuppliesSystem.Entity.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using static CleaningSuppliesSystem.Business.Validators.Validators;

namespace CleaningSuppliesSystem.Business.Concrete
{
    public class SubCategoryManager : GenericManager<SubCategory>, ISubCategoryService
    {
        private readonly ISubCategoryRepository _subCategoryRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public SubCategoryManager(
            IRepository<SubCategory> repository,
            ISubCategoryRepository subCategoryRepository, 
            ICategoryRepository categoryRepository,
            IMapper mapper)
            : base(repository)
        {
            _subCategoryRepository = subCategoryRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<(bool IsSuccess, string Message, int CreatedId)> TCreateSubCategoryAsync(CreateSubCategoryDto createSubCategoryDto)
        {
            var validator = new CreateSubCategoryValidator();
            var validationResult = await validator.ValidateAsync(createSubCategoryDto);
            if (!validationResult.IsValid)
            {
                var message = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
                return (false, message, 0);
            }
            var subCategory = _mapper.Map<SubCategory>(createSubCategoryDto);
            subCategory.CreatedDate = DateTime.Now;
            await _subCategoryRepository.CreateAsync(subCategory);
            return (true, "Ürün başarıyla oluşturuldu.", subCategory.Id);
        }
        public async Task<(bool IsSuccess, string Message, int UpdatedId)> TUpdateSubCategoryAsync(UpdateSubCategoryDto updateSubCategoryDto)
        {
            var validator = new UpdateSubCategoryValidator();
            var validationResult = await validator.ValidateAsync(updateSubCategoryDto);
            if (!validationResult.IsValid)
            {
                var message = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
                return (false, message, 0);
            }
            var subCategory = await _subCategoryRepository.GetByIdAsync(updateSubCategoryDto.Id);
            if (subCategory == null)
                return (false, "Alt kategori bulunamadı.", 0);

            _mapper.Map(updateSubCategoryDto, subCategory);
            subCategory.UpdatedDate = DateTime.Now;

            await _subCategoryRepository.UpdateAsync(subCategory);
            return (true, "Alt kategori başarıyla güncellendi.", subCategory.Id);
        }
        public async Task<(bool IsSuccess, string Message, int SoftDeletedId)> TSoftDeleteSubCategoryAsync(int id)
        {
            var subCategory = await _subCategoryRepository.GetByIdAsync(id);
            if (subCategory == null)
                return (false, "Alt kategori bulunamadı.", 0);

            if (subCategory.IsDeleted)
                return (false, "Alt kategori zaten silinmiş.", subCategory.Id);

            // Category içinde kullanılıyor mu?
            var isUsedInCategories = await _categoryRepository.AnyAsync(x => x.SubCategoryId == id && !x.IsDeleted);
            if (isUsedInCategories)
                return (false, "Bu alt kategori hâlâ ürün grubu kategorilerinde kullanılıyor. Silme işlemi yapılamaz.", subCategory.Id);

            subCategory.IsDeleted = true;
            subCategory.DeletedDate = DateTime.Now;

            await _subCategoryRepository.UpdateAsync(subCategory);
            return (true, "Alt kategori başarıyla silindi.", subCategory.Id);
        }
        public async Task<(bool IsSuccess, string Message, int UndoSoftDeletedId)> TUndoSoftDeleteSubCategoryAsync(int id)
        {
            var subCategory = await _subCategoryRepository.GetByIdAsync(id);
            if (subCategory == null)
                return (false, "Alt kategori bulunamadı.", 0);

            if (!subCategory.IsDeleted)
                return (false, "Alt kategori zaten aktif.", subCategory.Id);

            var isDuplicate = await _subCategoryRepository.AnyAsync(x =>
                x.Name == subCategory.Name &&
                x.IsDeleted == false &&
                x.Id != subCategory.Id);

            if (isDuplicate)
                return (false, $"'{subCategory.Name}' isminde başka bir aktif alt kategori mevcut. Geri alma yapılamadı.", subCategory.Id);

            subCategory.IsDeleted = false;
            subCategory.DeletedDate = null;

            await _subCategoryRepository.UpdateAsync(subCategory);
            return (true, "Alt kategori başarıyla geri getirildi.", subCategory.Id);
        }
        public async Task<(bool IsSuccess, string Message)> TPermanentDeleteSubCategoryAsync(int id)
        {
            var subCategory = await _subCategoryRepository.GetByIdAsync(id);
            if (subCategory == null)
                return (false, "Alt kategori bulunamadı.");

            if (!subCategory.IsDeleted)
                return (false, "Alt kategori soft silinmemiş. Önce soft silmeniz gerekir.");

            await _subCategoryRepository.DeleteAsync(subCategory.Id);
            return (true, "Alt kategori kalıcı olarak silindi.");
        }
        public async Task<List<ResultSubCategoryDto>> TGetActiveByTopCategoryIdAsync(int topCategoryId)
        {
            var entities = await _subCategoryRepository.GetActiveByTopCategoryIdAsync(topCategoryId);
            return _mapper.Map<List<ResultSubCategoryDto>>(entities);
        }
        public async Task<List<ResultSubCategoryDto>> TGetActiveSubCategoriesAsync()
        {
            var entities = await _subCategoryRepository.GetActiveSubCategoriesAsync();
            return _mapper.Map<List<ResultSubCategoryDto>>(entities);
        }
        public async Task<List<ResultSubCategoryDto>> TGetDeletedSubCategoriesAsync()
        {
            var entities = await _subCategoryRepository.GetDeletedSubCategoriesAsync();
            return _mapper.Map<List<ResultSubCategoryDto>>(entities);
        }
        public async Task<List<(int Id, bool IsSuccess, string Message)>> TSoftDeleteRangeSubCategoryAsync(List<int> ids)
        {
            var results = new List<(int Id, bool IsSuccess, string Message)>();
            var validSubCategories = new List<SubCategory>();

            var usedInCategories = new List<string>();
            var notFound = new List<int>();
            var alreadyDeleted = new List<string>();

            foreach (var id in ids)
            {
                var subCategory = await _subCategoryRepository.GetByIdAsync(id);
                if (subCategory == null)
                {
                    notFound.Add(id);
                    continue;
                }

                if (subCategory.IsDeleted)
                {
                    alreadyDeleted.Add(subCategory.Name);
                    continue;
                }

                var isUsed = await _categoryRepository.AnyAsync(x => x.SubCategoryId == id && !x.IsDeleted);
                if (isUsed)
                {
                    usedInCategories.Add(subCategory.Name);
                    continue;
                }

                validSubCategories.Add(subCategory);
            }

            // Eğer herhangi bir sorun varsa, hiçbirini silme
            if (usedInCategories.Any() || notFound.Any() || alreadyDeleted.Any())
            {
                if (usedInCategories.Any())
                    results.Add((0, false, $"{string.Join(", ", usedInCategories)} alt kategorileri ürün grubu kategorilerinde kullanıldığı için işlem yapılamadı."));

                if (alreadyDeleted.Any())
                    results.Add((0, false, $"{string.Join(", ", alreadyDeleted)} alt kategorileri zaten silinmiş."));

                if (notFound.Any())
                    results.Add((0, false, $"ID {string.Join(", ", notFound)} alt kategorileri bulunamadı."));

                return results;
            }

            // Her şey uygunsa, silme işlemlerini yap
            foreach (var subCategory in validSubCategories)
            {
                subCategory.IsDeleted = true;
                subCategory.DeletedDate = DateTime.Now;
                await _subCategoryRepository.UpdateAsync(subCategory);
                results.Add((subCategory.Id, true, $"'{subCategory.Name}' alt kategorisi başarıyla silindi."));
            }

            return results;
        }

        public async Task<List<(int Id, bool IsSuccess, string Message)>> TUndoSoftDeleteRangeSubCategoryAsync(List<int> ids)
        {
            var results = new List<(int Id, bool IsSuccess, string Message)>();
            var validSubCategories = new List<SubCategory>();

            var nameConflicts = new List<string>();
            var alreadyActive = new List<string>();
            var notFound = new List<int>();

            foreach (var id in ids)
            {
                var subCategory = await _subCategoryRepository.GetByIdAsync(id);
                if (subCategory == null)
                {
                    notFound.Add(id);
                    continue;
                }

                if (!subCategory.IsDeleted)
                {
                    alreadyActive.Add(subCategory.Name);
                    continue;
                }

                var isDuplicate = await _subCategoryRepository.AnyAsync(x =>
                    x.Name == subCategory.Name && !x.IsDeleted && x.Id != subCategory.Id);

                if (isDuplicate)
                {
                    nameConflicts.Add(subCategory.Name);
                    continue;
                }

                validSubCategories.Add(subCategory);
            }

            if (nameConflicts.Any() || alreadyActive.Any() || notFound.Any())
            {
                if (nameConflicts.Any())
                    results.Add((0, false, $"{string.Join(", ", nameConflicts)} alt kategorileri isim çakışması nedeniyle geri alınamadı."));

                if (alreadyActive.Any())
                    results.Add((0, false, $"{string.Join(", ", alreadyActive)} alt kategorileri zaten aktif durumda."));

                if (notFound.Any())
                    results.Add((0, false, $"ID {string.Join(", ", notFound)} alt kategorileri bulunamadı."));

                return results;
            }

            foreach (var subCategory in validSubCategories)
            {
                subCategory.IsDeleted = false;
                subCategory.DeletedDate = null;
                await _subCategoryRepository.UpdateAsync(subCategory);
                results.Add((subCategory.Id, true, $"'{subCategory.Name}' alt kategorisi başarıyla geri getirildi."));
            }

            return results;
        }
        public async Task TPermanentDeleteRangeSubCategoryAsync(List<int> ids)
        {
            await _subCategoryRepository.PermanentDeleteRangeAsync(ids);
        }

        
    }
}
