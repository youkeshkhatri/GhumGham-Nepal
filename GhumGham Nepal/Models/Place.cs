using System;
using System.Collections.Generic;

namespace GhumGham_Nepal.Models
{
    public partial class Place
    {
        public int PlaceId { get; set; }
        public string? PlaceName { get; set; }
        public string? Location { get; set; }
        public string? Introduction { get; set; }
        public string? Description1 { get; set; }
        public string? Description2 { get; set; }
        public string? Description3 { get; set; }
        public string? Hotel1 { get; set; }
        public string? Hotel2 { get; set; }
        public string? Hotel3 { get; set; }
        public string? ThumbnailUrl { get; set; }

        public virtual ICollection<Review> Reviews { get; set; }
        public Place()
        {
            Reviews = new HashSet<Review>();
        }
    }
}
