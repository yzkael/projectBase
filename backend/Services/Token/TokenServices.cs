using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using backend.Domain.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace backend.Services.Token
{
    public class TokenServices : ITokenServices
    {
        private readonly UserManager<Account> _userManager;
        private readonly SymmetricSecurityKey _key;
        private readonly IConfiguration _config;

        public TokenServices(IConfiguration config, SymmetricSecurityKey key, UserManager<Account> userManager)
        {
            _config = config;
            _key = key;
            _userManager = userManager;
        }

        public async Task<string> GenerateAccessToken(Account userAccount)
        {
            var roles = await _userManager.GetRolesAsync(userAccount);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, userAccount.Id.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            return CreateToken(claims, DateTime.UtcNow.AddMinutes(15));
        }

        public string GenerateRefreshToken(Account userAccount)
        {
            // Refresh tokens usually don't need roles. Keeping it minimal reduces payload size.
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, userAccount.Id.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
            };
            return CreateToken(claims, DateTime.UtcNow.AddDays(1));
        }

        public async Task<string?> RefreshAccessToken(string refreshToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _key,
                ValidateIssuer = true,
                ValidIssuer = _config["JWT:Issuer"],
                ValidateAudience = true,
                ValidAudience = _config["JWT:Audience"],
                ValidateLifetime = true, // 👈 Crucial: Rejects expired refresh tokens
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                // 👈 Crucial: Validates the signature and integrity of the token
                var principal = tokenHandler.ValidateToken(refreshToken, validationParameters, out SecurityToken validatedToken);

                // Double check that it's actually a JWT
                if (validatedToken is not JwtSecurityToken jwtSecurityToken ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }

                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                             ?? principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

                if (string.IsNullOrEmpty(userId)) return null;

                var usuario = await _userManager.FindByIdAsync(userId);
                if (usuario == null) return null;

                return await GenerateAccessToken(usuario);
            }
            catch (Exception)
            {
                // Token validation failed (expired, tampered, bad signature, etc.)
                return null;
            }
        }

        // Helper method to keep your code DRY (Don't Repeat Yourself)
        private string CreateToken(IEnumerable<Claim> claims, DateTime expiresAt)
        {
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _config["JWT:Issuer"],
                Audience = _config["JWT:Audience"],
                SigningCredentials = creds,
                IssuedAt = DateTime.UtcNow,
                NotBefore = DateTime.UtcNow,
                Expires = expiresAt
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescription);

            return tokenHandler.WriteToken(token);
        }
    }
}