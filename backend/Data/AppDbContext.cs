using System;
using backend.Domain.User;
using Microsoft.AspNetCore.Identity;
using Audit.EntityFramework;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using backend.Domain;
using backend.Domain.Metadata;

namespace backend.Data
{
    public class AppDbContext : AuditIdentityDbContext<Account, Privilege, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

    }
}