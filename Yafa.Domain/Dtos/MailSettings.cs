using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yafa.Domain.Dtos
{
    public class MailSettings
    {
        public string? FromEmail { get; set; }
        public string? SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public bool EnableSsl { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public bool IsBodyHtml { get; set; }
        public bool UseDefaultCredentials { get; set; }
    }


    //public class MailSettings
    //{
    //    public string? FromEmail { get; set; }
    //    public string? SmtpServer { get; set; }
    //    public int SmtpPort { get; set; }
    //    public bool EnableSsl { get; set; }
    //    public bool IsBodyHtml { get; set; }
    //    public bool UseDefaultCredentials { get; set; }
    //}
}
