using CleaningSuppliesSystem.DTO.DTOs.UserDtos;

namespace CleaningSuppliesSystem.WebUI.Areas.Developer.Models
{
    public class RolesIndexViewModel
    {
        public List<UserViewModel> UserList { get; set; }
        public List<AssignRoleDto> AssignRoleList { get; set; }
    }
}
