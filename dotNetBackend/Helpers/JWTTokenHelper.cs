using dotNetBackend.models.DbFirst;
using dotNetBackend.models.Enums;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace dotNetBackend.Helpers
{
    public static class JWTTokenHelper
    {
        public static Guid GetUserIdFromToken(HttpContext httpContext)
        {
            var userGuidStr = httpContext.User.Claims.First(claim => claim.Type == "UserId");

            return Guid.Parse(userGuidStr.Value);
        }

        public static Guid GetJTIFromToken(HttpContext httpContext)
        {
            var userGuidStr = httpContext.User.Claims.First(claim => claim.Type == "JTI");

            return Guid.Parse(userGuidStr.Value);
        }

        public static List<Role> GetRolesFromToken(HttpContext httpContext)
        {
            return httpContext.User.Claims
                .Where(claim => claim.Type == "roles")
                .Select(claim => claim.Value.ToRole())
                .ToList();
        }
    }
}
