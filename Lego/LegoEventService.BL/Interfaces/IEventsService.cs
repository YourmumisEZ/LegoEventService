using LegoEventService.Domain.Entitites;

namespace LegoEventService.BL.Interfaces
{
    public interface IEventsService
    {
        Task<Guid> CreateEvent(string eventName);
        Task<Event> GetEvent(Guid eventId);
        Task<IEnumerable<Event>> GetEvents();
    }
}