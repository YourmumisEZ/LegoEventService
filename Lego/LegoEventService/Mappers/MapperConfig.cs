using LegoEventService.Domain.Entitites;
using LegoEventService.Models.ResponseModels;
using Mapster;

namespace LegoEventService.Mappers
{
    public class MapperConfig
    {
        public static void RegisterMappings()
        {
            TypeAdapterConfig<Event, EventResponseModel>.NewConfig()
                .Map(dest => dest.EventId, src => new Guid(src.EventId))
                .Map(dest => dest.CreatedDate, src => DateTime.Parse(src.CreatedDate));

            TypeAdapterConfig<SignUp, SignUpsResponseModel>.NewConfig()
                .Map(dest => dest.EventId, src => new Guid(src.EventId))
                .Map(dest => dest.CreatedDate, src => DateTime.Parse(src.CreatedDate));
        }
    }
}
