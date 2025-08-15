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

        // Fatura için snapshot alınacak adres bilgileri
        public string FullAddress { get; set; }
        public string AddressTitle { get; set; }
        public string RecipientName { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Neighborhood { get; set; }
        public string Street { get; set; }
        public string BuildingNumber { get; set; }
        public string ApartmentNumber { get; set; }
        public string PostalCode { get; set; }

        // Fatura için kullanıcı bilgileri (snapshot)
        public string CustomerName { get; set; }
        public string CustomerTaxNumber { get; set; }
        public string CustomerTaxOffice { get; set; }
    }
}
