using CleaningSuppliesSystem.DTO.DTOs.BrandDtos;

namespace CleaningSuppliesSystem.WebUI.Areas.Admin.Models
{
    public class BrandViewModel
    {
        public List<ResultBrandDto> BrandViewList { get; set; } = new();
    }
}
