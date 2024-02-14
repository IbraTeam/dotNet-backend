using dotNetBackend.models.DTO;
using dotNetBackend.models.Enums;

namespace dotNetBackend.Servises
{
    public interface IRequestService
    {
        List<RequestDTO> GetUsersRequests(Guid userId); //  Получение списка заявок пользователя на забронированные аудитории - /api/request/users
        RequestDTO CreatRequest(CreateRequest createRequest, Guid userId, Role userRole); // Создание заявки - /api/request/creat
        TableDTO GetRequests(RequestsFilter requestsFilter); // Получение всех заявок с фильтрацией и пагинацией(для деканата) : /api/request
        void CancelRequest(Guid requestId, Guid userId); // Отмена заявки: /api/request/:requestId  (delete)
        RequestDTO AcceptOrCancelRequest(Guid requestId, bool accept); // Подтверждение/отклонение заявки (для деканата): /api/request/:requestId (post)
        List<RequestDTO> GetAcceptedRequests(Guid audienceId); // Получение бронирований аудитории (передаем id аудитории) /api/request/:audienceId
    }
}
/*
    1.
    5. 
    6. 

    //    Получение расписания (подтвержденные заявки) - /api/request/approved
 */