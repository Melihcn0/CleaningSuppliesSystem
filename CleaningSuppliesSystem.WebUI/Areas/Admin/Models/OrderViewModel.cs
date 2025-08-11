using CleaningSuppliesSystem.DTO.DTOs.InvoiceDtos;
using CleaningSuppliesSystem.DTO.DTOs.OrderItemDtos;

namespace CleaningSuppliesSystem.WebUI.Areas.Admin.Models
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }

        // Direkt isim-soyisim
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public InvoiceDto Invoice { get; set; }
    }


}
