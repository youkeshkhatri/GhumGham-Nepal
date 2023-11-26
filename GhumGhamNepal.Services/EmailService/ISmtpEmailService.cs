using GhumGham_Nepal.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhumGhamNepal.Core.Services.EmailService
{
    public interface ISmtpEmailService
    {
        Task<ServiceResult> SendAsync(string subject, string receiverAddress, string htmlBody, string bcc = "", string textBody = "", string cc = "");

        string GetEmailTemplateContent(string templateFileName);
    }
}
