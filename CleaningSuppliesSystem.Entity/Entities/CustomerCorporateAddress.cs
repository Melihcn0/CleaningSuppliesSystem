using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Entity.Entities
{
    public class CustomerCorporateAddress
    {
        public int Id { get; set; }
        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public string? CompanyName { get; set; }
        public string? TaxOffice { get; set; }
        public string? TaxNumber { get; set; }
        public string? AddressTitle { get; set; }
        public string? Address { get; set; }

        public int CityId { get; set; }           // <-- ekle
        public string? CityName { get; set; }

        public int DistrictId { get; set; }       // <-- ekle
        public string? DistrictName { get; set; }

        public bool IsDefault { get; set; } = false;
    }

}
