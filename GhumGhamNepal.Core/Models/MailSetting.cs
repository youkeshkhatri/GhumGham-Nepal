using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhumGhamNepal.Core.Models
{
    [Table("MailSetting")]
    public class MailSetting
    {
        [Key]
        public int Id { get; set; }
        public string? FromEmail { get; set; }
        public string? HostName { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public bool? IsAmazonEmailService { get; set; }
        public string? ReplyToEmail { get; set; }
    }
}
