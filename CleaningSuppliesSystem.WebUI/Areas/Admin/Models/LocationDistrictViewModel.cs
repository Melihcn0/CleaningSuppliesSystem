using CleaningSuppliesSystem.DTO.DTOs.LocationDtos;

namespace CleaningSuppliesSystem.WebUI.Areas.Admin.Models
{
    public class LocationDistrictViewModel
    {
        public List<ResultLocationDistrictDto> DistrictViewList { get; set; } = new();
    }
}
