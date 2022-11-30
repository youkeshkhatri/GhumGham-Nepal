namespace GhumGham_Nepal.DTO
{
    public class PlaceDTO
    {
        public int Id { get; set; }
        public string? PlaceName { get; set; }
        public string? Location { get; set; }
        public string? Introduction { get; set; }
        public string? Description1 { get; set; }
        public string? Description2 { get; set; }
        public string? Description3 { get; set; }
        public string? Hotel1 { get; set; }
        public string? Hotel2 { get; set; }
        public string? Hotel3 { get; set; }
        public IFormFile? ThumbnailFile { get; set; }
        public string? ThumbnailUrl { get; set; }
    }
}
