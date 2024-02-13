namespace dotNetBackend.models.Enums
{
    public enum Status
    {
        Pending,
        Accepted,
        Rejected
    }

    public static class StatusExtantion
    {
        public static Status ToStatus(this string status)
        {
            return status switch
            {
                "Pending" => Status.Pending,
                "Accepted" => Status.Accepted,
                "Rejected" => Status.Rejected,
                _ => throw new InvalidDataException()
            };
        }
    }
}
