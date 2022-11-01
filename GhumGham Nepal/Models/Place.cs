using System;
using System.Collections.Generic;

namespace GhumGham_Nepal.Models
{
    public partial class Place
    {
        public Place()
        {
            Reviews = new HashSet<Review>();
        }

        public int PlaceId { get; set; }
        public string? PlaceName { get; set; }
        public string? Introduction { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? ThumbnailUrl { get; set; }

        public virtual ICollection<Review> Reviews { get; set; }
    }
}
