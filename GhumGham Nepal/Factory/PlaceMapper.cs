using GhumGham_Nepal.DTO;
using GhumGham_Nepal.Models;

namespace GhumGham_Nepal.Factory
{
    public static class PlaceMapper
    {
        public static Place ToEntity(this PlaceDTO dto)
        {
            Place entity = new()
            {
                PlaceId = dto.Id,
                PlaceName = dto.PlaceName,
                Introduction = dto.Introduction,
                Description = dto.Description,
                ThumbnailUrl = dto.ThumbnailUrl
            };

            return entity;
        }

        public static List<PlaceDTO> ToDTO(this List<Place> entity)
        {
            if (entity == null)
                return new List<PlaceDTO>();

            var dto = entity.Select(x => new PlaceDTO
            {
                Id = x.PlaceId,
                PlaceName = x.PlaceName,
                Introduction = x.Introduction,
                Description = x.Description,
                ThumbnailUrl = x.ThumbnailUrl
            }).ToList();

            return dto;
        }

        public static PlaceDTO ToDTO(this Place entity)
        {
            if (entity == null)
                return new PlaceDTO();

            PlaceDTO dto = new()
            {
                Id = entity.PlaceId,
                PlaceName = entity.PlaceName,
                Introduction = entity.Introduction,
                Description = entity.Description,
                ThumbnailUrl = entity.ThumbnailUrl
            };

            return dto;
        }


    }
}
