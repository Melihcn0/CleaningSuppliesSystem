using CleaningSuppliesSystem.Entity.Entities;
using CleaningSuppliesSystem.WebUI.DTOs.InvoiceDtos;
using CleaningSuppliesSystem.WebUI.DTOs.OrderItemDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.WebUI.DTOs.OrderDtos
{
    public class ResultOrderDto
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
        public ICollection<ResultOrderItemDto> OrderItems { get; set; }
        public ResultInvoiceDto Invoice { get; set; }
    }
}
