using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.Customer.OrderItemDtos
{
    public class AddToOrderDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
