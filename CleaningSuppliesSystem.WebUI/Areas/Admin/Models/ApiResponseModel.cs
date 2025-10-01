using CleaningSuppliesSystem.DTO.DTOs.Home.ContactDtos;
using CleaningSuppliesSystem.WebUI.Models;

namespace CleaningSuppliesSystem.WebUI.Areas.Admin.Models
{
    public class ApiResponseModel
    {
        public int ActiveCount { get; set; }
        public int DeletedCount { get; set; }
        public PagedResponse<ResultContactDto> PagedContacts { get; set; }
    }

}
