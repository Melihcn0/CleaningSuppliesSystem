using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Business.Abstract
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task NewUserMailAsync(string username, string email);
        Task SendUserWelcomeMailAsync(string username, string email);
        Task SendTwoFactorCodeMailAsync(string username, string email, string token);
        Task SendTwoFactorStatusChangedMailAsync(string username, string email, bool isEnabled);
        Task PassiveUserLoginMailAsync(string username, string email);
        Task SendAccountActivationMailAsync(string username, string email);
        Task SendPasswordResetMailAsync(string username, string email);
        Task SendPasswordResetMailLinkAsync(string username, string email, string token);
    }
}
