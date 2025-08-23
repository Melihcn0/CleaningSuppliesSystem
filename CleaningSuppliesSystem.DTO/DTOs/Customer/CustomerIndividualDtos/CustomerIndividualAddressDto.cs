using CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerProfileDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerIndivivualDtos
{
    public class CustomerIndividualAddressDto : ICustomerAddress
    {
        public int Id { get; set; }
        public string AddressTitle { get; set; }
        public string Address { get; set; }
        public string CityName { get; set; }
        public string DistrictName { get; set; }
        public bool IsDefault { get; set; }
        public int CityId { get; set; }       // Şehir Id
        public int DistrictId { get; set; }   // İlçe Id
        public AddressType Type => AddressType.Individual;
    }
}
