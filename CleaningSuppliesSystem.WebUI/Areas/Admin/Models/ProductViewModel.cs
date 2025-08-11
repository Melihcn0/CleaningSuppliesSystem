using CleaningSuppliesSystem.DTO.DTOs.BrandDtos;
using CleaningSuppliesSystem.DTO.DTOs.ProductDtos;

namespace CleaningSuppliesSystem.WebUI.Areas.Admin.Models
{
    public class ProductViewModel
    {
        public List<ResultProductDto> ProductViewList { get; set; } = new();
    }
}
