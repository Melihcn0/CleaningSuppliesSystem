using CleaningSuppliesSystem.DTO.DTOs.CategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.ProductDtos;

namespace CleaningSuppliesSystem.WebUI.Models
{
    public class ProductListViewModel
    {
        public PagedResponse<ResultProductDto> PagedProducts { get; set; }
        public List<ResultCategoryDto> Categories { get; set; }
        public int? SelectedCategoryId { get; set; }
    }
}
