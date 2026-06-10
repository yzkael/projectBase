using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Askmethat.Aspnet.JsonLocalizer.Localizer;
using backend.Dtos.Auth;
using backend.Services.Auth;
using backend.Services.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _authServices;
        private readonly IJsonStringLocalizer _localizer;
        public AuthController(IAuthServices authService, IJsonStringLocalizer localizer)
        {
            _authServices = authService;
            _localizer = localizer;
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginRequest(LoginRequestDto dto)
        {
            var loginResponse = await _authServices.Login(dto);
            if (loginResponse == null)
            {
                return BadRequest(_localizer["WrongCredentials"].Value);
            }
            return Ok(loginResponse);
        }
        [HttpPost]
        [Route("logout")]
        [AllowAnonymous]
        public IActionResult Logout()
        {
            var logoutSuccesful = _authServices.Logout();
            if (!logoutSuccesful)
            {
                return BadRequest(_localizer["LogoutFailed"].Value);
            }
            return Ok(_localizer["LogoutSuccess"].Value);
        }
    }

}