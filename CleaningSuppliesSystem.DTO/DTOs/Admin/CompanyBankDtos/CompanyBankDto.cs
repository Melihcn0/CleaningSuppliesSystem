using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.Admin.CompanyBankDtos
{
    public class CompanyBankDto
    {
        public int Id { get; set; }
        public int AppUserId { get; set; }
        public string? BankName { get; set; }
        public string? AccountHolder { get; set; }
        public string? IBAN { get; set; }
    }
}
