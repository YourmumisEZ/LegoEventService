using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using LegoEventService.BL.Interfaces;
using LegoEventService.Domain.Entitites;

namespace LegoEventService.BL.Services
{
    public class EventsService : IEventsService
    {
        private readonly IDynamoDBContext _context;
        public EventsService(IDynamoDBContext context)
        {
            _context = context;
        }

        public async Task<Guid> CreateEvent(string eventName)
        {
            if (await GetEventByName(eventName) == null)
            {
                var newGuid = Guid.NewGuid();

                await _context.SaveAsync(new Event
                {
                    EventId = newGuid.ToString(),
                    EventName = eventName,
                    CreatedDate = DateTime.UtcNow.ToString()
                });

                return newGuid;
            }

            throw new Exception("Event with that name already exists");
        }

        public async Task<Event> GetEvent(Guid eventId)
        {
            var request = new ScanRequest
            {
                TableName = "Events"
            };

            var scanConditions = new List<ScanCondition>
            {
                new ScanCondition("EventId", ScanOperator.Equal, eventId.ToString())
            };

            var result = await _context.ScanAsync<Event>(new List<ScanCondition>()).GetRemainingAsync();

            return result?.FirstOrDefault();
        }

        public async Task<IEnumerable<Event>> GetEvents()
        {
            var request = new ScanRequest
            {
                TableName = "Events"
            };

            var result = await _context.ScanAsync<Event>(new List<ScanCondition>()).GetRemainingAsync();

            return result;
        }

        private async Task<Event> GetEventByName(string eventName)
        {
            var request = new ScanRequest
            {
                TableName = "Events"
            };

            var scanConditions = new List<ScanCondition>
            {
                new ScanCondition("EventName", ScanOperator.Equal, eventName.ToString())
            };

            var result = await _context.ScanAsync<Event>(scanConditions).GetRemainingAsync();

            return result?.FirstOrDefault();
        }
    }
}
