using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Domain;
using backend.Domain.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Data.Configuration.User
{
    public class PersonConfiguration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.HasOne(p => p.Account)
                .WithOne(a => a.Person)
                .HasForeignKey<Account>(a => a.PersonId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(p => p.FirstName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(p => p.LastName)
                .HasMaxLength(100)
                .IsRequired();

            // 'Carnet' (ID/Document) should usually be unique and indexed
            builder.Property(p => p.Carnet)
                .HasMaxLength(50)
                .IsRequired();

            builder.HasIndex(p => p.Carnet)
                .IsUnique();

            builder.Property(p => p.Telephone)
                .HasMaxLength(30)
                .IsRequired();

            // Base entity auditing property tracking (from EntityBase)
            builder.Property(p => p.CreatedBy)
                .IsRequired();
        }
    }
}