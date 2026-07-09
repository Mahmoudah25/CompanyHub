using CompanyHub.Application.Common.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Infrastructure.Email
{
    public class EmailService : IEmailService
    {
        private readonly EmailSetting emailSetting;
        public EmailService(IOptions<EmailSetting> option)
        {
            emailSetting = option.Value;
        }
        public async Task SendEmail(string to, string subject, string body)
        {
            var client = new SmtpClient(emailSetting.host, emailSetting.port);
            client.Credentials = new System.Net.NetworkCredential(emailSetting.Email, emailSetting.Password);
            client.EnableSsl = emailSetting.EnableSSL;
            var email = new MailMessage(
                emailSetting.Email,
                to,
                subject,
                body);
            email.IsBodyHtml = true;
            await client.SendMailAsync(email);

        }
    }
}
