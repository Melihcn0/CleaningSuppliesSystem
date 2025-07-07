using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.WebUI.DTOs.StockEntryDtos
{
    public class UpdateStockEntryDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime EntryDate { get; set; }
        public string? Description { get; set; }
        public bool IsDeleted { get; set; }
    }
}
