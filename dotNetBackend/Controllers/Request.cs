using dotNetBackend.CustomValidationAttributes;
using dotNetBackend.Helpers;
using dotNetBackend.models.DTO;
using dotNetBackend.models.Enums;
using dotNetBackend.Servises;
using Microsoft.AspNetCore.Mvc;
using System;

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

        [HttpGet("users")] // Получение списка заявок пользователя на забронированные аудитории - /api/request/users
        //[CustomAuthorize(UserRole = "ADMIN")]
        public List<RequestDTO> GetUsersRequests([FromQuery] Guid userId)
        {
            return _requestService.GetUsersRequests(userId);
        }

        [HttpPost("create")]  // Создание заявки - /api/request/create
        public RequestDTO CreatRequest([FromBody] CreateRequest createRequest) 
        {
            Guid userId = Guid.Parse("fe8d64c2-5828-458d-a71a-566c5b0d0d5a"); //JWTTokenHelper.GetUserIdFromToken(HttpContext);

            return _requestService.CreatRequest(createRequest, userId);
        }

        [HttpGet] // Получение всех заявок с фильтрацией и пагинацией(для деканата): /api/request 
        public TableDTO GetListRequests([FromQuery] RequestsFilter requestsFilter)
        {
            return _requestService.GetRequests(requestsFilter);
        }

        [HttpDelete("{requestId}")] // Отмена заявки: /api/request/:requestId(delete)
        public RequestDTO CancelRequest([FromRoute] Guid requestId)
        {
            return _requestService.CancelRequest(requestId);  
        }

        [HttpPost("{requestId}")] // Подтверждение/отклонение заявки (для деканата): /api/request/:requestId (post)
        public RequestDTO AcceptOrCancelRequest([FromRoute] Guid requestId, [FromBody] bool accept)
        {
            return _requestService.AcceptOrCancelRequest(requestId, accept);
        }

        [HttpGet("{audienceId}")] // Получение бронирований аудитории (передаем id аудитории) /api/request/:audienceId
        public List<RequestDTO> GetListBooking([FromRoute] Guid audienceId)
        {
            return _requestService.GetBooking(audienceId);
        }
    }
}

