using Microsoft.AspNetCore.Mvc;
using LegoEventService.BL.Interfaces;
using LegoEventService.Models.ResponseModels;
using Mapster;
using Microsoft.AspNetCore.Authorization;

namespace LegoEventService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignupsController : LegoControllerBase
    {
        private readonly ISignupsService _signupsService;

        public SignupsController(IHttpContextAccessor accessor, ISignupsService signupsService, ILogger<SignupsController> logger) : base(accessor, logger)
        {
            _signupsService = signupsService;
        }

        [HttpPost("{eventId:guid}")]
        [Authorize("IsUser")]
        public async Task<IActionResult> SignUp([FromRoute] Guid eventId)
        {
            var email = _claims.FirstOrDefault(c => c.Type == "Email")?.Value;
            if (!string.IsNullOrEmpty(email))
            {
                return await ExecuteWithErrorHandling(async () =>
                {

                    await _signupsService.SignUp(email, eventId);
                });
            }

            return Unauthorized();
        }

        [HttpGet("event/{eventId:guid}")]
        [Authorize("IsAdmin")]
        public async Task<ActionResult<IEnumerable<SignUpsResponseModel>>> GetEventSignUps([FromRoute] Guid eventId)
        {
            return await ExecuteWithErrorHandling(async () =>
            {
                var result = await _signupsService.GetEventSignUps(eventId);

                return (result.Adapt<IEnumerable<SignUpsResponseModel>>());
            });
        }

        [HttpGet]
        [Authorize("IsAdmin")]
        public async Task<ActionResult<IEnumerable<SignUpsResponseModel>>> GetSignUps()
        {
            return await ExecuteWithErrorHandling(async () =>
            {
                var result = await _signupsService.GetSignUps();

                return (result.Adapt<IEnumerable<SignUpsResponseModel>>());
            });
        }
    }
}
