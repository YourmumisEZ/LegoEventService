namespace LegoEventService.Models.ResponseModels
{
    public class EventResponseModel
    {
        public Guid EventId { get; set; }
        public string EventName { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
