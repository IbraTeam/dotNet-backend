using dotNetBackend.models.Enums;

namespace dotNetBackend.models.DTO
{
    public class RequestsFilter
    {
        public DateOnly? Start { set; get; }
        public DateOnly? End { set; get; }
        public Status? Status { set; get; }
        public TypeBooking? Type { set; get; }
        public PairNumber? PairNumber { set; get; }
    }
}
