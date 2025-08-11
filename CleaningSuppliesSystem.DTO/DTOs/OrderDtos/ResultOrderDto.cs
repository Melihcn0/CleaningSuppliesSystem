    using CleaningSuppliesSystem.DTO.DTOs.InvoiceDtos;
    using CleaningSuppliesSystem.DTO.DTOs.OrderItemDtos;
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
            public string Status { get; set; }
            public int AppUserId { get; set; } // Siparişi veren kullanıcı
            public string FirstName { get; set; }  // Kullanıcının adı
            public string LastName { get; set; }   // Kullanıcının soyadı
            public string OrderNumber { get; set; }
            public ICollection<ResultOrderItemDto> OrderItems { get; set; }
            public InvoiceDto Invoice { get; set; }
            public DateTime CreatedDate { get; set; }
            public DateTime? ApprovedDate { get; set; }       // Onaylandı
            public DateTime? PreparingDate { get; set; }       // Hazırlanıyor
            public DateTime? ShippedDate { get; set; }         // Kargoya Verildi
            public DateTime? DeliveredDate { get; set; }       // Teslim Edildi
            public DateTime? CanceledDate { get; set; }        // İptal Edildi
            public DateTime? UpdatedDate { get; set; }
        }
    }
