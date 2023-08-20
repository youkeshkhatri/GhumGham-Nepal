using System;
using System.Collections.Generic;

namespace GhumGham_Nepal.Models
{
    public partial class Login
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public DateTime? LoginDate { get; set; }
        public DateTime? LogoutDate { get; set; }
        public bool? LoginStatus { get; set; }
    }
}
