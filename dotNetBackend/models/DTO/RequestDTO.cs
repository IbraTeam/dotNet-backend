using dotNetBackend.models.Enums;

namespace dotNetBackend.models.DTO
{
    public class RequestDTO
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public Status Status { get; set; }
        public DateTime DateTime { get; set; }
        public bool Repeated { get; set; }
        public TypeBooking TypeBooking { get; set; }
        public PairNumber PairNumber { get; set; }
        public Guid KeyId { get; set; }
    }
}
