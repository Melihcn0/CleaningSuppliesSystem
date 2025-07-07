using CleaningSuppliesSystem.Entity.Entities;
using CleaningSuppliesSystem.WebUI.DTOs.ProductDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.WebUI.DTOs.StockEntryDtos
{
    public class ResultStockEntryDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public ResultProductDto Product { get; set; }
        public int Quantity { get; set; }
        public DateTime EntryDate { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; }
        public string? Description { get; set; }
    }
}
