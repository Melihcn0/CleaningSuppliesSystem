using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Entity.Entities
{
    public class Finance
    {
        public int Id { get; set; }             // Kayıt kimliği
        public string Title { get; set; }       // Açıklama: "Sipariş Geliri", "Kargo Masrafı" vs.
        public string Type { get; set; }        // "Income" veya "Expense"
        public decimal Amount { get; set; }      // Net para tutarı (her zaman pozitif yazılır)
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}
