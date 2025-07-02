using CleaningSuppliesSystem.DTO.DTOs.InvoiceDtos;
using CleaningSuppliesSystem.DTO.DTOs.OrderItemDtos;
using CleaningSuppliesSystem.DTO.DTOs.PaymentDtos;
using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.OrderDtos
{
    public class ResultOrderDto
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
        public ICollection<ResultOrderItemDto> OrderItems { get; set; }
        public ResultPaymentDto Payment { get; set; }
        public ResultInvoiceDto Invoice { get; set; }
    }
}
