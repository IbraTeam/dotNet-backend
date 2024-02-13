using dotNetBackend.models.DTO;
using dotNetBackend.Servises;
using Microsoft.AspNetCore.Mvc;

namespace dotNetBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Request : ControllerBase
    {
        private IRequestService _requestService;

        public Request (IRequestService requestService)
        {
            _requestService = requestService;
        }

        [HttpGet("users")]
        public List<RequestDTO> GetUsersRequests([FromQuery] Guid userId)
        {
            return _requestService.GetUsersRequests(userId);
        }

        [HttpPost("create")]
        public RequestDTO CreatRequest([FromBody] CreateRequest createRequest)
        {
            return _requestService.CreatRequest(createRequest);
        }

        [HttpGet]
        public List<RequestDTO> GetListRequests([FromQuery] RequestsFilter requestsFilter)
        {
            return _requestService.GetRequests(requestsFilter);
        }

        [HttpDelete("{requestId}")]
        public RequestDTO CancelRequest([FromRoute] Guid requestId)
        {
            return _requestService.CancelRequest(requestId);  
        }

        [HttpPost("{requestId}")]
        public RequestDTO AcceptOrCancelRequest([FromRoute] Guid requestId, [FromBody] bool accept)
        {
            return _requestService.AcceptOrCancelRequest(requestId, accept);
        }

        [HttpGet("{audienceId}")]
        public List<RequestDTO> GetListBooking([FromRoute] Guid audienceId)
        {
            return _requestService.GetBooking(audienceId);
        }
    }
}

