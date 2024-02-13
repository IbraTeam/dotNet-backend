using dotNetBackend.models.DbFirst;
using dotNetBackend.models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace dotNetBackend.Servises
{
    public interface IRequestService
    {
        List<UserDTO> GetUsers();
        RequestDTO CreatRequest(CreateRequest createRequest);
        List<RequestDTO> GetRequests(RequestsFilter requestsFilter);
        RequestDTO CancelRequest(Guid requestId);
        RequestDTO AcceptOrCancelRequest(Guid requestId, bool accept);
        List<RequestDTO> GetBooking(Guid audienceId);
    }
}
