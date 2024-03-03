using dotNetBackend.models.DbFirst;
using dotNetBackend.models.Enums;

namespace dotNetBackend.models.DTO
{
    public class RequestDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string PairName { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime DateTime { get; set; }
        public int DayNumb {
            get
            {
                if (DateTime.DayOfWeek == DayOfWeek.Sunday) return 6;
                return (int)DateTime.DayOfWeek - 1;
            }
        }
        //public bool Repeated { get; set; }
        public string TypeBooking { get; set; } = null!;
        public PairNumber PairNumber { get; set; }
        public Guid? KeyId { get; set; }
        public UserDTO? User { get; set; } = null!;
    }

    public static class RequestExtantion
    {
        public static IQueryable<RequestDTO> SelectRequestDTO(this IQueryable<Request> request)
        {
            return request.Select(request => new RequestDTO()
            {
                Id = request.Id,
                Name = request.Key.Room,
                PairName = request.Name,
                DateTime = request.DateTime,
                Status = request.Status,
                PairNumber = (PairNumber)request.PairNumber,
                //Repeated = request.Repeated,
                TypeBooking = request.Type,
                KeyId = request.KeyId,
                User = request.User == null ? null : new UserDTO()
                {
                    Id = request.User.Id,
                    Name = request.User.Name,
                    Email = request.User.Email,
                    Role = request.User.Role,
                }
            });
        }

        public static RequestDTO ToRequestDTO(this Request request)
        {
            return new RequestDTO
            {
                Id = request.Id,
                Name = request.Key.Room,
                PairName = request.Name,
                Status = request.Status.ToString(),
                DateTime = request.DateTime,
                //Repeated = request.Repeated,
                KeyId = request.KeyId,
                PairNumber = (PairNumber)request.PairNumber,
                TypeBooking = request.Type,
                User = request.User == null ? null : new UserDTO()
                {
                    Id = request.User.Id,
                    Name = request.User.Name,
                    Email = request.User.Email,
                    Role = request.User.Role,
                }
            };
        }
    }
}
