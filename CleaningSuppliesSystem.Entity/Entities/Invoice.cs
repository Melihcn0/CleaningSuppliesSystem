using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Entity.Entities
{
    public class Invoice
    {
        public int Id { get; set; }
        public DateTime GeneratedAt { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
    }

}
