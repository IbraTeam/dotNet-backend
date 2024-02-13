using dotNetBackend.models.DbFirst;
using dotNetBackend.models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace dotNetBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Request : ControllerBase
    {
        NewContext _applicationDb;
        IDistributedCache _cache;
        public Request (NewContext applicationDb, IDistributedCache cache)
        {
            _applicationDb = applicationDb;
            _cache = cache;

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) // Устанавливаем время жизни объекта в кэше
            };

            _cache.SetString("1", "123");
        }

        [HttpGet("users")]
        public IEnumerable<UserDTO> GetListUsers()
        {
            //throw new NotImplementedException();

            return _applicationDb.Users.Select(user => new UserDTO()
            {
                Id = user.Id,
                Name = user.Name,
            });
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