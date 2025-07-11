using CleaningSuppliesSystem.DTO.DTOs.MailDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Business.Abstract
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequestDto request);
    }
}
