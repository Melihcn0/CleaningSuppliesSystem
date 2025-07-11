namespace CleaningSuppliesSystem.WebUI.Services.EmailServices
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task NewUserMailAsync(string username, string email);
        Task SendUserWelcomeMailAsync(string username, string email);
        Task SendTwoFactorCodeMailAsync(string username, string email, string token);
        Task SendTwoFactorStatusChangedMailAsync(string username, string email, bool isEnabled);
        Task PassiveUserLoginAlertAsync(string username, string email);
        Task SendAccountActivationMailAsync(string username, string email);
        Task SendPasswordResetMailAsync(string username, string email);
        Task SendPasswordResetMailLinkAsync(string username, string email, string token);
    }
}
