using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GhumGham_Nepal.Models
{
    public partial class Registration
    {
        public int? Id { get; set; }
        [Required]
        public string? FullName { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Phone { get; set; }
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        public string? ConfirmPassword { get; set; }
       
    }
}
