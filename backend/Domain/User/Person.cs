using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Domain.User;

namespace backend.Domain
{
    public class Person(Guid createdBy) : EntityBase(createdBy)
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Carnet { get; set; } = null!;
        public string Telephone { get; set; } = null!;
        public byte[]? Image { get; set; }
        public Account? Account { get; set; }
    }
}