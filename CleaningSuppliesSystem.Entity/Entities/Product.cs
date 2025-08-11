using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Entity.Entities
{
    public class Product
    {
            public int Id { get; set; }
            public string Name { get; set; }
            public string? ImageUrl { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal DiscountRate { get; set; }
            public decimal DiscountedPrice { get; set; }
            public int? StockQuantity { get; set; }
            public DateTime CreatedDate { get; set; }
            public DateTime? UpdatedDate { get; set; }
            public DateTime? DeletedDate { get; set; }
            public bool IsDeleted { get; set; }
            public bool IsShown { get; set; }
            public int BrandId { get; set; }
            public Brand Brand { get; set; }

        }
}
