using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.SubCategoryDtos
{
    public class UpdateSubCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? TopCategoryId { get; set; }
    }
}
