using GhumGham_Nepal.DTO;
using GhumGham_Nepal.Models;

namespace GhumGham_Nepal.Factory
{
    public interface IPlaceMapper
    {
        public Place ToEntity(PlaceDTO dto);

    }
}
