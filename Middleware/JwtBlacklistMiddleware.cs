using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SafeScribe.Api.Services;

namespace SafeScribe.Api.Middleware
{
    public class JwtBlacklistMiddleware
    {
        private readonly RequestDelegate _next;
        public JwtBlacklistMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext ctx, ITokenBlacklistService blacklist)
        {
            if (ctx.User?.Identity?.IsAuthenticated == true)
            {
                var jti = ctx.User.FindFirst(ClaimTypes.Sid)?.Value
                          ?? ctx.User.FindFirst("jti")?.Value;

                if (!string.IsNullOrEmpty(jti) && await blacklist.IsBlacklistedAsync(jti))
                {
                    ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await ctx.Response.WriteAsync("Token invalidado (logout).");
                    return;
                }
            }
            await _next(ctx);
        }
    }
}
