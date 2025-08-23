using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerIndivivualDtos
{
    public class CreateCustomerIndividualAddressDto
    {
        public string AddressTitle { get; set; }
        public string Address { get; set; }
        public int? CityId { get; set; }
        public int DistrictId { get; set; }
        public int AppUserId { get; set; }
        public string CityName { get; set; }
        public string DistrictName { get; set; }
    }
}
