using CleaningSuppliesSystem.DTO.DTOs.CategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.OrderItemDtos;
using CleaningSuppliesSystem.DTO.DTOs.StockEntryDtos;
using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.ProductDtos
{
    public class UpdateProductDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public string Name { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountRate { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CategoryId { get; set; }
    }
}
