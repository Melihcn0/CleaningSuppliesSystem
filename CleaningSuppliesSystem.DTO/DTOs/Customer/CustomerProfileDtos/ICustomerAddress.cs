using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerProfileDtos
{
    public interface ICustomerAddress
    {
        int Id { get; }
        string AddressTitle { get; }
        string Address { get; }
        string City { get; }
        string District { get; }
        bool IsDefault { get; }
        AddressType Type { get; }
    }
}
