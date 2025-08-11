using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.DiscountDtos
{
    public class UpdateDiscountDto
    {
        public int Id { get; set; }
        public decimal DiscountRate { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
