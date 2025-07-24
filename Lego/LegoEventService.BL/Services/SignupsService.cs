using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using LegoEventService.BL.Interfaces;
using LegoEventService.Domain.Entitites;

namespace LegoEventService.BL.Services
{
    public class SignupsService : ISignupsService
    {
        private readonly IAmazonDynamoDB _dbClient;

        public SignupsService(IAmazonDynamoDB dbClient)
        {
            _dbClient = dbClient;
        }

        public async Task SignUp(string userEmail, Guid eventId)
        {
            if (!string.IsNullOrEmpty(userEmail) && await CheckIfUserHasSignedUpForEvent(userEmail, eventId))
            {
                var item = new Dictionary<string, AttributeValue>
                {
                    { "UserEmail", new AttributeValue { S = userEmail } },
                    { "EventId", new AttributeValue { S = eventId.ToString() } },
                    { "CreatedDate", new AttributeValue { S = DateTime.UtcNow.ToString() } },
                };
                var request = new PutItemRequest
                {
                    TableName = "SignUps",
                    Item = item
                };

                await _dbClient.PutItemAsync(request);
            }
        }

        public async Task<IEnumerable<SignUp>> GetEventSignUps(Guid eventId)
        {
            var request = new ScanRequest
            {
                TableName = "SignUps"
            };

            var scanConditions = new List<ScanCondition>
            {
                new ScanCondition("EventId", ScanOperator.Equal, eventId.ToString())
            };

            var context = new DynamoDBContext(_dbClient);
            var result = await context.ScanAsync<SignUp>(scanConditions).GetRemainingAsync();

            return result;
        }

        public async Task<IEnumerable<SignUp>> GetSignUps()
        {
            var request = new ScanRequest
            {
                TableName = "SignUps"
            };

            var context = new DynamoDBContext(_dbClient);
            var result = await context.ScanAsync<SignUp>(new List<ScanCondition>()).GetRemainingAsync();

            return result;
        }

        private async Task<bool> CheckIfUserHasSignedUpForEvent(string userEmail, Guid eventId)
        {
            var request = new ScanRequest
            {
                TableName = "SignUps"
            };

            var scanConditions = new List<ScanCondition>
            {
                new ScanCondition("EventId", ScanOperator.Equal, eventId.ToString()),
                new ScanCondition("UserEmail", ScanOperator.Equal, userEmail)
            };

            var context = new DynamoDBContext(_dbClient);
            var result = await context.ScanAsync<SignUp>(scanConditions).GetRemainingAsync();

            return result == null;
        }
    }
}
