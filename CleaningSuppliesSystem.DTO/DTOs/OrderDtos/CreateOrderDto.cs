using CleaningSuppliesSystem.DTO.DTOs.InvoiceDtos;
using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.OrderDtos
{
    public class CreateOrderDto
    {
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
        public int AppUserId { get; set; }
    }
}
