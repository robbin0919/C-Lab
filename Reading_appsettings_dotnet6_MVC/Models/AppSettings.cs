using System;

namespace ConfigDemo.Models
{
    public class AppSettings
    {
        public string ApiUrl { get; set; }
        public int MaxItems { get; set; }
        public bool IsEnabled { get; set; }
        public EmailSettings EmailSettings { get; set; }
    }

    public class EmailSettings
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SenderEmail { get; set; }
    }
}