using DbUp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using Penni.Application.Common.Exceptions;
using Penni.Application.Common.Interfaces;
using Penni.Domain.Entities;
using Penni.Domain.Enum;
using System.Data;
using System.Reflection;

namespace Penni.Infrastructure.DbConfig
{
    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly ILogger<DatabaseInitializer> _logger;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public DatabaseInitializer(ILogger<DatabaseInitializer> logger, IConfiguration configuration, ApplicationDbContext context)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
        }

        public void CreateDatabase(string defaultConnection)
        {
            try
            {
                var builder = new NpgsqlConnectionStringBuilder(defaultConnection);
                string databaseName = builder.Database;

                // Temporarily switch to 'postgres' DB to run CREATE DATABASE
                builder.Database = "postgres";
                var serverConnectionString = builder.ConnectionString;

                using var connection = new NpgsqlConnection(serverConnectionString);
                connection.Open();

                using var checkDbCmd = new NpgsqlCommand("SELECT 1 FROM pg_database WHERE datname = @db", connection);
                checkDbCmd.Parameters.AddWithValue("db", databaseName);

                var exists = checkDbCmd.ExecuteScalar() != null;

                if (!exists)
                {
                    _logger.LogInformation("Creating database '{Database}'...", databaseName);
                    using var createDbCmd = new NpgsqlCommand($"CREATE DATABASE \"{databaseName}\"", connection);
                    createDbCmd.ExecuteNonQuery();
                    _logger.LogInformation("Database '{Database}' created successfully.", databaseName);
                }
                else
                {
                    _logger.LogInformation("Database '{Database}' already exists.", databaseName);
                }
            }
            catch (NpgsqlException ex)
            {
                _logger.LogError(ex, "PostgreSQL connection failed.");
                throw new PenniException(503, "SERVICE_UNAVAILABLE", ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during DB initialization.");
                throw new PenniException(500, "DATABASE_INIT_ERROR", ex.Message);
            }
        }
        public void MigrateDatabase(string connectionString)
        {
            var upgrader =
                DeployChanges.To
                    .PostgresqlDatabase(connectionString)
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                    .LogToConsole()
                    .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.Error);
                Console.ResetColor();
                throw new Exception("Database upgrade failed", result.Error);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Database upgrade successful");
            Console.ResetColor();
        }
        public async Task SeedAdminUserAsync()
        {
            var adminEmail = _configuration["Admin:Email"];

            // Check if user already exists
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == adminEmail);
            if (existingUser == null)
            {
                var user = new Users
                {
                    Id = Guid.NewGuid().ToString(),
                    Username = _configuration["Admin:Name"],
                    Email = adminEmail,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(_configuration["Admin:Password"]),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Assign admin role
                _context.UserRoles.Add(new UserRole
                {
                    UserId = user.Id,
                    RoleId = (int)UserRoleEnum.Admin
                });

                await _context.SaveChangesAsync();
            }
        }

        public async Task SeedRolesAsync()
        {
            var existingRoles = await _context.Roles.Select(r => r.RoleName).ToListAsync();

            foreach (var role in Enum.GetValues(typeof(UserRoleEnum)).Cast<UserRoleEnum>())
            {
                if (!existingRoles.Contains(role.ToString()))
                {
                    _context.Roles.Add(new Roles
                    {
                        Id = (int)role,
                        RoleName = role.ToString()
                    });
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
