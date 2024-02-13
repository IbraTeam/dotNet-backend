using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using dotNetBackend.models.DbFirst;

namespace dotNetBackend.CustomValidationAttributes
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        public CustomAuthorizeAttribute()
        { }   

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            await CheckTokenJTI(context);
            CheckTokenUserId(context);
        }

        private async Task CheckTokenJTI(AuthorizationFilterContext context)
        {
            var JTI = GetJTIFromToken(context.HttpContext);
            var userId = GetUserIdFromToken(context.HttpContext);

            var dbContext = context.HttpContext.RequestServices.GetService<NewContext>();
            //var accessToken = await dbContext.RefreshTokens.FirstOrDefaultAsync(refreshToken => refreshToken.AccessTokenJTI == JTI && refreshToken.UserId == userId);
            //if (accessToken == null)
            //{
            //    Forbid(context, "JWTToken", "Error: this token is no longer available!");
            //    return;
            //}
        }

        private void CheckTokenUserId(AuthorizationFilterContext context)
        {
            var userGuidStr = GetUserIdFromToken(context.HttpContext).ToString();

            if (userGuidStr == null)
            {
                Forbid(context, "JWTToken", "Error: a token without a UserId.");
                return;
            }

            if (!Guid.TryParse(userGuidStr, out Guid userGuid))
            {
                Forbid(context, "JWTToken", "Error: a token contain an uncorrected userId");
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

        public static Guid GetUserIdFromToken(HttpContext httpContext)
        {
            var userGuidStr = httpContext.User.Claims.First(claim => claim.Type == "UserId");

            return Guid.Parse(userGuidStr.Value);
        }

        //public static string? GetValueFromToken(HttpContext httpContext, string type)
        //{
        //    var userGuidStr = httpContext.User.Claims.First(claim => claim.Type == type);

        //    return (userGuidStr != null) ? userGuidStr.Value : null;
        //}
    }
}
