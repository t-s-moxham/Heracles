using System.Net.Http.Headers;
using Heracles.Infrastructure;
using Heracles.Web;
using Microsoft.EntityFrameworkCore;

namespace Heracles.IntegrationTests;

// ReSharper disable InconsistentNaming

public class TestEnvironment : IAsyncLifetime
{
    public readonly HttpClient AnonymousClient;
    public readonly HttpClient AuthenticatedClient;

    private record LoginRequest(string email, string password);

    private record LoginResponse(string accessToken);

    private readonly CustomWebApplicationFactory<Program> Factory = new();

    private async Task<string> GetBearerToken()
    {
        var response = await AnonymousClient.PostAsJsonAsync(
            "/auth/login",
            new LoginRequest(TestConstants.Email, TestConstants.Password)
        );

        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<LoginResponse>();

        return body!.accessToken;
    }

    public TestEnvironment()
    {
        AnonymousClient = Factory.CreateClient();
        AuthenticatedClient = Factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        AuthenticatedClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            await GetBearerToken()
        );
    }

    public async Task TruncateTablesAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var tableNames = dbContext
            .Model.GetEntityTypes()
            .Select(t => t.GetTableName())
            .Distinct()
            .ToList();

        var commands = tableNames.Select(tableName =>
            dbContext.Database.ExecuteSqlAsync(
                $"SET FOREIGN_KEY_CHECKS = 0; TRUNCATE TABLE `{tableName}`;"
            )
        );

        await Task.WhenAll(commands);
    }

    public Task DisposeAsync()
    {
        Factory.Dispose();
        return Task.CompletedTask;
    }
}
