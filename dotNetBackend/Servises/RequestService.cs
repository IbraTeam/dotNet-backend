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

        public void CancelRequest(Guid requestId, Guid userId) // ++
        {
            var request = _contextDb.Requests.FirstOrDefault(request => request.UserId == userId && request.Id == requestId);
            if (request == null)
            {
                throw new NotFoundException($"Request {requestId} or user {userId} not found!");
            }

            _contextDb.Requests.Remove(request);
            _contextDb.SaveChanges();
        }

        public TableDTO GetRequests(RequestsFilter requestsFilter) // ++
        {
            if (requestsFilter.WeekStart is null)
            {
                requestsFilter.WeekStart = DateTime.Now.AddDays(1 - (int)DateTime.Now.DayOfWeek);
            }

            string[] statuses = (requestsFilter.Status ?? []).Select(status => status.ToString()).ToArray();

            var temp = _contextDb.Requests
                .Include(request => request.User)
                .Include(request => request.Key)
                .Where(request => (requestsFilter.Status != null ? statuses.Contains(request.Status) : true) &&
                                  (requestsFilter.PairNumber != null ? request.PairNumber == (short)requestsFilter.PairNumber : true) &&
                                  (requestsFilter.Type != null ? request.Type == requestsFilter.Type.ToString() : true) &&
                                  (requestsFilter.AudienceId != null ? request.KeyId == requestsFilter.AudienceId : true));

            requestsFilter.WeekStart = requestsFilter.WeekStart.Value.Date;
            var dayOfWeek = DateOnly.FromDateTime((DateTime)requestsFilter.WeekStart);
            if (dayOfWeek.DayOfWeek != DayOfWeek.Monday)
            {
                throw new BadRequestException("The week should start on Monday!");
            }

            temp = temp.Where(request => requestsFilter.WeekStart < request.DateTime &&
                              request.DateTime < requestsFilter.WeekStart + new TimeSpan(7, 0, 0, 0));

            return new TableDTO()
            {
                Requests = temp.OrderBy(request => request.DateTime).ThenBy(request => request.PairNumber).SelectRequestDTO().ToList(),
                WeekStart = requestsFilter.WeekStart.Value,
                WeekEnd = requestsFilter.WeekStart.Value.AddDays(7)
            };
        }

        public void AcceptOrCancelRequest(Guid requestId, AcceptDTO acceptDTO) // ++
        {
            var requestFirst = _contextDb.Requests.FirstOrDefault(request => request.Id == requestId);
            if (requestFirst is null)
            {
                throw new NotFoundException($"Request with guid {requestId} not found!");
            }

            var repeatedRequests = _contextDb.Requests.Include(request => request.User).Where(request => request.RepeatId == requestFirst.RepeatId).ToList();
            if (acceptDTO.Accept)
            {
                foreach (var repeatedRequest in repeatedRequests)
                {
                    var thereIsAcceptedRequest = _contextDb.Requests
                        .Include(request => request.User)
                        .Any(request => request.DateTime.Date == repeatedRequest.DateTime.Date &&
                                          request.PairNumber == repeatedRequest.PairNumber &&
                                          request.KeyId == repeatedRequest.KeyId &&
                                          request.Status == Status.Accepted.ToString() &&
                                          ((repeatedRequest.User.Role == "TEACHER" || repeatedRequest.User.Role == "DEAN" || repeatedRequest.User.Role == "ADMIN") && acceptDTO.Accept ? 
                                                    request.User.Role == "TEACHER" || request.User.Role == "DEAN" || request.User.Role == "ADMIN" : true));

                    if (thereIsAcceptedRequest)
                    {
                        throw new BadRequestException("There is a confirmed application!");
                    }
                }
            }

            foreach (var repeatedRequest in repeatedRequests)
            {
                repeatedRequest.Status = (acceptDTO.Accept ? Status.Accepted : Status.Rejected).ToString();
            }
            _contextDb.SaveChanges();

            if (acceptDTO.Accept)
            {
                foreach (var repeatedRequest in repeatedRequests)
                {
                    var pendingRequests = _contextDb.Requests
                        .Include(request => request.User)
                        .Where(request => request.DateTime.Date == repeatedRequest.DateTime.Date &&
                                          request.PairNumber == repeatedRequest.PairNumber &&
                                          // request.Status == Status.Pending.ToString() &&
                                          request.KeyId == repeatedRequest.KeyId &&
                                          request.User.Role == Role.Student.ToString())
                        .ToList();

                    foreach (Request req in pendingRequests)
                    {
                        req.Status = Status.Rejected.ToString();
                    }
                }
            }

            _contextDb.SaveChanges();
        }

        public void CreateRequest(CreateRequest createRequest, Guid userId, Role userRole, bool deanCreate = false) // ++
        {
            if (createRequest.RepeatCount < 1 || 10 < createRequest.RepeatCount)
            {
                throw new BadRequestException("RepeatCount from 1 to 10");
            }

            createRequest.DateTime = createRequest.DateTime.Date.ToLocalTime();
            Status requestStatus = Status.Pending; //deanCreate ? Status.Accepted : Status.Pending;

            if (userRole == Role.Student)
            {
                var teachersAcceptedRequests = _contextDb.Requests
                    .Include(request => request.User)
                    .Where(request => request.KeyId == createRequest.KeyId &&
                           request.DateTime.Date == createRequest.DateTime.Date &&
                           request.PairNumber == (short)createRequest.PairNumber &&
                           request.Status == Status.Accepted.ToString() &&
                           (request.User.Role == Role.Teacher.ToString() || request.User.Role == Role.Dean.ToString()));

                requestStatus = teachersAcceptedRequests.Any() ? Status.Rejected : Status.Pending;
                createRequest.RepeatCount = 1;
            }

            var key = _contextDb.Keys.FirstOrDefault(key => key.Id == createRequest.KeyId) ?? throw new BadRequestException($"Key with guid {createRequest.KeyId} not found");

            Guid RepeatId = Guid.NewGuid();
            for (int i = 0; i < createRequest.RepeatCount; i++)
            {
                var newRequest = new Request()
                {
                    Name = createRequest.PairName ?? "",
                    Status = requestStatus.ToString(),
                    DateTime = createRequest.DateTime.AddDays(7 * i),
                    KeyId = createRequest.KeyId,
                    UserId = userId,
                    PairNumber = (short)createRequest.PairNumber,
                    Type = createRequest.TypeBooking.ToString(),
                    Id = Guid.NewGuid(),
                    RepeatId = RepeatId
                };

                _contextDb.Requests.Add(newRequest);
            }
 
            if (_contextDb.SaveChanges() != createRequest.RepeatCount)
            {
                throw new DbUpdateException("Failed to save!");
            }
            
            if (deanCreate)
            {
                var requestId = (_contextDb.Requests.FirstOrDefault(request => request.RepeatId == RepeatId) ?? throw new BadRequestException("Ошибка при подтверждении пары, созданной деканатом!")).Id ;
                AcceptOrCancelRequest(requestId, new() { Accept = true });
            }
        }

        public TableDTO GetAcceptedRequests(Guid? audienceId, DateTime? WeekStart) // ++
        {
            return GetRequests(new RequestsFilter() { Status = [Status.Accepted], WeekStart = WeekStart, AudienceId = audienceId });
        }

        public TableDTO GetUsersRequests(Guid userId, DateTime? WeekStart) // ++
        {
            if (WeekStart is null)
            {
                WeekStart = DateTime.Now.AddDays(1 - (int)DateTime.Now.DayOfWeek);
            }

            var request = _contextDb.Requests
                .Include(request => request.Key)
                .Include(request => request.User)
                .Where(request => request.UserId == userId)
                .SelectRequestDTO()
                .ToList();

            return new TableDTO()
            {
                WeekStart = (DateTime)WeekStart,
                WeekEnd = WeekStart.Value.AddDays(7),
                Requests = request
            };
        }

        public List<Audience> GetFreeAudiences(AudienceFilter audienceFilter, Role userRole)
        {
            if (audienceFilter.PairNumber == null || audienceFilter.BookingTime == null || audienceFilter.RepeatedCount == null)
            {
                throw new BadRequestException("PairNumber or BookingTime or RepeatedCount is null!");
            }

            if (audienceFilter.RepeatedCount < 1 || 10 < audienceFilter.RepeatedCount)
            {
                throw new BadRequestException("RepeatedCount from 1 to 10");
            }

            // находится в этом интервале и дни недели равны audienceFilter.BookingTime -- audienceFilter.BookingTime + audienceFilter.RepeatedCount * 7 == audienceFilter.BookingTime.Value.Date

            var leftBorder = audienceFilter.BookingTime.Value.Date;
            var rightBorder = audienceFilter.BookingTime.Value.Date.AddDays(((int)audienceFilter.RepeatedCount - 1) * 7);
            var DayOfWeek = audienceFilter.BookingTime.Value.Date.DayOfWeek;
            return _contextDb.Keys
                .Where(key => !key.Requests.Any(request =>
                    (userRole == Role.Teacher || userRole == Role.Dean || userRole == Role.Admin ? request.User.Role == "TEACHER" || request.User.Role == "DEAN" || request.User.Role == "ADMIN" : true) &&
                    request.Status == Status.Accepted.ToString() &&
                    request.PairNumber == (short)audienceFilter.PairNumber &&
                    (leftBorder <= request.DateTime.Date && request.DateTime.Date <= rightBorder && request.DateTime.Date.DayOfWeek == DayOfWeek)
                 ))
                .Select(key => new Audience() { Name = key.Room, KeyId = key.Id })
                .ToList();
        }

        public void CreatePair(CreatePair createPair) // ++
        {
            var userIdTeacher = _contextDb.Users
                .Any(user => user.Id == createPair.TeacherId &&
                     (user.Role == "Teacher" || user.Role == "Dean" || user.Role == "Admin" ||
                      user.Role == "TEACHER" || user.Role == "DEAN" || user.Role == "ADMIN"));
            if (!userIdTeacher)
            {
                throw new BadRequestException("User not found or user is not teacher!");
            }

            CreateRequest(new CreateRequest()
            {
                DateTime = createPair.DateTime,
                KeyId = createPair.KeyId,
                PairNumber = createPair.PairNumber,
                RepeatCount = createPair.RepeatCount,
                TypeBooking = TypeBooking.Pair,
                PairName = createPair.PairName
            }, createPair.TeacherId, Role.Teacher, true);
        }
    }
}
/*
 Bearer eyJhbGciOiJIUzI1NiJ9.eyJzdWIiOiJ1c2Vhc2Zhc2ZzYXNmckBleGFtcGxlLmNvbSIsInJvbGVzIjpbIlJPTEVfREVBTiJdLCJleHAiOjE3MTgwMTg4MjQsInVzZXJJZCI6IjNmYTg1ZjY0LTU3MTctNDU2Mi1iM2ZjLTJjOTYzZjY2YWZhNiIsImlhdCI6MTcwODAxNTIyNCwianRpIjoiNGFmNDVlZmItZTRiMy00NjJjLWE2NjQtZmQ3OWFkN2QxZTIzIn0._KlW8JMr6O5oK08ZjioqY2kyDk_NJkVfNj6YWFCEW50

{
  "dateTime": "2024-02-17T08:07:21.201Z",
  "repeatCount": 1,
  "typeBooking": "Booking",
  "pairNumber": 0,
  "keyId": "1806dab8-e3bd-42b5-969c-d6c0f01662c8"
}

{
  "dateTime": "2024-02-17T08:58:47.382Z",
  "repeatCount": 2,
  "pairNumber": 0,
  "teacherId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "keyId": "1806dab8-e3bd-42b5-969c-d6c0f01662c8"
}
 
 */