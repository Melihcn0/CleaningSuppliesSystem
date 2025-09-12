using CleaningSuppliesSystem.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.Admin.CompanyAddressDtos
{
    public class CompanyAddressDto
    {
        public int Id { get; set; }
        public int AppUserId { get; set; }
        public string? CompanyName { get; set; }
        public string? TaxOffice { get; set; }
        public string? TaxNumber { get; set; }
        public string? Address { get; set; }
        public string? CityName { get; set; }
        public string? DistrictName { get; set; }
    }
}
