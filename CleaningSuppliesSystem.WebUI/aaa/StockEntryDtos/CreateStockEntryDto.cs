using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.WebUI.DTOs.StockEntryDtos
{
    public class CreateStockEntryDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime EntryDate { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;
        public string? Description { get; set; }
    }
}
