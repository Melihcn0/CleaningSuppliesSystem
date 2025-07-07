using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.WebUI.DTOs.OrderItemDtos
{
    public class UpdateOrderItemDto
    {
        public int Id { get; set; }            // Güncellenecek OrderItem ID’si
        public int Quantity { get; set; }      // Yeni adet
    }
}
