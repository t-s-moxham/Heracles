using System.Net;
using Heracles.Web;
using Microsoft.AspNetCore.Http;

namespace Heracles.IntegrationTests;

public class BasicAuthentication(TestEnvironment test) : IClassFixture<TestEnvironment>
{
    [Theory]
    [InlineData("GET", "/exercises")]
    [InlineData("GET", "/exercises/123e4567-e89b-12d3-a456-426614174000")]
    [InlineData("POST", "/exercises")]
    [InlineData("PATCH", "/exercises/123e4567-e89b-12d3-a456-426614174000")]
    [InlineData("DELETE", "/exercises/123e4567-e89b-12d3-a456-426614174000")]
    public async Task RequiresAuthenticationOnAllEndpoints(string method, string endpoint)
    {
        var request = new HttpRequestMessage(new HttpMethod(method), endpoint);

        var response = await test.AnonymousClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
