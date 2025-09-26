using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Entity.Entities
{
    public class PromoAlert
    {
        public int Id { get; set; }
        public string Title { get; set; } // Başlık
        public string Description { get; set; } // Açıklama
        public string Icon { get; set; } // question, error, success, warning, info vb.
        public bool IsShown { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }
    }
}
