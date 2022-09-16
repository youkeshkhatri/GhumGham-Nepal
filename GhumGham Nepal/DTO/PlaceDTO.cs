namespace GhumGham_Nepal.DTO
{
    public class PlaceDTO
    {
        public int Id { get; set; }
        public string? PlaceName { get; set; }
        public string? Introduction { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? Rating { get; set; }
        public string? Reviews { get; set; }
        public string? Gallery { get; set; }
        public IFormFile? ThumbnailFile { get; set; }
        public string? ThumbnailUrl { get; set; }
    }
}
