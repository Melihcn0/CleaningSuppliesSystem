using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.ToggleDtos
{
    public class ToggleStatusDto
    {
        public int UserId { get; set; }
        public bool NewStatus { get; set; }
    }

}
