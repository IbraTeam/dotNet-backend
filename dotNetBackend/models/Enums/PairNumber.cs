using System.Text.Json.Serialization;

namespace dotNetBackend.models.Enums
{
    // [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PairNumber
    {
        First,
        Second,
        Third,
        Fourth,
        Fifth,
        Sixth,
        Seventh,
        Eighth,
        Ninth,
        Ten
    }

    //public static class PairNumberExtantion
    //{
    //    public static PairNumber ToPairNumber(this short status)
    //    {
    //        return status switch
    //        {
    //            0 => PairNumber.First,
    //            1 => PairNumber.Second,
    //            2 => PairNumber.Third,
    //            3 => PairNumber.Fourth,
    //            4 => PairNumber.Fifth,
    //            5 => PairNumber.Sixth,
    //            6 => PairNumber.Seventh,
    //            7 => PairNumber.Eighth,
    //            8 => PairNumber.Ninth,
    //            9 => PairNumber.Ten,
    //            _ => throw new InvalidDataException()
    //        };
    //    }
    //}
    
}
