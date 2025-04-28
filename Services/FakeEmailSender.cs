using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace AI_WebsiteBuilder.Services
{
    public class FakeEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Fake email sender: just log to console
            Console.WriteLine($"Fake email sent to {email}: {subject}");
            return Task.CompletedTask;
        }
    }
}
