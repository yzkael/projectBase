using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Dtos.Auth
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; } = null!;
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = null!;
    }
    public class LoginResponseDto
    {
        public string Username { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public List<string> Privilege { get; set; } = [];
    }
}