using System;
using System.Collections.Generic;

namespace GhumGham_Nepal.Models
{
    public partial class Registration
    {
        public int RegistrationId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
    }
}
