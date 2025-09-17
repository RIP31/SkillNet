using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace SkillNet.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // For now, we'll log the email. Integrate with SendGrid/Mailgun in production.
            // Example with SendGrid:
            /*
            var apiKey = _configuration["SendGrid:ApiKey"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("noreply@skillnet.com", "SkillNet");
            var to = new EmailAddress(email);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlMessage);
            await client.SendEmailAsync(msg);
            */

            // Log email for development
            var emailsDir = "emails";
            if (!System.IO.Directory.Exists(emailsDir))
            {
                System.IO.Directory.CreateDirectory(emailsDir);
            }
            System.IO.File.WriteAllText(
                $"{emailsDir}/{email}_{DateTime.Now:yyyyMMddHHmmss}.html",
                htmlMessage
            );
        }
    }
}