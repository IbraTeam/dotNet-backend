using dotNetBackend.Controllers;
using dotNetBackend.models.DbFirst;
using dotNetBackend.models.DTO;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace dotNetBackend.Servises
{
    public interface IRequestService
    {
        List<RequestDTO> GetUsersRequests(Guid userId); //  Получение списка заявок пользователя на забронированные аудитории - /api/request/users
        RequestDTO CreatRequest(CreateRequest createRequest); // Создание заявки - /api/request/creat
        List<RequestDTO> GetRequests(RequestsFilter requestsFilter); // Получение всех заявок с фильтрацией и пагинацией(для деканата) : /api/request
        RequestDTO CancelRequest(Guid requestId); // Отмена заявки: /api/request/:requestId  (delete)
        RequestDTO AcceptOrCancelRequest(Guid requestId, bool accept); // Подтверждение/отклонение заявки (для деканата): /api/request/:requestId (post)
        List<RequestDTO> GetBooking(Guid audienceId); // Получение бронирований аудитории (передаем id аудитории) /api/request/:audienceId
    }
}
/*
    1.
    5. 
    6. 

    //    Получение расписания (подтвержденные заявки) - /api/request/approved
 */