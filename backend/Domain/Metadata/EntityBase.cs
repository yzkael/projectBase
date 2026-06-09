using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Domain
{
    public abstract class EntityBase
    {
        public EntityBase(Guid createdBy)
        {
            CreatedBy = createdBy;
            LastUpdateBy = createdBy;
        }
        public Guid Id { get; init; } = Guid.NewGuid();
        public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? DeletedAt { get; set; }
        public Guid CreatedBy { get; init; }
        public DateTimeOffset LastUpdateAt { get; set; } = DateTimeOffset.UtcNow;
        public Guid LastUpdateBy { get; set; }
        public bool IsActive { get; set; } = true;
    }
}