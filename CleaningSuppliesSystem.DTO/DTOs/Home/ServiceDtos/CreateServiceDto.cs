using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.Home.ServiceDtos
{
    public class CreateServiceDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int ServiceIconId { get; set; }
        public bool IsShown { get; set; }
    }
}
