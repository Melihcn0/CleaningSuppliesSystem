using CleaningSuppliesSystem.Entity.Entities;

public class InvoiceItem
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public Invoice Invoice { get; set; }

    public string ProductName { get; set; }
    public decimal Quantity { get; set; }
    public string Unit { get; set; }

    public decimal UnitPrice { get; set; }          // Liste fiyatı (KDV hariç)
    public decimal TotalPrice { get; set; }         // KDV dahil toplam
    public decimal VatRate { get; set; }
    public decimal VatAmount { get; set; }

    public decimal? DiscountRate { get; set; }      // %
    public decimal? DiscountAmount { get; set; }    // TL

    // ✅ Burada yeni
    public decimal? DiscountedUnitPrice { get; set; } // İndirimli birim fiyat (KDV hariç)
}
