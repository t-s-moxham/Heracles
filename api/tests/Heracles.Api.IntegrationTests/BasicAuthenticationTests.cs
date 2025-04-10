using System.Net;
using Heracles.Web;
using Microsoft.AspNetCore.Http;

namespace Heracles.IntegrationTests;

public class BasicAuthentication(TestEnvironment test) : IClassFixture<TestEnvironment>
{
    private const string ExerciseId = "123e4567-e89b-12d3-a456-426614174000";

    [Theory]
    [InlineData("GET", "/exercises")]
    [InlineData("GET", $"/exercises/{ExerciseId}")]
    [InlineData("POST", "/exercises")]
    [InlineData("PATCH", $"/exercises/{ExerciseId}")]
    [InlineData("DELETE", $"/exercises/{ExerciseId}")]
    public async Task Authorize_RequiresAuthenticationOnAllEndpoints(string method, string endpoint)
    {
        var request = new HttpRequestMessage(new HttpMethod(method), endpoint);

        var response = await test.AnonymousClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
