using LegoEventService.BL.Interfaces;
using LegoEventService.Models.GraphQlResponseModels;

namespace LegoEventService.Mutations
{
    public class SignupMutation
    {
        private readonly ISignupsService _signupsService;
        private readonly ILogger<SignupMutation> _logger;

        public SignupMutation(ISignupsService signupsService, ILogger<SignupMutation> logger)
        {
            _signupsService = signupsService;
            _logger = logger;
        }
        public async Task<SignUpsResponseModel> CreateSignUp(string eventId, string email)
        {
            var result = new SignUpsResponseModel
            {
                Success = false
            };

            try
            {
                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(eventId))
                {
                    await _signupsService.SignUp(email, Guid.Parse(eventId));
                    result.Success = true;
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(message: ex.Message, ex);
                throw new GraphQLException(ex.Message);
            }
        }
    }
}
