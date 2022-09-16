using System;
using System.Collections.Generic;

namespace GhumGham_Nepal.Models
{
    public partial class Place
    {
        public int Id { get; set; }
        public string? PlaceName { get; set; }
        public string? Introduction { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? ThumbnailUrl { get; set; }
    }
}
