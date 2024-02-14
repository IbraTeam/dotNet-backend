using dotNetBackend.models.DbFirst;

namespace dotNetBackend.models.DTO
{
    public class RequestDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime DateTime { get; set; }
        public bool Repeated { get; set; }
        public string TypeBooking { get; set; } = null!;
        public short PairNumber { get; set; }
        public Guid? KeyId { get; set; }
    }

    public static class RequestExtantion
    {
        public static IQueryable<RequestDTO> SelectRequestDTO(this IQueryable<Request> request)
        {
            return request.Select(request => new RequestDTO()
            {
                Id = request.Id,
                Name = request.Name,
                DateTime = request.DateTime,
                Status = request.Status,
                PairNumber = request.PairNumber,
                Repeated = request.Repeated,
                TypeBooking = request.Type,
                KeyId = request.KeyId
            });
        }

        public static RequestDTO ToRequestDTO(this Request request)
        {
            return new RequestDTO
            {
                Id = request.Id,
                Name = request.Name,
                Status = request.Status.ToString(),
                DateTime = request.DateTime,
                Repeated = request.Repeated,
                KeyId = request.KeyId,
                PairNumber = request.PairNumber,
                TypeBooking = request.Type
            };
        }
    }
}
