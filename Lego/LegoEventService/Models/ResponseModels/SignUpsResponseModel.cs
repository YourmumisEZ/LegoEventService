namespace LegoEventService.Models.ResponseModels
{
    public class SignUpsResponseModel
    {
        public Guid EventId { get; set; }
        public string UserEmail { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
