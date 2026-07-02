using System.Net;
using System.Net.Mail;
using UamHelpDeskPA.Api.Interfaces;

namespace UamHelpDeskPA.Api.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(
            IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendEmailAsync(
            string to,
            string subject,
            string body)
        {
            try
            {
                var smtp = new SmtpClient(
                    _configuration["Smtp:Host"],
                    _configuration.GetValue<int>("Smtp:Port"));

                smtp.EnableSsl = true;

                smtp.Credentials =
                    new NetworkCredential(
                        _configuration["Smtp:SenderEmail"],
                        _configuration["Smtp:Password"]);

                var message = new MailMessage
                {
                    From = new MailAddress(
                        _configuration["Smtp:SenderEmail"]!,
                        _configuration["Smtp:SenderName"]),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                message.To.Add(to);

                await smtp.SendMailAsync(message);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}