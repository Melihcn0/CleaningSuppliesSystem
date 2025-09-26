using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.Home.PromoAlertDtos
{
    public class UpdatePromoAlertDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
    }
}
