using LegoEventService.Domain.Entitites;

namespace LegoEventService.BL.Interfaces
{
    public interface ISignupsService
    {
        Task<IEnumerable<SignUp>> GetEventSignUps(Guid eventId);
        Task<IEnumerable<SignUp>> GetSignUps();
        Task SignUp(string userEmail, Guid eventId);
    }
}