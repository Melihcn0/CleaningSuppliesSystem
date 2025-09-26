using CleaningSuppliesSystem.Business.Abstract;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Business.Concrete
{
    public class EmailManager(IConfiguration _config) : IEmailService
    {
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            using var smtpClient = new SmtpClient(_config["Mail:Smtp"])
            {
                Port = int.Parse(_config["Mail:Port"]),
                Credentials = new NetworkCredential(
                    _config["Mail:Username"],
                    _config["Mail:Password"]
                ),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_config["Mail:Sender"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };

            mailMessage.To.Add(to);

            await smtpClient.SendMailAsync(mailMessage);
        }
        public async Task NewUserMailAsync(string username, string email)
        {
            var adminMail = _config["Mail:AdminReceiver"];
            var subject = "Yeni Kullanıcı Kaydı";
            var htmlBody = $@"
              <div style='font-family: Arial; background-color:#f9f9f9; padding: 20px; border-radius: 8px;'>
                <h2 style='color: #5bc0de;'>Yeni Kullanıcı Kaydı</h2>
                <p><strong>{username}</strong> adlı kullanıcı sisteme kayıt oldu.</p>
                <p>Email adresi: <strong>{email}</strong></p>
                <p>Lütfen yönetim panelinden hesabı onaylayın.</p>
                <hr style='margin-top:20px;'>
                <small style='color: gray;'>Bu mail ESS Star Tedarik sisteminden gönderilmiştir.</small>
              </div>";

            await SendEmailAsync(adminMail, subject, htmlBody);
        }
        public async Task SendUserWelcomeMailAsync(string username, string email)
        {
            try
            {
                var subject = "ESS Star Tedarik'e Hoş Geldiniz!";
                var htmlBody = $@"
          <div style='font-family: Arial; background-color:#f4f4f4; padding: 20px; border-radius: 8px;'>
            <h2 style='color: #5cb85c;'>Kayıt Başarılı 🎉</h2>
            <p>Merhaba <strong>{username}</strong>,</p>
            <p>Sisteme başarıyla kayıt oldunuz.</p>
            <p>Yetkili kişi hesabınızı onayladığında giriş yapabileceksiniz.</p>
            <p>Durumunuzla ilgili bilgilendirme e-posta yoluyla yapılacaktır.</p>
            <hr>
            <small>Bu mail otomatik olarak gönderilmiştir. Yanıtlamanıza gerek yoktur.</small>
          </div>";

                await SendEmailAsync(email, subject, htmlBody);
            }
            catch (Exception ex)
            {
                // Logla ki nerede patlıyor göresin
                Console.WriteLine($"[SendUserWelcomeMailAsync Hata]: {ex.Message}");

                // burayı swallow edebilirsin (hata fırlatmadan geçsin)
                // ya da throw ex; diyerek yukarıya geri fırlatabilirsin
            }
        }
        public async Task SendAccountActivationMailAsync(string username, string email)
        {
            var subject = "✅ Hesabınız Aktif Edildi";
            var htmlBody = $@"
              <div style='font-family: Arial; background-color:#e8faff; padding: 20px; border-radius: 8px;'>
                <h2 style='color: #007bff;'>Hesabınız Aktif!</h2>
                <p>Merhaba <strong>{username}</strong>,</p>
                <p>ESS Star Tedarik hesabınız yetkili kişi tarafından aktif hale getirildi.</p>
                <p>Artık giriş yaparak siparişlerinizi yönetebilir ve kampanyalardan faydalanabilirsiniz.</p>
                <a href='https://localhost:7020/Home/Index' style='display:inline-block; margin-top:10px; padding:10px 20px; background-color:#007bff; color:white; text-decoration:none; border-radius:5px;'>Giriş Yap</a>
                <hr>
                <small>Bu mail otomatik olarak gönderilmiştir. Yanıtlamanıza gerek yoktur.</small>
              </div>";

            await SendEmailAsync(email, subject, htmlBody);
        }
        public async Task SendTwoFactorCodeMailAsync(string username, string email, string token)
        {
            var subject = "🔐 ESS Star Tedarik 2FA Giriş Kodunuz";
            var htmlBody = $@"
            <div style='font-family: Arial, sans-serif; background-color:#fafafa; padding: 24px; border-radius: 8px; border: 1px solid #ddd;'>
            <h2 style='color: #d9534f;'>İki Aşamalı Giriş Kodu</h2>
            <p>Merhaba <strong>{username}</strong>,</p>
            <p>Giriş işleminizi doğrulamak için aşağıdaki kodu kullanınız:</p>
            <div style='background-color:#f7f7f7; padding: 12px; margin: 20px 0; text-align:center;
                        font-size: 26px; font-weight: bold; color: #333; letter-spacing: 2px;
                        border-radius: 6px; border: 1px dashed #aaa;'>
                {token}
            </div>
            <p style='color:#555;'>Kodun geçerlilik süresi: <strong>2 dakika</strong></p>
            <p>Bu işlem size ait değilse lütfen sistem yöneticisine hemen bildiriniz.</p>
            <hr style='margin-top:30px; border:none; border-top:1px solid #eee;'>
            <small style='color: gray;'>Bu e-posta ESS Star Tedarik güvenlik sistemi tarafından otomatik olarak gönderilmiştir. Lütfen yanıtlamayınız.</small>
            </div>
            ";

            await SendEmailAsync(email, subject, htmlBody);
        }
        public async Task SendTwoFactorStatusChangedMailAsync(string username, string email, bool isEnabled)
        {
            string subject;
            string htmlBody;

            if (isEnabled)
            {
                subject = "İki Aşamalı Giriş Başarıyla Aktif Edildi";
                htmlBody = $@"
                <div style='font-family: Arial; background-color:#e8f5e9; padding:20px; border-radius:8px;'>
                    <h2 style='color:#388e3c;'>Güvenlik Ayarınız Güncellendi 🔐</h2>
                    <p>Merhaba <strong>{username}</strong>,</p>
                    <p>İki aşamalı giriş özelliğini <strong>başarıyla aktif hale getirdiniz</strong>.</p>
                    <p>Girişlerde artık ek doğrulama kodu gerekecektir.</p>
                    <hr>
                    <small style='color:gray;'>ESS Star Tedarik sistemi tarafından otomatik olarak gönderilmiştir.</small>
                </div>";
            }
            else
            {
                subject = "İki Aşamalı Giriş Devre Dışı Bırakıldı";
                htmlBody = $@"
                <div style='font-family: Arial; background-color:#fff3cd; padding:20px; border-radius:8px;'>
                    <h2 style='color:#856404;'>Güvenlik Ayarınız Güncellendi 🛡️</h2>
                    <p>Merhaba <strong>{username}</strong>,</p>
                    <p>İki aşamalı giriş özelliğini <strong>devre dışı bıraktınız</strong>.</p>
                    <p>Artık girişlerde ek doğrulama yapılmayacaktır.</p>
                    <hr>
                    <small style='color:gray;'>ESS Star Tedarik sistemi tarafından otomatik olarak gönderilmiştir.</small>
                </div>";
            }

            await SendEmailAsync(email, subject, htmlBody);
        }
        public async Task PassiveUserLoginMailAsync(string username, string email)
        {
            var adminMail = _config["Mail:AdminReceiver"];
            var subject = "Pasif Kullanıcı Giriş Denemesi";

            var htmlBody = $@"
            <div style='font-family: Arial; background-color:#fff8f8; padding: 20px; border-radius: 8px; border:1px solid #ffd6d6;'>
                <h2 style='color: #d9534f;'>Pasif Kullanıcı Giriş Denemesi</h2>
                <p><strong>{username}</strong> adlı kullanıcı (<strong>{email}</strong>) sisteme giriş yapmaya çalıştı.</p>
                <p>Ancak hesabı <strong>aktif değil</strong>. Kullanıcının erişim sağlayabilmesi için hesabının yönetici panelinden aktifleştirilmesi gerekmektedir.</p>
                <hr style='margin-top:20px;'>
                <small style='color: gray;'>Bu bildirim ESS Star Tedarik sisteminden otomatik olarak gönderilmiştir.</small>
            </div>";

            await SendEmailAsync(adminMail, subject, htmlBody);
        }
        public async Task SendPasswordResetMailAsync(string username, string email)
        {
            var subject = "🔐 Şifreniz Güncellendi";
            var htmlBody = $@"
        <div style='font-family: Arial; background-color:#fffbe6; padding: 20px; border-radius: 8px;'>
            <h2 style='color:#f0ad4e;'>Şifre Sıfırlama Bilgilendirmesi</h2>
            <p>Merhaba <strong>{username}</strong>,</p>
            <p>Şifreniz kısa süre önce başarıyla güncellendi.</p>
            <p>Eğer bu işlemi siz gerçekleştirmediyseniz, lütfen <strong>hemen sistem yöneticisiyle iletişime geçin</strong>.</p>
            <hr style='margin-top:20px;'>
            <small style='color: gray;'>Bu mail ESS Star Tedarik tarafından otomatik olarak gönderilmiştir. Yanıtlamanıza gerek yoktur.</small>
        </div>";

            await SendEmailAsync(email, subject, htmlBody);
        }
        public async Task SendPasswordResetMailLinkAsync(string username, string email, string token)
        {
            var subject = "🔑 Şifre Sıfırlama Bağlantınız";
            var resetLink = $"https://localhost:7020/PasswordReset/ResetForm?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(email)}";
            var htmlBody = $@"
            <div style='font-family: Arial; background-color:#e8f4fc; padding: 20px; border-radius: 8px;'>
                <h2 style='color:#0275d8;'>Şifre Sıfırlama Talebi</h2>
                <p>Merhaba <strong>{username}</strong>,</p>
                <p>Şifrenizi sıfırlamak için aşağıdaki bağlantıya tıklayabilirsiniz:</p>
                <p style='margin: 20px 0;'>
                    <a href='{resetLink}' style='background-color:#0275d8; color:white; padding:10px 20px; text-decoration:none; border-radius:5px;'>
                        🔐 Şifremi Sıfırla
                    </a>
                </p>
                <p>Bu bağlantı yalnızca <strong>15 dakika</strong> boyunca geçerlidir. Eğer bu işlemi siz başlatmadıysanız, bu e-postayı dikkate almayabilirsiniz.</p>
                <hr style='margin-top:20px;'>
                <small style='color: gray;'>Bu mail ESS Star Tedarik tarafından otomatik olarak gönderilmiştir. Yanıtlamanıza gerek yoktur.</small>
            </div>";

            await SendEmailAsync(email, subject, htmlBody);
        }
    }
}
