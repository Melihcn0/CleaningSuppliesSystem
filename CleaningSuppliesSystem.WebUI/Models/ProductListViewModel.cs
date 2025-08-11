using CleaningSuppliesSystem.DTO.DTOs.CategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.ProductDtos;

namespace CleaningSuppliesSystem.WebUI.Models
{
    public class ProductListViewModel
    {
        public List<ResultProductDto> Products { get; set; }
        public List<ResultCategoryDto> Categories { get; set; }
        public int? SelectedCategoryId { get; set; }
    }
}
