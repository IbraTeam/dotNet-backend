using dotNetBackend.models.DTO;
using dotNetBackend.models.Enums;

namespace dotNetBackend.Servises
{
    public interface IRequestService
    {
        TableDTO GetUsersRequests(Guid userId, DateTime? WeekStart); //  Получение списка заявок пользователя на забронированные аудитории - /api/request/users
        RequestDTO CreateRequest(CreateRequest createRequest, Guid userId, Role userRole); // Создание заявки - /api/request/creat
        TableDTO GetRequests(RequestsFilter requestsFilter); // Получение всех заявок с фильтрацией и пагинацией(для деканата) : /api/request
        void CancelRequest(Guid requestId, Guid userId); // Отмена заявки: /api/request/:requestId  (delete)
        RequestDTO AcceptOrCancelRequest(Guid requestId, bool accept); // Подтверждение/отклонение заявки (для деканата): /api/request/:requestId (post)
        TableDTO GetAcceptedRequests(Guid audienceId, DateTime? WeekStart); // Получение бронирований аудитории (передаем id аудитории) /api/request/:audienceId
        List<Audience> GetFreeAudiences(AudienceFilter audienceFilter);
        RequestDTO CreatePair(CreatePair createPair);
    }
}