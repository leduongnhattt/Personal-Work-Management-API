using Microsoft.AspNetCore.Http;
using PersonalWorkManagement.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

namespace PersonalWorkManagement.Middleware
{
    public class TokenRefreshMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenRefreshMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip token refresh for login and refresh endpoints
            if (context.Request.Path.Value == "/api/Auth/login" || 
                context.Request.Path.Value == "/api/Auth/refresh" ||
                context.Request.Path.Value == "/api/Auth/register")
            {
                await _next(context);
                return;
            }

            // Check if we have a refresh token
            if (context.Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
            {
                // Check if the current request has an expired token
                var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (!string.IsNullOrEmpty(token))
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(token);

                    // If token is expired, try to refresh it
                    if (jwtToken.ValidTo < DateTime.UtcNow)
                    {
                        var userService = context.RequestServices.GetRequiredService<UserService>();
                        var response = await userService.RefreshTokenAsync(refreshToken);
                        if (response.Success)
                        {
                            // Update the refresh token cookie
                            context.Response.Cookies.Append("refreshToken", response.Data.RefreshToken, new CookieOptions
                            {
                                HttpOnly = true,
                                Secure = true,
                                SameSite = SameSiteMode.None,
                                Expires = DateTime.UtcNow.AddDays(7)
                            });

                            // Update the Authorization header with the new token
                            context.Request.Headers["Authorization"] = $"Bearer {response.Data.AccessToken}";
                        }
                    }
                }
            }

            await _next(context);
        }
    }
} 