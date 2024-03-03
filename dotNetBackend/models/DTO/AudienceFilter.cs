using dotNetBackend.models.Enums;

namespace dotNetBackend.models.DTO
{
    public class AudienceFilter
    {
        public DateTime? BookingTime { get; set; }
        public PairNumber? PairNumber { get; set; }
        public int? RepeatedCount { get; set; } 
    }
}
