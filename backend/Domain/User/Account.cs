using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace backend.Domain.User
{
    public class Account : IdentityUser<Guid>
    {
        public Guid PersonId { get; set; }
        public Person Person { get; set; } = null!;
    }
}