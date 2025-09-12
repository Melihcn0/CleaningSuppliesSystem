using System;

namespace CleaningSuppliesSystem.Entity.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? ImageUrl { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountRate { get; set; }
        public int? StockQuantity { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsShown { get; set; }
        public int BrandId { get; set; }
        public int CategoryId { get; set; }
        public Brand Brand { get; set; }
        public int VatRate { get; set; }
        public decimal PriceWithVat => UnitPrice * (1 + VatRate / 100);
        public decimal DiscountedPriceWithVat => PriceWithVat * (1 - DiscountRate / 100);
    }

}
