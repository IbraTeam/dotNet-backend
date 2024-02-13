using dotNetBackend.models.DbFirst;
using dotNetBackend.models.Enums;
using System.Net;

namespace dotNetBackend.models.DTO
{
    public class RequestDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Status Status { get; set; }
        public DateTime DateTime { get; set; }
        public bool Repeated { get; set; }
        public TypeBooking TypeBooking { get; set; }
        public PairNumber PairNumber { get; set; }
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
                Status = request.Status.ToStatus(),
                PairNumber = request.PairNumber.ToPairNumber(),
                Repeated = request.Repeated,
                TypeBooking = request.Type.ToTypeBooking()
            });
        }

        public static RequestDTO ToRequestDTO(this Request request)
        {
            return new RequestDTO
            {
                Id = request.Id,
                Name = request.Name,
                Status = Status.Pending,
                DateTime = request.DateTime,
                Repeated = request.Repeated,
                KeyId = request.KeyId,
                PairNumber = request.PairNumber.ToPairNumber(),
                TypeBooking = request.Type.ToTypeBooking()
            };
        }
    }
}
