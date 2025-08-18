using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerIndivivualDtos
{
    public class CreateCustomerIndivivualAddressDto
    {
        public string AddressTitle { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public int AppUserId { get; set; }
    }
}
