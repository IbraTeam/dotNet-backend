using dotNetBackend.models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace dotNetBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Request : ControllerBase
    {
        [HttpGet("users")]
        public IEnumerable<User> GetListUsers()
        {
            throw new NotImplementedException();
        }

        [HttpPost("create")]
        public ResponseDTO CreatRequest([FromBody] CreateRequest createRequest)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public IEnumerable<RequestDTO> GetListRequests([FromQuery] RequestsFilter requestsFilter)
        {
            throw new NotImplementedException();
        }

        [HttpDelete("{requestId}")]
        public ResponseDTO CancelRequest([FromRoute] Guid requestId)
        {
            throw new NotImplementedException();
        }

        [HttpPost("{requestId}")]
        public ResponseDTO AcceptOrCancelRequest([FromRoute] Guid requestId, [FromBody] bool accept)
        {
            throw new NotImplementedException();
        }

        [HttpGet("{audienceId}")]
        public IEnumerable<RequestDTO> GetListBooking([FromRoute] Guid audienceId)
        {
            throw new NotImplementedException();
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