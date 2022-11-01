using System;
using System.Collections.Generic;

namespace GhumGham_Nepal.Models
{
    public partial class Review
    {
        public int ReviewId { get; set; }
        public int? PlaceRefId { get; set; }
        public int? UserRefId { get; set; }
        public decimal? Rating { get; set; }
        public string? Feedback { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public Guid? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Place? PlaceRef { get; set; }
        public virtual Registration? UserRef { get; set; }
    }
}
