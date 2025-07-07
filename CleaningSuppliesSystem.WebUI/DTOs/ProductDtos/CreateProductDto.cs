using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.WebUI.DTOs.ProductDtos
{
    public class CreateProductDto
    {
        public string ImageUrl { get; set; }
        public string Name { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountRate { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;
        public int CategoryId { get; set; }
    }
}
