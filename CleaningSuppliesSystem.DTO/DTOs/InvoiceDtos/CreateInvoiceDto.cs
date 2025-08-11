using CleaningSuppliesSystem.DTO.DTOs.OrderDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.InvoiceDtos
{
    public class CreateInvoiceDto
    {
        public int OrderId { get; set; }
        public DateTime GeneratedAt { get; set; }

        // Eğer Order’dan bazı bilgileri göstermek istersen ekle:
        public ResultOrderDto? Order { get; set; }
    }
}
