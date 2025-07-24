using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using LegoEventService.BL.Interfaces;
using LegoEventService.Domain.Entitites;
using Microsoft.Extensions.Logging;

namespace LegoEventService.BL.Services
{
    public class EventsService : IEventsService
    {
        private readonly IAmazonDynamoDB _dbClient;

        public EventsService(IAmazonDynamoDB dbClient)
        {
            _dbClient = dbClient;
        }

        public async Task<Guid> CreateEvent(string eventName)
        {
            if (await GetEventByName(eventName) == null)
            {
                var newGuid = Guid.NewGuid();

                var item = new Dictionary<string, AttributeValue>
            {
                { "EventId", new AttributeValue { S = newGuid.ToString() } },
                { "EventName", new AttributeValue { S = eventName } },
                { "CreatedDate", new AttributeValue { S = DateTime.UtcNow.ToString() } },
            };
                var request = new PutItemRequest
                {
                    TableName = "Events",
                    Item = item
                };

                await _dbClient.PutItemAsync(request);

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

            var context = new DynamoDBContext(_dbClient);
            var result = await context.ScanAsync<Event>(new List<ScanCondition>()).GetRemainingAsync();

            return result?.FirstOrDefault();
        }

        public async Task<IEnumerable<Event>> GetEvents()
        {
            var request = new ScanRequest
            {
                TableName = "Events"
            };

            var context = new DynamoDBContext(_dbClient);
            var result = await context.ScanAsync<Event>(new List<ScanCondition>()).GetRemainingAsync();

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

            var context = new DynamoDBContext(_dbClient);

            var result = await context.ScanAsync<Event>(scanConditions).GetRemainingAsync();

            return result?.FirstOrDefault();
        }
    }
}
