using Microsoft.Extensions.Logging;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Text.RegularExpressions;
using GhumGham_Nepal.Services;
using GhumGham_Nepal.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using GhumGhamNepal.Core.Models.DbEntity;

namespace GhumGhamNepal.Core.Services.EmailService
{
    public class SmtpEmailService : ISmtpEmailService
    {
        #region ctor & prop 
        private readonly IRepository<MailSetting> _mailSettingRepo;
        private const string _appURL = "localhost:5212";
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SmtpEmailService(IRepository<MailSetting> mailSettingRepo, IWebHostEnvironment webHostEnvironment)
        {
            _mailSettingRepo = mailSettingRepo;
            _webHostEnvironment = webHostEnvironment;
        }

        #endregion

        public async Task<ServiceResult> SendAsync(string subject, string receiverAddress, string htmlBody, string bcc = "", string textBody = "", string cc = "")
        {
            var _mailSetting = await _mailSettingRepo.Table.FirstOrDefaultAsync().ConfigureAwait(false);
            if(_mailSetting == null)
                return ServiceResult.Fail("Mail settings not set.");

            //get sub domain name
            string organizationName = "Unknown";

            if (_appURL != null)
            {
                string pattern = @"https://(\w+)\.?.*";
                Match match = Regex.Match(_appURL, pattern);
                organizationName = (string.IsNullOrEmpty(match.Groups[1].Value) ? "Unknown" : match.Groups[1].Value);
            }

            // create message
            var email = new MimeMessage();
            
            email.Sender = MailboxAddress.Parse(_mailSetting.FromEmail);
            email.Sender.Name = "GhumGham Nepal";

            email.From.Add(email.Sender);

            foreach (var receiver in receiverAddress.Split(','))
            {
                email.To.Add(MailboxAddress.Parse(receiver));
            }

            if (!string.IsNullOrEmpty(cc))
            {
                foreach (var ccc in cc.Split(','))
                {
                    email.Cc.Add(MailboxAddress.Parse(ccc));
                }
            }

            if (!string.IsNullOrEmpty(bcc))
            {
                foreach (var bc in bcc.Split(','))
                {
                    email.Bcc.Add(MailboxAddress.Parse(bc));
                }
            }

            email.ReplyTo.Add(new MailboxAddress("", _mailSetting.ReplyToEmail));
            email.Subject = subject;
            var builder = new BodyBuilder() { HtmlBody = htmlBody };
            email.Body = builder.ToMessageBody();

            try
            {
                // send email
                using (var smtp = new SmtpClient())
                {
                    smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    smtp.Connect(_mailSetting.HostName, _mailSetting.Port, SecureSocketOptions.StartTls);

                    smtp.Authenticate(_mailSetting.UserName, _mailSetting.Password);
                    await smtp.SendAsync(email).ConfigureAwait(false);
                    smtp.Disconnect(true);
                }
                return new ServiceResult(true)
                {
                    Message = new List<string> { "Email.Sent.Success" }
                };
            }
            catch (Exception)
            {
                //_logger.LogError(ex.Message);
                return ServiceResult.Fail("Email.Sent.Failure");
            }
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            await SendAsync(subject, email, htmlMessage);
        }

        public string GetEmailTemplateContent(string templateFileName)
        {
            string templatePath = Path.Combine(_webHostEnvironment.WebRootPath, "EmailTemplates", templateFileName);

            if (File.Exists(templatePath))
            {
                return File.ReadAllText(templatePath);
            }

            return null; // Template not found
        }

    }
}
