using CleaningSuppliesSystem.DTO.DTOs.ProductDtos;
using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.StockEntryDtos
{
    public class ResultStockEntryDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public ResultProductDto Product { get; set; }
        public int Quantity { get; set; }
        public DateTime EntryDate { get; set; }
        public string? Description { get; set; }
        public bool IsDeleted { get; set; }
    }
}
