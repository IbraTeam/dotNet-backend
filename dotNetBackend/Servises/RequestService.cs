using dotNetBackend.Exceptions;
using dotNetBackend.models.DbFirst;
using dotNetBackend.models.DTO;
using dotNetBackend.models.Enums;
using dotNetBackend.Servises;
using Microsoft.EntityFrameworkCore;

namespace dotNetBackend.Services
{
    public class RequestService : IRequestService
    {
        private NewContext _contextDb;

        public RequestService(NewContext contextDb)
        {
            _contextDb = contextDb;
        }

        public List<RequestDTO> GetRequests(RequestsFilter requestsFilter)
        {
            throw new NotImplementedException();
        }

        public RequestDTO CancelRequest(Guid requestId)
        {
            throw new BadRequestException("Используй AcceptOrCancelRequest с параметром false");
        }

        public RequestDTO AcceptOrCancelRequest(Guid requestId, bool accept)
        {
            var request = _contextDb.Requests.FirstOrDefault(request => request.Id == requestId);

            if(request is null)
            {
                throw new NotFoundException($"Request with guid {requestId} not found!");
            }

            request.Status = (accept ? Status.Accepted : Status.Rejected).ToString();
            _contextDb.SaveChanges();

            return request.ToRequestDTO();
        }

        public RequestDTO CreatRequest(CreateRequest createRequest)
        {
            var newRequest = new Request
            {
                Name = createRequest.Name,
                Status = Status.Pending.ToString(),
                DateTime = createRequest.DateTime,
                Repeated = createRequest.Repeated,
                KeyId = createRequest.KeyId,
                PairNumber = (short)createRequest.PairNumber,
                Type = createRequest.TypeBooking.ToString()
            };

            _contextDb.Requests.Add(newRequest);
            _contextDb.SaveChangesAsync();

            return newRequest.ToRequestDTO();
        }

        public List<RequestDTO> GetBooking(Guid audienceId)
        {
            return _contextDb.Requests
                .Include(request => request.Key)
                .Where(reques => reques.Key.Id == audienceId && reques.Status == Status.Accepted.ToString())
                .SelectRequestDTO()
                .ToList();
        }

        public List<RequestDTO> GetUsersRequests(Guid userId)
        {
            return _contextDb.Requests
                .Where(request => request.UserId == userId)
                .SelectRequestDTO()
                .ToList();
        }
    }
}

/*
    1. Получение всех заявок с фильтрацией и пагинацией(для деканата): /api/request 

    2. Отмена заявки: /api/request/:requestId  (delete)

    3. Подтверждение/отклонение заявки (для деканата): /api/request/:requestId (post)
    4. Создание заявки - /api/request/create
    5. Получение бронирований аудитории (передаем id аудитории) /api/request/:audienceId
    6. Получение списка заявок пользователя на забронированные аудитории - /api/request/users

    //    Получение расписания (подтвержденные заявки) - /api/request/approved
 */