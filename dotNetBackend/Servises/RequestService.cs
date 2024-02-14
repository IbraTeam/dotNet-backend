﻿using dotNetBackend.Exceptions;
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
                Requests = temp.OrderBy(request => request.DateTime).ThenBy(request => request.PairNumber).SelectRequestDTO().ToList(),
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

        public RequestDTO CreateRequest(CreateRequest createRequest, Guid userId, Role userRole)
        {
            createRequest.DateTime = createRequest.DateTime.Date.ToLocalTime();
            Status requestStatus = Status.Pending;

            var dublicatRequests = _contextDb.Requests
                    .Include(request => request.User)
                    .Any(request => request.KeyId == createRequest.KeyId &&
                                      request.UserId == userId &&
                                      (request.DateTime.Date == createRequest.DateTime.Date ||
                                          request.Repeated && request.DateTime.DayOfWeek == createRequest.DateTime.DayOfWeek));
            if (dublicatRequests)
            {
                throw new BadRequestException("Such an request already exists!");
            }

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

            var newRequest = new Request()
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

        public TableDTO GetAcceptedRequests(Guid audienceId, DateTime? WeekStart)
        {
            return GetRequests(new RequestsFilter() { Status = Status.Accepted, WeekStart = WeekStart });
        }

        public TableDTO GetUsersRequests(Guid userId, DateTime? WeekStart)
        {
            if (WeekStart is null)
            {
                WeekStart = DateTime.Now.AddDays(1 - (int)DateTime.Now.DayOfWeek);
            }

            var request = _contextDb.Requests
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
    }
}

/*
Задачи:
    8. Проверка, что не существую заявки дубликата при создании

    9. Докер
 */