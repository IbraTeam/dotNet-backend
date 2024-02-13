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
        public List<UserDTO> GetListUsers()
        {
            return _requestService.GetUsers();
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

/*
    1. Получение списка заявок пользователя на забронированные аудитории - /api/request/users
    2. Создание заявки - /api/request/create
    3. Получение всех заявок с фильтрацией и пагинацией(для деканата): /api/request 
    4. Отмена заявки: /api/request/:requestId  (delete)

    5. Подтверждение/отклонение заявки (для деканата): /api/request/:requestId (post)

    6. Получение бронирований аудитории (передаем id аудитории) /api/request/:audienceId

    //    Получение расписания (подтвержденные заявки) - /api/request/approved
 */