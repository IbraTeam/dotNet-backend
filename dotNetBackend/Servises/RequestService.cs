using dotNetBackend.models.DbFirst;
using dotNetBackend.models.DTO;
using dotNetBackend.models.Enums;
using dotNetBackend.Servises;

namespace dotNetBackend.Services
{
    public class RequestService : IRequestService
    {
        private NewContext _contextDb;

        public RequestService(NewContext contextDb)
        {
            _contextDb = contextDb;
        }

        public RequestDTO AcceptOrCancelRequest(Guid requestId, bool accept)
        {
            throw new NotImplementedException();
        }

        public RequestDTO CancelRequest(Guid requestId)
        {
            throw new NotImplementedException();
        }

        public RequestDTO CreatRequest(CreateRequest createRequest)
        {
            throw new NotImplementedException();
        }

        public List<RequestDTO> GetBooking(Guid audienceId)
        {
            throw new NotImplementedException();
        }

        public List<RequestDTO> GetRequests(RequestsFilter requestsFilter)
        {
            throw new NotImplementedException();
        }

        public List<RequestDTO> GetUsersRequests(Guid userId)
        {
            return _contextDb.Requests
                .Where(request => request.UserId == userId)
                .Select(request => new RequestDTO()
                 {
                    Id = request.Id,
                    Name = request.Name,
                    DateTime = request.DateTime,
                    Status = request.Status.ToStatus(), 
                    PairNumber = request.PairNumber.ToPairNumber(),
                    Repeated = request.Repeated,
                    TypeBooking = request.Type.ToTypeBooking()
                 })
                .ToList();
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