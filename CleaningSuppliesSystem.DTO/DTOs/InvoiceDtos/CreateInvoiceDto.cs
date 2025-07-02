using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.InvoiceDtos
{
    public class CreateInvoiceDto
    {
        public DateTime GeneratedAt { get; set; }
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
        public int OrderId { get; set; }
    }
}
