using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.Admin.CompanyBankDtos
{
    public class UpdateCompanyBankDto
    {
        public int Id { get; set; }
        public string BankName { get; set; }
        public string AccountHolder { get; set; }
        public string IBAN { get; set; }
    }
}
