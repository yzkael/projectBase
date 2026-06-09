using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Domain.User;

namespace backend.Services.Token
{
    public interface ITokenServices
    {
        public Task<string> GenerateAccessToken(Account userAccount);
        public string GenerateRefreshToken(Account userAccount);
        public Task<string?> RefreshAccessToken(string refreshToken);
    }
}