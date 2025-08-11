using CleaningSuppliesSystem.DTO.DTOs.BrandDtos;
using CleaningSuppliesSystem.DTO.DTOs.SubCategoryDtos;

namespace CleaningSuppliesSystem.WebUI.Areas.Admin.Models
{
    public class SubCategoryViewModel
    {
        public List<ResultSubCategoryDto> SubCategoryViewList { get; set; } = new();
    }
}
