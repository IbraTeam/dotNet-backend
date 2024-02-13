using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using dotNetBackend.models.DbFirst;
using Microsoft.Extensions.Caching.Distributed;
using dotNetBackend.models.Enums;
using System.Linq;
using Microsoft.IdentityModel.Tokens;

namespace dotNetBackend.CustomValidationAttributes
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        public Role? UserRole {  get; set; }

        public CustomAuthorizeAttribute()
        { }   

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            await CheckTokenJTI(context);
            CheckRoles(context);
        }

        private void CheckRoles(AuthorizationFilterContext context)
        {
            if (UserRole == null) return;

            // User < Student < Teacher < deam < admin
            List<Role> roles = GetRolesFromToken(context.HttpContext);
            if(roles.Where(role => (int)role >= (int)UserRole).ToList().IsNullOrEmpty())
            {
                Forbid(context, "JWTToken", "Error: bad role!");
                return;
            }
        }

        private async Task CheckTokenJTI(AuthorizationFilterContext context)
        {
            var redisContext = context.HttpContext.RequestServices.GetService<IDistributedCache>();

            var JTI = GetJTIFromToken(context.HttpContext);
            var accessToken = await redisContext.GetStringAsync(JTI.ToString());
            if (accessToken == null)
            {
                Forbid(context, "JWTToken", "Error: this token is no longer available!");
                return;
            }
        }

        private void Forbid(AuthorizationFilterContext context, string key, string message)
        {
            context.HttpContext.Response.Headers.Add(key, message);
            context.Result = new ForbidResult();
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
