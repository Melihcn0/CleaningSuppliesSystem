using CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerProfileDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerCorporateDtos
{
    public class CustomerCorporateAddressDto : ICustomerAddress
    {
        public int Id { get; set; }
        public string AddressTitle { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public bool IsDefault { get; set; }
        public string CompanyName { get; set; }
        public string TaxOffice { get; set; }
        public string TaxNumber { get; set; }
        public AddressType Type => AddressType.Corporate;
    }
}
