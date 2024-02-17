using dotNetBackend.models.Enums;

namespace dotNetBackend.models.DTO
{
    public class CreateRequest
    {
        public DateTime DateTime { get; set; }
        //public bool Repeated { get; set; }
        public int RepeatCount { get; set; }
        public TypeBooking TypeBooking { get; set; }
        public PairNumber PairNumber { get; set; }
        public Guid KeyId { get; set; }
    }
}
