using CleaningSuppliesSystem.Entity.Entities;
using CleaningSuppliesSystem.WebUI.DTOs.OrderDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.WebUI.DTOs.InvoiceDtos
{
    public class ResultInvoiceDto
    {
        public int Id { get; set; }
        public DateTime GeneratedAt { get; set; }
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
        public int OrderId { get; set; }
        public ResultOrderDto Order { get; set; }
    }
}
