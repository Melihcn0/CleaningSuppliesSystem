// LoginResultDto.cs
using Microsoft.AspNetCore.Identity;

namespace CleaningSuppliesSystem.DTO.DTOs.UserDtos
{
    public class LoginResultDto
    {
        public bool Succeeded { get; set; }
        public List<IdentityError> Errors { get; set; } = new();
        public string Role { get; set; } = string.Empty;
    }
}
