using Amazon.DynamoDBv2.DataModel;


namespace LegoEventService.Domain.Entitites
{
    [DynamoDBTable("Events")]
    public class Event
    {
        public string EventId { get; set; }
        public string EventName { get; set; }
        public string CreatedDate { get; set; }
    }
}
