using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Dtos.Auth;

namespace backend.Services.Auth
{
    public interface IAuthServices
    {
        public Task<LoginResponseDto?> Login(LoginRequestDto loginRequest);
        public bool Logout();
    }
}