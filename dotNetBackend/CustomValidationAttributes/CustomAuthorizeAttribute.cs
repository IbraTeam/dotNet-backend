using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using dotNetBackend.models.Enums;
using Microsoft.IdentityModel.Tokens;
using dotNetBackend.Helpers;
using dotNetBackend.models.DbFirst;

namespace dotNetBackend.CustomValidationAttributes
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        public string? UserRole { set; get; }

        public CustomAuthorizeAttribute()
        { }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (await CheckTokenJTI(context)) return;
            if (CheckRoles(context)) return;
        }

        private bool CheckRoles(AuthorizationFilterContext context)
        {
            if (UserRole == null) return false;
            Role userRole = UserRole.ToRole();

            // User < Student < Teacher < deam < admin
            List<Role> roles = JWTTokenHelper.GetRolesFromToken(context.HttpContext);
            if (roles.Where(role => (int)role >= (int)userRole).ToList().IsNullOrEmpty())
            {
                Forbid(context, "JWTToken", "Error: bad role!");
                return true;
            }
            return false;
        }

        private async Task<bool> CheckTokenJTI(AuthorizationFilterContext context)
        {
            var db = RedisManager.GetDatabase();
            var JTI = JWTTokenHelper.GetJTIFromToken(context.HttpContext);
            
            string? accessToken = await db.StringGetAsync(JTI.ToString());
            if (accessToken == null)
            {
                Forbid(context, "JWTToken", "Error: this token is no longer available!");
                return true;
            }
            return false;
        }

        private void Forbid(AuthorizationFilterContext context, string key, string message)
        {
            context.HttpContext.Response.Headers.TryAdd(key, message);
            context.Result = new ForbidResult();
        }
    }
}
