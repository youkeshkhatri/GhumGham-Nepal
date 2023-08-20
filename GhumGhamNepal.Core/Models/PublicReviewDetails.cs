namespace GhumGham_Nepal.Models
{
    public class PublicReviewDetails
    {
        public int Id { get; set; }
        public int PlaceId { get; set; }
        public string? ReviewerName { get; set; }
        public string? Role { get; set; }
        public double? Rating { get; set; }
        public string? Comment { get; set; }
    }
}
