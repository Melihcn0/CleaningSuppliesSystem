using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerProfileDtos
{
    public class CreateCustomerAddressDto
    {
        public string AddressTitle { get; set; }
        public string Address { get; set; }
        public int AppUserId { get; set; }
    }
}
