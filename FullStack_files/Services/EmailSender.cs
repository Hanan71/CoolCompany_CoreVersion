using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace CoolCompanyEstore.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // هنا فقط تكتب رسالة في الكونسول بدل الإرسال الحقيقي
            Console.WriteLine($"Fake Email Sent to {email}: {subject}");
            return Task.CompletedTask;
        }
    }
}
