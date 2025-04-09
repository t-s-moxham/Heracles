using System.Text.Json.Serialization;
using Heracles.Infrastructure;
using Heracles.Web.auth;
using Heracles.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Heracles.Web;

// ReSharper disable once ClassNeverInstantiated.Global
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddInfrastructureServices(builder.Environment, builder.Configuration);

        // Add auth services

        builder
            .Services.AddAuthorization()
            .AddIdentityApiEndpoints<AppIdentityUser>()
            .AddEntityFrameworkStores<AppDbContext>();

        builder.Services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 14;
        });

        // Add auth handler

        builder.Services.AddScoped<IAuthorizationHandler, ExerciseCrudAuthorizationHandler>();

        // TODO add to its own config file
        builder.Services.AddScoped<ExerciseService>();

        builder
            .Services.AddControllers()
            .AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())
            );

        builder.Services.AddOpenApi();

        var app = builder.Build();

        app.MapGroup("auth").MapIdentityApi<AppIdentityUser>();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        if (app.Environment.IsProduction())
        {
            app.UseHttpsRedirection();
        }

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
