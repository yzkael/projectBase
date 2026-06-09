using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Dtos.Auth;
using backend.Services.Auth;
using backend.Services.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _authServices;

        public AuthController(IAuthServices authService)
        {
            _authServices = authService;
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginRequest(LoginRequestDto dto)
        {
            var loginResponse = await _authServices.Login(dto);
            if (loginResponse == null)
            {
                return BadRequest("Wrong username or password.");
            }
            return Ok(loginResponse);
        }
    }
}