using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.FinanceDtos
{
    public class ResultFinanceDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime Date { get; set; }
    }
}
