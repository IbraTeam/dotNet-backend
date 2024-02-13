using dotNetBackend.models.Enums;

namespace dotNetBackend.models.DTO
{
    public class RequestsFilter
    {
        public DateOnly start { set; get; }
        public DateOnly end { set; get; }
        public Status status { set; get; }
        public TypeBooking type { set; get; }
        public PairNumber pairNumber { set; get; }
    }
}
