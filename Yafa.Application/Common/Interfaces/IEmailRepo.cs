using System.Net.Mail;

namespace Yafa.Application.Common.Interfaces
{
    public interface IEmailRepo
    {
        Task<bool> SendMailAsync(string to, string subject, string? cc, string? bcc, string htmlBody);
    }
}
