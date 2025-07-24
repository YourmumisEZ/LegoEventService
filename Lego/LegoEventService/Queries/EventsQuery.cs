using LegoEventService.BL.Interfaces;
using LegoEventService.BL.Services;
using LegoEventService.Domain.Entitites;
using LegoEventService.Mutations;
using Microsoft.Extensions.Logging;

namespace LegoEventService.Queries
{
    public class EventsQuery
    {
        private readonly IEventsService _eventService;
        private readonly ILogger<EventsQuery> _logger;

        public EventsQuery(IEventsService eventService, ILogger<EventsQuery> logger)
        {
            _eventService = eventService;
            _logger = logger;
        }

        public async Task<IEnumerable<Event>> GetEvents()
        {
            try
            {
                var result = await _eventService.GetEvents();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(message: ex.Message, ex);
                throw new GraphQLException(ex.Message);
            }
        }
    }
}
