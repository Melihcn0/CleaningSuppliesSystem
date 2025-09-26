using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.UserDtos
{
    public class UserListDto
    {
        public int Id { get; set; }
        public string NameSurname { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public List<string> Role { get; set; }
    }
}
