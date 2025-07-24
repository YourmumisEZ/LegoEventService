using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Security.Claims;

namespace LegoEventService.Controllers
{
    [ApiController]
    public class LegoControllerBase : ControllerBase
    {
        protected readonly IEnumerable<Claim> _claims;
        protected readonly ILogger _logger;

        protected LegoControllerBase(IHttpContextAccessor accessor, ILogger logger)
        {
            _claims = accessor.HttpContext?.User?.Claims ?? [];
            _logger = logger;
        }

        protected async Task<ActionResult<T>> ExecuteWithErrorHandling<T>(Func<Task<T>> function)
        {
            try
            {
                var result = await function();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(message: ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        Message = "An error occurred while processing your request.",
                        Details = ex.Message
                    });
            }
        }

        protected async Task<ActionResult> ExecuteWithErrorHandling(Func<Task> function)
        {
            try
            {
                await function();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(message: ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        Message = "An error occurred while processing your request.",
                        Details = ex.Message
                    });
            }
        }
    }
}
