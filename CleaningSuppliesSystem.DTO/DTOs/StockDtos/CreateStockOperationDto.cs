using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.StockDtos
{
    public class CreateStockOperationDto
    {
        public int TopCategoryId { get; set; }
        public int SubCategoryId { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public bool? TransactionType { get; set; }
    }

}

