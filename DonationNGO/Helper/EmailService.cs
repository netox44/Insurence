using Insurence.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Insurence.Helper
{
    public class EmailService
    {
        private readonly MailSettings _mailSettings;

        public EmailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            var email = new MimeMessage();

            email.From.Add(new MailboxAddress(
                _mailSettings.DisplayName,
                _mailSettings.Mail
            ));

            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            email.Body = new BodyBuilder
            {
                HtmlBody = htmlBody
            }.ToMessageBody();

            using var smtp = new SmtpClient();

            // IMPORTANT: Gmail needs STARTTLS on port 587
            await smtp.ConnectAsync(
                _mailSettings.Host,
                _mailSettings.Port,
                SecureSocketOptions.StartTls
            );

            await smtp.AuthenticateAsync(
                _mailSettings.Mail,
                _mailSettings.Password
            );

            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
