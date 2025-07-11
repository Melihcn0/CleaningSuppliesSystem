using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.MailDtos;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Business.Concrete
{
    public class MailManager : IMailService
    {
        private readonly IConfiguration _config;

        public MailManager(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(MailRequestDto request)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Ess Star Tedarik", _config["Mail:From"]));
            email.To.Add(MailboxAddress.Parse(request.ToEmail));
            email.Subject = request.Subject;

            var builder = new BodyBuilder();
            if (request.IsHtml)
                builder.HtmlBody = request.Body;
            else
                builder.TextBody = request.Body;

            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_config["Mail:SmtpHost"], int.Parse(_config["Mail:SmtpPort"]), SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_config["Mail:Username"], _config["Mail:Password"]);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
