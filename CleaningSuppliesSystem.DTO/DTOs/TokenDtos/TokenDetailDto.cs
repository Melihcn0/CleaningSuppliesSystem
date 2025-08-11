using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.TokenDtos
{
    public class TokenDetailDto
    {
        public string Token { get; set; }
        public DateTime ExpireDate { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }


}
