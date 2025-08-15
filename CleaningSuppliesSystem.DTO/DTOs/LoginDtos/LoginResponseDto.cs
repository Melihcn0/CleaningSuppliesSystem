using CleaningSuppliesSystem.DTO.DTOs.TokenDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.DTO.DTOs.LoginDtos
{
    public class LoginResponseDto
    {
        public InnerTokenDto Token { get; set; }
        public string Message { get; set; }
        public string Theme { get; set; }
    }

}
