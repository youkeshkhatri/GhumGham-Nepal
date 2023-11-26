using System.ComponentModel.DataAnnotations.Schema;

namespace GhumGhamNepal.Core.Models.DbEntity
{
    public class PublicReviewDetails
    {
        public int Id { get; set; }
        public int PlaceId { get; set; }
        public string ReviewerName { get; set; }
        [NotMapped]
        public string Role { get; set; }
        public double? Rating { get; set; }
        public string Comment { get; set; }
        public int? UserRefId { get; set; }
    }
}
