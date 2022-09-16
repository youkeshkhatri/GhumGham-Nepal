using System;
using System.Collections.Generic;

namespace GhumGham_Nepal.Models
{
    public partial class Review
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public double? Rating { get; set; }
        public string? Review1 { get; set; }

        public virtual User? User { get; set; }
    }
}
