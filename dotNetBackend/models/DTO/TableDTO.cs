namespace dotNetBackend.models.DTO
{
    public class TableDTO
    {
        public List<RequestDTO> requests {  get; set; }
        public DateTime WeekStart { get; set; }
        public DateTime WeekEnd { get; set; }
    }
}
