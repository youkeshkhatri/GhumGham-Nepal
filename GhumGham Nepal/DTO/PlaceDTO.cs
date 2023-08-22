using GhumGhamNepal.Core.Models.DbEntity;

namespace GhumGham_Nepal.DTO
{
    public class PlaceDTO
    {
        public int Id { get; set; }
        public string? PlaceName { get; set; }
        public string? Location { get; set; }
        public string? Introduction { get; set; }
        public string? Description1 { get; set; }
        public IFormFile? File { get; set; }
        public string? ThumbnailUrl { get; set; }
        public PublicReviewDetails? Reviews { get; set; }
    }
}
