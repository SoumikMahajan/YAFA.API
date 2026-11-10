using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SendGrid.Helpers.Mail;
using System.Net;
using System.Net.Mail;
using System.Reflection.PortableExecutable;
using Yafa.Application.Common.Interfaces;
using Yafa.Domain.Dtos;

namespace Yafa.Infrastructure.Repository
{
    public class EmailService : IEmailRepo
    {
        private readonly ILogService _logService;
        private readonly LocalMailSettings _localMailSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EmailService(ILogService logService, IOptions<LocalMailSettings> localMailSettings, IHttpContextAccessor httpContextAccessor)
        {
            _logService = logService;           
            _localMailSettings = localMailSettings.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> SendMailAsync(string to, string subject, string? cc, string? bcc, string htmlBody)
        {
            try
            {
                using (var client = new SmtpClient(_localMailSettings.SmtpServer, _localMailSettings.SmtpPort))
                {
                    client.EnableSsl = _localMailSettings.EnableSsl;
                    client.UseDefaultCredentials = _localMailSettings.UseDefaultCredentials;

                    if (!_localMailSettings.UseDefaultCredentials)
                    {
                        client.Credentials = new NetworkCredential(
                            _localMailSettings.Username,
                            _localMailSettings.Password
                        );
                    }

                    using (var mailMessage = new MailMessage())
                    {
                        mailMessage.From = new MailAddress(_localMailSettings.FromEmail ?? "");
                        mailMessage.To.Add(to);

                        if (!string.IsNullOrWhiteSpace(cc))
                            mailMessage.CC.Add(cc);

                        if (!string.IsNullOrWhiteSpace(bcc))
                            mailMessage.Bcc.Add(bcc);

                        mailMessage.Subject = subject;
                        mailMessage.Body = htmlBody;
                        mailMessage.IsBodyHtml = _localMailSettings.IsBodyHtml;

                        await client.SendMailAsync(mailMessage);
                        await _logService.LogCustomAsync($"Email sent successfully to {to}","SUCCESS");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                await _logService.LogExceptionErrorAsync(ex);
                return false;
            }
        }
    }
}

//private readonly string _sendGridApiKey;

//public EmailService(IConfiguration configuration)
//{
//    _sendGridApiKey = configuration["SendGrid:ApiKey"];
//}

//public async Task SendEmailAsync(string toEmail, string subject, string templateId, object dynamicTemplateData)
//{
//    var client = new SendGridClient(_sendGridApiKey);
//    var from = new EmailAddress("admin@giftflow.us", "GiftFlow");
//    var to = new EmailAddress(toEmail);
//    var msg = MailHelper.CreateSingleTemplateEmail(from, to, templateId, dynamicTemplateData);

//    var response = await client.SendEmailAsync(msg);
//    if (!response.IsSuccessStatusCode)
//    {
//        throw new Exception($"Failed to send email: {response.StatusCode}");
//    }
//}
