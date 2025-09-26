using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.OrderDtos
{
    public class ExpiredOrderDto
    {
        public string OrderNumber { get; set; }
        public string CustomerNameSurname { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public int DiffDays { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

}
