using Heracles.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Infrastructure;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IWebHostEnvironment environment,
        IConfiguration configuration
    )
    {
        ConfigureDatabase(services, environment, configuration);
        return services;
    }

    private static void SeedDevelopmentUser(DbContext context)
    {
        var exists = context.Set<AppIdentityUser>().Any(u => u.Email == TestConstants.Email);

        if (exists)
        {
            return;
        }

        var devUser = new AppIdentityUser()
        {
            Email = TestConstants.Email,
            NormalizedEmail = TestConstants.Email.ToUpper(),
            UserName = TestConstants.Email,
            NormalizedUserName = TestConstants.Email.ToUpper(),
            EmailConfirmed = true,
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            SecurityStamp = Guid.NewGuid().ToString(),
        };

        var passwordHasher = new PasswordHasher<AppIdentityUser>();
        devUser.PasswordHash = passwordHasher.HashPassword(devUser, TestConstants.Password);

        context.Set<AppIdentityUser>().Add(devUser);
        context.SaveChanges();
    }

    private static DbContextOptionsBuilder AddSeeding(
        this DbContextOptionsBuilder builder,
        IWebHostEnvironment environment
    )
    {
        builder.UseSeeding(
            (context, _) =>
            {
                if (
                    environment.EnvironmentName
                    is EnvironmentConstants.Development
                        or EnvironmentConstants.Test
                )
                {
                    SeedDevelopmentUser(context);
                }
            }
        );

        return builder;
    }

    private static void ConfigureDatabase(
        IServiceCollection services,
        IWebHostEnvironment environment,
        IConfiguration configuration
    )
    {
        if (environment.EnvironmentName == EnvironmentConstants.Test)
        {
            services.AddDbContext<AppDbContext>(builder =>
                builder.UseInMemoryDatabase("Database:Test").AddSeeding(environment)
            );
            return;
        }

        var connectionString = environment.EnvironmentName switch
        {
            EnvironmentConstants.Development => configuration["Database:Development"],
            EnvironmentConstants.Production => throw new NotImplementedException(
                "Need Production Database String"
            ),
            _ => throw new ArgumentException("Invalid Environment"),
        };

        if (connectionString == null)
        {
            throw new ArgumentException("Database connection string must be provided");
        }

        services.AddDbContext<AppDbContext>(builder =>
        {
            builder.UseNpgsql(connectionString).AddSeeding(environment);
        });
    }
}
