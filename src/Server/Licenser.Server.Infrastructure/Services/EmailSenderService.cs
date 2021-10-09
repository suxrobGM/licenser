using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Licenser.Server.Domain.Services.Abstractions;

namespace Licenser.Server.Infrastructure.Services
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly IConfiguration _config;

        public EmailSenderService(IConfiguration configuration)
        {
            _config = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                var host = _config.GetSection("EmailConfiguration:Host").Value;
                var port = int.Parse(_config.GetSection("EmailConfiguration:Port").Value);
                var enableSsl = bool.Parse(_config.GetSection("EmailConfiguration:EnableSsl").Value);
                var username = _config.GetSection("EmailConfiguration:NoReplyAccount:UserName").Value;
                var password = _config.GetSection("EmailConfiguration:NoReplyAccount:Password").Value;

                using var mailMessage = new MailMessage(username, email)
                {
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true
                };

                using var smtpClient = new SmtpClient(host, port)
                {
                    EnableSsl = enableSsl,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(username, password)
                };

                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}