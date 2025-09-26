using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.Home.PromoAlertDtos
{
    public class ResultPromoAlertDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsShown { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }
    }

}
