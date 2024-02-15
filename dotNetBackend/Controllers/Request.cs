﻿using dotNetBackend.CustomValidationAttributes;
using dotNetBackend.Helpers;
using dotNetBackend.models.DbFirst;
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
        private NewContext _newContext;

        public Request (IRequestService requestService, NewContext newContext)
        {
            _requestService = requestService;
            _newContext = newContext;
        }

        [HttpGet("users")] // Получение списка заявок пользователя (со всеми статусами) - /api/request/users 
        [CustomAuthorize(UserRole = "Student")]
        public TableDTO GetUsersRequests([FromQuery] DateTime? WeekStart)
        {
            Guid userId = JWTTokenHelper.GetUserIdFromToken(HttpContext);

            return _requestService.GetUsersRequests(userId, WeekStart);
        }

        [HttpPost("create")]  // Создание заявки - /api/request/create 
        [CustomAuthorize(UserRole = "Student")]
        public RequestDTO CreatRequest([FromBody] CreateRequest createRequest) 
        {
            Guid userId = JWTTokenHelper.GetUserIdFromToken(HttpContext);
            Role userRole = JWTTokenHelper.GetHeighstRoleFromToken(HttpContext);

            return _requestService.CreateRequest(createRequest, userId, userRole);
        }

        [HttpGet] // Получение всех заявок с фильтрацией и пагинацией(для деканата): /api/request 
        [CustomAuthorize(UserRole = "Dean")]
        public TableDTO GetListRequests([FromQuery] RequestsFilter requestsFilter)
        {
            return _requestService.GetRequests(requestsFilter);
        }

        [HttpDelete("{requestId}")] // Отмена заявки: /api/request/:requestId(delete) 
        [CustomAuthorize(UserRole = "Student")]
        public ResponseDTO CancelRequest([FromRoute] Guid requestId)
        {
            Guid userId = JWTTokenHelper.GetUserIdFromToken(HttpContext);

            _requestService.CancelRequest(requestId, userId);

            return new ResponseDTO()
            {
                StatusCode = 200,
                Message = "OK"
            };
        }

        [HttpPost("{requestId}")] // Подтверждение/отклонение заявки (для деканата): /api/request/:requestId (post) 
        [CustomAuthorize(UserRole = "Dean")]
        public RequestDTO AcceptOrCancelRequest([FromRoute] Guid requestId, [FromBody] bool accept)
        {
            return _requestService.AcceptOrCancelRequest(requestId, accept);
        }

        [HttpGet("approved/{audienceId}")] // Получение подтвержденных заявок /api/request/approved/:audienceId
        [CustomAuthorize(UserRole = "User")]
        public TableDTO GetListBooking([FromRoute] Guid audienceId, [FromQuery] DateTime? WeekStart)
        {
            return _requestService.GetAcceptedRequests(audienceId, WeekStart);
        }

        [HttpGet("test")]
        public string Test()
        {
            return "Ok";
        }

        [HttpGet("testDb")]
        public string TestBd()
        {
            var request = _newContext.Requests.FirstOrDefault();

            string ans = "request not found";

            if (request != null)
            {
                ans = $"some request guid: {request.Id}";
            }

            return ans;
        }

        [HttpGet("testRedis")]
        public string TestRedis()
        {
            var db = RedisManager.GetDatabase();

            string? key = db.StringGet("key");

            return key != null ? $"there is key with value: {key}" : "there is not key";
        }
    }
}

