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
                Description1 = dto.Description1,
                Description2 = dto.Description2,
                Description3 = dto.Description3,
                Hotel1 = dto.Hotel1,
                Hotel2 = dto.Hotel2,
                Hotel3 = dto.Hotel3,
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
                Description1 = x.Description1,
                Description2 = x.Description2,
                Description3 = x.Description3,
                Hotel1 = x.Hotel1,
                Hotel2 = x.Hotel2,
                Hotel3 = x.Hotel3,
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
                Description1 = entity.Description1,
                Description2 = entity.Description2,
                Description3 = entity.Description3,
                Hotel1 = entity.Hotel1,
                Hotel2 = entity.Hotel2,
                Hotel3 = entity.Hotel3,
                ThumbnailUrl = entity.ThumbnailUrl
            };

            return dto;
        }


    }
}
