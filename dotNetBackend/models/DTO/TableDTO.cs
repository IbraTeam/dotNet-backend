namespace dotNetBackend.models.DTO
{
    public class TableDTO
    {
        public List<RequestDTO> Requests { get; set; } = null!;
        public DateTime WeekStart { get; set; }
        public DateTime WeekEnd { get; set; }
    }
}
