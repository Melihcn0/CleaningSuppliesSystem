using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DataAccess.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.CategoryDtos;
using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Business.Concrete
{
    public class CategoryManager : GenericManager<Category>, ICategoryService
    {
        private readonly ICategoryRepository _CategoryRepository;
        private readonly IMapper _mapper;

        public CategoryManager(IRepository<Category> repository, ICategoryRepository categoryRepository, IMapper mapper)
        : base(repository)
        {
            _CategoryRepository = categoryRepository;
            _mapper = mapper;
        }
        public async Task<(bool IsSuccess, string Message)> TCreateCategoryAsync(CreateCategoryDto createCategoryDto)
        {
            var Category = _mapper.Map<Category>(createCategoryDto);
            await _CategoryRepository.CreateAsync(Category);
            return (true, "Kategori başarıyla oluşturuldu.");
        }
        public async Task<(bool IsSuccess, string Message)> TUpdateCategoryAsync(UpdateCategoryDto updateCategoryDto)
        {
            var Category = await _CategoryRepository.GetByIdAsync(updateCategoryDto.Id);
            if (Category == null)
                return (false, "Kategori bulunamadı.");
            _mapper.Map(updateCategoryDto, Category);
            await _CategoryRepository.UpdateAsync(Category);
            return (true, "Kategori başarıyla güncellendi.");
        }
        public async Task<(bool IsSuccess, string Message)> TSoftDeleteCategoryAsync(int id)
        {
            var Category = await _CategoryRepository.GetByIdAsync(id);
            if (Category == null)
                return (false, "Kategori bulunamadı.");

            if (Category.IsDeleted)
                return (false, "Kategori zaten silinmiş durumda.");

            await _CategoryRepository.SoftDeleteAsync(Category);
            return (true, "Kategori başarıyla silindi.");
        }
        public async Task<(bool IsSuccess, string Message)> TUndoSoftDeleteCategoryAsync(int id)
        {
            var Category = await _CategoryRepository.GetByIdAsync(id);
            if (Category == null)
                return (false, "Kategori bulunamadı.");

            if (!Category.IsDeleted)
                return (false, "Kategori zaten aktif durumda.");

            await _CategoryRepository.UndoSoftDeleteAsync(Category);
            return (true, "Kategori başarıyla geri getirildi.");
        }
    }
}
