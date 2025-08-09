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
        private readonly IDynamoDBContext _context;

        public SignupsService(IDynamoDBContext context)
        {
            _context = context;
        }

        public async Task SignUp(string userEmail, Guid eventId)
        {
            if (!string.IsNullOrEmpty(userEmail) && await CheckIfUserHasSignedUpForEvent(userEmail, eventId))
            {
                await _context.SaveAsync(new SignUp
                {
                    UserEmail = userEmail,
                    EventId = eventId.ToString(),
                    CreatedDate = DateTime.UtcNow.ToString()
                });
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

            var result = await _context.ScanAsync<SignUp>(scanConditions).GetRemainingAsync();

            return result;
        }

        public async Task<IEnumerable<SignUp>> GetSignUps()
        {
            var request = new ScanRequest
            {
                TableName = "SignUps"
            };

            var result = await _context.ScanAsync<SignUp>(new List<ScanCondition>()).GetRemainingAsync();

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

            var result = await _context.ScanAsync<SignUp>(scanConditions).GetRemainingAsync();

            return result == null || result.Count == 0;
        }
    }
}
