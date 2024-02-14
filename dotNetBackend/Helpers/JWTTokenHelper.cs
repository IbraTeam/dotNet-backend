﻿using dotNetBackend.models.Enums;

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

        public static Role GetHeighstRoleFromToken(HttpContext httpContext)
        {
            var roles = httpContext.User.Claims
                .Where(claim => claim.Type == "roles")
                .Select(claim => claim.Value.ToRole())
                .ToList();
            roles.Sort(new RoleComparator());

            return roles[roles.Count - 1];
        }
    }
}
