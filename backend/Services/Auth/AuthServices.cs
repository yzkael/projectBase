using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Domain.User;
using backend.Dtos.Auth;
using backend.Services.Token;
using Microsoft.AspNetCore.Identity;

namespace backend.Services.Auth
{
    public class AuthServices : IAuthServices
    {
        private readonly UserManager<Account> _userManager;
        private readonly ITokenServices _tokenServices;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthServices(UserManager<Account> userManager, ITokenServices tokenServices, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _tokenServices = tokenServices;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<LoginResponseDto?> Login(LoginRequestDto loginRequest)
        {
            var userExists = await _userManager.FindByNameAsync(loginRequest.Username);
            if (userExists == null)
            {
                return null;
            }
            if (userExists.IsActive == false || userExists.EmailConfirmed == false)
            {
                return null;
            }
            var passwordMatch = await _userManager.CheckPasswordAsync(userExists, loginRequest.Password);
            if (passwordMatch == false)
            {
                return null;
            }
            var accessToken = await _tokenServices.GenerateAccessToken(userExists);
            var refreshToken = _tokenServices.GenerateRefreshToken(userExists);
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                // 3. Append Access Token Cookie
                httpContext.Response.Cookies.Append("access-token", accessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false, // ⚠️ Change to 'true' in production when using HTTPS!
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(15) // Best practice: Match your actual JWT expiry (15 mins)
                });

                // 4. Append Refresh Token Cookie
                httpContext.Response.Cookies.Append("refresh-token", refreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false, // ⚠️ Change to 'true' in production when using HTTPS!
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddDays(1), // Best practice: Match your actual JWT expiry (1 day)
                    Path = "/api/auth/refresh" // Keeps cookie isolated to the refresh route
                });
            }
            return new LoginResponseDto
            {
                Username = userExists.UserName!,
                FullName = $"{userExists.Person.FirstName} {userExists.Person.LastName}",
                Privilege = (await _userManager.GetRolesAsync(userExists)).ToList()
            };
        }
    }
}