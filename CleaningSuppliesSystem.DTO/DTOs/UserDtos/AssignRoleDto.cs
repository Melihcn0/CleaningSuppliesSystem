using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.UserDtos
{
    public class AssignRoleDto
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public bool RoleExist { get; set; }
    }
}
