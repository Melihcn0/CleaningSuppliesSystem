using CleaningSuppliesSystem.DTO.DTOs.LocationDtos;

namespace CleaningSuppliesSystem.WebUI.Areas.Admin.Models
{
    public class LocationCityViewModel
    {
        public List<ResultLocationCityDto> CityViewList { get; set; } = new();
        public List<ResultLocationCityWithLocationDistrictDto> CityWithDistrictViewList { get; set; } = new();
    }
}
