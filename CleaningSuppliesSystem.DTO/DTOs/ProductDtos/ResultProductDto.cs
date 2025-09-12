namespace CleaningSuppliesSystem.DTO.DTOs.ProductDtos
{
    public class ResultProductDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public string Name { get; set; }
        public decimal UnitPrice { get; set; } // birim fiyat
        public decimal TotalPrice { get; set; } // frontta hesaplama yapılıp gösterilen totalprice
        public decimal DiscountRate { get; set; } // indirim oranı
        public int? StockQuantity { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
        public int TopCategoryId { get; set; }
        public string TopCategoryName { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsShown { get; set; }
        public decimal VatRate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public decimal PriceWithVat => UnitPrice * (1 + VatRate / 100);

        // KDV dahil indirimli fiyat
        public decimal DiscountedPriceWithVat => PriceWithVat * (1 - DiscountRate / 100);
    }
}