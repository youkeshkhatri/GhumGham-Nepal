using GhumGham_Nepal.DTO;
using GhumGham_Nepal.Models;

namespace GhumGham_Nepal.Factory
{
    public static class PlaceMapper
    {
        public static Place ToEntity(this PlaceDTO dto, List<CommonAttachment> attachments)
        {
            Place entity = new();
            if(attachments != null && attachments.Count > 0)
            {
                foreach (var item in attachments)
                {
                    entity.FileFormat = item.FileFormat;
                    entity.Size = item.Size;
                    entity.FileType = item.FileType;
                    entity.ServerFileName = item.ServerFileName;
                    entity.UserFileName = item.UserFileName;
                    entity.FileLocation = item.FileLocation;

                    entity.ThumbnailUrl += dto.ThumbnailUrl + item.ServerFileName;
                }
            }
            else
            {
                entity.FileFormat = null;
                entity.Size = null;
                entity.FileType = null;
                entity.ServerFileName = null;
                entity.UserFileName = null;
                entity.FileLocation = null;
            }
            entity.PlaceId = dto.Id;
            entity.PlaceName = dto.PlaceName;
            entity.Introduction = dto.Introduction;
            entity.Description1 = dto.Description1;

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
                ThumbnailUrl = entity.ThumbnailUrl
            };

            return dto;
        }


    }
}
