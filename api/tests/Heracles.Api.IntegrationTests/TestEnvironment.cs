using System.Net.Http.Headers;
using Bogus;
using Heracles.Infrastructure;
using Heracles.Web;
using Microsoft.EntityFrameworkCore;

// ReSharper disable PropertyCanBeMadeInitOnly.Local
// ReSharper disable InconsistentNaming

namespace Heracles.IntegrationTests;

public class TestEnvironment : IAsyncLifetime
{
    public readonly HttpClient AnonymousClient;
    public readonly HttpClient AuthenticatedClient;

    private class AuthRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    private record LoginResponse(string accessToken);

    private readonly CustomWebApplicationFactory<Program> Factory = new();

    public AppDbContext DbContext { get; private set; }

    public async Task<HttpClient> GenerateFakeUserClient()
    {
        var fakeUser = new Faker<AuthRequest>()
            .RuleFor(x => x.Email, f => f.Person.Email)
            .RuleFor(x => x.Password, f => f.Random.AlphaNumeric(14));

        var request = fakeUser.Generate();

        await AnonymousClient.PostAsJsonAsync("/auth/register", request);

        var client = Factory.CreateClient();

        await AuthenticateClient(client, request);

        return client;
    }

    private async Task<string> GetBearerToken(AuthRequest request)
    {
        var response = await AnonymousClient.PostAsJsonAsync("/auth/login", request);

        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<LoginResponse>();

        return body!.accessToken;
    }

    public TestEnvironment()
    {
        AnonymousClient = Factory.CreateClient();
        AuthenticatedClient = Factory.CreateClient();

        DbContext = Factory.Services.GetRequiredService<AppDbContext>();
    }

    private async Task AuthenticateClient(HttpClient client, AuthRequest request)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            await GetBearerToken(request)
        );
    }

    public async Task InitializeAsync()
    {
        await AuthenticateClient(
            AuthenticatedClient,
            new AuthRequest() { Email = TestConstants.Email, Password = TestConstants.Password }
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
