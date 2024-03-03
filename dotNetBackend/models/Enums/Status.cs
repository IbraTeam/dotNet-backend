using System.Text.Json.Serialization;

namespace dotNetBackend.models.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Status
    {
        Pending,
        Accepted,
        Rejected
    }

    //public static class StatusExtantion
    //{
    //    public static Status ToStatus(this string status)
    //    {
    //        return status switch
    //        {
    //            "Pending" => Status.Pending,
    //            "Accepted" => Status.Accepted,
    //            "Rejected" => Status.Rejected,
    //            _ => throw new InvalidDataException()
    //        };
    //    }
    //}
}
