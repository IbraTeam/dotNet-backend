using System.Text.Json.Serialization;

namespace dotNetBackend.models.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TypeBooking
    {
        Booking,
        Pair
    }

    //public static class TypeBookingExtantion
    //{
    //    public static TypeBooking ToTypeBooking(this string status)
    //    {
    //        return status switch
    //        {
    //            "Booking" => TypeBooking.Booking,
    //            "Pair" => TypeBooking.Pair,
    //            _ => throw new InvalidDataException()
    //        };
    //    }
    //}
}
