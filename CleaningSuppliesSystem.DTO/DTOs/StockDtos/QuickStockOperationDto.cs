using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.StockDtos
{
    public class QuickStockOperationDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public bool TransactionType { get; set; }
    }
}
