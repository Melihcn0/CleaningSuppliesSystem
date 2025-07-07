using CleaningSuppliesSystem.Entity.Entities;
using CleaningSuppliesSystem.WebUI.DTOs.OrderDtos;
using CleaningSuppliesSystem.WebUI.DTOs.ProductDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.WebUI.DTOs.OrderItemDtos
{
    public class ResultOrderItemDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public ResultOrderDto Order { get; set; }
        public int ProductId { get; set; }
        public ResultProductDto Product { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
