using CleaningSuppliesSystem.Entity.Entities;
using Microsoft.AspNetCore.Http;
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
        public IFormFile? ImageFile { get; set; }
        public string? ImageUrl { get; set; }
        public string Name { get; set; }
        public decimal UnitPrice { get; set; }
        public int? BrandId { get; set; }
        public decimal VatRate { get; set; }
    }
}
