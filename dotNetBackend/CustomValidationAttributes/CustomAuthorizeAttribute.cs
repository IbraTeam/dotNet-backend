using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using dotNetBackend.models.Enums;
using Microsoft.IdentityModel.Tokens;
using dotNetBackend.Helpers;

namespace dotNetBackend.CustomValidationAttributes
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        public string? UserRole { set; get; }

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
            Role userRole = UserRole.ToRole();

            // User < Student < Teacher < deam < admin
            List<Role> roles = JWTTokenHelper.GetRolesFromToken(context.HttpContext);
            if(roles.Where(role => (int)role >= (int)userRole).ToList().IsNullOrEmpty())
            {
                Forbid(context, "JWTToken", "Error: bad role!");
                return;
            }
        }

        private async Task CheckTokenJTI(AuthorizationFilterContext context)
        {
            var redisContext = context.HttpContext.RequestServices.GetService<IDistributedCache>();

            var JTI = JWTTokenHelper.GetJTIFromToken(context.HttpContext);
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
    }
}
