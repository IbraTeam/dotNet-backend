using System.Text.Json.Serialization;

namespace dotNetBackend.models.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Role // Авторизации зависит от порядка ролей, чем выше, тем больше прав
    {
        User = 0,
        Student = 1,
        Teacher = 2,
        Dean = 3,
        Admin = 4
    }

    public static class RoleExtantion
    {
        public static Role ToRole(this string status)
        {
            return status switch
            {
                "Dean" => Role.Dean,
                "Teacher" => Role.Teacher,
                "Student" => Role.Student,
                "User" => Role.User,
                "Admin" => Role.Admin,
                _ => throw new InvalidDataException()
            };
        }
    }

    public class RoleComparator : IComparer<Role>
    {
        public int Compare(Role x, Role y)
        {
            if (x == y) return 0;
            if ((int) x < (int) y) return -1;
            return 1;
        }
    }
}
