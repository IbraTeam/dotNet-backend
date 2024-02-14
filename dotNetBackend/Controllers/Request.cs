using dotNetBackend.CustomValidationAttributes;
using dotNetBackend.Helpers;
using dotNetBackend.models.DTO;
using dotNetBackend.models.Enums;
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

        [HttpGet("users")] // Получение списка заявок пользователя (со всеми статусами) - /api/request/users +++
        [CustomAuthorize(UserRole = "STUDENT")]
        public List<RequestDTO> GetUsersRequests()
        {
            Guid userId = JWTTokenHelper.GetUserIdFromToken(HttpContext);

            return _requestService.GetUsersRequests(userId);
        }

        [HttpPost("create")]  // Создание заявки - /api/request/create +++
        [CustomAuthorize(UserRole = "STUDENT")]
        public RequestDTO CreatRequest([FromBody] CreateRequest createRequest) 
        {
            //Guid userId = Guid.Parse("fe8d64c2-5828-458d-a71a-566c5b0d0d5a"); //JWTTokenHelper.GetUserIdFromToken(HttpContext);
            //Role userRole =  Role.Student;// JWTTokenHelper.GetHeighstRoleFromToken(HttpContext);
            Guid userId = JWTTokenHelper.GetUserIdFromToken(HttpContext);
            Role userRole = JWTTokenHelper.GetHeighstRoleFromToken(HttpContext);

            return _requestService.CreateRequest(createRequest, userId, userRole);
        }

        [HttpGet] // Получение всех заявок с фильтрацией и пагинацией(для деканата): /api/request +++
        [CustomAuthorize(UserRole = "DEAN")]
        public TableDTO GetListRequests([FromQuery] RequestsFilter requestsFilter)
        {
            return _requestService.GetRequests(requestsFilter);
        }

        [HttpDelete("{requestId}")] // Отмена заявки: /api/request/:requestId(delete) +++
        [CustomAuthorize(UserRole = "STUDENT")]
        public ResponseDTO CancelRequest([FromRoute] Guid requestId)
        {
            // Guid userId = Guid.Parse("fe8d64c2-5828-458d-a71a-566c5b0d0d5a");
            Guid userId = JWTTokenHelper.GetUserIdFromToken(HttpContext);

            _requestService.CancelRequest(requestId, userId);

            return new ResponseDTO()
            {
                StatusCode = 200,
                Message = "OK"
            };
        }

        [HttpPost("{requestId}")] // Подтверждение/отклонение заявки (для деканата): /api/request/:requestId (post) +++
        [CustomAuthorize(UserRole = "DEAN")]
        public RequestDTO AcceptOrCancelRequest([FromRoute] Guid requestId, [FromBody] bool accept)
        {
            return _requestService.AcceptOrCancelRequest(requestId, accept);
        }

        [HttpGet("{audienceId}")] // Получение подтвержденных заявок /api/request/:audienceId
        //[CustomAuthorize(UserRole = "USER")]
        public List<RequestDTO> GetListBooking([FromRoute] Guid audienceId)
        {
            return _requestService.GetAcceptedRequests(audienceId);
        }
    }
}

