using Amazon.DynamoDBv2.DataModel;

namespace LegoEventService.Domain.Entitites
{
    [DynamoDBTable("SignUps")]
    public class SignUp
    {
        public string EventId { get; set; }
        public string UserEmail { get; set; }
        public string CreatedDate { get; set; }
    }
}