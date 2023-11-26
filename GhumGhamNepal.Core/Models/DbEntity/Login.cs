using System;
using System.Collections.Generic;

namespace GhumGhamNepal.Core.Models.DbEntity
{
    public partial class Login
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime? LoginDate { get; set; }
        public DateTime? LogoutDate { get; set; }
        public bool? LoginStatus { get; set; }
    }
}
