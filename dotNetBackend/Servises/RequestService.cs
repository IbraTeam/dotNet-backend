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

        public void CancelRequest(Guid requestId, Guid userId)
        {
            var request = _contextDb.Requests.FirstOrDefault(request => request.UserId == userId && request.Id == requestId);
            if (request == null)
            {
                throw new NotFoundException($"Request {requestId} or user {userId} not found!");
            }

            _contextDb.Requests.Remove(request);
            _contextDb.SaveChanges();
        }

        public TableDTO GetRequests(RequestsFilter requestsFilter)
        {
            if (requestsFilter.WeekStart is null)
            {
                requestsFilter.WeekStart = DateTime.Now.AddDays(1 - (int)DateTime.Now.DayOfWeek);
            }

            var temp = _contextDb.Requests
                .Where(request => (requestsFilter.Status != null ? request.Status == requestsFilter.Status.ToString() : true) &&
                                  (requestsFilter.PairNumber != null ? request.PairNumber == (short)requestsFilter.PairNumber : true) &&
                                  (requestsFilter.Type != null ? request.Type == requestsFilter.Type.ToString() : true));

            requestsFilter.WeekStart = requestsFilter.WeekStart.Value.Date;
            var dayOfWeek = DateOnly.FromDateTime((DateTime)requestsFilter.WeekStart);
            if (dayOfWeek.DayOfWeek != DayOfWeek.Monday)
            {
                throw new BadRequestException("The week should start on Monday!");
            }

            temp = temp.Where(request => requestsFilter.WeekStart < request.DateTime && 
                              request.DateTime < requestsFilter.WeekStart + new TimeSpan(7, 0, 0, 0) || request.Repeated);

            return new TableDTO()
            {
                requests = temp.OrderBy(request => request.DateTime).ThenBy(request => request.PairNumber).SelectRequestDTO().ToList(),
                WeekStart = requestsFilter.WeekStart.Value,
                WeekEnd = requestsFilter.WeekStart.Value.AddDays(7)
            };
        }

        public RequestDTO AcceptOrCancelRequest(Guid requestId, bool accept)
        {
            var request = _contextDb.Requests.FirstOrDefault(request => request.Id == requestId);

            if(request is null)
            {
                throw new NotFoundException($"Request with guid {requestId} not found!");
            }

            var thereIsAcceptedRequest = _contextDb.Requests
                .Any(acceptedRequest => (acceptedRequest.DateTime.Date == request.DateTime.Date ||
                                            (acceptedRequest.Repeated || request.Repeated) && request.DateTime.DayOfWeek == acceptedRequest.DateTime.DayOfWeek) && 
                                        acceptedRequest.PairNumber == request.PairNumber &&
                                        acceptedRequest.Status == Status.Accepted.ToString());
            if (thereIsAcceptedRequest && accept)
            {
                throw new BadRequestException("There is a confirmed application!");
            }

            request.Status = (accept ? Status.Accepted : Status.Rejected).ToString();
            _contextDb.SaveChanges();

            // Отменяем заявки студентов в ту же аудиторию на ту же дата-время
            var pendingRequests = _contextDb.Requests
                .Include(request => request.User)
                .Where(pendingRequest => pendingRequest.Status == Status.Pending.ToString() &&
                                 pendingRequest.PairNumber == request.PairNumber &&
                                 (pendingRequest.DateTime == request.DateTime ||
                                      request.Repeated && pendingRequest.DateTime.DayOfWeek == request.DateTime.DayOfWeek) &&
                                 pendingRequest.User.Role == Role.Student.ToString());

            foreach(Request req in  pendingRequests)
            {
                req.Status = Status.Rejected.ToString();
            }

            _contextDb.SaveChanges();

            return request.ToRequestDTO();
        }

        public RequestDTO CreatRequest(CreateRequest createRequest, Guid userId, Role userRole)
        {
            createRequest.DateTime = createRequest.DateTime.Date.ToLocalTime();
            Status requestStatus = Status.Pending;

            if (userRole == Role.Student)
            {
                var teachersAcceptedRequests = _contextDb.Requests
                    .Include(request => request.User)
                    .Where(request => request.KeyId == createRequest.KeyId &&
                           (request.DateTime.Date == createRequest.DateTime.Date ||
                               request.Repeated && request.DateTime.DayOfWeek == createRequest.DateTime.DayOfWeek) &&
                           request.PairNumber == (short)createRequest.PairNumber &&
                           request.Status == Status.Accepted.ToString() &&
                           (request.User.Role == Role.Teacher.ToString() || request.User.Role == Role.Dean.ToString()));

                requestStatus = teachersAcceptedRequests.Any() ? Status.Rejected : Status.Pending;
                createRequest.Repeated = false;
            }

            var newRequest = new Request
            {
                Name = createRequest.Name,
                Status = requestStatus.ToString(),
                DateTime = createRequest.DateTime,
                Repeated = createRequest.Repeated,
                KeyId = createRequest.KeyId,
                UserId = userId,
                PairNumber = (short)createRequest.PairNumber,
                Type = createRequest.TypeBooking.ToString(),
                Id = Guid.NewGuid()
            };

            _contextDb.Requests.Add(newRequest);
            if(_contextDb.SaveChanges() != 1)
            {
                throw new DbUpdateException("Failed to save!");
            }

            return newRequest.ToRequestDTO();
        }

        public List<RequestDTO> GetAcceptedRequests(Guid audienceId)
        {
            return _contextDb.Requests
                .Where(reques => reques.KeyId == audienceId && reques.Status == Status.Accepted.ToString())
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

Задачи:
    1. Автопринятие завок + 
    2. Автоотклюнение заявок студентов +
    3. Отлов исключений +
    5. Действие системы если препод подал заявку на забронированную аудиторию + 
    8. Автоотклонение заявок студентов если учителю подтвердили заявку +

    6. Получение расписания подтвержденных заявок(открыт для всех)  Поменять List на TableDTO
    7. В модель заявки указать день недели

    8. Проверка, что не существую заявки дубликата при создании


    {
        "name": "string",
        "dateTime": "2024-02-18T06:05:48.750Z",
        "repeated": true,
        "typeBooking": "Booking",
        "pairNumber": "First",
        "keyId": "1806dab8-e3bd-42b5-969c-d6c0f06662c8"
    }
             
 */