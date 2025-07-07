using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.CategoryDtos
{
    public class CreateCategoryDto
    {
        public string ImageUrl { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
    }
}
