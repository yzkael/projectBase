using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Domain.User
{
    public class Job(Guid createdBy) : EntityBase(createdBy)
    {
        public List<Privilege> Privileges { get; set; } = [];
    }
}