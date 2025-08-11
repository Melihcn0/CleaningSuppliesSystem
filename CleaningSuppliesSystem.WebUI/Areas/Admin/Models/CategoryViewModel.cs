using CleaningSuppliesSystem.DTO.DTOs.CategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.StockDtos;
using System.Globalization;

namespace CleaningSuppliesSystem.WebUI.Areas.Admin.Models
{
    public class CategoryViewModel
    {
        public List<ResultCategoryDto> CategoryViewList { get; set; } = new();
    }
}
