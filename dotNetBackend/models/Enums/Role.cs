using System.Text.Json.Serialization;

namespace dotNetBackend.models.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Role // Авторизации зависит от порядка ролей, чем выше, тем больше прав
    {
        USER,
        STUDENT,
        TEACHER,
        DEAN,
        ADMIN
    }

    public static class RoleExtantion
    {
        public static Role ToRole(this string status)
        {
            return status switch
            {
                "DEAN" => Role.DEAN,
                "TEACHER" => Role.TEACHER,
                "STUDENT" => Role.STUDENT,
                "USER" => Role.USER,
                "ADMIN" => Role.ADMIN,
                _ => throw new InvalidDataException()
            };
        }
    }
}
