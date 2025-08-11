using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.Home.ServiceIconDtos
{
    public class UpdateServiceIconDto
    {
        public int Id { get; set; }
        public string IconName { get; set; }
        public string IconUrl { get; set; }
    }
}
