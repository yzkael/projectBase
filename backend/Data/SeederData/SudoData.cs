using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Data.SeederData
{
    public class SudoData
    {
        public const string Section = "SudoData";
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Carnet { get; set; } = null!;
        public string Telephone { get; set; } = null!;
        public byte[]? Image { get; set; }
        public string Role { get; set; } = null!;
        public string Privilege { get; set; } = null!;
        public SudoAccount Account { get; set; } = null!;
    }

    public class SudoAccount
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}