using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Data.SeederData;
using backend.Domain;
using backend.Domain.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace backend.Data
{
    public class AppDbSeeder
    {
        private readonly ILogger<AppDbSeeder> _logger;
        private readonly AppDbContext _context;
        private readonly SudoData _sudoData;
        private readonly UserManager<Account> _userManager;
        public AppDbSeeder(ILogger<AppDbSeeder> logger, AppDbContext context, IOptions<SudoData> sudoData, UserManager<Account> userManager)
        {
            _logger = logger;
            _context = context;
            _sudoData = sudoData.Value;
            _userManager = userManager;
        }

        public async Task InitialiseAsync()
        {
            try
            {
                await TrySeedAsync();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        public async Task TrySeedAsync()
        {
            // Apply pending migrations
            if ((await _context.Database.GetPendingMigrationsAsync()).Any())
            {
                _logger.LogInformation("Applying pending migrations...");
                await _context.Database.MigrateAsync();
                _logger.LogInformation("Migrations applied successfully.");
            }
            else
            {
                _logger.LogInformation("No pending migrations found.");
            }

            // Seed Sudo Person
            if (!await _context.Persons.AnyAsync(p => p.Carnet == _sudoData.Carnet))
            {
                var newSudoPerson = new Person(Guid.NewGuid())
                {
                    FirstName = _sudoData.FirstName,
                    LastName = _sudoData.LastName,
                    Carnet = _sudoData.Carnet,
                    Telephone = _sudoData.Telephone,
                    Image = _sudoData.Image
                };
                _context.Persons.Add(newSudoPerson);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Sudo person created successfully.");
            }
            else
            {
                _logger.LogInformation("Sudo person already exists. Skipping creation.");
            }

            // Seed Sudo User Account
            if (!await _context.Users.AnyAsync(u => u.UserName == _sudoData.Account.Username))
            {
                var personId = await _context.Persons.Where(p => p.Carnet == _sudoData.Carnet).Select(p => p.Id).FirstOrDefaultAsync();
                var user = new Account
                {
                    UserName = _sudoData.Account.Username,
                    Email = _sudoData.Account.Email,
                    PersonId = personId
                };
                var result = await _userManager.CreateAsync(user, _sudoData.Account.Password);
                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to create sudo user: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                    throw new Exception("Failed to create sudo user.");
                }
                else
                {
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Sudo user created successfully.");
                }
            }
            else
            {
                _logger.LogInformation("Sudo user already exists. Skipping creation.");
            }

            // Sudo Privilege
            if (!await _context.Roles.AnyAsync(r => r.Name == _sudoData.Role))
            {
                var sudoRole = new Privilege()
                {
                    Name = _sudoData.Role,
                    NormalizedName = _sudoData.Role.ToUpper()

                };
                _context.Roles.Add(sudoRole);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Sudo role created successfully.");
            }
            else
            {
                _logger.LogInformation("Sudo role already exists. Skipping creation.");
            }
            // Assign Sudo Privilege to Sudo User
            var sudoUser = await _userManager.FindByNameAsync(_sudoData.Account.Username);
            if (sudoUser != null)
            {
                if (!await _userManager.IsInRoleAsync(sudoUser, _sudoData.Role))
                {
                    var result = await _userManager.AddToRoleAsync(sudoUser, _sudoData.Role);
                    if (!result.Succeeded)
                    {
                        _logger.LogError("Failed to assign sudo role to sudo user: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                        throw new Exception("Failed to assign sudo role to sudo user.");
                    }
                    else
                    {
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Sudo role assigned to sudo user successfully.");
                    }

                }

            }
        }
    }

    public static class InitialiserExtensions
    {


        public static async Task InitialiseDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var initialiser = scope.ServiceProvider.GetRequiredService<AppDbSeeder>();

            await initialiser.InitialiseAsync();
        }
    }
}