using dotNetBackend.models.DbFirst;
using dotNetBackend.models.DTO;
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
            return new RequestDTO()
            {

            };
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

        public List<UserDTO> GetUsers()
        {
            throw new NotImplementedException();
        }
    }
}
