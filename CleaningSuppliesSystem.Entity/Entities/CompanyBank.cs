using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Entity.Entities
{
    public class CompanyBank
    {
        public int Id { get; set; }
        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public string? BankName { get; set; }
        public string? AccountHolder { get; set; }
        public string? IBAN { get; set; }

    }
}
