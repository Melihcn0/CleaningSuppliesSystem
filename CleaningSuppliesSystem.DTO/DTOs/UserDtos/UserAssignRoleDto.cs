using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.UserDtos
{
    public class UserAssignRoleDto
    {
        public int UserId { get; set; }
        public List<AssignRoleDto> Roles { get; set; }
    }

}
