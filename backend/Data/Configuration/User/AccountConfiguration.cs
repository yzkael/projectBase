using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Domain.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Data.Configuration.User
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.HasOne(a => a.Person)
                .WithOne(p => p.Account)
                .HasForeignKey<Account>(a => a.PersonId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}