using CleaningSuppliesSystem.Entity.Entities;
using CleaningSuppliesSystem.WebUI.DTOs.CategoryDtos;
using CleaningSuppliesSystem.WebUI.DTOs.OrderItemDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.WebUI.DTOs.ProductDtos
{
    public class ResultProductDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public string Name { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public int CategoryId { get; set; }
        public ResultCategoryDto Category { get; set; }
        public ICollection<ResultOrderItemDto> OrderItems { get; set; }
    }
}
