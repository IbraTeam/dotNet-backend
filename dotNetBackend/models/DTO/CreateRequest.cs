using dotNetBackend.models.Enums;

namespace dotNetBackend.models.DTO
{
    public class CreateRequest
    {
        public string? Name { get; set; }
        public DateOnly DateOnly { get; set; }
        public bool Repeated { get; set; }
        public TypeBooking TypeBooking { get; set; }
        public PairNumber PairNumber { get; set; }
    }
}
