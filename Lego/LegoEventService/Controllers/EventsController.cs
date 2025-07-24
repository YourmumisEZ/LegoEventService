using LegoEventService.BL.Interfaces;
using LegoEventService.Models.RequestModels;
using LegoEventService.Models.ResponseModels;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegoEventService.Controllers
{
    [Route("api/[controller]")]
    public class EventsController : LegoControllerBase
    {
        private readonly IEventsService _eventService;
        public EventsController(IHttpContextAccessor accessor, IEventsService eventService, ILogger<EventsController> logger)
            : base(accessor, logger)
        {
            _eventService = eventService;
        }

        [HttpPost]
        [Authorize("IsAdmin")]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateEventRequestModel req)
        {
            return await ExecuteWithErrorHandling(async () =>
            {
                var result = await _eventService.CreateEvent(req.EventName);
                return result;
            });
        }

        [HttpGet]
        [Authorize("IsUserOrAdmin")]
        public async Task<ActionResult<IEnumerable<EventResponseModel>>> GetEvents()
        {
            return await ExecuteWithErrorHandling(async () =>
            {
                var result = await _eventService.GetEvents();

                return (result.Adapt<IEnumerable<EventResponseModel>>());
            });
        }

        [HttpGet("{eventId:guid}")]
        [Authorize("IsUserOrAdmin")]
        public async Task<ActionResult<EventResponseModel>> GetEvent([FromRoute] Guid eventId)
        {
            return await ExecuteWithErrorHandling(async () =>
            {
                var result = await _eventService.GetEvent(eventId);

                return (result.Adapt<EventResponseModel>());
            });
        }
    }
}
